


' Target File: Cls_Viewport.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

Public NotInheritable Class Cls_Viewport
    Inherits UserControl


#Region "Inspecter"

    ' Target File: Cls_Viewport.vb
    ' Project Namespace: mdi_test
    ' Architecture Standard: VB.NET | Option Strict On | Explicit Binding
    ' Said Rules Compliance: Zero Guessing | No Non-Existent Token Variables | Locked Classes Only
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    Public Sub CenterCameraAroundShapeWorldBounds(ByVal targetShape As Cls_Base_Shape)
        If targetShape Is Nothing Then Return

        ' 1. GEOMETRIC WORLD CENTER EXTRACTION: Pull the raw unrotated world coordinates of the shape center
        Dim worldCenter As PointF = targetShape.CalculateGeometricCenter()

        ' 2. SCREEN VIEW CENTER DECOMPOSITION: Extract half of the current visible screen control dimensions
        Dim screenCenterX As Single = Me.Width / 2.0F
        Dim screenCenterY As Single = Me.Height / 2.0F

        ' 3. RE-MATRIX PAN TRANSMISSION: Factor the current zoom multiplier to align world geometry to screen center pixels
        Dim targetPanX As Single = screenCenterX - (worldCenter.X * Me._zoomFactor)
        Dim targetPanY As Single = screenCenterY - (worldCenter.Y * Me._zoomFactor)

        ' 4. COMMIT TRANSLATION: Overwrite your private tracking field with the newly balanced viewport coordinates
        Me._panOffset = New PointF(targetPanX, targetPanY)

        ' Synchronize the unrotated workspace instantly by forcing a screen repaint and streaming fresh HUD data
        Me.Invalidate() : Me.StreamTelemetry()
    End Sub

    'Public Sub CenterCameraAroundShapeWorldBounds(ByVal targetShape As Cls_Base_Shape)
    '    If targetShape Is Nothing Then Return

    '    ' 1. GEOMETRIC CENTER DECOMPOSITION: Pull the shape unrotated center point on the stack
    '    Dim shapeCenter As PointF = targetShape.CalculateGeometricCenter()

    '    ' 2. VIEWPORT CENTER CALCULATION: Compute the midpoints of the raw control screen pixels
    '    Dim screenHalfWidth As Single = Me.Width / 2.0F
    '    Dim screenHalfHeight As Single = Me.Height / 2.0F

    '    ' 3. SCALE RE-ALIGNMENT MATRIX: Transform world coordinates to match your current zoom factor offset
    '    Dim nextPanX As Single = screenHalfWidth - (shapeCenter.X * Me._zoomFactor)
    '    Dim nextPanY As Single = screenHalfHeight - (shapeCenter.Y * Me._zoomFactor)

    '    ' 4. LOCK VIEW DISPLACEMENTS: Authoritatively assign the pan offset coordinate structures
    '    Me._panOffset = New PointF(nextPanX, nextPanY)

    '    ' Force immediate screen redraw and telemetry updates to mirror new camera positioning
    '    Me.Invalidate() : Me.StreamTelemetry()
    'End Sub

#End Region

#Region "Telemetry"

    Private Sub PublishStatusBarTelemetry(ByVal screenPos As Point, ByVal worldPt As PointF)
        Dim hostFrm As Form = Me.FindForm()
        If hostFrm IsNot Nothing AndAlso TypeOf hostFrm Is Cls_Drawing Then
            Dim bridge As IMdiParentBridge = DirectCast(hostFrm, Cls_Drawing).ParentBridge
            Dim statusBar As Cls_StatusBar = bridge?.WorkspaceLayout?.MyStatusBar

            If statusBar IsNot Nothing Then
                Dim telemetryData As Str_Telemetry_Payload

                ' Map metrics directly to your revised value-type layout structure fields
                telemetryData.TotalSheets = bridge.WorkspaceLayout.MyDocManager.DocumentRegistry.Count
                telemetryData.State = Me._currentState
                telemetryData.TotalObjects = Me.CanvasData.SchematicComponents.Count
                telemetryData.SelectedObjects = Me._selectionMgr.Count
                telemetryData.ZoomVal = Me._zoomFactor
                telemetryData.Viewoffset = Me._panOffset
                telemetryData.ScreenPoint = New PointF(screenPos.X, screenPos.Y)
                telemetryData.WorldPoint = worldPt

                ' Send the stack-allocated structure straight down to the status bar UI fields
                statusBar.UpdateTelemetryHUD(telemetryData)
            End If
        End If
    End Sub

    Public Sub StreamTelemetry()
        Dim mainHost As System.Windows.Forms.Form = Me.FindForm()

        If mainHost IsNot Nothing AndAlso TypeOf mainHost Is Cls_Drawing Then
            Dim sheet As Cls_Drawing = DirectCast(mainHost, Cls_Drawing)
            Dim bridge As IMdiParentBridge = sheet.ParentBridge

            If bridge?.WorkspaceLayout?.MyStatusBar IsNot Nothing Then
                ' 1. EXTRACTION: Read the persistent active count directly from your selection authority
                Dim activeSelectionCount As Integer = Me._selectionMgr.Count

                ' 2. STRING FORMATTING: Compose high-frequency CAD operational telemetry details
                Dim hudText As String = String.Format("State: {0} | Scale: {1:F1} | Selected Shapes: {2}",
                                                  Me._currentState.ToString(), Me._zoomFactor, activeSelectionCount)

                ' 3. STREAM PAYLOAD: Push the metric text directly down to the status bar control strip
                bridge.WorkspaceLayout.MyStatusBar.UpdateStatusMetrics(hudText)
            End If
        End If
    End Sub

    'Private Sub StreamTelemetry()
    '    Dim mainFrm As Form = Me.FindForm()
    '    If mainFrm IsNot Nothing AndAlso TypeOf mainFrm Is frmMain Then
    '        Dim bridge As IMdiParentBridge = DirectCast(mainFrm, IMdiParentBridge)

    '        Dim sheetCount As Integer = 0
    '        If bridge.MdiAction?.ProjectManager?.DocumentRegistry IsNot Nothing Then
    '            sheetCount = bridge.MdiAction.ProjectManager.DocumentRegistry.Count
    '        End If

    '        Dim metricsText As String = String.Format("State: {0} | Scale: {1:F1} | Active Sheets: {2}",
    '                                             Me._currentState, Me.ZoomFactor, sheetCount)

    '        ' AUTHENTIC ROUTING: Pulls the true custom class component directly out of your workspace layout engine
    '        Dim customStatusBar As Cls_StatusBar = bridge.WorkspaceLayout.MyStatusBar
    '        If customStatusBar IsNot Nothing Then
    '            customStatusBar.UpdateStatusMetrics(metricsText)
    '        End If
    '    End If
    'End Sub


