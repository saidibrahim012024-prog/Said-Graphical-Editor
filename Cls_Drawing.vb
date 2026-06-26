
Public Class Cls_Drawing
    Inherits System.Windows.Forms.Form

    Private ReadOnly _viewport As Cls_Viewport
    Private ReadOnly _documentId As Integer
    Private ReadOnly _bridge As IMdiParentBridge
    Private ReadOnly _schematicComponents As New List(Of Cls_Base_Shape)()

    Public Sub New(ByVal docId As Integer, ByVal bridgeRef As IMdiParentBridge)
        MyBase.New()
        If bridgeRef Is Nothing Then Throw New ArgumentNullException(NameOf(bridgeRef))
        Me._documentId = docId
        Me._bridge = bridgeRef

        ' Construct the stateless viewport renderer explicitly
        Me._viewport = New Cls_Viewport() With {.Dock = System.Windows.Forms.DockStyle.Fill}
        Me.Text = $"Canvas Sheet {docId}"
        Me.Controls.Add(Me._viewport)
    End Sub

    Public ReadOnly Property ParentBridge() As IMdiParentBridge
        Get
            Return Me._bridge
        End Get
    End Property

    Public ReadOnly Property Viewport() As Cls_Viewport
        Get
            Return Me._viewport
        End Get
    End Property

    Public ReadOnly Property DocumentId() As Integer
        Get
            Return Me._documentId
        End Get
    End Property

    Public ReadOnly Property SchematicComponents() As List(Of Cls_Base_Shape)
        Get
            Return Me._schematicComponents
        End Get
    End Property

    Protected Overrides Sub OnFormClosing(ByVal e As System.Windows.Forms.FormClosingEventArgs)
        MyBase.OnFormClosing(e)
        ' Instantly execute data unregistration through the securely stored bridge pointer
        Me._bridge.WorkspaceLayout.MyDocManager.UnregisterDocument(Me._documentId)

        ' Refresh the layout elements instantly on screen
        Me._bridge.WorkspaceLayout.MyInspector.BindSelectionTarget(Nothing)
    End Sub
    'xxxxxx


    ''' <summary>
    ''' Performs rotation-safe and zoom-invariant hit testing using local origin space boundaries.
    ''' Fully type-safe under Option Strict On and resides safely under the 25-line limit.
    ''' </summary>
    Public Function HitTestShapes(ByVal worldPoint As PointF, ByVal transformer As Cls_Coordinate_Transformer) As Cls_Base_Shape
        If Me._schematicComponents Is Nothing OrElse transformer Is Nothing Then Return Nothing

        ' Scan backwards from the top-most z-order element down to the background grid
        For i As Integer = Me._schematicComponents.Count - 1 To 0 Step -1
            Dim gate As Cls_Base_Shape = Me._schematicComponents(i)

            ' 1. Project the world mouse coordinate cleanly into the gate's local centered grid space
            Dim localMouse As PointF = transformer.TransformWorldToLocalShapeSpace(worldPoint, gate)

            ' 2. Construct a local footprint bounding box centered precisely around origin (0,0)
            Dim halfW As Single = gate.Bounds.Width / 2.0F
            Dim halfH As Single = gate.Bounds.Height / 2.0F
            Dim localBounds As New RectangleF(-halfW, -halfH, gate.Bounds.Width, gate.Bounds.Height)

            ' 3. Check boundary intersection against the corrected local envelope matrix
            If localBounds.Contains(localMouse) Then
                Return gate ' Match found! Instantly returns the targeted logic block reference
            End If
        Next

        Return Nothing

    End Function

    ''' <summary>
    ''' Clears a specific subset collection array of schematic gates from the primary database repository.
    ''' FIXED: Provides an explicit membership definition to permanently resolve compilation blocks.
    ''' </summary>
    Public Sub PurgeShapesGroup(ByVal shapesToPurge As IList(Of Cls_Base_Shape))
        If shapesToPurge Is Nothing OrElse shapesToPurge.Count = 0 Then
            Exit Sub
        End If

        ' Iterate through the provided deletion tracking collection array list
        For Each gate As Cls_Base_Shape In shapesToPurge
            ' Type-safely remove each component out of the single master tracking collection repository
            If Me._schematicComponents.Contains(gate) Then
                Me._schematicComponents.Remove(gate)
            End If
        Next

    End Sub

    ''' <summary>
    ''' Iterates and paints the background shapes database model primitives safely within the active matrix transform context.
    ''' </summary>
    Public Sub RenderWorld(g As Graphics)
        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

        ' Aligned precisely with the single-parameter contract requirement
        For Each shape As Cls_Base_Shape In _schematicComponents
            shape.Render(g)
        Next
    End Sub

End Class


'' Target File: Cls_Drawing.vb
'' Project Namespace: mdi_test
'' Architecture Standard: VB.NET | Option Strict On | Explicit Binding
'' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

'Public Class Cls_Drawing
'    Inherits System.Windows.Forms.Form

'    Private ReadOnly _viewport As Cls_Viewport
'    Private ReadOnly _documentId As Integer

'    Public Sub New(ByVal docId As Integer)
'        MyBase.New()
'        Me._documentId = docId
'        Me._viewport = New Cls_Viewport() With {.Dock = System.Windows.Forms.DockStyle.Fill}

'        Me.Text = $"Canvas Sheet {docId}"
'        Me.Controls.Add(Me._viewport)
'    End Sub

'    Public ReadOnly Property Viewport() As Cls_Viewport
'        Get
'            Return Me._viewport
'        End Get
'    End Property

'    Public ReadOnly Property DocumentId() As Integer
'        Get
'            Return Me._documentId
'        End Get
'    End Property

'    Protected Overrides Sub OnFormClosing(ByVal e As System.Windows.Forms.FormClosingEventArgs)
'        MyBase.OnFormClosing(e)
'        Dim mainFrm As System.Windows.Forms.Form = Me.MdiParent
'        If mainFrm Is Nothing Then mainFrm = System.Windows.Forms.Application.OpenForms("frmMain")

'        If mainFrm IsNot Nothing AndAlso TypeOf mainFrm Is IMdiParentBridge Then
'            Dim bridge As IMdiParentBridge = DirectCast(mainFrm, IMdiParentBridge)
'            bridge.MdiAction?.ProcessDocumentClosureEviction(Me._documentId)
'            bridge.WorkspaceLayout?.MyExplorer?.RefreshHierarchyTree(bridge.MdiAction.ProjectManager)
'        End If
'    End Sub
'End Class


'' Target File: Cls_Drawing.vb
'' Project Namespace: mdi_test
'' Architecture Standard: VB.NET | Option Strict On | Explicit Binding
'' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

'Public Class Cls_Drawing
'    Inherits System.Windows.Forms.Form



'    Private ReadOnly _viewport As Cls_Viewport
'    Private _documentId As Integer

'    Public Sub New(ByVal docId As Integer)
'        MyBase.New()
'        Me._documentId = docId
'        Me._viewport = New Cls_Viewport() With {.Dock = System.Windows.Forms.DockStyle.Fill}

'        Me.Text = $"Canvas Sheet {docId}"
'        Me.Controls.Add(Me._viewport)
'    End Sub

'    Public ReadOnly Property Viewport() As Cls_Viewport
'        Get
'            Return Me._viewport
'        End Get
'    End Property

'    Public ReadOnly Property DocumentId() As Integer
'        Get
'            Return Me._documentId
'        End Get
'    End Property


'    Protected Overrides Sub OnFormClosing(ByVal e As System.Windows.Forms.FormClosingEventArgs)
'        MyBase.OnFormClosing(e)

'        ' 1. Discover the main parent window shell hosting this custom drawing container sheet
'        Dim mainFrm As System.Windows.Forms.Form = Me.MdiParent
'        If mainFrm Is Nothing Then mainFrm = System.Windows.Forms.Application.OpenForms("frmMain")

'        ' 2. Cast cleanly through the parent communication bridge contract to invoke the controller
'        If mainFrm IsNot Nothing AndAlso TypeOf mainFrm Is IMdiParentBridge Then
'            Dim bridge As IMdiParentBridge = DirectCast(mainFrm, IMdiParentBridge)

'            ' 3. TRACE LINK: Route the closure notification directly back into your core brain instance
'            If bridge.MdiAction IsNot Nothing Then
'                bridge.MdiAction.ProcessDocumentClosureEviction(Me.DocumentId)
'            End If

'            ' 4. Instantly instruct the explorer layout pane to strip out dead nodes from the project tree
'            If bridge.WorkspaceLayout?.MyExplorer IsNot Nothing Then
'                bridge.WorkspaceLayout.MyExplorer.RefreshHierarchyTree(bridge.MdiAction.ProjectManager)
'            End If
'        End If
'    End Sub

'End Class
