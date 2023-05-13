Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Timers

Public Class PrimaryForm
    Private oMutex As Mutex
    Private Shared sdrprocid As Integer = 0
    Private Shared ReadOnly sdrproc As New Process()
    Private Shared ignorefuture As Boolean = False
    Private Shared ActiveAppPath As String = String.Empty
    Private Shared ActiveJavaProcess As Process
    Private Const GWL_STYLE As Long = (-16&)
    Private Const WS_VISIBLE As Long = &H10000000

    Public pchecktimer As New Timers.Timer(60000)

    ' VALIDATE SETTINGS AND START WATCHDOG TIMER
    Private Sub PrimaryForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        oMutex = New Mutex(False, "SDRTrunk Monitor")

        If oMutex.WaitOne(0, False) <> False Then
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

            ActiveAppPath = My.Settings.SDRTPath
            AutoRestartMenuItem.Checked = My.Settings.AutoRestart

            If My.Settings.ExternalCommand <> String.Empty Then
                RunExternalMenuItem.Checked = My.Settings.RunExternal
            Else
                RunExternalMenuItem.Enabled = False
            End If

            pchecktimer.Interval = My.Settings.Watchdog * 1000
            pchecktimer.Start()
            pchecktimer.Enabled = False
        Else
            oMutex.Close()
            oMutex = Nothing
            End
        End If
    End Sub

    ' ENABLE/DISABLE MENU ITEMS BASED ON PROCESS STATE
    Private Sub TrayMenu_Opening(sender As Object, e As CancelEventArgs) Handles TrayMenu.Opening
        Select Case SDRTState()
            Case 0
                StartMenuItem.Enabled = True
                StopMenuItem.Enabled = False
                RestartMenuItem.Enabled = False
            Case 1
                StartMenuItem.Enabled = False
                StopMenuItem.Enabled = True
                RestartMenuItem.Enabled = True
            Case 2
                StartMenuItem.Enabled = False
                StopMenuItem.Enabled = False
                RestartMenuItem.Enabled = False
        End Select

        ViewLogMenuItem.Enabled = Not LogWindow.Visible
    End Sub

    ' MENU ITEM TO START SDRTRUNK
    Private Sub StartSDRTrunkToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StartMenuItem.Click
        UpdateLog(Environment.NewLine & "USER INITIATED START" & Environment.NewLine, 1)
        StartSDRT()
    End Sub

    ' MENU ITEM TO STOP SDRTRUNK
    Private Sub StopSDRTrunkToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StopMenuItem.Click
        UpdateLog(Environment.NewLine & "USER INITIATED STOP" & Environment.NewLine, 1)
        StopSDRT()
    End Sub

    ' MENU ITEM TO RESTART SDRTRUNK
    Private Sub RestartSDRTrunkToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RestartMenuItem.Click
        UpdateLog(Environment.NewLine & "USER INITIATED RESTART" & Environment.NewLine, 1)
        StopSDRT()

        If RunExternalMenuItem.Checked = True Then
            ExecuteExternal()
        End If

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

    ' MENU ITEM TO TOGGLE EXTERNAL COMMAND BETWEEN RESTARTS
    Private Sub RunExternalMenuItem_Click(sender As Object, e As EventArgs) Handles RunExternalMenuItem.CheckedChanged
        My.Settings.RunExternal = RunExternalMenuItem.Checked
        My.Settings.Save()
    End Sub

    ' MENU ITEM TO SHOW ABOUT FORM
    Private Sub AboutMenuItem_Click(sender As Object, e As EventArgs) Handles AboutMenuItem.Click
        About.Show()
    End Sub

    ' MENU ITEM TO EXIT APPLICATION
    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitMenuItem.Click
        TrayNotifyIcon.Visible = False
        Close()
    End Sub

    ' START SDRTRUNK IN A NEW PROCESS AND REDIRECT STANDARD OUTPUT TO ASYNC OUTPUT HANDLER
    Private Sub StartSDRT()
        If InvokeRequired Then
            Invoke(Sub() StartSDRT())
        Else
            If sdrprocid <> 0 Then
                sdrproc.CancelOutputRead()
                sdrproc.Close()
            End If

            ActiveAppPath = My.Settings.SDRTPath
            sdrproc.StartInfo.UseShellExecute = False
            sdrproc.StartInfo.RedirectStandardOutput = True
            sdrproc.StartInfo.FileName = ActiveAppPath & "\bin\java.exe"

            Select Case My.Settings.SDRTVersion
                Case "0.5.x"
                    sdrproc.StartInfo.Arguments = "--add-exports=javafx.base/com.sun.javafx.event=ALL-UNNAMED --add-modules=jdk.incubator.vector --add-exports=java.desktop/com.sun.java.swing.plaf.windows=ALL-UNNAMED -classpath """ & My.Settings.SDRTPath & "\lib\*"" io.github.dsheirer.gui.SDRTrunk"
                Case "0.6.x"
                    sdrproc.StartInfo.Arguments = "--add-exports=javafx.base/com.sun.javafx.event=ALL-UNNAMED --add-exports=java.desktop/com.sun.java.swing.plaf.windows=ALL-UNNAMED --add-modules=jdk.incubator.vector --enable-preview --enable-native-access=ALL-UNNAMED ""-Djava.library.path=c:\Program Files\SDRplay\API\x64"" -classpath """ & My.Settings.SDRTPath & "\lib\*"" io.github.dsheirer.gui.SDRTrunk"
            End Select

            sdrproc.Start()
            sdrprocid = sdrproc.Id
            sdrproc.BeginOutputReadLine()

            ' WAIT FOR JAVA WINDOW
            Dim proclist As Process()
            Dim procfound As Boolean = False
            Dim runchecks As Integer = 0

            Do Until procfound Or (runchecks = 100)
                Thread.Sleep(50)
                Application.DoEvents()
                proclist = Process.GetProcesses

                For Each sproc As Process In proclist
                    If sproc.MainWindowTitle = ActiveAppPath & "\bin\java.exe" Then
                        ActiveJavaProcess = sproc
                        SetWindow(sproc.MainWindowHandle, 2)
                        procfound = True
                        Exit For
                    End If
                Next

                If Not procfound Then
                    runchecks += 1
                End If
            Loop

            LogWindow.Show()

            If runchecks = 100 Then
                UpdateLog(Environment.NewLine & "SDRTRUNK FAILED TO START" & Environment.NewLine, 3)
                sdrproc.CancelOutputRead()
                sdrproc.Close()
                sdrprocid = 0
                ActiveAppPath = String.Empty
            Else
                ' ENABLE WATCHDOG TIMER
                ignorefuture = False
                pchecktimer.Enabled = True
                TrayNotifyIcon.Text = "SDRTrunk Monitor" & Environment.NewLine & "Monitoring PID " & sdrprocid.ToString()
            End If
        End If
    End Sub

    ' STOP SDRTRUNK AND DISABLE WATCHDOG TIMER
    Private Sub StopSDRT()
        If InvokeRequired Then
            Invoke(Sub() StopSDRT())
        Else
            If sdrprocid <> 0 Then
                pchecktimer.Enabled = False
                sdrproc.CancelOutputRead()
                sdrproc.CloseMainWindow()

                Dim attempts As Integer = 0

                Do Until sdrproc.HasExited
                    attempts += 1
                    Thread.Sleep(50)
                    Application.DoEvents()

                    If attempts = 100 Then
                        sdrproc.Kill()
                    End If
                Loop

                sdrproc.Close()
                ActiveJavaProcess = Nothing
                sdrprocid = 0
                LogWindow.Hide()
                TrayNotifyIcon.Text = "SDRTrunk Monitor"
            End If

            ActiveAppPath = String.Empty
        End If
    End Sub

    ' ASYNC OUTPUT HANDLER FOR SDRTRUNK COMMAND WINDOW - MONITOR FOR ERRORS
    Private Sub ReadStandardOutput(sender As Object, args As DataReceivedEventArgs)
        If args.Data IsNot Nothing Then
            If args.Data.Contains("Couldn't design final output low pass filter") Or args.Data.Contains("org.usb4java.LibUsbException") Or args.Data.Contains("java.lang.IllegalArgumentException") Or args.Data.Contains("throwing away samples") Then
                UpdateLog(args.Data, 2)

                If AutoRestartMenuItem.CheckState = CheckState.Checked Then
                    UpdateLog(Environment.NewLine & "AUTO RESTART INITIATED" & Environment.NewLine, 3)
                    StopSDRT()

                    If RunExternalMenuItem.Checked = True Then
                        ExecuteExternal()
                    End If

                    StartSDRT()
                Else
                    If Not ignorefuture Then
                        TrayNotifyIcon.BalloonTipText = "SDRTRunk Process Appears to Have Failed"
                        TrayNotifyIcon.ShowBalloonTip(1)
                        ignorefuture = True
                    End If
                End If
            ElseIf args.Data.Contains("starting main application gui") Then
                UpdateLog(args.Data, 0)
                SetWindow(ActiveJavaProcess.MainWindowHandle, 2)
                Invoke(Sub() HideLog())
            Else
                UpdateLog(args.Data, 0)
            End If
        End If
    End Sub

    ' UPDATE LOG WINDOW
    Public Sub UpdateLog(ltext As String, highlight As Integer)
        If InvokeRequired Then
            Invoke(Sub() UpdateLog(ltext, highlight))
        Else
            Dim ltextfcolor As New ColorDialog()
            Dim ltextbcolor As New ColorDialog()

            If highlight > 0 Then
                ltextfcolor.Color = LogWindow.LogTextBox.SelectionColor
                ltextbcolor.Color = LogWindow.LogTextBox.SelectionBackColor

                Select Case highlight
                    Case 1
                        LogWindow.LogTextBox.SelectionColor = Color.White
                        LogWindow.LogTextBox.SelectionBackColor = Color.DarkGreen
                    Case 2
                        LogWindow.LogTextBox.SelectionBackColor = Color.Orange
                    Case 3
                        LogWindow.LogTextBox.SelectionColor = Color.White
                        LogWindow.LogTextBox.SelectionBackColor = Color.DarkRed
                End Select
            End If

            LogWindow.LogTextBox.AppendText(ltext & Environment.NewLine)

            If highlight > 0 Then
                LogWindow.LogTextBox.SelectionColor = ltextfcolor.Color
                LogWindow.LogTextBox.SelectionBackColor = ltextbcolor.Color
            End If

            LogWindow.LogTextBox.SelectionStart = LogWindow.LogTextBox.Text.Length
            LogWindow.LogTextBox.ScrollToCaret()
        End If
    End Sub

    ' CHECK PROCESS STATUS WHEN WATCHDOG TIMER FIRES
    Private Sub TimerElapsed(ByVal sender As Object, ByVal e As ElapsedEventArgs)
        If SDRTState() = 0 Then
            If AutoRestartMenuItem.CheckState = CheckState.Checked Then
                UpdateLog(Environment.NewLine & "AUTO RESTART INITIATED" & Environment.NewLine, 3)

                StopSDRT()

                If RunExternalMenuItem.Checked = True Then
                    ExecuteExternal()
                End If

                StartSDRT()
            Else
                TrayNotifyIcon.BalloonTipText = "SDRTRunk Process has Exited"
                TrayNotifyIcon.ShowBalloonTip(1)
                pchecktimer.Enabled = False
                sdrproc.CancelOutputRead()
                sdrproc.Close()
                ActiveJavaProcess = Nothing
                sdrprocid = 0
                Invoke(Sub() HideLog())
            End If
        End If
    End Sub

    Private Shared Sub HideLog()
        LogWindow.Hide()
        LogWindow.TopMost = False
    End Sub

    Private Sub ExecuteExternal()
        UpdateLog("EXECUTING EXTERNAL COMMAND [" & My.Settings.ExternalCommand & "]" & Environment.NewLine, 1)

        Dim extproc As New Process()
        Dim splitloc = InStr(My.Settings.ExternalCommand, " ")
        extproc.StartInfo.UseShellExecute = True
        extproc.StartInfo.RedirectStandardOutput = False

        If splitloc = 0 Then
            extproc.StartInfo.FileName = My.Settings.ExternalCommand
        Else
            extproc.StartInfo.FileName = My.Settings.ExternalCommand.Substring(0, splitloc - 1)
            extproc.StartInfo.Arguments = My.Settings.ExternalCommand.Substring(splitloc)
        End If

        Try
            extproc.Start()

            Do Until extproc.HasExited
                Thread.Sleep(50)
                Application.DoEvents()
            Loop
        Catch ex As Win32Exception
            UpdateLog("EXTERNAL COMMAND FAILED TO EXECUTE" & Environment.NewLine, 3)
        End Try

        extproc.Close()
    End Sub

    <DllImport("User32.dll", EntryPoint:="ShowWindow", SetLastError:=True)>
    Private Shared Function SetWindow(hWnd As IntPtr, nShowCmd As Integer) As Boolean
    End Function

    ' RETURN SDRTRUNK PROCESS STATE
    '   0 = NOT RUNNING
    '   1 = STARTED BY THIS APP
    '   2 = RUNNING, BUT NOT STARTED BY THIS APP

    Private Shared Function SDRTState()
        SDRTState = 0
        Dim proclist As Process() = Process.GetProcessesByName("java")

        For Each sproc As Process In proclist
            If sproc.MainModule.FileName = ActiveAppPath & "\bin\java.exe" Then
                SDRTState = If(sproc.Id = sdrprocid, 1, 2)
                Exit For
            End If
        Next
    End Function

End Class
