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
    Private Shared MaxLogLength As Integer = 500

    Public pchecktimer As New Timers.Timer(60000)
    Public extruntimer As New Timers.Timer(60000)

    Public RestartErrors As New List(Of String)({
        "Couldn't design final output low pass filter",
        "org.usb4java.LibUsbException",
        "java.lang.IllegalArgumentException",
        "throwing away samples",
        "Maximum USB transfer buffer errors reached - transfer buffers exhausted - shutting down USB tuner"
    })

    ' VALIDATE SETTINGS AND START WATCHDOG TIMER
    Private Sub PrimaryForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        oMutex = New Mutex(False, "SDRTrunk Monitor")

        If oMutex.WaitOne(0, False) <> False Then
            AddHandler sdrproc.OutputDataReceived, AddressOf ReadStandardOutput
            AddHandler pchecktimer.Elapsed, New ElapsedEventHandler(AddressOf WatchdogTimerElapsed)
            AddHandler extruntimer.Elapsed, New ElapsedEventHandler(AddressOf ExternalTimerElapsed)

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

            If My.Settings.TimedExternalCommand <> String.Empty Then
                RunTimedExternalMenuItem.Checked = My.Settings.RunTimedExternal
            Else
                RunTimedExternalMenuItem.Enabled = False
            End If

            pchecktimer.Interval = My.Settings.Watchdog * 1000
            pchecktimer.Start()
            pchecktimer.Enabled = False

            extruntimer.Interval = My.Settings.ExternalCommandTimer * 1000
            extruntimer.Start()
            extruntimer.Enabled = False

            If My.Settings.RunAtStartup = True And SDRTState() = 0 Then
                UpdateLog(FormattedTime() & " AUTO RUN AT STARTUP INITIATED START")
                StartSDRT()
            End If
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
        UpdateLog(FormattedTime() & " USER INITIATED START")
        StartSDRT()
    End Sub

    ' MENU ITEM TO STOP SDRTRUNK
    Private Sub StopSDRTrunkToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StopMenuItem.Click
        UpdateLog(FormattedTime() & " USER INITIATED STOP")
        StopSDRT()
    End Sub

    ' MENU ITEM TO RESTART SDRTRUNK
    Private Sub RestartSDRTrunkToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RestartMenuItem.Click
        UpdateLog(FormattedTime() & " USER INITIATED RESTART")
        StopSDRT()

        If RunExternalMenuItem.Checked = True Then
            ExecuteExternal(My.Settings.ExternalCommand, True, False)
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

    ' MENU ITEM TO TOGGLE EXTERNAL TIMED COMMAND
    Private Sub RunTimedExternalMenuItem_Click(sender As Object, e As EventArgs) Handles RunTimedExternalMenuItem.CheckedChanged
        My.Settings.RunTimedExternal = RunTimedExternalMenuItem.Checked
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
            LogWindow.TopMost = True
            LogWindow.TopMost = False

            If runchecks = 100 Then
                UpdateLog(FormattedTime() & " SDRTRUNK FAILED TO START")
                sdrproc.CancelOutputRead()
                sdrproc.Close()
                sdrprocid = 0
                ActiveAppPath = String.Empty
            Else
                ' ENABLE WATCHDOG TIMER
                ignorefuture = False
                pchecktimer.Enabled = True
                extruntimer.Enabled = True
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
                extruntimer.Enabled = False
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
            Dim RestartNeeded As Boolean = False

            For Each ErrorMessage As String In RestartErrors
                If args.Data.Contains(ErrorMessage) Then
                    RestartNeeded = True
                End If
            Next

            If RestartNeeded = True Then
                UpdateLog(args.Data)

                If AutoRestartMenuItem.CheckState = CheckState.Checked Then
                    UpdateLog(FormattedTime() & " AUTO RESTART INITIATED")
                    StopSDRT()

                    If RunExternalMenuItem.Checked = True Then
                        ExecuteExternal(My.Settings.ExternalCommand, True, False)
                    End If

                    StartSDRT()
                Else
                    If Not ignorefuture Then
                        UpdateLog(FormattedTime() & " SDRTRUNK PROCESS FAILED")
                        TrayNotifyIcon.BalloonTipText = "SDRTRunk Process Appears to Have Failed"
                        TrayNotifyIcon.ShowBalloonTip(1)
                        ignorefuture = True
                    End If
                End If
            ElseIf args.Data.Contains("starting main application gui") Then
                UpdateLog(args.Data)
                SetWindow(ActiveJavaProcess.MainWindowHandle, 2)
                Invoke(Sub() HideLog())
            Else
                UpdateLog(args.Data)
            End If
        End If
    End Sub

    ' UPDATE LOG WINDOW
    Public Sub UpdateLog(ltext As String)
        If InvokeRequired Then
            Invoke(Sub() UpdateLog(ltext))
        Else
            LogWindow.LogTextBox.AppendText(ltext & Environment.NewLine)

            If LogWindow.LogTextBox.Lines.Length - 1 > MaxLogLength Then
                Dim LogArray As List(Of String) = LogWindow.LogTextBox.Lines.ToList()
                LogArray.RemoveAt(0)
                LogWindow.LogTextBox.Lines = LogArray.ToArray()
                LogWindow.LogTextBox.Refresh()
            End If
        End If
    End Sub

    ' CHECK PROCESS STATUS WHEN WATCHDOG TIMER FIRES
    Private Sub WatchdogTimerElapsed(ByVal sender As Object, ByVal e As ElapsedEventArgs)
        If SDRTState() = 0 Then
            If AutoRestartMenuItem.CheckState = CheckState.Checked Then
                UpdateLog(FormattedTime() & " AUTO RESTART INITIATED")

                StopSDRT()

                If RunExternalMenuItem.Checked = True Then
                    ExecuteExternal(My.Settings.ExternalCommand, True, False)
                End If

                StartSDRT()
            Else
                UpdateLog(FormattedTime() & " SDRTRUNK PROCESS FAILED")
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

    ' RUN EXTERNAL COMMAND WHEN EXTERNAL TIMER FIRES
    Private Sub ExternalTimerElapsed(ByVal sender As Object, ByVal e As ElapsedEventArgs)
        If RunTimedExternalMenuItem.CheckState = CheckState.Checked Then
            ExecuteExternal(My.Settings.TimedExternalCommand, False, My.Settings.TimedExternalMinimized)
        End If
    End Sub

    ' HIDE LOG WINDOW
    Private Shared Sub HideLog()
        LogWindow.Hide()
        LogWindow.TopMost = False
    End Sub

    'RUN EXTERNAL COMMAND
    Private Sub ExecuteExternal(extcommand As String, syncwait As Boolean, minwindow As Boolean)
        UpdateLog(FormattedTime() & " EXECUTING EXTERNAL COMMAND [" & extcommand & "]")

        Dim extproc As New Process()
        Dim splitloc = InStr(extcommand, " ")
        extproc.StartInfo.UseShellExecute = True
        extproc.StartInfo.RedirectStandardOutput = False

        If splitloc = 0 Then
            extproc.StartInfo.FileName = extcommand
        Else
            extproc.StartInfo.FileName = extcommand.Substring(0, splitloc - 1)
            extproc.StartInfo.Arguments = extcommand.Substring(splitloc)
        End If

        If minwindow Then
            extproc.StartInfo.WindowStyle = ProcessWindowStyle.Minimized
        End If

        Try
            extproc.Start()

            If syncwait Then
                Do Until extproc.HasExited
                    Thread.Sleep(50)
                    Application.DoEvents()
                Loop
            End If
        Catch ex As Win32Exception
            UpdateLog(FormattedTime() & " EXTERNAL COMMAND FAILED TO EXECUTE")
        End Try
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

    Private Function FormattedTime()
        Dim timestamp As DateTime = DateTime.Now
        FormattedTime = timestamp.ToString("yyyy-MM-dd HH:mm:ss")
    End Function

End Class