#End Region
    Public Sub NotifySelectionTargetChanged()
        Dim hostForm As System.Windows.Forms.Form = Me.FindForm()

        ' Cast the container form directly to your strongly-typed document sheet wrapper
        If hostForm IsNot Nothing AndAlso TypeOf hostForm Is Cls_Drawing Then
            Dim drawingSheet As Cls_Drawing = DirectCast(hostForm, Cls_Drawing)

            ' Directly fire the shape selection payload straight up to your brain controller
            If drawingSheet.ParentBridge?.MdiAction IsNot Nothing Then
                drawingSheet.ParentBridge.MdiAction.UpdateActiveSelectionMetrics(Me._activeShape)
            End If
        End If
    End Sub

    Public ReadOnly Property CanvasData() As Cls_Drawing
        Get
            ' 1. Locate the immediate parent workspace form window hosting this viewport
            Dim hostSheet As System.Windows.Forms.Form = Me.FindForm()

            ' 2. TYPE-SAFE EVALUATION: Check if the container matches your document data form wrapper
            If hostSheet IsNot Nothing AndAlso TypeOf hostSheet Is Cls_Drawing Then
                ' Return the true, live drawing form instance as the authoritative canvas data channel
                Return DirectCast(hostSheet, Cls_Drawing)
            End If
            Return Nothing
        End Get
    End Property
    ' Define an explicit type-safe delegate signature for the collection update channel
    Public Delegate Sub ShapeCollectionChangedEventHandler(ByVal sender As Object, ByVal e As EventArgs)


    ' The viewport announces database changes to anyone who listens
    Public Event ShapeCollectionChanged As ShapeCollectionChangedEventHandler


        Private _currentState As ViewportState = ViewportState.Idle
    Private _worldDragStart As PointF
    Private _startMousePos As Point
    Private _activeShape As Cls_Base_Shape
    Private ReadOnly _transformer As New Cls_Coordinate_Transformer()
    'Private ReadOnly _canvasData As New Cls_Drawing(Me)
    Private ReadOnly _selectionMgr As New Cls_Selection_Manager()
    Private ReadOnly _resizeMgr As New Cls_Resize_Manager()
    Private ReadOnly _rotationMgr As New Cls_Rotation_Manager()
    Private ReadOnly _grid As New Cls_Grid_Manager()

    Private _panOffset As PointF = New PointF(0.0F, 0.0F)
    Private _zoomFactor As Single = 1.0F
    Private _currentTool As CanvasTool = CanvasTool.SelectPointer

#Region "Properties & Constructors"


    Public ReadOnly Property Grid() As Cls_Grid_Manager
        Get
            Return Me._grid
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

    Public Property ZoomFactor() As Single
        Get
            Return Me._zoomFactor
        End Get
        Set(ByVal value As Single)
            Me._zoomFactor = Math.Max(0.1F, Math.Min(10.0F, value))
        End Set
    End Property

    Public Property IsGridVisible() As Boolean
        Get
            Return Me._grid.IsVisible
        End Get
        Set(ByVal value As Boolean)
            Me._grid.IsVisible = value
            Me.Invalidate()
        End Set
    End Property

    Public Property IsGridSnappingEnabled() As Boolean
        Get
            Return Me._grid.IsSnapEnabled
        End Get
        Set(ByVal value As Boolean)
            Me._grid.IsSnapEnabled = value
            Me.Invalidate()
        End Set
    End Property

    Public Property ActiveShape() As Cls_Base_Shape
        Get
            Return Me._activeShape
        End Get
        Set(ByVal value As Cls_Base_Shape)
            Me._activeShape = value
            Me.Invalidate()
        End Set
    End Property

