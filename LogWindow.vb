Imports System.ComponentModel

Public Class LogWindow

    Private Sub LogWindow_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        Me.LogTextBox.Width = Me.Width - 16
        Me.LogTextBox.Height = Me.Height - 38
    End Sub

    Private Sub LogWindow_Closing(sender As Object, e As CancelEventArgs) Handles MyBase.Closing
        Me.Hide()
        e.Cancel = True
    End Sub

End Class