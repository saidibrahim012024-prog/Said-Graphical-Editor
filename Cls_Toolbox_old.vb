' Target File: Cls_Toolbox.vb
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Drawing
Imports System.Windows.Forms

Public NotInheritable Class Cls_Toolbox
    Private ReadOnly _panel As Panel
    Private _lblTitle As Label
    Private _flpTools As FlowLayoutPanel

    ' Public event channel to notify the parent system when a tool is changed
    Public Event ToolSelected As EventHandler(Of ToolSelectedEventArgs)

    Public ReadOnly Property Panel As Panel
        Get
            Return _panel
        End Get
    End Property

    Public Sub New()
        _panel = New Panel()
        BuildToolbox()
    End Sub

    Private Sub BuildToolbox()
        With _panel
            .Width = 200
            .BorderStyle = BorderStyle.FixedSingle
            .Dock = DockStyle.Left
            .Padding = New Padding(10)
        End With

        _lblTitle = New Label() With {.Text = "Graphics Toolbox", .Dock = DockStyle.Top, .Height = 25, .AutoSize = False}
        _panel.Controls.Add(_lblTitle)

        _flpTools = New FlowLayoutPanel() With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.TopDown, .WrapContents = False, .AutoScroll = True}
        _panel.Controls.Add(_flpTools)
        _flpTools.BringToFront()

        ' Dynamically instantiate tool button options safely
        CreateToolButton("Select Pointer", CanvasTool.SelectPointer)
        CreateToolButton("Draw Line", CanvasTool.Line)
        CreateToolButton("Draw Circle", CanvasTool.Circle)
        CreateToolButton("Draw Rectangle", CanvasTool.Rectangle)

        ApplyCurrentTheme()
    End Sub

    Private Sub CreateToolButton(text As String, toolType As CanvasTool)
        Dim btn As New Button() With {
            .Text = text,
            .Size = New Size(178, 35),
            .Margin = New Padding(0, 4, 0, 4),
            .Tag = toolType,
            .UseVisualStyleBackColor = True
        }
        AddHandler btn.Click, AddressOf OnToolButtonClick
        _flpTools.Controls.Add(btn)
    End Sub

    Private Sub OnToolButtonClick(sender As Object, e As EventArgs)
        Dim btn As Button = TryCast(sender, Button)
        If btn IsNot Nothing AndAlso btn.Tag IsNot Nothing Then
            Dim chosenTool As CanvasTool = DirectCast(btn.Tag, CanvasTool)
            RaiseEvent ToolSelected(Me, New ToolSelectedEventArgs(chosenTool))
        End If
    End Sub

    Public Sub ApplyCurrentTheme()
        Dim config As Cls_Configuration_Manager = Cls_Configuration_Manager.Instance
        _panel.BackColor = config.PanelBackColor
        _lblTitle.Font = config.HeaderFont
        _lblTitle.ForeColor = config.PanelForeColor
        _flpTools.BackColor = Color.Transparent

        For Each ctrl As Control In _flpTools.Controls
            If TypeOf ctrl Is Button Then ctrl.Font = config.UiFont
        Next
    End Sub
End Class

' Strongly-typed custom event argument payload
Public NotInheritable Class ToolSelectedEventArgs
    Inherits EventArgs
    Public ReadOnly Property SelectedTool As CanvasTool

    Public Sub New(tool As CanvasTool)
        Me.SelectedTool = tool
    End Sub
End Class
