<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
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
        Dim resources As ComponentModel.ComponentResourceManager = New ComponentModel.ComponentResourceManager(GetType(MainForm))
        TrayIcon = New NotifyIcon(components)
        SuspendLayout()
        ' 
        ' TrayIcon
        ' 
        TrayIcon.BalloonTipText = "TEXT03"
        TrayIcon.BalloonTipTitle = "TEXT04"
        TrayIcon.Icon = CType(resources.GetObject("TrayIcon.Icon"), Icon)
        TrayIcon.Text = "SDRTrunk Monitor"
        TrayIcon.Visible = True
        ' 
        ' MainForm
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 450)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "MainForm"
        ShowInTaskbar = False
        Text = "MainForm"
        WindowState = FormWindowState.Minimized
        ResumeLayout(False)
    End Sub

    Friend WithEvents TrayIcon As NotifyIcon
End Class
