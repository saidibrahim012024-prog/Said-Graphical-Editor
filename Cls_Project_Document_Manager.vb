' Target File: Cls_Project_Document_Manager.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Collections.Generic

Public NotInheritable Class Cls_Project_Document_Manager

    Private ReadOnly _registryMap As New Dictionary(Of Integer, Cls_Drawing)()
    Private ReadOnly _dirtyDocumentsList As New List(Of Integer)()

    Public Sub New()
        ' Explicitly initializes an empty project container memory segment
    End Sub

    Public ReadOnly Property DocumentRegistry() As Dictionary(Of Integer, Cls_Drawing)
        Get
            Return Me._registryMap
        End Get
    End Property

    ' Target File: Cls_Project_Document_Manager.vb
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    Public Function RegisterDocument(ByVal bridge As IMdiParentBridge) As Cls_Drawing
        If bridge Is Nothing Then Throw New ArgumentNullException(NameOf(bridge))
        Dim nextId As Integer = Me.GenerateUniqueDocumentId()

        ' Explicitly inject the bridge reference cleanly into the document model lifecycle
        Dim newSheet As New Cls_Drawing(nextId, bridge)

        Me._registryMap.Add(nextId, newSheet)
        bridge.RegisterMdiChild(newSheet)
        Return newSheet
    End Function

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
        ' Restored contract: Evaluates if drawing sheet ID contains unsaved CAD changes
        Return Me._dirtyDocumentsList.Contains(documentId)
    End Function

    Private Function GenerateUniqueDocumentId() As Integer
        Dim highestId As Integer = 0
        For Each id As Integer In Me._registryMap.Keys
            If id > highestId Then highestId = id
        Next
        Return highestId + 1
    End Function




    'Public Function RegisterDocument(ByVal bridge As IMdiParentBridge) As Cls_Drawing
    '    If bridge Is Nothing Then Throw New ArgumentNullException(NameOf(bridge))
    '    Dim nextId As Integer = Me.GenerateUniqueDocumentId()
    '    Dim newSheet As New Cls_Drawing(nextId)

    '    Me._registryMap.Add(nextId, newSheet)
    '    bridge.RegisterMdiChild(newSheet)
    '    Return newSheet
    'End Function

    'Public Sub UnregisterDocument(ByVal documentId As Integer)
    '    If Me._registryMap.ContainsKey(documentId) Then
    '        Me._registryMap.Remove(documentId)
    '    End If
    '    If Me._dirtyDocumentsList.Contains(documentId) Then
    '        Me._dirtyDocumentsList.Remove(documentId)
    '    End If
    'End Sub

    'Private Function GenerateUniqueDocumentId() As Integer
    '    Dim highestId As Integer = 0
    '    For Each id As Integer In Me._registryMap.Keys
    '        If id > highestId Then highestId = id
    '    Next
    '    Return highestId + 1
    'End Function
End Class

