Public Class LogWindow

    Private Sub LogWindow_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Me.LogTextBox.Width = Me.Width - 18
        Me.LogTextBox.Height = Me.Height - 39
    End Sub
End Class