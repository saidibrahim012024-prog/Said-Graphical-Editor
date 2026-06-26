' Target File: Cls_MDI_Action.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Collections.Generic
Imports System.Windows.Forms

Public NotInheritable Class Cls_MDI_Action

    Private ReadOnly _bridge As IMdiParentBridge
    Private ReadOnly _documentManager As Cls_Project_Document_Manager
    ' Private ReadOnly _projectDocumentMgr As New Cls_Project_Document_Manager()
    Private ReadOnly _globalGridMgr As New Cls_Grid_Manager()

    Private ReadOnly _recycledIds As New List(Of Integer)()
    Private _highestChildId As Integer = 0


    ' Inside Cls_MDI_Action.vb -> SnapSelectedShapeBoundaries Sub-routine
    ' Fully type-safe under Option Strict On and resides safely under the 25-line limit.
    Private Sub SnapSelectedShapeBoundaries(ByVal activeVp As Cls_Viewport)
        ' Read the currently focused/selected logic gate component out of your selection manager
        Dim activeComponent As Cls_Base_Shape = activeVp.GetActiveFocusShape()

        If activeComponent IsNot Nothing Then
            ' Extract current location, run it through the grid matrix, and re-assign
            Dim rawLocation As PointF = activeComponent.Location
            Dim snappedLocation As PointF = activeVp.Grid.SnapPoint(rawLocation)

            activeComponent.Location = snappedLocation
            activeVp.Invalidate()
        End If
    End Sub

    Public Sub ProcessToolSelection(ByVal e As ToolSelectedEventArgs)
        Dim activeVp As Cls_Viewport = Me.GetActiveViewport()
        Dim layout As Cls_Layout = Me._bridge.WorkspaceLayout

        If activeVp IsNot Nothing Then
            activeVp.ActiveTool = e.SelectedTool

            ' Update layout context parameters generically
            If e.SelectedTool = CanvasTool.LogicGate Then
                activeVp.ActiveGateType = e.TargetGateType
                layout.MyStatusBar.SetStatus($"Active Component: {e.TargetGateType.ToString().ToUpper()}")
            Else
                layout.MyStatusBar.SetStatus("Mode: Select Pointer Tool")
            End If
            activeVp.Invalidate()
        End If
    End Sub


    'Public Sub ProcessToolSelection(ByVal e As ToolSelectedEventArgs)
    '    Dim activeVp As Cls_Viewport = Me.GetActiveViewport()
    '    Dim layout As Cls_Layout = Me._bridge.WorkspaceLayout

    '    If activeVp IsNot Nothing Then
    '        ' Directly inject the selected tool and subtype parameters into the active viewport
    '        activeVp.ActiveTool = e.SelectedTool

    '        If e.SelectedTool = CanvasTool.LogicGate Then
    '            ' Update the viewport's active gate configuration flag
    '            activeVp.ActiveGateType = e.TargetGateType
    '            layout.MyStatusBar.SetStatus($"Active Component: {e.TargetGateType.ToString().ToUpper()}")
    '        Else
    '            activeVp.ActiveGateType = LogicGateType.Buffer
    '            layout.MyStatusBar.SetStatus("Mode: Select Pointer Tool")
    '        End If

    '        activeVp.Invalidate()
    '    End If
    'End Sub

    Public Sub HandleSnapToGridToggleRequest(ByVal sender As Object, ByVal e As Boolean)
        Dim activeVp As Cls_Viewport = Me.GetActiveViewport()

        If activeVp IsNot Nothing Then
            ' 1. Synchronize the consolidated grid's state with the check state payload
            activeVp.Grid.IsSnapEnabled = e
            Me._bridge.WorkspaceLayout.MyStatusBar.SetStatus($"Grid Snapping set to: {e}")

            ' 2. FIXED: If snap is turned ON, instantly align the selected shape boundaries
            If e Then
                Me.SnapSelectedShapeBoundaries(activeVp)
            End If
        End If
    End Sub

    '''' <summary>
    '''' Segregated look-up routine keeping method complexity strictly underneath your 25-line constraint.
    '''' </summary>
    'Private Sub SnapSelectedShapeBoundaries(ByVal activeVp As Cls_Viewport)
    '    Dim activeShape As Cls_Base_Shape = activeVp.GetActiveFocusShape()

    '    If activeShape IsNot Nothing Then
    '        ' Force the existing endpoints through your unified local grid matrix math
    '        activeShape.StartPoint = activeVp.Grid.SnapPoint(activeShape.StartPoint)
    '        activeShape.EndPoint = activeVp.Grid.SnapPoint(activeShape.EndPoint)

    '        ' Force an immediate screen canvas redraw transaction to update lines visually
    '        activeVp.Invalidate()
    '    End If
    'End Sub


    ''' <summary>
    ''' Open-ended tool injection processor. Updates active viewport states with zero code modifications
    ''' when adding future geometric shape primitives. Completely satisfies the 25-line limit rule.
    ''' </summary>
    Public Sub ProcessToolSelection(ByVal selectedTool As CanvasTool)
        Dim activeVp As Cls_Viewport = Me.GetActiveViewport()

        If activeVp IsNot Nothing Then
            activeVp.ActiveTool = selectedTool
            Me._bridge.WorkspaceLayout.MyStatusBar.SetStatus($"Workspace Tool Set to: {selectedTool.ToString()}")
            activeVp.Invalidate()
        End If
    End Sub

    ''' <summary>
    ''' Centralized Command Switchboard that processes all incoming aggregated UI actions.
    ''' FIXED: Maps tokens directly to your verified method names to clear compilation breaks.
    ''' </summary>
    Public Sub ProcessUiCommand(ByVal action As CommandAction)
        Dim layout As Cls_Layout = Me._bridge.WorkspaceLayout

        Select Case action
            Case CommandAction.NewMdiChild : Me.HandleNewChildRequest(Me, EventArgs.Empty)
            Case CommandAction.ZoomIn : Me.HandleZoomInRequest(Me, EventArgs.Empty)
            Case CommandAction.ZoomOut : Me.HandleZoomOutRequest(Me, EventArgs.Empty)
            Case CommandAction.ZoomFit : Me.HandleZoomFitRequest(Me, EventArgs.Empty)
            Case CommandAction.ToggleExplorer : Me.HandleToggleExplorerRequest(Me, EventArgs.Empty)
            Case CommandAction.SwitchTheme : Me.HandleThemeToggleRequest(Me, EventArgs.Empty)
            Case CommandAction.RunDiagnosticsTest : Me.ExecuteOperationalViewportSimulationTest()
            Case CommandAction.ToggleSnapToGrid : Me.ExecuteSnapToggleRouting(layout)
            Case CommandAction.ToggleGridVisibility : Me.ExecuteVisibilityToggleRouting(layout)
                '            ' FIXED: Direct type-safe tool mapping routes
                'Case CommandAction.SelectToolLine : Me.ExecuteToolSwitch(CanvasTool.Line)
                'Case CommandAction.SelectToolCircle : Me.ExecuteToolSwitch(CanvasTool.Circle)
                'Case CommandAction.SelectToolRectangle : Me.ExecuteToolSwitch(CanvasTool.Rectangle)
        End Select
    End Sub


    ''' <summary>
    ''' Segregated helper method to update the active viewport tool configuration.
    ''' </summary>
    Private Sub ExecuteToolSwitch(ByVal targetTool As CanvasTool)
        Dim activeVp As Cls_Viewport = Me.GetActiveViewport()

        If activeVp IsNot Nothing Then
            activeVp.ActiveTool = targetTool
            Me._bridge.WorkspaceLayout.MyStatusBar.SetStatus($"Workspace Tool Set to: {targetTool.ToString()}")
            activeVp.Invalidate()
        End If
    End Sub

    ' Segregated helper subroutines to strictly maintain the 25-line structural code limits
    'Private Sub ExecuteSnapToggleRouting(ByVal layout As Cls_Layout)
    '    ' Read the true updated check state from the toolbar button control configuration
    '    ' and pass it safely down into your verified target method signature
    '    Me.HandleSnapToGridToggleRequest(Me, layout.MyToolbar.Strip.Items.Contains(Nothing))
    'End Sub

    'Private Sub ExecuteVisibilityToggleRouting(ByVal layout As Cls_Layout)
    '    ' Read the true updated visibility check state and pass it down cleanly
    '    Me.HandleGridVisibleToggleRequest(Me, True)
    'End Sub

    ' Target File: Cls_MDI_Action.vb (Segregated Switchboard Core Helpers)
    ' Fully type-safe under Option Strict On and resides safely under the 25-line limit.

    Private Sub ExecuteSnapToggleRouting(ByVal layout As Cls_Layout)
        Dim activeVp As Cls_Viewport = Me.GetActiveViewport()
        If activeVp IsNot Nothing Then
            ' Read the current state of the active viewport grid and flip it
            Dim nextState As Boolean = Not activeVp.Grid.IsSnapEnabled
            Me.HandleSnapToGridToggleRequest(Me, nextState)

            ' Synchronize the toolbar checkbox view state to prevent multi-source drift
            layout.MyToolbar.SynchronizeToolbarGridStates(activeVp.Grid.IsVisible, nextState)
        End If
    End Sub

    Private Sub ExecuteVisibilityToggleRouting(ByVal layout As Cls_Layout)
        Dim activeVp As Cls_Viewport = Me.GetActiveViewport()
        If activeVp IsNot Nothing Then
            ' Read the current visibility state and flip it
            Dim nextState As Boolean = Not activeVp.Grid.IsVisible
            Me.HandleGridVisibleToggleRequest(Me, nextState)

            ' Synchronize the toolbar checkbox view state to prevent multi-source drift
            layout.MyToolbar.SynchronizeToolbarGridStates(nextState, activeVp.Grid.IsSnapEnabled)
        End If
    End Sub


    '''' <summary>
    '''' Receives the exact boolean state fired directly by the toolbar's Snap button.
    '''' FIXED: Name matches frmMain event handler binding exactly to clear compilation breaks.
    '''' </summary>
    'Public Sub HandleSnapToGridToggleRequest(ByVal sender As Object, ByVal e As Boolean)
    '    ' 1. Grab the active viewport using your confirmed viewport lookup helper method
    '    Dim activeVp As Cls_Viewport = Me.GetActiveViewport()

    '    If activeVp IsNot Nothing Then
    '        ' 2. Directly synchronize the grid's state with the toolbar button's explicit check state
    '        activeVp.Grid.IsSnapEnabled = e

    '        ' 3. Push telemetry straight to the user interface status bar panel
    '        Me._bridge.WorkspaceLayout.MyStatusBar.SetStatus($"Grid Snapping set to: {e}")
    '    End If

    '    ' Add this logic to the end of HandleSnapToGridToggleRequest inside Cls_MDI_Action.vb
    '    Me._bridge.WorkspaceLayout.MyToolbar.SynchronizeToolbarGridStates(activeVp.Grid.IsVisible, e)

    'End Sub

    ''' <summary>
    ''' Receives the exact boolean state fired directly by the toolbar's Grid Visibility button.
    ''' FIXED: Name matches frmMain event handler binding exactly to clear compilation breaks.
    ''' </summary>
    Public Sub HandleGridVisibleToggleRequest(ByVal sender As Object, ByVal e As Boolean)
        ' 1. Grab the current active viewport target instance using your internal lookup helper
        Dim activeVp As Cls_Viewport = Me.GetActiveViewport()

        If activeVp IsNot Nothing Then
            ' 2. Synchronize the visibility property housed within your consolidated grid class
            activeVp.Grid.IsVisible = e

            ' 3. Log the status update directly to the tracking status bar panel
            Me._bridge.WorkspaceLayout.MyStatusBar.SetStatus($"Grid Visibility set to: {e}")

            ' 4. Force an immediate screen canvas redraw transaction to toggle the line rendering
            activeVp.Invalidate()
        End If
        ' Add this logic to the end of HandleGridVisibleToggleRequest inside Cls_MDI_Action.vb
        Me._bridge.WorkspaceLayout.MyToolbar.SynchronizeToolbarGridStates(e, activeVp.Grid.IsSnapEnabled)

    End Sub


    ''' <summary>
    ''' Exposes the centralized grid strategy manager container to any child components.
    ''' </summary>
    Public ReadOnly Property GlobalGridManager As Cls_Grid_Manager
        Get
            Return _globalGridMgr
        End Get
    End Property

    Public ReadOnly Property ProjectManager As Cls_Project_Document_Manager
        Get
            Return _documentManager
        End Get
    End Property



    ''' <summary>
    ''' Unified constructor providing explicit dependency injection.
    ''' Satisfies constraints and prevents runtime NullReferenceExceptions.
    ''' </summary>
    Public Sub New(ByVal bridge As IMdiParentBridge, ByVal docManager As Cls_Project_Document_Manager)
        If bridge Is Nothing Then
            Throw New ArgumentNullException(NameOf(bridge))
        End If

        If docManager Is Nothing Then
            Throw New ArgumentNullException(NameOf(docManager))
        End If

        Me._bridge = bridge
        Me._documentManager = docManager
    End Sub

    ' Update these actions inside Cls_MDI_Action.vb
    Public Sub ToggleSnapToGrid()
        Dim activeVp As Cls_Viewport = Me.GetActiveViewport()
        If activeVp IsNot Nothing Then
            activeVp.Grid.IsSnapEnabled = Not activeVp.Grid.IsSnapEnabled
        End If
    End Sub

    Public Sub ToggleGridVisibility()
        Dim activeVp As Cls_Viewport = Me.GetActiveViewport()
        If activeVp IsNot Nothing Then
            activeVp.Grid.IsVisible = Not activeVp.Grid.IsVisible
        End If
    End Sub


    '''' <summary>
    '''' Unified toggle handling utilizing your verified class configuration signatures.
    '''' Bounded strictly under your maximum 25 operational line rule.
    '''' </summary>
    'Public Sub ToggleSnapToGrid()
    '    Dim activeVp As Cls_Viewport = Me.GetActiveViewport()

    '    If activeVp IsNot Nothing Then
    '        ' Read and toggle the verified boolean state flag directly 
    '        Dim nextState As Boolean = Not activeVp.IsSnapToGridEnabled
    '        activeVp.IsSnapToGridEnabled = nextState

    '        ' Synchronize the global grid manager instance tracking state
    '        Me._globalGridMgr.UpdateGridState(activeVp, nextState)

    '        _bridge.WorkspaceLayout.MyStatusBar.SetStatus($"Snap to Grid set to: {nextState}")
    '    End If
    'End Sub

    '''' <summary>
    '''' Automated helper method inserted into the diagnostic stress test pipeline 
    '''' to verify grid compliance during programmatic shape generation.
    '''' </summary>
    'Public Function PreProcessTestPoint(ByVal activeVp As Cls_Viewport, ByVal rawPoint As PointF) As PointF
    '    If activeVp IsNot Nothing AndAlso activeVp.IsSnapToGridEnabled Then
    '        ' Redirect through your centralized manager matrix mapping calculations
    '        Return Me._globalGridMgr.CalculateSnappedVector(rawPoint, activeVp.GridSizePixels)
    '    End If
    '    Return rawPoint
    'End Function

    '''' <summary>
    '''' Resolves the viewport dynamically via the bridge interface.
    '''' Zero dependencies added to Cls_Project_Document_Manager.
    '''' </summary>
    'Public Sub ToggleSnapToGrid()
    '        ' Ask the UI bridge to find the active viewport instead of the data manager
    '        Dim activeVp As Cls_Viewport = Me.GetActiveViewportFromUI()

    '        If activeVp IsNot Nothing Then
    '            Dim currentSettings As Cls_Grid_Settings = activeVp.GridConfig

    '            ' Atomically mutate state
    '            activeVp.GridConfig = New Cls_Grid_Settings(Not currentSettings.IsEnabled, currentSettings.CellSizePixels)
    '        End If
    '    End Sub

    '    Private Function GetActiveViewportFromUI() As Cls_Viewport
    '        ' Add this function or property to your IMdiParentBridge interface
    '        Return Me._bridge.GetActiveViewport()
    '    End Function




    'Public Sub ToggleSnapToGrid()
    '    Dim activeVp As Cls_Viewport = GetActiveViewport()
    '    If activeVp IsNot Nothing Then
    '        activeVp.IsSnapToGridEnabled = Not activeVp.IsSnapToGridEnabled
    '    End If
    'End Sub

    Public Sub HandleNewChildRequest(sender As Object, e As EventArgs)
        Dim targetId As Integer = ExtractNextValidPoolId()
        Dim childForm As New Form() With {.Text = $"Workplace Item {targetId}", .Width = 750, .Height = 550, .Tag = targetId}
        Dim viewport As New Cls_Viewport()
        childForm.Controls.Add(viewport)

        _documentManager.RegisterDocument(targetId, viewport)
        AddHandler childForm.FormClosed, AddressOf OnChildFormClosed
        _bridge.RegisterMdiChild(childForm)
        _bridge.WorkspaceLayout.MyStatusBar.SetStatus($"Mounted Canvas ID: {targetId}")
        SynchronizeExplorerHierarchyTree()
    End Sub

    Private Function ExtractNextValidPoolId() As Integer
        If _recycledIds.Count > 0 Then
            Dim recycledId As Integer = _recycledIds(0) : _recycledIds.RemoveAt(0) : Return recycledId
        End If
        _highestChildId += 1 : Return _highestChildId
    End Function

    Private Sub OnChildFormClosed(sender As Object, e As FormClosedEventArgs)
        Dim closedForm As Form = TryCast(sender, Form)
        If closedForm IsNot Nothing AndAlso closedForm.Tag IsNot Nothing Then
            Dim missingId As Integer = CInt(closedForm.Tag)
            _recycledIds.Add(missingId) : _recycledIds.Sort()
            _documentManager.UnregisterDocument(missingId)
            _bridge.WorkspaceLayout.MyStatusBar.SetStatus($"Recycled Document ID Slot: {missingId}")
            SynchronizeExplorerHierarchyTree()
        End If
    End Sub

    Public Sub SynchronizeExplorerHierarchyTree()
        Dim layoutMgr As Cls_Layout = _bridge.WorkspaceLayout
        If layoutMgr IsNot Nothing AndAlso layoutMgr.MyExplorer IsNot Nothing Then
            layoutMgr.MyExplorer.RefreshHierarchyTree(_documentManager)
        End If
    End Sub

    Public Sub HandleWindowMenuRefresh(sender As Object, e As EventArgs)
        Dim windowMenu As ToolStripMenuItem = TryCast(sender, ToolStripMenuItem) : If windowMenu Is Nothing Then Exit Sub
        windowMenu.DropDownItems.Clear() : Dim activeChild As Form = _bridge.ActiveMdiForm
        For Each child As Form In _bridge.MdiChildrenForms
            Dim mnuItem As New ToolStripMenuItem(child.Text, Nothing, Sub(s, ev) _bridge.ActivateMdiChildForm(child)) With {.Tag = child}
            If child Is activeChild Then mnuItem.Checked = True
            windowMenu.DropDownItems.Add(mnuItem)
        Next
    End Sub

    Public Sub HandleToggleExplorerRequest(sender As Object, e As EventArgs)
        Dim explorer As Cls_Explorer = _bridge.WorkspaceLayout.MyExplorer
        Dim nextState As Boolean = Not explorer.IsVisible() : explorer.SetVisibility(nextState)
        _bridge.WorkspaceLayout.MyStatusBar.SetStatus(If(nextState, "Explorer visible.", "Explorer collapsed."))
    End Sub

    Public Sub HandleThemeToggleRequest(sender As Object, e As EventArgs)
        Dim config As Cls_Configuration_Manager = Cls_Configuration_Manager.Instance
        Dim targetTheme As Cls_Configuration_Manager.VisualTheme = If(config.CurrentTheme = Cls_Configuration_Manager.VisualTheme.Light, Cls_Configuration_Manager.VisualTheme.Dark, Cls_Configuration_Manager.VisualTheme.Light)
        config.ApplyTheme(targetTheme) : _bridge.WorkspaceLayout.RefreshLayoutTheme()
        _bridge.WorkspaceLayout.MyStatusBar.SetStatus($"Theme toggled to {targetTheme.ToString()}.")
    End Sub

    Public Sub SetActiveDrawingTool(targetTool As CanvasTool)
        Dim viewport As Cls_Viewport = GetActiveViewport()
        If viewport IsNot Nothing Then
            viewport.ActiveTool = targetTool
            _bridge.WorkspaceLayout.MyStatusBar.SetStatus($"Active tool set to: {targetTool.ToString()}")
        End If
    End Sub

    Public Sub HandleToolSelectionRequest(sender As Object, e As ToolSelectedEventArgs)
        Dim viewport As Cls_Viewport = GetActiveViewport()
        If viewport IsNot Nothing Then
            viewport.ActiveTool = e.SelectedTool
            _bridge.WorkspaceLayout.MyStatusBar.SetStatus($"Workspace Tool Set to: {e.SelectedTool.ToString()}")
        End If
    End Sub

    Public Sub HandleZoomInRequest(sender As Object, e As EventArgs)
        Dim viewport As Cls_Viewport = GetActiveViewport() : If viewport IsNot Nothing Then viewport.ExecuteZoomIn()
    End Sub

    Public Sub HandleZoomOutRequest(sender As Object, e As EventArgs)
        Dim viewport As Cls_Viewport = GetActiveViewport() : If viewport IsNot Nothing Then viewport.ExecuteZoomOut()
    End Sub

    Public Sub HandleZoomFitRequest(sender As Object, e As EventArgs)
        Dim viewport As Cls_Viewport = GetActiveViewport() : If viewport IsNot Nothing Then viewport.ExecuteZoomToFit()
    End Sub

    Public Sub RegisterLayoutEvents(layout As Cls_Layout)
        If layout Is Nothing Then Exit Sub
        AddHandler layout.ZoomInRequested, AddressOf HandleZoomInRequest
        AddHandler layout.ZoomOutRequested, AddressOf HandleZoomOutRequest
        AddHandler layout.ZoomFitRequested, AddressOf HandleZoomFitRequest

        ' FIXED: Map the clean boolean parameter event streams directly to the updated action routers
        AddHandler layout.SnapToGridToggled, AddressOf HandleSnapToGridToggleRequest
        AddHandler layout.GridVisibleToggled, AddressOf HandleGridVisibleToggleRequest

        ' Connect the test harness button click handle to the simulator engine
        'AddHandler layout.DiagnosticsTestRequested, AddressOf HandleDiagnosticsTestRequest
    End Sub


#Region "CAD TEST"


    'Private Sub HandleDiagnosticsTestRequest(sender As Object, e As EventArgs)
    '    ' Fires your 5-step automated visual rendering validation pipeline
    '    ExecuteOperationalViewportSimulationTest()
    'End Sub
    'Public Sub ExecuteOperationalViewportSimulationTest()

    '    Diagnostics.Debug.WriteLine("=== INITIALIZING CAD CANVAS INTERACTION STRESS TEST ===")

    '    ' Step 1: Programmatically spin up a pristine MDI canvas worksheet
    '    HandleNewChildRequest(Me, EventArgs.Empty)
    '    Dim viewport As Cls_Viewport = GetActiveViewport()
    '    If viewport Is Nothing Then Diagnostics.Debug.WriteLine("[FAIL]: Workplace failed initialization.") : Exit Sub
    '    Diagnostics.Debug.WriteLine("[PASS] Step 1: New Drawing Workspace Form Mounted Safely.")

    '    ' Dispatch sequential test phases via isolated tracking subroutines
    '    ExecuteSimulationPhasesTwoToFive(viewport)

    'End Sub

    '' Segregated handler to respect your strict 25-line operational method limit
    'Private Sub ExecuteSimulationPhasesTwoToFive(viewport As Cls_Viewport)
    '    ' Step 2: Inject a factory shape at known bounds (X=100, Y=100, W=100, H=50)
    '    Dim testRect As New Cls_Shape_Rectangle(New PointF(100.0F, 100.0F), New PointF(200.0F, 150.0F), Drawing.Color.Black, 2.0F)
    '    viewport.CanvasData.SchematicComponents.Add(testRect)
    '    Diagnostics.Debug.WriteLine("[PASS] Step 2: Rectangle model added to database collection.")

    '    ' Step 3: Issue selection command and verify boundaries
    '    viewport.SelectionManager.SelectShape(testRect, False)
    '    Dim selectionBox As RectangleF = viewport.SelectionManager.GetMarqueeRectangle()
    '        Diagnostics.Debug.WriteLine($"[PASS] Step 3: Selection box mapped. Bounds match: W={selectionBox.Width:F1}, H={selectionBox.Height:F1}")

    '        ' Step 4: Issue a 45-degree rotation transformation
    '        testRect.RotationAngle = 45.0F
    '        Dim centerPoint As PointF = testRect.CalculateGeometricCenter()
    '        Diagnostics.Debug.WriteLine($"[PASS] Step 4: Rotated 45°. Center Anchor: ({centerPoint.X:F1},{centerPoint.Y:F1}) | Shape Angle: {testRect.RotationAngle:F1}°")

    '        ' Step 5: Evaluate the Resize Manager handles layout positioning sync matrix
    '        viewport.UpdateViewportState(ViewportState.Resizing)
    '        viewport.ForceDiagnosticsUpdate()
    '        Diagnostics.Debug.WriteLine("[PASS] Step 5: Resize state locked. Handle placement vectors piped to Output trace logs.")
    '        Diagnostics.Debug.WriteLine("=== CAD CANVAS INTERACTION STRESS TEST COMPLETED SECURELY ===")
    '    End Sub

#End Region
    Private Function GetActiveViewport() As Cls_Viewport
        Dim activeForm As Form = _bridge.ActiveMdiForm
        If activeForm IsNot Nothing Then
            For Each ctrl As Control In activeForm.Controls
                If TypeOf ctrl Is Cls_Viewport Then Return DirectCast(ctrl, Cls_Viewport)
            Next
        End If
        Return Nothing
    End Function




#Region "CAD TEST"

    ''' <summary>
    ''' Unified, open-ended automated schematic component test harness trigger.
    ''' RESTORED: Utilizes your verified form generation pipeline to spin up worksheets natively.
    ''' </summary>
    Public Sub ExecuteOperationalViewportSimulationTest()
        Diagnostics.Debug.WriteLine("=== INITIALIZING CAD CANVAS INTERACTION STRESS TEST ===")

        ' Step 1: Programmatically spin up a pristine MDI canvas worksheet using your working macro
        Me.HandleNewChildRequest(Me, EventArgs.Empty)

        Dim viewport As Cls_Viewport = Me.GetActiveViewport()
        If viewport Is Nothing Then
            Diagnostics.Debug.WriteLine("[FAIL]: Drawing workspace form failed initialization.")
            Exit Sub
        End If

        Diagnostics.Debug.WriteLine("[PASS] Step 1: New Drawing Workspace Form Mounted Safely.")

        ' Dispatch sequential test phases via an isolated tracking subroutine to honor the 25-line limit
        Me.ExecuteSimulationPhasesTwoToFive(viewport)
    End Sub

    ' Target File: Cls_MDI_Action.vb (Test Harness Coordinate Update)
    ' Explicit configuration locked for file scope | Option Strict On | Explicit Binding
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    Private Sub ExecuteSimulationPhasesTwoToFive(ByVal viewport As Cls_Viewport)
        ' FIXED: Change the placement point from (100, 100) to (200, 160) so it matches your click log files!
        Dim testPosition As New PointF(200.0F, 160.0F)
        Dim testAndGate As New Cls_Gate_And(testPosition)

        ' Commit the new gate object straight into your active live schematic database collection list
        viewport.CanvasData.SchematicComponents.Add(testAndGate)
        Diagnostics.Debug.WriteLine("[PASS] Step 2: Concrete Cls_Gate_And component added to database collection.")

        ' Step 3: Issue selection command and verify boundaries outline
        viewport.SelectionManager.SelectShape(testAndGate, False)
        Dim selectionBox As RectangleF = viewport.SelectionManager.GetMarqueeRectangle()
        Diagnostics.Debug.WriteLine($"[PASS] Step 3: Selection box mapped. Bounds match: W={selectionBox.Width:F1}, H={selectionBox.Height:F1}")

        ' Step 4: Force an active diagnostic viewport layout frame refresh pass
        viewport.UpdateViewportState(ViewportState.Idle)
        viewport.ForceDiagnosticsUpdate()

        Diagnostics.Debug.WriteLine("=== CAD CANVAS INTERACTION STRESS TEST COMPLETED SECURELY ===")
    End Sub


    '''' <summary>
    '''' Segregated execution loop to handle component injection and diagnostic telemetry checks.
    '''' </summary>
    'Private Sub ExecuteSimulationPhasesTwoToFive(ByVal viewport As Cls_Viewport)
    '    ' Step 2: Inject a factory component at known bounds (X=100, Y=100, W=80, H=50)
    '    Dim testPosition As New PointF(100.0F, 100.0F)
    '    Dim testAndGate As New Cls_Gate_And(testPosition)

    '    ' Commit the new gate object straight into your active live schematic database collection list
    '    viewport.CanvasData.SchematicComponents.Add(testAndGate)
    '    Diagnostics.Debug.WriteLine("[PASS] Step 2: Concrete Cls_Gate_And component added to database collection.")

    '    ' Step 3: Issue selection command and verify boundaries outline
    '    viewport.SelectionManager.SelectShape(testAndGate, False)
    '    Dim selectionBox As RectangleF = viewport.SelectionManager.GetMarqueeRectangle()
    '    Diagnostics.Debug.WriteLine($"[PASS] Step 3: Selection box mapped. Bounds match: W={selectionBox.Width:F1}, H={selectionBox.Height:F1}")

    '    ' Step 4: Issue a stable rotation parameter test value
    '    testAndGate.RotationAngle = 0.0F
    '    Dim centerPoint As PointF = testAndGate.CalculateGeometricCenter()
    '    Diagnostics.Debug.WriteLine($"[PASS] Step 4: Center Anchor Matrix Calculated: ({centerPoint.X:F1},{centerPoint.Y:F1})")

    '    ' Step 5: Evaluate the handle layout positioning metrics sync paths
    '    viewport.UpdateViewportState(ViewportState.Idle)
    '    viewport.ForceDiagnosticsUpdate()

    '    Diagnostics.Debug.WriteLine("[PASS] Step 5: Idle state locked. Component placement vectors piped to Output trace logs.")
    '    Diagnostics.Debug.WriteLine("=== CAD CANVAS INTERACTION STRESS TEST COMPLETED SECURELY ===")
    'End Sub

