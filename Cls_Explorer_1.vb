' Target File: Cls_Explorer.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms

Public NotInheritable Class Cls_Explorer

    ' Target File: Cls_Explorer.vb
    ' Architecture Standard: VB.NET | Option Strict On | Explicit Binding
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    Public Sub RefreshHierarchyTree()
        If Me._docManager Is Nothing OrElse Me._treeView Is Nothing Then Return

        If Me._treeView.InvokeRequired Then
            Me._treeView.BeginInvoke(New Action(AddressOf RefreshHierarchyTree))
            Return
        End If

        Me._treeView.BeginUpdate() : Me._treeView.Nodes.Clear()
        Dim rootNode As New TreeNode("📁 Project Workspace Documents")
        Me._treeView.Nodes.Add(rootNode)

        ' Iterates straight over the strongly-typed window instances currently alive in memory
        For Each kvp As KeyValuePair(Of Integer, Cls_Drawing) In Me._docManager.DocumentRegistry
            Dim sheet As Cls_Drawing = kvp.Value

            If sheet IsNot Nothing AndAlso sheet.IsHandleCreated AndAlso Not sheet.IsDisposed Then
                Dim docNode As New TreeNode(sheet.Text) With {.Tag = sheet.Viewport}
                Me.PopulateShapeNodes(docNode, sheet.SchematicComponents)
                rootNode.Nodes.Add(docNode)
            End If
        Next
        Me._treeView.ExpandAll() : Me._treeView.EndUpdate()
    End Sub

    'Public Sub RefreshHierarchyTree()
    '    If Me._treeView Is Nothing Then Return

    '    If Me._treeView.InvokeRequired Then
    '        Me._treeView.BeginInvoke(New Action(AddressOf RefreshHierarchyTree))
    '        Return
    '    End If

    '    Me._treeView.BeginUpdate() : Me._treeView.Nodes.Clear()

    '    ' 1. Pull the main parent window shell frame contract safely through the host panel control
    '    Dim parentHost As Form = System.Windows.Forms.Application.OpenForms("frmMain")
    '    Dim bridge As IMdiParentBridge = TryCast(parentHost, IMdiParentBridge)

    '    ' 2. THE FOCUS STRATEGY: Grab ONLY the active canvas viewport currently drawn on screen
    '    Dim activeView As Cls_Viewport = bridge?.MdiAction?.GetActiveMdiViewport()

    '    If activeView IsNot Nothing AndAlso activeView.CanvasData?.SchematicComponents IsNot Nothing Then
    '        Dim sheetTitle As String = If(activeView.FindForm() IsNot Nothing, activeView.FindForm().Text, "Active Canvas Sheet")
    '        Dim rootNode As New TreeNode($"📁 {sheetTitle}") With {.Tag = activeView}
    '        Me._treeView.Nodes.Add(rootNode)

    '        ' 3. Paint exclusively the live, modified primitives list block 
    '        Me.PopulateShapeNodes(rootNode, activeView.CanvasData.SchematicComponents)
    '    End If

    '    Me._treeView.ExpandAll() : Me._treeView.EndUpdate()
    'End Sub

    'Public Sub RefreshHierarchyTree()
    '    If Me._docManager Is Nothing OrElse Me._treeView Is Nothing Then Return

    '    If Me._treeView.InvokeRequired Then
    '        Me._treeView.BeginInvoke(New Action(AddressOf RefreshHierarchyTree))
    '        Return
    '    End If

    '    Me._treeView.BeginUpdate() : Me._treeView.Nodes.Clear()
    '    Dim rootNode As New TreeNode("📁 Project Workspace Documents")
    '    Me._treeView.Nodes.Add(rootNode)

    '    For Each kvp As KeyValuePair(Of Integer, Cls_Viewport) In Me._docManager.DocumentRegistry
    '        Dim activeView As Cls_Viewport = kvp.Value
    '        If activeView IsNot Nothing AndAlso activeView.IsHandleCreated AndAlso Not activeView.IsDisposed Then
    '            Dim docNode As New TreeNode($"Canvas Sheet {kvp.Key}") With {.Tag = activeView}

    '            ' SELF-HEALING COLLECTION EXTRACTION: 
    '            ' Fall back directly to your PropertyGrid's focused object target to capture live elements
    '            Dim liveList As List(Of Cls_Base_Shape) = Me.ExtractLiveComponentsList(activeView)
    '            Me.PopulateShapeNodes(docNode, liveList)
    '            rootNode.Nodes.Add(docNode)
    '        End If
    '    Next
    '    Me._treeView.ExpandAll() : Me._treeView.EndUpdate()
    'End Sub

    Private Function ExtractLiveComponentsList(ByVal activeView As Cls_Viewport) As List(Of Cls_Base_Shape)
        ' 1. If the core viewport data layer contains live elements, return it immediately
        If activeView?.CanvasData?.SchematicComponents IsNot Nothing AndAlso activeView.CanvasData.SchematicComponents.Count > 0 Then
            Return activeView.CanvasData.SchematicComponents
        End If

        ' 2. REFERENCE FALLBACK: Read the live shape primitive currently bound inside your PropertyGrid inspector
        Dim activeShape As Cls_Base_Shape = TryCast(Me._gridEditor.SelectedObject, Cls_Base_Shape)
        If activeShape IsNot Nothing Then
            Dim temporaryList As New List(Of Cls_Base_Shape)()
            temporaryList.Add(activeShape)
            Return temporaryList
        End If
        Return Nothing
    End Function

    'Public Sub RefreshHierarchyTree()
    '    If Me._docManager Is Nothing OrElse Me._treeView Is Nothing Then Return

    '    If Me._treeView.InvokeRequired Then
    '        Me._treeView.BeginInvoke(New Action(AddressOf RefreshHierarchyTree))
    '        Return
    '    End If

    '    Me._treeView.BeginUpdate() : Me._treeView.Nodes.Clear()
    '    Dim rootNode As New TreeNode("📁 Project Workspace Documents")
    '    Me._treeView.Nodes.Add(rootNode)

    '    ' AUTHENTIC FIX: Loops straight through the verified, persistent backing data manager
    '    For Each kvp As KeyValuePair(Of Integer, Cls_Viewport) In Me._docManager.DocumentRegistry
    '        Dim activeView As Cls_Viewport = kvp.Value

    '        If activeView IsNot Nothing AndAlso activeView.IsHandleCreated AndAlso Not activeView.IsDisposed Then
    '            Dim docName As String = $"Canvas Sheet {kvp.Key}"
    '            If Me._docManager.IsProjectFileDirty(kvp.Key) Then docName &= " *"

    '            Dim docNode As New TreeNode(docName) With {.Tag = activeView}
    '            Me.PopulateShapeNodes(docNode, activeView.CanvasData.SchematicComponents)
    '            rootNode.Nodes.Add(docNode)
    '        End If
    '    Next
    '    Me._treeView.ExpandAll() : Me._treeView.EndUpdate()
    'End Sub

    Private Sub PopulateShapeNodes(ByVal parentNode As TreeNode, ByVal shapesList As List(Of Cls_Base_Shape))
        If parentNode Is Nothing Then Exit Sub

        If shapesList Is Nothing Then
            Diagnostics.Debug.WriteLine("[DROP DEBUG ALERT] PopulateShapeNodes Aborted: shapesList is Nothing!")
            Exit Sub
        End If

        Diagnostics.Debug.WriteLine($"[DROP DEBUG 3] PopulateShapeNodes Enumerating. Current Active Input Elements Count: {shapesList.Count}")

        For Each shape As Cls_Base_Shape In shapesList
            If shape IsNot Nothing Then
                Dim typeName As String = shape.GetType().Name.Replace("Cls_Shape_", "")
                Dim nodeText As String = String.Format("🎯 Primitive: {0} [X:{1:F0}, Y:{2:F0}]", typeName, shape.X, shape.Y)
                Dim shapeNode As New TreeNode(nodeText) With {.Tag = shape}
                parentNode.Nodes.Add(shapeNode)
            End If
        Next
    End Sub

    'Private Sub PopulateShapeNodes(ByVal parentNode As TreeNode, ByVal shapesList As List(Of Cls_Base_Shape))
    '    If shapesList Is Nothing OrElse parentNode Is Nothing Then Return

    '    For Each shape As Cls_Base_Shape In shapesList
    '        If shape IsNot Nothing Then
    '            Dim typeName As String = shape.GetType().Name.Replace("Cls_Shape_", "")
    '            Dim nodeText As String = String.Format("🎯 Primitive: {0} [X:{1:F0}, Y:{2:F0}]",
    '                                               typeName, shape.Location.X, shape.Location.Y)

    '            Dim shapeNode As New TreeNode(nodeText) With {.Tag = shape}
    '            parentNode.Nodes.Add(shapeNode)
    '        End If
    '    Next
    'End Sub


    Private ReadOnly _gridEditor As PropertyGrid
    Private ReadOnly _treeView As System.Windows.Forms.TreeView
    Private ReadOnly _panel As Panel
    Private _isInternalSync As Boolean = False
    Private ReadOnly _docManager As Cls_Project_Document_Manager


    ' Constructor now injects the core data manager dependency directly
    Public Sub New(ByVal manager As Cls_Project_Document_Manager)
        MyBase.New()
        Me._docManager = manager
        Me._panel = New Panel() With {.Width = 260}
        Me._treeView = New TreeView() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.None, .HideSelection = False}
        Me._gridEditor = New PropertyGrid() With {.Dock = DockStyle.Fill, .CommandsVisibleIfAvailable = True}
        AddHandler Me._treeView.AfterSelect, AddressOf Me.OnTreeViewNodeSelected
        AddHandler Me._treeView.NodeMouseDoubleClick, AddressOf Me.OnTreeViewNodeDoubleClicked
        AddHandler Me._gridEditor.PropertyValueChanged, AddressOf Me.OnPropertyGridValueChanged

        Me.AssembleSplitLayout()
    End Sub

    'Public Sub RefreshHierarchyTree()
    '    ' Parameterless call reads straight from the persistent data reference field
    '    If Me._docManager Is Nothing OrElse Me._treeView Is Nothing Then Return

    '    If Me._treeView.InvokeRequired Then
    '        Me._treeView.BeginInvoke(New Action(AddressOf RefreshHierarchyTree))
    '        Return
    '    End If

    '    Me._treeView.BeginUpdate() : Me._treeView.Nodes.Clear()
    '    Dim rootNode As New TreeNode("📁 Project Workspace Documents")
    '    Me._treeView.Nodes.Add(rootNode)

    '    For Each kvp As KeyValuePair(Of Integer, Cls_Viewport) In Me._docManager.DocumentRegistry
    '        Dim docName As String = $"Canvas Sheet {kvp.Key}"
    '        If Me._docManager.IsProjectFileDirty(kvp.Key) Then docName &= " *"

    '        Dim docNode As New TreeNode(docName) With {.Tag = kvp.Value}
    '        Me.PopulateShapeNodes(docNode, kvp.Value.CanvasData.SchematicComponents)
    '        rootNode.Nodes.Add(docNode)
    '    Next
    '    Me._treeView.ExpandAll() : Me._treeView.EndUpdate()
    'End Sub

    'xxxxxxxxxxxxxxxxxxxxxxx
    'Public Sub New()
    '    Me._panel = New Panel() With {.Width = 260}
    '    Me._treeView = New TreeView() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.None, .HideSelection = False}
    '    Me._gridEditor = New PropertyGrid() With {.Dock = DockStyle.Fill, .CommandsVisibleIfAvailable = True}

    '    AddHandler Me._treeView.AfterSelect, AddressOf Me.OnTreeViewNodeSelected
    '    AddHandler Me._treeView.NodeMouseDoubleClick, AddressOf Me.OnTreeViewNodeDoubleClicked
    '    AddHandler Me._gridEditor.PropertyValueChanged, AddressOf Me.OnPropertyGridValueChanged

    '    Me.AssembleSplitLayout()
    'End Sub

    Public ReadOnly Property TreeViewControl() As TreeView
        Get
            Return Me._treeView
        End Get
    End Property
    Public ReadOnly Property PropertyGridControl As PropertyGrid
        Get
            Return Me._gridEditor
        End Get
    End Property
    Public ReadOnly Property Panel() As Panel
        Get
            Return Me._panel
        End Get
    End Property

    Public ReadOnly Property Document_Manager As Cls_Project_Document_Manager
        Get
            Return _docManager
        End Get
    End Property

    ' Target File: Cls_Explorer.vb
    ' Project Namespace: mdi_test
    ' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    Public Sub RefreshHierarchyTree(ByVal projectMgr As Cls_Project_Document_Manager)
        If projectMgr Is Nothing OrElse Me._treeView Is Nothing Then Return

        If Me._treeView.InvokeRequired Then
            Me._treeView.BeginInvoke(New Action(Of Cls_Project_Document_Manager)(AddressOf RefreshHierarchyTree), projectMgr)
            Return
        End If

        ' SELF-CLEANING GATE: Evict entries whose hosting Cls_Drawing windows are severed from the main UI tree
        Me.ScrubSeveredDrawingFormsFromModel(projectMgr)

        Me._treeView.BeginUpdate() : Me._treeView.Nodes.Clear()
        Dim rootNode As New TreeNode("📁 Project Workspace Documents")
        Me._treeView.Nodes.Add(rootNode)

        For Each kvp As KeyValuePair(Of Integer, Cls_Viewport) In projectMgr.DocumentRegistry
            Dim docName As String = $"Canvas Sheet {kvp.Key}"
            If projectMgr.IsProjectFileDirty(kvp.Key) Then docName &= " *"

            Dim docNode As New TreeNode(docName) With {.Tag = kvp.Value}
            Me.PopulateShapeNodes(docNode, kvp.Value.CanvasData.SchematicComponents)
            rootNode.Nodes.Add(docNode)
        Next
        Me._treeView.ExpandAll() : Me._treeView.EndUpdate()

        ' Add this single verification block to the end of Cls_Explorer.RefreshHierarchyTree()
        Dim parentHost As Form = Me._panel.FindForm()
        If parentHost IsNot Nothing Then
            ' Invalidates the window hierarchy to force an immediate stream recalculation pass
            parentHost.Invalidate(True)
        End If

    End Sub

    Private Sub ScrubSeveredDrawingFormsFromModel(ByVal projectMgr As Cls_Project_Document_Manager)
        Dim deadDocIds As New List(Of Integer)()
        Dim mainFrm As Form = Application.OpenForms("frmMain")
        If mainFrm Is Nothing Then Return

        For Each kvp As KeyValuePair(Of Integer, Cls_Viewport) In projectMgr.DocumentRegistry
            ' 1. Locate the actual Cls_Drawing form container instance hosting this viewport
            Dim hostedForm As Form = kvp.Value.FindForm()
            Dim drawingForm As Cls_Drawing = TryCast(hostedForm, Cls_Drawing)

            ' 2. CRITICAL UI TREE VERIFICATION:
            ' If the form is null, disposed, or no longer exists inside the main window's real control hierarchy, it is dead
            If drawingForm Is Nothing OrElse drawingForm.IsDisposed OrElse Not mainFrm.Controls.Contains(drawingForm) Then
                ' If you use a nested workspace panel inside Cls_Layout, change "mainFrm.Controls.Contains" 
                ' to check your layout's tab container control array directly instead!
                deadDocIds.Add(kvp.Key)
            End If
        Next

        ' Erase dead tracking handles cleanly to synchronize your backend model with the UI screen
        For i As Integer = 0 To deadDocIds.Count - 1
            projectMgr.DocumentRegistry.Remove(deadDocIds(i))
        Next
    End Sub

    'Public Sub RefreshHierarchyTree(ByVal projectMgr As Cls_Project_Document_Manager)
    '    If projectMgr Is Nothing OrElse Me._treeView Is Nothing Then Return

    '    If Me._treeView.InvokeRequired Then
    '        Me._treeView.BeginInvoke(New Action(Of Cls_Project_Document_Manager)(AddressOf RefreshHierarchyTree), projectMgr)
    '        Return
    '    End If

    '    ' SELF-CLEANING GATE: Evict zombie entries whose layout panels have been dropped or destroyed
    '    Me.ScrubDeadDocumentHandlesFromModel(projectMgr)

    '    Me._treeView.BeginUpdate() : Me._treeView.Nodes.Clear()
    '    Dim rootNode As New TreeNode("📁 Project Workspace Documents")
    '    Me._treeView.Nodes.Add(rootNode)

    '    ' Iterates exclusively through live, validated canvas documents remaining in the data matrix
    '    For Each kvp As KeyValuePair(Of Integer, Cls_Viewport) In projectMgr.DocumentRegistry
    '        Dim docName As String = $"Canvas Sheet {kvp.Key}"
    '        If projectMgr.IsProjectFileDirty(kvp.Key) Then docName &= " *"

    '        Dim docNode As New TreeNode(docName) With {.Tag = kvp.Value}
    '        Me.PopulateShapeNodes(docNode, kvp.Value.CanvasData.SchematicComponents)
    '        rootNode.Nodes.Add(docNode)
    '    Next
    '    Me._treeView.ExpandAll() : Me._treeView.EndUpdate()
    'End Sub

    Private Sub ScrubDeadDocumentHandlesFromModel(ByVal projectMgr As Cls_Project_Document_Manager)
        Dim deadDocIds As New List(Of Integer)()

        For Each kvp As KeyValuePair(Of Integer, Cls_Viewport) In projectMgr.DocumentRegistry
            Dim hostForm As Form = kvp.Value.FindForm()
            ' If a tab/dock panel has closed, FindForm evaluates to Nothing or its Disposed state flags to True
            If hostForm Is Nothing OrElse hostForm.IsDisposed OrElse Not hostForm.Created Then
                deadDocIds.Add(kvp.Key)
            End If
        Next

        ' Atomic collection modifications executed on a separate loop block to safeguard iterator boundaries
        For i As Integer = 0 To deadDocIds.Count - 1
            projectMgr.DocumentRegistry.Remove(deadDocIds(i))
        Next
    End Sub

    Private Sub ExecuteShapeFocusTransaction(ByVal targetShape As Cls_Base_Shape)
        Dim parentForm As Form = Me._panel.FindForm()
        If parentForm IsNot Nothing AndAlso TypeOf parentForm Is IMdiParentBridge Then
            Dim bridge As IMdiParentBridge = DirectCast(parentForm, IMdiParentBridge)

            For Each child As Form In bridge.MdiChildrenForms
                If TypeOf child Is Cls_Drawing Then
                    Dim drawingForm As Cls_Drawing = DirectCast(child, Cls_Drawing)
                    ' Clean type-safe check against the primitive shape collection array bounds
                    If drawingForm.Viewport.CanvasData.SchematicComponents.Contains(targetShape) Then
                        bridge.ActivateMdiChildForm(drawingForm)
                        drawingForm.Viewport.FocusAndZoomIntoComponent(targetShape)
                        Exit Sub
                    End If
                End If
            Next
        End If
    End Sub

    'Public Sub RefreshHierarchyTree(ByVal projectMgr As Cls_Project_Document_Manager)
    '    If projectMgr Is Nothing OrElse Me._treeView Is Nothing Then Return

    '    If Me._treeView.InvokeRequired Then
    '        Me._treeView.BeginInvoke(New Action(Of Cls_Project_Document_Manager)(AddressOf RefreshHierarchyTree), projectMgr)
    '        Return
    '    End If

    '    Me._treeView.BeginUpdate() : Me._treeView.Nodes.Clear()
    '    Dim rootNode As New TreeNode("📁 Project Workspace Documents")
    '    Me._treeView.Nodes.Add(rootNode)

    '    ' Iterates exclusively through live, valid document mappings in the dictionary array
    '    For Each kvp As KeyValuePair(Of Integer, Cls_Viewport) In projectMgr.DocumentRegistry
    '        Dim docName As String = $"Canvas Sheet {kvp.Key}"
    '        If projectMgr.IsProjectFileDirty(kvp.Key) Then docName &= " *"

    '        Dim docNode As New TreeNode(docName) With {.Tag = kvp.Value}
    '        Me.PopulateShapeNodes(docNode, kvp.Value.CanvasData.SchematicComponents)
    '        rootNode.Nodes.Add(docNode)
    '    Next

    '    Me._treeView.ExpandAll() : Me._treeView.EndUpdate()
    'End Sub

    Public Sub SynchronizeWorkspaceTree(ByVal mdiParent As Form)
        If mdiParent Is Nothing OrElse Me._treeView Is Nothing Then Return

        If Me._treeView.InvokeRequired Then
            Me._treeView.BeginInvoke(New Action(Of Form)(AddressOf SynchronizeWorkspaceTree), mdiParent)
            Return
        End If

        Diagnostics.Debug.WriteLine($"[CAD TEST - EXPLORER LOOP] Rebuilding tree. Raw MdiChildren Array Count: {mdiParent.MdiChildren.Length}")
        Me._treeView.BeginUpdate() : Me._treeView.Nodes.Clear()
        Dim rootNode As New TreeNode("📁 Active Design Workspace Sheets")
        Me._treeView.Nodes.Add(rootNode)

        For Each child As Form In mdiParent.MdiChildren
            Diagnostics.Debug.WriteLine($" -> Processing Child window array entry: Text='{child.Text}', IsDisposed={child.IsDisposed}, Disposing={child.Disposing}, Visible={child.Visible}")

            If Not child.IsDisposed AndAlso Not child.Disposing AndAlso child.Visible Then
                Dim view As Cls_Viewport = Me.TryDiscoverViewportInsideForm(child)
                If view IsNot Nothing AndAlso view.CanvasData IsNot Nothing Then
                    Dim docNode As New TreeNode(child.Text) With {.Tag = view}
                    Me.PopulateShapeChildNodes(docNode, view.CanvasData.SchematicComponents)
                    rootNode.Nodes.Add(docNode)
                End If
            End If
        Next

        rootNode.ExpandAll() : Me._treeView.EndUpdate()
    End Sub

    'Public Sub SynchronizeWorkspaceTree(ByVal mdiParent As Form)
    '    If mdiParent Is Nothing OrElse Me._treeView Is Nothing Then Return

    '    If Me._treeView.InvokeRequired Then
    '        Me._treeView.BeginInvoke(New Action(Of Form)(AddressOf SynchronizeWorkspaceTree), mdiParent)
    '        Return
    '    End If

    '    Me._treeView.BeginUpdate() : Me._treeView.Nodes.Clear()
    '    Dim rootNode As New TreeNode("📁 Active Design Workspace Sheets")
    '    Me._treeView.Nodes.Add(rootNode)

    '    For Each child As Form In mdiParent.MdiChildren
    '        ' CRITICAL GUARD: Only map the form if it is NOT currently closing, closing down, or disposed
    '        If Not child.IsDisposed AndAlso Not child.Disposing AndAlso child.Visible Then
    '            Dim view As Cls_Viewport = Me.TryDiscoverViewportInsideForm(child)
    '            If view IsNot Nothing AndAlso view.CanvasData IsNot Nothing Then
    '                Dim docNode As New TreeNode(child.Text) With {.Tag = view}
    '                Me.PopulateShapeChildNodes(docNode, view.CanvasData.SchematicComponents)
    '                rootNode.Nodes.Add(docNode)
    '            End If
    '        End If
    '    Next

    '    rootNode.ExpandAll() : Me._treeView.EndUpdate()
    'End Sub

    'Public Sub SynchronizeWorkspaceTree(ByVal mdiParent As Form)
    '    If mdiParent Is Nothing OrElse Me._treeView Is Nothing Then Return

    '    If Me._treeView.InvokeRequired Then
    '        Me._treeView.BeginInvoke(New Action(Of Form)(AddressOf SynchronizeWorkspaceTree), mdiParent)
    '        Return
    '    End If

    '    Me._treeView.BeginUpdate() : Me._treeView.Nodes.Clear()
    '    Dim rootNode As New TreeNode("📁 Active Design Workspace Sheets")
    '    Me._treeView.Nodes.Add(rootNode)

    '    For Each child As Form In mdiParent.MdiChildren
    '        ' FIX: Guardrail against closed, closing, or disposed visual window layout panels
    '        If Not child.IsDisposed AndAlso child.Visible Then
    '            Dim view As Cls_Viewport = Me.TryDiscoverViewportInsideForm(child)
    '            If view IsNot Nothing AndAlso view.CanvasData IsNot Nothing Then
    '                Dim docNode As New TreeNode(child.Text) With {.Tag = view}
    '                Me.PopulateShapeChildNodes(docNode, view.CanvasData.SchematicComponents)
    '                rootNode.Nodes.Add(docNode)
    '            End If
    '        End If
    '    Next
    '    rootNode.ExpandAll() : Me._treeView.EndUpdate()
    'End Sub

    'Public Sub RefreshHierarchyTree(ByVal projectMgr As Cls_Project_Document_Manager)
    '    If projectMgr Is Nothing OrElse Me._treeView Is Nothing Then Return

    '    If Me._treeView.InvokeRequired Then
    '        Me._treeView.BeginInvoke(New Action(Of Cls_Project_Document_Manager)(AddressOf RefreshHierarchyTree), projectMgr)
    '        Return
    '    End If

    '    ' FIX: Gather dead or closed Document IDs to purge from the background model container registry
    '    Me.PurgeClosedDocumentsFromRegistry(projectMgr)

    '    Me._treeView.BeginUpdate() : Me._treeView.Nodes.Clear()
    '    Dim rootNode As New TreeNode("📁 Project Workspace Documents")
    '    Me._treeView.Nodes.Add(rootNode)

    '    For Each kvp As KeyValuePair(Of Integer, Cls_Viewport) In projectMgr.DocumentRegistry
    '        Dim docName As String = $"Canvas Sheet {kvp.Key}"
    '        If projectMgr.IsProjectFileDirty(kvp.Key) Then docName &= " *"

    '        Dim docNode As New TreeNode(docName) With {.Tag = kvp.Value}
    '        Me.PopulateShapeNodes(docNode, kvp.Value.CanvasData.SchematicComponents)
    '        rootNode.Nodes.Add(docNode)
    '    Next
    '    Me._treeView.ExpandAll() : Me._treeView.EndUpdate()
    'End Sub

    Private Sub PurgeClosedDocumentsFromRegistry(ByVal projectMgr As Cls_Project_Document_Manager)
        Dim deadDocIds As New List(Of Integer)()

        For Each kvp As KeyValuePair(Of Integer, Cls_Viewport) In projectMgr.DocumentRegistry
            Dim associatedForm As Form = kvp.Value.FindForm()
            ' If the viewport's hosting canvas sheet window has been closed or destroyed, tag it for deletion
            If associatedForm Is Nothing OrElse associatedForm.IsDisposed Then
                deadDocIds.Add(kvp.Key)
            End If
        Next

        ' Atomic collection mutations performed cleanly on a separate line loop to safeguard iterator boundaries
        For i As Integer = 0 To deadDocIds.Count - 1
            projectMgr.DocumentRegistry.Remove(deadDocIds(i))
        Next
    End Sub

    Public Sub SynchronizeWorkspaceTree(ByVal activeView As Cls_Viewport)
        If Me._treeView Is Nothing Then Exit Sub

        If Me._treeView.InvokeRequired Then
            Diagnostics.Debug.WriteLine("[TREE DEBUG MARSHAL] Marshalling SynchronizeWorkspaceTree(Viewport) to UI thread...")
            Me._treeView.BeginInvoke(New Action(Of Cls_Viewport)(AddressOf SynchronizeWorkspaceTree), activeView)
            Return
        End If

        If activeView Is Nothing OrElse activeView.CanvasData Is Nothing Then
            Diagnostics.Debug.WriteLine($"[TREE DEBUG ABORT] SynchronizeWorkspaceTree aborted. View Null={activeView Is Nothing}, CanvasData Null={If(activeView IsNot Nothing, activeView.CanvasData Is Nothing, True)}")
            Exit Sub
        End If

        Me._treeView.BeginUpdate() : Me._treeView.Nodes.Clear()
        Dim rootNode As New TreeNode("📁 Active Design Workspace Sheets")
        Me._treeView.Nodes.Add(rootNode)

        Dim parentForm As Form = activeView.FindForm()
        Dim sheetTitle As String = If(parentForm IsNot Nothing, parentForm.Text, "Active Sheet Canvas")
        Dim docNode As New TreeNode(sheetTitle) With {.Tag = activeView}
        rootNode.Nodes.Add(docNode)

        Diagnostics.Debug.WriteLine($" -> Mapping Component Child Nodes. Count: {activeView.CanvasData.SchematicComponents?.Count}")
        Me.PopulateShapeChildNodes(docNode, activeView.CanvasData.SchematicComponents)
        rootNode.ExpandAll() : Me._treeView.EndUpdate()
    End Sub

    'Public Sub RefreshHierarchyTree(ByVal projectMgr As Cls_Project_Document_Manager)
    '    If projectMgr Is Nothing Then
    '        Diagnostics.Debug.WriteLine("[TREE DEBUG ERROR] RefreshHierarchyTree aborted: projectMgr is Nothing!")
    '        Exit Sub
    '    End If

    '    If Me._treeView.InvokeRequired Then
    '        Diagnostics.Debug.WriteLine("[TREE DEBUG MARSHAL] Marshalling RefreshHierarchyTree to the UI Thread...")
    '        Me._treeView.BeginInvoke(New Action(Of Cls_Project_Document_Manager)(AddressOf RefreshHierarchyTree), projectMgr)
    '        Return
    '    End If

    '    Diagnostics.Debug.WriteLine($"[TREE DEBUG START] RefreshHierarchyTree called. Registry count: {projectMgr.DocumentRegistry?.Count}")
    '    Me._treeView.BeginUpdate() : Me._treeView.Nodes.Clear()
    '    Dim rootNode As New TreeNode("📁 Project Workspace Documents")
    '    Me._treeView.Nodes.Add(rootNode)

    '    For Each kvp As KeyValuePair(Of Integer, Cls_Viewport) In projectMgr.DocumentRegistry
    '        Dim docName As String = $"Canvas Sheet {kvp.Key}"
    '        If projectMgr.IsProjectFileDirty(kvp.Key) Then docName &= " *"

    '        Dim docNode As New TreeNode(docName) With {.Tag = kvp.Value}
    '        Diagnostics.Debug.WriteLine($" -> Adding Sheet Node: ID={kvp.Key} | Components: {kvp.Value?.CanvasData?.SchematicComponents?.Count}")
    '        Me.PopulateShapeNodes(docNode, kvp.Value.CanvasData.SchematicComponents)
    '        rootNode.Nodes.Add(docNode)
    '    Next
    '    Me._treeView.ExpandAll() : Me._treeView.EndUpdate()
    '    Diagnostics.Debug.WriteLine($"[TREE DEBUG SUCCESS] RefreshHierarchyTree completed. Total Root Nodes: {Me._treeView.Nodes.Count}")
    'End Sub




    Private Sub OnPropertyGridValueChanged(ByVal s As Object, ByVal e As PropertyValueChangedEventArgs)
        If e.ChangedItem Is Nothing Then Exit Sub

        Dim parentForm As Form = Me._panel.FindForm()
        If parentForm IsNot Nothing AndAlso TypeOf parentForm Is frmMain Then
            Dim mainRoot As IMdiParentBridge = DirectCast(parentForm, IMdiParentBridge)
            Dim activeView As Cls_Viewport = Me.TryGetViewportFromBridge(mainRoot)

            If activeView IsNot Nothing Then
                ' Forces live node label adjustments inside the text string format
                Me.SynchronizeWorkspaceTree(activeView)
                activeView.Invalidate()
            End If
        End If
    End Sub


    Public Sub UpdatePropertyGridInspector(ByVal targetShape As Cls_Base_Shape)
        If Me._gridEditor Is Nothing Then Return

        If Me._gridEditor.InvokeRequired Then
            Dim d As New Action(Of Cls_Base_Shape)(AddressOf UpdatePropertyGridInspector)
            Me._gridEditor.BeginInvoke(d, New Object() {targetShape})
            Return
        End If

        Me._gridEditor.SelectedObject = targetShape
        Me.SynchronizeHierarchySelection(targetShape)
    End Sub

    Public Sub SynchronizeHierarchySelection(ByVal targetShape As Cls_Base_Shape)
        If Me._treeView Is Nothing OrElse targetShape Is Nothing Then Return

        If Me._treeView.InvokeRequired Then
            Dim d As New Action(Of Cls_Base_Shape)(AddressOf SynchronizeHierarchySelection)
            Me._treeView.BeginInvoke(d, New Object() {targetShape})
            Return
        End If

        Me._isInternalSync = True
        For Each root As TreeNode In Me._treeView.Nodes
            Me.ScanAndSelectNode(root, targetShape)
        Next
        Me._isInternalSync = False
    End Sub

    'Public Sub SynchronizeWorkspaceTree(ByVal activeView As Cls_Viewport)
    '    If Me._treeView Is Nothing Then Return

    '    If Me._treeView.InvokeRequired Then
    '        Dim d As New Action(Of Cls_Viewport)(AddressOf SynchronizeWorkspaceTree)
    '        Me._treeView.BeginInvoke(d, New Object() {activeView})
    '        Return
    '    End If

    '    Me._treeView.BeginUpdate()
    '    Me._treeView.Nodes.Clear()
    '    Dim rootNode As New TreeNode("📁 Active Design Workspace Sheets")
    '    Me._treeView.Nodes.Add(rootNode)

    '    If activeView IsNot Nothing AndAlso activeView.CanvasData IsNot Nothing Then
    '        Dim parentForm As Form = activeView.FindForm()
    '        Dim sheetTitle As String = If(parentForm IsNot Nothing, parentForm.Text, "Active Sheet Canvas")
    '        Dim docNode As New TreeNode(sheetTitle) With {.Tag = activeView}
    '        rootNode.Nodes.Add(docNode)
    '        Me.PopulateShapeChildNodes(docNode, activeView.CanvasData.SchematicComponents)
    '    End If

    '    rootNode.ExpandAll()
    '    Me._treeView.EndUpdate()
    'End Sub

    'Public Sub SynchronizeWorkspaceTree(ByVal mdiParent As Form)
    '    If mdiParent Is Nothing OrElse Me._treeView Is Nothing Then Return

    '    If Me._treeView.InvokeRequired Then
    '        Dim d As New Action(Of Form)(AddressOf SynchronizeWorkspaceTree)
    '        Me._treeView.BeginInvoke(d, New Object() {mdiParent})
    '        Return
    '    End If

    '    Diagnostics.Debug.WriteLine($"[BOOT TRACE 3] SynchronizeTree Fired. Active MDI Children Count: {mdiParent.MdiChildren.Length}")
    '    Me._treeView.BeginUpdate() : Me._treeView.Nodes.Clear()
    '    Dim rootNode As New TreeNode("📁 Active Design Workspace Sheets")
    '    Me._treeView.Nodes.Add(rootNode)

    '    For Each child As Form In mdiParent.MdiChildren
    '        Dim view As Cls_Viewport = Me.TryDiscoverViewportInsideForm(child)
    '        If view IsNot Nothing AndAlso view.CanvasData IsNot Nothing Then
    '            Dim docNode As New TreeNode(child.Text) With {.Tag = view}
    '            Diagnostics.Debug.WriteLine($" -> Mapping Sheet Node: Name={child.Text} | Total Components: {view.CanvasData.SchematicComponents.Count}")
    '            Me.PopulateShapeChildNodes(docNode, view.CanvasData.SchematicComponents)
    '            rootNode.Nodes.Add(docNode)
    '        End If
    '    Next

    '    rootNode.ExpandAll() : Me._treeView.EndUpdate()
    'End Sub

    'Public Sub RefreshHierarchyTree(ByVal projectMgr As Cls_Project_Document_Manager)
    '    If projectMgr Is Nothing OrElse Me._treeView Is Nothing Then Return

    '    If Me._treeView.InvokeRequired Then
    '        Dim d As New Action(Of Cls_Project_Document_Manager)(AddressOf RefreshHierarchyTree)
    '        Me._treeView.BeginInvoke(d, New Object() {projectMgr})
    '        Return
    '    End If

    '    Me._treeView.BeginUpdate() : Me._treeView.Nodes.Clear()
    '    Dim rootNode As New TreeNode("📁 Project Workspace Documents")
    '    Me._treeView.Nodes.Add(rootNode)

    '    For Each kvp As KeyValuePair(Of Integer, Cls_Viewport) In projectMgr.DocumentRegistry
    '        Dim docId As Integer = kvp.Key
    '        Dim docName As String = $"Canvas Sheet {docId}"
    '        If projectMgr.IsProjectFileDirty(docId) Then docName &= " *"

    '        Dim docNode As New TreeNode(docName) With {.Tag = kvp.Value}
    '        Me.PopulateShapeNodes(docNode, kvp.Value.CanvasData.SchematicComponents)
    '        rootNode.Nodes.Add(docNode)
    '    Next

    '    Me._treeView.ExpandAll() : Me._treeView.EndUpdate()
    'End Sub

    Private Sub AssembleSplitLayout()
        Dim splitterContainer As New SplitContainer() With {
            .Dock = DockStyle.Fill,
            .Orientation = Orientation.Horizontal,
            .SplitterDistance = 250
        }

        Me._treeView.Dock = DockStyle.Fill
        Me._gridEditor.Dock = DockStyle.Fill

        splitterContainer.Panel1.Controls.Add(Me._treeView)
        splitterContainer.Panel2.Controls.Add(Me._gridEditor)
        Me._panel.Controls.Add(splitterContainer)
    End Sub

