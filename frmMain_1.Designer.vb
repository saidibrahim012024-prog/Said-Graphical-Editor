<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain_1
    Inherits System.Windows.Forms.Form

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
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

    Private components As System.ComponentModel.IContainer

    'Required by the Windows Form Designer
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.msTopMenu = New System.Windows.Forms.MenuStrip()
        Me.mnuFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuNewChild = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsTopToolbar = New System.Windows.Forms.ToolStrip()
        Me.btnNewToolbar = New System.Windows.Forms.ToolStripButton()
        Me.pnlLeftToolbox = New System.Windows.Forms.Panel()
        Me.lblToolbox = New System.Windows.Forms.Label()
        Me.pnlRightExplorer = New System.Windows.Forms.Panel()
        Me.lblExplorer = New System.Windows.Forms.Label()
        Me.msTopMenu.SuspendLayout()
        Me.tsTopToolbar.SuspendLayout()
        Me.pnlLeftToolbox.SuspendLayout()
        Me.pnlRightExplorer.SuspendLayout()
        Me.SuspendLayout()
        '
        'msTopMenu
        '
        Me.msTopMenu.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.msTopMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuFile})
        Me.msTopMenu.Location = New System.Drawing.Point(0, 0)
        Me.msTopMenu.Name = "msTopMenu"
        Me.msTopMenu.Size = New System.Drawing.Size(1008, 28)
        Me.msTopMenu.TabIndex = 1
        Me.msTopMenu.Text = "MenuStrip1"
        '
        'mnuFile
        '
        Me.mnuFile.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuNewChild})
        Me.mnuFile.Name = "mnuFile"
        Me.mnuFile.Size = New System.Drawing.Size(46, 24)
        Me.mnuFile.Text = "&File"
        '
        'mnuNewChild
        '
        Me.mnuNewChild.Name = "mnuNewChild"
        Me.mnuNewChild.Size = New System.Drawing.Size(224, 26)
        Me.mnuNewChild.Text = "&New MDI Child"
        '
        'tsTopToolbar
        '
        Me.tsTopToolbar.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.tsTopToolbar.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnNewToolbar})
        Me.tsTopToolbar.Location = New System.Drawing.Point(0, 28)
        Me.tsTopToolbar.Name = "tsTopToolbar"
        Me.tsTopToolbar.Size = New System.Drawing.Size(1008, 27)
        Me.tsTopToolbar.TabIndex = 2
        Me.tsTopToolbar.Text = "ToolStrip1"
        '
        'btnNewToolbar
        '
        Me.btnNewToolbar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.btnNewToolbar.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.btnNewToolbar.Name = "btnNewToolbar"
        Me.btnNewToolbar.Size = New System.Drawing.Size(113, 24)
        Me.btnNewToolbar.Text = "New Child Form"
        '
        'pnlLeftToolbox
        '
        Me.pnlLeftToolbox.BackColor = System.Drawing.SystemColors.ControlLight
        Me.pnlLeftToolbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlLeftToolbox.Controls.Add(Me.lblToolbox)
        Me.pnlLeftToolbox.Dock = System.Windows.Forms.DockStyle.Left
        Me.pnlLeftToolbox.Location = New System.Drawing.Point(0, 55)
        Me.pnlLeftToolbox.Name = "pnlLeftToolbox"
        Me.pnlLeftToolbox.Size = New System.Drawing.Size(200, 674)
        Me.pnlLeftToolbox.TabIndex = 3
        '
        'lblToolbox
        '
        Me.lblToolbox.AutoSize = True
        Me.lblToolbox.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblToolbox.Location = New System.Drawing.Point(11, 11)
        Me.lblToolbox.Name = "lblToolbox"
        Me.lblToolbox.Size = New System.Drawing.Size(66, 20)
        Me.lblToolbox.TabIndex = 0
        Me.lblToolbox.Text = "Toolbox"
        '
        'pnlRightExplorer
        '
        Me.pnlRightExplorer.BackColor = System.Drawing.SystemColors.ControlLight
        Me.pnlRightExplorer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlRightExplorer.Controls.Add(Me.lblExplorer)
        Me.pnlRightExplorer.Dock = System.Windows.Forms.DockStyle.Right
        Me.pnlRightExplorer.Location = New System.Drawing.Point(808, 55)
        Me.pnlRightExplorer.Name = "pnlRightExplorer"
        Me.pnlRightExplorer.Size = New System.Drawing.Size(200, 674)
        Me.pnlRightExplorer.TabIndex = 4
        '
        'lblExplorer
        '
        Me.lblExplorer.AutoSize = True
        Me.lblExplorer.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold)
        Me.lblExplorer.Location = New System.Drawing.Point(11, 11)
        Me.lblExplorer.Name = "lblExplorer"
        Me.lblExplorer.Size = New System.Drawing.Size(109, 20)
        Me.lblExplorer.TabIndex = 0
        Me.lblExplorer.Text = "Explorer Panel"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.Size(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1008, 729)
        Me.Controls.Add(Me.pnlRightExplorer)
        Me.Controls.Add(Me.pnlLeftToolbox)
        Me.Controls.Add(Me.tsTopToolbar)
        Me.Controls.Add(Me.msTopMenu)
        Me.IsMdiContainer = True
        Me.MainMenuStrip = Me.msTopMenu
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "MDI Application Layout"
        Me.msTopMenu.ResumeLayout(False)
        Me.msTopMenu.PerformLayout()
        Me.tsTopToolbar.ResumeLayout(False)
        Me.tsTopToolbar.PerformLayout()
        Me.pnlLeftToolbox.ResumeLayout(False)
        Me.pnlLeftToolbox.PerformLayout()
        Me.pnlRightExplorer.ResumeLayout(False)
        Me.pnlRightExplorer.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents msTopMenu As MenuStrip
    Friend WithEvents mnuFile As ToolStripMenuItem
    Friend WithEvents mnuNewChild As ToolStripMenuItem
    Friend WithEvents tsTopToolbar As ToolStrip
    Friend WithEvents btnNewToolbar As ToolStripButton
    Friend WithEvents pnlLeftToolbox As Panel
    Friend WithEvents lblToolbox As Label
    Friend WithEvents pnlRightExplorer As Panel
    Friend WithEvents lblExplorer As Label
End Class


