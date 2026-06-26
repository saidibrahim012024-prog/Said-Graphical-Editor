' Target File: Cls_Explorer.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Collections.Generic
Imports System.Windows.Forms

Public NotInheritable Class Cls_Explorer
    Inherits System.Windows.Forms.Panel

    Private ReadOnly _treeView As System.Windows.Forms.TreeView
    Private ReadOnly _gridEditor As System.Windows.Forms.PropertyGrid
    Private ReadOnly _splitContainer As System.Windows.Forms.SplitContainer
    Private ReadOnly _docManager As Cls_Project_Document_Manager


    ' Target File: Cls_Explorer.vb
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

        For Each kvp As KeyValuePair(Of Integer, Cls_Drawing) In Me._docManager.DocumentRegistry
            Dim sheet As Cls_Drawing = kvp.Value
            If sheet IsNot Nothing AndAlso sheet.IsHandleCreated AndAlso Not sheet.IsDisposed Then
                Dim docName As String = sheet.Text + If(Me._docManager.IsProjectFileDirty(kvp.Key), " *", "")
                Dim docNode As New TreeNode(docName) With {.Tag = sheet.Viewport}
                Me.PopulateShapeNodes(docNode, sheet.SchematicComponents)
                rootNode.Nodes.Add(docNode)
            End If
        Next
        Me._treeView.ExpandAll() : Me._treeView.EndUpdate()
    End Sub

#Region "Constructor & Initialization"

    Public Sub New(ByVal manager As Cls_Project_Document_Manager)
        MyBase.New()
        If manager Is Nothing Then Throw New ArgumentNullException(NameOf(manager))
        Me._docManager = manager

        Me.Dock = DockStyle.Fill
        Me._treeView = New TreeView() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.None, .HideSelection = False}
        Me._gridEditor = New PropertyGrid() With {.Dock = DockStyle.Fill, .CommandsVisibleIfAvailable = True}
        Me._splitContainer = New SplitContainer() With {.Dock = DockStyle.Fill, .Orientation = Orientation.Horizontal, .SplitterDistance = 350}

        Me.AssembleSplitLayout()
        Me.WireExplorerEvents()
    End Sub

    Private Sub AssembleSplitLayout()
        Me._splitContainer.Panel1.Controls.Add(Me._treeView)
        Me._splitContainer.Panel2.Controls.Add(Me._gridEditor)
        Me.Controls.Add(Me._splitContainer)
    End Sub

    Private Sub WireExplorerEvents()
        AddHandler Me._treeView.AfterSelect, AddressOf Me.OnTreeViewNodeSelected
        AddHandler Me._gridEditor.PropertyValueChanged, AddressOf Me.OnPropertyGridValueChanged
    End Sub

#End Region

#Region "Exposed Property UI Components"

    Public ReadOnly Property TreeViewControl() As TreeView
        Get
            Return Me._treeView
        End Get
    End Property

    Public ReadOnly Property PropertyGridControl() As PropertyGrid
        Get
            Return Me._gridEditor
        End Get
    End Property

#End Region

#Region "Authoritative Synchronization Engines"

    'Public Sub RefreshHierarchyTree()
    '    If Me._docManager Is Nothing OrElse Me._treeView Is Nothing Then Return

    '    If Me._treeView.InvokeRequired Then
    '        Me._treeView.BeginInvoke(New Action(AddressOf RefreshHierarchyTree))
    '        Return
    '    End If

    '    Me._treeView.BeginUpdate() : Me._treeView.Nodes.Clear()
    '    Dim rootNode As New TreeNode("📁 Project Workspace Documents")
    '    Me._treeView.Nodes.Add(rootNode)

    '    For Each kvp As KeyValuePair(Of Integer, Cls_Drawing) In Me._docManager.DocumentRegistry
    '        Dim sheet As Cls_Drawing = kvp.Value
    '        If sheet IsNot Nothing AndAlso sheet.IsHandleCreated AndAlso Not sheet.IsDisposed Then
    '            Dim docName As String = sheet.Text + If(Me._docManager.IsProjectFileDirty(kvp.Key), " *", "")
    '            Dim docNode As New TreeNode(docName) With {.Tag = sheet.Viewport}
    '            Me.PopulateShapeNodes(docNode, sheet.SchematicComponents)
    '            rootNode.Nodes.Add(docNode)
    '        End If
    '    Next
    '    Me._treeView.ExpandAll() : Me._treeView.EndUpdate()
    'End Sub

    Private Sub PopulateShapeNodes(ByVal parentNode As TreeNode, ByVal shapesList As List(Of Cls_Base_Shape))
        If shapesList Is Nothing OrElse parentNode Is Nothing Then Return

        For Each shape As Cls_Base_Shape In shapesList
            If shape IsNot Nothing Then
                Dim typeName As String = shape.GetType().Name.Replace("Cls_Shape_", "")
                Dim nodeText As String = String.Format("🎯 Primitive: {0} [X:{1:F0}, Y:{2:F0}]", typeName, shape.X, shape.Y)
                Dim shapeNode As New TreeNode(nodeText) With {.Tag = shape}
                parentNode.Nodes.Add(shapeNode)
            End If
        Next
    End Sub

    Public Sub UpdatePropertyGridInspector(ByVal targetObject As Object)
        If Me._gridEditor.InvokeRequired Then
            Me._gridEditor.BeginInvoke(New Action(Of Object)(AddressOf UpdatePropertyGridInspector), targetObject)
            Return
        End If
        Me._gridEditor.SelectedObject = targetObject
    End Sub

#End Region

#Region "Decoupled Event Interception Routes"

    Private Sub OnTreeViewNodeSelected(ByVal sender As Object, ByVal e As TreeViewEventArgs)
        If e.Node?.Tag Is Nothing Then Return

        Dim targetShape As Cls_Base_Shape = TryCast(e.Node.Tag, Cls_Base_Shape)
        If targetShape IsNot Nothing Then
            Me.UpdatePropertyGridInspector(targetShape)
            Return
        End If

        Dim targetView As Cls_Viewport = TryCast(e.Node.Tag, Cls_Viewport)
        If targetView IsNot Nothing Then
            Me.UpdatePropertyGridInspector(targetView)
        End If
    End Sub

    Private Sub OnPropertyGridValueChanged(ByVal s As Object, ByVal e As PropertyValueChangedEventArgs)
        If e.ChangedItem Is Nothing Then Return

        Dim parentForm As Form = Me.FindForm()
        If parentForm IsNot Nothing AndAlso TypeOf parentForm Is IMdiParentBridge Then
            Dim bridge As IMdiParentBridge = DirectCast(parentForm, IMdiParentBridge)
            Dim activeChild As Form = bridge.ActiveMdiForm

            If activeChild IsNot Nothing AndAlso TypeOf activeChild Is Cls_Drawing Then
                Dim drawingForm As Cls_Drawing = DirectCast(activeChild, Cls_Drawing)
                Me.RefreshHierarchyTree()
                drawingForm.Viewport.Invalidate()
            End If
        End If
    End Sub

    Public Sub ApplyCurrentTheme()
        Me.BackColor = System.Drawing.SystemColors.Control
        Me._treeView.BackColor = System.Drawing.SystemColors.Window
        Me._gridEditor.ViewBackColor = System.Drawing.SystemColors.Window
    End Sub

#End Region

End Class