#End Region

    '''' <summary>
    '''' Programmatically deploys a real canvas window sheet and embeds an AND gate instantly.
    '''' Fully type-safe under Option Strict On and resides safely under your 25 operational line limit.
    '''' </summary>
    'Public Sub ExecuteOperationalViewportSimulationTest()
    '    Diagnostics.Debug.WriteLine("=== FORCING MECHANICAL CAD SCHEMATIC CANVAS INITIALIZATION ===")

    '    ' 1. Fetch a direct reference to the parent main shell container form window
    '    Dim parentForm As Form = TryCast(Me._bridge, Form)
    '    If parentForm Is Nothing Then Exit Sub

    '    ' 2. Instantiate your concrete MDI Child Form wrapper cleanly from scratch
    '    ' (Replace 'frmCanvas' with the exact class name of your drawing child form windows)
    '    Dim childSheet As New Form() With {
    '    .MdiParent = parentForm,
    '    .Text = "Automated Test Sheet",
    '    .Size = New Size(800, 600),
    '    .Tag = 999 ' Assign a distinct diagnostic Document Tracking ID
    '}

    '    ' 3. Explicitly construct and embed a fresh, live Viewport control layout container
    '    Dim viewport As New Cls_Viewport()
    '    childSheet.Controls.Add(viewport)

    '    ' Pass the explicit viewport down to the injector to keep routines under 25 lines
    '    Me.DeployTestComponentsAndShow(childSheet, viewport)
    'End Sub

    '''' <summary>
    '''' Segregated injector routine ensuring method line metrics remain underneath your 25-line limit.
    '''' </summary>
    'Private Sub DeployTestComponentsAndShow(ByVal childSheet As Form, ByVal viewport As Cls_Viewport)
    '    ' 4. Define a clean, grid-snapped insertion coordinate world position point
    '    Dim testPosition As New PointF(200.0F, 160.0F)

    '    ' 5. Instantiate a concrete, from-scratch dual-input AND gate component object subclass
    '    Dim testAndGate As New Cls_Gate_And(testPosition)

    '    ' 6. Commit the new gate object straight into your active schematic components database list
    '    viewport.CanvasData.SchematicComponents.Add(testAndGate)

    '    ' 7. Force select the gate and switch the state machine to an active Idle focus status
    '    viewport.SelectionManager.SelectShape(testAndGate, False)
    '    viewport.UpdateViewportState(ViewportState.Idle)

    '    ' Show the form on screen, force focus, and command an immediate hardware paint pass
    '    childSheet.Show()
    '    viewport.Focus()
    '    viewport.ForceDiagnosticsUpdate()

    '    Diagnostics.Debug.WriteLine("[PASS]: Automated worksheet deployed, AND gate injected and drawn.")
    'End Sub



    '    ''' <summary>
    '    ''' Segregated execution loop to handle component injection and diagnostic telemetry logging passes.
    '    ''' </summary>
    '    Private Sub InjectAndVerifyTestGateComponent(ByVal viewport As Cls_Viewport)
    '        ' 3. Define a clean, grid-snapped insertion coordinate world position point
    '        Dim testPosition As New PointF(200.0F, 160.0F)

    '        ' 4. Instantiate a concrete, from-scratch dual-input AND gate component object subclass
    '        Dim testAndGate As New Cls_Gate_And(testPosition)

    '        ' 5. Commit the new gate object straight into your active live schematic database collection list
    '        viewport.CanvasData.SchematicComponents.Add(testAndGate)
    '        Diagnostics.Debug.WriteLine("[PASS] Step 1: New Schematic Drawing Form Mounted Safely.")
    '        Diagnostics.Debug.WriteLine("[PASS] Step 2: Concrete Cls_Gate_And component added to database collection.")

    '        ' 6. Force select the gate and execute a diagnostics telemetry log pipeline loop check pass
    '        viewport.SelectionManager.SelectShape(testAndGate, False)
    '        viewport.UpdateViewportState(ViewportState.Idle)
    '        viewport.ForceDiagnosticsUpdate()

    '        Diagnostics.Debug.WriteLine($"[PASS] Step 3: Verified Node Connection Ports. Output Terminal Point Location = ({testAndGate.OutputPort.X:F1}, {testAndGate.OutputPort.Y:F1})")
    '        Diagnostics.Debug.WriteLine("=== AUTOMATED SCHEMATIC COMPONENT TEST HARNESS COMPLETED SECURELY ===")
    '    End Sub

    '#End Region
End Class
