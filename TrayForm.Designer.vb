<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TrayForm
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
        TrayMenu = New ContextMenuStrip(components)
        StartMenuItem = New ToolStripMenuItem()
        StopMenuItem = New ToolStripMenuItem()
        RestartMenuItem = New ToolStripMenuItem()
        ToolStripSeparator1 = New ToolStripSeparator()
        AutoRestartMenuItem = New ToolStripMenuItem()
        ToolStripSeparator2 = New ToolStripSeparator()
        ExitMenuItem = New ToolStripMenuItem()
        TrayMenu.SuspendLayout()
        SuspendLayout()
        ' 
        ' TrayMenu
        ' 
        TrayMenu.Items.AddRange(New ToolStripItem() {StartMenuItem, StopMenuItem, RestartMenuItem, ToolStripSeparator1, AutoRestartMenuItem, ToolStripSeparator2, ExitMenuItem})
        TrayMenu.Name = "ContextMenuStrip1"
        TrayMenu.Size = New Size(181, 148)
        TrayMenu.Text = "TEXT02"' 
        ' StartMenuItem
        ' 
        StartMenuItem.Name = "StartMenuItem"
        StartMenuItem.Size = New Size(180, 22)
        StartMenuItem.Text = "Start SDRTrunk"' 
        ' StopMenuItem
        ' 
        StopMenuItem.Enabled = False
        StopMenuItem.Name = "StopMenuItem"
        StopMenuItem.Size = New Size(180, 22)
        StopMenuItem.Text = "Stop SDRTrunk"' 
        ' RestartMenuItem
        ' 
        RestartMenuItem.Enabled = False
        RestartMenuItem.Name = "RestartMenuItem"
        RestartMenuItem.Size = New Size(180, 22)
        RestartMenuItem.Text = "Restart SDRTrunk"' 
        ' ToolStripSeparator1
        ' 
        ToolStripSeparator1.Name = "ToolStripSeparator1"
        ToolStripSeparator1.Size = New Size(177, 6)
        ' 
        ' AutoRestartMenuItem
        ' 
        AutoRestartMenuItem.Checked = True
        AutoRestartMenuItem.CheckOnClick = True
        AutoRestartMenuItem.CheckState = CheckState.Checked
        AutoRestartMenuItem.Name = "AutoRestartMenuItem"
        AutoRestartMenuItem.Size = New Size(180, 22)
        AutoRestartMenuItem.Text = "Auto Restart"' 
        ' ToolStripSeparator2
        ' 
        ToolStripSeparator2.Name = "ToolStripSeparator2"
        ToolStripSeparator2.Size = New Size(177, 6)
        ' 
        ' ExitMenuItem
        ' 
        ExitMenuItem.Name = "ExitMenuItem"
        ExitMenuItem.Size = New Size(180, 22)
        ExitMenuItem.Text = "Exit"' 
        ' TrayForm
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 450)
        FormBorderStyle = FormBorderStyle.None
        Name = "TrayForm"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.Manual
        Text = "SDRTrunk Monitor"
        TopMost = True
        TrayMenu.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents TrayMenu As ContextMenuStrip
    Friend WithEvents StartMenuItem As ToolStripMenuItem
    Friend WithEvents StopMenuItem As ToolStripMenuItem
    Friend WithEvents RestartMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents ExitMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents AutoRestartMenuItem As ToolStripMenuItem
End Class
