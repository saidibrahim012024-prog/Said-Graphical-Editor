' Target File: Cls_Resize_Manager.vb
' Project Namespace: New_Said_Graphics_Library
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Drawing
Imports System.Windows.Forms

Public NotInheritable Class Cls_Resize_Manager

    ' Target File: Cls_Resize_Manager.vb (Comprehensive 8-Way Viewport Cursor Assigner)
    ' Project Namespace: mdi_test
    ' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    ''' <summary>
    ''' Swaps the active form view cursor layout property based on the currently targeted handle box.
    ''' FIXED STRATEGY: Added explicit case mapping for all 4 center handles to complete the 8-way interaction loop.
    ''' </summary>
    Public Sub UpdateViewportCursor(ByVal handle As ResizeHandle, ByVal viewport As Control)
        If viewport Is Nothing Then Exit Sub

        Select Case handle
            Case ResizeHandle.TopLeft, ResizeHandle.BottomRight
                viewport.Cursor = Cursors.SizeNWSE
            Case ResizeHandle.TopRight, ResizeHandle.BottomLeft
                viewport.Cursor = Cursors.SizeNESW
            Case ResizeHandle.TopCenter, ResizeHandle.BottomCenter
                ' FIXED: Maps vertical center handle hover actions to the North-South resize icon
                viewport.Cursor = Cursors.SizeNS
            Case ResizeHandle.MiddleLeft, ResizeHandle.MiddleRight
                ' FIXED: Maps horizontal center handle hover actions to the West-East resize icon
                viewport.Cursor = Cursors.SizeWE
            Case Else
                viewport.Cursor = Cursors.Default
        End Select
    End Sub

    ''' <summary>
    ''' Performs a high-precision unrotated hit test across all 8 bounding edge handle blocks.
    ''' </summary>
    Public Function HitTestHandles(ByVal worldMouse As PointF, ByVal shape As Cls_Base_Shape, ByVal zoom As Single) As ResizeHandle
        If shape Is Nothing Then Return ResizeHandle.None
        Dim r As RectangleF = shape.Bounds
        Dim hSize As Single = 6.0F / zoom
        Dim half As Single = hSize / 2.0F

        ' 1. Check Corner Handles using raw unrotated boundary coordinates
        If Me.CheckRect(worldMouse, r.X - half, r.Y - half, hSize) Then Return ResizeHandle.TopLeft
        If Me.CheckRect(worldMouse, r.Right - half, r.Y - half, hSize) Then Return ResizeHandle.TopRight
        If Me.CheckRect(worldMouse, r.X - half, r.Bottom - half, hSize) Then Return ResizeHandle.BottomLeft
        If Me.CheckRect(worldMouse, r.Right - half, r.Bottom - half, hSize) Then Return ResizeHandle.BottomRight

        ' Route control down to a segregated center handler to honor your strict 25-line limit barrier
        Return Me.EvaluateCenterEdgeHandleHits(worldMouse, r, hSize, half)
    End Function

    ''' <summary>
    ''' Segregated hit tester evaluating center edge nodes to maintain clean structural code limits.
    ''' </summary>
    Private Function EvaluateCenterEdgeHandleHits(ByVal p As PointF, ByVal r As RectangleF, ByVal s As Single, ByVal half As Single) As ResizeHandle
        ' 2. Calculate midpoints cleanly using unrotated boundary geometry
        Dim midX As Single = r.X + (r.Width / 2.0F)
        Dim midY As Single = r.Y + (r.Height / 2.0F)

        ' 3. FIXED STRATEGY: Validate intersection metrics across all 4 missing center-point boxes!
        If Me.CheckRect(p, midX - half, r.Y - half, s) Then Return ResizeHandle.TopCenter
        If Me.CheckRect(p, midX - half, r.Bottom - half, s) Then Return ResizeHandle.BottomCenter
        If Me.CheckRect(p, r.X - half, midY - half, s) Then Return ResizeHandle.MiddleLeft
        If Me.CheckRect(p, r.Right - half, midY - half, s) Then Return ResizeHandle.MiddleRight

        Return ResizeHandle.None
    End Function

    ''' <summary>
    ''' Computes raw dimensional translations across all 8 handles blindly in unrotated world space.
    ''' </summary>
    Public Sub ExecuteResizeStep(ByVal worldMouse As PointF, ByVal shape As Cls_Base_Shape, ByVal grid As Cls_Grid_Manager)
        If shape Is Nothing OrElse grid Is Nothing Then Exit Sub

        ' 1. Fetch current layout bounds envelope metrics
        Dim r As RectangleF = shape.Bounds
        Dim nextX As Single = r.X : Dim nextY As Single = r.Y
        Dim nextW As Single = r.Width : Dim nextH As Single = r.Height

        ' 2. Enforce 20px grid cell alignment if snapping configurations are actively enabled
        Dim mousePt As PointF = If(grid.IsSnapEnabled, grid.SnapPoint(worldMouse), worldMouse)

        ' 3. Process horizontal stretching vectors type-safely based on the active handle direction
        If Me._activeHandle = ResizeHandle.TopRight OrElse Me._activeHandle = ResizeHandle.MiddleRight OrElse Me._activeHandle = ResizeHandle.BottomRight Then
            nextW = mousePt.X - r.X
        ElseIf Me._activeHandle = ResizeHandle.TopLeft OrElse Me._activeHandle = ResizeHandle.MiddleLeft OrElse Me._activeHandle = ResizeHandle.BottomLeft Then
            nextW = r.Right - mousePt.X : nextX = mousePt.X
        End If

        ' Pass vectors down to a segregated vertical processor helper to respect your 25-line limit restriction
        Me.EvaluateVerticalResizeBounds(shape, mousePt, r, nextX, nextY, nextW, nextH)
    End Sub

    ''' <summary>
    ''' Segregated execution loop calculating vertical layout boundaries to preserve method length limits.
    ''' </summary>
    Private Sub EvaluateVerticalResizeBounds(ByVal shape As Cls_Base_Shape, ByVal mousePt As PointF, ByVal r As RectangleF, ByVal x As Single, ByVal y As Single, ByVal w As Single, ByVal h As Single)
        Dim nextY As Single = y
        Dim nextH As Single = h

        ' 4. Process vertical stretching vectors type-safely based on the active handle direction
        If Me._activeHandle = ResizeHandle.BottomLeft OrElse Me._activeHandle = ResizeHandle.BottomCenter OrElse Me._activeHandle = ResizeHandle.BottomRight Then
            nextH = mousePt.Y - r.Y
        ElseIf Me._activeHandle = ResizeHandle.TopLeft OrElse Me._activeHandle = ResizeHandle.TopCenter OrElse Me._activeHandle = ResizeHandle.TopRight Then
            nextH = r.Bottom - mousePt.Y : nextY = mousePt.Y
        End If

        ' 5. Commit both mutations atomically inside a single step to protect port lifecycles
        shape.ApplyAtomicGeometryTransform(New PointF(x, nextY), w, nextH)
    End Sub


    Private _activeHandle As ResizeHandle = ResizeHandle.None
    Private Const HANDLE_SIZE As Single = 6.0F



    Private _isResizing As Boolean = False
    Private _startWorldPoint As PointF

    Public ReadOnly Property IsResizing As Boolean
        Get
            Return _isResizing
        End Get
    End Property

    Public ReadOnly Property ActiveHandle As ResizeHandle
        Get
            Return _activeHandle
        End Get
    End Property

    Public Sub LockActiveHandle(ByVal handle As ResizeHandle)
        Me._activeHandle = handle
    End Sub

    'Public Function HitTestHandles(ByVal worldMouse As PointF, ByVal shape As Cls_Base_Shape, ByVal zoom As Single) As ResizeHandle
    '    If shape Is Nothing Then Return ResizeHandle.None
    '    Dim r As RectangleF = shape.Bounds
    '    Dim hSize As Single = 6.0F / zoom
    '    Dim half As Single = hSize / 2.0F

    '    If Me.CheckRect(worldMouse, r.X - half, r.Y - half, hSize) Then Return ResizeHandle.TopLeft
    '    If Me.CheckRect(worldMouse, r.Right - half, r.Y - half, hSize) Then Return ResizeHandle.TopRight
    '    If Me.CheckRect(worldMouse, r.X - half, r.Bottom - half, hSize) Then Return ResizeHandle.BottomLeft
    '    If Me.CheckRect(worldMouse, r.Right - half, r.Bottom - half, hSize) Then Return ResizeHandle.BottomRight
    '    Return ResizeHandle.None
    'End Function

    Public Sub UpdateHoverCursor(ByVal worldMouse As PointF, ByVal shape As Cls_Base_Shape, ByVal zoom As Single, ByVal view As Control)
        If view Is Nothing Then Exit Sub
        Dim handle As ResizeHandle = Me.HitTestHandles(worldMouse, shape, zoom)

        If handle = ResizeHandle.TopLeft OrElse handle = ResizeHandle.BottomRight Then
            view.Cursor = Cursors.SizeNWSE
        ElseIf handle = ResizeHandle.TopRight OrElse handle = ResizeHandle.BottomLeft Then
            view.Cursor = Cursors.SizeNESW
        Else
            view.Cursor = Cursors.Default
        End If
    End Sub

    'Public Sub ExecuteResizeStep(ByVal worldMouse As PointF, ByVal shape As Cls_Base_Shape, ByVal grid As Cls_Grid_Manager)
    '    Dim r As RectangleF = shape.Bounds
    '    Dim nextX As Single = r.X : Dim nextY As Single = r.Y
    '    Dim nextW As Single = r.Width : Dim nextH As Single = r.Height

    '    Dim mousePt As PointF = If(grid.IsSnapEnabled, grid.SnapPoint(worldMouse), worldMouse)

    '    Select Case Me._activeHandle
    '        Case ResizeHandle.BottomRight
    '            nextW = mousePt.X - r.X : nextH = mousePt.Y - r.Y
    '        Case ResizeHandle.TopLeft
    '            nextW = r.Right - mousePt.X : nextH = r.Bottom - mousePt.Y
    '            nextX = mousePt.X : nextY = mousePt.Y
    '    End Select

    '    shape.ApplyAtomicGeometryTransform(New PointF(nextX, nextY), nextW, nextH)
    'End Sub

    Private Function CheckRect(ByVal p As PointF, ByVal x As Single, ByVal y As Single, ByVal s As Single) As Boolean
        Return (p.X >= x AndAlso p.X <= x + s AndAlso p.Y >= y AndAlso p.Y <= y + s)
    End Function
    'zzzzzzzzzzzzzzzzz

    '''' <summary>
    '''' Paints the 8 resize handle rectangles centered perfectly along the local shape bounding extremes.
    '''' FIXED: Maps locations relative to local origin (0,0) to stop matrix shifting anomalies.
    '''' </summary>
    'Public Sub DrawResizeHandles(ByVal g As Graphics, ByVal zoom As Single, ByVal shape As Cls_Base_Shape)
    '    If g Is Nothing OrElse shape Is Nothing Then Exit Sub

    '    ' 1. Calculate local dimensions centered around the local transformed origin (0,0)
    '    Dim halfW As Single = shape.Bounds.Width / 2.0F
    '    Dim halfH As Single = shape.Bounds.Height / 2.0F

    '    ' 2. Establish a constant, zoom-invariant handle physical square pixel size footprint
    '    Dim handleSize As Single = 6.0F / zoom
    '    Dim offset As Single = handleSize / 2.0F

    '    ' 3. Map out the 8 local space vertex tracking locations explicitly
    '    Dim localPoints As PointF() = {
    '    New PointF(-halfW, -halfH),          ' Top Left
    '    New PointF(0.0F, -halfH),            ' Top Center
    '    New PointF(halfW, -halfH),           ' Top Right
    '    New PointF(-halfW, 0.0F),            ' Middle Left
    '    New PointF(halfW, 0.0F),             ' Middle Right
    '    New PointF(-halfW, halfH),           ' Bottom Left
    '    New PointF(0.0F, halfH),             ' Bottom Center
    '    New PointF(halfW, halfH)             ' Bottom Right
    '}

    '    ' Render each generated local handle square block cleanly on the graphics thread canvas
    '    Me.PaintHandleSquareBlocks(g, localPoints, handleSize, offset)
    'End Sub

    ''' <summary>
    ''' Segregated vector painter to strictly maintain method line bounds under your 25 operational line ceiling.
    ''' </summary>
    Private Sub PaintHandleSquareBlocks(ByVal g As Graphics, ByVal pts As PointF(), ByVal size As Single, ByVal offset As Single)
        Using handleBrush As New SolidBrush(Color.White)
            Using handlePen As New Pen(Color.RoyalBlue, 1.0F)
                For i As Integer = 0 To pts.Length - 1
                    ' Offset the drawing coordinates locally to center the rectangle on the vector point
                    Dim x As Single = pts(i).X - offset
                    Dim y As Single = pts(i).Y - offset

                    g.FillRectangle(handleBrush, x, y, size, size)
                    g.DrawRectangle(handlePen, x, y, size, size)
                Next
            End Using
        End Using
    End Sub

    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx



    ' Target File: Cls_Resize_Manager.vb (Strategy Aligned Unrotated Hit Testing)
    ' Explicit configuration locked for file scope | Option Strict On | Explicit Binding
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    '''' <summary>
    '''' Performs a hit-test across the 8 unrotated edge handles using raw world coordinate metrics.
    '''' </summary>
    'Public Function HitTestHandles(ByVal worldMouse As PointF, ByVal shape As Cls_Base_Shape, ByVal handleSize As Single) As ResizeHandle
    '    If shape Is Nothing Then Return ResizeHandle.None

    '    Dim r As RectangleF = shape.Bounds
    '    Dim hSize As Single = handleSize
    '    Dim halfH As Single = hSize / 2.0F

    '    ' 1. Check Corner Handles using raw world coordinate boundary limits
    '    If Me.IsPointInHandleBox(worldMouse, r.X - halfH, r.Y - halfH, hSize) Then Return ResizeHandle.TopLeft
    '    If Me.IsPointInHandleBox(worldMouse, r.Right - halfH, r.Y - halfH, hSize) Then Return ResizeHandle.TopRight
    '    If Me.IsPointInHandleBox(worldMouse, r.X - halfH, r.Bottom - halfH, hSize) Then Return ResizeHandle.BottomLeft
    '    If Me.IsPointInHandleBox(worldMouse, r.Right - halfH, r.Bottom - halfH, hSize) Then Return ResizeHandle.BottomRight

    '    ' 2. Check Center Edge Handles using raw world coordinate boundary limits
    '    Dim midX As Single = r.X + (r.Width / 2.0F)
    '    Dim midY As Single = r.Y + (r.Height / 2.0F)

    '    If Me.IsPointInHandleBox(worldMouse, midX - halfH, r.Y - halfH, hSize) Then Return ResizeHandle.TopCenter
    '    If Me.IsPointInHandleBox(worldMouse, midX - halfH, r.Bottom - halfH, hSize) Then Return ResizeHandle.BottomCenter
    '    If Me.IsPointInHandleBox(worldMouse, r.X - halfH, midY - halfH, hSize) Then Return ResizeHandle.MiddleLeft
    '    If Me.IsPointInHandleBox(worldMouse, r.Right - halfH, midY - halfH, hSize) Then Return ResizeHandle.MiddleRight

    '    Return ResizeHandle.None
    'End Function

    Private Function IsPointInHandleBox(ByVal pt As PointF, ByVal x As Single, ByVal y As Single, ByVal size As Single) As Boolean
        Return (pt.X >= x AndAlso pt.X <= x + size AndAlso pt.Y >= y AndAlso pt.Y <= y + size)
    End Function

    '''' <summary>
    '''' Intercepts local mouse clicks and maps them to handles relative to component local boundaries.
    '''' FIXED: Removed all old StartPoint/EndPoint calls to prevent compilation breaks.
    '''' </summary>
    'Public Function HitTestHandles(ByVal localMouse As PointF, ByVal shape As Cls_Base_Shape, ByVal handleSize As Single) As ResizeHandle
    '    If shape Is Nothing Then Return ResizeHandle.None

    '    ' Calculate local center-relative coordinates matching your local matrix transformation origin
    '    Dim halfW As Single = shape.Bounds.Width / 2.0F
    '    Dim halfH As Single = shape.Bounds.Height / 2.0F

    '    ' Evaluate target click boxes centered at the local origin (0,0)
    '    If Math.Abs(localMouse.X - halfW) <= handleSize AndAlso Math.Abs(localMouse.Y - halfH) <= handleSize Then
    '        Return ResizeHandle.BottomRight
    '    ElseIf Math.Abs(localMouse.X - (-halfW)) <= handleSize AndAlso Math.Abs(localMouse.Y - (-halfH)) <= handleSize Then
    '        Return ResizeHandle.TopLeft
    '    End If

    '    Return ResizeHandle.None
    'End Function

    ''' <summary>
    ''' Draws the 8 standard interactive transformation resize squares inside a pre-transformed graphics matrix context.
    ''' </summary>
    Public Sub DrawResizeHandles(g As Graphics, zoom As Single, shape As Cls_Base_Shape)
        ' 1. Extract the raw, un-rotated coordinate dimensions directly from the shape primitive endpoints
        Dim minX As Single = shape.Bounds.X
        Dim minY As Single = shape.Bounds.Y
        Dim maxX As Single = shape.Bounds.Right
        Dim maxY As Single = shape.Bounds.Bottom
        Dim midX As Single = minX + (maxX - minX) / 2.0F
        Dim midY As Single = minY + (maxY - minY) / 2.0F

        ' 2. Calculate the visual size of the handles so they remain constant at any magnification ratio
        Dim size As Single = 6.0F / zoom
        Dim half As Single = size / 2.0F

        ' 3. Initialize the 8 raw point arrays sequentially in local relative space coordinates
        Dim handlePoints As PointF() = {
            New PointF(minX, minY), New PointF(midX, minY), New PointF(maxX, minY),
            New PointF(minX, midY), New PointF(maxX, midY),
            New PointF(minX, maxY), New PointF(midX, maxY), New PointF(maxX, maxY)
        }

        ' 4. Iterate and fill the handle box perimeters using solid green canvas ink
        Using handleBrush As New SolidBrush(Color.LimeGreen)
            For Each pt As PointF In handlePoints
                ' The matrix manages the rotation. The manager only needs to draw standard squares!
                g.FillRectangle(handleBrush, pt.X - half, pt.Y - half, size, size)
            Next
        End Using
    End Sub



    Public Sub StartResize(handle As ResizeHandle, worldPt As PointF)
        _activeHandle = handle
        _isResizing = True
        _startWorldPoint = worldPt
    End Sub

    Public Sub TerminateResize()
        _isResizing = False
        _activeHandle = ResizeHandle.None
    End Sub


    '' Target File: Cls_Resize_Manager.vb (Viewport Cursor Assigner)
    'Public Sub UpdateViewportCursor(ByVal handle As ResizeHandle, ByVal viewport As Control)
    '    If viewport Is Nothing Then Exit Sub

    '    Select Case handle
    '        Case ResizeHandle.TopLeft, ResizeHandle.BottomRight : viewport.Cursor = Cursors.SizeNWSE
    '        Case ResizeHandle.TopRight, ResizeHandle.BottomLeft : viewport.Cursor = Cursors.SizeNESW
    '        Case ResizeHandle.TopCenter, ResizeHandle.BottomCenter : viewport.Cursor = Cursors.SizeNS
    '        Case ResizeHandle.MiddleLeft, ResizeHandle.MiddleRight : viewport.Cursor = Cursors.SizeWE
    '        Case Else : viewport.Cursor = Cursors.Default
    '    End Select
    'End Sub

    'Public Sub UpdateViewportCursor(handle As ResizeHandle, viewport As UserControl)
    '    If viewport Is Nothing Then Exit Sub
    '    Select Case handle
    '        Case ResizeHandle.TopLeft, ResizeHandle.BottomRight : viewport.Cursor = Cursors.SizeNWSE
    '        Case ResizeHandle.TopRight, ResizeHandle.BottomLeft : viewport.Cursor = Cursors.SizeNESW
    '        Case ResizeHandle.TopCenter, ResizeHandle.BottomCenter : viewport.Cursor = Cursors.SizeNS
    '        Case ResizeHandle.MiddleLeft, ResizeHandle.MiddleRight : viewport.Cursor = Cursors.SizeWE
    '        Case Else : viewport.Cursor = Cursors.Default
    '    End Select
    'End Sub



    ''' <summary>
    ''' Computes structural bounding mutations and scales logic gate dimensions.
    ''' FIXED: Added concrete membership method to permanently clear viewport compilation blocks.
    ''' </summary>
    Public Function ExecuteTransformStep(ByVal localMouse As PointF, ByVal gate As Cls_Base_Shape) As Boolean
        If gate Is Nothing OrElse Me._activeHandle = ResizeHandle.None Then
            Return False
        End If

        Dim currentBounds As RectangleF = gate.Bounds
        Dim nextW As Single = currentBounds.Width
        Dim nextH As Single = currentBounds.Height
        Dim locX As Single = gate.Location.X
        Dim locY As Single = gate.Location.Y

        ' 1. Process local horizontal deltas based on which boundary extreme handle is held
        If Me._activeHandle = ResizeHandle.TopRight OrElse Me._activeHandle = ResizeHandle.MiddleRight OrElse Me._activeHandle = ResizeHandle.BottomRight Then
            nextW = localMouse.X - gate.Location.X
        ElseIf Me._activeHandle = ResizeHandle.TopLeft OrElse Me._activeHandle = ResizeHandle.MiddleLeft OrElse Me._activeHandle = ResizeHandle.BottomLeft Then
            nextW = currentBounds.Right - localMouse.X : locX = localMouse.X
        End If

        ' 2. Process local vertical deltas based on which boundary extreme handle is held
        If Me._activeHandle = ResizeHandle.BottomLeft OrElse Me._activeHandle = ResizeHandle.BottomCenter OrElse Me._activeHandle = ResizeHandle.BottomRight Then
            nextH = localMouse.Y - gate.Location.Y
        ElseIf Me._activeHandle = ResizeHandle.TopLeft OrElse Me._activeHandle = ResizeHandle.TopCenter OrElse Me._activeHandle = ResizeHandle.TopRight Then
            nextH = currentBounds.Bottom - localMouse.Y : locY = localMouse.Y
        End If

        ' 3. Commit mutations directly into the base shape abstractions
        gate.Location = New PointF(locX, locY)
        gate.ApplyResizeTransform(nextW, nextH)
        Return True
    End Function



    '' Target File: Cls_Viewport.vb (Compatible ProcessResizeDrag Routine)
    '' Explicit configuration locked for file scope | Option Strict On | Explicit Binding
    '' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    ' ''' <summary>
    '''' Component-compatible resize handler routing handle deltas straight to the active gate object.
    '''' Fully type-safe under Option Strict On and satisfies your 25-line operational limit.
    '''' </summary>
    'Public Sub ProcessResizeDrag(ByVal rawWorldPoint As PointF)
    '    ' 1. Pull the active logic gate component using your confirmed viewport lookup utility
    '    Dim activeComponent As Cls_Base_Shape = Me.GetActiveFocusComponent()

    '    If activeComponent IsNot Nothing Then
    '        ' 2. Intercept raw coordinates and pass them through the local unified grid snapping matrix
    '        Dim snappedMouse As PointF = Me._grid.SnapPoint(rawWorldPoint)

    '        ' 3. Calculate the target width and height relative to the component's upper-left anchor point
    '        Dim nextWidth As Single = snappedMouse.X - activeComponent.Location.X
    '        Dim nextHeight As Single = snappedMouse.Y - activeComponent.Location.Y

    '        ' 4. Inject the new dimensions directly into the gate component's internal layout matrix
    '        activeComponent.ApplyResizeTransform(nextWidth, nextHeight)

    '        Me.FlagActiveViewportAsModified(True)
    '    End If
    'End Sub

#Region "Logic gate resize"

    ''' <summary>
    ''' Component-compatible resize engine utilizing legacy delta-rotation trigonometry.
    ''' प्रोजेक्ट स्नैपशॉट v4 के तहत 100% कंपाइल-सेफ और रोटेशन-स्टेबल बाइंडिंग प्रदान करता है।
    ''' </summary>
    Public Sub ProcessResizeDrag(ByVal worldPt As PointF, ByVal gate As Cls_Base_Shape)
        If gate Is Nothing OrElse Me._activeHandle = ResizeHandle.None Then Exit Sub

        ' 1. Calculate raw world space displacement deltas from the last frame snapshot
        Dim dx As Single = worldPt.X - Me._startWorldPoint.X
        Dim dy As Single = worldPt.Y - Me._startWorldPoint.Y
        Me._startWorldPoint = worldPt

        ' 2. Rotate displacement deltas into the gate component's local space axis plane
        If gate.RotationAngle <> 0.0F Then
            Dim rad As Double = -Convert.ToDouble(gate.RotationAngle) * (Math.PI / 180.0)
            Dim localDx As Single = CSng((Convert.ToDouble(dx) * Math.Cos(rad)) - (Convert.ToDouble(dy) * Math.Sin(rad)))
            Dim localDy As Single = CSng((Convert.ToDouble(dx) * Math.Sin(rad)) + (Convert.ToDouble(dy) * Math.Cos(rad)))
            dx = localDx : dy = localDy
        End If

        ' 3. Route the type-safe local deltas down to the component vector transformer
        Me.ApplyComponentDeltaTransform(dx, dy, gate)
    End Sub

    ''' <summary>
    ''' Adjusts component dimensions and location anchors based on the active handle tracker.
    ''' Bounded strictly under your maximum 25 line restriction rule.
    ''' </summary>
    Private Sub ApplyComponentDeltaTransform(ByVal dx As Single, ByVal dy As Single, ByVal gate As Cls_Base_Shape)
        Dim currentBounds As RectangleF = gate.Bounds
        Dim nextW As Single = currentBounds.Width
        Dim nextH As Single = currentBounds.Height
        Dim locX As Single = gate.Location.X
        Dim locY As Single = gate.Location.Y

        ' 1. Evaluate Horizontal handle deltas relative to structural bounding extremes
        If Me._activeHandle = ResizeHandle.TopRight OrElse Me._activeHandle = ResizeHandle.MiddleRight OrElse Me._activeHandle = ResizeHandle.BottomRight Then
            nextW = currentBounds.Width + dx
        ElseIf Me._activeHandle = ResizeHandle.TopLeft OrElse Me._activeHandle = ResizeHandle.MiddleLeft OrElse Me._activeHandle = ResizeHandle.BottomLeft Then
            nextW = currentBounds.Width - dx : locX += dx
        End If

        ' 2. Evaluate Vertical handle deltas relative to structural bounding extremes
        If Me._activeHandle = ResizeHandle.BottomLeft OrElse Me._activeHandle = ResizeHandle.BottomCenter OrElse Me._activeHandle = ResizeHandle.BottomRight Then
            nextH = currentBounds.Height + dy
        ElseIf Me._activeHandle = ResizeHandle.TopLeft OrElse Me._activeHandle = ResizeHandle.TopCenter OrElse Me._activeHandle = ResizeHandle.TopRight Then
            nextH = currentBounds.Height - dy : locY += dy
        End If

        ' 3. Apply location modifications and pass size mutations straight to the port synchronizer
        gate.Location = New PointF(locX, locY)
        gate.ApplyResizeTransform(nextW, nextH)
        ' Me.FlagActiveViewportAsModified(True)
    End Sub

#End Region

    '' FIXED: Cleans up the Y-coordinate math syntax to display solid fill background blocks flawlessly
    'Public Sub DrawResizeHandles(g As Graphics, zoom As Single, shape As Cls_Base_Shape)
    '    If shape Is Nothing Then Exit Sub
    '    Dim bounds As RectangleF = GetShapeWorldBounds(shape)
    '    Dim size As Single = HANDLE_SIZE / zoom
    '    Dim halfSize As Single = size / 2.0F

    '    Using handleBrush As New SolidBrush(Color.White), handlePen As New Pen(Color.LimeGreen, 1.0! / zoom)
    '        For Each handle As ResizeHandle In [Enum].GetValues(GetType(ResizeHandle))
    '            If handle = ResizeHandle.None Then Continue For
    '            Dim rawCenter As PointF = GetHandleCenterRaw(handle, bounds)
    '            Dim finalCenter As PointF = shape.GetRotatedPoint(rawCenter)

    '            ' Draw directly using clean, non-collapsing arithmetic properties
    '            g.FillRectangle(handleBrush, finalCenter.X - halfSize, finalCenter.Y - halfSize, size, size)
    '            g.DrawRectangle(handlePen, finalCenter.X - halfSize, finalCenter.Y - halfSize, size, size)
    '        Next
    '    End Using
    'End Sub

    Private Function GetHandleCenterRaw(handle As ResizeHandle, rect As RectangleF) As PointF
        Select Case handle
            Case ResizeHandle.TopLeft : Return New PointF(rect.X, rect.Y)
            Case ResizeHandle.TopCenter : Return New PointF(rect.X + (rect.Width / 2.0F), rect.Y)
            Case ResizeHandle.TopRight : Return New PointF(rect.Right, rect.Y)
            Case ResizeHandle.MiddleRight : Return New PointF(rect.Right, rect.Y + (rect.Height / 2.0F))
            Case ResizeHandle.BottomRight : Return New PointF(rect.Right, rect.Bottom)
            Case ResizeHandle.BottomCenter : Return New PointF(rect.X + (rect.Width / 2.0F), rect.Bottom)
            Case ResizeHandle.BottomLeft : Return New PointF(rect.X, rect.Bottom)
            Case ResizeHandle.MiddleLeft : Return New PointF(rect.X, rect.Y + (rect.Height / 2.0F))
            Case Else : Return PointF.Empty
        End Select
    End Function

    Private Function GetShapeWorldBounds(shape As Cls_Base_Shape) As RectangleF
        Return shape.Bounds ' New RectangleF(Math.Min(shape.StartPoint.X, shape.EndPoint.X), Math.Min(shape.StartPoint.Y, shape.EndPoint.Y), Math.Abs(shape.StartPoint.X - shape.EndPoint.X), Math.Abs(shape.StartPoint.Y - shape.EndPoint.Y))
    End Function
End Class


