' Target File: Cls_Viewport.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Diagnostics
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

Public NotInheritable Class Cls_Viewport
    Inherits UserControl


    ' Inside Cls_Viewport.vb (Cleaned Properties Section)
    Private _activeGateType As LogicGateType = LogicGateType.Buffer


    ' Target File: Cls_Viewport.vb (Refactored Drag-and-Drop Workspace Target)
    ' Strictly type-safe under Option Strict On and resides safely under the 25-line limit.
    Private Sub AddNewComponentToWorkspace(ByVal gateKind As LogicGateType, ByVal targetPos As PointF)
        Dim newGate As Cls_Base_Shape = Nothing

        Select Case gateKind
            Case LogicGateType.AndGate : newGate = New Cls_Gate_And(targetPos)
            Case LogicGateType.NotGate : newGate = New Cls_Gate_Not(targetPos)
        End Select

        If newGate IsNot Nothing Then
            ' FIXED: Route straight to your active schematic components database collection array
            Me._canvasData.Shapes.Add(newGate)
            Me.FlagActiveViewportAsModified(True)
            Me.Invalidate()
        End If
    End Sub

    Private Function CreateShapeInstance(ByVal tool As CanvasTool, ByVal startPt As PointF, ByVal endPt As PointF, ByVal col As Color, ByVal thick As Single) As Cls_Base_Shape
        Select Case tool
            Case CanvasTool.LogicGate
                ' Direct the factory loop to instantiate a complete, structured logic gate component object
                Return Me.InstantiateInteractiveGateSubclass(startPt)
            Case Else
                Return Nothing
        End Select
    End Function

    ''' <summary>
    ''' Segregated helper to instantiate concrete logic gate objects while keeping routines under 25 lines.
    ''' </summary>
    Private Function InstantiateInteractiveGateSubclass(ByVal targetPos As PointF) As Cls_Base_Shape
        Select Case Me._activeGateType
            Case LogicGateType.AndGate
                Return New Cls_Gate_And(targetPos)
            Case LogicGateType.NotGate
                Return New Cls_Gate_Not(targetPos)
            Case Else
                Return Nothing
        End Select
    End Function

    ''' <summary>
    ''' Dynamically resolves the currently active focused logic gate component from the selection list.
    ''' Fully type-safe under Option Strict On and resides safely under the 25-line limit.
    ''' </summary>
    Public Function GetActiveFocusComponent() As Cls_Base_Shape
        ' 1. Check if the selection manager has any actively registered selections left
        If Me._selectionMgr IsNot Nothing AndAlso Me._selectionMgr.Count > 0 Then
            ' 2. Pull the most recently selected shape object out of the tracking collection array
            Dim activeItem As Object = Me._selectionMgr.SelectedShapes(Me._selectionMgr.Count - 1)

            ' 3. Safely evaluate if the item inherits from our master schematic component class
            If TypeOf activeItem Is Cls_Base_Shape Then
                ' 4. Cast explicitly under Option Strict On and return the object reference
                Return DirectCast(activeItem, Cls_Base_Shape)
            End If
        End If

        Return Nothing
    End Function

    Public Property ActiveGateType() As LogicGateType
        Get
            Return Me._activeGateType
        End Get
        Set(ByVal value As LogicGateType)
            Me._activeGateType = value
        End Set
    End Property


    ''' <summary>
    ''' Paints a highly distinguishable, thick Royal Blue outline around selected canvas primitives.
    ''' Satisfies Option Strict On and resides safely underneath the 25-line limit constraint.
    ''' </summary>
    Private Sub RenderSelectionOutline(ByVal g As Graphics, ByVal zoom As Single, ByVal shape As Cls_Base_Shape)
        If g Is Nothing OrElse shape Is Nothing Then Exit Sub

        ' FIXED: Changed color from LimeGreen to RoyalBlue, and increased line weight base to 2.5
        Using selectPen As New Pen(Color.RoyalBlue, 2.5F / zoom) With {.DashStyle = DashStyle.Dash}
            'Dim minX As Single = Math.Min(shape.StartPoint.X, shape.EndPoint.X)
            'Dim minY As Single = Math.Min(shape.StartPoint.Y, shape.EndPoint.Y)
            'Dim w As Single = Math.Abs(shape.StartPoint.X - shape.EndPoint.X)
            'Dim h As Single = Math.Abs(shape.StartPoint.Y - shape.EndPoint.Y)

            ' Call the central coordinate transformer class to build the isolated space
            Using localMatrix As Matrix = Me._transformer.CreateLocalShapeMatrix(Me._panOffset, zoom, shape.RotationAngle, shape.CalculateGeometricCenter())
                g.Transform = localMatrix
                g.DrawRectangle(selectPen, shape.Bounds.X, shape.Bounds.Y, shape.Bounds.Width, shape.Bounds.Height) 'minX, minY, w, h)
            End Using
        End Using
    End Sub


    ''' <summary>
    ''' Evaluates the hit-test target at the raw cursor intersection and initializes states.
    ''' Preserves the raw world point anchor to eliminate group dragging dead zones.
    ''' </summary>
    Private Sub ProcessShapeSelectionToggles(ByVal worldPt As PointF)
        Dim hitShape As Cls_Base_Shape = Me._canvasData.HitTestShapes(worldPt)
        Dim isShiftDown As Boolean = (ModifierKeys And Keys.Shift) = Keys.Shift

        If hitShape IsNot Nothing Then
            ' If the shape is already part of the active selection, initiate a move transaction
            If Me._selectionMgr.Contains(hitShape) AndAlso Not isShiftDown Then
                Me._currentState = ViewportState.GroupDragging
                ' FIXED: Lock the unmutated raw world coordinate as the trajectory baseline anchor
                Me._worldDragStart = worldPt
                Exit Sub
            End If

            Me._selectionMgr.SelectShape(hitShape, isShiftDown)
            If Not isShiftDown Then Me.EvaluateSelectionModifierKeys(hitShape, worldPt)
        Else
            Me.InitializeMarqueeTracking(worldPt, isShiftDown)
        End If
    End Sub

    ''' <summary>
    ''' Initializes workspace canvas multi-selection tracking bounds.
    ''' </summary>
    Private Sub InitializeMarqueeTracking(ByVal worldPt As PointF, ByVal isShiftDown As Boolean)
        Me._currentState = ViewportState.MarqueeSelecting
        Me._selectionMgr.IsMarqueeSelecting = True
        Me._selectionMgr.MarqueeStartPoint = worldPt
        Me._selectionMgr.MarqueeCurrentPoint = worldPt

        If Not isShiftDown Then
            Me._selectionMgr.Clear()
        End If
    End Sub

    ''' <summary>
    ''' Intercepts context modifiers to activate advanced interactive gizmo modes.
    ''' </summary>
    Private Sub EvaluateSelectionModifierKeys(ByVal targetShape As Cls_Base_Shape, ByVal worldPt As PointF)
        If (ModifierKeys And Keys.Alt) = Keys.Alt Then
            Me.InitiateActiveRotation(targetShape)
        End If
    End Sub

    ''' <summary>
    ''' Displaces all selected schematic components relative to grid snapping rules.
    ''' Fully type-safe under Option Strict On and resides safely under the 25-line limit.
    ''' </summary>
    Private Sub ExecuteGroupMoveTranslation(ByVal rawWorldPoint As PointF)
        ' 1. Compute cumulative displacement from baseline anchor and pass through grid snapping
        Dim rawDx As Single = rawWorldPoint.X - Me._worldDragStart.X
        Dim rawDy As Single = rawWorldPoint.Y - Me._worldDragStart.Y
        Dim snappedDelta As PointF = Me._grid.SnapPoint(New PointF(rawDx, rawDy))

        If snappedDelta.X <> 0.0F OrElse snappedDelta.Y <> 0.0F Then
            ' 2. Transform each selected component type-safely via our updated method signature
            For Each item As Object In Me._selectionMgr.SelectedShapes
                If TypeOf item Is Cls_Base_Shape Then
                    Dim gate As Cls_Base_Shape = DirectCast(item, Cls_Base_Shape)
                    gate.ApplyDeltaTransform(snappedDelta.X, snappedDelta.Y)
                End If
            Next

            ' 3. Advance anchor tracking step variables by actual translated distance metrics
            Me._worldDragStart.X += snappedDelta.X
            Me._worldDragStart.Y += snappedDelta.Y
            Me.Invalidate()
        End If
        Me.FlagActiveViewportAsModified(True)
    End Sub


    ''' <summary>
    ''' Selects the target component shape, centers the canvas, scales the zoom factor,
    ''' and forces input focus back to the control to ensure fluid mouse panning operations.
    ''' </summary>
    Friend Sub FocusAndZoomIntoComponent(targetShape As Cls_Base_Shape)
        If targetShape Is Nothing Then Exit Sub
        _selectionMgr.Clear() : _selectionMgr.SelectShape(targetShape, False)

        Dim minX As Single = targetShape.Bounds.X 'Math.Min(targetShape.StartPoint.X, targetShape.EndPoint.X)
        Dim minY As Single = targetShape.Bounds.Y ' Math.Min(targetShape.StartPoint.Y, targetShape.EndPoint.Y)
        Dim w As Single = Math.Max(1.0F, targetShape.Bounds.Width)
        Dim h As Single = Math.Max(1.0F, targetShape.Bounds.Height)

        Dim targetScaleX As Single = (CSng(Me.Width) - 100.0F) / w
        Dim targetScaleY As Single = (CSng(Me.Height) - 100.0F) / h
        _zoomFactor = Math.Max(0.2F, Math.Min(5.0F, Math.Min(targetScaleX, targetScaleY)))

        _panOffset.X = (CSng(Me.Width) - w * _zoomFactor) / 2.0F - minX * _zoomFactor
        _panOffset.Y = (CSng(Me.Height) - h * _zoomFactor) / 2.0F - minY * _zoomFactor

        ' FIXED: Force physical WinForms message tracking focus back onto the canvas!
        ' This ensures mouse wheel panning and drag actions fire straight into the viewport handlers.
        Me.Focus()

        ForceDiagnosticsUpdate()
    End Sub

    Friend Sub ForceDiagnosticsUpdate()
        Dim localScreen As Point = Me.PointToClient(Cursor.Position)
        Dim localWorld As PointF = ConvertScreenToWorld(localScreen)
        UpdateDiagnosticsHUD(localScreen, localWorld)

        Dim childForm As Form = Me.FindForm()
        If childForm?.MdiParent IsNot Nothing Then
            Dim mainRoot As IMdiParentBridge = TryCast(childForm.MdiParent, IMdiParentBridge)
            If mainRoot?.WorkspaceLayout IsNot Nothing Then
                ' 1. Synchronize the lowercase property fields inspector grid
                mainRoot.WorkspaceLayout.MyExplorer.InspectShapesGroup(_selectionMgr.SelectedShapes)

                ' 2. Synchronize the uppercase hierarchy TreeView explorer browser grid
                mainRoot.MdiAction.SynchronizeExplorerHierarchyTree()
            End If
        End If
        Me.Invalidate()
    End Sub


    ''' <summary>
    ''' Accepts a selection focus request from an external UI panel, updates selectors, and repaints.
    ''' </summary>
    Friend Sub FocusShapeFromExternalSource(targetShape As Cls_Base_Shape)
        If targetShape Is Nothing Then Exit Sub

        ' 1. Clear any multi-selection group arrays to lock focus on the single clicked item
        _selectionMgr.Clear()

        ' 2. Register the shape into the viewport tracking selection manager
        _selectionMgr.SelectShape(targetShape, False)

        ' 3. Trigger a complete pipeline audit, property grid synchronization, and visual frame redraw
        ForceDiagnosticsUpdate()
    End Sub

    Public Function ConvertScreenToWorld(screenPoint As Point) As PointF
        Return _transformer.TransformScreenToWorld(screenPoint, _panOffset, _zoomFactor)
    End Function

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        Dim g As Graphics = e.Graphics

        Using cameraMatrix As Matrix = _transformer.CreateCameraMatrix(_panOffset, _zoomFactor)
            g.Transform = cameraMatrix

            ' Fetch optimized culling bounds straight from the central coordinator authority
            Dim visibleBounds As RectangleF = _transformer.CalculateWorldBounds(_panOffset, _zoomFactor, Me.Width, Me.Height)
            'Dim gridmgr As Cls_Grid_Manager = TryGetGlobalGridManager()
            '_snapEngine.DrawBackgroundGrid(g, _zoomFactor, visibleBounds, gridmgr)
            Me._grid.Render(g, _zoomFactor, visibleBounds)

            ' Render shapes via a protected pipeline to stop matrix contamination
            RenderProtectedWorldCanvas(g, cameraMatrix)

            RenderSelectionOverlays(g)
            If _currentState = ViewportState.MarqueeSelecting Then DrawMarqueeOverlayWindow(g)
            If _currentState = ViewportState.Drawing Then DrawPreviewGeometry(g)
            ExecuteDiagnosticTelemetryLogging()
        End Using
        g.ResetTransform()
    End Sub

    ' Segregated loop passing elements through the transformer's protection wrapper safely
    Private Sub RenderProtectedWorldCanvas(g As Graphics, cameraMatrix As Matrix)
        For Each shape As Cls_Base_Shape In _canvasData.Shapes
            _transformer.ExecuteSafeShapeRender(g, cameraMatrix, shape)
        Next
    End Sub

    ' Target File: Cls_Viewport.vb -> inside RenderSelectionOverlays method:
    Private Sub RenderSelectionOverlays(g As Graphics)
        Dim activeShape As Cls_Base_Shape = TryGetActiveFocusShape()

        For Each shape As Cls_Base_Shape In _selectionMgr.SelectedShapes
            RenderSelectionOutline(g, _zoomFactor, shape)
        Next

        If activeShape IsNot Nothing Then
            Dim isCtrlDown As Boolean = (ModifierKeys And Keys.Control) = Keys.Control

            ' FIXED: Pull a clean local shape matrix context directly from the central manager
            Using localMatrix As Matrix = _transformer.CreateLocalShapeMatrix(_panOffset, _zoomFactor, activeShape.RotationAngle, activeShape.CalculateGeometricCenter())
                g.Transform = localMatrix
                ExecuteHandlePlacementDebugTrace(localMatrix, activeShape)

                If _currentState = ViewportState.Resizing OrElse isCtrlDown Then
                    _resizeMgr.DrawResizeHandles(g, _zoomFactor, activeShape)
                End If
                If _currentState = ViewportState.Rotating Then
                    _rotationMgr.DrawRotationOverlayMetrics(g, _zoomFactor, activeShape)
                End If
            End Using
        End If
    End Sub


    ''' <summary>
    ''' Emits real-time coordinate transformation parameters to the Output Window for handle placement debugging.
    ''' </summary>
    Private Sub ExecuteHandlePlacementDebugTrace(activeTransform As Matrix, shape As Cls_Base_Shape)
        Diagnostics.Debug.WriteLine("=== REAL-TIME GIZMO HANDLE MATRIX AUDIT ===")
        Diagnostics.Debug.WriteLine($"Active Target Primitive Model: {shape.GetType().Name}")
        Diagnostics.Debug.WriteLine($"Local Shape Attributes: Angle={shape.RotationAngle:F1}° | Center=({shape.CalculateGeometricCenter().X:F1},{shape.CalculateGeometricCenter().Y:F1})")
        Diagnostics.Debug.WriteLine($"Global Viewport Attributes: Camera Zoom={_zoomFactor:F2}x | Active State={_currentState.ToString()}")

        ' Safely parse the active transformation vector array slots
        If activeTransform IsNot Nothing Then
            Dim elements As Single() = activeTransform.Elements
            Diagnostics.Debug.WriteLine($"Render Matrix Vectors: M11={elements(0):F3}, M12={elements(1):F3}, M21={elements(2):F3}, M22={elements(3):F3}")
            Diagnostics.Debug.WriteLine($"Render Translation Deltas: DX={elements(4):F1}, DY={elements(5):F1}")
        End If
        Diagnostics.Debug.WriteLine("-------------------------------------------")
    End Sub


    Private Sub FlagActiveViewportAsModified(isModified As Boolean)
        Dim childForm As Form = Me.FindForm()
        If childForm IsNot Nothing AndAlso childForm.MdiParent IsNot Nothing Then
            ' 1. Walk straight up to the central MDI framework bridge interface
            Dim mainRoot As IMdiParentBridge = TryCast(childForm.MdiParent, IMdiParentBridge)

            ' 2. Securely update the centralized document status tracking maps inline
            If mainRoot IsNot Nothing AndAlso childForm.Tag IsNot Nothing Then
                Dim documentId As Integer = CInt(childForm.Tag)
                mainRoot.MdiAction.ProjectManager.FlagDocumentAsDirty(documentId, isModified)
            End If
        End If
    End Sub


    Private Sub CommitDrawingShape()
        Dim thick As Single = 2.0! / _zoomFactor
        Dim newShape As Cls_Base_Shape = CreateShapeInstance(_currentTool, _worldStartPoint, _worldCurrentPoint, Color.FromArgb(30, 30, 30), thick)
        If newShape IsNot Nothing Then
            _canvasData.Shapes.Add(newShape)

            ' FIXED: Notify the global database registry that this drawing model has modified data
            FlagActiveViewportAsModified(True)
        End If
        Me.Invalidate()
    End Sub


    Private ReadOnly _canvasData As New Cls_Drawing_Canvas(Me)
    'Private ReadOnly _snapEngine As New Cls_Grid_Snap_Engine()
    Private ReadOnly _rotationMgr As New Cls_Rotation_Manager()
    Private ReadOnly _resizeMgr As New Cls_Resize_Manager()
    Private ReadOnly _selectionMgr As New Cls_Selection_Manager()
    Private ReadOnly _keyboardMgr As Cls_Keyboard_Manager
    Private ReadOnly _transformer As New Cls_Coordinate_Transformer()

    Private _currentTool As CanvasTool = CanvasTool.LogicGate
    Private _zoomFactor As Single = 1.0F
    Private _panOffset As New PointF(0.0F, 0.0F)
    Private _currentState As ViewportState = ViewportState.Idle


    'Private _gridConfig As New Cls_Grid_Settings(False, 20)

    Private _startMousePos As Point
    Private _worldStartPoint As PointF
    Private _worldCurrentPoint As PointF
    Private _worldDragStart As PointF
    Private ReadOnly _grid As New Cls_Viewport_Grid()


#Region "Properties"

    Public ReadOnly Property Grid() As Cls_Viewport_Grid
        Get
            Return Me._grid
        End Get
    End Property

    Public Property CurrentState As ViewportState
        Get
            Return _currentState
        End Get
        Set(value As ViewportState)
            _currentState = value
        End Set
    End Property
    Public Property ZoomFactor As Single
        Get
            Return _zoomFactor
        End Get
        Set(value As Single)
            _zoomFactor = Math.Max(0.1F, Math.Min(10.0F, value))
        End Set
    End Property



    Public ReadOnly Property CanvasData As Cls_Drawing_Canvas
        Get
            Return _canvasData
        End Get
    End Property
    Public Property ActiveTool As CanvasTool
        Get
            Return _currentTool
        End Get
        Set(value As CanvasTool)
            _currentTool = value
        End Set
    End Property

    Public ReadOnly Property RotationManager As Cls_Rotation_Manager
        Get
            Return _rotationMgr
        End Get
    End Property


    Public ReadOnly Property SelectionManager As Cls_Selection_Manager
        Get
            Return _selectionMgr
        End Get
    End Property

#End Region

    Public Sub New()
        Me.Dock = DockStyle.Fill
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint Or
                    ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or
                    ControlStyles.Selectable, True)
        Me.BackColor = Color.White

        ' Add these setup options to Public Sub New() inside Cls_Viewport.vb
        Me.AllowDrop = True
        AddHandler Me.DragEnter, AddressOf Me.OnViewportDragEnter
        AddHandler Me.DragDrop, AddressOf Me.OnViewportDragDrop

        _keyboardMgr = New Cls_Keyboard_Manager(Me)
    End Sub

    Public Sub ZoomAtMouse(screenMousePos As Point, scaleStep As Single)
        Dim targetZoom As Single = Math.Max(0.1F, Math.Min(10.0F, _zoomFactor * scaleStep))
        If targetZoom = _zoomFactor Then Exit Sub

        Dim deltaX As Single = (CSng(screenMousePos.X) - _panOffset.X) * (1.0F - scaleStep)
        Dim deltaY As Single = (CSng(screenMousePos.Y) - _panOffset.Y) * (1.0F - scaleStep)

        _panOffset.X += deltaX : _panOffset.Y += deltaY
        _zoomFactor = targetZoom : Me.Invalidate()
    End Sub

    Public Sub ZoomInAtCenter()
        ZoomAtMouse(New Point(Me.Width \ 2, Me.Height \ 2), 1.2F)
    End Sub

    Public Sub ZoomOutAtCenter()
        ZoomAtMouse(New Point(Me.Width \ 2, Me.Height \ 2), 0.8F)
    End Sub

    ''' <summary>
    ''' Computes the absolute bounding frame of all shapes and adjusts zoom scales and panning deltas 
    ''' to center and fit the drawing perfectly within the current form window view.
    ''' </summary>
    Public Sub ZoomFit()
        If _canvasData.Shapes.Count = 0 Then
            _zoomFactor = 1.0F : _panOffset = New PointF(0.0F, 0.0F) : Me.Invalidate() : Exit Sub
        End If

        ' 1. Calculate the absolute world space bounding box of all shapes combined
        Dim extents As RectangleF = CalculateShapeExtents()

        ' 2. Calculate separate horizontal and vertical scale steps factoring in a safe 40-pixel padding margin
        Dim targetScaleX As Single = (CSng(Me.Width) - 40.0F) / extents.Width
        Dim targetScaleY As Single = (CSng(Me.Height) - 40.0F) / extents.Height

        ' 3. Apply the smaller scale factor as the new clamped global ZoomFactor
        _zoomFactor = Math.Max(0.1F, Math.Min(10.0F, Math.Min(targetScaleX, targetScaleY)))

        ' 4. FIXED: Shift the camera pan translation variables to align the shape bounding box exactly with your form center pixels
        _panOffset.X = (CSng(Me.Width) - extents.Width * _zoomFactor) / 2.0F - extents.X * _zoomFactor
        _panOffset.Y = (CSng(Me.Height) - extents.Height * _zoomFactor) / 2.0F - extents.Y * _zoomFactor

        Me.Invalidate()
    End Sub

    Private Function CalculateShapeExtents() As RectangleF
        Dim minX As Single = Single.MaxValue, minY As Single = Single.MaxValue
        Dim maxX As Single = Single.MinValue, maxY As Single = Single.MinValue
        For Each shape As Cls_Base_Shape In _canvasData.Shapes
            minX = Math.Min(minX, shape.Bounds.X)
            minY = Math.Min(minY, shape.Bounds.Y)
            maxX = Math.Max(maxX, shape.Bounds.Right)
            maxY = Math.Max(maxY, shape.Bounds.Bottom)
        Next
        Return New RectangleF(minX, minY, maxX - minX, maxY - minY)
    End Function

    Protected Overrides Sub OnMouseWheel(e As MouseEventArgs)
        MyBase.OnMouseWheel(e)
        If Not Me.Focused OrElse _currentState <> ViewportState.Idle Then Exit Sub
        ZoomAtMouse(e.Location, If(e.Delta > 0, 1.1F, 0.9F))
    End Sub

    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        Me.Focus()
        If _currentState <> ViewportState.Idle Then Exit Sub
        If e.Button = MouseButtons.Middle Then
            _currentState = ViewportState.Panning : _startMousePos = e.Location : Me.Cursor = Cursors.NoMove2D
        ElseIf e.Button = MouseButtons.Left Then
            Me.Capture = True : ProcessLeftMouseDown(e)
        End If
    End Sub

    Private Sub ProcessLeftMouseDown(e As MouseEventArgs)
        Dim rawWorldPoint As PointF = ConvertScreenToWorld(e.Location)
        If (ModifierKeys And Keys.Alt) = Keys.Alt AndAlso _currentTool <> CanvasTool.SelectPointer Then
            _currentState = ViewportState.Panning : _startMousePos = e.Location : Me.Cursor = Cursors.NoMove2D : Exit Sub
        End If
        If _currentTool = CanvasTool.SelectPointer Then
            ProcessPointerSelection(rawWorldPoint)
        Else
            _currentState = ViewportState.Drawing

            ' Inside Cls_Viewport.vb Mouse Interaction Loops:
            'Dim gridMgr As Cls_Grid_Manager = TryGetGlobalGridManager()

            ' Hand the global strategy bucket to your local worker to calculate coordinates
            ' _worldCurrentPoint = _snapEngine.Snap(rawWorldPoint, gridMgr)

            ' _worldStartPoint = _snapEngine.Snap(rawWorldPoint, gridMgr)
            _worldStartPoint = Me._grid.SnapPoint(rawWorldPoint)
            _worldCurrentPoint = _worldStartPoint

        End If
    End Sub

    Private Sub ProcessPointerSelection(worldPt As PointF)
        Dim activeFocus As Cls_Base_Shape = TryGetActiveFocusShape()
        Dim isCtrlDown As Boolean = (ModifierKeys And Keys.Control) = Keys.Control
        If (isCtrlDown OrElse _currentState = ViewportState.Resizing) AndAlso activeFocus IsNot Nothing Then
            Dim localPt As PointF = ConvertWorldToLocalShapeSpace(worldPt, activeFocus)
            Dim handle As ResizeHandle = _resizeMgr.HitTestHandles(localPt, activeFocus, Math.Max(0.5F, _zoomFactor))
            If handle <> ResizeHandle.None Then
                _currentState = ViewportState.Resizing : _resizeMgr.StartResize(handle, worldPt) : Exit Sub
            End If
        End If
        ProcessShapeSelectionToggles(worldPt)
    End Sub

    Friend Sub InitiateActiveRotation(targetShape As Cls_Base_Shape)
        If targetShape IsNot Nothing Then
            _currentState = ViewportState.Rotating : _rotationMgr.StartRotation(targetShape)
            Me.Cursor = Cursors.SizeAll : Me.Invalidate()
        End If
    End Sub
    ' Target File: Cls_Viewport.vb
    ' Refactored Mouse Move Router meeting your strict 25-line operational limit
    Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
        MyBase.OnMouseMove(e)
        Dim rawWorldPoint As PointF = ConvertScreenToWorld(e.Location)

        Select Case _currentState
            Case ViewportState.Panning
                ExecutePanningDrag(e)
            Case ViewportState.MarqueeSelecting
                _selectionMgr.MarqueeCurrentPoint = rawWorldPoint : Me.Invalidate()
            Case ViewportState.Resizing
                _resizeMgr.ProcessResizeDrag(rawWorldPoint, TryGetActiveFocusShape()) : Me.Invalidate()
            Case ViewportState.Rotating
                ' FIXED: Convert the world mouse point into local shape space to calculate angles accurately
                Dim activeFocus As Cls_Base_Shape = TryGetActiveFocusShape()
                If activeFocus IsNot Nothing Then
                    _rotationMgr.ProcessRotation(rawWorldPoint, activeFocus, GetActiveStatusBar())
                    Me.Invalidate()
                End If
            Case ViewportState.Drawing
                'Dim gridMgr As Cls_Grid_Manager = TryGetGlobalGridManager()

                '_worldCurrentPoint = _snapEngine.Snap(rawWorldPoint, gridMgr)
                _worldCurrentPoint = Me._grid.SnapPoint(rawWorldPoint)
                Me.Invalidate()
            Case ViewportState.GroupDragging
                ExecuteGroupMoveTranslation(rawWorldPoint)
            Case Else
                ProcessCursorHoverStates(rawWorldPoint)
        End Select
        UpdateDiagnosticsHUD(e.Location, rawWorldPoint)
    End Sub

    Sub ExecutePanningDrag(e As MouseEventArgs)
        Dim screenDeltaX As Single = CSng(e.X - _startMousePos.X)
        Dim screenDeltaY As Single = CSng(e.Y - _startMousePos.Y)
        _panOffset.X += (screenDeltaX / _zoomFactor)
        _panOffset.Y += (screenDeltaY / _zoomFactor)
        _startMousePos = e.Location : Me.Invalidate()
    End Sub

    Private Sub ProcessCursorHoverStates(rawWorldPoint As PointF)
        Dim activeFocus As Cls_Base_Shape = TryGetActiveFocusShape()
        Dim isCtrlDown As Boolean = (ModifierKeys And Keys.Control) = Keys.Control
        If _currentTool = CanvasTool.SelectPointer AndAlso activeFocus IsNot Nothing AndAlso isCtrlDown Then
            Dim localPt As PointF = ConvertWorldToLocalShapeSpace(rawWorldPoint, activeFocus)
            Dim hoverHandle As ResizeHandle = _resizeMgr.HitTestHandles(localPt, activeFocus, Math.Max(0.5F, _zoomFactor))
            _resizeMgr.UpdateViewportCursor(hoverHandle, Me)
        ElseIf Me.Cursor <> Cursors.Default AndAlso _currentState = ViewportState.Idle Then
            Me.Cursor = Cursors.Default
        End If
    End Sub
    Private Function GetActiveStatusBar() As Cls_StatusBar
        Dim childForm As Form = Me.FindForm()
        If childForm?.MdiParent IsNot Nothing Then
            Dim mainRoot As IMdiParentBridge = TryCast(childForm.MdiParent, IMdiParentBridge)
            If mainRoot IsNot Nothing Then Return mainRoot.WorkspaceLayout.MyStatusBar
        End If
        Return Nothing
    End Function
    Private Sub UpdateDiagnosticsHUD(screenMousePos As Point, worldMousePos As PointF)
        Dim childForm As Form = Me.FindForm() : If childForm?.MdiParent Is Nothing Then Exit Sub
        Dim mainRoot As IMdiParentBridge = TryCast(childForm.MdiParent, IMdiParentBridge) : If mainRoot Is Nothing Then Exit Sub
        Dim txt As String = "Selection: None"
        If _selectionMgr.Count = 1 Then
            txt = $"Selected: {SelectionManager.SelectedShapes(0).GetType().Name.Replace("Cls_Shape", "")}"
            If _currentState = ViewportState.Rotating Then txt &= $" (Rotating {_rotationMgr.CurrentAngle:F0}°)" Else If _currentState = ViewportState.Resizing Then txt &= " (Transforming)"
        ElseIf _selectionMgr.Count > 1 Then
            txt = $"Selected: {_selectionMgr.Count} Shapes" : If _currentState = ViewportState.GroupDragging Then txt &= " (Moving Group)"
        End If
        mainRoot.WorkspaceLayout.MyStatusBar.UpdateTelemetryHUD(_canvasData.Shapes.Count, txt, _zoomFactor, screenMousePos.X, screenMousePos.Y, worldMousePos.X, worldMousePos.Y)
    End Sub
    Protected Overrides Sub OnMouseUp(e As MouseEventArgs)
        MyBase.OnMouseUp(e)
        If e.Button = MouseButtons.Left Then Me.Capture = False
        Select Case _currentState
            Case ViewportState.Panning, ViewportState.Resizing, ViewportState.GroupDragging
                _currentState = ViewportState.Idle : Me.Cursor = Cursors.Default : If _currentState = ViewportState.Resizing Then _resizeMgr.TerminateResize()
            'Case ViewportState.Rotating
                'If Not (ModifierKeys And Keys.Alt) = Keys.Alt Then
                '    _currentState = ViewportState.Idle : _rotationMgr.TerminateRotation() : Me.Cursor = Cursors.Default
                'End If
                '' Target File: Cls_Viewport.vb -> inside OnMouseUp Select Case block:
            Case ViewportState.Rotating
                ' FIXED: Terminate the active drag transaction immediately upon left-click mouse release
                _rotationMgr.TerminateRotation()

                ' Check if the user is still holding the physical ALT key down to determine cursor HUD visibility
                If (ModifierKeys And Keys.Alt) = Keys.Alt Then
                    _currentState = ViewportState.Idle
                    Me.Cursor = Cursors.SizeAll
                Else
                    _currentState = ViewportState.Idle
                    Me.Cursor = Cursors.Default
                End If

            Case ViewportState.MarqueeSelecting
                _currentState = ViewportState.Idle : _selectionMgr.IsMarqueeSelecting = False
                _selectionMgr.FinalizeMarqueeSelection(_canvasData.Shapes, (ModifierKeys And Keys.Shift) = Keys.Shift)
            Case ViewportState.Drawing
                _currentState = ViewportState.Idle : CommitDrawingShape()
        End Select
        ForceDiagnosticsUpdate()
    End Sub
    Private Sub ProcessLeftMouseUp(e As MouseEventArgs)
        ' Placeholder channel for structural routing hooks
        FlagActiveViewportAsModified(True)
    End Sub
    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
        MyBase.OnKeyDown(e)
        _keyboardMgr.ProcessKeyDownTransaction(e)
    End Sub
    Protected Overrides Sub OnKeyUp(e As KeyEventArgs)
        MyBase.OnKeyUp(e)
        _keyboardMgr.ProcessKeyUpTransaction(e)
    End Sub
    Friend Sub UpdateViewportState(newState As ViewportState)
        _currentState = newState
    End Sub
    Friend Function GetActiveFocusShape() As Cls_Base_Shape
        Return TryGetActiveFocusShape()
    End Function

    Private Sub ExecuteDiagnosticTelemetryLogging()
        Dim focus As Cls_Base_Shape = TryGetActiveFocusShape()
        Diagnostics.Debug.WriteLine("=== REAL-TIME CAD RENDER PIPELINE AUDIT ===")
        Diagnostics.Debug.WriteLine($"Camera Metrics: Scale={_zoomFactor:F2} | Panning=({_panOffset.X:F1},{_panOffset.Y:F1})")
        Diagnostics.Debug.WriteLine($"Active State Machine Frame Node: {_currentState.ToString()}")
        If _currentState = ViewportState.MarqueeSelecting Then
            Dim mRect As RectangleF = _selectionMgr.GetMarqueeRectangle()
            Diagnostics.Debug.WriteLine($" -> Marquee Bounds: X={mRect.X:F1}, Y={mRect.Y:F1}, W={mRect.Width:F1}, H={mRect.Height:F1}")
        End If
    End Sub

    Private Sub DrawMarqueeOverlayWindow(g As Graphics)
        Using marqueePen As New Pen(Color.RoyalBlue, 1.0! / _zoomFactor) With {.DashStyle = DashStyle.Dash}
            Using marqueeBrush As New SolidBrush(Color.FromArgb(15, Color.RoyalBlue))
                Dim rect As RectangleF = _selectionMgr.GetMarqueeRectangle()
                g.FillRectangle(marqueeBrush, rect.X, rect.Y, rect.Width, rect.Height)
                g.DrawRectangle(marqueePen, rect.X, rect.Y, rect.Width, rect.Height)
            End Using
        End Using
    End Sub
    Private Sub RenderRotationOverlayMetrics(g As Graphics, zoom As Single)
        Dim config As Cls_Configuration_Manager = Cls_Configuration_Manager.Instance
        Dim center As PointF = _rotationMgr.PivotCenter
        Using pivotPen As New Pen(Color.OrangeRed, 1.5! / zoom)
            Dim r As Single = 8.0F / zoom
            g.DrawEllipse(pivotPen, center.X - r, center.Y - r, r * 2.0F, r * 2.0F)
            g.DrawLine(pivotPen, center.X - r - 4.0F / zoom, center.Y, center.X + r + 4.0F / zoom, center.Y)
            g.DrawLine(pivotPen, center.X, center.Y - r - 4.0F / zoom, center.X, center.Y + r + 4.0F / zoom)
        End Using
        Using textBrush As New SolidBrush(Color.OrangeRed)
            g.DrawString($"{_rotationMgr.CurrentAngle:F1}°", config.UiFont, textBrush, New PointF(center.X + (15.0F / zoom), center.Y - (25.0F / zoom)))
        End Using
    End Sub

    Private Function ConvertWorldToLocalShapeSpace(worldPoint As PointF, shape As Cls_Base_Shape) As PointF
        If shape Is Nothing OrElse shape.RotationAngle = 0.0F Then Return worldPoint
        Using rotationMatrix As New Matrix()
            rotationMatrix.RotateAt(-shape.RotationAngle, shape.CalculateGeometricCenter())
            Dim points As PointF() = {worldPoint}
            rotationMatrix.TransformPoints(points)
            Return points(0)
        End Using
    End Function
    Private Function TryGetActiveFocusShape() As Cls_Base_Shape
        If _selectionMgr IsNot Nothing AndAlso _selectionMgr.Count > 0 Then
            Return _selectionMgr.SelectedShapes(_selectionMgr.Count - 1)
        End If
        Return Nothing
    End Function
    Private Function CalculateVisibleWorldBounds(activeMatrix As Matrix) As RectangleF
        Using inverseMatrix As Matrix = activeMatrix.Clone()
            If inverseMatrix.IsInvertible Then
                inverseMatrix.Invert()
                Dim points As PointF() = {New PointF(0, 0), New PointF(Math.Max(1, Me.Width), Math.Max(1, Me.Height))}
                inverseMatrix.TransformPoints(points)
                Return New RectangleF(points(0).X, points(0).Y, points(1).X - points(0).X, points(1).Y - points(0).Y)
            End If
            Return New RectangleF(0, 0, Math.Max(1, Me.Width), Math.Max(1, Me.Height))
        End Using
    End Function
    Protected Overrides Function IsInputKey(keyData As Keys) As Boolean
        Dim pureKey As Keys = keyData And Keys.KeyCode
        If (keyData And Keys.Modifiers) <> 0 OrElse pureKey = Keys.Delete Then Return True
        Return MyBase.IsInputKey(keyData)
    End Function


    ''' <summary>
    ''' Draws a real-time component preview outline during schematic creation clicks.
    ''' Fully type-safe under Option Strict On and resides safely under the 25-line limit.
    ''' </summary>
    Private Sub DrawPreviewGeometry(ByVal g As Graphics)
        If g Is Nothing Then Exit Sub
        g.SmoothingMode = SmoothingMode.AntiAlias

        Using previewPen As New Pen(Color.Red, 1.5! / Me._zoomFactor)
            Dim minX As Single = Math.Min(Me._worldStartPoint.X, Me._worldCurrentPoint.X)
            Dim minY As Single = Math.Min(Me._worldStartPoint.Y, Me._worldCurrentPoint.Y)
            Dim w As Single = Math.Abs(Me._worldStartPoint.X - Me._worldCurrentPoint.X)
            Dim h As Single = Math.Abs(Me._worldStartPoint.Y - Me._worldCurrentPoint.Y)

            Dim bounds As New RectangleF(minX, minY, w, h)
            Me.RenderActiveToolPreview(g, previewPen, bounds)
        End Using
    End Sub

    ''' <summary>
    ''' Segregated dispatcher routine maintaining strict 25-line structural code limits.
    ''' </summary>
    Private Sub RenderActiveToolPreview(ByVal g As Graphics, ByVal previewPen As Pen, ByVal bounds As RectangleF)
        Select Case Me._currentTool
            Case CanvasTool.LogicGate
                ' Render a dashed bounding envelope with a text string identifier for the gate type
                previewPen.DashStyle = DashStyle.Dash
                g.DrawRectangle(previewPen, bounds.X, bounds.Y, bounds.Width, bounds.Height)

                Using textBrush As New SolidBrush(Color.Red)
                    Dim label As String = Me._activeGateType.ToString().ToUpper()
                    g.DrawString(label, Me.Font, textBrush, bounds.X + 4.0F, bounds.Y + 4.0F)
                End Using
        End Select
    End Sub




