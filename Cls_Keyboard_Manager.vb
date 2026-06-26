' Target File: Cls_Keyboard_Manager.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Windows.Forms

Public NotInheritable Class Cls_Keyboard_Manager

    ''' <summary>
    ''' Processes non-overlapping keyboard inputs and delegates deletions type-safely.
    ''' FIXED: Explicitly unifies your key events without polluting view file scopes.
    ''' </summary>
    Public Sub HandleWorkspaceKeyDown(ByVal e As KeyEventArgs, ByVal viewport As Cls_Viewport)
        If e Is Nothing OrElse viewport Is Nothing Then Exit Sub

        ' Evaluate hotkey triggers securely via explicit key code match statements
        Select Case e.KeyCode
            Case Keys.Delete
                Me.ExecutePurgeTransaction(viewport)
        End Select
    End Sub

    Private Sub ExecutePurgeTransaction(ByVal viewport As Cls_Viewport)
        Dim selMgr As Cls_Selection_Manager = viewport.SelectionManager

        ' Verify selection arrays possess items before invoking mutational commands
        If selMgr IsNot Nothing AndAlso selMgr.Count > 0 Then
            viewport.CanvasData.PurgeShapesGroup(selMgr.SelectedShapes)
            selMgr.Clear()
            viewport.Invalidate()
        End If
    End Sub
End Class

