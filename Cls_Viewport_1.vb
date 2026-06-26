' Target File: Cls_Viewport.vb (Strict State Machine Engine Core)
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

Public NotInheritable Class Cls_Viewport
    Inherits UserControl


    ' Target File: Cls_Viewport.vb (Unified Unrotated Overlays Router)
    ' Strictly type-safe under Option Strict On and resides safely under the 25-line limit.

    Private Sub RenderSelectionOverlays(ByVal g As Graphics, ByVal cameraMatrix As Matrix)
        Dim activeShape As Cls_Base_Shape = Me._selectionMgr.TryGetActiveFocus()

        ' 1. FORCE STRATEGY: Clamp the graphics transformer context to the unrotated camera matrix upfront!
        g.Transform = cameraMatrix

        ' 2. Draw the unrotated blue marquee outline boxes wrapped around all selected elements
        For Each shape As Cls_Base_Shape In Me._selectionMgr.SelectedShapes
            Me.RenderSelectionOutline(g, Me._zoomFactor, shape)
        Next

        If activeShape IsNot Nothing Then
            Dim isCtrlDown As Boolean = (ModifierKeys And Keys.Control) = Keys.Control

            ' 3. Draw the unrotated resize handle boxes natively over raw world coordinates
            If Me._currentState = ViewportState.Resizing OrElse isCtrlDown Then
                Me._resizeMgr.DrawResizeHandles(g, Me._zoomFactor, activeShape)
            End If

            ' 4. FIXED STRATEGY: Draw rotation crosshairs natively over raw unrotated coordinates!
            ' This checks your state machine flag directly, rendering the orange metrics upon click down.
            If Me._currentState = ViewportState.Rotating Then
                Me._rotationMgr.DrawRotationOverlayMetrics(g, Me._zoomFactor, activeShape)
            End If
        End If
    End Sub


    Private Sub ProcessLeftMouseDown(ByVal e As MouseEventArgs)
        Dim rawWorldPoint As PointF = Me._transformer.TransformScreenToWorld(e.Location, Me._panOffset, Me._zoomFactor)
        Dim activeFocus As Cls_Base_Shape = Me._selectionMgr.TryGetActiveFocus()

        Dim isCtrlDown As Boolean = (ModifierKeys And Keys.Control) = Keys.Control
        Dim isAltDown As Boolean = (ModifierKeys And Keys.Alt) = Keys.Alt

        ' 1. SIZING HANDLE CHECK: Intercept scaling routes exclusively if CONTROL is held down
        If isCtrlDown AndAlso activeFocus IsNot Nothing Then
            Dim handle As ResizeHandle = Me._resizeMgr.HitTestHandles(rawWorldPoint, activeFocus, Me._zoomFactor)
            If handle <> ResizeHandle.None Then
                Me._resizeMgr.LockActiveHandle(handle)
                Me._currentState = ViewportState.Resizing : Me._worldDragStart = rawWorldPoint : Exit Sub
            End If
        End If

        ' 2. FIXED ROTATION CHECK: Capture anchors and force a repaint INSTANTLY on ALT + Click!
        If isAltDown AndAlso activeFocus IsNot Nothing Then
            Me._currentState = ViewportState.Rotating
            Me._worldDragStart = rawWorldPoint
            Me.Cursor = Cursors.UpArrow
            Me.Invalidate() ' ◄─ Forces the screen to paint the rotation crosshairs immediately!
            Exit Sub
        End If

        ' 3. STANDARD SELECTION ROUTE: Fall through if no modification keys are pressed
        Me.EvaluateStandardCanvasInteractionRoute(rawWorldPoint)
    End Sub

    Private Sub ExecuteRotatingPipeline(ByVal worldPt As PointF)
        Dim focusGate As Cls_Base_Shape = Me._selectionMgr.TryGetActiveFocus()
        If focusGate Is Nothing Then Exit Sub

        ' FIXED: Passes both the current world cursor and the initial drag anchor tracking point!
        Me._rotationMgr.ExecuteRotationStep(worldPt, Me._worldDragStart, focusGate)

        ' Note: We do NOT execute "Me._worldDragStart = worldPt" here! 
        ' Keeping the anchor locked at the initial click position preserves the cumulative drag trajectory.
        Me.Invalidate()
    End Sub

    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
        MyBase.OnMouseDown(e)

        ' 1. FORCE VIEWPORT SECLUSION: Command focus directly onto this glass rendering surface
        Me.Focus()

        If Me._currentState <> ViewportState.Idle Then Exit Sub

        ' 2. Middle Mouse or ALT-Click (with zero items selected) forces immediate Panning actions
        If e.Button = MouseButtons.Middle OrElse (e.Button = MouseButtons.Left AndAlso (ModifierKeys And Keys.Alt) = Keys.Alt AndAlso Me._selectionMgr.Count = 0) Then
            Me._currentState = ViewportState.Panning : Me._startMousePos = e.Location : Me.Cursor = Cursors.NoMove2D : Exit Sub
        End If

        If e.Button = MouseButtons.Left Then
            ' 3. Capture hardware coordinates and isolate execution path strictly within this control boundary
            Me.Capture = True
            Dim worldPt As PointF = Me._transformer.TransformScreenToWorld(e.Location, Me._panOffset, Me._zoomFactor)
            Me.EvaluateLeftMouseDownStateTransitionMatrix(worldPt)
        End If
    End Sub

    'Private Sub ExecuteRotatingPipeline(ByVal worldPt As PointF)
    '    Dim focusGate As Cls_Base_Shape = Me._selectionMgr.TryGetActiveFocus()
    '    If focusGate Is Nothing Then Exit Sub

    '    ' 1. Print raw input coordinates relative to our locked state machine anchors
    '    Diagnostics.Debug.WriteLine($"[ROTATION MOVEMENT] Cursor: ({worldPt.X:F1}, {worldPt.Y:F1}) | Anchor Baseline: ({Me._worldDragStart.X:F1}, {Me._worldDragStart.Y:F1})")

    '    ' 2. FIXED: Pass the initial click anchor down blindly to allow cumulative angle shifts
    '    Me._rotationMgr.ExecuteRotationStep(worldPt, Me._worldDragStart, focusGate)

    '    ' 3. Note: Do NOT execute "Me._worldDragStart = worldPt" here! 
    '    ' Keeping the anchor locked at the initial click position preserves the full drag trajectory.
    '    Me.Invalidate()
    'End Sub

    'Private Sub ExecuteRotatingPipeline(ByVal worldPt As PointF)
    '    Dim focusGate As Cls_Base_Shape = Me._selectionMgr.TryGetActiveFocus()
    '    If focusGate Is Nothing Then Exit Sub

    '    ' FIXED: Passes both the current world cursor and the initial drag anchor tracking point!
    '    Me._rotationMgr.ExecuteRotationStep(worldPt, Me._worldDragStart, focusGate)

    '    ' Update your drag anchor to track your pointer frames sequentially
    '    Me._worldDragStart = worldPt
    '    Me.Invalidate()
    'End Sub

    'Private Sub ProcessLeftMouseDown(ByVal e As MouseEventArgs)
    '    Dim rawWorldPoint As PointF = Me._transformer.TransformScreenToWorld(e.Location, Me._panOffset, Me._zoomFactor)
    '    Dim activeFocus As Cls_Base_Shape = Me._selectionMgr.TryGetActiveFocus()

    '    Dim isCtrlDown As Boolean = (ModifierKeys And Keys.Control) = Keys.Control
    '    Dim isAltDown As Boolean = (ModifierKeys And Keys.Alt) = Keys.Alt

    '    ' 1. RESIZE INTERCEPTION: Handle resizing handles exclusively if CONTROL is active
    '    If isCtrlDown AndAlso activeFocus IsNot Nothing Then
    '        Dim handle As ResizeHandle = Me._resizeMgr.HitTestHandles(rawWorldPoint, activeFocus, Me._zoomFactor)
    '        If handle <> ResizeHandle.None Then
    '            Me._resizeMgr.LockActiveHandle(handle)
    '            Me._currentState = ViewportState.Resizing : Me._worldDragStart = rawWorldPoint : Exit Sub
    '        End If
    '    End If

    '    ' 2. FIXED ROTATION INTERCEPTION: Intercept pivot vectors exclusively if ALT is active!
    '    ' This permanently blocks shapes from moving when you intend to rotate them.
    '    If isAltDown AndAlso activeFocus IsNot Nothing Then
    '        Me.InitiateActiveRotation(activeFocus)
    '        Me._worldDragStart = rawWorldPoint
    '        Exit Sub ' Lock state instantly and terminate click processing hooks
    '    End If

    '    ' 3. STANDARD ROUTE: Proceed with standard drags or marquee crossing selections
    '    Me.EvaluateStandardCanvasInteractionRoute(rawWorldPoint)
    'End Sub

    ' Target File: Cls_Viewport.vb (Selection Router Pipeline Pass)
    ' Explicit configuration locked for file scope | Option Strict On | Explicit Binding
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    ''' <summary>
    ''' Standard selection router pipeline evaluating component click hits or marquee tracks.
    ''' FIXED: Provides an explicit member definition to permanently resolve compilation blocks.
    ''' </summary>
    Private Sub EvaluateStandardCanvasInteractionRoute(ByVal rawWorldPoint As PointF)
        ' Scan the master collection database track using your rotation-safe hit-tester
        Dim hitGate As Cls_Base_Shape = Me._canvasData.HitTestShapes(rawWorldPoint, Me._transformer)
        Dim isShiftDown As Boolean = (ModifierKeys And Keys.Shift) = Keys.Shift

        If hitGate IsNot Nothing Then
            ' An existing gate component was hit: Force GroupDragging mode and lock anchors
            Me._currentState = ViewportState.GroupDragging
            Me._worldDragStart = rawWorldPoint
            Me._selectionMgr.SelectShape(hitGate, isShiftDown)
        Else
            ' No component intersected: Safely initialize a marquee selection crossing track
            If Not isShiftDown Then Me._selectionMgr.Clear()
            Me._selectionMgr.IsMarqueeSelecting = True
            Me._selectionMgr.MarqueeStartPoint = rawWorldPoint
            Me._selectionMgr.MarqueeCurrentPoint = rawWorldPoint
            Me._currentState = ViewportState.MarqueeSelecting
        End If
        Me.Invalidate()
    End Sub


    Private _currentState As ViewportState = ViewportState.Idle
    Private _worldDragStart As PointF

    ' Central infrastructure tracking references
    Private ReadOnly _transformer As New Cls_Coordinate_Transformer()
    Private ReadOnly _canvasData As New Cls_Drawing_Canvas(Me)
    Private ReadOnly _selectionMgr As New Cls_Selection_Manager()
    Private ReadOnly _resizeMgr As New Cls_Resize_Manager()
    Private ReadOnly _rotationMgr As New Cls_Rotation_Manager()
    Private ReadOnly _grid As New Cls_Grid_Manager()

    Private _panOffset As PointF = New PointF(0.0F, 0.0F)
    Private _zoomFactor As Single = 1.0F
    Private _currentTool As CanvasTool = CanvasTool.SelectPointer

    Private ReadOnly _keyboardMgr As Cls_Keyboard_Manager


    Private _activeGateType As LogicGateType = LogicGateType.Buffer


    Private _startMousePos As Point
    Private _worldStartPoint As PointF
    Private _worldCurrentPoint As PointF


#Region "Properties"

    Public ReadOnly Property Grid() As Cls_Grid_Manager
        Get
            Return Me._grid
        End Get
    End Property

    Public ReadOnly Property CanvasData() As Cls_Drawing_Canvas
        Get
            Return Me._canvasData
        End Get
    End Property

    Public ReadOnly Property SelectionManager() As Cls_Selection_Manager
        Get
            Return Me._selectionMgr
        End Get
    End Property

    Public ReadOnly Property RotationManager() As Cls_Rotation_Manager
        Get
            Return Me._rotationMgr
        End Get
    End Property

    Public Property CurrentState() As ViewportState
        Get
            Return Me._currentState
        End Get
        Set(ByVal value As ViewportState)
            Me._currentState = value
        End Set
    End Property

    Public Property ActiveTool() As CanvasTool
        Get
            Return Me._currentTool
        End Get
        Set(ByVal value As CanvasTool)
            Me._currentTool = value
        End Set
    End Property

    Public Property ActiveGateType() As LogicGateType
        Get
            Return Me._activeGateType
        End Get
        Set(ByVal value As LogicGateType)
            Me._activeGateType = value
        End Set
    End Property

    Public Property ZoomFactor() As Single
        Get
            Return Me._zoomFactor
        End Get
        Set(ByVal value As Single)
            Me._zoomFactor = Math.Max(0.1F, Math.Min(10.0F, value))
        End Set
    End Property

#End Region

#Region "Constructor & Initialization"

    Public Sub New()
        Me.Dock = DockStyle.Fill
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint Or
                    ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or
                    ControlStyles.Selectable, True)
        Me.BackColor = Color.White

        Me.AllowDrop = True
        'AddHandler Me.DragEnter, AddressOf Me.OnViewportDragEnter
        'AddHandler Me.DragDrop, AddressOf Me.OnViewportDragDrop

        Me._keyboardMgr = New Cls_Keyboard_Manager(Me)
    End Sub

#End Region