#End Region


    ' Target File: Cls_Viewport.vb
    ' Project Namespace: mdi_test
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    Public Sub EndStateTransitionMouseUp()
        Me._currentState = ViewportState.Idle

        Dim currentHost As Form = Me.FindForm()
        If currentHost IsNot Nothing AndAlso TypeOf currentHost Is Cls_Drawing Then
            Dim drawingSheet As Cls_Drawing = DirectCast(currentHost, Cls_Drawing)

            If drawingSheet.ParentBridge?.MdiAction IsNot Nothing Then
                Dim lastSelectedShape As Cls_Base_Shape = Me._selectionMgr.TryGetActiveFocus()
                drawingSheet.ParentBridge.MdiAction.UpdateActiveSelectionMetrics(lastSelectedShape)

                ' 🎯 REFRESH THE INSPECTOR TREE VIEW: Force immediate localized layout updates
                drawingSheet.ParentBridge.MdiAction.SynchronizeInspectorTreeToActiveSheet()
            End If
        End If
        Me.Invalidate()
    End Sub

    'Public Sub EndStateTransitionMouseUp()
    '    Dim currentHost As Form = Me.FindForm()
    '    Dim isShiftDown As Boolean = (ModifierKeys And Keys.Shift) = Keys.Shift

    '    ' RULE 1: If user was stretching a box, finalize the marquee sweep across live components
    '    If Me._currentState = ViewportState.MarqueeSelecting AndAlso Me.CanvasData?.SchematicComponents IsNot Nothing Then
    '        Me._selectionMgr.FinalizeMarqueeSelection(Me.CanvasData.SchematicComponents, isShiftDown)
    '        Me._selectionMgr.IsMarqueeSelecting = False
    '    End If

    '    Me._currentState = ViewportState.Idle

    '    ' PUSH SELECTION UP TO INSPECTOR: Query your authority focus target reference 
    '    If currentHost IsNot Nothing AndAlso TypeOf currentHost Is Cls_Drawing Then
    '        Dim drawingSheet As Cls_Drawing = DirectCast(currentHost, Cls_Drawing)
    '        Dim activeFocus As Cls_Base_Shape = Me._selectionMgr.TryGetActiveFocus()

    '        If drawingSheet.ParentBridge?.MdiAction IsNot Nothing Then
    '            drawingSheet.ParentBridge.MdiAction.UpdateActiveSelectionMetrics(activeFocus)
    '        End If
    '    End If
    '    Me.Invalidate()
    'End Sub

    'Public Sub EndStateTransitionMouseUp()
    '    Me._currentState = ViewportState.Idle

    '    Dim currentHost As Form = Me.FindForm()
    '    If currentHost IsNot Nothing AndAlso TypeOf currentHost Is Cls_Drawing Then
    '        Dim drawingSheet As Cls_Drawing = DirectCast(currentHost, Cls_Drawing)

    '        If drawingSheet.ParentBridge?.MdiAction IsNot Nothing Then
    '            ' SYNCHRONIZED FOCUS: Extract the absolute authority on last selection directly from the manager
    '            Dim lastSelectedShape As Cls_Base_Shape = Me._selectionMgr.TryGetActiveFocus()

    '            ' Authoritatively push the persistent selection target down to bind into the inspector rows
    '            drawingSheet.ParentBridge.MdiAction.UpdateActiveSelectionMetrics(lastSelectedShape)
    '        End If
    '    End If
    '    Me.Invalidate()
    'End Sub

    'Public Sub EndStateTransitionMouseUp()
    '    Me._currentState = ViewportState.Idle

    '    Dim currentHost As Form = Me.FindForm()
    '    If currentHost IsNot Nothing AndAlso TypeOf currentHost Is Cls_Drawing Then
    '        Dim drawingSheet As Cls_Drawing = DirectCast(currentHost, Cls_Drawing)

    '        ' PERSISTENCE CHANNEL: Only send the shape to the property grid if we have a valid selection!
    '        If drawingSheet.ParentBridge?.MdiAction IsNot Nothing AndAlso Me._activeShape IsNot Nothing Then
    '            ' This locks the active shape inside the inspector row matrix continuously on release
    '            drawingSheet.ParentBridge.MdiAction.UpdateActiveSelectionMetrics(Me._activeShape)
    '        End If
    '    End If
    '    Me.Invalidate()
    'End Sub


    'Public Sub EndStateTransitionMouseUp()
    '    Me._currentState = ViewportState.Idle

    '    Dim currentHost As Form = Me.FindForm()
    '    If currentHost IsNot Nothing AndAlso TypeOf currentHost Is Cls_Drawing Then
    '        Dim drawingSheet As Cls_Drawing = DirectCast(currentHost, Cls_Drawing)

    '        If drawingSheet.ParentBridge?.MdiAction IsNot Nothing Then
    '            Dim activeGate As Cls_Base_Shape = Me._selectionMgr.TryGetActiveFocus()
    '            drawingSheet.ParentBridge.MdiAction.UpdateActiveSelectionMetrics(activeGate)
    '        End If
    '    End If
    '    Me.Invalidate()
    'End Sub

    'Public Sub EndStateTransitionMouseUp()
    '    Me._currentState = ViewportState.Idle
    '    RaiseEvent ShapeCollectionChanged(Me, EventArgs.Empty)
    '    Me.Invalidate()
    'End Sub

    'Public Sub EndStateTransitionMouseUp()
    '    Me._currentState = ViewportState.Idle

    '    Dim mainFrm As Form = Me.FindForm()
    '    If mainFrm IsNot Nothing AndAlso TypeOf mainFrm Is IMdiParentBridge Then
    '        Dim bridge As IMdiParentBridge = DirectCast(mainFrm, IMdiParentBridge)

    '        ' 1. EXECUTE COUPLING SIGNAL: Instantly forces the old sheets to refresh their shape nodes
    '        bridge.MdiAction.SignalCanvasDataChanged()

    '        ' 2. Keep the property grid inspector synced with the active unrotated target
    '        If Me._activeShape IsNot Nothing AndAlso bridge.WorkspaceLayout?.MyExplorer IsNot Nothing Then
    '            bridge.WorkspaceLayout.MyExplorer.UpdatePropertyGridInspector(Me._activeShape)
    '        End If
    '    End If
    '    Me.Invalidate()
    'End Sub

    'Public Sub EndStateTransitionMouseUp()
    '    Diagnostics.Debug.WriteLine($"[DROP DEBUG 1] MouseUp Fired. Former State: {Me._currentState}")
    '    Me._currentState = ViewportState.Idle

    '    Dim mainFrm As Form = Me.FindForm()
    '    If mainFrm IsNot Nothing AndAlso TypeOf mainFrm Is IMdiParentBridge Then
    '        Dim bridge As IMdiParentBridge = DirectCast(mainFrm, IMdiParentBridge)
    '        Dim explorer As Cls_Explorer = bridge.WorkspaceLayout?.MyExplorer

    '        If explorer IsNot Nothing Then
    '            Diagnostics.Debug.WriteLine($"[DROP DEBUG 2] Target Cls_Explorer Found. Active Shapes In Viewport Data Collection: {Me._canvasData?.SchematicComponents?.Count}")
    '            ' Invoke the parameterless node population pass directly
    '            explorer.RefreshHierarchyTree()

    '            If Me._activeShape IsNot Nothing Then
    '                explorer.UpdatePropertyGridInspector(Me._activeShape)
    '            End If
    '        End If
    '    End If
    '    Me.Invalidate()
    'End Sub

    'Public Sub EndStateTransitionMouseUp()
    '    Me._currentState = ViewportState.Idle

    '    Dim mainFrm As Form = Me.FindForm()
    '    If mainFrm IsNot Nothing AndAlso TypeOf mainFrm Is IMdiParentBridge Then
    '        Dim bridge As IMdiParentBridge = DirectCast(mainFrm, IMdiParentBridge)

    '        ' AUTHENTIC SYNC TRACE: Forces the tree view to instantly update its shape records
    '        If bridge.WorkspaceLayout?.MyExplorer IsNot Nothing Then
    '            bridge.WorkspaceLayout.MyExplorer.RefreshHierarchyTree()
    '        End If

    '        ' Safely update the property grid inspector if a shape is active
    '        If Me._activeShape IsNot Nothing AndAlso bridge.WorkspaceLayout.MyExplorer IsNot Nothing Then
    '            bridge.WorkspaceLayout.MyExplorer.UpdatePropertyGridInspector(Me._activeShape)
    '        End If
    '    End If
    '    Me.Invalidate()
    'End Sub



#Region "explorer"

    '' Target File: Cls_Viewport.vb
    '' Architecture Standard: VB.NET | Option Strict On | Explicit Binding
    '' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    'Public Sub SignalShapeCollectionChanged()
    '    Dim parentForm As System.Windows.Forms.Form = Me.FindForm()

    '    If parentForm IsNot Nothing AndAlso TypeOf parentForm Is IMdiParentBridge Then
    '        Dim bridge As IMdiParentBridge = DirectCast(parentForm, IMdiParentBridge)

    '        ' AUTHORITATIVE SIGNAL: Instantly force the explorer layout pane to re-render inner shape text labels
    '        If bridge.WorkspaceLayout?.MyExplorer IsNot Nothing Then
    '            bridge.WorkspaceLayout.MyExplorer.RefreshHierarchyTree()
    '        End If
    '    End If
    'End Sub



