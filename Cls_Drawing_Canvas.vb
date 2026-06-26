' Target File: Cls_Drawing_Canvas.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Collections.Generic
Imports System.Drawing

Public NotInheritable Class Cls_Drawing_Canvas
    Private ReadOnly _viewport As Cls_Viewport

    ' FIXED: The SINGLE, authoritative master data repository list for all active schematic gates
    Private ReadOnly _schematicComponents As New List(Of Cls_Base_Shape)()

#Region "Properties"

    Public ReadOnly Property SchematicComponents() As List(Of Cls_Base_Shape)
        Get
            Return Me._schematicComponents
        End Get
    End Property

#End Region

    Public Sub New(ByVal parentViewport As Cls_Viewport)
        Me._viewport = parentViewport
    End Sub

    ''' <summary>
    ''' Performs rotation-safe and zoom-invariant hit testing using local origin space boundaries.
    ''' Fully type-safe under Option Strict On and resides safely under the 25-line limit.
    ''' </summary>
    Public Function HitTestShapes(ByVal worldPoint As PointF, ByVal transformer As Cls_Coordinate_Transformer) As Cls_Base_Shape
        If Me._schematicComponents Is Nothing OrElse transformer Is Nothing Then Return Nothing

        ' Scan backwards from the top-most z-order element down to the background grid
        For i As Integer = Me._schematicComponents.Count - 1 To 0 Step -1
            Dim gate As Cls_Base_Shape = Me._schematicComponents(i)

            ' 1. Project the world mouse coordinate cleanly into the gate's local centered grid space
            Dim localMouse As PointF = transformer.TransformWorldToLocalShapeSpace(worldPoint, gate)

            ' 2. Construct a local footprint bounding box centered precisely around origin (0,0)
            Dim halfW As Single = gate.Bounds.Width / 2.0F
            Dim halfH As Single = gate.Bounds.Height / 2.0F
            Dim localBounds As New RectangleF(-halfW, -halfH, gate.Bounds.Width, gate.Bounds.Height)

            ' 3. Check boundary intersection against the corrected local envelope matrix
            If localBounds.Contains(localMouse) Then
                Return gate ' Match found! Instantly returns the targeted logic block reference
            End If
        Next

        Return Nothing

    End Function

    ''' <summary>
    ''' Clears a specific subset collection array of schematic gates from the primary database repository.
    ''' FIXED: Provides an explicit membership definition to permanently resolve compilation blocks.
    ''' </summary>
    Public Sub PurgeShapesGroup(ByVal shapesToPurge As IList(Of Cls_Base_Shape))
        If shapesToPurge Is Nothing OrElse shapesToPurge.Count = 0 Then
            Exit Sub
        End If

        ' Iterate through the provided deletion tracking collection array list
        For Each gate As Cls_Base_Shape In shapesToPurge
            ' Type-safely remove each component out of the single master tracking collection repository
            If Me._schematicComponents.Contains(gate) Then
                Me._schematicComponents.Remove(gate)
            End If
        Next

    End Sub

    ''' <summary>
    ''' Iterates and paints the background shapes database model primitives safely within the active matrix transform context.
    ''' </summary>
    Public Sub RenderWorld(g As Graphics)
        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

        ' Aligned precisely with the single-parameter contract requirement
        For Each shape As Cls_Base_Shape In _schematicComponents
            shape.Render(g)
        Next
    End Sub


End Class
