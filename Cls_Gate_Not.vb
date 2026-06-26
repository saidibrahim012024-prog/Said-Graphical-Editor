' Target File: Cls_Gate_Not.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Drawing

Public NotInheritable Class Cls_Gate_Not
    Inherits Cls_Base_Shape

    Public Overrides ReadOnly Property GateType() As LogicGateType
        Get
            Return LogicGateType.NotGate
        End Get
    End Property

    Public Sub New(ByVal loc As PointF)
        MyBase.New(loc)
    End Sub

    Protected Overrides Sub SynchronizeInputPorts(ByVal r As RectangleF, ByVal centerY As Single)
        Me.InPorts.Clear()
        Me.InPorts.Add(New PointF(r.X, centerY))
    End Sub

    ' Target File: Cls_Gate_Not.vb (Dynamic Vector Alignment Pass)
    ' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable

    Public Overrides Sub ExecuteDrawGeometry(ByVal g As Graphics)
        If g Is Nothing Then Exit Sub
        Dim r As RectangleF = Me.Bounds
        Dim centerY As Single = r.Y + (r.Height / 2.0F)
        Dim bubbleDiameter As Single = 8.0F

        Using mainPen As New Pen(Me.StrokeColor, Me.Thickness)
            ' FIXED: Paint inverter triangle coordinates relative to the live 'r' bounding edges
            g.DrawLine(mainPen, r.X, r.Y, r.X, r.Bottom)
            g.DrawLine(mainPen, r.X, r.Y, r.Right - bubbleDiameter, centerY)
            g.DrawLine(mainPen, r.X, r.Bottom, r.Right - bubbleDiameter, centerY)

            g.DrawEllipse(mainPen, r.Right - bubbleDiameter, centerY - (bubbleDiameter / 2.0F), bubbleDiameter, bubbleDiameter)
            Me.PaintConnectionTerminalPins(g)
        End Using
    End Sub

    'Public Overrides Sub ExecuteDrawGeometry(ByVal g As Graphics)
    '    If g Is Nothing Then Exit Sub
    '    Dim r As RectangleF = Me.Bounds
    '    Dim centerY As Single = r.Y + (r.Height / 2.0F)
    '    Dim bubbleDiameter As Single = 8.0F
    '    Dim bubbleRadius As Single = bubbleDiameter / 2.0F

    '    Using mainPen As New Pen(Me.StrokeColor, Me.Thickness)
    '        g.DrawLine(mainPen, r.X, r.Y, r.X, r.Bottom)
    '        g.DrawLine(mainPen, r.X, r.Y, r.Right - bubbleDiameter, centerY)
    '        g.DrawLine(mainPen, r.X, r.Bottom, r.Right - bubbleDiameter, centerY)

    '        g.DrawEllipse(mainPen, r.Right - bubbleDiameter, centerY - bubbleRadius, bubbleDiameter, bubbleDiameter)
    '        Me.PaintConnectionTerminalPins(g)
    '    End Using
    'End Sub

    Private Sub PaintConnectionTerminalPins(ByVal g As Graphics)
        Using portBrush As New SolidBrush(Color.IndianRed)
            Dim dotDiameter As Single = 6.0F
            Dim radius As Single = dotDiameter / 2.0F
            g.FillEllipse(portBrush, Me.OutPort.X - radius, Me.OutPort.Y - radius, dotDiameter, dotDiameter)
            For Each port As PointF In Me.InPorts
                g.FillEllipse(portBrush, port.X - radius, port.Y - radius, dotDiameter, dotDiameter)
            Next
        End Using
    End Sub
End Class
