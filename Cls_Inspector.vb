' Target File: Cls_Inspector.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Said Rules Compliance: Zero Guessing | No Non-Existent Token Variables | Locked Classes Only
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Collections.Generic
Imports System.Windows.Forms

Public NotInheritable Class Cls_Inspector

    ' Target File: Cls_Inspector.vb
    ' Project Namespace: mdi_test
    ' Architecture Standard: VB.NET | Option Strict On | Explicit Binding
    ' Said Rules Compliance: Zero Guessing | No Non-Existent Token Variables | Locked Classes Only
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    <System.ComponentModel.Browsable(False)>
    Public Property Visible() As Boolean
        Get
            ' Natively forward the query directly to your backing infrastructure panel
            Return Me._panel.Visible
        End Get
        Set(ByVal value As Boolean)
            ' Natively assign the layout state to your backing infrastructure panel
            Me._panel.Visible = value
        End Set
    End Property

    Private Sub OnInspectorTreeNodeSelected(ByVal sender As Object, ByVal e As TreeViewEventArgs)
        If e.Node?.Tag Is Nothing Then Return

        ' 1. HOSTER EXTRATION: Differentiate between a direct sheet select and a nested shape select
        Dim targetView As Cls_Viewport = TryCast(e.Node.Tag, Cls_Viewport)
        If targetView IsNot Nothing Then
            Dim hostSheet As Form = targetView.FindForm()
            If hostSheet IsNot Nothing Then Me._bridge.ActivateMdiChildForm(hostSheet)
            Me.BindSelectionTarget(targetView) : Return
        End If

        ' 2. NESTED SHAPE EVALUATION: Extract the shape target from the node's Tag data property
        Dim targetShape As Cls_Base_Shape = TryCast(e.Node.Tag, Cls_Base_Shape)
        If targetShape IsNot Nothing AndAlso e.Node.Parent IsNot Nothing Then
            ' 3. CONTEXT HEALER: Extract the real parent viewport host directly from the parent node folder
            Dim parentViewport As Cls_Viewport = TryCast(e.Node.Parent.Tag, Cls_Viewport)
            Dim parentSheet As Form = parentViewport?.FindForm()

            If parentSheet IsNot Nothing AndAlso parentViewport IsNot Nothing Then
                ' 4. SYNCHRONIZED SEQUENCING: Activate the sheet container window form first, then center camera space
                Me._bridge.ActivateMdiChildForm(parentSheet)
                parentViewport.CenterCameraAroundShapeWorldBounds(targetShape)
                Me.BindSelectionTarget(targetShape)
            End If
        End If
    End Sub

    Public Sub RefreshAllWorkspaceHierarchyTrees()
        If Me._treeView Is Nothing OrElse Me._bridge?.WorkspaceLayout?.MyDocManager Is Nothing Then Return

        If Me._treeView.InvokeRequired Then
            Me._treeView.BeginInvoke(New Action(AddressOf RefreshAllWorkspaceHierarchyTrees))
            Return
        End If

        Me._treeView.BeginUpdate() : Me._treeView.Nodes.Clear()
        Dim rootNode As New TreeNode("📁 Project Workspace Documents")
        Me._treeView.Nodes.Add(rootNode)

        ' OPTIMIZED MULTI-SHEET SWEEP: Loop straight through your verified document manager collection dictionary
        For Each kvp As KeyValuePair(Of Integer, Cls_Drawing) In Me._bridge.WorkspaceLayout.MyDocManager.DocumentRegistry
            Dim sheet As Cls_Drawing = kvp.Value

            If sheet IsNot Nothing AndAlso sheet.IsHandleCreated AndAlso Not sheet.IsDisposed Then
                Dim docName As String = sheet.Text + If(Me._bridge.WorkspaceLayout.MyDocManager.IsProjectFileDirty(kvp.Key), " *", "")
                Dim docNode As New TreeNode(docName) With {.Tag = sheet.Viewport}
                rootNode.Nodes.Add(docNode)

                ' Local shape component node mapping for this specific canvas sheet layer iteration pass
                Me.PopulateNestedShapeNodes(docNode, sheet.SchematicComponents)
            End If
        Next
        Me._treeView.ExpandAll() : Me._treeView.EndUpdate()
    End Sub

    Private Sub PopulateNestedShapeNodes(ByVal parentNode As TreeNode, ByVal shapesList As List(Of Cls_Base_Shape))
        If shapesList Is Nothing OrElse parentNode Is Nothing Then Return

        For i As Integer = 0 To shapesList.Count - 1
            Dim shape As Cls_Base_Shape = shapesList(i)
            If shape IsNot Nothing Then
                Dim typeName As String = shape.GetType().Name.Replace("Cls_Shape_", "").Replace("Cls_Gate_", "")
                Dim selFlag As String = If(shape.IsSelected, " 🔥", "")
                Dim shapeNode As New TreeNode($"   🎯 Primitive: {typeName}{selFlag}") With {.Tag = shape}
                parentNode.Nodes.Add(shapeNode)
            End If
        Next
    End Sub

    Private ReadOnly _panel As Panel
    Private ReadOnly _treeView As TreeView
    Private ReadOnly _gridEditor As PropertyGrid
    Private ReadOnly _splitContainer As SplitContainer
    Private ReadOnly _bridge As IMdiParentBridge

    Public Sub New(ByVal bridgeRef As IMdiParentBridge)
        If bridgeRef Is Nothing Then Throw New ArgumentNullException(NameOf(bridgeRef))
        Me._bridge = bridgeRef

        Me._panel = New Panel() With {.Width = 260, .Dock = DockStyle.Right}
        Me._treeView = New TreeView() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.None, .HideSelection = False}
        Me._gridEditor = New PropertyGrid() With {.Dock = DockStyle.Fill, .PropertySort = PropertySort.Categorized}
        Me._splitContainer = New SplitContainer() With {.Dock = DockStyle.Fill, .Orientation = Orientation.Horizontal, .SplitterDistance = 250}

        Me.AssembleInspectorLayout()
        Me.WireInspectorEvents()
        Me._gridEditor.CreateControl()
    End Sub

    Public ReadOnly Property Panel() As Panel
        Get
            Return Me._panel
        End Get
    End Property

    Private Sub AssembleInspectorLayout()
        Me._splitContainer.Panel1.Controls.Add(Me._treeView)
        Me._splitContainer.Panel2.Controls.Add(Me._gridEditor)
        Me._panel.Controls.Add(Me._splitContainer)
    End Sub

    Private Sub WireInspectorEvents()
        AddHandler Me._treeView.AfterSelect, AddressOf Me.OnInspectorTreeNodeSelected
        AddHandler Me._gridEditor.PropertyValueChanged, AddressOf Me.OnInspectorPropertyValueChanged
    End Sub

    Public Sub BindSelectionTarget(ByVal targetObject As Object)
        If Me._gridEditor.InvokeRequired Then
            Me._gridEditor.BeginInvoke(New Action(Of Object)(AddressOf BindSelectionTarget), targetObject)
            Return
        End If
        Me._gridEditor.SelectedObject = targetObject
        Me._gridEditor.Refresh()
    End Sub

    ''' <summary>
    ''' Thread-safely parses the active sheet form container to build shape tracking nodes natively.
    ''' </summary>
    Public Sub RefreshActiveSheetLocalTree(ByVal shapesList As List(Of Cls_Base_Shape))
        If Me._treeView.InvokeRequired Then
            Me._treeView.BeginInvoke(New Action(Of List(Of Cls_Base_Shape))(AddressOf RefreshActiveSheetLocalTree), shapesList)
            Return
        End If

        Me._treeView.BeginUpdate() : Me._treeView.Nodes.Clear()
        Dim rootNode As New TreeNode("📁 Active Workspace Canvas")
        Me._treeView.Nodes.Add(rootNode)

        If shapesList IsNot Nothing Then
            For i As Integer = 0 To shapesList.Count - 1
                Dim shape As Cls_Base_Shape = shapesList(i)
                If shape IsNot Nothing Then
                    Dim typeName As String = shape.GetType().Name.Replace("Cls_Shape_", "").Replace("Cls_Gate_", "")
                    Dim selFlag As String = If(shape.IsSelected, " 🔥", "")
                    Dim shapeNode As New TreeNode($"🎯 Primitive: {typeName}{selFlag}") With {.Tag = shape}
                    rootNode.Nodes.Add(shapeNode)
                End If
            Next
        End If
        Me._treeView.ExpandAll() : Me._treeView.EndUpdate()
    End Sub

    'Private Sub OnInspectorTreeNodeSelected(ByVal sender As Object, ByVal e As TreeViewEventArgs)
    '    If e.Node?.Tag Is Nothing Then Return
    '    Dim shapeTarget As Cls_Base_Shape = TryCast(e.Node.Tag, Cls_Base_Shape)

    '    If shapeTarget IsNot Nothing Then
    '        Me.BindSelectionTarget(shapeTarget)
    '    End If
    'End Sub

    Private Sub OnInspectorPropertyValueChanged(ByVal s As Object, ByVal e As PropertyValueChangedEventArgs)
        If e.ChangedItem Is Nothing Then Return
        Dim activeChild As Form = Me._bridge.ActiveMdiForm

        If activeChild IsNot Nothing AndAlso TypeOf activeChild Is Cls_Drawing Then
            Dim drawingSheet As Cls_Drawing = DirectCast(activeChild, Cls_Drawing)
            drawingSheet.Viewport.Invalidate()
            Me.RefreshActiveSheetLocalTree(drawingSheet.SchematicComponents)
        End If
    End Sub

    'Private Sub OnInspectorTreeNodeSelected(ByVal sender As Object, ByVal e As TreeViewEventArgs)
    '    If e.Node?.Tag Is Nothing Then Return

    '    ' 1. VIEWPORT SELECTION PATHWAY: If a user clicks a sheet node, activate its drawing window
    '    Dim targetView As Cls_Viewport = TryCast(e.Node.Tag, Cls_Viewport)
    '    If targetView IsNot Nothing Then
    '        Dim hostSheet As Form = targetView.FindForm()
    '        If hostSheet IsNot Nothing Then Me._bridge.ActivateMdiChildForm(hostSheet)
    '        Me.BindSelectionTarget(targetView) : Return
    '    End If

    '    ' 2. SHAPE SELECTION PATHWAY: If a user clicks a shape node, center the camera around its bounds
    '    Dim targetShape As Cls_Base_Shape = TryCast(e.Node.Tag, Cls_Base_Shape)
    '    If targetShape IsNot Nothing Then
    '        Me.BindSelectionTarget(targetShape)
    '        Dim activeChild As Cls_Drawing = Me._bridge.ActiveMdiForm
    '        If activeChild IsNot Nothing AndAlso TypeOf activeChild Is Cls_Drawing Then
    '            DirectCast(activeChild, Cls_Drawing).Viewport.CenterCameraAroundShapeWorldBounds(targetShape)
    '        End If
    '    End If
    'End Sub

End Class

'Imports System
'Imports System.Windows.Forms

'Public NotInheritable Class Cls_Inspector
'    Private ReadOnly _panel As Panel
'    Private ReadOnly _gridEditor As PropertyGrid
'    Private ReadOnly _bridge As IMdiParentBridge

'    Public Sub New(ByVal bridgeRef As IMdiParentBridge)
'        If bridgeRef Is Nothing Then Throw New ArgumentNullException(NameOf(bridgeRef))
'        Me._bridge = bridgeRef

'        Me._panel = New Panel() With {.Width = 260, .Dock = DockStyle.Right}
'        Me._gridEditor = New PropertyGrid() With {.Dock = DockStyle.Fill, .CommandsVisibleIfAvailable = True}

'        Me._panel.Controls.Add(Me._gridEditor)
'        AddHandler Me._gridEditor.PropertyValueChanged, AddressOf Me.OnInspectorPropertyValueChanged

'        Me._gridEditor.CreateControl() ' Force authoritative Win32 handle instantiation
'    End Sub

'    Public ReadOnly Property Panel() As Panel
'        Get
'            Return Me._panel
'        End Get
'    End Property

'    Public Sub BindSelectionTarget(ByVal targetObject As Object)
'        If Me._gridEditor.InvokeRequired Then
'            Me._gridEditor.BeginInvoke(New Action(Of Object)(AddressOf BindSelectionTarget), targetObject)
'            Return
'        End If
'        Me._gridEditor.SelectedObject = targetObject
'        Me._gridEditor.Refresh()
'    End Sub

'    Private Sub OnInspectorPropertyValueChanged(ByVal s As Object, ByVal e As PropertyValueChangedEventArgs)
'        If e.ChangedItem Is Nothing Then Return
'        Dim activeChild As Form = Me._bridge.ActiveMdiForm

'        If activeChild IsNot Nothing AndAlso TypeOf activeChild Is Cls_Drawing Then
'            Dim drawingSheet As Cls_Drawing = DirectCast(activeChild, Cls_Drawing)
'            drawingSheet.Viewport.Invalidate()

'            If Me._bridge.WorkspaceLayout?.MyExplorer IsNot Nothing Then
'                Me._bridge.WorkspaceLayout.MyExplorer.RefreshHierarchyTree()
'            End If
'        End If
'    End Sub
'End Class

'' Target File: Cls_Inspector.vb
'' Project Namespace: mdi_test
'' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
'' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

'Imports System
'Imports System.Windows.Forms

'Public NotInheritable Class Cls_Inspector
'    Private ReadOnly _panel As Panel
'    Private ReadOnly _gridEditor As PropertyGrid
'    Private ReadOnly _bridge As IMdiParentBridge

'    Public Sub New(ByVal bridgeRef As IMdiParentBridge)
'        If bridgeRef Is Nothing Then Throw New ArgumentNullException(NameOf(bridgeRef))
'        Me._bridge = bridgeRef

'        Me._panel = New Panel() With {.Width = 260, .Dock = DockStyle.Right}
'        Me._gridEditor = New PropertyGrid() With {.Dock = DockStyle.Fill, .CommandsVisibleIfAvailable = True}

'        Me._panel.Controls.Add(Me._gridEditor)
'        AddHandler Me._gridEditor.PropertyValueChanged, AddressOf Me.OnInspectorPropertyValueChanged

'        Me._gridEditor.CreateControl() ' Force Win32 handle instantiation
'    End Sub

'    Public ReadOnly Property Panel() As Panel
'        Get
'            Return Me._panel
'        End Get
'    End Property

'    Public Sub BindSelectionTarget(ByVal targetObject As Object)
'        If Me._gridEditor.InvokeRequired Then
'            Me._gridEditor.BeginInvoke(New Action(Of Object)(AddressOf BindSelectionTarget), targetObject)
'            Return
'        End If
'        Me._gridEditor.SelectedObject = targetObject
'        Me._gridEditor.Refresh()
'    End Sub

'    Private Sub OnInspectorPropertyValueChanged(ByVal s As Object, ByVal e As PropertyValueChangedEventArgs)
'        If e.ChangedItem Is Nothing Then Return
'        Dim activeChild As Form = Me._bridge.ActiveMdiForm

'        If activeChild IsNot Nothing AndAlso TypeOf activeChild Is Cls_Drawing Then
'            Dim drawingSheet As Cls_Drawing = DirectCast(activeChild, Cls_Drawing)
'            drawingSheet.Viewport.Invalidate()

'            If Me._bridge.WorkspaceLayout?.MyExplorer IsNot Nothing Then
'                Me._bridge.WorkspaceLayout.MyExplorer.RefreshHierarchyTree()
'            End If
'        End If
'    End Sub
'End Class