#End Region


    ' Target File: Cls_Viewport.vb
    ' Target File: Cls_Viewport.vb
    ' Project Namespace: mdi_test
    ' Said Rules Compliance: Zero Guessing | No Non-Existent Token Variables | Locked Classes Only
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    Private Sub EvaluateStandardCanvasSelectionTrack(ByVal worldPt As PointF)
        Dim hitGate As Cls_Base_Shape = Me.CanvasData.HitTestShapes(worldPt, Me._transformer)
        Dim isShiftDown As Boolean = (ModifierKeys And Keys.Shift) = Keys.Shift

        If hitGate IsNot Nothing Then
            ' RULE 3: Shift + Click evaluates a selection status toggle
            If isShiftDown AndAlso Me._selectionMgr.IsShapeSelected(hitGate) Then
                Me._selectionMgr.DeselectShape(hitGate)
                Me.RequestStateTransition(ViewportState.Idle, worldPt)
            Else
                Me._selectionMgr.SelectShape(hitGate, isShiftDown)
                Me.RequestStateTransition(ViewportState.GroupDragging, worldPt)
            End If
        Else
            ' RULE 2: Clicking on empty space clears selection if Shift is not held down
            If Not isShiftDown Then Me._selectionMgr.Clear()
            Me._selectionMgr.IsMarqueeSelecting = True
            Me._selectionMgr.MarqueeStartPoint = worldPt
            Me._selectionMgr.MarqueeCurrentPoint = worldPt
            Me.RequestStateTransition(ViewportState.MarqueeSelecting, worldPt)
        End If
    End Sub

    Private Sub ClearAllShapeSelections()
        If Me.CanvasData?.SchematicComponents Is Nothing Then Return
        For i As Integer = 0 To Me.CanvasData.SchematicComponents.Count - 1
            Dim shape As Cls_Base_Shape = Me.CanvasData.SchematicComponents(i)
            If shape IsNot Nothing Then shape.IsSelected = False
        Next
    End Sub


    '''' <summary>
    '''' Standard selection router pipeline evaluating component click hits, multi-selection toggles, or marquee boxes.
    '''' </summary>
    'Private Sub EvaluateStandardCanvasSelectionTrack(ByVal worldPt As PointF)
    '    Dim hitGate As Cls_Base_Shape = Me.CanvasData.HitTestShapes(worldPt, Me._transformer)
    '    Dim isShiftDown As Boolean = (ModifierKeys And Keys.Shift) = Keys.Shift

    '    If hitGate IsNot Nothing Then
    '        ' FIXED STRATEGY: Intercept shift clicks to evaluate a clean selection status toggle!
    '        If isShiftDown AndAlso Me._selectionMgr.IsShapeSelected(hitGate) Then
    '            Me._selectionMgr.DeselectShape(hitGate)
    '            Me.RequestStateTransition(ViewportState.Idle, worldPt)
    '        Else
    '            Me._selectionMgr.SelectShape(hitGate, isShiftDown)
    '            Me.RequestStateTransition(ViewportState.GroupDragging, worldPt)
    '        End If
    '    Else
    '        ' No component intersected: Initialize standard marquee selection box window tracking
    '        If Not isShiftDown Then Me._selectionMgr.Clear()
    '        Me._selectionMgr.IsMarqueeSelecting = True
    '        Me._selectionMgr.MarqueeStartPoint = worldPt
    '        Me._selectionMgr.MarqueeCurrentPoint = worldPt
    '        Me.RequestStateTransition(ViewportState.MarqueeSelecting, worldPt)
    '    End If
    'End Sub

#Region "Explrer updating"
    ' Target File: Cls_Viewport.vb (Authoritative Shell Event Synchronizer Block)
    ' Explicit configuration locked for file scope | Option Strict On | Explicit Binding
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    Protected Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)
        MyBase.OnMouseUp(e)
        Me.Capture = False

        Dim completedMarquee As Boolean = (Me._currentState = ViewportState.MarqueeSelecting)
        If completedMarquee Then
            Me._selectionMgr.FinalizeMarqueeSelection(Me.CanvasData.SchematicComponents, (ModifierKeys And Keys.Shift) = Keys.Shift)
        End If

        ' 1. Transition the core state machine token back to Idle configuration rules
        Me.RequestStateTransition(ViewportState.Idle, PointF.Empty)

        ' 2. FIXED STRATEGY: Notify the parent orchestration brain to push real-time property and tree updates!
        Me.BroadcastShellHierarchyRefresh()
        ' 3. EXECUTION DISPATCH: Fire the rigid state conversion and refresh the tree view UI
        Me.EndStateTransitionMouseUp()
    End Sub

    Protected Overrides Sub OnKeyDown(ByVal e As KeyEventArgs)
        MyBase.OnKeyDown(e)

        ' 3. SHAPE DELETION TRANSITION MATRIX TRACK
        If e.KeyCode = Keys.Delete AndAlso Me._selectionMgr.Count > 0 Then
            Me.CanvasData.PurgeShapesGroup(Me._selectionMgr.SelectedShapes)
            Me._selectionMgr.Clear()

            ' FIXED STRATEGY: Force tree and property grid synchronization immediately following deletions
            Me.BroadcastShellHierarchyRefresh()
        End If
    End Sub

    ' Target File: Cls_Viewport.vb
    ' Project Namespace: mdi_test
    ' Architecture Standard: VB.NET | Option Strict On | Explicit Binding
    ' Said Rules Compliance: Zero Guessing | No Non-Existent Token Variables | Locked Classes Only
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    Private Sub InstantiateDroppedGateComponent(ByVal tokenName As String, ByVal insertionPt As PointF)
        Dim newGate As Cls_Base_Shape = Nothing

        If String.Equals(tokenName, "ANDGATE", StringComparison.OrdinalIgnoreCase) Then
            newGate = New Cls_Gate_And(insertionPt)
        ElseIf String.Equals(tokenName, "NOTGATE", StringComparison.OrdinalIgnoreCase) Then
            newGate = New Cls_Gate_Not(insertionPt)
        ElseIf String.Equals(tokenName, "ORGATE", StringComparison.OrdinalIgnoreCase) Then
            newGate = New Cls_Gate_Or(insertionPt)
        End If

        If newGate IsNot Nothing Then
            Me.CanvasData.SchematicComponents.Add(newGate)
            Me._selectionMgr.SelectShape(newGate, False)

            ' RULE 1 & 4 ENFORCEMENT: Explicitly force the dropped primitive selection state
            newGate.IsSelected = True

            Dim hostFrm As Form = Me.FindForm()
            If hostFrm IsNot Nothing AndAlso TypeOf hostFrm Is Cls_Drawing Then
                Dim sheet As Cls_Drawing = DirectCast(hostFrm, Cls_Drawing)
                ' Force the PropertyGrid rows to lock focus onto the newly dropped shape metrics
                sheet.ParentBridge.MdiAction?.UpdateActiveSelectionMetrics(newGate)
                sheet.ParentBridge.MdiAction.SynchronizeInspectorTreeToActiveSheet()
            End If
            Me.BroadcastShellHierarchyRefresh()
        End If

    End Sub


    '''' <summary>
    '''' Instantiates a concrete logic gate subclass and pushes it directly into the master database.
    '''' </summary>
    'Private Sub InstantiateDroppedGateComponent(ByVal tokenName As String, ByVal insertionPt As PointF)
    '    Dim newGate As Cls_Base_Shape = Nothing

    '    If String.Equals(tokenName, "ANDGATE", StringComparison.OrdinalIgnoreCase) Then
    '        newGate = New Cls_Gate_And(insertionPt)
    '    ElseIf String.Equals(tokenName, "NOTGATE", StringComparison.OrdinalIgnoreCase) Then
    '        newGate = New Cls_Gate_Not(insertionPt)
    '    ElseIf String.Equals(tokenName, "ORGATE", StringComparison.OrdinalIgnoreCase) Then
    '        newGate = New Cls_Gate_Or(insertionPt)
    '    End If

    '    If newGate IsNot Nothing Then
    '        Me.CanvasData.SchematicComponents.Add(newGate)
    '        Me._selectionMgr.SelectShape(newGate, False)

    '        ' FIXED STRATEGY: Update the outer shell layout panels instantly when a tool component drops!
    '        Me.BroadcastShellHierarchyRefresh()
    '    End If
    'End Sub

    ''' <summary>
    ''' High-utility companion router method to lookup the main brain and update panels under the 25-line ceiling.
    ''' </summary>
    Private Sub BroadcastShellHierarchyRefresh()
        Dim parentForm As Form = Me.FindForm()
        Dim mainBridge As IMdiParentBridge = TryCast(parentForm?.MdiParent, IMdiParentBridge)

        If mainBridge?.MdiAction IsNot Nothing Then
            ' 1. Command the brain to clear and rebuild the TreeView nodes tracking collection list data
            'mainBridge.MdiAction.SynchronizeExplorerHierarchyTree(Me)

            ' 2. Command the brain to refresh the property grids using the current top focus target component
            mainBridge.MdiAction.FlushPropertyGridInspector(Me._selectionMgr.TryGetActiveFocus())
        End If

        Me.Invalidate()
    End Sub

#End Region



    ' Target File: Cls_Viewport.vb (Strategic Constructor Theme Initialization)
    ' Explicit configuration locked for file scope | Option Strict On | Explicit Binding
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    Public Sub New()
        ' 1. Allocate default double-buffering style flags to eliminate layout redraw stutters
        Me.DoubleBuffered = True

        ' 2. FIXED STRATEGY: Pull the authoritative backing color directly from the configuration singleton!
        Dim config As Cls_Configuration_Manager = Cls_Configuration_Manager.Instance
        Me.BackColor = config.PanelBackColor

        ' 3. Unlock the WinForms OLE drag landing entry gate for toolbox component drops
        Me.AllowDrop = True
    End Sub




#Region "Consolidated State Transition Matrix Input Entries"

    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        Me.Focus()

        Dim isAltDown As Boolean = (ModifierKeys And Keys.Alt) = Keys.Alt
        If e.Button = MouseButtons.Middle OrElse (e.Button = MouseButtons.Left AndAlso isAltDown AndAlso Me._selectionMgr.Count = 0) Then
            Me._currentState = ViewportState.Panning : Me._startMousePos = e.Location : Me.Cursor = Cursors.NoMove2D : Exit Sub
        End If

        If e.Button = MouseButtons.Left Then
            Me.Capture = True
            Me.Do_State_Transition_On_Mouse_down(e)

        End If
    End Sub

    Protected Overrides Sub OnMouseMove(ByVal e As MouseEventArgs)
        MyBase.OnMouseMove(e)
        Me.Do_State_Transition_On_Mouse_Move(e)
    End Sub

    'Protected Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)
    '    MyBase.OnMouseUp(e)
    '    If e.Button = MouseButtons.Left Then
    '        Me.Do_State_Transition_On_Mouse_Up(e)
    '    End If
    'End Sub

    Protected Overrides Sub OnMouseWheel(ByVal e As MouseEventArgs)
        MyBase.OnMouseWheel(e)
        Dim scaleStep As Single = If(e.Delta > 0, 1.1F, 0.9F)
        Me.ExecuteZoomStepByDelta(scaleStep, e.Location)
    End Sub

    'Protected Overrides Sub OnKeyDown(ByVal e As KeyEventArgs)
    '    MyBase.OnKeyDown(e)
    '    If e.KeyCode = Keys.Delete AndAlso Me._selectionMgr.Count > 0 Then
    '        Me._canvasData.PurgeShapesGroup(Me._selectionMgr.SelectedShapes)
    '        Me._selectionMgr.Clear()
    '        Me.Invalidate()
    '    End If
    'End Sub

#End Region

#Region "State Transition Logic Processing"

    Private Sub Do_State_Transition_On_Mouse_down(ByVal e As MouseEventArgs)
        Dim worldPt As PointF = Me._transformer.TransformScreenToWorld(e.Location, Me._panOffset, Me._zoomFactor)
        Dim focusGate As Cls_Base_Shape = Me._selectionMgr.TryGetActiveFocus()
        Dim isCtrlDown As Boolean = (ModifierKeys And Keys.Control) = Keys.Control
        Dim isAltDown As Boolean = (ModifierKeys And Keys.Alt) = Keys.Alt

        If isCtrlDown AndAlso focusGate IsNot Nothing Then
            Dim handle As ResizeHandle = Me._resizeMgr.HitTestHandles(worldPt, focusGate, Me._zoomFactor)
            If handle <> ResizeHandle.None Then
                Me._resizeMgr.LockActiveHandle(handle)
                Me.RequestStateTransition(ViewportState.Resizing, worldPt) : Exit Sub
            End If
        End If

        If isAltDown AndAlso focusGate IsNot Nothing Then
            Me.RequestStateTransition(ViewportState.Rotating, worldPt)
            Me.Cursor = Cursors.UpArrow : Exit Sub
        End If

        Me.EvaluateStandardCanvasSelectionTrack(worldPt)

        ' Insert at the absolute end of Cls_Viewport.Do_State_Transition_On_Mouse_down:
        Dim currentHost As Form = Me.FindForm()
        If currentHost IsNot Nothing AndAlso TypeOf currentHost Is Cls_Drawing Then
            Dim drawingSheet As Cls_Drawing = DirectCast(currentHost, Cls_Drawing)

            ' Push the focused shape instance target reference up to the inspector
            If drawingSheet.ParentBridge?.MdiAction IsNot Nothing Then
                drawingSheet.ParentBridge.MdiAction.UpdateActiveSelectionMetrics(focusGate)
            End If
        End If
    End Sub



    ''' <summary>
    ''' Routes active trajectory movements and broadcasts live pixel metrics out to the status bar HUD.
    ''' </summary>
    Private Sub Do_State_Transition_On_Mouse_Move(ByVal e As MouseEventArgs)
        ' 1. Project the hardware screen pointer pixels to absolute world space coordinates
        Dim worldPt As PointF = Me._transformer.TransformScreenToWorld(e.Location, Me._panOffset, Me._zoomFactor)

        ' 2. Broadcast live, high-frequency coordinate metrics straight out to our status bar panel
        Me.PublishStatusBarTelemetry(e.Location, worldPt)

        ' 3. Process active movement tracking loops inside their isolated state tracks
        Select Case Me._currentState
            Case ViewportState.Panning : Me.ExecutePanningDrag(e.Location)
            Case ViewportState.GroupDragging : Me.ExecuteGroupDraggingPipeline(worldPt)
            Case ViewportState.Resizing : Me.ExecuteResizingPipeline(worldPt)
            Case ViewportState.Rotating : Me.ExecuteRotatingPipeline(worldPt)
            Case ViewportState.MarqueeSelecting : Me._selectionMgr.MarqueeCurrentPoint = worldPt : Me.Invalidate()
            Case ViewportState.Idle : Me._resizeMgr.UpdateHoverCursor(worldPt, Me._selectionMgr.TryGetActiveFocus(), Me._zoomFactor, Me)
        End Select
    End Sub




    Private Sub Do_State_Transition_On_Mouse_Up(ByVal e As MouseEventArgs)
        Me.Capture = False

        If Me._currentState = ViewportState.MarqueeSelecting Then
            Me._selectionMgr.FinalizeMarqueeSelection(Me.CanvasData.SchematicComponents, (ModifierKeys And Keys.Shift) = Keys.Shift)
        End If

        Me.RequestStateTransition(ViewportState.Idle, PointF.Empty)
    End Sub

