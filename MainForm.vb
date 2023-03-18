' SDRTRUNK MONITOR
'
' COPYRIGHT (c) 2023 DOUGLAS GRAHAM, ALL RIGHTS RESERVED
'
' MONITORS SDRTRUNK PROCESS STATUS AND OPTIONALLY RESTARTS SDRTRUNK WHEN AN ERROR IS ENCOUNTERED

Public Class MainForm

    ' TRAY ICON MENU TRIGGER - ON RIGHT CLICK SHOW MENU
    Private Sub TrayIcon_MouseClick(sender As Object, e As MouseEventArgs) Handles TrayIcon.MouseClick
        If e.Button = Windows.Forms.MouseButtons.Right Then
            TrayForm.Show()
            TrayForm.Activate()
            TrayForm.Width = 1
            TrayForm.Height = 1
        End If
    End Sub

    ' INITIALIZE WATCHDOG TIMER
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        TrayForm.Startup()
    End Sub
End Class
