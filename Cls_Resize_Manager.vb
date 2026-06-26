' Target File: Cls_Resize_Manager.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Drawing
Imports System.Windows.Forms

Public Enum ResizeHandle
    None
    TopLeft
    TopCenter
    TopRight
    MiddleLeft
    MiddleRight
    BottomLeft
    BottomCenter
    BottomRight
End Enum

Public NotInheritable Class Cls_Resize_Manager
    Private _activeHandle As ResizeHandle = ResizeHandle.None

    Public ReadOnly Property ActiveHandle() As ResizeHandle
        Get
            Return Me._activeHandle
        End Get
    End Property

    Public Sub LockActiveHandle(ByVal handle As ResizeHandle)
        Me._activeHandle = handle
    End Sub

    Public Function HitTestHandles(ByVal worldMouse As PointF, ByVal shape As Cls_Base_Shape, ByVal zoom As Single) As ResizeHandle
        If shape Is Nothing Then Return ResizeHandle.None
        Dim r As RectangleF = shape.Bounds
        Dim hSize As Single = 6.0F / zoom
        Dim half As Single = hSize / 2.0F

        If Me.CheckRect(worldMouse, r.X - half, r.Y - half, hSize) Then Return ResizeHandle.TopLeft
        If Me.CheckRect(worldMouse, r.Right - half, r.Y - half, hSize) Then Return ResizeHandle.TopRight
        If Me.CheckRect(worldMouse, r.X - half, r.Bottom - half, hSize) Then Return ResizeHandle.BottomLeft
        If Me.CheckRect(worldMouse, r.Right - half, r.Bottom - half, hSize) Then Return ResizeHandle.BottomRight
        Return Me.EvaluateCenterEdgeHandleHits(worldMouse, r, hSize, half)
    End Function

    Private Function EvaluateCenterEdgeHandleHits(ByVal p As PointF, ByVal r As RectangleF, ByVal s As Single, ByVal half As Single) As ResizeHandle
        Dim midX As Single = r.X + (r.Width / 2.0F)
        Dim midY As Single = r.Y + (r.Height / 2.0F)

        If Me.CheckRect(p, midX - half, r.Y - half, s) Then Return ResizeHandle.TopCenter
        If Me.CheckRect(p, midX - half, r.Bottom - half, s) Then Return ResizeHandle.BottomCenter
        If Me.CheckRect(p, r.X - half, midY - half, s) Then Return ResizeHandle.MiddleLeft
        If Me.CheckRect(p, r.Right - half, midY - half, s) Then Return ResizeHandle.MiddleRight
        Return ResizeHandle.None
    End Function

    Public Sub UpdateHoverCursor(ByVal worldMouse As PointF, ByVal shape As Cls_Base_Shape, ByVal zoom As Single, ByVal view As Control)
        If view Is Nothing Then Exit Sub
        Dim handle As ResizeHandle = Me.HitTestHandles(worldMouse, shape, zoom)

        Select Case handle
            Case ResizeHandle.TopLeft, ResizeHandle.BottomRight : view.Cursor = Cursors.SizeNWSE
            Case ResizeHandle.TopRight, ResizeHandle.BottomLeft : view.Cursor = Cursors.SizeNESW
            Case ResizeHandle.TopCenter, ResizeHandle.BottomCenter : view.Cursor = Cursors.SizeNS
            Case ResizeHandle.MiddleLeft, ResizeHandle.MiddleRight : view.Cursor = Cursors.SizeWE
            Case Else : view.Cursor = Cursors.Default
        End Select
    End Sub

    Public Sub ExecuteResizeStep(ByVal worldMouse As PointF, ByVal shape As Cls_Base_Shape, ByVal grid As Cls_Grid_Manager)
        If shape Is Nothing OrElse grid Is Nothing Then Exit Sub
        Dim r As RectangleF = shape.Bounds
        Dim nextX As Single = r.X : Dim nextY As Single = r.Y
        Dim nextW As Single = r.Width : Dim nextH As Single = r.Height
        Dim mousePt As PointF = If(grid.IsSnapEnabled, grid.SnapPoint(worldMouse), worldMouse)

        If Me._activeHandle = ResizeHandle.TopRight OrElse Me._activeHandle = ResizeHandle.MiddleRight OrElse Me._activeHandle = ResizeHandle.BottomRight Then
            nextW = mousePt.X - r.X
        ElseIf Me._activeHandle = ResizeHandle.TopLeft OrElse Me._activeHandle = ResizeHandle.MiddleLeft OrElse Me._activeHandle = ResizeHandle.BottomLeft Then
            nextW = r.Right - mousePt.X : nextX = mousePt.X
        End If

        Me.EvaluateVerticalResizeBounds(shape, mousePt, r, nextX, nextY, nextW, nextH)
    End Sub

    Private Sub EvaluateVerticalResizeBounds(ByVal shape As Cls_Base_Shape, ByVal mousePt As PointF, ByVal r As RectangleF, ByVal x As Single, ByVal y As Single, ByVal w As Single, ByVal h As Single)
        Dim nextY As Single = y : Dim nextH As Single = h

        If Me._activeHandle = ResizeHandle.BottomLeft OrElse Me._activeHandle = ResizeHandle.BottomCenter OrElse Me._activeHandle = ResizeHandle.BottomRight Then
            nextH = mousePt.Y - r.Y
        ElseIf Me._activeHandle = ResizeHandle.TopLeft OrElse Me._activeHandle = ResizeHandle.TopCenter OrElse Me._activeHandle = ResizeHandle.TopRight Then
            nextH = r.Bottom - mousePt.Y : nextY = mousePt.Y
        End If

        shape.ApplyAtomicGeometryTransform(New PointF(x, nextY), w, nextH)
    End Sub

    Public Sub DrawResizeHandles(ByVal g As Graphics, ByVal zoom As Single, ByVal shape As Cls_Base_Shape)
        If g Is Nothing OrElse shape Is Nothing Then Exit Sub
        Dim r As RectangleF = shape.Bounds
        Dim hSize As Single = 6.0F / zoom : Dim offset As Single = hSize / 2.0F
        Dim midX As Single = r.X + (r.Width / 2.0F) : Dim midY As Single = r.Y + (r.Height / 2.0F)

        Dim localPoints As PointF() = {
            New PointF(r.X, r.Y), New PointF(midX, r.Y), New PointF(r.Right, r.Y),
            New PointF(r.X, midY), New PointF(r.Right, midY),
            New PointF(r.X, r.Bottom), New PointF(midX, r.Bottom), New PointF(r.Right, r.Bottom)
        }

        Using handleBrush As New SolidBrush(Color.White)
            Using handlePen As New Pen(Color.RoyalBlue, 1.0F / zoom)
                For i As Integer = 0 To localPoints.Length - 1
                    Dim x As Single = localPoints(i).X - offset
                    Dim y As Single = localPoints(i).Y - offset
                    g.FillRectangle(handleBrush, x, y, hSize, hSize)
                    g.DrawRectangle(handlePen, x, y, hSize, hSize)
                Next
            End Using
        End Using
    End Sub

    Private Function CheckRect(ByVal p As PointF, ByVal x As Single, ByVal y As Single, ByVal s As Single) As Boolean
        Return (p.X >= x AndAlso p.X <= x + s AndAlso p.Y >= y AndAlso p.Y <= y + s)
    End Function
End Class
