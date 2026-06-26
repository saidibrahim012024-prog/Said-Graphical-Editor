' Target File: Cls_Base_Shape.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | MustInherit
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing

Public MustInherit Class Cls_Base_Shape
    Private _location As PointF
    Private _size As SizeF = New SizeF(80.0F, 50.0F)

    Protected ReadOnly InPorts As New List(Of PointF)()
    Protected OutPort As PointF

    Private _StrokeColor As Color = Color.Black
    Private _Thickness As Single = 2.0F
    Private _RotationAngle As Single = 0.0F

#Region "Properties"

    Public Property Location() As PointF
        Get
            Return Me._location
        End Get
        Set(ByVal value As PointF)
            Me._location = value
            Me.UpdatePortNodePlacements()
        End Set
    End Property

    Public ReadOnly Property Bounds() As RectangleF
        Get
            Return New RectangleF(Me._location.X, Me._location.Y, Me._size.Width, Me._size.Height)
        End Get
    End Property

    Public ReadOnly Property InputPorts() As IList(Of PointF)
        Get
            Return Me.InPorts.AsReadOnly()
        End Get
    End Property

    Public ReadOnly Property OutputPort() As PointF
        Get
            Return Me.OutPort
        End Get
    End Property

    Public MustOverride ReadOnly Property GateType() As LogicGateType

#End Region

#Region "Constructors"

    ' Target File: Cls_Base_Shape.vb (Constructor Size Initialization Pass)
    ' Explicit configuration locked for file scope | Option Strict On | Explicit Binding
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

#Region "Constructors"

    Public Sub New(ByVal initialLocation As PointF)
        Me._location = initialLocation
        ' FIXED: Explicitly force standard footprint dimensions upon base instantiation
        Me._size = New SizeF(80.0F, 50.0F)
        Me.UpdatePortNodePlacements()
    End Sub

    Public Sub New(ByVal initialLocation As PointF, ByVal color As Color, ByVal width As Single, Optional ByVal angle As Single = 0.0F)
        Me._location = initialLocation
        Me._size = New SizeF(80.0F, 50.0F) ' FIXED: Locked size backing field initialization
        Me._StrokeColor = color
        Me._Thickness = width
        Me._RotationAngle = angle
        Me.UpdatePortNodePlacements()
    End Sub

#End Region


    'Public Sub New(ByVal initialLocation As PointF)
    '    Me._location = initialLocation
    '    Me.UpdatePortNodePlacements()
    'End Sub

    'Public Sub New(ByVal initialLocation As PointF, ByVal color As Color, ByVal width As Single, Optional ByVal angle As Single = 0.0F)
    '    Me._location = initialLocation
    '    Me._StrokeColor = color
    '    Me._Thickness = width
    '    Me._RotationAngle = angle
    '    Me.UpdatePortNodePlacements()
    'End Sub

#End Region

    Public Sub UpdatePortNodePlacements()
        Dim r As RectangleF = Me.Bounds
        Dim centerY As Single = Convert.ToSingle(Math.Round(Convert.ToDouble(r.Y + (r.Height / 2.0F))))
        Me.OutPort = New PointF(r.Right, centerY)
        Me.SynchronizeInputPorts(r, centerY)
    End Sub

    Protected MustOverride Sub SynchronizeInputPorts(ByVal r As RectangleF, ByVal centerY As Single)
    Public MustOverride Sub Render(ByVal g As Graphics, ByVal p As Pen)

    Public Function HitTest(ByVal worldPoint As PointF) As Boolean
        Return Me.Bounds.Contains(worldPoint)
    End Function

