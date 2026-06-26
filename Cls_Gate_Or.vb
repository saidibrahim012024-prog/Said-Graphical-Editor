' Target File: Cls_Gate_Or.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Drawing
Imports System.Drawing.Drawing2D

Public NotInheritable Class Cls_Gate_Or
    Inherits Cls_Base_Shape

#Region "Properties & Constructor Initialization"

    Public Overrides ReadOnly Property GateType() As LogicGateType
        Get
            Return LogicGateType.OrGate
        End Get
    End Property

    Public Sub New(ByVal loc As PointF)
        MyBase.New(loc)
        ' Enforce standard default CAD layout footprint dimensions
        Me.Size = New SizeF(80.0F, 50.0F)
    End Sub

#End Region

#Region "Authoritative Port Pin Synchronization"

    ''' <summary>
    ''' Recalculates electrical pin vectors dynamically along the curved perimeters.
    ''' </summary>
    Protected Overrides Sub SynchronizeInputPorts(ByVal r As RectangleF, ByVal centerY As Single)
        Me.InPorts.Clear()

        ' For curved MIL-SPEC backplanes, input pins insert slightly right to meet the inner arc line
        Dim curveInsetX As Single = r.X + (r.Width * 0.1F)

        ' Distribute Upper and Lower input pin nodes symmetrically
        Me.InPorts.Add(New PointF(curveInsetX, r.Y + (r.Height * 0.25F)))
        Me.InPorts.Add(New PointF(curveInsetX, r.Y + (r.Height * 0.75F)))
    End Sub

#End Region

#Region "Vector Geometry Graphics Rendering Pipeline"

    ''' <summary>
    ''' Constructs the traditional MIL-SPEC curved OR gate vector symbol geometry path.
    ''' </summary>
    Public Overrides Sub ExecuteDrawGeometry(ByVal g As Graphics)
        If g Is Nothing Then Exit Sub
        Dim r As RectangleF = Me.Bounds

        Using mainPen As New Pen(Me.StrokeColor, Me.Thickness)
            Using path As New GraphicsPath()
                ' 1. Construct the curved input backplane arc chord segment
                ' A bounded chord sweeping through -30 to 60 degrees captures the traditional concave arc
                path.AddArc(r.X - (r.Width * 0.3F), r.Y, r.Width * 0.8F, r.Height, -30.0F, 60.0F)

                ' 2. Formulate the upper parabolic nose curve path using a high-precision spline
                path.AddBezier(New PointF(r.X + (r.Width * 0.4F), r.Y),
                               New PointF(r.X + (r.Width * 0.7F), r.Y + (r.Height * 0.05F)),
                               New PointF(r.X + (r.Width * 0.9F), r.Y + (r.Height * 0.3F)),
                               New PointF(r.Right, r.Y + (r.Height / 2.0F)))

                ' 3. Formulate the lower parabolic nose curve path backwards to the bottom flank corner
                path.AddBezier(New PointF(r.Right, r.Y + (r.Height / 2.0F)),
                               New PointF(r.X + (r.Width * 0.9F), r.Bottom - (r.Height * 0.3F)),
                               New PointF(r.X + (r.Width * 0.7F), r.Bottom - (r.Height * 0.05F)),
                               New PointF(r.X + (r.Width * 0.4F), r.Bottom))

                path.CloseFigure()
                g.DrawPath(mainPen, path)
            End Using

            ' 4. Overlay red pin dot terminal markers on top of the vector lines
            Me.PaintConnectionTerminalPins(g)
        End Using
    End Sub

    ''' <summary>
    ''' Segregated graphics helper to draw electrical pin dots under your 25 operational line maximum limit.
    ''' </summary>
    Private Sub PaintConnectionTerminalPins(ByVal g As Graphics)
        Using portBrush As New SolidBrush(Color.IndianRed)
            Dim dotDiameter As Single = 6.0F
            Dim halfRadius As Single = dotDiameter / 2.0F

            ' Draw standard electrical output node point marker on the far right edge intersection
            g.FillEllipse(portBrush, Me.OutPort.X - halfRadius, Me.OutPort.Y - halfRadius, dotDiameter, dotDiameter)

            ' Draw all vertically distributed input node points sequentially
            For Each port As PointF In Me.InPorts
                g.FillEllipse(portBrush, port.X - halfRadius, port.Y - halfRadius, dotDiameter, dotDiameter)
            Next
        End Using
    End Sub

#End Region
End Class

