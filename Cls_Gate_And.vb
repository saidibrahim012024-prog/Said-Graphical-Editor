' Target File: Cls_Gate_And.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Drawing
Imports System.Drawing.Drawing2D

Public NotInheritable Class Cls_Gate_And
    Inherits Cls_Base_Shape

    Public Overrides ReadOnly Property GateType() As LogicGateType
        Get
            Return LogicGateType.AndGate
        End Get
    End Property

    Public Sub New(ByVal loc As PointF)
        MyBase.New(loc)
    End Sub

    Protected Overrides Sub SynchronizeInputPorts(ByVal r As RectangleF, ByVal centerY As Single)
        Me.InPorts.Clear()
        Me.InPorts.Add(New PointF(r.X, r.Y + (r.Height * 0.25F)))
        Me.InPorts.Add(New PointF(r.X, r.Y + (r.Height * 0.75F)))
    End Sub

    ' Target File: Cls_Gate_And.vb (Dynamic Vector Alignment Pass)
    ' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    Public Overrides Sub ExecuteDrawGeometry(ByVal g As Graphics)
        If g Is Nothing Then Exit Sub

        ' Fetch the live, synchronized absolute world boundary tracking rectangle
        Dim r As RectangleF = Me.Bounds

        Using mainPen As New Pen(Me.StrokeColor, Me.Thickness)
            Using path As New GraphicsPath()
                ' FIXED: Paint every vector segment relative to the live, shifting 'r' bounds!
                path.AddLine(r.X, r.Bottom, r.X, r.Y)
                path.AddLine(r.X, r.Y, r.X + (r.Width * 0.4F), r.Y)
                path.AddArc(r.X + (r.Width * 0.1F), r.Y, r.Width * 0.8F, r.Height, -90.0F, 180.0F)
                path.AddLine(r.X + (r.Width * 0.4F), r.Bottom, r.X, r.Bottom)
                path.CloseFigure()

                g.DrawPath(mainPen, path)
            End Using

            Me.PaintConnectionTerminalPins(g)
        End Using
    End Sub

    'Public Overrides Sub ExecuteDrawGeometry(ByVal g As Graphics)
    '    If g Is Nothing Then Exit Sub
    '    Dim r As RectangleF = Me.Bounds

    '    Using mainPen As New Pen(Me.StrokeColor, Me.Thickness)
    '        Using path As New GraphicsPath()
    '            path.AddLine(r.X, r.Bottom, r.X, r.Y)
    '            path.AddLine(r.X, r.Y, r.X + (r.Width * 0.4F), r.Y)
    '            path.AddArc(r.X + (r.Width * 0.1F), r.Y, r.Width * 0.8F, r.Height, -90.0F, 180.0F)
    '            path.AddLine(r.X + (r.Width * 0.4F), r.Bottom, r.X, r.Bottom)
    '            path.CloseFigure()
    '            g.DrawPath(mainPen, path)
    '        End Using

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
