' Target File: Cls_Keyboard_Manager.vb
' Project Namespace: New_Said_Graphics_Library
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Windows.Forms

Public NotInheritable Class Cls_Keyboard_Manager

    ' Target File: Cls_Keyboard_Manager.vb
    ' Refactored helper method meeting your 25-line operational limit
    Private Sub ExecuteAltRotationTrigger()
        If _viewport.ActiveTool = CanvasTool.SelectPointer Then
            Dim focusShape As Cls_Base_Shape = _viewport.GetActiveFocusShape()

            '' FIXED: Direct the viewport to formally instantiate the tracking metrics
            'If focusShape IsNot Nothing AndAlso _viewport.CurrentState = ViewportState.Idle Then
            '    _viewport.InitiateActiveRotation(focusShape)
            'End If
        End If
    End Sub

    Private ReadOnly _viewport As Cls_Viewport

    Public Sub New(parentViewport As Cls_Viewport)
        If parentViewport Is Nothing Then Throw New ArgumentNullException(NameOf(parentViewport))
        _viewport = parentViewport
    End Sub

    Public Sub ProcessKeyDownTransaction(e As KeyEventArgs)
        If e Is Nothing OrElse _viewport.CurrentState <> ViewportState.Idle Then Exit Sub

        Select Case e.KeyCode
            Case Keys.Delete
                ExecuteDeleteShortcut() : e.Handled = True
                'Case Keys.Menu ' Aligned ALT key press event routing block
                '    ExecuteAltRotationTrigger() : e.Handled = True
        End Select
    End Sub

    Public Sub ProcessKeyUpTransaction(e As KeyEventArgs)
        If e Is Nothing Then Exit Sub
        Select Case e.KeyCode
            Case Keys.Menu ' Aligned ALT key release event routing block
                ExecuteAltRotationRelease() : e.Handled = True
        End Select
    End Sub

    Private Sub ExecuteDeleteShortcut()
        Dim selectionMgr As Cls_Selection_Manager = _viewport.SelectionManager
        If selectionMgr.Count > 0 Then
            _viewport.CanvasData.PurgeShapesGroup(selectionMgr.SelectedShapes)
            selectionMgr.Clear() : _viewport.ForceDiagnosticsUpdate()
        End If
    End Sub

    'Private Sub ExecuteAltRotationTrigger()
    '    If _viewport.ActiveTool = CanvasTool.SelectPointer Then
    '        Dim focusShape As Cls_Base_Shape = _viewport.GetActiveFocusShape()
    '        If focusShape IsNot Nothing Then
    '            _viewport.UpdateViewportState(ViewportState.Rotating)
    '            _viewport.Cursor = Cursors.SizeAll : _viewport.Invalidate()
    '        End If
    '    End If
    'End Sub

    Private Sub ExecuteAltRotationRelease()
        If _viewport.CurrentState = ViewportState.Rotating Then
            _viewport.UpdateViewportState(ViewportState.Idle)
            _viewport.Cursor = Cursors.Default : _viewport.Invalidate()
        End If
    End Sub
End Class