#End Region

#Region "Consolidated Interaction Pipelines"

    'Private Sub EvaluateStandardCanvasSelectionTrack(ByVal worldPt As PointF)
    '    Dim hitGate As Cls_Base_Shape = Me._canvasData.HitTestShapes(worldPt, Me._transformer)
    '    Dim isShiftDown As Boolean = (ModifierKeys And Keys.Shift) = Keys.Shift

    '    If hitGate IsNot Nothing Then
    '        Me._selectionMgr.SelectShape(hitGate, isShiftDown)
    '        Me.RequestStateTransition(ViewportState.GroupDragging, worldPt)
    '    Else
    '        If Not isShiftDown Then Me._selectionMgr.Clear()
    '        Me._selectionMgr.IsMarqueeSelecting = True
    '        Me._selectionMgr.MarqueeStartPoint = worldPt
    '        Me._selectionMgr.MarqueeCurrentPoint = worldPt
    '        Me.RequestStateTransition(ViewportState.MarqueeSelecting, worldPt)
    '    End If
    'End Sub

    Private Sub ExecutePanningDrag(ByVal currentScreenPos As Point)
        Dim dx As Single = Convert.ToSingle(currentScreenPos.X - Me._startMousePos.X)
        Dim dy As Single = Convert.ToSingle(currentScreenPos.Y - Me._startMousePos.Y)

        Me._panOffset.X += dx : Me._panOffset.Y += dy
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

    Private Sub ExecuteRotatingPipeline(ByVal worldPt As PointF)
        Dim focusGate As Cls_Base_Shape = Me._selectionMgr.TryGetActiveFocus()
        If focusGate Is Nothing Then Exit Sub

        Me._rotationMgr.ExecuteRotationStep(worldPt, Me._worldDragStart, focusGate)
        Me.Invalidate()
    End Sub

