Imports System.Runtime.InteropServices
Imports System.Timers

Public Class TrayForm
    ' PATH TO SDRTRUNK
    Private Shared ReadOnly sdrt_path As String = "C:\SDR\SDRTrunk"

    ' WATCHDOG TIMER IN ms
    Private Shared ReadOnly pchecktimer As New Timer(60000)

    ' STARTUP ARGUMENTS - UNCOMMENT BASED ON SDRTRUNK VERSION

    '0.5.x
    Private Shared ReadOnly SDRTArgs As String = "--add-exports=javafx.base/com.sun.javafx.event=ALL-UNNAMED --add-modules=jdk.incubator.vector --add-exports=java.desktop/com.sun.java.swing.plaf.windows=ALL-UNNAMED -classpath " & sdrt_path & "\lib\* io.github.dsheirer.gui.SDRTrunk"

    '0.6.x
    'Private Shared ReadOnly SDRTArgs As String = "--add-exports=javafx.base/com.sun.javafx.event=ALL-UNNAMED --add-modules=jdk.incubator.vector --add-exports=java.desktop/com.sun.java.swing.plaf.windows=ALL-UNNAMED -classpath " & sdrt_path & "\lib\* io.github.dsheirer.gui.SDRTrunk --enable-preview --enable-native-access=ALL-UNNAMED ""-Djava.library.path=C:\Program Files\SDRplay\API\x64"""

    Private Shared ReadOnly sdrproc As New Process()
    Private Shared sdrprocid As Integer = 0

    ' SETUP WATCHDOG TIMER AND START IT
    Public Sub Startup()
        AddHandler sdrproc.OutputDataReceived, AddressOf ReadStandardOutput
        AddHandler pchecktimer.Elapsed, New ElapsedEventHandler(AddressOf TimerElapsed)
        pchecktimer.Start()
        pchecktimer.Enabled = False
    End Sub

    ' SHOW TRAY MENU WHEN TRAY ICON IS RIGHT-CLICKED
    Private Sub TrayForm_Activated(sender As Object, e As EventArgs) Handles MyBase.Activated
        ' SET MENU ITEM STATES BASES ON SDRTRUNK PROCESS STATE
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

        If LogWindow.Visible Then
            Me.ViewLogMenuItem.Enabled = False
        Else
            Me.ViewLogMenuItem.Enabled = True
        End If

        TrayMenu.Show(Cursor.Position)
        Me.Left = TrayMenu.Left + 1
        Me.Top = TrayMenu.Top + 1
    End Sub

    ' HIDE TRAY MENU
    Private Sub TrayForm_Deactivate(sender As Object, e As EventArgs) Handles MyBase.Deactivate
        Me.Hide()
    End Sub

    ' MENU ITEM TO START SDRTRUNK
    Private Sub StartSDRTrunkToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StartMenuItem.Click
        Start_SDRT()
    End Sub

    ' MENU ITEM TO STOP SDRTRUNK
    Private Sub StopSDRTrunkToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StopMenuItem.Click
        Stop_SDRT()
    End Sub

    ' MENU ITEM TO RESTART SDRTRUNK
    Private Sub RestartSDRTrunkToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RestartMenuItem.Click
        Stop_SDRT()
        Start_SDRT()
    End Sub

    ' MENU ITEM TO SHOW LOG WINDOW
    Private Sub ViewLogMenuItem_Click(sender As Object, e As EventArgs) Handles ViewLogMenuItem.Click
        LogWindow.Show()
        LogWindow.Focus()
        Application.DoEvents()
    End Sub

    ' MENU ITEM TO EXIT PROGRAM
    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitMenuItem.Click
        MainForm.TrayIcon.Visible = False
        End
    End Sub

    ' START SDRTRUNK IN A NEW PROCESS AND REDIRECT STANDARD OUTPUT TO ASYNC OUTPUT HANDLER
    Private Sub Start_SDRT()
        If sdrprocid <> 0 Then
            sdrproc.CancelOutputRead()
            sdrproc.Close()
        End If

        sdrproc.StartInfo.UseShellExecute = False
        sdrproc.StartInfo.RedirectStandardOutput = True
        sdrproc.StartInfo.FileName = sdrt_path & "\bin\java.exe"
        sdrproc.StartInfo.Arguments = SDRTArgs
        sdrproc.Start()
        sdrprocid = sdrproc.Id
        sdrproc.BeginOutputReadLine()

        LogWindow.Show()

        ' ENABLE WATCHDOG TIMER
        pchecktimer.Enabled = True

        ' MINIMIZE INITIAL JAVA WINDOW
        Dim sprocrun As Boolean = False
        Dim proclist As Process()

        Do Until sprocrun = True
            Threading.Thread.Sleep(50)
            proclist = Process.GetProcesses

            For Each sproc As Process In proclist
                If sproc.MainWindowTitle = sdrt_path & "\bin\java.exe" Then
                    SetWindow(sproc.MainWindowHandle, 2)
                    sprocrun = True
                    Exit For
                End If
            Next

            Application.DoEvents()
        Loop

        LogWindow.Focus()
    End Sub

    <DllImport("User32.dll", EntryPoint:="ShowWindow", SetLastError:=True)>
    Private Shared Function SetWindow(ByVal hWnd As IntPtr, nShowCmd As Integer) As Boolean
    End Function

    ' STOP SDRTRUNK AND DISABLE WATCHDOG TIMER
    Private Shared Sub Stop_SDRT()
        sdrproc.Kill()
        sdrproc.CancelOutputRead()
        sdrproc.Close()
        sdrprocid = 0
        pchecktimer.Enabled = False
        LogWindow.Hide()
    End Sub

    ' ASYNC OUTPUT HANDLER FOR SDRTRUNK COMMAND WINDOW - MONITOR FOR ERRORS
    Private Sub ReadStandardOutput(sender As Object, args As DataReceivedEventArgs)
        If args.Data IsNot Nothing Then
            UpdateLog(args.Data)

            If args.Data.Contains("Couldn't design final output low pass filter") Or args.Data.Contains("org.usb4java.LibUsbException") Or args.Data.Contains("java.lang.IllegalArgumentException") Then
                If Me.AutoRestartMenuItem.CheckState = CheckState.Checked Then
                    Stop_SDRT()
                    Start_SDRT()
                Else
                    MsgBox("SDRTrunk appears to have failed" & System.Environment.NewLine & System.Environment.NewLine & args.Data, vbExclamation, "SDRTrunk Error")
                    pchecktimer.Enabled = False
                End If
            End If
        End If
    End Sub

    ' UPDATE LOG WINDOW
    Public Sub UpdateLog(ltext)
        If Me.InvokeRequired Then
            Me.Invoke(Sub() UpdateLog(ltext))
        Else
            LogWindow.LogTextBox.AppendText(ltext & System.Environment.NewLine)
        End If
    End Sub

    ' CHECK PROCESS STATUS WHEN WATCHDOG TIMER FIRES
    Private Sub TimerElapsed(ByVal sender As Object, ByVal e As ElapsedEventArgs)
        If SDRTState() = 0 Then
            If Me.AutoRestartMenuItem.CheckState = CheckState.Checked Then
                Stop_SDRT()
                Start_SDRT()
            Else
                sdrproc.CancelOutputRead()
                sdrproc.Close()
                sdrprocid = 0
                pchecktimer.Enabled = False
                LogWindow.Hide()
                MsgBox("SDRTrunk process appears to have exited", vbExclamation, "SDRTrunk Not Running")
            End If
        End If
    End Sub

    ' RETURN SDRTRUNK PROCESS STATE
    '   0 = NOT RUNNING
    '   1 = STARTED BY THIS APP
    '   2 = RUNNING, BUT NOT STARTED BY THIS APP

    Private Shared Function SDRTState()
        SDRTState = 0
        Dim proclist As Process() = Process.GetProcessesByName("java")

        For Each sproc As Process In proclist
            If sproc.MainModule.FileName = sdrt_path & "\bin\java.exe" Then
                If sproc.Id = sdrprocid Then
                    SDRTState = 1
                Else
                    SDRTState = 2
                End If
            End If
        Next
    End Function
End Class