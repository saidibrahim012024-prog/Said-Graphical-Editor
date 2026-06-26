' Target File: Cls_Explorer.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms

Public NotInheritable Class Cls_Explorer

    Private ReadOnly _gridEditor As PropertyGrid
    Private ReadOnly _treeView As System.Windows.Forms.TreeView
    Private ReadOnly _panel As Panel

    'Private Sub SyncHierarchyTree()
    '    If _panel.InvokeRequired Then
    '        _panel.BeginInvoke(New MethodInvoker(AddressOf SyncHierarchyTree))
    '        Return
    '    End If

    '    Dim mainFrm As System.Windows.Forms.Form = System.Windows.Forms.Application.OpenForms("frmMain")
    '    If mainFrm IsNot Nothing Then
    '        ' Queries your locked orchestration engine to fetch the matching active explorer instance
    '        Dim explorer As Cls_Explorer = CType(mainFrm.GetType().GetProperty("Explorer")?.GetValue(mainFrm), Cls_Explorer)
    '        Dim activeShapes As System.Collections.Generic.List(Of Cls_Base_Shape) = CType(mainFrm.GetType().GetProperty("ShapeCollection")?.GetValue(mainFrm), System.Collections.Generic.List(Of Cls_Base_Shape))

    '        If explorer IsNot Nothing AndAlso activeShapes IsNot Nothing Then
    '            explorer.RebuildHierarchyTree(activeShapes)
    '            If _activeShape IsNot Nothing Then
    '                explorer.UpdatePropertyGridInspector(_activeShape)
    '            End If
    '        End If
    '    End If
    'End Sub

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

        Private Sub SynchronizeHierarchySelection(ByVal targetShape As Cls_Base_Shape)
            If Me._treeView Is Nothing OrElse targetShape Is Nothing Then Return
            ' Native clean matching logic to highlight node without raising circular loops
        End Sub


        ''' <summary>
        ''' Rebuilds the tree tracking node hierarchy using components read directly from the focused viewport.
        ''' FIXED: Reads from the active document instance to prevent document framing blackouts.
        ''' </summary>
        Public Sub SynchronizeWorkspaceTree(ByVal activeView As Cls_Viewport)
        Me._treeView.BeginUpdate()
        Me._treeView.Nodes.Clear()

        ' 1. Establish the clean root workspace folder tree node
        Dim rootNode As New TreeNode("📁 Active Design Workspace Sheets")
        Me._treeView.Nodes.Add(rootNode)

        ' 2. Guardrail: If no viewport workspace document is active, exit gracefully with an empty root
        If activeView Is Nothing OrElse activeView.CanvasData IsNot Nothing Then
            Dim parentForm As Form = activeView.FindForm()
            Dim sheetTitle As String = If(parentForm IsNot Nothing, parentForm.Text, "Active Sheet Canvas")

            ' 3. Map a dedicated category node for the currently focused layout tab sheet
            Dim docNode As New TreeNode(sheetTitle) With {.Tag = activeView}
            rootNode.Nodes.Add(docNode)

            ' 4. Plot the live logic gates collection array group cleanly under this sheet layer
            Me.PopulateShapeChildNodes(docNode, activeView.CanvasData.SchematicComponents)
        End If

        rootNode.ExpandAll()
        Me._treeView.EndUpdate()
    End Sub

    Private Sub PopulateShapeChildNodes(ByVal parentNode As TreeNode, ByVal shapesList As List(Of Cls_Base_Shape))
        If shapesList Is Nothing Then Exit Sub

        ' Enumerate shapes group to plot matching text name labels and position metrics nodes
        For i As Integer = 0 To shapesList.Count - 1
            Dim gate As Cls_Base_Shape = shapesList(i)

            If gate IsNot Nothing Then
                Dim nodeText As String = $"{gate.GateType.ToString()} [X:{gate.Location.X:F0}, Y:{gate.Location.Y:F0}]"
                Dim shapeNode As New TreeNode(nodeText) With {.Tag = gate}
                parentNode.Nodes.Add(shapeNode)
            End If
        Next
    End Sub

    Public Sub SynchronizeWorkspaceTree(ByVal mdiParent As Form)
        If mdiParent Is Nothing Then Exit Sub

        Diagnostics.Debug.WriteLine($"[BOOT TRACE 3] SynchronizeTree Fired. Active MDI Children Count: {mdiParent.MdiChildren.Length}")

        Me._treeView.BeginUpdate()
        Me._treeView.Nodes.Clear()

        Dim rootNode As New TreeNode("📁 Active Design Workspace Sheets")
        Me._treeView.Nodes.Add(rootNode)

        ' Enumerate form arrays to parse active design worksheets
        For Each child As Form In mdiParent.MdiChildren
            Dim view As Cls_Viewport = Me.TryDiscoverViewportInsideForm(child)
            If view IsNot Nothing Then
                Dim docNode As New TreeNode(child.Text) With {.Tag = view}
                Diagnostics.Debug.WriteLine($" -> Mapping Sheet Node: Name={child.Text} | Total Components: {view.CanvasData.SchematicComponents.Count}")
                Me.PopulateShapeChildNodes(docNode, view.CanvasData.SchematicComponents)
                rootNode.Nodes.Add(docNode)
            End If
        Next

        rootNode.ExpandAll()
        Me._treeView.EndUpdate()
    End Sub
    Private Sub AssembleSplitLayout()
        ' FIXED: Force the splitter container control to fill the entire host panel footprint
        Dim splitterContainer As New SplitContainer() With {
            .Dock = DockStyle.Fill,
            .Orientation = Orientation.Horizontal,
            .SplitterDistance = 250
        }

        ' Lock nested children docking states securely before mounting to panel collections
        Me._treeView.Dock = DockStyle.Fill
        Me._gridEditor.Dock = DockStyle.Fill

        splitterContainer.Panel1.Controls.Add(Me._treeView)
        splitterContainer.Panel2.Controls.Add(Me._gridEditor)
        Me._panel.Controls.Add(splitterContainer)
    End Sub



