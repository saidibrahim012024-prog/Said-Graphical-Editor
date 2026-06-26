' Target File: Cls_Base_Shape.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | MustInherit
' Said Rules Compliance: Zero Guessing | No Non-Existent Token Variables | Locked Classes Only
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing

Public MustInherit Class Cls_Base_Shape

    Private _location As PointF
    Private _size As SizeF

    Private _strokeColor As Color
    Private _thickness As Single
    Private _rotationAngle As Single
    Private _isSelected As Boolean ' <--- RESTORED STRATEGY TOKEN

    Protected ReadOnly InPorts As New List(Of PointF)()
    Protected OutPort As PointF

    Public Sub New(ByVal initialLocation As PointF)
        Me._strokeColor = Color.Black
        Me._thickness = 2.0F
        Me._rotationAngle = 0.0F
        Me._size = New SizeF(80.0F, 50.0F)
        Me._isSelected = False
        Me.Location = initialLocation
    End Sub

#Region "Properties"

    <Category("Selection State"), Description("Indicates if this primitive component is active in the current selection group.")>
    Public Property IsSelected() As Boolean
        Get
            Return Me._isSelected
        End Get
        Set(ByVal value As Boolean)
            Me._isSelected = value
        End Set
    End Property

    <Category("Placement Matrix"), Description("The absolute insertion position coordinate of the component.")>
    Public Property Location() As PointF
        Get
            Return Me._location
        End Get
        Set(ByVal value As PointF)
            Me._location = value
            Me.UpdatePortNodePlacements()
        End Set
    End Property

    <Category("Placement Matrix"), Description("The width and height footprint bounds dimensions.")>
    Public Property Size() As SizeF
        Get
            Return Me._size
        End Get
        Set(ByVal value As SizeF)
            Dim w As Single = Math.Max(40.0F, value.Width)
            Dim h As Single = Math.Max(30.0F, value.Height)
            Me._size = New SizeF(w, h)
            Me.UpdatePortNodePlacements()
        End Set
    End Property

    <Category("Placement Matrix"), Description("The global angular orientation constraint degree.")>
    Public Property RotationAngle() As Single
        Get
            Return Me._rotationAngle
        End Get
        Set(ByVal value As Single)
            Me._rotationAngle = value
        End Set
    End Property

    <Category("Visual Styling"), Description("The high-contrast color used to draw the component perimeters.")>
    Public Property StrokeColor() As Color
        Get
            Return Me._strokeColor
        End Get
        Set(ByVal value As Color)
            Me._strokeColor = value
        End Set
    End Property

    <Category("Visual Styling"), Description("The layout pen pixel width thickness.")>
    Public Property Thickness() As Single
        Get
            Return Me._thickness
        End Get
        Set(ByVal value As Single)
            Me._thickness = value
        End Set
    End Property

    <Category("Placement Matrix"), Description("The explicit bounding container wrapping the component geometry.")>
    Public ReadOnly Property Bounds() As RectangleF
        Get
            Dim secureW As Single = If(Me._size.Width <= 0.0F, 80.0F, Me._size.Width)
            Dim secureH As Single = If(Me._size.Height <= 0.0F, 50.0F, Me._size.Height)
            Return New RectangleF(Me._location.X, Me._location.Y, secureW, secureH)
        End Get
    End Property

#End Region

    Public Property X() As Single
        Get
            Return _location.X
        End Get
        Set(ByVal value As Single)
            _location.X = value
        End Set
    End Property

    Public Property Y() As Single
        Get
            Return _location.Y
        End Get
        Set(ByVal value As Single)
            _location.Y = value
        End Set
    End Property

#Region "Ports"
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

    Public Sub UpdatePortNodePlacements()
        Dim r As RectangleF = Me.Bounds
        Dim centerY As Single = r.Y + (r.Height / 2.0F)
        Me.OutPort = New PointF(r.Right, centerY)
        Me.SynchronizeInputPorts(r, centerY)
    End Sub
#End Region

#Region "Overrides"
    Protected MustOverride Sub SynchronizeInputPorts(ByVal r As RectangleF, ByVal centerY As Single)
    Public MustOverride Sub ExecuteDrawGeometry(ByVal g As Graphics)
    Public MustOverride ReadOnly Property GateType() As LogicGateType
#End Region

#Region "Render"
    Public Sub Render(ByVal g As Graphics)
        If g Is Nothing Then Exit Sub
        Me.ExecuteDrawGeometry(g)
    End Sub
#End Region

#Region "Geometry"
    Public Function HitTest(ByVal worldPoint As PointF) As Boolean
        Return Me.Bounds.Contains(worldPoint)
    End Function

    Public Sub ApplyResizeTransform(ByVal targetWidth As Single, ByVal targetHeight As Single)
        Me.Size = New SizeF(targetWidth, targetHeight)
    End Sub

    Public Sub ApplyDeltaTransform(ByVal dx As Single, ByVal dy As Single)
        Dim nextX As Single = Me._location.X + dx
        Dim nextY As Single = Me._location.Y + dy
        Me.Location = New PointF(nextX, nextY)
    End Sub

    Public Function CalculateGeometricCenter() As PointF
        Dim r As RectangleF = Me.Bounds
        Dim cX As Single = r.X + (r.Width / 2.0F)
        Dim cY As Single = r.Y + (r.Height / 2.0F)
        Return New PointF(cX, cY)
    End Function

    Public Sub ApplyAtomicGeometryTransform(ByVal nextLocation As PointF, ByVal nextWidth As Single, ByVal nextHeight As Single)
        Dim cleanW As Single = Math.Max(40.0F, nextWidth)
        Dim cleanH As Single = Math.Max(30.0F, nextHeight)
        Me._location = nextLocation
        Me._size = New SizeF(cleanW, cleanH)
        Me.UpdatePortNodePlacements()
    End Sub
#End Region

End Class

