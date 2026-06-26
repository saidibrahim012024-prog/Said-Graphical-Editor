Partial Public Class Cls_MDI_Action

    Public Sub ExecuteNewDocumentTransaction()

        ' Delegate creation completely to the factory
        Dim spawnedSheet As Cls_Drawing = Me._projectManager.RegisterDocument(Me._parentBridge)

        '' Synchronize UI elements instantly
        'If Me._parentBridge.WorkspaceLayout?.MyExplorer IsNot Nothing Then
        '    Me._parentBridge.WorkspaceLayout.MyExplorer.RefreshHierarchyTree() 'Me._projectManager)
        'End If

    End Sub


End Class