#Region "Legacy Geometric & Property Grid Mappings"

    '<Category("Geometric Matrix"), Description("The absolute starting vector coordinate point.")>
    'Public ReadOnly Property StartPoint() As PointF
    '    Get
    '        Return Me._location
    '    End Get
    'End Property

    '<Category("Geometric Matrix"), Description("The absolute ending vector coordinate point.")>
    'Public ReadOnly Property EndPoint() As PointF
    '    Get
    '        Return New PointF(Me.Bounds.Right, Me.Bounds.Bottom)
    '    End Get
    'End Property

    <Category("Geometric Matrix"), Description("The global structural angular rotation degree.")>
    Public Property RotationAngle() As Single
        Get
            Return Me._RotationAngle
        End Get
        Set(ByVal value As Single)
            Me._RotationAngle = value
        End Set
    End Property

    <Category("Visual Styling"), Description("The high-contrast stroke color vector outline.")>
    Public Property StrokeColor() As Color
        Get
            Return Me._StrokeColor
        End Get
        Set(ByVal value As Color)
            Me._StrokeColor = value
        End Set
    End Property

    <Category("Visual Styling"), Description("The physical layout pixel thickness line footprint.")>
    Public Property Thickness() As Single
        Get
            Return Me._Thickness
        End Get
        Set(ByVal value As Single)
            Me._Thickness = value
        End Set
    End Property

    Public ReadOnly Property GeometricCenter() As PointF
        Get
            Return Me.CalculateGeometricCenter()
        End Get
    End Property

#End Region

#Region "Rendering & Transformation Core Subroutines"

    Public Sub Render(ByVal g As Graphics)
        Me.ExecuteDrawGeometry(g)
    End Sub

    Public Sub ExecuteDrawGeometry(ByVal g As Graphics)
        If g Is Nothing Then Exit Sub
        Using pen As New Pen(Me._StrokeColor, Me._Thickness)
            Me.Render(g, pen)
        End Using
    End Sub

    ''' <summary>
    ''' High-precision center calculator utilizing explicit rounding to prevent rendering jitter.
    ''' </summary>
    Public Function CalculateGeometricCenter() As PointF
        Dim b As RectangleF = Me.Bounds
        Dim cX As Single = Convert.ToSingle(Math.Round(Convert.ToDouble(b.X + (b.Width / 2.0F))))
        Dim cY As Single = Convert.ToSingle(Math.Round(Convert.ToDouble(b.Y + (b.Height / 2.0F))))
        Return New PointF(cX, cY)
    End Function

    Public Sub ApplyDeltaTransform(ByVal dx As Single, ByVal dy As Single)
        Dim currentX As Single = Me._location.X
        Dim currentY As Single = Me._location.Y
        Me.Location = New PointF(currentX + dx, currentY + dy)
    End Sub



    ''' <summary>
    ''' High-precision rotation transformer satisfying strict type casting rules.
    ''' </summary>
    Public Function GetRotatedPoint(ByVal localPoint As PointF) As PointF
        Dim center As PointF = Me.CalculateGeometricCenter()
        If Me._RotationAngle = 0.0F Then Return localPoint

        Dim radians As Double = Convert.ToDouble(Me._RotationAngle) * (Math.PI / 180.0)
        Dim cosTheta As Double = Math.Cos(radians)
        Dim sinTheta As Double = Math.Sin(radians)

        Dim dx As Double = Convert.ToDouble(localPoint.X) - Convert.ToDouble(center.X)
        Dim dy As Double = Convert.ToDouble(localPoint.Y) - Convert.ToDouble(center.Y)

        Dim rotatedX As Single = Convert.ToSingle((dx * cosTheta) - (dy * sinTheta) + Convert.ToDouble(center.X))
        Dim rotatedY As Single = Convert.ToSingle((dx * sinTheta) + (dy * cosTheta) + Convert.ToDouble(center.Y))

        Return New PointF(rotatedX, rotatedY)
    End Function

#End Region

#Region "Component Resizing Pass"

    Public Sub ApplyResizeTransform(ByVal targetWidth As Single, ByVal targetHeight As Single)
        Dim cleanW As Single = Math.Max(40.0F, targetWidth)
        Dim cleanH As Single = Math.Max(30.0F, targetHeight)

        Me._size = New SizeF(cleanW, cleanH)
        Me.UpdatePortNodePlacements()
    End Sub

#End Region
End Class
