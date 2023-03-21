<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SettingsForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim resources As ComponentModel.ComponentResourceManager = New ComponentModel.ComponentResourceManager(GetType(SettingsForm))
        PathLabel = New Label()
        SaveButton = New Button()
        CancelSetButton = New Button()
        SDRTPathTextBox = New TextBox()
        VersionGroupBox = New GroupBox()
        V6RadioButton = New RadioButton()
        V5RadioButton = New RadioButton()
        TimerLabel = New Label()
        PollTimerTextBox = New TextBox()
        SDRTFolderDialog = New FolderBrowserDialog()
        SelectDirButton = New Button()
        SettingsToolTip = New ToolTip(components)
        VersionGroupBox.SuspendLayout()
        SuspendLayout()
        ' 
        ' PathLabel
        ' 
        PathLabel.AutoSize = True
        PathLabel.Location = New Point(12, 28)
        PathLabel.Name = "PathLabel"
        PathLabel.Size = New Size(86, 15)
        PathLabel.TabIndex = 0
        PathLabel.Text = "SDRTrunk Path:"' 
        ' SaveButton
        ' 
        SaveButton.Location = New Point(445, 82)
        SaveButton.Name = "SaveButton"
        SaveButton.Size = New Size(75, 23)
        SaveButton.TabIndex = 6
        SaveButton.Text = "Save"
        SaveButton.UseVisualStyleBackColor = True
        ' 
        ' CancelSetButton
        ' 
        CancelSetButton.CausesValidation = False
        CancelSetButton.DialogResult = DialogResult.Cancel
        CancelSetButton.Location = New Point(526, 82)
        CancelSetButton.Name = "CancelSetButton"
        CancelSetButton.Size = New Size(75, 23)
        CancelSetButton.TabIndex = 7
        CancelSetButton.Text = "Cancel"
        CancelSetButton.UseVisualStyleBackColor = True
        ' 
        ' SDRTPathTextBox
        ' 
        SDRTPathTextBox.Location = New Point(110, 24)
        SDRTPathTextBox.Name = "SDRTPathTextBox"
        SDRTPathTextBox.Size = New Size(277, 23)
        SDRTPathTextBox.TabIndex = 1
        ' 
        ' VersionGroupBox
        ' 
        VersionGroupBox.Controls.Add(V6RadioButton)
        VersionGroupBox.Controls.Add(V5RadioButton)
        VersionGroupBox.Location = New Point(478, 6)
        VersionGroupBox.Name = "VersionGroupBox"
        VersionGroupBox.Size = New Size(123, 53)
        VersionGroupBox.TabIndex = 3
        VersionGroupBox.TabStop = False
        VersionGroupBox.Text = "SDRTrunk Version"' 
        ' V6RadioButton
        ' 
        V6RadioButton.AutoSize = True
        V6RadioButton.Location = New Point(67, 22)
        V6RadioButton.Name = "V6RadioButton"
        V6RadioButton.Size = New Size(55, 19)
        V6RadioButton.TabIndex = 1
        V6RadioButton.TabStop = True
        V6RadioButton.Text = "v0.6.x"
        V6RadioButton.UseVisualStyleBackColor = True
        ' 
        ' V5RadioButton
        ' 
        V5RadioButton.AutoSize = True
        V5RadioButton.Location = New Point(6, 22)
        V5RadioButton.Name = "V5RadioButton"
        V5RadioButton.Size = New Size(55, 19)
        V5RadioButton.TabIndex = 0
        V5RadioButton.TabStop = True
        V5RadioButton.Text = "v0.5.x"
        V5RadioButton.UseVisualStyleBackColor = True
        ' 
        ' TimerLabel
        ' 
        TimerLabel.AutoSize = True
        TimerLabel.Location = New Point(12, 56)
        TimerLabel.Name = "TimerLabel"
        TimerLabel.Size = New Size(92, 15)
        TimerLabel.TabIndex = 5
        TimerLabel.Text = "Poll Timer (Sec):"' 
        ' PollTimerTextBox
        ' 
        PollTimerTextBox.Location = New Point(110, 53)
        PollTimerTextBox.Name = "PollTimerTextBox"
        PollTimerTextBox.Size = New Size(67, 23)
        PollTimerTextBox.TabIndex = 4
        ' 
        ' SDRTFolderDialog
        ' 
        SDRTFolderDialog.Description = "Select the SDRTrunk Base Folder"
        SDRTFolderDialog.ShowNewFolderButton = False
        SDRTFolderDialog.UseDescriptionForTitle = True
        ' 
        ' SelectDirButton
        ' 
        SelectDirButton.Location = New Point(393, 24)
        SelectDirButton.Name = "SelectDirButton"
        SelectDirButton.Size = New Size(75, 23)
        SelectDirButton.TabIndex = 2
        SelectDirButton.Text = "Select"
        SelectDirButton.UseVisualStyleBackColor = True
        ' 
        ' SettingsForm
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(613, 111)
        ControlBox = False
        Controls.Add(SelectDirButton)
        Controls.Add(PollTimerTextBox)
        Controls.Add(TimerLabel)
        Controls.Add(VersionGroupBox)
        Controls.Add(SDRTPathTextBox)
        Controls.Add(CancelSetButton)
        Controls.Add(SaveButton)
        Controls.Add(PathLabel)
        FormBorderStyle = FormBorderStyle.FixedDialog
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "SettingsForm"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Settings"
        VersionGroupBox.ResumeLayout(False)
        VersionGroupBox.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents PathLabel As Label
    Friend WithEvents SaveButton As Button
    Friend WithEvents CancelSetButton As Button
    Friend WithEvents VersionGroupBox As GroupBox
    Friend WithEvents V6RadioButton As RadioButton
    Friend WithEvents V5RadioButton As RadioButton
    Friend WithEvents TimerLabel As Label
    Friend WithEvents PollTimerTextBox As TextBox
    Friend WithEvents SDRTFolderDialog As FolderBrowserDialog
    Friend WithEvents SelectDirButton As Button
    Friend WithEvents SDRTPathTextBox As TextBox
    Friend WithEvents SettingsToolTip As ToolTip
End Class
