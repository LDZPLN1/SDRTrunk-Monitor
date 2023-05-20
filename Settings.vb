Imports System.ComponentModel
Imports System.IO

Public Class SettingsForm

    ' SHOW CURRENT SETTINGS
    Private Sub SettingsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SDRTPathTextBox.Text = My.Settings.SDRTPath

        Select Case My.Settings.SDRTVersion
            Case "0.5.x"
                V5RadioButton.Checked = True
            Case "0.6.x"
                V6RadioButton.Checked = True
            Case Else
                V5RadioButton.Checked = False
                V6RadioButton.Checked = False
        End Select

        ExtCommandTextBox.Text = My.Settings.ExternalCommand
        PollTimerTextBox.Text = My.Settings.Watchdog
        TimedCommandTextBox.Text = My.Settings.TimedExternalCommand
        ExtTimerTextBox.Text = My.Settings.ExternalCommandTimer
        RunTimedExtMinCheckBox.Checked = My.Settings.TimedExternalMinimized

        If SDRTPathTextBox.Text = String.Empty Then
            VersionGroupBox.Enabled = False
        End If

        If TimedCommandTextBox.Text = String.Empty Then
            ExtTimerTextBox.Enabled = False
            RunTimedExtMinCheckBox.Enabled = False
        End If

        AutoValidate = AutoValidate.EnablePreventFocusChange
    End Sub

    ' SELECT DIRECTORY AND VALIDATE
    Private Sub SelectDirButton_Click(sender As Object, e As EventArgs) Handles SelectDirButton.Click
        If SDRTFolderDialog.ShowDialog() = DialogResult.OK Then
            SDRTPathTextBox.Text = SDRTFolderDialog.SelectedPath
            SDRTPathTextBox_Validate()
        End If
    End Sub

    ' VALIDATE AND SAVE SETTINGS
    Private Sub SaveButton_Click(sender As Object, e As EventArgs) Handles SaveButton.Click
        Dim valerror As Boolean = False

        If SDRTPathTextBox.Text = String.Empty Then
            SettingsToolTip.Show("SDRTrunk directory not selected", SDRTPathTextBox, 0, SDRTPathTextBox.Height, 5000)
            valerror = True
        Else
            If Not File.Exists(SDRTPathTextBox.Text & "\bin\sdr-trunk.bat") Then
                SettingsToolTip.Show("SDRTrunk not found in selected directory", SDRTPathTextBox, 0, SDRTPathTextBox.Height, 5000)
                valerror = True
            End If
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
            My.Settings.ExternalCommand = ExtCommandTextBox.Text
            My.Settings.Watchdog = PollTimerTextBox.Text
            My.Settings.TimedExternalCommand = TimedCommandTextBox.Text
            My.Settings.ExternalCommandTimer = ExtTimerTextBox.Text
            My.Settings.TimedExternalMinimized = RunTimedExtMinCheckBox.Checked
            My.Settings.Save()

            If My.Settings.ExternalCommand <> String.Empty Then
                PrimaryForm.RunExternalMenuItem.Checked = My.Settings.RunExternal
                PrimaryForm.RunExternalMenuItem.Enabled = True
            Else
                PrimaryForm.RunExternalMenuItem.Checked = False
                PrimaryForm.RunExternalMenuItem.Enabled = False
            End If

            If My.Settings.TimedExternalCommand <> String.Empty Then
                PrimaryForm.RunTimedExternalMenuItem.Checked = My.Settings.RunTimedExternal
                PrimaryForm.RunTimedExternalMenuItem.Enabled = True
            Else
                PrimaryForm.RunTimedExternalMenuItem.Checked = False
                PrimaryForm.RunTimedExternalMenuItem.Enabled = False
            End If

            Dim tstate As Boolean = PrimaryForm.pchecktimer.Enabled

            PrimaryForm.pchecktimer.Stop()
            PrimaryForm.pchecktimer.Interval = My.Settings.Watchdog * 1000
            PrimaryForm.pchecktimer.Start()
            PrimaryForm.pchecktimer.Enabled = tstate

            tstate = PrimaryForm.extruntimer.Enabled

            PrimaryForm.extruntimer.Stop()
            PrimaryForm.extruntimer.Interval = My.Settings.ExternalCommandTimer * 1000
            PrimaryForm.extruntimer.Start()
            PrimaryForm.extruntimer.Enabled = tstate
            Close()
        End If
    End Sub

    ' CANCEL AND CLOSE FORM
    Private Sub CancelButton_Click(sender As Object, e As EventArgs) Handles CancelSetButton.Click, CancelSetButton.Click
        AutoValidate = AutoValidate.Disable
        Close()
    End Sub

    ' VALIDATE POLL TIMER SETTING IS AN INTEGER AND WITHIN RANGE
    Private Sub PollTimerTextBox_Validating(sender As Object, e As CancelEventArgs) Handles PollTimerTextBox.Validating
        If PollTimerTextBox.TextLength = 0 Then PollTimerTextBox.Text = "300"

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

    ' VALIDATE EXTERNAL COMMAND TIMER SETTING IS AN INTEGER AND WITHIN RANGE
    Private Sub ExtTimerTextBox_Validating(sender As Object, e As CancelEventArgs) Handles ExtTimerTextBox.Validating
        If ExtTimerTextBox.TextLength = 0 Then PollTimerTextBox.Text = "3600"

        If Integer.TryParse(ExtTimerTextBox.Text, Nothing) Then
            If Int(ExtTimerTextBox.Text) < 60 Or Int(ExtTimerTextBox.Text) > 43200 Then
                SettingsToolTip.Show("Please enter a vlue between 60 and 43200.", sender, 0, sender.Height, 5000)
                ExtTimerTextBox.SelectAll()
                e.Cancel = True
            Else
                SettingsToolTip.SetToolTip(ExtTimerTextBox, String.Empty)
            End If
        Else
            SettingsToolTip.Show("Please enter a vlue between 600 and 43200.", sender, 0, sender.Height, 5000)
            ExtTimerTextBox.SelectAll()
            e.Cancel = True
        End If
    End Sub

    Private Sub SDRTPathTextBox_Validating(sender As Object, e As CancelEventArgs) Handles SDRTPathTextBox.Validating
        SDRTPathTextBox_Validate()
    End Sub

    Private Sub SDRTPathTextBox_Validate()
        If SDRTPathTextBox.Text <> String.Empty Then
            If File.Exists(SDRTPathTextBox.Text & "\bin\sdr-trunk.bat") Then
                VersionGroupBox.Enabled = True
            Else
                VersionGroupBox.Enabled = False
                SettingsToolTip.Show("SDRTrunk not found in selected directory", SDRTPathTextBox, 0, SDRTPathTextBox.Height, 5000)
            End If
        Else
            VersionGroupBox.Enabled = False
        End If
    End Sub

    Private Sub TimedCommandTextBox_Validated(sender As Object, e As EventArgs) Handles TimedCommandTextBox.Validated
        If TimedCommandTextBox.Text = String.Empty Then
            ExtTimerTextBox.Enabled = False
            RunTimedExtMinCheckBox.Enabled = False
        Else
            ExtTimerTextBox.Enabled = True
            RunTimedExtMinCheckBox.Enabled = True
        End If
    End Sub
End Class