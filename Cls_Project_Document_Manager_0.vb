' Target File: Cls_Project_Document_Manager.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Collections.Generic

Public NotInheritable Class Cls_Project_Document_Manager
    ' Target File: Cls_Project_Document_Manager.vb
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    Public Function RegisterDocument(ByVal bridge As IMdiParentBridge) As Cls_Drawing
        If bridge Is Nothing Then Throw New ArgumentNullException(NameOf(bridge))
        Dim nextId As Integer = Me.GenerateUniqueDocumentId()
        Dim newSheet As New Cls_Drawing(nextId)

        ' 1. Record the tracking pair inside the dictionary
        Me._registryMap.Add(nextId, newSheet.Viewport)

        ' 2. THE NEW STRATEGY HOOK: Bind the layout explorer directly to this canvas's lifecycle events
        If bridge.WorkspaceLayout?.MyExplorer IsNot Nothing Then
            AddHandler newSheet.Viewport.ShapeCollectionChanged, AddressOf Me.OnViewportCollectionMutated
        End If

        bridge.RegisterMdiChild(newSheet)
        Return newSheet
    End Function

    Private Sub OnViewportCollectionMutated(ByVal sender As Object, ByVal e As EventArgs)
        Dim mainFrm As Form = Application.OpenForms("frmMain")
        If mainFrm IsNot Nothing AndAlso TypeOf mainFrm Is IMdiParentBridge Then
            Dim bridge As IMdiParentBridge = DirectCast(mainFrm, IMdiParentBridge)

            ' The observer receives the announcement and instantly updates the tree nodes cleanly
            bridge.WorkspaceLayout.MyExplorer.RefreshHierarchyTree()
        End If
    End Sub


    Private ReadOnly _registryMap As New Dictionary(Of Integer, Cls_Viewport)()
    Private ReadOnly _dirtyDocumentsList As New List(Of Integer)()

    Public Sub New()
        ' Explicitly initializes an empty project container memory segment
    End Sub

    Public ReadOnly Property DocumentRegistry() As Dictionary(Of Integer, Cls_Viewport)
        Get
            Return Me._registryMap
        End Get
    End Property

    'Public Function RegisterDocument(ByVal bridge As IMdiParentBridge) As Cls_Drawing
    '    If bridge Is Nothing Then Throw New ArgumentNullException(NameOf(bridge))
    '    Dim nextId As Integer = Me.GenerateUniqueDocumentId()
    '    Dim newSheet As New Cls_Drawing(nextId)

    '    Me._registryMap.Add(nextId, newSheet.Viewport)
    '    bridge.RegisterMdiChild(newSheet)
    '    Return newSheet
    'End Function

    Public Sub UnregisterDocument(ByVal documentId As Integer)
        If Me._registryMap.ContainsKey(documentId) Then
            Me._registryMap.Remove(documentId)
        End If
        If Me._dirtyDocumentsList.Contains(documentId) Then
            Me._dirtyDocumentsList.Remove(documentId)
        End If
    End Sub

    Public Sub FlagDocumentAsDirty(ByVal documentId As Integer, ByVal isDirty As Boolean)
        If isDirty Then
            If Not Me._dirtyDocumentsList.Contains(documentId) Then
                Me._dirtyDocumentsList.Add(documentId)
            End If
        Else
            If Me._dirtyDocumentsList.Contains(documentId) Then
                Me._dirtyDocumentsList.Remove(documentId)
            End If
        End If
    End Sub

    Public Function IsProjectFileDirty(ByVal documentId As Integer) As Boolean
        ' Fixed member contract: Evaluates if sheet ID contains unsaved CAD changes
        Return Me._dirtyDocumentsList.Contains(documentId)
    End Function

    Public Function CalculateGlobalShapesDatabaseCount() As Integer
        Dim unifiedTotal As Integer = 0
        For Each kvp As KeyValuePair(Of Integer, Cls_Viewport) In Me._registryMap
            If kvp.Value?.CanvasData?.SchematicComponents IsNot Nothing Then
                unifiedTotal += kvp.Value.CanvasData.SchematicComponents.Count
            End If
        Next
        Return unifiedTotal
    End Function

    Private Function GenerateUniqueDocumentId() As Integer
        Dim highestId As Integer = 0
        For Each id As Integer In Me._registryMap.Keys
            If id > highestId Then highestId = id
        Next
        Return highestId + 1
    End Function
End Class

'' Target File: Cls_Project_Document_Manager.vb
'' Project Namespace: mdi_test
'' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
'' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

'Imports System
'Imports System.Collections.Generic

'Public NotInheritable Class Cls_Project_Document_Manager
'    Private ReadOnly _registryMap As New Dictionary(Of Integer, Cls_Viewport)()
'    Private ReadOnly _dirtyDocumentsList As New List(Of Integer)()

'    Public ReadOnly Property DocumentRegistry() As Dictionary(Of Integer, Cls_Viewport)
'        Get
'            Return Me._registryMap
'        End Get
'    End Property

