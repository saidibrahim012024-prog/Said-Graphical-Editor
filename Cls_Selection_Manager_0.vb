' Target File: Cls_Selection_Manager.vb
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Collections.Generic
Imports System.Drawing

Public NotInheritable Class Cls_Selection_Manager

    ' Target File: Cls_Selection_Manager.vb (Active Focus Shape Extraction)
    ' Explicit configuration locked for file scope | Option Strict On | Explicit Binding
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    ''' <summary>
    ''' Extracts the primary active focus shape component from the selection cache array registry.
    ''' FIXED: Added explicit member method to permanently clear view space compilation blocks.
    ''' </summary>
    Public Function TryGetActiveFocus() As Cls_Base_Shape
        ' If selection matrices are empty, return nothing to safely short-circuit parent threads
        If Me._selectedShapes IsNot Nothing AndAlso Me._selectedShapes.Count > 0 Then
            ' Return the first locked element index tracking focus authority natively
            Return Me._selectedShapes(0)
        End If
        Return Nothing
    End Function


    Private ReadOnly _selectedShapes As New List(Of Cls_Base_Shape)()
    Private _isMarqueeSelecting As Boolean = False
    Private _marqueeStartPoint As PointF
    Private _marqueeCurrentPoint As PointF


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

#End Region

    ''' <summary>
    ''' Type-safely registers a schematic shape component into the active selection registry list.
    ''' Satisfies Option Strict On and resides safely underneath your 25 operational line limit.
    ''' </summary>
    Public Sub SelectShape(ByVal shape As Cls_Base_Shape, ByVal appendSelection As Boolean)
        If shape Is Nothing Then Exit Sub

        If Not appendSelection Then
            Me._selectedShapes.Clear()
        End If

        ' Prevent duplicate component reference allocations within the tracking array
        If Not Me._selectedShapes.Contains(shape) Then
            Me._selectedShapes.Add(shape)
        End If
    End Sub

    Public Function Contains(ByVal shape As Cls_Base_Shape) As Boolean
        Return Me._selectedShapes.Contains(shape)
    End Function

    Public Sub Clear()
        Me._selectedShapes.Clear()
    End Sub

    ''' <summary>
    ''' Checks if a specific logic component shape is currently registered inside the active selection group.
    ''' </summary>
    Public Function IsShapeSelected(ByVal shape As Cls_Base_Shape) As Boolean
        If shape Is Nothing OrElse Me._selectedShapes Is Nothing Then Return False

        ' Return the lookup evaluation boolean condition natively
        Return Me._selectedShapes.Contains(shape)
    End Function

    ''' <summary>
    ''' Explicitly removes a single component from the selection cache array registry.
    ''' </summary>
    Public Sub DeselectShape(ByVal shape As Cls_Base_Shape)
        If shape IsNot Nothing AndAlso Me._selectedShapes IsNot Nothing Then
            Me._selectedShapes.Remove(shape)
        End If
    End Sub


    'xxxxxxxxxxxxxxxxxxxxxx
    '    Public ReadOnly Property SelectedShapes As List(Of Cls_Base_Shape)
    '    Get
    '        Return _selectedShapes
    '    End Get
    'End Property
    'Public ReadOnly Property Count As Integer
    '    Get
    '        Return _selectedShapes.Count
    '    End Get
    'End Property
    Public Property IsMarqueeSelecting As Boolean
        Get
            Return _isMarqueeSelecting
        End Get
        Set(value As Boolean)
            _isMarqueeSelecting = value
        End Set
    End Property
    Public Property MarqueeStartPoint As PointF
        Get
            Return _marqueeStartPoint
        End Get
        Set(value As PointF)
            _marqueeStartPoint = value
        End Set
    End Property
    Public Property MarqueeCurrentPoint As PointF
        Get
            Return _marqueeCurrentPoint
        End Get
        Set(value As PointF)
            _marqueeCurrentPoint = value
        End Set
    End Property

    Public Sub New()
    End Sub

    'Public Sub Clear()
    '    _selectedShapes.Clear()
    'End Sub

    'Public Sub SelectShape(shape As Cls_Base_Shape, toggleMode As Boolean)
    '    If shape Is Nothing Then Exit Sub
    '    If toggleMode Then
    '        If _selectedShapes.Contains(shape) Then _selectedShapes.Remove(shape) Else _selectedShapes.Add(shape)
    '    ElseIf Not _selectedShapes.Contains(shape) Then
    '        _selectedShapes.Clear()
    '        _selectedShapes.Add(shape)
    '    End If
    'End Sub

    ' Target File: Cls_Selection_Manager.vb (Unified Schematic Tracking Pass)
    ' Explicit configuration locked for file scope | Option Strict On | Explicit Binding
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    ''' <summary>
    ''' Sweeps across the active schematic components collection to finalize marquee crossing boundaries.
    ''' Fully type-safe under Option Strict On and resides safely under the 25-line limit.
    ''' </summary>
    Public Sub FinalizeMarqueeSelection(ByVal components As List(Of Cls_Base_Shape), ByVal isShiftDown As Boolean)
        If Not isShiftDown Then
            Me.Clear() ' Flush old selection arrays if Shift isn't held down
        End If

        If components IsNot Nothing Then
            Dim marqueeRect As RectangleF = Me.GetMarqueeRectangle()

            ' Scan the live schematic components collection directly
            For Each gate As Cls_Base_Shape In components
                ' If the gate component intersects the selection bounds window, register it
                If marqueeRect.IntersectsWith(gate.Bounds) Then
                    Me.SelectShape(gate, True)
                End If
            Next
        End If
    End Sub

    'Public Sub FinalizeMarqueeSelection(allShapes As List(Of Cls_Base_Shape), toggleMode As Boolean)
    '    _isMarqueeSelecting = False
    '    If Not toggleMode Then _selectedShapes.Clear()

    '    Dim marqueeBox As RectangleF = GetMarqueeRectangle()
    '    For Each shape As Cls_Base_Shape In allShapes
    '        ' Dim shapeBox As New RectangleF(Math.Min(shape.StartPoint.X, shape.EndPoint.X), Math.Min(shape.StartPoint.Y, shape.EndPoint.Y), Math.Abs(shape.StartPoint.X - shape.EndPoint.X), Math.Abs(shape.StartPoint.Y - shape.EndPoint.Y))
    '        If marqueeBox.Contains(shape.Bounds) AndAlso Not _selectedShapes.Contains(shape) Then
    '            _selectedShapes.Add(shape)
    '        End If
    '    Next
    'End Sub

    Public Function GetMarqueeRectangle() As RectangleF
        Dim x As Single = Math.Min(_marqueeStartPoint.X, _marqueeCurrentPoint.X)
        Dim y As Single = Math.Min(_marqueeStartPoint.Y, _marqueeCurrentPoint.Y)
        Return New RectangleF(x, y, Math.Abs(_marqueeStartPoint.X - _marqueeCurrentPoint.X), Math.Abs(_marqueeStartPoint.Y - _marqueeCurrentPoint.Y))
    End Function

    'Public Function Contains(shape As Cls_Base_Shape) As Boolean
    '    Return _selectedShapes.Contains(shape)
    'End Function
End Class