#Region "Property Grid Inspector Channels"

    Public Sub InspectShapesGroup(ByVal shapesList As List(Of Cls_Base_Shape))
        If shapesList Is Nothing OrElse shapesList.Count = 0 Then
            Me._gridEditor.SelectedObjects = Nothing : Exit Sub
        End If

        Dim objectArray(shapesList.Count - 1) As Object
        For i As Integer = 0 To shapesList.Count - 1
            objectArray(i) = shapesList(i)
        Next

        Me._gridEditor.SelectedObjects = objectArray
    End Sub

    Public Function IsVisible() As Boolean
        Return Me._panel.Visible
    End Function

    Public Sub SetVisibility(ByVal state As Boolean)
        Me._panel.Visible = state
    End Sub

    Public Sub ApplyCurrentTheme()
        Dim config As Cls_Configuration_Manager = Cls_Configuration_Manager.Instance
        If config Is Nothing Then Exit Sub

        Me._panel.BackColor = config.PanelBackColor
        Me._treeView.BackColor = config.PanelBackColor
        Me._treeView.ForeColor = config.PanelForeColor
        Me._treeView.Font = config.UiFont

        Me._gridEditor.BackColor = config.PanelBackColor
        Me._gridEditor.ViewBackColor = config.PanelBackColor
        Me._gridEditor.HelpBackColor = config.PanelBackColor
    End Sub