'    Public Function RegisterDocument(ByVal bridge As IMdiParentBridge) As Cls_Drawing
'        If bridge Is Nothing Then Throw New ArgumentNullException(NameOf(bridge))
'        Dim nextId As Integer = Me.GenerateUniqueDocumentId()
'        Dim newSheet As New Cls_Drawing(nextId)

'        Me._registryMap.Add(nextId, newSheet.Viewport)
'        bridge.RegisterMdiChild(newSheet)
'        Return newSheet
'    End Function

'    Public Sub UnregisterDocument(ByVal documentId As Integer)
'        If Me._registryMap.ContainsKey(documentId) Then
'            Me._registryMap.Remove(documentId)
'        End If
'        If Me._dirtyDocumentsList.Contains(documentId) Then
'            Me._dirtyDocumentsList.Remove(documentId)
'        End If
'    End Sub

'    Private Function GenerateUniqueDocumentId() As Integer
'        Dim highestId As Integer = 0
'        For Each id As Integer In Me._registryMap.Keys
'            If id > highestId Then highestId = id
'        Next
'        Return highestId + 1
'    End Function
'End Class

'' Target File: Cls_Project_Document_Manager.vb
'' Project Namespace: mdi_test
'' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
'' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

'Imports System
'Imports System.Collections.Generic

'Public NotInheritable Class Cls_Project_Document_Manager


'    Public Function CreateNewWorkspaceSheet(ByVal bridge As IMdiParentBridge) As Cls_Drawing
'        Dim nextId As Integer = Me.GenerateUniqueDocumentId()
'        Dim newSheet As New Cls_Drawing(nextId)

'        ' Explicitly register the strongly-typed tracking control pair in your dictionary
'        Me.DocumentRegistry.Add(nextId, newSheet.Viewport)

'        ' Push the window safely out to the main MDI UI frame to mount docking behaviors
'        bridge.RegisterMdiChild(newSheet)
'        Return newSheet
'    End Function

'    Private Function GenerateUniqueDocumentId() As Integer
'        Dim highestId As Integer = 0
'        For Each id As Integer In Me.DocumentRegistry.Keys
'            If id > highestId Then highestId = id
'        Next
'        Return highestId + 1
'    End Function

'    Public Sub UnregisterDocumentByViewport(ByVal view As Cls_Viewport)
'        If view Is Nothing Then Return
'        Dim targetId As Integer = -1

'        For Each kvp As KeyValuePair(Of Integer, Cls_Viewport) In Me.DocumentRegistry
'            If kvp.Value Is view Then
'                targetId = kvp.Key
'                Exit For
'            End If
'        Next

'        If targetId <> -1 Then
'            Me.DocumentRegistry.Remove(targetId)
'        End If
'    End Sub


'    Private ReadOnly _registryMap As New Dictionary(Of Integer, Cls_Viewport)()
'    Private ReadOnly _dirtyDocumentsList As New List(Of Integer)()

'    Public ReadOnly Property DocumentRegistry As Dictionary(Of Integer, Cls_Viewport)
'        Get
'            Return _registryMap
'        End Get
'    End Property

'    Public Sub New()
'        ' Explicitly initializes an empty project container memory segment
'    End Sub

'    Public Sub RegisterDocument(documentId As Integer, viewportInstance As Cls_Viewport)
'        If viewportInstance Is Nothing Then Throw New ArgumentNullException(NameOf(viewportInstance))
'        If _registryMap.ContainsKey(documentId) Then Exit Sub

'        _registryMap.Add(documentId, viewportInstance)
'    End Sub

'    Public Sub UnregisterDocument(documentId As Integer)
'        If _registryMap.ContainsKey(documentId) Then
'            _registryMap.Remove(documentId)
'        End If
'        If _dirtyDocumentsList.Contains(documentId) Then
'            _dirtyDocumentsList.Remove(documentId)
'        End If
'    End Sub

'    Public Sub FlagDocumentAsDirty(documentId As Integer, isDirty As Boolean)
'        If isDirty Then
'            If Not _dirtyDocumentsList.Contains(documentId) Then _dirtyDocumentsList.Add(documentId)
'        Else
'            If _dirtyDocumentsList.Contains(documentId) Then _dirtyDocumentsList.Remove(documentId)
'        End If
'    End Sub

'    Public Function IsProjectFileDirty(documentId As Integer) As Boolean
'        Return _dirtyDocumentsList.Contains(documentId)
'    End Function

'    Public Function CalculateGlobalShapesDatabaseCount() As Integer
'        Dim unifiedTotal As Integer = 0
'        For Each kvp As KeyValuePair(Of Integer, Cls_Viewport) In _registryMap
'            unifiedTotal += kvp.Value.CanvasData.SchematicComponents.Count
'        Next
'        Return unifiedTotal
'    End Function
'End Class

