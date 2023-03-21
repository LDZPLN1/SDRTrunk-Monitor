Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Timers

Public Class PrimaryForm
    Private Shared sdrprocid As Integer = 0
    Private Shared ReadOnly sdrproc As New Process()

    Public pchecktimer As New Timer(60000)

    Private Sub PrimaryForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AddHandler sdrproc.OutputDataReceived, AddressOf ReadStandardOutput
        AddHandler pchecktimer.Elapsed, New ElapsedEventHandler(AddressOf TimerElapsed)

        If My.Settings.Watchdog < 5 Or My.Settings.Watchdog > 3600 Then My.Settings.Watchdog = 60

        Do Until File.Exists(My.Settings.SDRTPath & "\bin\sdr-trunk.bat")
            Dim result As DialogResult = MessageBox.Show("Unable to locate SDRTrunk. Update settings?", "SDRTrunk Not Found", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If result = DialogResult.Yes Then 
                SettingsForm.ShowDialog()
            Else
                End
            End If
        Loop

        Me.AutoRestartMenuItem.Checked = My.Settings.AutoRestart
        pchecktimer.Interval = My.Settings.Watchdog * 1000
        pchecktimer.Start()
        pchecktimer.Enabled = False
    End Sub

    Private Sub TrayMenu_Opening(sender As Object, e As CancelEventArgs) Handles TrayMenu.Opening
        Select Case SDRTState()
            Case 0
                Me.StartMenuItem.Enabled = True
                Me.StopMenuItem.Enabled = False
                Me.RestartMenuItem.Enabled = False
            Case 1
                Me.StartMenuItem.Enabled = False
                Me.StopMenuItem.Enabled = True
                Me.RestartMenuItem.Enabled = True
            Case 2
                Me.StartMenuItem.Enabled = False
                Me.StopMenuItem.Enabled = False
                Me.RestartMenuItem.Enabled = False
        End Select

        Me.ViewLogMenuItem.Enabled = Not LogWindow.Visible
    End Sub

    ' MENU ITEM TO START SDRTRUNK
    Private Sub StartSDRTrunkToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StartMenuItem.Click
        StartSDRT()
    End Sub

    ' MENU ITEM TO STOP SDRTRUNK
    Private Sub StopSDRTrunkToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StopMenuItem.Click
        StopSDRT()
    End Sub

    ' MENU ITEM TO RESTART SDRTRUNK
    Private Sub RestartSDRTrunkToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RestartMenuItem.Click
        StopSDRT()
        StartSDRT()
    End Sub

    ' MENU ITEM TO SHOW LOG WINDOW
    Private Sub ViewLogMenuItem_Click(sender As Object, e As EventArgs) Handles ViewLogMenuItem.Click
        LogWindow.Show()
        LogWindow.Focus()
    End Sub

    ' MENU ITEM TO SHOW SETTINGS WINDOW
    Private Sub SettingsMenuItem_Click(sender As Object, e As EventArgs) Handles SettingsMenuItem.Click
        SettingsForm.Show()
    End Sub

    ' MENU ITEM TO TOGGLE AUTO RESTART
    Private Sub AutoRestartMenuItem_Click(sender As Object, e As EventArgs) Handles AutoRestartMenuItem.CheckedChanged
        My.Settings.AutoRestart = AutoRestartMenuItem.Checked
        My.Settings.Save()
    End Sub

    ' MENU ITEM TO EXIT APPLICATION
    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitMenuItem.Click
        Me.TrayNotifyIcon.Visible = False
        Me.Close()
    End Sub

    ' START SDRTRUNK IN A NEW PROCESS AND REDIRECT STANDARD OUTPUT TO ASYNC OUTPUT HANDLER
    Private Sub StartSDRT()
        If Me.InvokeRequired Then
            Me.Invoke(Sub() StartSDRT())
        Else
            If sdrprocid <> 0 Then
                sdrproc.CancelOutputRead()
                sdrproc.Close()
            End If

            sdrproc.StartInfo.UseShellExecute = False
            sdrproc.StartInfo.RedirectStandardOutput = True
            sdrproc.StartInfo.FileName = My.Settings.SDRTPath & "\bin\java.exe"

            If My.Settings.SDRTVersion = "0.5.x" Then
                sdrproc.StartInfo.Arguments = "--add-exports=javafx.base/com.sun.javafx.event=ALL-UNNAMED --add-modules=jdk.incubator.vector --add-exports=java.desktop/com.sun.java.swing.plaf.windows=ALL-UNNAMED -classpath """ & My.Settings.SDRTPath & "\lib\*"" io.github.dsheirer.gui.SDRTrunk"
            ElseIf My.Settings.SDRTVersion = "0.6.x" Then
                sdrproc.StartInfo.Arguments = "--add-exports=javafx.base/com.sun.javafx.Event=ALL-UNNAMED --add-modules=jdk.incubator.vector --add-exports=java.desktop/com.sun.java.swing.plaf.windows=ALL-UNNAMED -classpath """ & My.Settings.SDRTPath & "\Lib\*"" --enable-preview --enable-native-access=ALL-UNNAMED ""-Djava.library.path=C:\Program Files\SDRplay\API\x64"" io.github.dsheirer.gui.SDRTrunk"
            End If

            sdrproc.Start()
            sdrprocid = sdrproc.Id
            sdrproc.BeginOutputReadLine()
            LogWindow.Show()
            Application.DoEvents()

            ' MINIMIZE INITIAL JAVA WINDOW
            Dim sprocrun As Boolean = False
            Dim proclist As Process()
            Dim attempts As Integer = 0

            Do Until sprocrun = True
                Threading.Thread.Sleep(50)
                attempts += 1
                proclist = Process.GetProcesses

                For Each sproc As Process In proclist
                    If sproc.MainWindowTitle = My.Settings.SDRTPath & "\bin\java.exe" Then
                        SetWindow(sproc.MainWindowHandle, 2)
                        sprocrun = True
                        Exit For
                    End If
                Next

                If attempts = 40 Then sprocrun = True
            Loop

            LogWindow.Focus()

            ' ENABLE WATCHDOG TIMER
            pchecktimer.Enabled = True
        End If
    End Sub

    ' STOP SDRTRUNK AND DISABLE WATCHDOG TIMER
    Private Sub StopSDRT()
        If Me.InvokeRequired Then
            Me.Invoke(Sub() StopSDRT())
        Else
            If sdrprocid <> 0 Then
                pchecktimer.Enabled = False
                sdrproc.CancelOutputRead()
                sdrproc.Kill()
                sdrproc.WaitForExit()
                sdrproc.Close()
                sdrprocid = 0
                LogWindow.Hide()
                Application.DoEvents()
            End If
        End If
    End Sub

    ' ASYNC OUTPUT HANDLER FOR SDRTRUNK COMMAND WINDOW - MONITOR FOR ERRORS
    Private Sub ReadStandardOutput(sender As Object, args As DataReceivedEventArgs)
        If args.Data IsNot Nothing Then
            Me.Invoke(Sub() UpdateLog(args.Data))

            If args.Data.Contains("Couldn't design final output low pass filter") Or args.Data.Contains("org.usb4java.LibUsbException") Or args.Data.Contains("java.lang.IllegalArgumentException") Or args.Data.Contains("throwing away samples") Then
                If Me.AutoRestartMenuItem.CheckState = CheckState.Checked Then
                    TrayNotifyIcon.BalloonTipText = "SDRTRunk Process Appears to Have Failed. Restarting"
                    TrayNotifyIcon.ShowBalloonTip(1)
                    StopSDRT()
                    StartSDRT()
                Else
                    TrayNotifyIcon.BalloonTipText = "SDRTRunk Process Appears to Have Failed"
                    TrayNotifyIcon.ShowBalloonTip(1)
                End If
            End If
        End If
    End Sub

    ' UPDATE LOG WINDOW
    Public Shared Sub UpdateLog(ltext)
        LogWindow.LogTextBox.AppendText(ltext & System.Environment.NewLine)
        LogWindow.Refresh()
    End Sub

    ' CHECK PROCESS STATUS WHEN WATCHDOG TIMER FIRES
    Private Sub TimerElapsed(ByVal sender As Object, ByVal e As ElapsedEventArgs)
        If SDRTState() = 0 Then
            If Me.AutoRestartMenuItem.CheckState = CheckState.Checked Then
                TrayNotifyIcon.BalloonTipText = "SDRTRunk Process has Exited. Restarting"
                TrayNotifyIcon.ShowBalloonTip(1)
                StopSDRT()
                StartSDRT()
            Else
                TrayNotifyIcon.BalloonTipText = "SDRTRunk Process has Exited"
                TrayNotifyIcon.ShowBalloonTip(1)
                pchecktimer.Enabled = False
                sdrproc.CancelOutputRead()
                sdrproc.Close()
                sdrprocid = 0
                Me.Invoke(Sub() HideLog())
            End If
        End If
    End Sub

    Private Shared Sub HideLog()
        LogWindow.Hide()
    End Sub

    <DllImport("User32.dll", EntryPoint:="ShowWindow", SetLastError:=True)>
    Private Shared Function SetWindow(ByVal hWnd As IntPtr, nShowCmd As Integer) As Boolean
    End Function

    ' RETURN SDRTRUNK PROCESS STATE
    '   0 = NOT RUNNING
    '   1 = STARTED BY THIS APP
    '   2 = RUNNING, BUT NOT STARTED BY THIS APP

    Private Function SDRTState()
        SDRTState = 0
        Dim proclist As Process() = Process.GetProcessesByName("java")

        For Each sproc As Process In proclist
            If sproc.MainModule.FileName = My.Settings.SDRTPath & "\bin\java.exe" Then
                If sproc.Id = sdrprocid Then
                    SDRTState = 1
                Else
                    SDRTState = 2
                End If

                Exit For
            End If
        Next
    End Function

End Class
