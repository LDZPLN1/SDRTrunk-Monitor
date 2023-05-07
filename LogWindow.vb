Imports System.ComponentModel

Public Class LogWindow

    Private Sub LogWindow_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        LogTextBox.Width = Width - 16
        LogTextBox.Height = Height - 38
    End Sub

    Private Sub LogWindow_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If e.CloseReason = CloseReason.UserClosing Then
            e.Cancel = True
            Hide()
        End If
    End Sub
End Class