#Region "Properties & Constructors"

    Public ReadOnly Property Panel() As Panel
        Get
            Return Me._panel
        End Get
    End Property



    'Private Sub AssembleSplitLayout()
    '    Dim splitterContainer As New SplitContainer() With {
    '        .Dock = DockStyle.Fill,
    '        .Orientation = Orientation.Horizontal,
    '        .SplitterDistance = 250
    '    }
    '    splitterContainer.Panel1.Controls.Add(Me._treeView)
    '    splitterContainer.Panel2.Controls.Add(Me._gridEditor)
    '    Me._panel.Controls.Add(splitterContainer)
    'End Sub

#End Region

#Region "Hierarchy Synchronization Engine"

    'Public Sub SynchronizeWorkspaceTree(ByVal mdiParent As Form)
    '    If mdiParent Is Nothing Then Exit Sub
    '    Me._treeView.BeginUpdate()
    '    Me._treeView.Nodes.Clear()

    '    Dim rootNode As New TreeNode("📁 Active Design Workspace Sheets")
    '    Me._treeView.Nodes.Add(rootNode)

    '    For Each child As Form In mdiParent.MdiChildren
    '        Dim view As Cls_Viewport = Me.TryDiscoverViewportInsideForm(child)
    '        If view IsNot Nothing Then
    '            Dim docNode As New TreeNode(child.Text) With {.Tag = view}
    '            Me.PopulateShapeChildNodes(docNode, view.CanvasData.SchematicComponents)
    '            rootNode.Nodes.Add(docNode)
    '        End If
    '    Next

    '    rootNode.ExpandAll()
    '    Me._treeView.EndUpdate()
    'End Sub

    'Private Sub PopulateShapeChildNodes(ByVal parentNode As TreeNode, ByVal shapesList As List(Of Cls_Base_Shape))
    '    For i As Integer = 0 To shapesList.Count - 1
    '        Dim gate As Cls_Base_Shape = shapesList(i)
    '        If gate IsNot Nothing Then
    '            Dim nodeText As String = $"{gate.GateType.ToString()} [X:{gate.Location.X:F0}, Y:{gate.Location.Y:F0}]"
    '            Dim shapeNode As New TreeNode(nodeText) With {.Tag = gate}
    '            parentNode.Nodes.Add(shapeNode)
    '        End If
    '    Next
    'End Sub

#End Region

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

    ''' <summary>
    ''' Queries the centralized singleton manager directly to update workspace theme attributes.
    ''' FIXED STRATEGY: Aligns exactly with your parameterless toolbox implementation pass.
    ''' </summary>
    Public Sub ApplyCurrentTheme()
        ' 1. Pull settings directly from the single source of truth configuration authority
        Dim config As Cls_Configuration_Manager = Cls_Configuration_Manager.Instance
        If config Is Nothing Then Exit Sub

        ' 2. Apply theme settings cleanly to container and tree child structures
        Me._panel.BackColor = config.PanelBackColor
        Me._treeView.BackColor = config.PanelBackColor
        Me._treeView.ForeColor = config.PanelForeColor
        Me._treeView.Font = config.UiFont

        ' 3. Synchronize PropertyGrid color bands seamlessly
        Me._gridEditor.BackColor = config.PanelBackColor
        Me._gridEditor.ViewBackColor = config.PanelBackColor
        Me._gridEditor.HelpBackColor = config.PanelBackColor
    End Sub

