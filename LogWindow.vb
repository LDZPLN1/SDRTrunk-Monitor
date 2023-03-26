Imports System.ComponentModel

Public Class LogWindow

    Private Sub LogWindow_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        LogTextBox.Width = Width - 16
        LogTextBox.Height = Height - 38
    End Sub

    Private Sub LogWindow_Closing(sender As Object, e As CancelEventArgs) Handles MyBase.Closing
        Hide()
        e.Cancel = True
    End Sub

End Class