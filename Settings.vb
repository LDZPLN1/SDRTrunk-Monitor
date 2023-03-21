Imports System.ComponentModel
Imports System.IO

Public Class SettingsForm

    ' SHOW CURRENT SETTINGS
    Private Sub SettingsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SDRTPathTextBox.Text = My.Settings.SDRTPath
        PollTimerTextBox.Text = My.Settings.Watchdog

        Select Case My.Settings.SDRTVersion
            Case "0.5.x"
                V5RadioButton.Checked = True
            Case "0.6.x"
                V6RadioButton.Checked = True
            Case Else
                V5RadioButton.Checked = False
                V6RadioButton.Checked = False
        End Select
    End Sub

    ' SELECT DIRECTORY AND VALIDATE
    Private Sub SelectDirButton_Click(sender As Object, e As EventArgs) Handles SelectDirButton.Click
        If SDRTFolderDialog.ShowDialog() = DialogResult.OK Then
            If File.Exists(SDRTFolderDialog.SelectedPath & "\bin\sdr-trunk.bat") Then
                SDRTPathTextBox.Text = SDRTFolderDialog.SelectedPath
            Else
                SettingsToolTip.Show("SDRTrunk not found in selected directory", SDRTPathTextBox, 0, SDRTPathTextBox.Height, 5000)
            End If
        End If
    End Sub

    ' VALIDATE AND SAVE SETTINGS
    Private Sub SaveButton_Click(sender As Object, e As EventArgs) Handles SaveButton.Click
        Dim valerror As Boolean = False

        If Not File.Exists(SDRTPathTextBox.Text & "\bin\sdr-trunk.bat") Then
            SettingsToolTip.Show("SDRTrunk not found in selected directory", SDRTPathTextBox, 0, SDRTPathTextBox.Height, 5000)
            valerror = True
        End If

        If Not V5RadioButton.Checked And Not V6RadioButton.Checked And Not valerror Then
            SettingsToolTip.Show("Please select a version", VersionGroupBox, 0, VersionGroupBox.Height, 5000)
            valerror = True
        End If

        If Not valerror Then
            If V5RadioButton.Checked = True Then
                My.Settings.SDRTVersion = "0.5.x"
            ElseIf V6RadioButton.Checked = True Then
                My.Settings.SDRTVersion = "0.6.x"
            End If

            My.Settings.SDRTPath = SDRTPathTextBox.Text
            My.Settings.Watchdog = PollTimerTextBox.Text

            My.Settings.Save()

            PrimaryForm.pchecktimer.Stop()
            PrimaryForm.pchecktimer.Interval = My.Settings.Watchdog * 1000
            PrimaryForm.pchecktimer.Start()
            Me.Close()
        End If
    End Sub

    ' CANCEL AND CLOSE FORM
    Private Sub CancelButton_Click(sender As Object, e As EventArgs) Handles CancelSetButton.Click, CancelSetButton.Click
        AutoValidate = AutoValidate.Disable
        Me.Close()
    End Sub

    ' VALIDATE POLL TIMER SETTING IS AN INTEGER AND WITHIN RANGE
    Private Sub PollTimerTextBox_Validating(sender As Object, e As CancelEventArgs) Handles PollTimerTextBox.Validating
        If PollTimerTextBox.TextLength = 0 Then PollTimerTextBox.Text = "60"

        If Integer.TryParse(PollTimerTextBox.Text, Nothing) Then
            If Int(PollTimerTextBox.Text) < 5 Or Int(PollTimerTextBox.Text) > 3600 Then
                SettingsToolTip.Show("Please enter a vlue between 5 and 3600.", sender, 0, sender.Height, 5000)
                PollTimerTextBox.SelectAll()
                e.Cancel = True
            Else
                SettingsToolTip.SetToolTip(PollTimerTextBox, String.Empty)
            End If
        Else
            SettingsToolTip.Show("Please enter a vlue between 5 and 3600.", sender, 0, sender.Height, 5000)
            PollTimerTextBox.SelectAll()
            e.Cancel = True
        End If
    End Sub

End Class