#Region "Zooming"
    ' Target File: Cls_Viewport.vb (Zoom-at-Mouse Subsystem)
    ' Explicit configuration locked for file scope | Option Strict On | Explicit Binding
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    '''' <summary>
    '''' Catches hardware mouse wheel scroll messages to initiate view scaling.
    '''' </summary>
    'Protected Overrides Sub OnMouseWheel(ByVal e As MouseEventArgs)
    '    MyBase.OnMouseWheel(e)

    '    ' 1. Calculate a dynamic scale delta factor based on scroll wheel orientation rotation directional ticks
    '    Dim scaleDelta As Single = If(e.Delta > 0, 1.1F, 0.9F)

    '    ' 2. Pass control straight down to the execution engine compositor to calculate new panning offsets
    '    Me.ExecuteZoomCalculation(scaleDelta, e.Location)
    'End Sub

    Protected Overrides Sub OnMouseWheel(ByVal e As MouseEventArgs)
        MyBase.OnMouseWheel(e)
        ' ZOOM AT MOUSE MATRIX: Calculate focus anchor before scale change
        Dim mouseWorldBefore As PointF = Me._transformer.TransformScreenToWorld(e.Location, Me._panOffset, Me._zoomFactor)

        Dim scaleStep As Single = If(e.Delta > 0, 1.1F, 0.9F)
        Me._zoomFactor = Math.Max(0.1F, Math.Min(10.0F, Me._zoomFactor * scaleStep))

        ' Shift pan offset dynamically to pin the world coordinate directly beneath your cursor
        Me._panOffset.X = Convert.ToSingle(Convert.ToDouble(e.Location.X) - Convert.ToDouble(mouseWorldBefore.X) * Convert.ToDouble(Me._zoomFactor))
        Me._panOffset.Y = Convert.ToSingle(Convert.ToDouble(e.Location.Y) - Convert.ToDouble(mouseWorldBefore.Y) * Convert.ToDouble(Me._zoomFactor))
        Me.Invalidate()
    End Sub

    ''' <summary>
    ''' Executes zoom-at-mouse calculations and shifts pan offsets to pin the coordinate under the cursor.
    ''' Fully type-safe under Option Strict On and resides safely under your 25 operational line limit.
    ''' </summary>
    Private Sub ExecuteZoomCalculation(ByVal scaleDelta As Single, ByVal screenCursorPos As Point)
        ' 1. Capture the absolute world space coordinate location sitting directly underneath the cursor BEFORE scaling
        Dim worldFocusBefore As PointF = Me._transformer.TransformScreenToWorld(screenCursorPos, Me._panOffset, Me._zoomFactor)

        ' 2. Apply scale factor mutation and clamp to standard boundaries (Min 10% zoom, Max 1000% zoom)
        Dim nextZoom As Single = Me._zoomFactor * scaleDelta
        Me._zoomFactor = Math.Max(0.1F, Math.Min(10.0F, nextZoom))

        ' 3. RE-CENTER MATRIX FORMULA: Calculate double-precision offsets to keep the focus coordinate locked under the cursor
        Dim nextPanX As Double = Convert.ToDouble(screenCursorPos.X) - (Convert.ToDouble(worldFocusBefore.X) * Convert.ToDouble(Me._zoomFactor))
        Dim nextPanY As Double = Convert.ToDouble(screenCursorPos.Y) - (Convert.ToDouble(worldFocusBefore.Y) * Convert.ToDouble(Me._zoomFactor))

        ' 4. Commit the new shifted panning parameters type-safely back to the layout tracking anchors
        Me._panOffset.X = Convert.ToSingle(nextPanX)
        Me._panOffset.Y = Convert.ToSingle(nextPanY)

        ' 5. Invalidate the workspace surface to force a clean, unrotated visual paint redraw pass
        Me.Invalidate()
    End Sub


    ' Target File: Cls_Viewport.vb (Zoom-to-Fit Subsystem)
    ' Explicit configuration locked for file scope | Option Strict On | Explicit Binding
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    ''' <summary>
    ''' Scans all active logic gates to calculate layout bounds and fits everything centered on screen.
    ''' Fully type-safe under Option Strict On and resides safely under your 25 operational line limit.
    ''' </summary>
    Public Sub ExecuteZoomToFit()
        Dim components As List(Of Cls_Base_Shape) = Me._canvasData.SchematicComponents

        ' Reset instantly to standard 100% origin defaults if the workspace is completely empty
        If components Is Nothing OrElse components.Count = 0 Then
            Me._zoomFactor = 1.0F : Me._panOffset = New PointF(0.0F, 0.0F) : Me.Invalidate() : Exit Sub
        End If

        ' Initialize outer extremes using the first placed element footprint bounds
        Dim firstRect As RectangleF = components(0).Bounds
        Dim minX As Single = firstRect.X : Dim minY As Single = firstRect.Y
        Dim maxX As Single = firstRect.Right : Dim maxY As Single = firstRect.Bottom

        ' Expand boundary envelopes by iterating across remaining active schematic gates
        For i As Integer = 1 To components.Count - 1
            Dim r As RectangleF = components(i).Bounds
            minX = Math.Min(minX, r.X) : minY = Math.Min(minY, r.Y)
            maxX = Math.Max(maxX, r.Right) : maxY = Math.Max(maxY, r.Bottom)
        Next

        Me.CalculateZoomToFitOffsets(minX, minY, maxX - minX, maxY - minY)
    End Sub

    ''' <summary>
    ''' Segregated math processor ensuring method metrics remain underneath your 25-line limit barrier.
    ''' </summary>
    Private Sub CalculateZoomToFitOffsets(ByVal contentX As Single, ByVal contentY As Single, ByVal contentW As Single, ByVal contentH As Single)
        ' Enforce 50px boundary safety margins on all four viewport frame edges
        Dim padding As Single = 100.0F
        Dim viewW As Single = Math.Max(1.0F, Convert.ToSingle(Me.Width) - padding)
        Dim viewH As Single = Math.Max(1.0F, Convert.ToSingle(Me.Height) - padding)

        ' Calculate the required zoom factor ratios and clamp safely within standard boundaries
        Dim scaleX As Single = viewW / Math.Max(1.0F, contentW)
        Dim scaleY As Single = viewH / Math.Max(1.0F, contentH)
        Me._zoomFactor = Math.Max(0.1F, Math.Min(4.0F, Math.Min(scaleX, scaleY)))

        ' Centering Matrix Formula: Position content directly in the middle of the workspace pane
        Dim offsetX As Double = (Convert.ToDouble(Me.Width) - Convert.ToDouble(contentW) * Convert.ToDouble(Me._zoomFactor)) / 2.0
        Dim offsetY As Double = (Convert.ToDouble(Me.Height) - Convert.ToDouble(contentH) * Convert.ToDouble(Me._zoomFactor)) / 2.0

        Me._panOffset.X = Convert.ToSingle(offsetX - Convert.ToDouble(contentX) * Convert.ToDouble(Me._zoomFactor))
        Me._panOffset.Y = Convert.ToSingle(offsetY - Convert.ToDouble(contentY) * Convert.ToDouble(Me._zoomFactor))
        Me.Invalidate()
    End Sub

    ' Target File: Cls_Viewport.vb (Incremental View Scaling Subsystem)
    ' Explicit configuration locked for file scope | Option Strict On | Explicit Binding
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    ''' <summary>
    ''' Increments the workspace scale by 10% centered precisely on the mid-point of the screen form window.
    ''' </summary>
    Public Sub ExecuteZoomIn()
        ' Calculate the absolute center pixel point vector of the current client control frame boundaries
        Dim centerScreen As New Point(Me.Width \ 2, Me.Height \ 2)

        ' 1.1F multiplies the active scale layer factor by a crisp, positive 110% step delta
        Me.ExecuteZoomCalculation(1.1F, centerScreen)
    End Sub

    ''' <summary>
    ''' Decrements the workspace scale by 10% centered precisely on the mid-point of the screen form window.
    ''' </summary>
    Public Sub ExecuteZoomOut()
        Dim centerScreen As New Point(Me.Width \ 2, Me.Height \ 2)

        ' 0.9F scales the active workspace layer down cleanly to a 90% step delta
        Me.ExecuteZoomCalculation(0.9F, centerScreen)
    End Sub

#End Region

#Region "Paint"
    ' Target File: Cls_Viewport.vb (Unrotated Strategy Paint Core)
    ' Project Namespace: mdi_test
    ' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    ' Target File: Cls_Viewport.vb (Complete Unified Render Engine Block)
    ' Project Namespace: mdi_test
    ' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

#Region "Unified Rendering Infrastructure"

    ''' <summary>
    ''' Establishes the core unrotated camera transform and anchors all drawing pipelines.
    ''' </summary>
    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        MyBase.OnPaint(e)
        Dim g As Graphics = e.Graphics

        ' 1. Establish the clean, unrotated base camera workspace matrix context authority
        Using cameraMatrix As Matrix = Me._transformer.CreateCameraMatrix(Me._panOffset, Me._zoomFactor)
            g.Transform = cameraMatrix

            ' 2. Calculate camera bounds dynamically and draw the drafting grid manager background
            Dim visibleBounds As RectangleF = Me._transformer.CalculateWorldBounds(Me._panOffset, Me._zoomFactor, Me.Width, Me.Height)
            Me._grid.Render(g, Me._zoomFactor, visibleBounds)

            ' 3. Render logic gate component symbols via the protected coordinate wrapper loop
            Me.RenderProtectedWorldCanvas(g, cameraMatrix)

            ' 4. STRATEGY ENFORCED: Pass cameraMatrix directly into overlays to protect unrotated handle drawing!
            Me.RenderSelectionOverlays(g, cameraMatrix)

            ' 5. Draw multi-selection marquee window box crossing frames if actively dragging
            If Me._currentState = ViewportState.MarqueeSelecting Then
                Me.DrawMarqueeOverlayWindow(g)
            End If
        End Using
        g.ResetTransform()
    End Sub

    ''' <summary>
    ''' Iterates through your master collection database list and draws each gate natively.
    ''' </summary>
    Private Sub RenderProtectedWorldCanvas(ByVal g As Graphics, ByVal cameraMatrix As Matrix)
        If g Is Nothing OrElse cameraMatrix Is Nothing Then Exit Sub

        Dim componentsList As List(Of Cls_Base_Shape) = Me._canvasData.SchematicComponents
        If componentsList Is Nothing Then Exit Sub

        ' Loop through your unified single master data repository track sequentially
        For i As Integer = 0 To componentsList.Count - 1
            Dim shape As Cls_Base_Shape = componentsList(i)

            If shape IsNot Nothing Then
                ' Delegate the drawing step to the coordinate transformer to append localized rotations safely
                Me._transformer.ExecuteSafeShapeRender(g, cameraMatrix, shape)
            End If
        Next
    End Sub

    '''' <summary>
    '''' Paints selection highlights and delegates gizmo overlays using unrotated camera-space matrices.
    '''' </summary>
    'Private Sub RenderSelectionOverlays(ByVal g As Graphics, ByVal cameraMatrix As Matrix)
    '    Dim activeShape As Cls_Base_Shape = Me._selectionMgr.TryGetActiveFocus()

    '    ' 1. FORCE STRATEGY: Lock the graphics transformer context to the unrotated camera matrix upfront!
    '    g.Transform = cameraMatrix

    '    ' 2. Draw the unrotated marquee outline boxes wrapped around all selected elements
    '    For Each shape As Cls_Base_Shape In Me._selectionMgr.SelectedShapes
    '        Me.RenderSelectionOutline(g, Me._zoomFactor, shape)
    '    Next

    '    If activeShape IsNot Nothing Then
    '        Dim isCtrlDown As Boolean = (ModifierKeys And Keys.Control) = Keys.Control

    '        ' 3. Draw the unrotated resize handle boxes natively over raw world coordinates
    '        If Me._currentState = ViewportState.Resizing OrElse isCtrlDown Then
    '            Me._resizeMgr.DrawResizeHandles(g, Me._zoomFactor, activeShape)
    '        End If

    '        ' 4. Draw the unrotated crosshairs and metrics labels centered on raw world coordinates
    '        If Me._currentState = ViewportState.Rotating Then
    '            Me._rotationMgr.DrawRotationOverlayMetrics(g, Me._zoomFactor, activeShape)
    '        End If
    '    End If
    'End Sub

    ''' <summary>
    ''' Paints a thick Royal Blue selection box wrapped directly around a component's edges.
    ''' </summary>
    Private Sub RenderSelectionOutline(ByVal g As Graphics, ByVal zoom As Single, ByVal shape As Cls_Base_Shape)
        If g Is Nothing OrElse shape Is Nothing Then Exit Sub

        ' 1. Establish a high-visibility, zoom-invariant selection pen vector
        Using selectPen As New Pen(Color.RoyalBlue, 2.5F / zoom) With {.DashStyle = DashStyle.Dash}
            ' 2. STRATEGY ALIGNED: Draw directly inside the unrotated workspace space!
            Dim r As RectangleF = shape.Bounds
            g.DrawRectangle(selectPen, r.X, r.Y, r.Width, r.Height)
        End Using
    End Sub

    ''' <summary>
    ''' Renders the translucent Royal Blue crossing window selection marquee frame on the screen.
    ''' </summary>
    Private Sub DrawMarqueeOverlayWindow(ByVal g As Graphics)
        If g Is Nothing Then Exit Sub

        ' 1. Calculate boundaries based on the selection manager's tracked trajectory points
        Dim x As Single = Math.Min(Me._selectionMgr.MarqueeStartPoint.X, Me._selectionMgr.MarqueeCurrentPoint.X)
        Dim y As Single = Math.Min(Me._selectionMgr.MarqueeStartPoint.Y, Me._selectionMgr.MarqueeCurrentPoint.Y)
        Dim w As Single = Math.Abs(Me._selectionMgr.MarqueeStartPoint.X - Me._selectionMgr.MarqueeCurrentPoint.X)
        Dim h As Single = Math.Abs(Me._selectionMgr.MarqueeStartPoint.Y - Me._selectionMgr.MarqueeCurrentPoint.Y)
        Dim marqueeBounds As New RectangleF(x, y, w, h)

        ' 2. Paint the translucent fill and dashed border matching professional CAD themes
        Using marqueeBrush As New SolidBrush(Color.FromArgb(15, Color.RoyalBlue))
            Using marqueePen As New Pen(Color.RoyalBlue, 1.0F / Me._zoomFactor) With {.DashStyle = DashStyle.Dash}
                g.FillRectangle(marqueeBrush, marqueeBounds)
                g.DrawRectangle(marqueePen, marqueeBounds.X, marqueeBounds.Y, marqueeBounds.Width, marqueeBounds.Height)
            End Using
        End Using
    End Sub

#End Region


#End Region

#Region "Native OLE Drag-and-Drop Infrastructure"
    ' Target File: Cls_Viewport.vb (OLE Drag-and-Drop Landing Subsystem)
    ' Explicit configuration locked for file scope | Option Strict On | Explicit Binding
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    ''' <summary>
    ''' Intercepts the mouse trajectory crossing over the window pane boundary.
    ''' Filters data objects type-safely to allow or reject drag operations.
    ''' </summary>
    Protected Overrides Sub OnDragEnter(ByVal drgevent As DragEventArgs)
        MyBase.OnDragEnter(drgevent)

        ' Verify if the payload package string contains a recognized logic gate token descriptor name
        If drgevent.Data IsNot Nothing AndAlso drgevent.Data.GetDataPresent(DataFormats.StringFormat) Then
            drgevent.Effect = DragDropEffects.Copy
        Else
            drgevent.Effect = DragDropEffects.None
        End If
    End Sub

    ''' <summary>
    ''' Executes upon left mouse release over the glass surface.
    ''' Unpacks string descriptors, projects coordinates, snaps to grid cells, and commits elements.
    ''' </summary>
    Protected Overrides Sub OnDragDrop(ByVal drgevent As DragEventArgs)
        MyBase.OnDragDrop(drgevent)
        If drgevent.Data Is Nothing OrElse Not drgevent.Data.GetDataPresent(DataFormats.StringFormat) Then Exit Sub

        ' 1. Unpack the serialized logic gate class descriptor string token out of the data package
        Dim componentToken As String = DirectCast(drgevent.Data.GetData(DataFormats.StringFormat), String)

        ' 2. Translate the hardware screen cursor position coordinates to local viewport window client space
        Dim clientPoint As Point = Me.PointToClient(New Point(drgevent.X, drgevent.Y))

        ' 3. Project client pixels through the transformer to calculate absolute world space coordinates
        Dim worldPoint As PointF = Me._transformer.TransformScreenToWorld(clientPoint, Me._panOffset, Me._zoomFactor)

        ' 4. Clamp the landing point vector perfectly straight onto your nearest 20px grid intersections
        Dim snappedInsertionPoint As PointF = Me._grid.SnapPoint(worldPoint)

        ' Pass control down to a segregated component dispatcher factory to respect the 25-line ceiling
        Me.InstantiateDroppedGateComponent(componentToken, snappedInsertionPoint)
    End Sub

    ' Target File: Cls_Viewport.vb (String Token Receiver Update Pass)
    ' Fully type-safe under Option Strict On and resides safely under the 25-line limit.

    Private Sub InstantiateDroppedGateComponent(ByVal tokenName As String, ByVal insertionPt As PointF)
        Dim newGate As Cls_Base_Shape = Nothing

        ' FIXED: Evaluate string tokens matching your explicit Enum name conversions!
        If String.Equals(tokenName, "ANDGATE", StringComparison.OrdinalIgnoreCase) Then
            newGate = New Cls_Gate_And(insertionPt)
        ElseIf String.Equals(tokenName, "NOTGATE", StringComparison.OrdinalIgnoreCase) Then
            newGate = New Cls_Gate_Not(insertionPt)
            'ElseIf String.Equals(tokenName, "ORGATE", StringComparison.OrdinalIgnoreCase) Then
            '    newGate = New Cls_Gate_Or(insertionPt)
        End If

        If newGate IsNot Nothing Then
            ' Commit straight into the authoritative master repository collection
            Me._canvasData.SchematicComponents.Add(newGate)

            ' Pre-select the newly dropped component instantly
            Me._selectionMgr.SelectShape(newGate, False)

            Me.Invalidate()
        End If
    End Sub

    '''' <summary>
    '''' Instantiates a concrete logic gate component subclass based on the parsed data format token.
    '''' </summary>
    'Private Sub InstantiateDroppedGateComponent(ByVal tokenName As String, ByVal insertionPt As PointF)
    '    Dim newGate As Cls_Base_Shape = Nothing

    '    ' Evaluate raw string string values explicitly under Option Strict On rules
    '    If String.Equals(tokenName, "AND_GATE", StringComparison.OrdinalIgnoreCase) Then
    '        newGate = New Cls_Gate_And(insertionPt)
    '    ElseIf String.Equals(tokenName, "NOT_GATE", StringComparison.OrdinalIgnoreCase) Then
    '        newGate = New Cls_Gate_Not(insertionPt)
    '        'ElseIf String.Equals(tokenName, "OR_GATE", StringComparison.OrdinalIgnoreCase) Then
    '        ' newGate = New Cls_Gate_Or(insertionPt)
    '    End If

    '    If newGate IsNot Nothing Then
    '        ' Commit straight to the single authoritative master list collection
    '        Me._canvasData.SchematicComponents.Add(newGate)

    '        ' Auto-select the newly dropped block instantly to show the royal blue selection handles
    '        Me._selectionMgr.SelectShape(newGate, False)

    '        Me.Invalidate()
    '    End If
    'End Sub

