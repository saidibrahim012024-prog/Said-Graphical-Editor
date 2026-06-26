Partial Public Class Cls_MDI_Action

    ' Target File: Cls_MDI_Action.vb
    ' Project Namespace: mdi_test
    ' Architecture Standard: VB.NET | Option Strict On | Explicit Binding
    ' Said Rules Compliance: Zero Guessing | No Non-Existent Token Variables | Locked Classes Only
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    Public Sub SynchronizeInspectorTreeToActiveSheet()
        ' Grab the strongly-typed inspector out of your layout engine instance configuration
        Dim customInspector As Cls_Inspector = Me._parentBridge.WorkspaceLayout?.MyInspector

        If customInspector IsNot Nothing Then
            ' AUTHENTIC CONDUIT DISPATCH: Force the inspector to run its full multi-sheet tracking loop pass
            customInspector.RefreshAllWorkspaceHierarchyTrees()
        End If
    End Sub

    'Public Sub SynchronizeInspectorTreeToActiveSheet()
    '    Dim activeChild As System.Windows.Forms.Form = Me._parentBridge.ActiveMdiForm
    '    Dim inspector As Cls_Inspector = Me._parentBridge.WorkspaceLayout?.MyInspector

    '    If inspector IsNot Nothing Then
    '        If activeChild IsNot Nothing AndAlso TypeOf activeChild Is Cls_Drawing Then
    '            Dim sheet As Cls_Drawing = DirectCast(activeChild, Cls_Drawing)
    '            ' Load the specific unrotated shapes list of the currently focused document
    '            inspector.RefreshActiveSheetLocalTree(sheet.SchematicComponents)
    '        Else
    '            ' Clear the inspector view entirely if no worksheet document remains open
    '            inspector.RefreshActiveSheetLocalTree(Nothing)
    '        End If
    '    End If
    'End Sub


    ' Target File: Cls_MDI_Action.vb
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    Public Sub UpdateActiveSelectionMetrics(ByVal selectedShape As Cls_Base_Shape)
        ' Grab the strongly-typed inspector out of your layout engine instance
        Dim customInspector As Cls_Inspector = Me._parentBridge.WorkspaceLayout?.MyInspector

        If customInspector IsNot Nothing Then
            ' Push the shape target reference straight down to bind into the grid editor rows
            customInspector.BindSelectionTarget(selectedShape)
        End If
    End Sub




    Private Delegate Sub SyncTreeDelegate(ByVal view As Cls_Viewport)

    Private _tvExplorer As TreeView
    Private _pgProperties As PropertyGrid

    ''' <summary>
    ''' Pushes the primary selected component reference directly out to the property grid panel inspector.
    ''' </summary>
    Public Sub FlushPropertyGridInspector(ByVal targetedShape As Cls_Base_Shape)
        If Me._pgProperties IsNot Nothing Then
            Me._pgProperties.SelectedObject = targetedShape
        End If
    End Sub
    'Private Sub RefreshExplorerTreeUI()

    '    ' 1. Forces the data registry to update its references to the live focused UI controls
    '    Dim activeView As Cls_Viewport = Me.GetActiveMdiViewport()

    '    ' 2. Instruct the parameterless tree view to paint the newly captured live collections
    '    If Me._parentBridge.WorkspaceLayout?.MyExplorer IsNot Nothing Then
    '        Me._parentBridge.WorkspaceLayout.MyExplorer.RefreshHierarchyTree()
    '    End If
    'End Sub


    'Private Sub RefreshExplorerTreeUI()
    '    ' Decoupled UI helper that safely handles re-rendering layout tree view elements
    '    If Me._parentBridge.WorkspaceLayout?.MyExplorer IsNot Nothing Then
    '        Me._parentBridge.WorkspaceLayout.MyExplorer.RefreshHierarchyTree()
    '    End If
    'End Sub


    'Public Sub SignalCanvasDataChanged()
    '    ' Guard against uninitialized layout structures safely
    '    If Me._parentBridge?.WorkspaceLayout?.MyExplorer Is Nothing Then Return

    '    ' Authoritatively force the explorer tree view panel to clear and redraw nodes natively
    '    Me._parentBridge.WorkspaceLayout.MyExplorer.RefreshHierarchyTree()
    'End Sub


#Region "SynchronizeExplorerHierarchyTree"


    '''' <summary>
    '''' Clears and rebuilds the TreeView hierarchy tracking nodes to reflect your master canvas database.
    '''' </summary>
    'Public Sub SynchronizeExplorerHierarchyTree_0(ByVal view As Cls_Viewport)
    '    If Me._tvExplorer Is Nothing OrElse view Is Nothing Then Exit Sub

    '    Me._tvExplorer.BeginUpdate()
    '    Me._tvExplorer.Nodes.Clear()

    '    Dim rootNode As TreeNode = Me._tvExplorer.Nodes.Add("Root_Schematic", "Schematic Workspace")
    '    Dim components As List(Of Cls_Base_Shape) = view.CanvasData.SchematicComponents

    '    ' Enumerate database shapes group to plot matching child hierarchy tracking nodes
    '    For i As Integer = 0 To components.Count - 1
    '        Dim gate As Cls_Base_Shape = components(i)
    '        If gate IsNot Nothing Then
    '            Dim displayLabel As String = $"{gate.GateType.ToString()} [X:{gate.Location.X:F0}, Y:{gate.Location.Y:F0}]"
    '            Dim childNode As TreeNode = rootNode.Nodes.Add(i.ToString(), displayLabel)
    '            childNode.Tag = gate
    '        End If
    '    Next

    '    rootNode.ExpandAll()
    '    Me._tvExplorer.EndUpdate()
    'End Sub

    '''' <summary>
    '''' Clears and rebuilds the TreeView hierarchy nodes safely across any thread context.
    '''' </summary>
    'Public Sub SynchronizeExplorerHierarchyTree_1(ByVal view As Cls_Viewport)
    '    If view Is Nothing Then Exit Sub

    '    ' Marshal high-frequency updates safely back onto the main thread context handle container
    '    Dim mainBridge As IMdiParentBridge = TryCast(Me._parentBridge, IMdiParentBridge)
    '    If mainBridge?.WorkspaceLayout?.MyExplorer?.InvokeRequired Then
    '        Dim methodCallback As New SyncTreeDelegate(AddressOf Me.SynchronizeExplorerHierarchyTree_1)
    '        mainBridge.WorkspaceLayout.MyExplorer.BeginInvoke(methodCallback, New Object() {view})
    '        Exit Sub
    '    End If

    '    ' FIXED STRATEGY: Pipe your data tracking manager directly to the explorer's established contract caller!
    '    If mainBridge?.WorkspaceLayout?.MyExplorer IsNot Nothing Then
    '        mainBridge.WorkspaceLayout.MyExplorer.RefreshHierarchyTree() 'Me._projectManager)

    '        ' Automatically refresh your property grid view mapping based on the focused shape
    '        Dim activeFocus As Cls_Base_Shape = view.SelectionManager.TryGetActiveFocus()
    '        mainBridge.WorkspaceLayout.MyExplorer.UpdatePropertyGridInspector(activeFocus)
    '    End If
    'End Sub


    '''' <summary>
    '''' Clears and rebuilds the TreeView hierarchy nodes safely across any calling thread context.
    '''' FIXED STRATEGY: Enforces explicit InvokeRequired marshaling to guarantee UI updates stick.
    '''' </summary>
    'Public Sub SynchronizeExplorerHierarchyTree_2(ByVal view As Cls_Viewport)
    '    If Me._tvExplorer Is Nothing OrElse view Is Nothing Then Exit Sub

    '    ' 1. FIXED Thread-Safe Gatekeeper check to marshal data onto the main UI thread handle context
    '    If Me._tvExplorer.InvokeRequired Then
    '        Dim methodCallback As New SyncTreeDelegate(AddressOf Me.SynchronizeExplorerHierarchyTree_2)
    '        Me._tvExplorer.BeginInvoke(methodCallback, New Object() {view})
    '        Exit Sub
    '    End If

    '    ' 2. Safe execution pass running strictly on the main thread ownership layer
    '    Me._tvExplorer.BeginUpdate()
    '    Me._tvExplorer.Nodes.Clear()

    '    Dim rootNode As TreeNode = Me._tvExplorer.Nodes.Add("Root_Schematic", "📁 Active Design Workspace Sheets")

    '    ' 3. Delegate the cross-sheet node tree generation to a segregated helper method
    '    Me.PopulateMdiChildrenExplorerNodes(rootNode, view)
    'End Sub

#End Region


    ''' <summary>
    ''' Segregated node populator to respect your strict 25 operational line limit per method.
    ''' </summary>
    Private Sub PopulateMdiChildrenExplorerNodes(ByVal rootNode As TreeNode, ByVal activeView As Cls_Viewport)

        ' Enumerate the authoritative child forms collection directly from our MDI parent wrapper
        For Each child As Form In Me._parentBridge.MdiChildrenForms
            Dim vport As Cls_Viewport = Me.TryDiscoverViewportInsideForm(child)

            If vport IsNot Nothing Then
                ' Append a design sheet category block matching the child container text title
                Dim docNode As TreeNode = rootNode.Nodes.Add(child.Text, child.Text)
                With docNode
                    .Tag = vport
                End With
                Dim components As List(Of Cls_Base_Shape) = vport.CanvasData.SchematicComponents

                ' Plot concrete child gate item tracking nodes directly under this sheet layer
                For i As Integer = 0 To components.Count - 1
                    Dim gate As Cls_Base_Shape = components(i)
                    If gate IsNot Nothing Then
                        Dim label As String = $"{gate.GateType.ToString()} [X:{gate.Location.X:F0}, Y:{gate.Location.Y:F0}]"
                        docNode.Nodes.Add(i.ToString(), label).Tag = gate
                    End If
                Next
            End If
        Next

        rootNode.ExpandAll()
        Me._tvExplorer.EndUpdate()
    End Sub


    '''' <summary>
    '''' Inspects the MDI parent form layout to bind required shell components type-safely.
    '''' </summary>
    'Private Sub BindParentShellUiComponents()
    '    ' Guard against uninitialized bridge infrastructure safely
    '    If Me._parentBridge?.WorkspaceLayout?.MyExplorer Is Nothing Then Return

    '    ' AUTHORITATIVE FIX: Pull the strongly-typed panel component directly from the layout engine
    '    Dim layoutExplorer As Cls_Explorer = Me._parentBridge.WorkspaceLayout.MyExplorer

    '    ' Bind internal references cleanly from the explorer container instead of hunting generic control trees
    '    Me._tvExplorer = layoutExplorer.TreeViewControl
    '    Me._pgProperties = layoutExplorer.PropertyGridControl
    'End Sub

End Class
