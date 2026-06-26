Partial Public Class Cls_MDI_Action

    Public Sub ProcessDocumentClosureEviction(ByVal documentId As Integer)
        ' 1. Safely unregister the dead document ID from the project manager dictionary
        Me._projectManager.UnregisterDocument(documentId)

        ' 2. Re-evaluate the live remaining viewport screen to refresh the layout metrics
        Dim activeView As Cls_Viewport = Me.GetActiveMdiViewport()
        If activeView IsNot Nothing Then
            activeView.Invalidate()
        Else
            ' EDGE CASE: If no drawing tabs remain open, force the custom HUD panels back to default
            Dim statusBar As Cls_StatusBar = Me._parentBridge.WorkspaceLayout?.MyStatusBar
            statusBar?.UpdateStatusMetrics("State: Idle | Scale: 1.0 | Active Sheets: 0")
        End If
    End Sub
    'Public Sub ProcessDocumentClosureEviction(ByVal documentId As Integer)
    '    Me._projectManager.UnregisterDocument(documentId)

    '    Dim activeView As Cls_Viewport = Me.GetActiveMdiViewport()
    '    If activeView IsNot Nothing Then
    '        activeView.Invalidate()
    '    Else
    '        Dim statusBar As Cls_StatusBar = Me._parentBridge.WorkspaceLayout?.MyStatusBar
    '        statusBar?.UpdateStatusMetrics("State: Idle | Scale: 1.0 | Active Sheets: 0")
    '    End If
    'End Sub

    Private Sub ExecuteCloseActiveDocumentFrame()
        If Me._parentBridge.ActiveMdiForm IsNot Nothing Then
            Me._parentBridge.ActiveMdiForm.Close()
            Me.FlushPropertyGridInspector(Nothing)
        End If
    End Sub
End Class