#Region "Drag And Drop"

    Private Sub OnViewportDragEnter(ByVal sender As Object, ByVal e As DragEventArgs)
        ' Verify if the item currently hovering over the canvas contains a valid logic gate enum token
        If e.Data.GetDataPresent(GetType(LogicGateType)) Then
            e.Effect = DragDropEffects.Copy
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub


    Private Sub OnViewportDragDrop(ByVal sender As Object, ByVal e As DragEventArgs)
        If e.Data.GetDataPresent(GetType(LogicGateType)) Then
            ' 1. Pull the exact logic gate type enumeration token out of the data layer
            Dim droppedGateKind As LogicGateType = DirectCast(e.Data.GetData(GetType(LogicGateType)), LogicGateType)

            ' 2. FIXED: Convert hardware Screen absolute pixels into client relative canvas space
            Dim screenPoint As New Point(e.X, e.Y)
            Dim clientMousePos As Point = Me.PointToClient(screenPoint)

            ' 3. Project the client mouse coordinates into your camera world space
            Dim rawWorldDropPoint As PointF = Me.ConvertScreenToWorld(clientMousePos)
            Dim snappedWorldPosition As PointF = Me._grid.SnapPoint(rawWorldDropPoint)

            ' 4. Instantiate and commit the logic gate straight to your database collection
            Me.AddNewComponentToWorkspace(droppedGateKind, snappedWorldPosition)
        End If
    End Sub



    'Private Sub AddNewComponentToWorkspace(ByVal gateKind As LogicGateType, ByVal targetPos As PointF)
    '    Dim newGate As Cls_Base_Shape = Nothing

    '    ' Factory router targeting explicit structural variants
    '    Select Case gateKind
    '        Case LogicGateType.AndGate : newGate = New Cls_Gate_And(targetPos)
    '        Case LogicGateType.NotGate : newGate = New Cls_Gate_Not(targetPos)
    '    End Select

    '    If newGate IsNot Nothing Then
    '        ' Commit straight to the drawing storage database model and repaint
    '        Me._canvasData.Shapes.Add(newGate)
    '        Me.FlagActiveViewportAsModified(True)
    '        Me.Invalidate()
    '    End If
    'End Sub


#End Region
End Class





