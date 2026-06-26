' Target File: Cls_Selection_Manager.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Said Rules Compliance: Zero Guessing | No Non-Existent Token Variables | Locked Classes Only
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Collections.Generic
Imports System.Drawing

Public NotInheritable Class Cls_Selection_Manager

    Private ReadOnly _selectedShapes As New List(Of Cls_Base_Shape)()
    Private _isMarqueeSelecting As Boolean = False
    Private _marqueeStartPoint As PointF
    Private _marqueeCurrentPoint As PointF

    Public Sub New()
    End Sub

#Region "Properties"

    Public ReadOnly Property SelectedShapes() As List(Of Cls_Base_Shape)
        Get
            Return Me._selectedShapes
        End Get
    End Property

    Public ReadOnly Property Count() As Integer
        Get
            Return Me._selectedShapes.Count
        End Get
    End Property

    Public Property IsMarqueeSelecting() As Boolean
        Get
            Return Me._isMarqueeSelecting
        End Get
        Set(ByVal value As Boolean)
            Me._isMarqueeSelecting = value
        End Set
    End Property

    Public Property MarqueeStartPoint() As PointF
        Get
            Return Me._marqueeStartPoint
        End Get
        Set(ByVal value As PointF)
            Me._marqueeStartPoint = value
        End Set
    End Property

    Public Property MarqueeCurrentPoint() As PointF
        Get
            Return Me._marqueeCurrentPoint
        End Get
        Set(ByVal value As PointF)
            Me._marqueeCurrentPoint = value
        End Set
    End Property

#End Region

    Public Function TryGetActiveFocus() As Cls_Base_Shape
        If Me._selectedShapes IsNot Nothing AndAlso Me._selectedShapes.Count > 0 Then
            Return Me._selectedShapes(0)
        End If
        Return Nothing
    End Function

    Public Sub SelectShape(ByVal shape As Cls_Base_Shape, ByVal appendSelection As Boolean)
        If shape Is Nothing Then Exit Sub

        If Not appendSelection Then
            Me.Clear()
        End If

        If Not Me._selectedShapes.Contains(shape) Then
            Me._selectedShapes.Add(shape)
            shape.IsSelected = True ' STRATEGY HOOK: Sync the underlying data flag
        End If
    End Sub

    Public Function Contains(ByVal shape As Cls_Base_Shape) As Boolean
        Return Me._selectedShapes.Contains(shape)
    End Function

    Public Sub Clear()
        ' STRATEGY HOOK: Turn off selection flags on existing items before clearing array
        For i As Integer = 0 To Me._selectedShapes.Count - 1
            If Me._selectedShapes(i) IsNot Nothing Then Me._selectedShapes(i).IsSelected = False
        Next
        Me._selectedShapes.Clear()
    End Sub

    Public Function IsShapeSelected(ByVal shape As Cls_Base_Shape) As Boolean
        If shape Is Nothing OrElse Me._selectedShapes Is Nothing Then Return False
        Return Me._selectedShapes.Contains(shape)
    End Function

    Public Sub DeselectShape(ByVal shape As Cls_Base_Shape)
        If shape IsNot Nothing AndAlso Me._selectedShapes IsNot Nothing Then
            Me._selectedShapes.Remove(shape)
            shape.IsSelected = False ' STRATEGY HOOK: Sync the underlying data flag
        End If
    End Sub

    Public Sub FinalizeMarqueeSelection(ByVal components As List(Of Cls_Base_Shape), ByVal isShiftDown As Boolean)
        If Not isShiftDown Then
            Me.Clear()
        End If

        If components IsNot Nothing Then
            Dim marqueeRect As RectangleF = Me.GetMarqueeRectangle()

            For Each gate As Cls_Base_Shape In components
                ' RULE 1: Sweep the bounding box of shapes to match selection
                If marqueeRect.IntersectsWith(gate.Bounds) Then
                    Me.SelectShape(gate, True)
                End If
            Next
        End If
    End Sub

    Public Function GetMarqueeRectangle() As RectangleF
        Dim x As Single = Math.Min(_marqueeStartPoint.X, _marqueeCurrentPoint.X)
        Dim y As Single = Math.Min(_marqueeStartPoint.Y, _marqueeCurrentPoint.Y)
        Return New RectangleF(x, y, Math.Abs(_marqueeStartPoint.X - _marqueeCurrentPoint.X), Math.Abs(_marqueeStartPoint.Y - _marqueeCurrentPoint.Y))
    End Function
End Class

