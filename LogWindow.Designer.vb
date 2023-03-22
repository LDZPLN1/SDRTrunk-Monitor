<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LogWindow
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
        Dim resources As ComponentModel.ComponentResourceManager = New ComponentModel.ComponentResourceManager(GetType(LogWindow))
        LogTextBox = New TextBox()
        SuspendLayout()
        ' 
        ' LogTextBox
        ' 
        LogTextBox.BackColor = SystemColors.Control
        LogTextBox.Font = New Font("Consolas", 8.25F, FontStyle.Regular, GraphicsUnit.Point)
        LogTextBox.ForeColor = SystemColors.WindowText
        LogTextBox.Location = New Point(0, 0)
        LogTextBox.Multiline = True
        LogTextBox.Name = "LogTextBox"
        LogTextBox.ReadOnly = True
        LogTextBox.ScrollBars = ScrollBars.Both
        LogTextBox.Size = New Size(1064, 598)
        LogTextBox.TabIndex = 0
        LogTextBox.WordWrap = False
        ' 
        ' LogWindow
        ' 
        AutoScaleDimensions = New SizeF(7F, 14F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = SystemColors.Control
        ClientSize = New Size(1064, 597)
        Controls.Add(LogTextBox)
        Font = New Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "LogWindow"
        Text = "Log"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents LogTextBox As TextBox
End Class