#End Region

#Region "View Scaler Logic Engine"

    Public Sub ExecuteZoomIn()
        Me.ExecuteZoomStepByDelta(1.1F, New Point(Me.Width \ 2, Me.Height \ 2))
    End Sub

    Public Sub ExecuteZoomOut()
        Me.ExecuteZoomStepByDelta(0.9F, New Point(Me.Width \ 2, Me.Height \ 2))
    End Sub

    Public Sub ExecuteZoomToFit()
        Dim components As List(Of Cls_Base_Shape) = Me.CanvasData.SchematicComponents
        If components Is Nothing OrElse components.Count = 0 Then
            Me._zoomFactor = 1.0F : Me._panOffset = New PointF(0.0F, 0.0F) : Me.Invalidate() : Exit Sub
        End If

        Dim firstRect As RectangleF = components(0).Bounds
        Dim minX As Single = firstRect.X : Dim minY As Single = firstRect.Y
        Dim maxX As Single = firstRect.Right : Dim maxY As Single = firstRect.Bottom

        For i As Integer = 1 To components.Count - 1
            Dim r As RectangleF = components(i).Bounds
            minX = Math.Min(minX, r.X) : minY = Math.Min(minY, r.Y)
            maxX = Math.Max(maxX, r.Right) : maxY = Math.Max(maxY, r.Bottom)
        Next

        Me.CalculateZoomToFitOffsets(minX, minY, maxX - minX, maxY - minY)
    End Sub
    Private Sub ExecuteZoomStepByDelta(ByVal scaleDelta As Single, ByVal screenCursorPos As Point)
        Dim worldFocusBefore As PointF = Me._transformer.TransformScreenToWorld(screenCursorPos, Me._panOffset, Me._zoomFactor)
        Dim nextZoom As Single = Me._zoomFactor * scaleDelta
        Me._zoomFactor = Math.Max(0.1F, Math.Min(10.0F, nextZoom))
        Dim nextPanX As Double = Convert.ToDouble(screenCursorPos.X) - (Convert.ToDouble(worldFocusBefore.X) * Convert.ToDouble(Me._zoomFactor))
        Dim nextPanY As Double = Convert.ToDouble(screenCursorPos.Y) - (Convert.ToDouble(worldFocusBefore.Y) * Convert.ToDouble(Me._zoomFactor))
        Me._panOffset.X = Convert.ToSingle(nextPanX) : Me._panOffset.Y = Convert.ToSingle(nextPanY)
        Me.Invalidate()
    End Sub
    Private Sub CalculateZoomToFitOffsets(ByVal contentX As Single, ByVal contentY As Single, ByVal contentW As Single, ByVal contentH As Single)
        Dim padding As Single = 100.0F
        Dim viewW As Single = Math.Max(1.0F, Convert.ToSingle(Me.Width) - padding)
        Dim viewH As Single = Math.Max(1.0F, Convert.ToSingle(Me.Height) - padding)
        Dim scaleX As Single = viewW / Math.Max(1.0F, contentW)
        Dim scaleY As Single = viewH / Math.Max(1.0F, contentH)
        Me._zoomFactor = Math.Max(0.1F, Math.Min(4.0F, Math.Min(scaleX, scaleY)))
        Dim offsetX As Double = (Convert.ToDouble(Me.Width) - Convert.ToDouble(contentW) * Convert.ToDouble(Me._zoomFactor)) / 2.0
        Dim offsetY As Double = (Convert.ToDouble(Me.Height) - Convert.ToDouble(contentH) * Convert.ToDouble(Me._zoomFactor)) / 2.0
        Me._panOffset.X = Convert.ToSingle(offsetX - Convert.ToDouble(contentX) * Convert.ToDouble(Me._zoomFactor))
        Me._panOffset.Y = Convert.ToSingle(offsetY - Convert.ToDouble(contentY) * Convert.ToDouble(Me._zoomFactor))
        Me.Invalidate()
    End Sub
