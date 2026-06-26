Public Class frmMain_1

    Private childCount As Integer = 0
    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Fallback initialization to ensure MDI Container status is active
        Me.IsMdiContainer = True
    End Sub

    ' Handler for the File Menu selection
    Private Sub mnuNewChild_Click(sender As Object, e As EventArgs) Handles mnuNewChild.Click
        CreateNewChildForm()
    End Sub

    ' Handler for the Toolbar Button click
    Private Sub btnNewToolbar_Click(sender As Object, e As EventArgs) Handles btnNewToolbar.Click
        CreateNewChildForm()
    End Sub

    ' Instantiates and displays a child window inside the center MDI space
    Private Sub CreateNewChildForm()
        childCount += 1

        Dim childForm As New Form()
        childForm.MdiParent = Me
        childForm.Text = "Document " & childCount
        childForm.Width = 400
        childForm.Height = 300

        ' Add a visual anchor inside the child form so you can see it easily
        Dim lblInsideChild As New Label()
        lblInsideChild.Text = "Content Window Area for: " & childForm.Text
        lblInsideChild.AutoSize = True
        lblInsideChild.Location = New Point(20, 20)
        childForm.Controls.Add(lblInsideChild)

        childForm.Show()
    End Sub

End Class