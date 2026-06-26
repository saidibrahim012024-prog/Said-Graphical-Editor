' Target File: Cls_MDI_Action.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms


Public NotInheritable Class Cls_MDI_Action

    ' Target File: Cls_MDI_Action.vb
    ' Project Namespace: mdi_test
    ' Architecture Standard: VB.NET | Option Strict On | Explicit Binding
    ' Said Rules Compliance: Zero Guessing | No Non-Existent Token Variables | Locked Classes Only
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    Public Sub ExecuteMdiLayoutLayoutTransform(ByVal arrangement As CommandAction)
        ' 1. CASTING: Safely pull the master shell form container via the parent bridge interface
        Dim parentForm As System.Windows.Forms.Form = TryCast(Me._parentBridge, System.Windows.Forms.Form)

        If parentForm IsNot Nothing Then
            ' 2. DISPATCH: Execute the authoritative layout transformation natively on the UI thread
            Select Case arrangement
                Case CommandAction.LayoutCascade
                    parentForm.LayoutMdi(MdiLayout.Cascade)
                Case CommandAction.LayoutTileHorizontal
                    parentForm.LayoutMdi(MdiLayout.TileHorizontal)
                Case CommandAction.LayoutTileVertical
                    parentForm.LayoutMdi(MdiLayout.TileVertical)
            End Select
        End If
    End Sub


    Private ReadOnly _parentBridge As IMdiParentBridge
    Private ReadOnly _projectManager As Cls_Project_Document_Manager

    Public Sub New(ByVal bridge As IMdiParentBridge, ByVal manager As Cls_Project_Document_Manager)
        Me._parentBridge = bridge
        Me._projectManager = manager
    End Sub

    Public ReadOnly Property ProjectManager() As Cls_Project_Document_Manager
        Get
            Return Me._projectManager
        End Get
    End Property


#Region "Authoritative UI Command Switchboard"

    ''' <summary>
    ''' The master execution entry point routing all menu, toolbar, and status clicks.
    ''' </summary>
    Public Sub ProcessUiCommand(ByVal action As CommandAction)
        Dim activeView As Cls_Viewport = Me.GetActiveMdiViewport()

        ' Document management options can run even with zero worksheets active
        Select Case action
            Case CommandAction.NewDocument
                Me.ExecuteNewDocumentTransaction()
                'RefreshExplorerTreeUI()
                Return
            Case CommandAction.ExecuteStressTest
                Me.RunAutomatedInteractionStressTest(activeView)
                Return

            Case CommandAction.LayoutCascade, CommandAction.LayoutTileHorizontal, CommandAction.LayoutTileVertical
                Me.ExecuteMdiLayoutLayoutTransform(action)
                Return

            Case CommandAction.ToggleInspectorPanel
                ' 🎯 THE VISIBILITY TOGGLE HOOK: Fetch the inspector instance out of your layout engine
                Dim inspector As Cls_Inspector = Me._parentBridge.WorkspaceLayout?.MyInspector
                If inspector IsNot Nothing Then
                    ' Invert the panel boolean state natively on the user interface thread
                    inspector.Visible = Not inspector.Visible
                End If

        End Select

        ' Hard guardrail: Viewport modifications are rejected if no document is focused
        If activeView Is Nothing Then Exit Sub

        Select Case action
            Case CommandAction.CloseDocument : Me.ExecuteCloseActiveDocumentFrame()
            Case CommandAction.ZoomIn : activeView.ExecuteZoomIn()
            Case CommandAction.ZoomOut : activeView.ExecuteZoomOut()
            Case CommandAction.ZoomFit : activeView.ExecuteZoomToFit()
            Case CommandAction.ToggleGrid : Me.ExecuteToggleGridVisibility(activeView)
            Case CommandAction.ToggleSnap : Me.ExecuteToggleGridSnapping(activeView)
            Case CommandAction.DeleteSelection : Me.ExecuteSelectionPurgeTransaction(activeView)
        End Select
        'RefreshExplorerTreeUI()
    End Sub




#End Region


#Region "View Property Inversions & State Synchronizations"

    Private Sub ExecuteToggleGridVisibility(ByVal view As Cls_Viewport)
        view.IsGridVisible = Not view.IsGridVisible
        Me.UpdateMenuCheckmarkStatus("menuItemGridVisible", view.IsGridVisible)
    End Sub

    Private Sub ExecuteToggleGridSnapping(ByVal view As Cls_Viewport)
        view.IsGridSnappingEnabled = Not view.IsGridSnappingEnabled
        Me.UpdateMenuCheckmarkStatus("menuItemGridSnapping", view.IsGridSnappingEnabled)
    End Sub

    Private Sub ExecuteSelectionPurgeTransaction(ByVal view As Cls_Viewport)
        Dim selMgr As Cls_Selection_Manager = view.SelectionManager
        If selMgr IsNot Nothing AndAlso selMgr.Count > 0 Then
            view.CanvasData.PurgeShapesGroup(selMgr.SelectedShapes)
            selMgr.Clear()

            Me.FlushPropertyGridInspector(Nothing)
            'Me.SynchronizeExplorerHierarchyTree(view)
            view.Invalidate()
        End If
    End Sub

#End Region

#Region "Status Bar, TreeView & PropertyGrid Synchronization Systems"



    Private Sub UpdateMenuCheckmarkStatus(ByVal menuItemName As String, ByVal isChecked As Boolean)
        Dim menuStripControl As MenuStrip = Nothing

        ' 1. Authoritatively resolve the menu reference directly through your layout framework
        If Me._parentBridge?.WorkspaceLayout?.MyMenu IsNot Nothing Then
            menuStripControl = Me._parentBridge.WorkspaceLayout.MyMenu.MenuStripControl
        End If

        ' 2. Process checkmark status mutations cleanly if the control reference is valid
        If menuStripControl IsNot Nothing Then
            Dim targetItem As ToolStripMenuItem = TryCast(menuStripControl.Items(menuItemName), ToolStripMenuItem)
            If targetItem IsNot Nothing Then
                targetItem.Checked = isChecked
            End If
        End If
    End Sub


#End Region

End Class