#End Region

    ' Target File: Cls_Viewport.vb (Active Focus Alias Bridge)
    ' Explicit configuration locked for file scope | Option Strict On | Explicit Binding
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    ''' <summary>
    ''' Public alias method returning the currently active focus component from the selection manager.
    ''' FIXED: Added explicit member method to permanently resolve external caller compilation blocks.
    ''' </summary>
    Public Function GetActiveFocusShape() As Cls_Base_Shape
        ' Query the authoritative selection manager to pull the top active focus shape
        If Me._selectionMgr IsNot Nothing Then
            Return Me._selectionMgr.TryGetActiveFocus()
        End If
        Return Nothing
    End Function

    ' Target File: Cls_Viewport.vb (Diagnostics Refresh Engine Pass)
    ' Explicit configuration locked for file scope | Option Strict On | Explicit Binding
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    ''' <summary>
    ''' Flushes live canvas tracking metrics and synchronizes external UI property panels.
    ''' FIXED: Provides an explicit member definition to permanently clear compilation blocks.
    ''' </summary>
    Friend Sub ForceDiagnosticsUpdate()
        ' 1. Fetch real-time hardware mouse coordinates mapped to world canvas space
        Dim localScreen As Point = Me.PointToClient(Cursor.Position)
        Dim localWorld As PointF = Me._transformer.TransformScreenToWorld(localScreen, Me._panOffset, Me._zoomFactor)

        ' 2. Intercept the root MDI parent workspace container frame type-safely
        Dim targetForm As Form = Me.FindForm()
        If targetForm IsNot Nothing AndAlso targetForm.MdiParent IsNot Nothing Then
            Dim parentRoot As IMdiParentBridge = TryCast(targetForm.MdiParent, IMdiParentBridge)

            ' 3. Flush the live selected shape collection straight out to your Explorer and Property Grids
            If parentRoot?.WorkspaceLayout IsNot Nothing Then
                Dim selectedList As List(Of Cls_Base_Shape) = Me._selectionMgr.SelectedShapes
                parentRoot.WorkspaceLayout.MyExplorer.InspectShapesGroup(selectedList)
                parentRoot.MdiAction.SynchronizeExplorerHierarchyTree()
            End If
        End If

        ' 4. Force an immediate layout redraw pass onto the screen glass canvas
        Me.Invalidate()
    End Sub

    ' Target File: Cls_Viewport.vb (Component Framing Navigation Pass)
    ' Explicit configuration locked for file scope | Option Strict On | Explicit Binding
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    ''' <summary>
    ''' Centers the viewport camera frame directly onto a single targeted schematic component.
    ''' Automatically calculates and adjusts the zoom scale factor and pan offsets dynamically.
    ''' </summary>
    Public Sub FocusAndZoomIntoComponent(ByVal targetShape As Cls_Base_Shape)
        If targetShape Is Nothing Then Exit Sub

        ' 1. Clear active selection arrays and force highlight focus onto the target element
        Me._selectionMgr.Clear()
        Me._selectionMgr.SelectShape(targetShape, False)

        ' 2. Read layout metrics straight out of your clean, unrotated bounding box envelope
        Dim r As RectangleF = targetShape.Bounds
        Dim marginPadding As Single = 100.0F

        ' 3. Calculate target scale adjustments based on current viewport window boundaries
        Dim targetScaleX As Single = (Convert.ToSingle(Me.Width) - marginPadding) / r.Width
        Dim targetScaleY As Single = (Convert.ToSingle(Me.Height) - marginPadding) / r.Height
        Me._zoomFactor = Math.Max(0.2F, Math.Min(5.0F, Math.Min(targetScaleX, targetScaleY)))

        ' 4. Center Panning Formula: Shift offsets to place the shape center in the middle of the pane
        Dim centerX As Double = (Convert.ToDouble(Me.Width) - Convert.ToDouble(r.Width) * Convert.ToDouble(Me._zoomFactor)) / 2.0
        Dim centerY As Double = (Convert.ToDouble(Me.Height) - Convert.ToDouble(r.Height) * Convert.ToDouble(Me._zoomFactor)) / 2.0

        Me._panOffset.X = Convert.ToSingle(centerX - Convert.ToDouble(r.X) * Convert.ToDouble(Me._zoomFactor))
        Me._panOffset.Y = Convert.ToSingle(centerY - Convert.ToDouble(r.Y) * Convert.ToDouble(Me._zoomFactor))

        ' 5. Command focus activation and push a synchronized diagnostics layout refresh pass
        Me.Focus()
        Me.ForceDiagnosticsUpdate()
    End Sub

    ' Target File: Cls_Viewport.vb (State Machine Mutator Pass)
    ' Explicit configuration locked for file scope | Option Strict On | Explicit Binding
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    ''' <summary>
    ''' Explicitly overrides the active state engine configuration tracking token.
    ''' FIXED: Added explicit member method to permanently clear testing framework compilation blocks.
    ''' </summary>
    Public Sub UpdateViewportState(ByVal targetState As ViewportState)
        ' Force change the internal private tracking token to the requested configuration state
        Me._currentState = targetState

        ' Invalidate the control surface to ensure overlays redraw instantly under the new state rules
        Me.Invalidate()
    End Sub

    ' Target File: Cls_Viewport.vb (Rotation Initialization Pass)
    ' Explicit configuration locked for file scope | Option Strict On | Explicit Binding
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    ''' <summary>
    ''' Instantly switches the canvas state tracking parameters into an active rotation state block.
    ''' FIXED: Provides an explicit member definition to permanently resolve compilation blocks.
    ''' </summary>
    Sub InitiateActiveRotation(ByVal targetShape As Cls_Base_Shape)
        If targetShape Is Nothing Then Exit Sub

        ' 1. Transform the active tracking state matrix node securely into rotation mode
        Me._currentState = ViewportState.Rotating

        ' 2. Swap the viewport display cursor icon to indicate an active angular pivot action gesture
        Me.Cursor = Cursors.UpArrow

        ' 3. Force an immediate screen redraw pass to render your orange-red crosshairs overlays fluidly
        Me.Invalidate()
    End Sub


#Region "New"


#Region "Properties & Core Overrides"

    'Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
    '        MyBase.OnMouseDown(e)
    '        Me.Focus()

    '        If e.Button = MouseButtons.Middle OrElse (e.Button = MouseButtons.Left AndAlso (ModifierKeys And Keys.Alt) = Keys.Alt AndAlso Me._selectionMgr.Count = 0) Then
    '            Me._currentState = ViewportState.Panning : Me._startMousePos = e.Location : Me.Cursor = Cursors.NoMove2D : Exit Sub
    '        End If

    '        If e.Button = MouseButtons.Left Then
    '            Me.Capture = True
    '            Dim worldPt As PointF = Me._transformer.TransformScreenToWorld(e.Location, Me._panOffset, Me._zoomFactor)
    '            Me.EvaluateLeftMouseDownStateTransitionMatrix(worldPt)
    '        End If
    '    End Sub

    Protected Overrides Sub OnMouseMove(ByVal e As MouseEventArgs)
            MyBase.OnMouseMove(e)
            Dim worldPt As PointF = Me._transformer.TransformScreenToWorld(e.Location, Me._panOffset, Me._zoomFactor)

            Select Case Me._currentState
                Case ViewportState.Panning : Me.ExecutePanningDrag(e.Location)
                Case ViewportState.GroupDragging : Me.ExecuteGroupDraggingPipeline(worldPt)
                Case ViewportState.Resizing : Me.ExecuteResizingPipeline(worldPt)
                Case ViewportState.Rotating : Me.ExecuteRotatingPipeline(worldPt)
                Case ViewportState.MarqueeSelecting : Me._selectionMgr.MarqueeCurrentPoint = worldPt : Me.Invalidate()
                Case ViewportState.Idle : Me._resizeMgr.UpdateHoverCursor(worldPt, Me._selectionMgr.TryGetActiveFocus(), Me._zoomFactor, Me)
            End Select
        End Sub

        Protected Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)
            MyBase.OnMouseUp(e)
            Me.Capture = False

            If Me._currentState = ViewportState.MarqueeSelecting Then
                Me._selectionMgr.FinalizeMarqueeSelection(Me._canvasData.SchematicComponents, (ModifierKeys And Keys.Shift) = Keys.Shift)
            End If

            Me._currentState = ViewportState.Idle
            Me.Cursor = Cursors.Default
            Me.Invalidate()
        End Sub


#End Region

    Private Sub EvaluateLeftMouseDownStateTransitionMatrix(ByVal worldPt As PointF)

        Dim focusGate As Cls_Base_Shape = Me._selectionMgr.TryGetActiveFocus()
        Dim isCtrlDown As Boolean = (ModifierKeys And Keys.Control) = Keys.Control

            ' Constraint 1: Check handles blindly if Control is active
            If isCtrlDown AndAlso focusGate IsNot Nothing Then
                Dim handle As ResizeHandle = Me._resizeMgr.HitTestHandles(worldPt, focusGate, Me._zoomFactor)
                If handle <> ResizeHandle.None Then
                    Me._resizeMgr.LockActiveHandle(handle)
                    Me._currentState = ViewportState.Resizing : Me._worldDragStart = worldPt : Exit Sub
                End If
            End If

            ' Constraint 2: Check standard shape hits or switch cleanly to marquee selection mode
            Dim hitGate As Cls_Base_Shape = Me._canvasData.HitTestShapes(worldPt, Me._transformer)
            If hitGate IsNot Nothing Then
                Me._selectionMgr.SelectShape(hitGate, (ModifierKeys And Keys.Shift) = Keys.Shift)
                Me._currentState = ViewportState.GroupDragging : Me._worldDragStart = worldPt
            Else
                If Not (ModifierKeys And Keys.Shift) = Keys.Shift Then Me._selectionMgr.Clear()
                Me._selectionMgr.IsMarqueeSelecting = True
                Me._selectionMgr.MarqueeStartPoint = worldPt
                Me._selectionMgr.MarqueeCurrentPoint = worldPt
                Me._currentState = ViewportState.MarqueeSelecting
            End If
            Me.Invalidate()
        End Sub

        Private Sub ExecutePanningDrag(ByVal currentScreenPos As Point)
            ' Calculate screen pixel deltas directly to bypass camera scaling multiplication loops
            Dim dx As Single = Convert.ToSingle(currentScreenPos.X - Me._startMousePos.X)
            Dim dy As Single = Convert.ToSingle(currentScreenPos.Y - Me._startMousePos.Y)

            Me._panOffset.X += dx
            Me._panOffset.Y += dy
            Me._startMousePos = currentScreenPos
            Me.Invalidate()
        End Sub

        Private Sub ExecuteGroupDraggingPipeline(ByVal worldPt As PointF)
            Dim focusGate As Cls_Base_Shape = Me._selectionMgr.TryGetActiveFocus()
            If focusGate Is Nothing Then Exit Sub

            Dim dx As Single = worldPt.X - Me._worldDragStart.X
            Dim dy As Single = worldPt.Y - Me._worldDragStart.Y

            If Me._grid.IsSnapEnabled Then
                Dim target As New PointF(focusGate.Location.X + dx, focusGate.Location.Y + dy)
                Dim snapped As PointF = Me._grid.SnapPoint(target)
                dx = snapped.X - focusGate.Location.X : dy = snapped.Y - focusGate.Location.Y
            End If

            If dx <> 0.0F OrElse dy <> 0.0F Then
                focusGate.ApplyDeltaTransform(dx, dy)
                Me._worldDragStart = worldPt : Me.Invalidate()
            End If
        End Sub

        Private Sub ExecuteResizingPipeline(ByVal worldPt As PointF)
            Dim focusGate As Cls_Base_Shape = Me._selectionMgr.TryGetActiveFocus()
            If focusGate Is Nothing Then Exit Sub

            Me._resizeMgr.ExecuteResizeStep(worldPt, focusGate, Me._grid)
            Me.Invalidate()
        End Sub

    'Private Sub ExecuteRotatingPipeline(ByVal worldPt As PointF)
    '    Dim focusGate As Cls_Base_Shape = Me._selectionMgr.TryGetActiveFocus()
    '    If focusGate Is Nothing Then Exit Sub

    '    Me._rotationMgr.ExecuteRotationStep(worldPt, focusGate)
    '    Me.Invalidate()
    'End Sub