#End Region

#Region "Decoupled Event Interception Routes"

    Private Sub OnTreeViewNodeSelected(ByVal sender As Object, ByVal e As TreeViewEventArgs)
        If Me._isInternalSync OrElse e?.Node?.Tag Is Nothing Then Exit Sub

        Dim targetShape As Cls_Base_Shape = TryCast(e.Node.Tag, Cls_Base_Shape)
        If targetShape IsNot Nothing Then
            Me._gridEditor.SelectedObject = targetShape
        Else
            Dim targetViewport As Cls_Viewport = TryCast(e.Node.Tag, Cls_Viewport)
            If targetViewport IsNot Nothing Then Me._gridEditor.SelectedObject = targetViewport
        End If
    End Sub

    Private Sub OnTreeViewNodeDoubleClicked(ByVal sender As Object, ByVal e As TreeNodeMouseClickEventArgs)
        If e?.Node?.Tag Is Nothing Then Exit Sub

        Dim targetShape As Cls_Base_Shape = TryCast(e.Node.Tag, Cls_Base_Shape)
        If targetShape IsNot Nothing Then
            Me.ExecuteShapeFocusTransaction(targetShape)
        End If
    End Sub

    'Private Sub ExecuteShapeFocusTransaction(ByVal targetShape As Cls_Base_Shape)
    '    Dim parentForm As Form = Me._panel.FindForm()
    '    If parentForm IsNot Nothing AndAlso TypeOf parentForm Is frmMain Then
    '        Dim mainRoot As IMdiParentBridge = DirectCast(parentForm, IMdiParentBridge)

    '        For Each kvp As KeyValuePair(Of Integer, Cls_Viewport) In mainRoot.MdiAction.ProjectManager.DocumentRegistry
    '            Dim vport As Cls_Viewport = kvp.Value
    '            If vport.CanvasData.SchematicComponents.Contains(targetShape) Then
    '                Dim childForm As Form = vport.FindForm()
    '                If childForm IsNot Nothing Then mainRoot.ActivateMdiChildForm(childForm)
    '                vport.FocusAndZoomIntoComponent(targetShape)
    '                Exit Sub
    '            End If
    '        Next
    '    End If
    'End Sub

    Function TryDiscoverViewportInsideForm(ByVal childForm As Form) As Cls_Viewport
        If childForm IsNot Nothing Then
            For Each ctrl As Control In childForm.Controls
                If TypeOf ctrl Is Cls_Viewport Then Return DirectCast(ctrl, Cls_Viewport)
            Next
        End If
        Return Nothing
    End Function

    Private Sub PopulateShapeChildNodes(ByVal parentNode As TreeNode, ByVal shapesList As List(Of Cls_Base_Shape))
        If shapesList Is Nothing Then Exit Sub
        For i As Integer = 0 To shapesList.Count - 1
            Dim gate As Cls_Base_Shape = shapesList(i)
            If gate IsNot Nothing Then
                Dim nodeText As String = $"{gate.GateType.ToString()} [X:{gate.Location.X:F0}, Y:{gate.Location.Y:F0}]"
                Dim shapeNode As New TreeNode(nodeText) With {.Tag = gate}
                parentNode.Nodes.Add(shapeNode)
            End If
        Next
    End Sub
    'Private Sub PopulateShapeNodes(ByVal parentNode As TreeNode, ByVal shapesList As List(Of Cls_Base_Shape))
    '    If shapesList Is Nothing Then Return
    '    For Each shape As Cls_Base_Shape In shapesList
    '        Dim typeName As String = shape.GetType().Name.Replace("Cls_Shape_", "")
    '        Dim shapeNode As New TreeNode($"🎯 Primitive: {typeName}") With {.Tag = shape}
    '        parentNode.Nodes.Add(shapeNode)
    '    Next
    'End Sub
    Private Sub ScanAndSelectNode(ByVal parentNode As TreeNode, ByVal targetShape As Cls_Base_Shape)
        If parentNode.Tag Is targetShape Then
            Me._treeView.SelectedNode = parentNode
            Me._treeView.Focus()
            Return
        End If
        For Each child As TreeNode In parentNode.Nodes
            Me.ScanAndSelectNode(child, targetShape)
        Next
    End Sub
    Private Function TryGetViewportFromBridge(ByVal bridge As IMdiParentBridge) As Cls_Viewport
        Dim activeChild As Form = bridge.ActiveMdiForm
        If activeChild IsNot Nothing Then
            For Each ctrl As Control In activeChild.Controls
                If TypeOf ctrl Is Cls_Viewport Then Return DirectCast(ctrl, Cls_Viewport)
            Next
        End If
        Return Nothing
    End Function
#End Region
End Class


