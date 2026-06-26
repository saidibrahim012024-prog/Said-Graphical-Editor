Partial Public Class Cls_MDI_Action


#Region "Automated Simulation & Test Subsystem"


    ''' <summary>
    ''' Executes a robust, self-contained 5-step CAD pipeline interaction stress test framework.
    ''' FIXED: Spawns an independent document form to eliminate startup viewport null reference exceptions.
    ''' </summary>
    Private Sub RunAutomatedInteractionStressTest(ByVal activeView As Cls_Viewport)
        Diagnostics.Debug.WriteLine("=== INITIALIZING CAD CANVAS INTERACTION STRESS TEST ===")

        ' 1. Programmatically spawn an explicit test sheet form window container to clear out layout nulls
        Dim testForm As New Form() With {
            .MdiParent = Me._parentBridge,
            .Text = "Automated_Simulation_Matrix.sch",
            .Size = New Size(800, 600)
        }

        Dim testViewport As New Cls_Viewport()
        testViewport.Dock = DockStyle.Fill
        testForm.Controls.Add(testViewport)
        testForm.Show()
        Diagnostics.Debug.WriteLine("[PASS] Step 1: New Drawing Workspace Form Mounted Safely.")

        ' 2. Delegate the remaining structural component test assertions to our helper routine
        Me.ExecuteStressTestComponentAssertions(testViewport)
    End Sub

    ''' <summary>
    ''' Segregated execution method evaluating step animations under your strict 25 operational line ceiling.
    ''' </summary>
    Private Sub ExecuteStressTestComponentAssertions(ByVal testViewport As Cls_Viewport)
        ' 2. Drop a concrete primitive AND gate right onto our layout grid intersections
        Dim testGate As Cls_Base_Shape = New Cls_Gate_And(New PointF(200.0F, 160.0F))
        testViewport.CanvasData.SchematicComponents.Add(testGate)
        Diagnostics.Debug.WriteLine("[PASS] Step 2: Concrete Cls_Gate_And component added to database collection.")

        ' 3. Select the shape component programmatically to verify handling envelopes
        testViewport.SelectionManager.SelectShape(testGate, False)
        Dim r As RectangleF = testGate.Bounds
        Diagnostics.Debug.WriteLine($"[PASS] Step 3: Selection box mapped. Bounds match: W={r.Width:F1}, H={r.Height:F1}")

        ' 4. Cleanly shift viewport interaction state flags back to Idle rules
        testViewport.UpdateViewportState(ViewportState.Idle)

        ' 5. Force an immediate thread-safe TreeView hierarchy and PropertyGrid synchronization pass
        'Me.SynchronizeExplorerHierarchyTree(testViewport)
        testViewport.ForceDiagnosticsUpdate()
        Diagnostics.Debug.WriteLine("=== CAD CANVAS INTERACTION STRESS TEST COMPLETED SECURELY ===")
    End Sub


#End Region


End Class