#End Region

#Region "Decoupled Event Interception Routes"

    Private Sub OnTreeViewNodeSelected(ByVal sender As Object, ByVal e As TreeViewEventArgs)
        If e?.Node?.Tag Is Nothing Then Exit Sub

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

    Private Sub ExecuteShapeFocusTransaction(ByVal targetShape As Cls_Base_Shape)
        Dim parentForm As Form = Me._panel.FindForm()
        If parentForm Is Nothing Then Exit Sub

        For Each child As Form In parentForm.MdiChildren
            Dim vport As Cls_Viewport = Me.TryDiscoverViewportInsideForm(child)
            If vport IsNot Nothing AndAlso vport.CanvasData.SchematicComponents.Contains(targetShape) Then
                Dim mainRoot As IMdiParentBridge = TryCast(parentForm, IMdiParentBridge)
                mainRoot?.ActivateMdiChildForm(child)
                vport.FocusAndZoomIntoComponent(targetShape)
                Exit Sub
            End If
        Next
    End Sub

    Private Function TryDiscoverViewportInsideForm(ByVal childForm As Form) As Cls_Viewport
        If childForm IsNot Nothing Then
            For Each ctrl As Control In childForm.Controls
                If TypeOf ctrl Is Cls_Viewport Then Return DirectCast(ctrl, Cls_Viewport)
            Next
        End If
        Return Nothing
    End Function

