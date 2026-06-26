Partial Public Class Cls_MDI_Action


    ''' <summary>
    ''' Catches form focus shifts and automatically updates the central status bar and explorer panel.
    ''' </summary>
    Private Sub OnMdiChildFormActivated(ByVal sender As Object, ByVal e As EventArgs)
        Dim activeView As Cls_Viewport = Me.GetActiveMdiViewport()

        If activeView IsNot Nothing Then
            ' Update both the hierarchical tree layout and the property inspector grid
            'Me.SynchronizeExplorerHierarchyTree(activeView)
            'Me.UpdateStatusStripTelemetry(New PointF(0.0F, 0.0F), activeView.SelectionManager.Count)
        End If
    End Sub





End Class
