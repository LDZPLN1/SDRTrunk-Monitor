<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class PrimaryForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim resources As ComponentModel.ComponentResourceManager = New ComponentModel.ComponentResourceManager(GetType(PrimaryForm))
        TrayNotifyIcon = New NotifyIcon(components)
        TrayMenu = New ContextMenuStrip(components)
        StartMenuItem = New ToolStripMenuItem()
        StopMenuItem = New ToolStripMenuItem()
        RestartMenuItem = New ToolStripMenuItem()
        ToolStripSeparator3 = New ToolStripSeparator()
        ViewLogMenuItem = New ToolStripMenuItem()
        ToolStripSeparator2 = New ToolStripSeparator()
        SettingsMenuItem = New ToolStripMenuItem()
        AutoRestartMenuItem = New ToolStripMenuItem()
        ToolStripSeparator1 = New ToolStripSeparator()
        ExitMenuItem = New ToolStripMenuItem()
        TrayMenu.SuspendLayout()
        SuspendLayout()
        ' 
        ' TrayNotifyIcon
        ' 
        TrayNotifyIcon.BalloonTipIcon = ToolTipIcon.Info
        TrayNotifyIcon.BalloonTipTitle = "SDRTrunk Monitor"
        TrayNotifyIcon.ContextMenuStrip = TrayMenu
        TrayNotifyIcon.Icon = CType(resources.GetObject("TrayNotifyIcon.Icon"), Icon)
        TrayNotifyIcon.Text = "SDRTrunk Monitor"
        TrayNotifyIcon.Visible = True
        ' 
        ' TrayMenu
        ' 
        TrayMenu.Items.AddRange(New ToolStripItem() {StartMenuItem, StopMenuItem, RestartMenuItem, ToolStripSeparator3, ViewLogMenuItem, ToolStripSeparator2, SettingsMenuItem, AutoRestartMenuItem, ToolStripSeparator1, ExitMenuItem})
        TrayMenu.Name = "TrayMenu"
        TrayMenu.Size = New Size(181, 198)
        ' 
        ' StartMenuItem
        ' 
        StartMenuItem.Name = "StartMenuItem"
        StartMenuItem.Size = New Size(180, 22)
        StartMenuItem.Text = "Start SDRTrunk"' 
        ' StopMenuItem
        ' 
        StopMenuItem.Name = "StopMenuItem"
        StopMenuItem.Size = New Size(180, 22)
        StopMenuItem.Text = "Stop SDRTrunk"' 
        ' RestartMenuItem
        ' 
        RestartMenuItem.Name = "RestartMenuItem"
        RestartMenuItem.Size = New Size(180, 22)
        RestartMenuItem.Text = "Restart SDRTrunk"' 
        ' ToolStripSeparator3
        ' 
        ToolStripSeparator3.Name = "ToolStripSeparator3"
        ToolStripSeparator3.Size = New Size(177, 6)
        ' 
        ' ViewLogMenuItem
        ' 
        ViewLogMenuItem.Name = "ViewLogMenuItem"
        ViewLogMenuItem.Size = New Size(180, 22)
        ViewLogMenuItem.Text = "View Log"' 
        ' ToolStripSeparator2
        ' 
        ToolStripSeparator2.Name = "ToolStripSeparator2"
        ToolStripSeparator2.Size = New Size(177, 6)
        ' 
        ' SettingsMenuItem
        ' 
        SettingsMenuItem.Name = "SettingsMenuItem"
        SettingsMenuItem.Size = New Size(180, 22)
        SettingsMenuItem.Text = "Settings"' 
        ' AutoRestartMenuItem
        ' 
        AutoRestartMenuItem.CheckOnClick = True
        AutoRestartMenuItem.Name = "AutoRestartMenuItem"
        AutoRestartMenuItem.Size = New Size(180, 22)
        AutoRestartMenuItem.Text = "Auto Restart"' 
        ' ToolStripSeparator1
        ' 
        ToolStripSeparator1.Name = "ToolStripSeparator1"
        ToolStripSeparator1.Size = New Size(177, 6)
        ' 
        ' ExitMenuItem
        ' 
        ExitMenuItem.Name = "ExitMenuItem"
        ExitMenuItem.Size = New Size(180, 22)
        ExitMenuItem.Text = "Exit"' 
        ' PrimaryForm
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 450)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "PrimaryForm"
        ShowInTaskbar = False
        Text = "SDRTrunk Monitor"
        WindowState = FormWindowState.Minimized
        TrayMenu.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents TrayNotifyIcon As NotifyIcon
    Friend WithEvents TrayMenu As ContextMenuStrip
    Friend WithEvents ExitMenuItem As ToolStripMenuItem
    Friend WithEvents SettingsMenuItem As ToolStripMenuItem
    Friend WithEvents AutoRestartMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents ViewLogMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents StartMenuItem As ToolStripMenuItem
    Friend WithEvents StopMenuItem As ToolStripMenuItem
    Friend WithEvents RestartMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
End Class