#End Region
#Region "Unified Blueprint Graphics Rendering"
    ' Target File: Cls_Viewport.vb
    ' Project Namespace: mdi_test
    ' Architecture Standard: VB.NET | Option Strict On | Explicit Binding
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    'Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
    '    MyBase.OnPaint(e)
    '    Dim g As Graphics = e.Graphics
    '    Using cameraMatrix As Matrix = Me._transformer.CreateCameraMatrix(Me._panOffset, Me._zoomFactor)
    '        g.Transform = cameraMatrix
    '        Dim visibleBounds As RectangleF = Me._transformer.CalculateWorldBounds(Me._panOffset, Me._zoomFactor, Me.Width, Me.Height)
    '        Me._grid.Render(g, Me._zoomFactor, visibleBounds)
    '        Me.RenderProtectedWorldCanvas(g, cameraMatrix)
    '        Me.RenderSelectionOverlays(g, cameraMatrix)
    '        If Me._currentState = ViewportState.MarqueeSelecting Then Me.DrawMarqueeOverlayWindow(g)
    '    End Using
    '    g.ResetTransform() : Me.StreamTelemetry()
    'End Sub

    'Private Sub RenderProtectedWorldCanvas(ByVal g As Graphics, ByVal cameraMatrix As Matrix)
    '    ' PURE STATELESS DATA-EXTRACTION: Grab the shapes straight from the hosting form container
    '    Dim hostSheet As Form = Me.FindForm()
    '    If hostSheet IsNot Nothing AndAlso TypeOf hostSheet Is Cls_Drawing Then
    '        Dim shapesList As List(Of Cls_Base_Shape) = DirectCast(hostSheet, Cls_Drawing).SchematicComponents
    '        For i As Integer = 0 To shapesList.Count - 1
    '            If shapesList(i) IsNot Nothing Then
    '                shapesList(i).Draw(g, Me)
    '            End If
    '        Next
    '    End If
    'End Sub

    'Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
    '    MyBase.OnPaint(e)
    '    Dim g As Graphics = e.Graphics
    '    g.Transform = Me.GetWorldMatrix()

    '    ' PURE STATELESS RENDERING: Extract the elements list directly from your host window
    '    Dim hostSheet As Form = Me.FindForm()
    '    If hostSheet IsNot Nothing AndAlso TypeOf hostSheet Is Cls_Drawing Then
    '        Dim shapesList As List(Of Cls_Base_Shape) = DirectCast(hostSheet, Cls_Drawing).SchematicComponents
    '        For Each shape As Cls_Base_Shape In shapesList
    '            If shape IsNot Nothing Then shape.Draw(g, Me)
    '        Next
    '    End If

    '    Me.StreamTelemetry()
    'End Sub



    Private Function GetWorldMatrix() As Matrix
        Dim transformMatrix As New Matrix()

        ' 1. SPATIAL CAMERA SHIFT: Apply your unrotated pan coordinate displacements first
        transformMatrix.Translate(Me._panOffset.X, Me._panOffset.Y)

        ' 2. SPATIAL ZOOM RATIO: Apply your uniform scaling multiplier ratio safely
        transformMatrix.Scale(Me._zoomFactor, Me._zoomFactor)

        Return transformMatrix
    End Function


    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        MyBase.OnPaint(e)
        Dim g As Graphics = e.Graphics
        Using cameraMatrix As Matrix = Me._transformer.CreateCameraMatrix(Me._panOffset, Me._zoomFactor)
            g.Transform = cameraMatrix
            Dim visibleBounds As RectangleF = Me._transformer.CalculateWorldBounds(Me._panOffset, Me._zoomFactor, Me.Width, Me.Height)
            Me._grid.Render(g, Me._zoomFactor, visibleBounds)
            Me.RenderProtectedWorldCanvas(g, cameraMatrix)
            Me.RenderSelectionOverlays(g, cameraMatrix)
            If Me._currentState = ViewportState.MarqueeSelecting Then Me.DrawMarqueeOverlayWindow(g)
        End Using
        g.ResetTransform()
        ' UNCONDITIONAL UPDATE TRIGGER: Stream live metrics directly to the status bar HUD
        Me.StreamTelemetry()
    End Sub
    Private Sub RenderProtectedWorldCanvas(ByVal g As Graphics, ByVal cameraMatrix As Matrix)
        If g Is Nothing OrElse cameraMatrix Is Nothing Then Exit Sub
        Dim componentsList As List(Of Cls_Base_Shape) = Me.CanvasData.SchematicComponents
        If componentsList Is Nothing Then Exit Sub
        For i As Integer = 0 To componentsList.Count - 1
            Dim shape As Cls_Base_Shape = componentsList(i)
            If shape IsNot Nothing Then Me._transformer.ExecuteSafeShapeRender(g, cameraMatrix, shape)
        Next
    End Sub

    'Private Sub RenderSelectionOverlays(ByVal g As Graphics, ByVal cameraMatrix As Matrix)
    '    Dim activeShape As Cls_Base_Shape = Me._selectionMgr.TryGetActiveFocus()
    '    g.Transform = cameraMatrix
    '    For Each shape As Cls_Base_Shape In Me._selectionMgr.SelectedShapes
    '        Me.RenderSelectionOutline(g, Me._zoomFactor, shape)
    '    Next
    '    If activeShape IsNot Nothing Then
    '        Dim isCtrlDown As Boolean = (ModifierKeys And Keys.Control) = Keys.Control
    '        If Me._currentState = ViewportState.Resizing OrElse isCtrlDown Then Me._resizeMgr.DrawResizeHandles(g, Me._zoomFactor, activeShape)
    '        If Me._currentState = ViewportState.Rotating Then Me._rotationMgr.DrawRotationOverlayMetrics(g, Me._zoomFactor, activeShape)
    '    End If
    'End Sub

    ' Target File: Cls_Viewport.vb
    ' Project Namespace: mdi_test
    ' Said Rules Compliance: Zero Guessing | No Non-Existent Token Variables | Locked Classes Only
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    Private Sub RenderSelectionOverlays(ByVal g As Graphics, ByVal cameraMatrix As Matrix)
        Dim activeShape As Cls_Base_Shape = Nothing
        Dim selectedCount As Integer = 0
        g.Transform = cameraMatrix

        If Me.CanvasData?.SchematicComponents IsNot Nothing Then
            For Each shape As Cls_Base_Shape In Me.CanvasData.SchematicComponents
                If shape IsNot Nothing AndAlso shape.IsSelected Then
                    selectedCount += 1
                    activeShape = shape
                    Me.RenderSelectionOutline(g, Me._zoomFactor, shape)
                End If
            Next
        End If

        ' RULE 4: The resize handles and overlay metrics are ONLY drawn if a single shape has focus
        If selectedCount = 1 AndAlso activeShape IsNot Nothing Then
            Dim isCtrlDown As Boolean = (ModifierKeys And Keys.Control) = Keys.Control
            If Me._currentState = ViewportState.Resizing OrElse isCtrlDown Then
                Me._resizeMgr.DrawResizeHandles(g, Me._zoomFactor, activeShape)
            End If
            If Me._currentState = ViewportState.Rotating Then
                Me._rotationMgr.DrawRotationOverlayMetrics(g, Me._zoomFactor, activeShape)
            End If
        End If
    End Sub

    'Private Sub RenderSelectionOverlays(ByVal g As Graphics, ByVal cameraMatrix As Matrix)
    '    Dim activeShape As Cls_Base_Shape = Nothing
    '    Dim selectedCount As Integer = 0
    '    g.Transform = cameraMatrix

    '    For Each shape As Cls_Base_Shape In Me.CanvasData.SchematicComponents
    '        If shape IsNot Nothing AndAlso shape.IsSelected Then
    '            selectedCount += 1
    '            activeShape = shape
    '        End If
    '    Next

    '    If selectedCount = 1 AndAlso activeShape IsNot Nothing Then
    '        Me.RenderSelectionOutline(g, Me._zoomFactor, activeShape)
    '        Dim isCtrlDown As Boolean = (ModifierKeys And Keys.Control) = Keys.Control
    '        If Me._currentState = ViewportState.Resizing OrElse isCtrlDown Then
    '            Me._resizeMgr.DrawResizeHandles(g, Me._zoomFactor, activeShape)
    '        End If
    '        If Me._currentState = ViewportState.Rotating Then
    '            Me._rotationMgr.DrawRotationOverlayMetrics(g, Me._zoomFactor, activeShape)
    '        End If
    '    End If
    'End Sub

    Private Sub RenderSelectionOutline(ByVal g As Graphics, ByVal zoom As Single, ByVal shape As Cls_Base_Shape)
        If g Is Nothing OrElse shape Is Nothing Then Exit Sub
        Using selectPen As New Pen(Color.RoyalBlue, 2.5F / zoom) With {.DashStyle = DashStyle.Dash}
            Dim r As RectangleF = shape.Bounds
            g.DrawRectangle(selectPen, r.X, r.Y, r.Width, r.Height)
        End Using
    End Sub
    Private Sub DrawMarqueeOverlayWindow(ByVal g As Graphics)
        If g Is Nothing Then Exit Sub
        Dim x As Single = Math.Min(Me._selectionMgr.MarqueeStartPoint.X, Me._selectionMgr.MarqueeCurrentPoint.X)
        Dim y As Single = Math.Min(Me._selectionMgr.MarqueeStartPoint.Y, Me._selectionMgr.MarqueeCurrentPoint.Y)
        Dim w As Single = Math.Abs(Me._selectionMgr.MarqueeStartPoint.X - Me._selectionMgr.MarqueeCurrentPoint.X)
        Dim h As Single = Math.Abs(Me._selectionMgr.MarqueeStartPoint.Y - Me._selectionMgr.MarqueeCurrentPoint.Y)
        Using marqueeBrush As New SolidBrush(Color.FromArgb(15, Color.RoyalBlue))
            Using marqueePen As New Pen(Color.RoyalBlue, 1.0F / Me._zoomFactor) With {.DashStyle = DashStyle.Dash}
                g.FillRectangle(marqueeBrush, x, y, w, h)
                g.DrawRectangle(marqueePen, x, y, w, h)
            End Using
        End Using
    End Sub
