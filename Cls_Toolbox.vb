' Target File: Cls_Toolbox.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Drawing
Imports System.Windows.Forms

Public NotInheritable Class Cls_Toolbox
    Private ReadOnly _panel As Panel
    Private _lblTitle As Label
    Private _flpTools As FlowLayoutPanel

    ' Single pipeline channel to notify when a schematic component is selected
    Public Event ToolSelected As EventHandler(Of ToolSelectedEventArgs)

    Public ReadOnly Property Panel() As Panel
        Get
            Return Me._panel
        End Get
    End Property

    Public Sub New()
        Me._panel = New Panel()
        Me.BuildToolbox()
    End Sub

    ''' <summary>
    ''' Instantiates a dedicated Digital Schematic Component UI layout.
    ''' </summary>
    Private Sub BuildToolbox()
        With Me._panel
            .Width = 200
            .BorderStyle = BorderStyle.FixedSingle
            .Dock = DockStyle.Left
            .Padding = New Padding(10)
        End With

        Me._lblTitle = New Label() With {.Text = "Logic Components", .Dock = DockStyle.Top, .Height = 25, .AutoSize = False}
        Me._panel.Controls.Add(Me._lblTitle)

        Me._flpTools = New FlowLayoutPanel() With {.Dock = DockStyle.Fill, .FlowDirection = FlowDirection.TopDown, .WrapContents = False, .AutoScroll = True}
        Me._panel.Controls.Add(Me._flpTools)
        Me._flpTools.BringToFront()

        ' Instantiate standard operational tools and digital gates dynamically
        Me.CreateToolButton("Select Pointer", CanvasTool.SelectPointer, LogicGateType.Buffer)
        ' FIXED: Replace the ToolStripSeparator with a native styled Control element
        Dim visualSeparator As New Label() With {
        .Height = 2,
        .Width = 178,
        .BorderStyle = BorderStyle.Fixed3D,
        .Margin = New Padding(0, 10, 0, 10),
        .AutoSize = False
    }
        Me._flpTools.Controls.Add(visualSeparator)

        Me.PopulateDigitalGateButtons()

        Me.ApplyCurrentTheme()
    End Sub

    Private Sub PopulateDigitalGateButtons()
        Me.CreateToolButton("AND Gate", CanvasTool.LogicGate, LogicGateType.AndGate)
        Me.CreateToolButton("OR Gate", CanvasTool.LogicGate, LogicGateType.OrGate)
        Me.CreateToolButton("NOT Gate", CanvasTool.LogicGate, LogicGateType.NotGate)
        Me.CreateToolButton("NAND Gate", CanvasTool.LogicGate, LogicGateType.NandGate)
        Me.CreateToolButton("NOR Gate", CanvasTool.LogicGate, LogicGateType.NorGate)
        Me.CreateToolButton("XOR Gate", CanvasTool.LogicGate, LogicGateType.XorGate)
    End Sub

    Private Sub CreateToolButton(ByVal text As String, ByVal toolType As CanvasTool, ByVal gateType As LogicGateType)
        Dim btn As New Button() With {
            .Text = text,
            .Size = New Size(178, 35),
            .Margin = New Padding(0, 4, 0, 4),
            .Tag = New Tuple(Of CanvasTool, LogicGateType)(toolType, gateType),
            .UseVisualStyleBackColor = True
        }
        'AddHandler btn.Click, AddressOf Me.OnToolButtonClick
        AddHandler btn.MouseDown, AddressOf Me.OnToolButtonMouseDown
        Me._flpTools.Controls.Add(btn)
    End Sub

    'Private Sub OnToolButtonMouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)
    '    Dim btn As Button = TryCast(sender, Button)

    '    If btn IsNot Nothing AndAlso btn.Tag IsNot Nothing AndAlso e.Button = MouseButtons.Left Then
    '        Dim metadata As Tuple(Of CanvasTool, LogicGateType) = DirectCast(btn.Tag, Tuple(Of CanvasTool, LogicGateType))

    '        If metadata.Item1 = CanvasTool.LogicGate Then
    '            ' FIXED: Initiate native DragDrop sequence immediately on Left Mouse Down
    '            btn.DoDragDrop(metadata.Item2, DragDropEffects.Copy)
    '        End If
    '    End If
    'End Sub

    'Private Sub OnToolButtonClick(ByVal sender As Object, ByVal e As EventArgs)
    '    Dim btn As Button = TryCast(sender, Button)

    '    If btn IsNot Nothing AndAlso btn.Tag IsNot Nothing Then
    '        Dim metadata As Tuple(Of CanvasTool, LogicGateType) = DirectCast(btn.Tag, Tuple(Of CanvasTool, LogicGateType))
    '        RaiseEvent ToolSelected(Me, New ToolSelectedEventArgs(metadata.Item1, metadata.Item2))
    '    End If
    'End Sub

    Public Sub ApplyCurrentTheme()

        Dim config As Cls_Configuration_Manager = Cls_Configuration_Manager.Instance
        Me._panel.BackColor = config.PanelBackColor
        Me._lblTitle.Font = config.HeaderFont
        Me._lblTitle.ForeColor = config.PanelForeColor
        Me._flpTools.BackColor = Color.Transparent

        For Each ctrl As Control In Me._flpTools.Controls
            If TypeOf ctrl Is Button Then ctrl.Font = config.UiFont
        Next
    End Sub

    ' Target File: Cls_Toolbox.vb (OLE String Serialization Adjustment Pass)
    ' Project Namespace: mdi_test
    ' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    Private Sub OnToolButtonMouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)
        Dim btn As Button = TryCast(sender, Button)

        If btn IsNot Nothing AndAlso btn.Tag IsNot Nothing AndAlso e.Button = MouseButtons.Left Then
            Dim metadata As Tuple(Of CanvasTool, LogicGateType) = DirectCast(btn.Tag, Tuple(Of CanvasTool, LogicGateType))

            If metadata.Item1 = CanvasTool.LogicGate Then
                ' FIXED: Serialize the strongly typed Enum value to an uppercase string identifier format token
                ' This explicitly closes the pipeline loop with the viewport format filters!
                Dim payloadToken As String = metadata.Item2.ToString().ToUpper()

                btn.DoDragDrop(payloadToken, DragDropEffects.Copy)
            End If
        End If
    End Sub

    '' Fully compliant with Option Strict On and under the 25-line limit
    'Private Sub OnToolButtonClick(ByVal sender As Object, ByVal e As EventArgs)
    '    Dim btn As Button = TryCast(sender, Button)

    '    If btn IsNot Nothing AndAlso btn.Tag IsNot Nothing Then
    '        Dim metadata As Tuple(Of CanvasTool, LogicGateType) = DirectCast(btn.Tag, Tuple(Of CanvasTool, LogicGateType))

    '        ' If the clicked button is a Logic Gate component, kick off a native WinForms DragDrop loop
    '        If metadata.Item1 = CanvasTool.LogicGate Then
    '            ' Package the gate enum token payload securely, letting Windows handle the drag sequence
    '            btn.DoDragDrop(metadata.Item2, DragDropEffects.Copy)
    '        End If
    '    End If
    'End Sub

End Class

Public NotInheritable Class ToolSelectedEventArgs
    Inherits EventArgs
    Public ReadOnly SelectedTool As CanvasTool
    Public ReadOnly TargetGateType As LogicGateType

    Public Sub New(ByVal tool As CanvasTool, ByVal gateType As LogicGateType)
        Me.SelectedTool = tool
        Me.TargetGateType = gateType
    End Sub
End Class