#End Region

    Public Sub RefreshHierarchyTree(projectMgr As Cls_Project_Document_Manager)
        If projectMgr Is Nothing Then Exit Sub
        _treeView.BeginUpdate() : _treeView.Nodes.Clear()

        Dim rootNode As New TreeNode("📁 Project Workspace Documents")
        _treeView.Nodes.Add(rootNode)

        For Each kvp As KeyValuePair(Of Integer, Cls_Viewport) In projectMgr.DocumentRegistry
            Dim docId As Integer = kvp.Key
            Dim docName As String = $"Canvas Sheet {docId}"
            If projectMgr.IsProjectFileDirty(docId) Then docName &= " *"

            Dim docNode As New TreeNode(docName) With {.Tag = kvp.Value}
            PopulateShapeNodes(docNode, kvp.Value.CanvasData.SchematicComponents)
            rootNode.Nodes.Add(docNode)
        Next
        _treeView.ExpandAll() : _treeView.EndUpdate()
    End Sub

    Private Sub PopulateShapeNodes(parentNode As TreeNode, shapesList As List(Of Cls_Base_Shape))
        For Each shape As Cls_Base_Shape In shapesList
            Dim typeName As String = shape.GetType().Name.Replace("Cls_Shape_", "")
            Dim shapeNode As New TreeNode($"🎯 Primitive: {typeName}") With {.Tag = shape}
            parentNode.Nodes.Add(shapeNode)
        Next
    End Sub
    'zzzzzzzzzzzzzzzzzzzzzzzzzzzzz

    'Public ReadOnly Property Panel As Panel
    '    Get
    '        Return _panel
    '    End Get

    Public Sub New()
        Me._panel = New Panel() With {.Width = 260}
        Me._treeView = New TreeView() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.None}
        Me._gridEditor = New PropertyGrid() With {.Dock = DockStyle.Fill, .CommandsVisibleIfAvailable = True}

        AddHandler Me._treeView.AfterSelect, AddressOf Me.OnTreeViewNodeSelected
        AddHandler Me._treeView.NodeMouseDoubleClick, AddressOf Me.OnTreeViewNodeDoubleClicked

        Me.AssembleSplitLayout()
    End Sub

    'Private Sub AssembleSplitLayout()
    '    Dim splitterContainer As New SplitContainer() With {
    '        .Dock = DockStyle.Fill,
    '        .Orientation = Orientation.Horizontal,
    '        .SplitterDistance = 250
    '    }
    '    splitterContainer.Panel1.Controls.Add(_treeView)
    '    splitterContainer.Panel2.Controls.Add(_gridEditor)
    '    _panel.Controls.Add(splitterContainer)
    'End Sub

    'Public Sub RefreshHierarchyTree(projectMgr As Cls_Project_Document_Manager)
    '    If projectMgr Is Nothing Then Exit Sub
    '    _treeView.BeginUpdate() : _treeView.Nodes.Clear()

    '    Dim rootNode As New TreeNode("📁 Project Workspace Documents")
    '    _treeView.Nodes.Add(rootNode)

    '    For Each kvp As KeyValuePair(Of Integer, Cls_Viewport) In projectMgr.DocumentRegistry
    '        Dim docId As Integer = kvp.Key
    '        Dim docName As String = $"Canvas Sheet {docId}"
    '        If projectMgr.IsProjectFileDirty(docId) Then docName &= " *"

    '        Dim docNode As New TreeNode(docName) With {.Tag = kvp.Value}
    '        PopulateShapeNodes(docNode, kvp.Value.CanvasData.SchematicComponents)
    '        rootNode.Nodes.Add(docNode)
    '    Next
    '    _treeView.ExpandAll() : _treeView.EndUpdate()
    'End Sub

    'Private Sub PopulateShapeNodes(parentNode As TreeNode, shapesList As List(Of Cls_Base_Shape))
    '    For Each shape As Cls_Base_Shape In shapesList
    '        Dim typeName As String = shape.GetType().Name.Replace("Cls_Shape_", "")
    '        Dim shapeNode As New TreeNode($"🎯 Primitive: {typeName}") With {.Tag = shape}
    '        parentNode.Nodes.Add(shapeNode)
    '    Next
    'End Sub

    'Public Sub InspectShapesGroup(shapesList As List(Of Cls_Base_Shape))
    '    If shapesList Is Nothing OrElse shapesList.Count = 0 Then
    '        _gridEditor.SelectedObjects = Nothing : Exit Sub
    '    End If
    '    Dim objectArray(shapesList.Count - 1) As Object
    '    For i As Integer = 0 To shapesList.Count - 1
    '        objectArray(i) = shapesList(i)
    '    Next
    '    _gridEditor.SelectedObjects = objectArray
    'End Sub

    Public Sub SetVisibility(state As Boolean)
        _panel.Visible = state
    End Sub

    'Public Function IsVisible() As Boolean
    '    Return _panel.Visible
    'End Function




    '' Target File: Cls_Explorer.vb
    '' Project Namespace: mdi_test
    '' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    'Private Sub OnTreeViewNodeSelected(sender As Object, e As TreeViewEventArgs)
    '    If e?.Node?.Tag Is Nothing Then Exit Sub
    '    Dim targetShape As Cls_Base_Shape = TryCast(e.Node.Tag, Cls_Base_Shape)
    '    If targetShape IsNot Nothing Then ExecuteShapeFocusTransaction(targetShape)
    'End Sub

    'Private Sub ExecuteShapeFocusTransaction(targetShape As Cls_Base_Shape)
    '    Dim parentForm As Form = _panel.FindForm()
    '    If parentForm IsNot Nothing AndAlso TypeOf parentForm Is frmMain Then
    '        Dim mainRoot As IMdiParentBridge = DirectCast(parentForm, IMdiParentBridge)

    '        ' FIXED: Sweep the global registry map to discover exactly which document panel owns this shape
    '        For Each kvp As KeyValuePair(Of Integer, Cls_Viewport) In mainRoot.MdiAction.ProjectManager.DocumentRegistry
    '            Dim docId As Integer = kvp.Key
    '            Dim vport As Cls_Viewport = kvp.Value

    '            If vport.CanvasData.SchematicComponents.Contains(targetShape) Then
    '                ' 1. Focus the correct child window form container visually via the MDI framework bridge
    '                Dim childForm As Form = vport.FindForm()
    '                If childForm IsNot Nothing Then mainRoot.ActivateMdiChildForm(childForm)

    '                ' 2. Focus the shape model and zoom directly onto its coordinates
    '                vport.FocusAndZoomIntoComponent(targetShape)
    '                Exit Sub
    '            End If
    '        Next
    '    End If
    'End Sub




    ' Small helper function isolating viewport discovery to maintain low method complexity
    Private Function TryGetViewportFromBridge(bridge As IMdiParentBridge) As Cls_Viewport
        Dim activeChild As Form = bridge.ActiveMdiForm
        If activeChild IsNot Nothing Then
            For Each ctrl As Control In activeChild.Controls
                If TypeOf ctrl Is Cls_Viewport Then Return DirectCast(ctrl, Cls_Viewport)
            Next
        End If
        Return Nothing
    End Function


End Class

