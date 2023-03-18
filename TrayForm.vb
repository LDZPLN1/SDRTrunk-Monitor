Imports System.Timers

Public Class TrayForm
    ' PATH TO SDRTRUNK
    Private Shared ReadOnly sdrt_path As String = "C:\SDR\SDRTrunk6"

    ' WATCHDOG TIMER IN ms
    Private Shared ReadOnly pchecktimer As New Timer(60000)

    ' STARTUP ARGUMENTS - UNCOMMENT BASED ON SDRTRUNK VERSION

    '0.5.x
    'Private Shared ReadOnly SDRTArgs As String = "--add-exports=javafx.base/com.sun.javafx.event=ALL-UNNAMED --add-modules=jdk.incubator.vector --add-exports=java.desktop/com.sun.java.swing.plaf.windows=ALL-UNNAMED -classpath " & sdrt_path & "\lib\* io.github.dsheirer.gui.SDRTrunk"

    '0.6.x
    Private Shared ReadOnly SDRTArgs As String = "--add-exports=javafx.base/com.sun.javafx.event=ALL-UNNAMED --add-modules=jdk.incubator.vector --add-exports=java.desktop/com.sun.java.swing.plaf.windows=ALL-UNNAMED --enable-preview --enable-native-access=ALL-UNNAMED ""-Djava.library.path=C:\Program Files\SDRplay\API\x64"" -classpath C:\SDR\SDRTrunk6\lib\* io.github.dsheirer.gui.SDRTrunk"

    Private Shared ReadOnly sdrproc As New Process()

    ' SETUP WATCHDOG TIMER AND START IT
    Public Sub Startup()
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

    ' AUTO RESTART - SET WATCHDOG TIMER STATE WHEN AUTO RESTART SETTING IS CHANGED
    Private Sub AutoRestartMenuItem_CheckStateChanged(sender As Object, e As EventArgs) Handles AutoRestartMenuItem.CheckStateChanged
        If Me.AutoRestartMenuItem.CheckState = CheckState.Unchecked Then
            pchecktimer.Enabled = False
        Else
            If SDRTState() = 1 Then
                pchecktimer.Enabled = True
            End If
        End If
    End Sub

    ' MENU ITEM TO EXIT PROGRAM
    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitMenuItem.Click
        MainForm.TrayIcon.Visible = False
        End
    End Sub

    ' START SDRTRUNK IN A NEW PROCESS AND REDIRECT STANDARD OUTPUT TO ASYNC OUTPUT HANDLER
    Private Sub Start_SDRT()
        AddHandler sdrproc.OutputDataReceived, AddressOf ReadStandardOutput

        Try
            sdrproc.CancelOutputRead()
        Catch ex As Exception
        End Try

        sdrproc.Close()
        sdrproc.StartInfo.UseShellExecute = False
        sdrproc.StartInfo.RedirectStandardOutput = True
        sdrproc.StartInfo.FileName = sdrt_path & "\bin\java.exe"
        sdrproc.StartInfo.Arguments = SDRTArgs
        sdrproc.Start()
        sdrproc.BeginOutputReadLine()

        ' ENABLE WATCHDOG TIMER IF AUTO RESTART IS ENABLED
        If Me.AutoRestartMenuItem.CheckState = CheckState.Checked Then
            pchecktimer.Enabled = True
        End If
    End Sub

    ' STOP SDRTRUNK AND DISABLE WATCHDOG TIMER
    Private Shared Sub Stop_SDRT()
        sdrproc.Kill()
        pchecktimer.Enabled = False
    End Sub

    ' ASYNC OUTPUT HANDLER FOR SDRTRUNK COMMAND WINDOW - MONITOR FOR ERRORS
    Private Sub ReadStandardOutput(sender As Object, args As DataReceivedEventArgs)
        If args.Data IsNot Nothing Then
            Debug.Print(args.Data)

            If args.Data.Contains("Couldn't design final output low pass filter") Or args.Data.Contains("org.usb4java.LibUsbException") Then
                If Me.AutoRestartMenuItem.CheckState = CheckState.Checked Then
                    Stop_SDRT()
                    Start_SDRT()
                Else
                    MsgBox("SDRTrunk appears to have failed", vbExclamation, "SDRTrunk Error")
                End If
            End If
        End If
    End Sub

    ' CHECK PROCESS STATUS WHEN WATCHDOG TIMER FIRES
    Private Sub TimerElapsed(ByVal sender As Object, ByVal e As ElapsedEventArgs)
        If SDRTState() = 0 Then
            Start_SDRT()
        End If
    End Sub

    ' RETURN SDRTRUNK PROCESS STATE
    '   0 = NOT RUNNING
    '   1 = STARTED BY THIS APP
    '   2 = RUNNING, BUT NOT STARTED BY THIS APP

    Private Shared Function SDRTState()
        SDRTState = 0
        Dim pid As Integer
        Dim proclist As Process() = Process.GetProcessesByName("java")

        For Each sproc As Process In proclist
            If sproc.MainModule.FileName = sdrt_path & "\bin\java.exe" Then
                Try
                    pid = sdrproc.Id
                    SDRTState = 1
                Catch ex As System.InvalidOperationException
                    SDRTState = 2
                End Try
            End If
        Next
    End Function
End Class