#End Region

    '#Region "Properties & Core Overrides"

    '        Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
    '        MyBase.OnMouseDown(e)
    '        Me.Focus()
    '        If e.Button = MouseButtons.Left Then
    '            Me.Capture = True
    '            Dim worldPt As PointF = Me._transformer.TransformScreenToWorld(e.Location, Me._panOffset, Me._zoomFactor)
    '            Me.EvaluateLeftMouseDownStateTransitionMatrix(worldPt)
    '        End If
    '    End Sub

    '    Protected Overrides Sub OnMouseMove(ByVal e As MouseEventArgs)
    '        MyBase.OnMouseMove(e)
    '        Dim worldPt As PointF = Me._transformer.TransformScreenToWorld(e.Location, Me._panOffset, Me._zoomFactor)

    '        ' Strict Execution routing isolated to current state flags exclusively
    '        Select Case Me._currentState
    '            Case ViewportState.GroupDragging : Me.ExecuteGroupDraggingPipeline(worldPt)
    '            Case ViewportState.Resizing : Me.ExecuteResizingPipeline(worldPt)
    '            Case ViewportState.Rotating : Me.ExecuteRotatingPipeline(worldPt)
    '            Case ViewportState.Idle : Me._resizeMgr.UpdateHoverCursor(worldPt, Me._selectionMgr.TryGetActiveFocus(), Me._zoomFactor, Me)
    '        End Select
    '    End Sub

    '    Protected Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)
    '        MyBase.OnMouseUp(e)
    '        If e.Button = MouseButtons.Left Then
    '            Me.Capture = False
    '            Me.RequestStateTransition(ViewportState.Idle, PointF.Empty)
    '        End If
    '    End Sub
    '#End Region

    '    ''' <summary>
    '    ''' Authoritative state transition matrix gatekeeper enforcing strict system constraints.
    '    ''' </summary>
    '    Private Sub RequestStateTransition(ByVal nextState As ViewportState, ByVal worldAnchor As PointF)
    '        ' Enforce lifecycle entry/exit cleanups upon transition validation
    '        If nextState = ViewportState.Idle Then
    '            Me.Cursor = Cursors.Default
    '        End If

    '        Me._currentState = nextState
    '        Me._worldDragStart = worldAnchor
    '        Me.Invalidate()
    '    End Sub

    '    ''' <summary>
    '    ''' Evaluates the primary mouse down matrix to choose the next legal interaction state.
    '    ''' </summary>
    '    Private Sub EvaluateLeftMouseDownStateTransitionMatrix(ByVal worldPt As PointF)
    '        Dim focusGate As Cls_Base_Shape = Me._selectionMgr.TryGetActiveFocus()
    '        Dim isCtrlDown As Boolean = (ModifierKeys And Keys.Control) = Keys.Control
    '        Dim isAltDown As Boolean = (ModifierKeys And Keys.Alt) = Keys.Alt

    '        ' Constraint 1: Check if the user is explicitly targeting a resize handle
    '        If isCtrlDown AndAlso focusGate IsNot Nothing Then
    '            Dim handle As ResizeHandle = Me._resizeMgr.HitTestHandles(worldPt, focusGate, Me._zoomFactor)
    '            If handle <> ResizeHandle.None Then
    '                Me._resizeMgr.LockActiveHandle(handle)
    '                Me.RequestStateTransition(ViewportState.Resizing, worldPt) : Exit Sub
    '            End If
    '        End If

    '        ' Constraint 2: Check if the user is explicitly triggering an unrotated pivot rotation
    '        If isAltDown AndAlso focusGate IsNot Nothing Then
    '            Me.RequestStateTransition(ViewportState.Rotating, worldPt) : Exit Sub
    '        End If

    '        ' Constraint 3: Perform standard component hit-testing to engage dragging operations
    '        Me.ProcessStandardSelectionOrDragMatrix(worldPt)
    '    End Sub

    '    Private Sub ProcessStandardSelectionOrDragMatrix(ByVal worldPt As PointF)
    '        Dim hitGate As Cls_Base_Shape = Me._canvasData.HitTestShapes(worldPt, Me._transformer)

    '        If hitGate IsNot Nothing Then
    '            Me._selectionMgr.SelectShape(hitGate, (ModifierKeys And Keys.Shift) = Keys.Shift)
    '            Me.RequestStateTransition(ViewportState.GroupDragging, worldPt)
    '        Else
    '            Me._selectionMgr.Clear()
    '            Me.RequestStateTransition(ViewportState.Idle, worldPt)
    '        End If
    '    End Sub

    '    Private Sub ExecuteGroupDraggingPipeline(ByVal worldPt As PointF)
    '        Dim focusGate As Cls_Base_Shape = Me._selectionMgr.TryGetActiveFocus()
    '        If focusGate Is Nothing Then Exit Sub

    '        Dim dx As Single = worldPt.X - Me._worldDragStart.X
    '        Dim dy As Single = worldPt.Y - Me._worldDragStart.Y

    '        If Me._grid.IsSnapEnabled Then
    '            Dim target As New PointF(focusGate.Location.X + dx, focusGate.Location.Y + dy)
    '            Dim snapped As PointF = Me._grid.SnapPoint(target)
    '            dx = snapped.X - focusGate.Location.X : dy = snapped.Y - focusGate.Location.Y
    '        End If

    '        If dx <> 0.0F OrElse dy <> 0.0F Then
    '            focusGate.ApplyDeltaTransform(dx, dy)
    '            Me._worldDragStart = worldPt : Me.Invalidate()
    '        End If
    '    End Sub

    '    Private Sub ExecuteResizingPipeline(ByVal worldPt As PointF)
    '        Dim focusGate As Cls_Base_Shape = Me._selectionMgr.TryGetActiveFocus()
    '        If focusGate Is Nothing Then Exit Sub

    '        ' Delegate the unrotated sizing calculation entirely to the stateless manager tool
    '        Me._resizeMgr.ExecuteResizeStep(worldPt, focusGate, Me._grid)
    '        Me.Invalidate()
    '    End Sub

    '    Private Sub ExecuteRotatingPipeline(ByVal worldPt As PointF)
    '        Dim focusGate As Cls_Base_Shape = Me._selectionMgr.TryGetActiveFocus()
    '        If focusGate Is Nothing Then Exit Sub

    '        ' Delegate the unrotated angle tracking calculations to the specialized rotation tool
    '        Me._rotationMgr.ExecuteRotationStep(worldPt, focusGate)
    '        Me.Invalidate()
    '    End Sub
    '    'zzzzzzzzzzzzzzzzzzzzzz

    '    Private Sub CommitGridSnappedResize(ByVal shape As Cls_Base_Shape, ByVal targetX As Single, ByVal targetY As Single, ByVal targetW As Single, ByVal targetH As Single)
    '        Dim finalX As Single = targetX : Dim finalY As Single = targetY
    '        Dim finalW As Single = targetW : Dim finalH As Single = targetH

    '        If Me._grid.IsSnapEnabled Then
    '            ' Snap both the upper-left insertion anchor and the bottom-right extreme edges cleanly
    '            Dim snappedTopLeft As PointF = Me._grid.SnapPoint(New PointF(targetX, targetY))
    '            Dim snappedBottomRight As PointF = Me._grid.SnapPoint(New PointF(targetX + targetW, targetY + targetH))

    '            finalX = snappedTopLeft.X : finalY = snappedTopLeft.Y
    '            finalW = snappedBottomRight.X - finalX
    '            finalH = snappedBottomRight.Y - finalY
    '        End If

    '        ' FIXED: Commit both layout updates atomically to stop property update stutters!
    '        shape.ApplyAtomicGeometryTransform(New PointF(finalX, finalY), finalW, finalH)

    '        Me.FlagActiveViewportAsModified(True)
    '        Me.Invalidate()
    '    End Sub

    '    Public Sub ProcessResizeDrag(ByVal rawWorldPoint As PointF)
    '        Dim activeComponent As Cls_Base_Shape = Me.TryGetActiveFocusShape()
    '        Dim handle As ResizeHandle = Me._resizeMgr.ActiveHandle
    '        If activeComponent Is Nothing OrElse handle = ResizeHandle.None Then Exit Sub

    '        ' 1. Calculate un-snapped movement delta since the last frame tick
    '        Dim rawDx As Single = rawWorldPoint.X - Me._worldDragStart.X
    '        Dim rawDy As Single = rawWorldPoint.Y - Me._worldDragStart.Y
    '        Me._worldDragStart = rawWorldPoint

    '        ' 2. Initialize proposed mutations using the current raw bounds layout envelopes
    '        Dim nextW As Single = activeComponent.Bounds.Width
    '        Dim nextH As Single = activeComponent.Bounds.Height
    '        Dim nextX As Single = activeComponent.Location.X
    '        Dim nextY As Single = activeComponent.Location.Y

    '        ' 3. Process horizontal changes based on whether left-side or right-side handles are held
    '        If handle = ResizeHandle.TopRight OrElse handle = ResizeHandle.MiddleRight OrElse handle = ResizeHandle.BottomRight Then
    '            nextW += rawDx
    '        ElseIf handle = ResizeHandle.TopLeft OrElse handle = ResizeHandle.MiddleLeft OrElse handle = ResizeHandle.BottomLeft Then
    '            nextW -= rawDx : nextX += rawDx
    '        End If

    '        ' Pass control to a segregated helper method to cleanly respect your 25-line limit barrier rule
    '        Me.EvaluateVerticalResizeDeltas(activeComponent, handle, nextX, nextY, nextW, nextH, rawDy)
    '    End Sub

    '    ''' <summary>
    '    ''' Segregated execution loop evaluating vertical deltas to preserve strict method length rules.
    '    ''' </summary>
    '    Private Sub EvaluateVerticalResizeDeltas(ByVal shape As Cls_Base_Shape, ByVal handle As ResizeHandle, ByVal x As Single, ByVal y As Single, ByVal w As Single, ByVal h As Single, ByVal dy As Single)
    '        Dim nextY As Single = y
    '        Dim nextH As Single = h

    '        ' 4. Process vertical changes based on whether top-side or bottom-side handles are held
    '        If handle = ResizeHandle.BottomLeft OrElse handle = ResizeHandle.BottomCenter OrElse handle = ResizeHandle.BottomRight Then
    '            nextH += dy
    '        ElseIf handle = ResizeHandle.TopLeft OrElse handle = ResizeHandle.TopCenter OrElse handle = ResizeHandle.TopRight Then
    '            nextH -= dy : nextY += dy
    '        End If

    '        ' 5. Commit mutations straight down to the grid snapping engine compositor
    '        Me.CommitGridSnappedResize(shape, x, nextY, w, nextH)
    '    End Sub

    '    '''' <summary>
    '    '''' High-precision compositor ensuring all multi-directional mutations snap square onto grid intersection lanes.
    '    '''' </summary>
    '    'Private Sub CommitGridSnappedResize(ByVal shape As Cls_Base_Shape, ByVal targetX As Single, ByVal targetY As Single, ByVal targetW As Single, ByVal targetH As Single)
    '    '    Dim finalX As Single = targetX : Dim finalY As Single = targetY
    '    '    Dim finalW As Single = targetW : Dim finalH As Single = targetH

    '    '    If Me._grid.IsSnapEnabled Then
    '    '        ' Snap both the upper-left insertion anchor and the bottom-right extreme edges
    '    '        Dim snappedTopLeft As PointF = Me._grid.SnapPoint(New PointF(targetX, targetY))
    '    '        Dim snappedBottomRight As PointF = Me._grid.SnapPoint(New PointF(targetX + targetW, targetY + targetH))

    '    '        finalX = snappedTopLeft.X : finalY = snappedTopLeft.Y
    '    '        finalW = Math.Max(40.0F, snappedBottomRight.X - finalX)
    '    '        finalH = Math.Max(30.0F, snappedBottomRight.Y - finalY)
    '    '    End If

    '    '    ' 6. Commit the final structural location and size modifications type-safely
    '    '    shape.Location = New PointF(finalX, finalY)
    '    '    shape.ApplyResizeTransform(finalW, finalH)

    '    '    Me.FlagActiveViewportAsModified(True)
    '    '    Me.Invalidate()
    '    'End Sub

    '    'yyyyyyyyyyyyyyyyyyyyyy
    '    'Protected Overrides Sub OnMouseMove(ByVal e As MouseEventArgs)
    '    '    MyBase.OnMouseMove(e)
    '    '    Dim rawWorldPoint As PointF = Me.ConvertScreenToWorld(e.Location)

    '    '    ' HARD LATCH: If a dedicated drag/resize operation is active, bypass ALL hover checks and focus changes!
    '    '    Select Case Me._currentState
    '    '        Case ViewportState.Resizing
    '    '            Me.ProcessResizeDrag(rawWorldPoint)
    '    '        Case ViewportState.GroupDragging
    '    '            Me.ExecuteGroupMoveTranslation(rawWorldPoint)
    '    '        Case ViewportState.Panning
    '    '            Me.ExecutePanningDrag(e)
    '    '        Case ViewportState.MarqueeSelecting
    '    '            Me._selectionMgr.MarqueeCurrentPoint = rawWorldPoint : Me.Invalidate()
    '    '        Case ViewportState.Rotating
    '    '            Me.ExecuteRotationHandlingLoop(rawWorldPoint)
    '    '        Case ViewportState.Drawing
    '    '            Me._worldCurrentPoint = Me._grid.SnapPoint(rawWorldPoint) : Me.Invalidate()
    '    '        Case ViewportState.Idle
    '    '            ' Focus checks and cursor swapping are ONLY allowed if the canvas is completely at rest
    '    '            Me.ProcessCursorHoverStates(rawWorldPoint)
    '    '    End Select

    '    '    Me.UpdateDiagnosticsHUD(e.Location, rawWorldPoint)
    '    'End Sub

    '    'Public Sub ProcessResizeDrag(ByVal rawWorldPoint As PointF)
    '    '    Dim activeComponent As Cls_Base_Shape = Me.TryGetActiveFocusShape()
    '    '    If activeComponent Is Nothing OrElse Me._resizeMgr.ActiveHandle = ResizeHandle.None Then Exit Sub

    '    '    ' 1. Telemetry Log: Check tracking anchors and incoming cursor values
    '    '    Diagnostics.Debug.WriteLine($"[RESIZE DRAG] Input Pt: ({rawWorldPoint.X:F2}, {rawWorldPoint.Y:F2}) | Anchor Pt: ({Me._worldDragStart.X:F2}, {Me._worldDragStart.Y:F2})")

    '    '    ' 2. Calculate continuous raw mouse movement steps from the last poll frame tick
    '    '    Dim rawDx As Single = rawWorldPoint.X - Me._worldDragStart.X
    '    '    Dim rawDy As Single = rawWorldPoint.Y - Me._worldDragStart.Y

    '    '    ' 3. Instantly step the trajectory tracking baseline anchor forward to track the mouse
    '    '    Me._worldDragStart = rawWorldPoint

    '    '    ' 4. Accumulate proposed dimensions before any grid clamping logic filters it
    '    '    Dim proposedW As Single = activeComponent.Bounds.Width + rawDx
    '    '    Dim proposedH As Single = activeComponent.Bounds.Height + rawDy

    '    '    Diagnostics.Debug.WriteLine($" -> Deltas: dX={rawDx:F2}, dY={rawDy:F2} | Current Size: W={activeComponent.Bounds.Width:F1}, H={activeComponent.Bounds.Height:F1} | Proposed Size: W={proposedW:F1}, H={proposedH:F1}")

    '    '    ' Route down to your unified sizing compositor helper to maintain 25-line structural bounds
    '    '    Me.CommitGridSnappedResize(activeComponent, proposedW, proposedH)
    '    'End Sub

    '    ' Target File: Cls_Viewport.vb (Resize Compositor Instrumentation)
    '    ' Strictly type-safe under Option Strict On and resides safely under the 25-line limit.

    '    Private Sub CommitGridSnappedResize(ByVal shape As Cls_Base_Shape, ByVal targetW As Single, ByVal targetH As Single)
    '        Dim finalWidth As Single = targetW
    '        Dim finalHeight As Single = targetH

    '        If Me._grid.IsSnapEnabled Then
    '            ' Mode A: Grid Clamping is Active
    '            Dim projectedTarget As PointF = Me._grid.SnapPoint(New PointF(shape.Bounds.X + targetW, shape.Bounds.Y + targetH))
    '            finalWidth = projectedTarget.X - shape.Bounds.X
    '            finalHeight = projectedTarget.Y - shape.Bounds.Y
    '            Diagnostics.Debug.WriteLine($" -> [SNAP ON] Clamped to Cell -> Final Size: W={finalWidth:F1}, H={finalHeight:F1}")
    '        Else
    '            ' Mode B: Free Pixel Scaling is Active
    '            Diagnostics.Debug.WriteLine($" -> [SNAP OFF] Free Pixel Scaling -> Final Size: W={finalWidth:F1}, H={finalHeight:F1}")
    '        End If

    '        ' Commit structural dimension modifications straight into the base abstract model shape
    '        shape.ApplyResizeTransform(finalWidth, finalHeight)

    '        Me.FlagActiveViewportAsModified(True)
    '        Me.Invalidate()
    '    End Sub


    '    '''' <summary>
    '    '''' Tracks continuous sizing trajectory vectors and clamps dimensions safely to the 20px grid.
    '    '''' Completely eliminates dimensional jumping and stutters under Option Strict On.
    '    '''' </summary>
    '    'Public Sub ProcessResizeDrag(ByVal rawWorldPoint As PointF)
    '    '    Dim activeComponent As Cls_Base_Shape = Me.TryGetActiveFocusShape()
    '    '    If activeComponent Is Nothing OrElse Me._resizeMgr.ActiveHandle = ResizeHandle.None Then Exit Sub

    '    '    ' 1. Calculate un-snapped movement delta since the last tracking frame tick
    '    '    Dim rawDx As Single = rawWorldPoint.X - Me._worldDragStart.X
    '    '    Dim rawDy As Single = rawWorldPoint.Y - Me._worldDragStart.Y

    '    '    ' 2. Instantly advance the anchor baseline tracking variable to match the current cursor frame
    '    '    Me._worldDragStart = rawWorldPoint

    '    '    ' 3. Calculate where the tracking node boundary edge wants to slide in absolute world space
    '    '    Dim targetWidth As Single = activeComponent.Bounds.Width + rawDx
    '    '    Dim targetHeight As Single = activeComponent.Bounds.Height + rawDy

    '    '    ' 4. Route coordinates through a clean, unrotated helper method to respect your 25-line ceiling
    '    '    Me.CommitGridSnappedResize(activeComponent, targetWidth, targetHeight)
    '    'End Sub

    '    '''' <summary>
    '    '''' Segregated execution loop snapping target layout dimensions precisely onto grid intersection lines.
    '    '''' </summary>
    '    'Private Sub CommitGridSnappedResize(ByVal shape As Cls_Base_Shape, ByVal targetW As Single, ByVal targetH As Single)
    '    '    ' 5. Pass the absolute target edge footprint directly through your 20px snapping calculator
    '    '    Dim projectedTarget As PointF = Me._grid.SnapPoint(New PointF(shape.Bounds.X + targetW, shape.Bounds.Y + targetH))

    '    '    ' 6. Derive the true mathematical width and height needed to snap perfectly onto that grid line
    '    '    Dim finalWidth As Single = projectedTarget.X - shape.Bounds.X
    '    '    Dim finalHeight As Single = projectedTarget.Y - shape.Bounds.Y

    '    '    ' 7. Commit size modifications type-safely via our core transformation routine
    '    '    shape.ApplyResizeTransform(finalWidth, finalHeight)

    '    '    Me.FlagActiveViewportAsModified(True)
    '    '    Me.Invalidate()
    '    'End Sub

    '    Private Sub ProcessLeftMouseDown(ByVal e As MouseEventArgs)
    '        Dim rawWorldPoint As PointF = Me.ConvertScreenToWorld(e.Location)
    '        Dim activeFocus As Cls_Base_Shape = Me.TryGetActiveFocusShape()

    '        Dim isCtrlDown As Boolean = (ModifierKeys And Keys.Control) = Keys.Control
    '        Dim isAltDown As Boolean = (ModifierKeys And Keys.Alt) = Keys.Alt

    '        If isCtrlDown AndAlso activeFocus IsNot Nothing Then
    '            ' Match the exact zoom-invariant target sizing metrics used in your hover trackers
    '            Dim targetHandleSize As Single = 6.0F / Me._zoomFactor
    '            Dim handle As ResizeHandle = Me._resizeMgr.HitTestHandles(rawWorldPoint, activeFocus, targetHandleSize)

    '            If handle <> ResizeHandle.None Then
    '                Me._currentState = ViewportState.Resizing
    '                Me._resizeMgr.StartResize(handle, rawWorldPoint)
    '                Exit Sub
    '            End If
    '        End If

    '        If isAltDown AndAlso activeFocus IsNot Nothing Then
    '            Me.InitiateActiveRotation(activeFocus) : Exit Sub
    '        End If

    '        Me.EvaluateStandardCanvasInteractionRoute(rawWorldPoint)
    '    End Sub

    '    Private Sub ProcessCursorHoverStates(ByVal rawWorldPoint As PointF)
    '        Dim activeFocus As Cls_Base_Shape = Me.TryGetActiveFocusShape()
    '        Dim isCtrlDown As Boolean = (ModifierKeys And Keys.Control) = Keys.Control

    '        If activeFocus IsNot Nothing AndAlso isCtrlDown Then
    '            ' Calculate a stable, high-visibility hit-test box size that scales inversely with zoom
    '            Dim targetHandleSize As Single = 6.0F / Me._zoomFactor

    '            ' Query the blind handle hit tester using your raw world cursor position coordinates
    '            Dim hoverHandle As ResizeHandle = Me._resizeMgr.HitTestHandles(rawWorldPoint, activeFocus, targetHandleSize)

    '            ' Command the manager to push the matching double-headed resize icon onto the viewport
    '            Me._resizeMgr.UpdateViewportCursor(hoverHandle, Me)
    '        ElseIf Me.Cursor <> Cursors.Default AndAlso Me._currentState = ViewportState.Idle Then
    '            Me.Cursor = Cursors.Default
    '        End If
    '    End Sub

    '    'Private Sub ProcessCursorHoverStates(ByVal rawWorldPoint As PointF)
    '    '    Dim activeFocus As Cls_Base_Shape = Me.TryGetActiveFocusShape()
    '    '    Dim isCtrlDown As Boolean = (ModifierKeys And Keys.Control) = Keys.Control

    '    '    ' FIXED: Suppress tool-specific blocks so resize handles track even when a Gate tool is active
    '    '    If activeFocus IsNot Nothing AndAlso isCtrlDown Then
    '    '        ' Query the blind handle hit tester using the raw unrotated world cursor coordinate
    '    '        Dim hoverHandle As ResizeHandle = Me._resizeMgr.HitTestHandles(rawWorldPoint, activeFocus, Me._zoomFactor)

    '    '        ' Command the manager to push the correct double-headed resize icon onto the viewport
    '    '        Me._resizeMgr.UpdateViewportCursor(hoverHandle, Me)
    '    '    ElseIf Me.Cursor <> Cursors.Default AndAlso Me._currentState = ViewportState.Idle Then
    '    '        Me.Cursor = Cursors.Default
    '    '    End If
    '    'End Sub

    '    'Private Sub ProcessLeftMouseDown(ByVal e As MouseEventArgs)
    '    '    Dim rawWorldPoint As PointF = Me.ConvertScreenToWorld(e.Location)
    '    '    Dim activeFocus As Cls_Base_Shape = Me.TryGetActiveFocusShape()

    '    '    Dim isCtrlDown As Boolean = (ModifierKeys And Keys.Control) = Keys.Control
    '    '    Dim isAltDown As Boolean = (ModifierKeys And Keys.Alt) = Keys.Alt

    '    '    ' 1. STRATEGY ALIGNED SIZING INTERCEPTION: Pass raw world vectors to the blind manager!
    '    '    If isCtrlDown AndAlso activeFocus IsNot Nothing Then
    '    '        ' Check handles against the raw, unrotated bounding limits matching our strategy layout
    '    '        Dim handle As ResizeHandle = Me._resizeMgr.HitTestHandles(rawWorldPoint, activeFocus, Me._zoomFactor)

    '    '        If handle <> ResizeHandle.None Then
    '    '            Me._currentState = ViewportState.Resizing
    '    '            Me._resizeMgr.StartResize(handle, rawWorldPoint)
    '    '            Exit Sub ' Lock state instantly and terminate click processing hooks
    '    '        End If
    '    '    End If

    '    '    If isAltDown AndAlso activeFocus IsNot Nothing Then
    '    '        Me.InitiateActiveRotation(activeFocus) : Exit Sub
    '    '    End If

    '    '    Me.EvaluateStandardCanvasInteractionRoute(rawWorldPoint)
    '    'End Sub

    '    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    '    ''' <summary>
    '    ''' Paints a thick Royal Blue selection box wrapped directly around a component's edges.
    '    ''' STRATEGY ALIGNED: Renders cleanly inside the unrotated camera canvas tracking context.
    '    ''' </summary>
    '    Private Sub RenderSelectionOutline(ByVal g As Graphics, ByVal zoom As Single, ByVal shape As Cls_Base_Shape)
    '        If g Is Nothing OrElse shape Is Nothing Then Exit Sub

    '        ' 1. Establish a high-visibility, zoom-invariant selection pen vector
    '        Using selectPen As New Pen(Color.RoyalBlue, 2.5F / zoom) With {.DashStyle = DashStyle.Dash}
    '            ' 2. FIXED STRATEGY: Draw directly inside the unrotated workspace space!
    '            ' Reverts to using raw, unrotated world coordinates to clear out all matrix offsets.
    '            Dim r As RectangleF = shape.Bounds

    '            g.DrawRectangle(selectPen, r.X, r.Y, r.Width, r.Height)
    '        End Using
    '    End Sub

    '    ' Target File: Cls_Viewport.vb (Unified Unrotated Overlays Pass)
    '    ' Strictly type-safe under Option Strict On and resides safely under the 25-line limit.

    '    Private Sub RenderSelectionOverlays(ByVal g As Graphics, ByVal cameraMatrix As Matrix)
    '        Dim activeShape As Cls_Base_Shape = Me.TryGetActiveFocusShape()

    '        ' 1. FORCE STRATEGY: Lock the graphics transformer context to the unrotated camera matrix upfront!
    '        ' This shields all subsequent overlay drawing commands from rotational pollution.
    '        g.Transform = cameraMatrix

    '        ' 2. Draw the unrotated marquee outline boxes wrapped around all selected elements
    '        For Each shape As Cls_Base_Shape In Me._selectionMgr.SelectedShapes
    '            Me.RenderSelectionOutline(g, Me._zoomFactor, shape)
    '        Next

    '        If activeShape IsNot Nothing Then
    '            Dim isCtrlDown As Boolean = (ModifierKeys And Keys.Control) = Keys.Control

    '            ' 3. Draw the unrotated resize handle boxes natively over raw coordinates
    '            If Me._currentState = ViewportState.Resizing OrElse isCtrlDown Then
    '                Me._resizeMgr.DrawResizeHandles(g, Me._zoomFactor, activeShape)
    '            End If

    '            ' 4. Draw the unrotated crosshairs and metrics labels centered on raw coordinates
    '            If Me._currentState = ViewportState.Rotating Then
    '                Me._rotationMgr.DrawRotationOverlayMetrics(g, Me._zoomFactor, activeShape)
    '            End If
    '        End If
    '    End Sub

    '    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
    '        MyBase.OnPaint(e)
    '        Dim g As Graphics = e.Graphics

    '        ' 1. Establish the clean, unrotated base camera workspace matrix context authority
    '        Using cameraMatrix As Matrix = Me._transformer.CreateCameraMatrix(Me._panOffset, Me._zoomFactor)
    '            g.Transform = cameraMatrix

    '            ' 2. Calculate culling limits and render the grid manager lines natively
    '            Dim visibleBounds As RectangleF = Me._transformer.CalculateWorldBounds(Me._panOffset, Me._zoomFactor, Me.Width, Me.Height)
    '            Me._grid.Render(g, Me._zoomFactor, visibleBounds)

    '            ' 3. Render logic gate components via the protected coordinate wrapper
    '            Me.RenderProtectedWorldCanvas(g, cameraMatrix)

    '            ' 4. FIXED: Pass cameraMatrix directly into overlays to protect unrotated handle drawing!
    '            Me.RenderSelectionOverlays(g, cameraMatrix)

    '            ' 5. Draw active structural multi-selection box window overlays
    '            If Me._currentState = ViewportState.MarqueeSelecting Then
    '                Me.DrawMarqueeOverlayWindow(g)
    '            End If

    '            Me.ExecuteDiagnosticTelemetryLogging()
    '        End Using
    '        g.ResetTransform()
    '    End Sub

    '    ''' <summary>
    '    ''' Accumulates raw mouse micro-movements continuously and forces sharp 20px grid lane snaps.
    '    ''' </summary>
    '    Private Sub ExecuteGroupMoveTranslation(ByVal rawWorldPoint As PointF)
    '        Dim gate As Cls_Base_Shape = Me.GetActiveFocusComponent()
    '        If gate Is Nothing Then Exit Sub

    '        ' 1. Calculate the raw, un-snapped distance delta relative to your LOCKED drag baseline anchor
    '        Dim rawDx As Single = rawWorldPoint.X - Me._worldDragStart.X
    '        Dim rawDy As Single = rawWorldPoint.Y - Me._worldDragStart.Y

    '        ' 2. Project the shape's absolute hypothetical next landing position in world space coordinates
    '        Dim hypotheticalTarget As New PointF(gate.Location.X + rawDx, gate.Location.Y + rawDy)

    '        ' 3. Pass that macro position directly through your 20px grid snapping engine matrix
    '        Dim snappedTarget As PointF = Me._grid.SnapPoint(hypotheticalTarget)

    '        ' 4. Derive the exact distance step required to land perfectly on the cell intersection lane
    '        Dim finalDeltaX As Single = snappedTarget.X - gate.Location.X
    '        Dim finalDeltaY As Single = snappedTarget.Y - gate.Location.Y

    '        If finalDeltaX <> 0.0F OrElse finalDeltaY <> 0.0F Then
    '            ' Displace all selected components type-safely via our core transformation routine
    '            gate.ApplyDeltaTransform(finalDeltaX, finalDeltaY)

    '            ' FIXED: Advance your anchor baseline ONLY by the distance the component actually shifted!
    '            ' This preserves sub-pixel mouse drifts across frames until they break past grid thresholds.
    '            Me._worldDragStart.X += finalDeltaX
    '            Me._worldDragStart.Y += finalDeltaY
    '        End If

    '        ' Force an immediate visual repaint on every message frame to update handle overlays fluidly
    '        Me.Invalidate()
    '    End Sub


    '    'Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
    '    '    MyBase.OnMouseDown(e)
    '    '    Me.Focus()

    '    '    If Me._currentState <> ViewportState.Idle Then Exit Sub

    '    '    If e.Button = MouseButtons.Middle Then
    '    '        Me._currentState = ViewportState.Panning : Me._startMousePos = e.Location : Me.Cursor = Cursors.NoMove2D
    '    '    ElseIf e.Button = MouseButtons.Left Then
    '    '        ' FIXED: Lock hardware capture and let ProcessLeftMouseDown control the state exclusively
    '    '        Me.Capture = True
    '    '        Me.ProcessLeftMouseDown(e)
    '    '    End If
    '    'End Sub

    '    'Private Sub ProcessLeftMouseDown(ByVal e As MouseEventArgs)
    '    '    Dim rawWorldPoint As PointF = Me.ConvertScreenToWorld(e.Location)
    '    '    Dim activeFocus As Cls_Base_Shape = Me.TryGetActiveFocusShape()

    '    '    Dim isCtrlDown As Boolean = (ModifierKeys And Keys.Control) = Keys.Control
    '    '    Dim isAltDown As Boolean = (ModifierKeys And Keys.Alt) = Keys.Alt

    '    '    ' 1. SHORTCUT MODIFIERS FIRST: Handle resizing and rotations exclusively
    '    '    If isCtrlDown AndAlso activeFocus IsNot Nothing Then
    '    '        Dim localPt As PointF = Me._transformer.TransformWorldToLocalShapeSpace(rawWorldPoint, activeFocus)
    '    '        Dim handle As ResizeHandle = Me._resizeMgr.HitTestHandles(localPt, activeFocus, Me._zoomFactor)
    '    '        If handle <> ResizeHandle.None Then
    '    '            Me._currentState = ViewportState.Resizing : Me._resizeMgr.StartResize(handle, rawWorldPoint) : Exit Sub
    '    '        End If
    '    '    End If
    '    '    If isAltDown AndAlso activeFocus IsNot Nothing Then
    '    '        Me.InitiateActiveRotation(activeFocus) : Exit Sub
    '    '    End If

    '    '    ' 2. PRIORITY CANVAS ROUTING: Direct to clean, isolated interaction logic tracks
    '    '    Me.EvaluateStandardCanvasInteractionRoute(rawWorldPoint)
    '    'End Sub

    '    Private Sub EvaluateStandardCanvasInteractionRoute(ByVal rawWorldPoint As PointF)
    '        ' Run our verified local bounds hit-test pass across the master schematic component list
    '        Dim hitGate As Cls_Base_Shape = Me._canvasData.HitTestShapes(rawWorldPoint, Me._transformer)
    '        Dim isShiftDown As Boolean = (ModifierKeys And Keys.Shift) = Keys.Shift

    '        If hitGate IsNot Nothing Then
    '            ' 3. FIXED: Component hit confirmed! Force GroupDragging and EXIT IMMEDIATELY to block fallthroughs
    '            Me._currentState = ViewportState.GroupDragging
    '            Me._worldDragStart = rawWorldPoint
    '            Me._selectionMgr.SelectShape(hitGate, isShiftDown)
    '        Else
    '            ' 4. Blank canvas click: Securely initialize marquee selection crossing tracks
    '            Me.InitializeMarqueeTracking(rawWorldPoint, isShiftDown)
    '        End If
    '    End Sub


    '    Public Function ConvertScreenToWorld(ByVal screenPoint As Point) As PointF
    '        Return Me._transformer.TransformScreenToWorld(screenPoint, Me._panOffset, Me._zoomFactor)
    '    End Function


    '    '''' <summary>
    '    '''' Accumulates raw mouse movements smoothly and slides all selected gate components.
    '    '''' FIXED: Passes rawWorldPoint into the final argument slot to fulfill the parameter contract.
    '    '''' </summary>
    '    'Private Sub ExecuteGroupMoveTranslation(ByVal rawWorldPoint As PointF)
    '    '    Dim gate As Cls_Base_Shape = Me.GetActiveFocusComponent()
    '    '    If gate Is Nothing Then Exit Sub

    '    '    ' 1. Calculate the un-snapped movement delta since the last frame tick
    '    '    Dim rawDx As Single = rawWorldPoint.X - Me._worldDragStart.X
    '    '    Dim rawDy As Single = rawWorldPoint.Y - Me._worldDragStart.Y

    '    '    ' 2. Evaluate movement vectors based on whether grid snapping is active
    '    '    If Me._grid.IsSnapEnabled Then
    '    '        ' Predict hypothetical next landing coordinate and clamp it to the 20px grid layout
    '    '        Dim targetPos As New PointF(gate.Location.X + rawDx, gate.Location.Y + rawDy)
    '    '        Dim snappedTarget As PointF = Me._grid.SnapPoint(targetPos)

    '    '        Dim finalDx As Single = snappedTarget.X - gate.Location.X
    '    '        Dim finalDy As Single = snappedTarget.Y - gate.Location.Y

    '    '        ' FIXED: Added rawWorldPoint as the required third parameter argument
    '    '        Me.ApplyMovementDelta(finalDx, finalDy, rawWorldPoint)
    '    '    Else
    '    '        ' FIXED: Added rawWorldPoint as the required third parameter argument
    '    '        Me.ApplyMovementDelta(rawDx, rawDy, rawWorldPoint)
    '    '    End If
    '    'End Sub


    '    '''' <summary>
    '    '''' High-priority mouse down controller overriding active tool configurations when clicking existing items.
    '    '''' Fully type-safe under Option Strict On and resides safely under the 25-line limit.
    '    '''' </summary>
    '    'Private Sub ProcessLeftMouseDown(ByVal e As MouseEventArgs)
    '    '    Dim rawWorldPoint As PointF = Me.ConvertScreenToWorld(e.Location)
    '    '    Dim activeFocus As Cls_Base_Shape = Me.TryGetActiveFocusShape()

    '    '    Dim isCtrlDown As Boolean = (ModifierKeys And Keys.Control) = Keys.Control
    '    '    Dim isAltDown As Boolean = (ModifierKeys And Keys.Alt) = Keys.Alt

    '    '    ' 1. SHORTCUT INTERCEPTION: Process handle resizing and rotations first
    '    '    If isCtrlDown AndAlso activeFocus IsNot Nothing Then
    '    '        Dim localPt As PointF = Me._transformer.TransformWorldToLocalShapeSpace(rawWorldPoint, activeFocus)
    '    '        Dim handle As ResizeHandle = Me._resizeMgr.HitTestHandles(localPt, activeFocus, Me._zoomFactor)
    '    '        If handle <> ResizeHandle.None Then
    '    '            Me._currentState = ViewportState.Resizing : Me._resizeMgr.StartResize(handle, rawWorldPoint) : Exit Sub
    '    '        End If
    '    '    End If
    '    '    If isAltDown AndAlso activeFocus IsNot Nothing Then
    '    '        Me.InitiateActiveRotation(activeFocus) : Exit Sub
    '    '    End If

    '    '    ' 2. PRIORITY OVERRIDE ROUTE: Force component hit-testing before checking tool types
    '    '    Me.EvaluateStandardCanvasInteractionRoute(rawWorldPoint)
    '    'End Sub

    '    '''' <summary>
    '    '''' Forces all blank space clicks into a marquee selection track to stop accidental drawing state switches.
    '    '''' Fully type-safe under Option Strict On and resides safely under the 25-line limit.
    '    '''' </summary>
    '    'Private Sub EvaluateStandardCanvasInteractionRoute(ByVal rawWorldPoint As PointF)
    '    '    ' 1. Check if an item exists under the current cursor point using your local-bounds hit-tester
    '    '    Dim hitGate As Cls_Base_Shape = Me._canvasData.HitTestShapes(rawWorldPoint, Me._transformer)
    '    '    Dim isShiftDown As Boolean = (ModifierKeys And Keys.Shift) = Keys.Shift

    '    '    If hitGate IsNot Nothing Then
    '    '        ' A component was successfully detected! Force GroupDragging mode and lock anchors
    '    '        Me._currentState = ViewportState.GroupDragging
    '    '        Me._worldDragStart = rawWorldPoint
    '    '        Me._selectionMgr.SelectShape(hitGate, isShiftDown)
    '    '    Else
    '    '        ' 2. FIXED: Clicking empty space ALWAYS initializes a marquee selection or stays idle.
    '    '        ' This completely breaks the fallthrough route to ViewportState.Drawing!
    '    '        Me.InitializeMarqueeTracking(rawWorldPoint, isShiftDown)
    '    '    End If
    '    'End Sub


    '    'Protected Overrides Sub OnMouseMove(ByVal e As MouseEventArgs)
    '    '    MyBase.OnMouseMove(e)
    '    '    Dim rawWorldPoint As PointF = Me.ConvertScreenToWorld(e.Location)

    '    '    ' 1. HARD GUARDRAIL: Mouse movement loops are forbidden from altering states out of Idle!
    '    '    Select Case Me._currentState
    '    '        Case ViewportState.Panning
    '    '            Me.ExecutePanningDrag(e)
    '    '        Case ViewportState.MarqueeSelecting
    '    '            Me._selectionMgr.MarqueeCurrentPoint = rawWorldPoint : Me.Invalidate()
    '    '        Case ViewportState.Resizing
    '    '            Me.ProcessResizeDrag(rawWorldPoint)
    '    '        Case ViewportState.Rotating
    '    '            Me.ExecuteRotationHandlingLoop(rawWorldPoint)
    '    '        Case ViewportState.Drawing
    '    '            ' Grid snap updates are only processed if a click down explicitly initiated drawing mode
    '    '            Me._worldCurrentPoint = Me._grid.SnapPoint(rawWorldPoint) : Me.Invalidate()
    '    '        Case ViewportState.GroupDragging
    '    '            Me.ExecuteGroupMoveTranslation(rawWorldPoint)
    '    '        Case ViewportState.Idle
    '    '            ' Idle state remains strictly locked to visual cursor hover shape updates only
    '    '            Me.ProcessCursorHoverStates(rawWorldPoint)
    '    '    End Select

    '    '    Me.UpdateDiagnosticsHUD(e.Location, rawWorldPoint)
    '    'End Sub

    '    'Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
    '    '    MyBase.OnMouseDown(e)
    '    '    Me.Focus()

    '    '    ' 1. HARD GUARDRAIL: Block any tracking initialization if the system is already busy
    '    '    If Me._currentState <> ViewportState.Idle Then Exit Sub

    '    '    If e.Button = MouseButtons.Middle Then
    '    '        Me._currentState = ViewportState.Panning
    '    '        Me._startMousePos = e.Location
    '    '        Me.Cursor = Cursors.NoMove2D
    '    '    ElseIf e.Button = MouseButtons.Left Then
    '    '        ' 2. FIXED: Lock hardware capture but do NOT set _currentState here!
    '    '        ' Let your ProcessLeftMouseDown method handle states cleanly based on hit-tests.
    '    '        Me.Capture = True
    '    '        Me.ProcessLeftMouseDown(e)
    '    '    End If
    '    'End Sub

    '    ''' <summary>
    '    ''' Segregated interaction dispatcher separating structural clicks from sketching canvas modes.
    '    ''' </summary>
    '    Private Sub RouteStandardCanvasGestures(ByVal rawWorldPoint As PointF, ByVal activeFocus As Cls_Base_Shape)
    '        Dim hitGate As Cls_Base_Shape = Me._canvasData.HitTestShapes(rawWorldPoint, Me._transformer)
    '        Dim isShiftDown As Boolean = (ModifierKeys And Keys.Shift) = Keys.Shift

    '        If hitGate IsNot Nothing Then
    '            ' 2. HIGH PRIORITY: An item was clicked! Force GroupDragging mode and lock the anchor position
    '            Me._currentState = ViewportState.GroupDragging : Me._worldDragStart = rawWorldPoint
    '            Me._selectionMgr.SelectShape(hitGate, isShiftDown)
    '        ElseIf Me._currentTool = CanvasTool.SelectPointer OrElse activeFocus IsNot Nothing Then
    '            ' 3. FIXED STATE PROTECTION: If a shape is active, block Drawing fallbacks entirely!
    '            ' Clicking blank space initiates a marquee selection window instead of spawning ghost shapes.
    '            Me.InitializeMarqueeTracking(rawWorldPoint, isShiftDown)
    '        Else
    '            ' 4. Pure empty canvas click with zero selections active: Open up drawing mode
    '            Me._currentState = ViewportState.Drawing
    '            Me._worldStartPoint = Me._grid.SnapPoint(rawWorldPoint)
    '            Me._worldCurrentPoint = Me._worldStartPoint
    '        End If
    '    End Sub

    '    Private Sub DrawPreviewGeometry(ByVal g As Graphics)
    '        If g Is Nothing Then Exit Sub
    '        g.SmoothingMode = SmoothingMode.AntiAlias

    '        Using previewPen As New Pen(Color.Red, 1.5! / Me._zoomFactor)
    '            Dim minX As Single = Math.Min(Me._worldStartPoint.X, Me._worldCurrentPoint.X)
    '            Dim minY As Single = Math.Min(Me._worldStartPoint.Y, Me._worldCurrentPoint.Y)
    '            Dim w As Single = Math.Max(80.0F, Math.Abs(Me._worldStartPoint.X - Me._worldCurrentPoint.X))
    '            Dim h As Single = Math.Max(50.0F, Math.Abs(Me._worldStartPoint.Y - Me._worldCurrentPoint.Y))

    '            ' FIXED: Enforces an immutable default footprint size to prevent text overlapping artifacts
    '            Dim bounds As New RectangleF(minX, minY, w, h)
    '            Me.RenderActiveToolPreview(g, previewPen, bounds)
    '        End Using
    '    End Sub


    '    ''' <summary>
    '    ''' Evaluates cursor pointer selections and blocks accidental fallback coordinate mutations.
    '    ''' Fully type-safe under Option Strict On and resides safely under the 25-line limit.
    '    ''' </summary>
    '    Private Sub ProcessPointerSelection(ByVal worldPt As PointF)
    '        Dim activeFocus As Cls_Base_Shape = Me.TryGetActiveFocusShape()
    '        Dim isCtrlDown As Boolean = (ModifierKeys And Keys.Control) = Keys.Control
    '        Dim isAltDown As Boolean = (ModifierKeys And Keys.Alt) = Keys.Alt

    '        ' 1. HARD RESIZE INTERCEPTION: If Control is active, ONLY allow handle transformations
    '        If isCtrlDown AndAlso activeFocus IsNot Nothing Then
    '            Dim localPt As PointF = Me._transformer.TransformWorldToLocalShapeSpace(worldPt, activeFocus)
    '            Dim handle As ResizeHandle = Me._resizeMgr.HitTestHandles(localPt, activeFocus, Math.Max(0.5F, Me._zoomFactor))

    '            If handle <> ResizeHandle.None Then
    '                Me._currentState = ViewportState.Resizing
    '                Me._resizeMgr.StartResize(handle, worldPt)
    '            End If
    '            ' FIXED: Exit immediately! Stop the routine from falling through to selection toggles.
    '            Exit Sub
    '        End If

    '        ' 2. HARD ROTATION INTERCEPTION: If Alt is active, ONLY allow rotation overrides
    '        If isAltDown Then
    '            If activeFocus IsNot Nothing Then Me.InitiateActiveRotation(activeFocus)
    '            Exit Sub
    '        End If

    '        ' 3. STANDARD ROUTE: Proceed with standard selection changes only if no shortcuts are active
    '        Me.ProcessShapeSelectionToggles(worldPt)
    '    End Sub

    '    Private Sub ExecuteRotationHandlingLoop(ByVal rawWorldPoint As PointF)
    '        Dim activeFocus As Cls_Base_Shape = Me.TryGetActiveFocusShape()
    '        If activeFocus IsNot Nothing Then
    '            Me._rotationMgr.ProcessRotation(rawWorldPoint, activeFocus, Me.GetActiveStatusBar())
    '            Me.Invalidate()
    '        End If
    '    End Sub




    '    ''' <summary>
    '    ''' Segregated helper routine keeping code complexity underneath your 25 operational line limit.
    '    ''' </summary>
    '    Private Sub EvaluateStandardCanvasClickRoute(ByVal rawWorldPoint As PointF)
    '        Dim hitGate As Cls_Base_Shape = Me._canvasData.HitTestShapes(rawWorldPoint, Me._transformer)
    '        Dim isShiftDown As Boolean = (ModifierKeys And Keys.Shift) = Keys.Shift

    '        If hitGate IsNot Nothing Then
    '            ' Move translations are only allowed if raw free gestures are performed
    '            Me._currentState = ViewportState.GroupDragging
    '            Me._worldDragStart = rawWorldPoint
    '            Me._selectionMgr.SelectShape(hitGate, isShiftDown)
    '        ElseIf Me._currentTool = CanvasTool.SelectPointer Then
    '            Me.InitializeMarqueeTracking(rawWorldPoint, isShiftDown)
    '        Else
    '            Me._currentState = ViewportState.Drawing
    '            Me._worldStartPoint = Me._grid.SnapPoint(rawWorldPoint)
    '            Me._worldCurrentPoint = Me._worldStartPoint
    '        End If
    '    End Sub



    '    'Protected Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)
    '    '    MyBase.OnMouseUp(e)

    '    '    ' Only alter state indicators if the matching left button trigger is physically released
    '    '    If e.Button = MouseButtons.Left Then
    '    '        Me.Capture = False
    '    '        Me.EvaluateLeftMouseUpStateTransition()
    '    '    End If

    '    '    Me.ForceDiagnosticsUpdate()
    '    'End Sub

    '    ''' <summary>
    '    ''' Segregated helper to isolate selection finalizations and preserve strict 25-line method boundaries.
    '    ''' </summary>
    '    Private Sub EvaluateLeftMouseUpStateTransition()
    '        Select Case Me._currentState
    '            Case ViewportState.Panning, ViewportState.Resizing, ViewportState.GroupDragging
    '                If Me._currentState = ViewportState.Resizing Then Me._resizeMgr.TerminateResize()
    '                Me._currentState = ViewportState.Idle : Me.Cursor = Cursors.Default

    '            Case ViewportState.Rotating
    '                Me._rotationMgr.TerminateRotation() : Me._currentState = ViewportState.Idle
    '                Me.Cursor = If((ModifierKeys And Keys.Alt) = Keys.Alt, Cursors.SizeAll, Cursors.Default)

    '            Case ViewportState.MarqueeSelecting
    '                Me._currentState = ViewportState.Idle : Me._selectionMgr.IsMarqueeSelecting = False
    '                Me._selectionMgr.FinalizeMarqueeSelection(Me._canvasData.SchematicComponents, (ModifierKeys And Keys.Shift) = Keys.Shift)

    '            Case ViewportState.Drawing
    '                Me._currentState = ViewportState.Idle : Me.CommitDrawingShape()
    '        End Select
    '    End Sub



    '    Private Sub ApplyMovementDelta(ByVal dx As Single, ByVal dy As Single, ByVal currentMouse As PointF)
    '        ' This helper handles atomic property updates only; rendering invalidation is offloaded above
    '        If dx <> 0.0F OrElse dy <> 0.0F Then
    '            For Each item As Object In Me._selectionMgr.SelectedShapes
    '                If TypeOf item Is Cls_Base_Shape Then
    '                    DirectCast(item, Cls_Base_Shape).ApplyDeltaTransform(dx, dy)
    '                End If
    '            Next
    '            Me._worldDragStart = currentMouse
    '        End If
    '    End Sub



    '    ''' <summary>
    '    ''' Synchronizes external UI property grids and hierarchy trees with live data metrics.
    '    ''' FIXED: Refreshes component data matrices BEFORE pushing updates to prevent UI lockups.
    '    ''' </summary>
    '    Friend Sub ForceDiagnosticsUpdate()
    '        ' 1. Re-evaluate real-time cursor positions relative to current camera tracks
    '        Dim localScreen As Point = Me.PointToClient(Cursor.Position)
    '        Dim localWorld As PointF = Me.ConvertScreenToWorld(localScreen)
    '        Me.UpdateDiagnosticsHUD(localScreen, localWorld)

    '        ' 2. Intercept the root workspace frame container bridge type-safely
    '        Dim childForm As Form = Me.FindForm()
    '        If childForm?.MdiParent IsNot Nothing Then
    '            Dim mainRoot As IMdiParentBridge = TryCast(childForm.MdiParent, IMdiParentBridge)

    '            If mainRoot?.WorkspaceLayout IsNot Nothing Then
    '                ' 3. FIXED: Flush and pass the live, updated component properties collection
    '                mainRoot.WorkspaceLayout.MyExplorer.InspectShapesGroup(Me._selectionMgr.SelectedShapes)

    '                ' 4. Force the hierarchy TreeView browser to rebuild its terminal text nodes
    '                mainRoot.MdiAction.SynchronizeExplorerHierarchyTree()
    '            End If
    '        End If

    '        ' 5. Force an immediate layout redraw pass onto the screen glass canvas
    '        Me.Invalidate()
    '    End Sub



    '#Region "Viewport Core Operations & Lookups"

    '    Public Function GetActiveFocusComponent() As Cls_Base_Shape
    '        If Me._selectionMgr IsNot Nothing AndAlso Me._selectionMgr.Count > 0 Then
    '            Dim activeItem As Object = Me._selectionMgr.SelectedShapes(Me._selectionMgr.Count - 1)

    '            If TypeOf activeItem Is Cls_Base_Shape Then
    '                Return DirectCast(activeItem, Cls_Base_Shape)
    '            End If
    '        End If
    '        Return Nothing
    '    End Function

    '    Friend Function TryGetActiveFocusShape() As Cls_Base_Shape
    '        Return Me.GetActiveFocusComponent()
    '    End Function

    '    Friend Function GetActiveFocusShape() As Cls_Base_Shape
    '        Return Me.GetActiveFocusComponent()
    '    End Function

    '    Private Sub FlagActiveViewportAsModified(ByVal isModified As Boolean)
    '        Dim childForm As Form = Me.FindForm()
    '        If childForm IsNot Nothing AndAlso childForm.MdiParent IsNot Nothing Then
    '            Dim mainRoot As IMdiParentBridge = TryCast(childForm.MdiParent, IMdiParentBridge)

    '            If mainRoot IsNot Nothing AndAlso childForm.Tag IsNot Nothing Then
    '                Dim documentId As Integer = CInt(childForm.Tag)
    '                mainRoot.MdiAction.ProjectManager.FlagDocumentAsDirty(documentId, isModified)
    '            End If
    '        End If
    '    End Sub




    '    Friend Sub InitiateActiveRotation(ByVal targetShape As Cls_Base_Shape)
    '        If targetShape IsNot Nothing Then
    '            Me._currentState = ViewportState.Rotating
    '            Me._rotationMgr.StartRotation(targetShape)
    '            Me.Cursor = Cursors.SizeAll
    '            Me.Invalidate()
    '        End If
    '    End Sub

    '    Friend Sub UpdateViewportState(ByVal newState As ViewportState)
    '        Me._currentState = newState
    '    End Sub

    '#End Region

    '#Region "Rendering Pipeline Overrides"

    '    'Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
    '    '    MyBase.OnPaint(e)
    '    '    Dim g As Graphics = e.Graphics

    '    '    Using cameraMatrix As Matrix = Me._transformer.CreateCameraMatrix(Me._panOffset, Me._zoomFactor)
    '    '        g.Transform = cameraMatrix

    '    '        Dim visibleBounds As RectangleF = Me._transformer.CalculateWorldBounds(Me._panOffset, Me._zoomFactor, Me.Width, Me.Height)
    '    '        Me._grid.Render(g, Me._zoomFactor, visibleBounds)

    '    '        Me.RenderProtectedWorldCanvas(g, cameraMatrix)
    '    '        Me.RenderSelectionOverlays(g)

    '    '        If Me._currentState = ViewportState.MarqueeSelecting Then Me.DrawMarqueeOverlayWindow(g)
    '    '        If Me._currentState = ViewportState.Drawing Then Me.DrawPreviewGeometry(g)
    '    '        Me.ExecuteDiagnosticTelemetryLogging()
    '    '    End Using
    '    '    g.ResetTransform()
    '    'End Sub

    '    Private Sub RenderProtectedWorldCanvas(ByVal g As Graphics, ByVal cameraMatrix As Matrix)
    '        If Me._canvasData.SchematicComponents IsNot Nothing Then
    '            For Each gate As Cls_Base_Shape In Me._canvasData.SchematicComponents
    '                Me._transformer.ExecuteSafeShapeRender(g, cameraMatrix, gate)
    '            Next
    '        End If
    '    End Sub

    '    Private Sub RenderSelectionOverlays(ByVal g As Graphics)
    '        Dim activeShape As Cls_Base_Shape = Me.TryGetActiveFocusShape()

    '        For Each shape As Cls_Base_Shape In Me._selectionMgr.SelectedShapes
    '            Me.RenderSelectionOutline(g, Me._zoomFactor, shape)
    '        Next

    '        If activeShape IsNot Nothing Then
    '            Dim isCtrlDown As Boolean = (ModifierKeys And Keys.Control) = Keys.Control

    '            Using localMatrix As Matrix = Me._transformer.CreateLocalShapeMatrix(Me._panOffset, Me._zoomFactor, activeShape.RotationAngle, activeShape.CalculateGeometricCenter())
    '                g.Transform = localMatrix
    '                Me.ExecuteHandlePlacementDebugTrace(localMatrix, activeShape)

    '                If Me._currentState = ViewportState.Resizing OrElse isCtrlDown Then
    '                    Me._resizeMgr.DrawResizeHandles(g, Me._zoomFactor, activeShape)
    '                End If
    '                If Me._currentState = ViewportState.Rotating Then
    '                    Me._rotationMgr.DrawRotationOverlayMetrics(g, Me._zoomFactor, activeShape)
    '                End If
    '            End Using
    '        End If
    '    End Sub

    '    'Private Sub RenderSelectionOutline(ByVal g As Graphics, ByVal zoom As Single, ByVal shape As Cls_Base_Shape)
    '    '    If g Is Nothing OrElse shape Is Nothing Then Exit Sub

    '    '    Using selectPen As New Pen(Color.RoyalBlue, 2.5F / zoom) With {.DashStyle = DashStyle.Dash}
    '    '        Dim halfW As Single = shape.Bounds.Width / 2.0F
    '    '        Dim halfH As Single = shape.Bounds.Height / 2.0F

    '    '        Using localMatrix As Matrix = Me._transformer.CreateLocalShapeMatrix(Me._panOffset, zoom, shape.RotationAngle, shape.CalculateGeometricCenter())
    '    '            g.Transform = localMatrix
    '    '            g.DrawRectangle(selectPen, -halfW, -halfH, shape.Bounds.Width, shape.Bounds.Height)
    '    '        End Using
    '    '    End Using
    '    'End Sub

    '#End Region

    '#Region "Mouse Interception, Placement & Movement Logic"


    '    ''' <summary>
    '    ''' Segregated helper executing component hits or routing straight to drawing paths.
    '    ''' </summary>
    '    Private Sub EvaluateComponentClickRoute(ByVal rawWorldPoint As PointF)
    '        Dim hitGate As Cls_Base_Shape = Me._canvasData.HitTestShapes(rawWorldPoint, Me._transformer)
    '        Dim isShiftDown As Boolean = (ModifierKeys And Keys.Shift) = Keys.Shift

    '        If hitGate IsNot Nothing Then
    '            ' 2. Component hit-test verified: Enforce structural move translations
    '            Me._currentState = ViewportState.GroupDragging
    '            Me._worldDragStart = rawWorldPoint
    '            Me._selectionMgr.SelectShape(hitGate, isShiftDown)
    '        ElseIf Me._currentTool = CanvasTool.SelectPointer Then
    '            Me.InitializeMarqueeTracking(rawWorldPoint, isShiftDown)
    '        Else
    '            ' 3. Pure empty canvas click: Open up the component sketching engine
    '            Me._currentState = ViewportState.Drawing
    '            Me._worldStartPoint = Me._grid.SnapPoint(rawWorldPoint)
    '            Me._worldCurrentPoint = Me._worldStartPoint
    '        End If
    '    End Sub


    '    Private Sub ProcessShapeSelectionToggles(ByVal worldPt As PointF)
    '        ' 1. Telemetry Log: Verify click input registration coordinates
    '        Diagnostics.Debug.WriteLine($"[HIT TEST] Input: Cursor World Pt = ({worldPt.X:F1}, {worldPt.Y:F1})")

    '        ' 2. Invoke the dynamic hit-test scan pass
    '        Dim hitShape As Cls_Base_Shape = Me._canvasData.HitTestShapes(worldPt, Me._transformer)
    '        Dim isShiftDown As Boolean = (ModifierKeys And Keys.Shift) = Keys.Shift

    '        If hitShape IsNot Nothing Then
    '            Diagnostics.Debug.WriteLine($"[HIT TEST] SUCCESS: Detected Component Type = {hitShape.GetType().Name} | Local Bounds = X:{hitShape.Bounds.X:F0}, Y:{hitShape.Bounds.Y:F0}, W:{hitShape.Bounds.Width:F0}, H:{hitShape.Bounds.Height:F0}")

    '            If Me._selectionMgr.Contains(hitShape) AndAlso Not isShiftDown Then
    '                Me._currentState = ViewportState.GroupDragging : Me._worldDragStart = worldPt : Exit Sub
    '            End If

    '            Me._selectionMgr.SelectShape(hitShape, isShiftDown)
    '            Diagnostics.Debug.WriteLine($"[HIT TEST] Selection Manager Sync: Count after select = {Me._selectionMgr.Count}")
    '            If Not isShiftDown Then Me.EvaluateSelectionModifierKeys(hitShape, worldPt)
    '        Else
    '            ' 3. Telemetry Log: Capture failure state explicitly
    '            Diagnostics.Debug.WriteLine("[HIT TEST] FAIL: No component intersection registered at target coordinate.")
    '            Me.InitializeMarqueeTracking(worldPt, isShiftDown)
    '        End If
    '    End Sub


    '    Private Sub ExecuteRotationMove(ByVal rawWorldPoint As PointF)
    '        Dim activeFocus As Cls_Base_Shape = Me.TryGetActiveFocusShape()
    '        If activeFocus IsNot Nothing Then
    '            Me._rotationMgr.ProcessRotation(rawWorldPoint, activeFocus, Me.GetActiveStatusBar())
    '            Me.Invalidate()
    '        End If
    '    End Sub

    '#End Region

    '#Region "Component Resizing Framework"

    '    '''' <summary>
    '    '''' Grabs the focused gate and projects mouse coordinates down to the specialized resize manager.
    '    '''' Satisfies Option Strict On and resides safely under your 25 operational line limit.
    '    '''' </summary>
    '    'Public Sub ProcessResizeDrag(ByVal rawWorldPoint As PointF)
    '    '    ' 1. Pull the active logic gate component using your verified viewport lookup utility
    '    '    Dim activeComponent As Cls_Base_Shape = Me.GetActiveFocusComponent()

    '    '    If activeComponent IsNot Nothing Then
    '    '        ' 2. Intercept raw coordinates and pass them through the local unified grid snapping matrix
    '    '        Dim snappedWorld As PointF = Me._grid.SnapPoint(rawWorldPoint)

    '    '        ' 3. Project the snapped world coordinate point into the shape's local unrotated space
    '    '        Dim localMouse As PointF = Me._transformer.TransformWorldToLocalShapeSpace(snappedWorld, activeComponent)

    '    '        ' 4. FIXED: Delegate the update to your specialized manager which tracks handles natively
    '    '        Dim wasModified As Boolean = Me._resizeMgr.ExecuteTransformStep(localMouse, activeComponent)

    '    '        If wasModified Then
    '    '            Me.FlagActiveViewportAsModified(True)
    '    '        End If
    '    '    End If
    '    'End Sub

    '    Private Sub ApplyComponentDeltaTransform(ByVal dx As Single, ByVal dy As Single, ByVal gate As Cls_Base_Shape)
    '        Dim currentBounds As RectangleF = gate.Bounds
    '        Dim nextW As Single = currentBounds.Width
    '        Dim nextH As Single = currentBounds.Height
    '        Dim locX As Single = gate.Location.X
    '        Dim locY As Single = gate.Location.Y
    '        If Me._resizeMgr.ActiveHandle = ResizeHandle.TopRight OrElse Me._resizeMgr.ActiveHandle = ResizeHandle.MiddleRight OrElse Me._resizeMgr.ActiveHandle = ResizeHandle.BottomRight Then
    '            nextW = currentBounds.Width + dx
    '        ElseIf Me._resizeMgr.ActiveHandle = ResizeHandle.TopLeft OrElse Me._resizeMgr.ActiveHandle = ResizeHandle.MiddleLeft OrElse Me._resizeMgr.ActiveHandle = ResizeHandle.BottomLeft Then
    '            nextW = currentBounds.Width - dx : locX += dx
    '        End If
    '        If Me._resizeMgr.ActiveHandle = ResizeHandle.BottomLeft OrElse Me._resizeMgr.ActiveHandle = ResizeHandle.BottomCenter OrElse Me._resizeMgr.ActiveHandle = ResizeHandle.BottomRight Then
    '            nextH = currentBounds.Height + dy
    '        ElseIf Me._resizeMgr.ActiveHandle = ResizeHandle.TopLeft OrElse Me._resizeMgr.ActiveHandle = ResizeHandle.TopCenter OrElse Me._resizeMgr.ActiveHandle = ResizeHandle.TopRight Then
    '            nextH = currentBounds.Height - dy : locY += dy
    '        End If
    '        gate.Location = New PointF(locX, locY)
    '        gate.ApplyResizeTransform(nextW, nextH)
    '        Me.FlagActiveViewportAsModified(True)
    '    End Sub

    '#End Region

    '#Region "Sketched Drawing & Active Preview Geometry"
    '    Private Sub CommitDrawingShape()
    '        Dim thick As Single = 2.0F / Me._zoomFactor
    '        Dim newShape As Cls_Base_Shape = Me.CreateShapeInstance(Me._currentTool, Me._worldStartPoint, Me._worldCurrentPoint, Color.FromArgb(30, 30, 30), thick)
    '        If newShape IsNot Nothing Then
    '            Me._canvasData.SchematicComponents.Add(newShape)
    '            Me.FlagActiveViewportAsModified(True)
    '        End If
    '        Me.Invalidate()
    '    End Sub
    '    Private Function CreateShapeInstance(ByVal tool As CanvasTool, ByVal startPt As PointF, ByVal endPt As PointF, ByVal col As Color, ByVal thick As Single) As Cls_Base_Shape
    '        Select Case tool
    '            Case CanvasTool.LogicGate : Return Me.InstantiateInteractiveGateSubclass(startPt)
    '            Case Else : Return Nothing
    '        End Select
    '    End Function
    '    Private Function InstantiateInteractiveGateSubclass(ByVal targetPos As PointF) As Cls_Base_Shape
    '        Select Case Me._activeGateType
    '            Case LogicGateType.AndGate : Return New Cls_Gate_And(targetPos)
    '            Case LogicGateType.NotGate : Return New Cls_Gate_Not(targetPos)
    '            Case Else : Return Nothing
    '        End Select
    '    End Function

    '    Private Sub RenderActiveToolPreview(ByVal g As Graphics, ByVal previewPen As Pen, ByVal bounds As RectangleF)
    '        Select Case Me._currentTool
    '            Case CanvasTool.LogicGate
    '                Dim finalW As Single = Math.Max(40.0F, bounds.Width)
    '                Dim finalH As Single = Math.Max(30.0F, bounds.Height)
    '                Dim adjBounds As New RectangleF(bounds.X, bounds.Y, finalW, finalH)
    '                previewPen.DashStyle = DashStyle.Dash
    '                g.DrawRectangle(previewPen, adjBounds.X, adjBounds.Y, adjBounds.Width, adjBounds.Height)
    '                Using textBrush As New SolidBrush(Color.Red)
    '                    Dim label As String = Me._activeGateType.ToString().ToUpper()
    '                    g.DrawString(label, Me.Font, textBrush, adjBounds.X + 4.0F, adjBounds.Y + 4.0F)
    '                End Using
    '        End Select
    '    End Sub
    '#End Region

    '#Region "Native OLE Drag-and-Drop Infrastructure"
    '    Private Sub OnViewportDragEnter(ByVal sender As Object, ByVal e As DragEventArgs)
    '        If e.Data.GetDataPresent(GetType(LogicGateType)) Then
    '            e.Effect = DragDropEffects.Copy
    '        Else
    '            e.Effect = DragDropEffects.None
    '        End If
    '    End Sub
    '    Private Sub OnViewportDragDrop(ByVal sender As Object, ByVal e As DragEventArgs)
    '        If e.Data.GetDataPresent(GetType(LogicGateType)) Then
    '            Dim droppedGateKind As LogicGateType = DirectCast(e.Data.GetData(GetType(LogicGateType)), LogicGateType)
    '            Dim screenPoint As New Point(e.X, e.Y)
    '            Dim clientMousePos As Point = Me.PointToClient(screenPoint)
    '            Dim rawWorldDropPoint As PointF = Me.ConvertScreenToWorld(clientMousePos)
    '            Dim snappedWorldPosition As PointF = Me._grid.SnapPoint(rawWorldDropPoint)
    '            Me.AddNewComponentToWorkspace(droppedGateKind, snappedWorldPosition)
    '        End If
    '    End Sub
    '    Private Sub AddNewComponentToWorkspace(ByVal gateKind As LogicGateType, ByVal targetPos As PointF)
    '        Dim newGate As Cls_Base_Shape = Nothing
    '        Select Case gateKind
    '            Case LogicGateType.AndGate : newGate = New Cls_Gate_And(targetPos)
    '            Case LogicGateType.NotGate : newGate = New Cls_Gate_Not(targetPos)
    '        End Select
    '        If newGate IsNot Nothing Then
    '            Me._canvasData.SchematicComponents.Add(newGate)
    '            Me.FlagActiveViewportAsModified(True)
    '            Me.Invalidate()
    '        End If
    '    End Sub
    '#End Region
    '#Region "Camera Matrices, Zooming, Panning & Keys"
    '    Public Sub ZoomAtMouse(ByVal screenMousePos As Point, ByVal scaleStep As Single)
    '        Dim targetZoom As Single = Math.Max(0.1F, Math.Min(10.0F, Me._zoomFactor * scaleStep))
    '        If targetZoom = Me._zoomFactor Then Exit Sub
    '        Dim deltaX As Single = (Convert.ToSingle(screenMousePos.X) - Me._panOffset.X) * (1.0F - scaleStep)
    '        Dim deltaY As Single = (Convert.ToSingle(screenMousePos.Y) - Me._panOffset.Y) * (1.0F - scaleStep)
    '        Me._panOffset.X += deltaX : Me._panOffset.Y += deltaY
    '        Me._zoomFactor = targetZoom : Me.Invalidate()
    '    End Sub
    '    Public Sub ZoomInAtCenter()
    '        Dim midX As Integer = CInt(Math.Round(Convert.ToDouble(Me.Width) / 2.0))
    '        Dim midY As Integer = CInt(Math.Round(Convert.ToDouble(Me.Height) / 2.0))
    '        Me.ZoomAtMouse(New Point(midX, midY), 1.2F)
    '    End Sub
    '    Public Sub ZoomOutAtCenter()
    '        Dim midX As Integer = CInt(Math.Round(Convert.ToDouble(Me.Width) / 2.0))
    '        Dim midY As Integer = CInt(Math.Round(Convert.ToDouble(Me.Height) / 2.0))
    '        Me.ZoomAtMouse(New Point(midX, midY), 0.8F)
    '    End Sub
    '    Public Sub ZoomFit()
    '        If Me._canvasData.SchematicComponents.Count = 0 Then
    '            Me._zoomFactor = 1.0F : Me._panOffset = New PointF(0.0F, 0.0F) : Me.Invalidate() : Exit Sub
    '        End If
    '        Dim extents As RectangleF = Me.CalculateShapeExtents()
    '        Dim targetScaleX As Single = (Convert.ToSingle(Me.Width) - 40.0F) / extents.Width
    '        Dim targetScaleY As Single = (Convert.ToSingle(Me.Height) - 40.0F) / extents.Height
    '        Me._zoomFactor = Math.Max(0.1F, Math.Min(10.0F, Math.Min(targetScaleX, targetScaleY)))
    '        Me._panOffset.X = (Convert.ToSingle(Me.Width) - extents.Width * Me._zoomFactor) / 2.0F - extents.X * Me._zoomFactor
    '        Me._panOffset.Y = (Convert.ToSingle(Me.Height) - extents.Height * Me._zoomFactor) / 2.0F - extents.Y * Me._zoomFactor
    '        Me.Invalidate()
    '    End Sub
    '    Private Function CalculateShapeExtents() As RectangleF
    '        Dim minX As Single = Single.MaxValue : Dim minY As Single = Single.MaxValue
    '        Dim maxX As Single = Single.MinValue : Dim maxY As Single = Single.MinValue
    '        For Each gate As Cls_Base_Shape In Me._canvasData.SchematicComponents
    '            minX = Math.Min(minX, gate.Bounds.X)
    '            minY = Math.Min(minY, gate.Bounds.Y)
    '            maxX = Math.Max(maxX, gate.Bounds.Right)
    '            maxY = Math.Max(maxY, gate.Bounds.Bottom)
    '        Next
    '        Return New RectangleF(minX, minY, maxX - minX, maxY - minY)
    '    End Function
    '    Protected Overrides Sub OnMouseWheel(ByVal e As MouseEventArgs)
    '        MyBase.OnMouseWheel(e)
    '        If Not Me.Focused OrElse Me._currentState <> ViewportState.Idle Then Exit Sub
    '        Me.ZoomAtMouse(e.Location, If(e.Delta > 0, 1.1F, 0.9F))
    '    End Sub
    '    Sub ExecutePanningDrag(ByVal e As MouseEventArgs)
    '        Dim screenDeltaX As Single = Convert.ToSingle(e.X - Me._startMousePos.X)
    '        Dim screenDeltaY As Single = Convert.ToSingle(e.Y - Me._startMousePos.Y)
    '        Me._panOffset.X += (screenDeltaX / Me._zoomFactor)
    '        Me._panOffset.Y += (screenDeltaY / Me._zoomFactor)
    '        Me._startMousePos = e.Location : Me.Invalidate()
    '    End Sub
    '    'Private Sub ProcessCursorHoverStates(ByVal rawWorldPoint As PointF)
    '    '    Dim activeFocus As Cls_Base_Shape = Me.TryGetActiveFocusShape()
    '    '    Dim isCtrlDown As Boolean = (ModifierKeys And Keys.Control) = Keys.Control
    '    '    If Me._currentTool = CanvasTool.SelectPointer AndAlso activeFocus IsNot Nothing AndAlso isCtrlDown Then
    '    '        Dim localPt As PointF = Me._transformer.TransformWorldToLocalShapeSpace(rawWorldPoint, activeFocus)
    '    '        Dim hoverHandle As ResizeHandle = Me._resizeMgr.HitTestHandles(localPt, activeFocus, Math.Max(0.5F, Me._zoomFactor))
    '    '        Me._resizeMgr.UpdateViewportCursor(hoverHandle, Me)
    '    '    ElseIf Me.Cursor <> Cursors.Default AndAlso Me._currentState = ViewportState.Idle Then
    '    '        Me.Cursor = Cursors.Default
    '    '    End If


    '    'End Sub


    '    ''' <summary>
    '    ''' Projects world space coordinates into the local unrotated space of a target component.
    '    ''' Satisfies Option Strict On and resides safely under your 25 operational line rule.
    '    ''' </summary>
    '    Private Function ConvertWorldToLocalShapeSpace(ByVal worldPoint As PointF, ByVal shape As Cls_Base_Shape) As PointF
    '        If shape Is Nothing OrElse shape.RotationAngle = 0.0F Then
    '            Return worldPoint
    '        End If

    '        Using rotationMatrix As New Matrix()
    '            ' Invert the rotation angle relative to the calculated geometric center
    '            rotationMatrix.RotateAt(-shape.RotationAngle, shape.CalculateGeometricCenter())

    '            Dim points As PointF() = {worldPoint}
    '            rotationMatrix.TransformPoints(points)

    '            Return points(0)
    '        End Using
    '    End Function

    '    Protected Overrides Sub OnKeyDown(ByVal e As KeyEventArgs)
    '        MyBase.OnKeyDown(e)
    '        Me._keyboardMgr.ProcessKeyDownTransaction(e)
    '    End Sub
    '    Protected Overrides Sub OnKeyUp(ByVal e As KeyEventArgs)
    '        MyBase.OnKeyUp(e)
    '        Me._keyboardMgr.ProcessKeyUpTransaction(e)
    '    End Sub
    '    Protected Overrides Function IsInputKey(ByVal keyData As Keys) As Boolean
    '        Dim pureKey As Keys = keyData And Keys.KeyCode
    '        If (keyData And Keys.Modifiers) <> 0 OrElse pureKey = Keys.Delete Then Return True
    '        Return MyBase.IsInputKey(keyData)
    '    End Function
    '#End Region

    '#Region "Diagnostics Telemetry & Layout Bridges"
    '    Private Function GetActiveStatusBar() As Cls_StatusBar
    '        Dim childForm As Form = Me.FindForm()
    '        If childForm?.MdiParent IsNot Nothing Then
    '            Dim mainRoot As IMdiParentBridge = TryCast(childForm.MdiParent, IMdiParentBridge)
    '            If mainRoot IsNot Nothing Then Return mainRoot.WorkspaceLayout.MyStatusBar
    '        End If
    '        Return Nothing
    '    End Function
    '    Private Sub UpdateDiagnosticsHUD(ByVal screenMousePos As Point, ByVal worldMousePos As PointF)
    '        Dim childForm As Form = Me.FindForm() : If childForm?.MdiParent Is Nothing Then Exit Sub
    '        Dim mainRoot As IMdiParentBridge = TryCast(childForm.MdiParent, IMdiParentBridge) : If mainRoot Is Nothing Then Exit Sub
    '        Dim txt As String = "Selection: None"
    '        If Me._selectionMgr.Count = 1 Then
    '            txt = $"Selected: {Me.SelectionManager.SelectedShapes(0).GetType().Name.Replace("Cls_Shape", "")}"
    '            txt &= If(Me._currentState = ViewportState.Rotating, $" (Rotating {Me._rotationMgr.CurrentAngle:F0}°)", If(Me._currentState = ViewportState.Resizing, " (Transforming)", ""))
    '        ElseIf Me._selectionMgr.Count > 1 Then
    '            txt = $"Selected: {Me._selectionMgr.Count} Shapes" : txt &= If(Me._currentState = ViewportState.GroupDragging, " (Moving Group)", "")
    '        End If
    '        Dim totalComponents As Integer = Me._canvasData.SchematicComponents.Count
    '        mainRoot.WorkspaceLayout.MyStatusBar.UpdateTelemetryHUD(totalComponents, txt, Me._zoomFactor, screenMousePos.X, screenMousePos.Y, worldMousePos.X, worldMousePos.Y)
    '    End Sub
    '    Private Sub ExecuteDiagnosticTelemetryLogging()
    '        Dim focus As Cls_Base_Shape = Me.TryGetActiveFocusShape()
    '        Diagnostics.Debug.WriteLine("=== REAL-TIME CAD RENDER PIPELINE AUDIT ===")
    '        Diagnostics.Debug.WriteLine($"Camera Metrics: Scale={Me._zoomFactor:F2} | Panning=({Me._panOffset.X:F1},{Me._panOffset.Y:F1})")
    '        Diagnostics.Debug.WriteLine($"Active State Machine Frame Node: {Me._currentState.ToString()}")
    '        If Me._currentState = ViewportState.MarqueeSelecting Then
    '            Dim mRect As RectangleF = Me._selectionMgr.GetMarqueeRectangle()
    '            Diagnostics.Debug.WriteLine($" -> Marquee Bounds: X={mRect.X:F1}, Y={mRect.Y:F1}, W={mRect.Width:F1}, H={mRect.Height:F1}")
    '        End If
    '    End Sub
    '    Private Sub DrawMarqueeOverlayWindow(ByVal g As Graphics)
    '        Using marqueePen As New Pen(Color.RoyalBlue, 1.0F / Me._zoomFactor) With {.DashStyle = DashStyle.Dash}
    '            Using marqueeBrush As New SolidBrush(Color.FromArgb(15, Color.RoyalBlue))
    '                Dim rect As RectangleF = Me._selectionMgr.GetMarqueeRectangle()
    '                g.FillRectangle(marqueeBrush, rect.X, rect.Y, rect.Width, rect.Height)
    '                g.DrawRectangle(marqueePen, rect.X, rect.Y, rect.Width, rect.Height)
    '            End Using
    '        End Using
    '    End Sub

    '    Private Sub ExecuteHandlePlacementDebugTrace(ByVal activeTransform As Matrix, ByVal shape As Cls_Base_Shape)
    '        Diagnostics.Debug.WriteLine("=== REAL-TIME GIZMO HANDLE MATRIX AUDIT ===")
    '        Diagnostics.Debug.WriteLine($"Active Target Primitive Model: {shape.GetType().Name} | Angle={shape.RotationAngle:F1}°")
    '        If activeTransform IsNot Nothing Then
    '            Dim elements As Single() = activeTransform.Elements
    '            Diagnostics.Debug.WriteLine($"Render Matrix Vectors: M11={elements(0):F3}, M22={elements(3):F3} | DX={elements(4):F1}, DY={elements(5):F1}")
    '        End If
    '        Diagnostics.Debug.WriteLine("-------------------------------------------")
    '    End Sub


    '    ''' <summary>
    '    ''' Accepts a selection focus request from an external UI panel, updates selectors, and repaints.
    '    ''' </summary>
    '    Friend Sub FocusShapeFromExternalSource(ByVal targetShape As Cls_Base_Shape)
    '        If targetShape Is Nothing Then Exit Sub
    '        Me._selectionMgr.Clear()
    '        Me._selectionMgr.SelectShape(targetShape, False)
    '        Me.ForceDiagnosticsUpdate()
    '    End Sub



    '#End Region


    '    ''' <summary>
    '    ''' Initializes workspace canvas multi-selection tracking bounds.
    '    ''' Satisfies Option Strict On and resides safely under your 25 operational line rule.
    '    ''' </summary>
    '    Private Sub InitializeMarqueeTracking(ByVal worldPt As PointF, ByVal isShiftDown As Boolean)
    '        Me._currentState = ViewportState.MarqueeSelecting
    '        Me._selectionMgr.IsMarqueeSelecting = True
    '        Me._selectionMgr.MarqueeStartPoint = worldPt
    '        Me._selectionMgr.MarqueeCurrentPoint = worldPt

    '        ' Clear previous selection array lists if Shift is not actively held down
    '        If Not isShiftDown Then
    '            Me._selectionMgr.Clear()
    '        End If
    '    End Sub


    '    ' Target File: Cls_Viewport.vb (FocusAndZoomIntoComponent Clean Pass)
    '    ' Strictly compliant with Option Strict On and under the 25-line limit
    '    Friend Sub FocusAndZoomIntoComponent(ByVal targetShape As Cls_Base_Shape)
    '        If targetShape Is Nothing Then Exit Sub
    '        Me._selectionMgr.Clear()
    '        Me._selectionMgr.SelectShape(targetShape, False)

    '        ' FIXED: Read coordinates straight out of the modern component Bounds envelope
    '        Dim minX As Single = targetShape.Bounds.X
    '        Dim minY As Single = targetShape.Bounds.Y
    '        Dim w As Single = Math.Max(1.0F, targetShape.Bounds.Width)
    '        Dim h As Single = Math.Max(1.0F, targetShape.Bounds.Height)

    '        Dim targetScaleX As Single = (Convert.ToSingle(Me.Width) - 100.0F) / w
    '        Dim targetScaleY As Single = (Convert.ToSingle(Me.Height) - 100.0F) / h
    '        Me._zoomFactor = Math.Max(0.2F, Math.Min(5.0F, Math.Min(targetScaleX, targetScaleY)))

    '        Me._panOffset.X = (Convert.ToSingle(Me.Width) - w * Me._zoomFactor) / 2.0F - minX * Me._zoomFactor
    '        Me._panOffset.Y = (Convert.ToSingle(Me.Height) - h * Me._zoomFactor) / 2.0F - minY * Me._zoomFactor

    '        Me.Focus()
    '        Me.ForceDiagnosticsUpdate()
    '    End Sub



    '    ''' <summary>
    '    ''' Intercepts keyboard context modifiers to activate advanced interactive gizmo modes.
    '    ''' Satisfies Option Strict On and resides safely under your 25 operational line rule.
    '    ''' </summary>
    '    Private Sub EvaluateSelectionModifierKeys(ByVal targetShape As Cls_Base_Shape, ByVal worldPt As PointF)
    '        ' Check if the user is holding down the physical ALT key during selection
    '        If (ModifierKeys And Keys.Alt) = Keys.Alt Then
    '            ' Transition the state machine into active rotation mode
    '            Me.InitiateActiveRotation(targetShape)
    '        End If
    '    End Sub

End Class
