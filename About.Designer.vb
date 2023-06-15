<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class About
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
        Dim resources As ComponentModel.ComponentResourceManager = New ComponentModel.ComponentResourceManager(GetType(About))
        OKButton = New Button()
        TextBoxDescription = New TextBox()
        LabelCompanyName = New Label()
        LabelCopyright = New Label()
        LabelVersion = New Label()
        LabelProductName = New Label()
        TableLayoutPanel = New TableLayoutPanel()
        TableLayoutPanel.SuspendLayout()
        SuspendLayout()
        ' 
        ' OKButton
        ' 
        OKButton.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        OKButton.DialogResult = DialogResult.Cancel
        OKButton.Location = New Point(371, 268)
        OKButton.Margin = New Padding(4, 3, 4, 3)
        OKButton.Name = "OKButton"
        OKButton.Size = New Size(88, 27)
        OKButton.TabIndex = 0
        OKButton.Text = "&OK"
        ' 
        ' TextBoxDescription
        ' 
        TextBoxDescription.Dock = DockStyle.Fill
        TextBoxDescription.Location = New Point(7, 119)
        TextBoxDescription.Margin = New Padding(7, 3, 4, 3)
        TextBoxDescription.Multiline = True
        TextBoxDescription.Name = "TextBoxDescription"
        TextBoxDescription.ReadOnly = True
        TextBoxDescription.ScrollBars = ScrollBars.Both
        TextBoxDescription.Size = New Size(452, 143)
        TextBoxDescription.TabIndex = 0
        TextBoxDescription.TabStop = False
        TextBoxDescription.Text = resources.GetString("TextBoxDescription.Text")
        ' 
        ' LabelCompanyName
        ' 
        LabelCompanyName.Dock = DockStyle.Fill
        LabelCompanyName.Location = New Point(7, 87)
        LabelCompanyName.Margin = New Padding(7, 0, 4, 0)
        LabelCompanyName.MaximumSize = New Size(0, 20)
        LabelCompanyName.Name = "LabelCompanyName"
        LabelCompanyName.Size = New Size(452, 20)
        LabelCompanyName.TabIndex = 0
        LabelCompanyName.Text = "Source: https://github.com/LDZPLN1/SDRTrunk-Monitor"
        LabelCompanyName.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' LabelCopyright
        ' 
        LabelCopyright.Dock = DockStyle.Fill
        LabelCopyright.Location = New Point(7, 58)
        LabelCopyright.Margin = New Padding(7, 0, 4, 0)
        LabelCopyright.MaximumSize = New Size(0, 20)
        LabelCopyright.Name = "LabelCopyright"
        LabelCopyright.Size = New Size(452, 20)
        LabelCopyright.TabIndex = 0
        LabelCopyright.Text = "Copyright"
        LabelCopyright.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' LabelVersion
        ' 
        LabelVersion.Dock = DockStyle.Fill
        LabelVersion.Location = New Point(7, 29)
        LabelVersion.Margin = New Padding(7, 0, 4, 0)
        LabelVersion.MaximumSize = New Size(0, 20)
        LabelVersion.Name = "LabelVersion"
        LabelVersion.Size = New Size(452, 20)
        LabelVersion.TabIndex = 0
        LabelVersion.Text = "Version"
        LabelVersion.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' LabelProductName
        ' 
        LabelProductName.Dock = DockStyle.Fill
        LabelProductName.Location = New Point(7, 0)
        LabelProductName.Margin = New Padding(7, 0, 4, 0)
        LabelProductName.MaximumSize = New Size(0, 20)
        LabelProductName.Name = "LabelProductName"
        LabelProductName.Size = New Size(452, 20)
        LabelProductName.TabIndex = 0
        LabelProductName.Text = "Product Name"
        LabelProductName.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' TableLayoutPanel
        ' 
        TableLayoutPanel.ColumnCount = 1
        TableLayoutPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100F))
        TableLayoutPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 20F))
        TableLayoutPanel.Controls.Add(LabelProductName, 0, 0)
        TableLayoutPanel.Controls.Add(LabelVersion, 0, 1)
        TableLayoutPanel.Controls.Add(LabelCopyright, 0, 2)
        TableLayoutPanel.Controls.Add(LabelCompanyName, 0, 3)
        TableLayoutPanel.Controls.Add(TextBoxDescription, 0, 4)
        TableLayoutPanel.Controls.Add(OKButton, 0, 5)
        TableLayoutPanel.Dock = DockStyle.Fill
        TableLayoutPanel.Location = New Point(10, 10)
        TableLayoutPanel.Margin = New Padding(4, 3, 4, 3)
        TableLayoutPanel.Name = "TableLayoutPanel"
        TableLayoutPanel.RowCount = 6
        TableLayoutPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 10F))
        TableLayoutPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 10F))
        TableLayoutPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 10F))
        TableLayoutPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 10F))
        TableLayoutPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 50F))
        TableLayoutPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 10F))
        TableLayoutPanel.Size = New Size(463, 298)
        TableLayoutPanel.TabIndex = 0
        ' 
        ' About
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        CancelButton = OKButton
        ClientSize = New Size(483, 318)
        Controls.Add(TableLayoutPanel)
        FormBorderStyle = FormBorderStyle.FixedDialog
        Margin = New Padding(4, 3, 4, 3)
        MaximizeBox = False
        MinimizeBox = False
        Name = "About"
        Padding = New Padding(10)
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "AboutBox1"
        TableLayoutPanel.ResumeLayout(False)
        TableLayoutPanel.PerformLayout()
        ResumeLayout(False)
    End Sub

    Friend WithEvents OKButton As Button
    Friend WithEvents TextBoxDescription As TextBox
    Friend WithEvents LabelCompanyName As Label
    Friend WithEvents LabelCopyright As Label
    Friend WithEvents LabelVersion As Label
    Friend WithEvents LabelProductName As Label
    Friend WithEvents TableLayoutPanel As TableLayoutPanel
End Class