#End Region
#Region "OLE Drag-and-Drop Landing Subsystem"
    Protected Overrides Sub OnDragEnter(ByVal drgevent As DragEventArgs)
        MyBase.OnDragEnter(drgevent)
        If drgevent.Data IsNot Nothing AndAlso drgevent.Data.GetDataPresent(DataFormats.StringFormat) Then
            drgevent.Effect = DragDropEffects.Copy
        Else
            drgevent.Effect = DragDropEffects.None
        End If
    End Sub
    Protected Overrides Sub OnDragDrop(ByVal drgevent As DragEventArgs)
        MyBase.OnDragDrop(drgevent)
        If drgevent.Data Is Nothing OrElse Not drgevent.Data.GetDataPresent(DataFormats.StringFormat) Then Exit Sub
        Dim componentToken As String = DirectCast(drgevent.Data.GetData(DataFormats.StringFormat), String)
        Dim clientPoint As Point = Me.PointToClient(New Point(drgevent.X, drgevent.Y))
        Dim worldPoint As PointF = Me._transformer.TransformScreenToWorld(clientPoint, Me._panOffset, Me._zoomFactor)
        Dim snappedInsertionPoint As PointF = Me._grid.SnapPoint(worldPoint)
        Me.InstantiateDroppedGateComponent(componentToken, snappedInsertionPoint)
    End Sub

    'Private Sub InstantiateDroppedGateComponent(ByVal tokenName As String, ByVal insertionPt As PointF)
    '    Dim newGate As Cls_Base_Shape = Nothing
    '    If String.Equals(tokenName, "ANDGATE", StringComparison.OrdinalIgnoreCase) Then
    '        newGate = New Cls_Gate_And(insertionPt)
    '    ElseIf String.Equals(tokenName, "NOTGATE", StringComparison.OrdinalIgnoreCase) Then
    '        newGate = New Cls_Gate_Not(insertionPt)
    '        'ElseIf String.Equals(tokenName, "ORGATE", StringComparison.OrdinalIgnoreCase) Then
    '        '    newGate = New Cls_Gate_Or(insertionPt)
    '    End If
    '    If newGate IsNot Nothing Then
    '        Me._canvasData.SchematicComponents.Add(newGate)
    '        Me._selectionMgr.SelectShape(newGate, False)
    '        Me.Invalidate()
    '    End If
    'End Sub
#End Region
#Region "Global Lifecycle Mutators"
    'Private Sub RequestStateTransition(ByVal nextState As ViewportState, ByVal worldAnchor As PointF)
    '    If nextState = ViewportState.Idle Then Me.Cursor = Cursors.Default
    '    Me._currentState = nextState
    '    Me._worldDragStart = worldAnchor
    '    Me.Invalidate()
    'End Sub
    ' Target File: Cls_Viewport.vb
    ' Project Namespace: mdi_test
    ' Architecture Standard: VB.NET | Option Strict On | Explicit Binding
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    Private Sub RequestStateTransition(ByVal nextState As ViewportState, ByVal worldAnchor As PointF)
        If nextState = ViewportState.Idle Then Me.Cursor = Cursors.Default
        Me._currentState = nextState
        Me._worldDragStart = worldAnchor

        ' FORCE HUD ALIGNMENT: Stream fresh operational metrics right as the state shifts
        Me.StreamTelemetry()

        Me.Invalidate()
    End Sub

    Friend Sub ForceDiagnosticsUpdate()
        Me.Invalidate()
    End Sub
    Public Sub FocusAndZoomIntoComponent(ByVal targetShape As Cls_Base_Shape)
        If targetShape Is Nothing Then Exit Sub
        Me._selectionMgr.Clear() : Me._selectionMgr.SelectShape(targetShape, False)
        Dim r As RectangleF = targetShape.Bounds
        Me._zoomFactor = Math.Max(0.2F, Math.Min(5.0F, (Convert.ToSingle(Me.Width) - 100.0F) / r.Width))
        Me._panOffset.X = Convert.ToSingle((Convert.ToDouble(Me.Width) - Convert.ToDouble(r.Width) * Convert.ToDouble(Me._zoomFactor)) / 2.0 - Convert.ToDouble(r.X) * Convert.ToDouble(Me._zoomFactor))
        Me._panOffset.Y = Convert.ToSingle((Convert.ToDouble(Me.Height) - Convert.ToDouble(r.Height) * Convert.ToDouble(Me._zoomFactor)) / 2.0 - Convert.ToDouble(r.Y) * Convert.ToDouble(Me._zoomFactor))
        Me.Focus() : Me.Invalidate()
    End Sub
    Public Sub UpdateViewportState(ByVal targetState As ViewportState)
        Me._currentState = targetState
        Me.Invalidate()
    End Sub
    Public Function GetActiveFocusShape() As Cls_Base_Shape
        Return Me._selectionMgr.TryGetActiveFocus()
    End Function
    Public Function ConvertScreenToWorld(ByVal screenPoint As Point) As PointF
        Return Me._transformer.TransformScreenToWorld(screenPoint, Me._panOffset, Me._zoomFactor)
    End Function
#End Region
End Class

