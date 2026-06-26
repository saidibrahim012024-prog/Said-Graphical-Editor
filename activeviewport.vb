Partial Public Class Cls_MDI_Action
    'Public Function GetActiveMdiViewport() As Cls_Viewport

    '    Dim activeChild As Form = Me._parentBridge.ActiveMdiForm

    '    If activeChild IsNot Nothing AndAlso TypeOf activeChild Is Cls_Drawing Then
    '        Dim drawingForm As Cls_Drawing = DirectCast(activeChild, Cls_Drawing)
    '        Dim liveViewport As Cls_Viewport = drawingForm.Viewport

    '        ' AUTHENTIC REFERENCE HEALER: Re-bind the active dictionary slot to the live screen instance
    '        If Me._projectManager.DocumentRegistry.ContainsKey(drawingForm.DocumentId) Then
    '            Me._projectManager.DocumentRegistry(drawingForm.DocumentId) = liveViewport
    '        End If

    '        Return liveViewport
    '    End If
    '    Return Nothing
    'End Function



    ''' <summary>
    ''' Private helper method extracting nested viewport control references safely.
    ''' </summary>
    Private Function TryDiscoverViewportInsideForm(ByVal childForm As Form) As Cls_Viewport
        If childForm IsNot Nothing Then
            For Each ctrl As Control In childForm.Controls
                If TypeOf ctrl Is Cls_Viewport Then Return DirectCast(ctrl, Cls_Viewport)
            Next
        End If
        Return Nothing
    End Function

    ' Target File: Cls_MDI_Action.vb
    ' Project Namespace: mdi_test
    ' Architecture Standard: VB.NET | Option Strict On | Explicit Binding
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    Public Function GetActiveMdiViewport() As Cls_Viewport
        ' 1. Pull the active child window form container safely via the interface bridge
        Dim activeChild As System.Windows.Forms.Form = Me._parentBridge.ActiveMdiForm

        ' 2. TYPE-SAFE CASTING: If it matches your layout data form, extract its stateless viewport
        If activeChild IsNot Nothing AndAlso TypeOf activeChild Is Cls_Drawing Then
            Return DirectCast(activeChild, Cls_Drawing).Viewport
        End If
        Return Nothing
    End Function



    ' Target File: Cls_MDI_Action.vb
    ' Architecture Standard: VB.NET | Option Strict On | Explicit Binding
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    Public Function GetActiveDrawingSheet() As Cls_Drawing
        ' Query the interface contract bridge to get the active window frame instance
        Dim activeChild As System.Windows.Forms.Form = Me._parentBridge.ActiveMdiForm

        If activeChild IsNot Nothing AndAlso TypeOf activeChild Is Cls_Drawing Then
            ' Natively extract the strongly-typed document sheet container safely
            Return DirectCast(activeChild, Cls_Drawing)
        End If
        Return Nothing
    End Function

    'Public Sub ProcessUiCommand(ByVal action As CommandAction)
    '    Dim activeSheet As Cls_Drawing = Me.GetActiveDrawingSheet()

    '    Select Case action
    '        Case CommandAction.NewDocument
    '            Me.ExecuteNewDocumentTransaction() : Me.RefreshExplorerTreeUI() : Return
    '        Case CommandAction.DeleteSelection
    '            ' Pass the true live database container directly into your purge transaction
    '            If activeSheet IsNot Nothing Then Me.ExecuteSelectionPurgeTransaction(activeSheet)
    '    End Select

    '    Me.RefreshExplorerTreeUI()
    'End Sub

End Class
