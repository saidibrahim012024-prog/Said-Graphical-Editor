
' Target File: frmMain.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Drawing
Imports System.Windows.Forms

Public NotInheritable Class frmMain
    Inherits Form
    Implements IMdiParentBridge

    Private _WorkspaceLayout As Cls_Layout
    Private _mdiAction As Cls_MDI_Action
    Private _isSnapToGridEnabled As Boolean = False

#Region "Explorer Initialization & Window Registration"

    Public Sub RegisterMdiChild(ByVal child As Form) Implements IMdiParentBridge.RegisterMdiChild
        If child Is Nothing Then Return

        child.MdiParent = Me
        Diagnostics.Debug.WriteLine($"[CAD TEST - REGISTER] Attached liveness hooks to Sheet: Title='{child.Text}'")
        child.Show()
        Me._mdiAction.SynchronizeInspectorTreeToActiveSheet()
        ' Clean parameterless view refresh via the single authoritative data manager instance
        '  Me._WorkspaceLayout.MyExplorer.RefreshHierarchyTree()
    End Sub

    Private Sub frmMain_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Me.IsMdiContainer = True
        Me.Text = "Authoritative Object-Oriented MDI Frame Layout Shell [v6]"
        Me.Size = New Size(1024, 768)
        Me.StartPosition = FormStartPosition.CenterScreen

        Diagnostics.Debug.WriteLine($"[BOOT TRACE 1] frmMain Handle Created: {Me.IsHandleCreated} | Total Controls: {Me.Controls.Count}")

        ' 1. Authoritatively instantiate your workspace layout engine first
        Me._WorkspaceLayout = New Cls_Layout(Me)

        ' 2. EXTRACT PERSISTENT REFERENCE: Pull the database manager directly out of the layout instance
        Dim persistentManager As Cls_Project_Document_Manager = Me._WorkspaceLayout.MyDocManager

        ' 3. Initialize your core action brain by passing the live persistent layout manager channel
        Me._mdiAction = New Cls_MDI_Action(Me, persistentManager)

        Me.WireCoreActionHandlers()
        Me._WorkspaceLayout.RefreshLayoutTheme()

        ' 4. Force immediate tree initialization tracking natively using the unified reference channel
        'Me._WorkspaceLayout.MyExplorer.RefreshHierarchyTree()
    End Sub

#End Region

#Region "IMdiParentBridge Explicit Property Implementation"

    Public Property IsSnapToGridEnabled() As Boolean Implements IMdiParentBridge.IsSnapToGridEnabled
        Get
            Return Me._isSnapToGridEnabled
        End Get
        Set(ByVal value As Boolean)
            Me._isSnapToGridEnabled = value
            Me.SyncSnapToGridState(value)
        End Set
    End Property

    Public ReadOnly Property WorkspaceLayout() As Cls_Layout Implements IMdiParentBridge.WorkspaceLayout
        Get
            Return Me._WorkspaceLayout
        End Get
    End Property

    Public ReadOnly Property ActiveMdiForm() As Form Implements IMdiParentBridge.ActiveMdiForm
        Get
            Return Me.ActiveMdiChild
        End Get
    End Property

    Public ReadOnly Property MdiChildrenForms() As Form() Implements IMdiParentBridge.MdiChildrenForms
        Get
            Return Me.MdiChildren
        End Get
    End Property

    Public ReadOnly Property MdiAction() As Cls_MDI_Action Implements IMdiParentBridge.MdiAction
        Get
            Return Me._mdiAction
        End Get
    End Property

    'Public ReadOnly Property Explorer() As Cls_Explorer Implements IMdiParentBridge.Explorer
    '    Get
    '        Return Me._WorkspaceLayout.MyExplorer
    '    End Get
    'End Property

#End Region

#Region "IMdiParentBridge Explicit Method Routing"

    Public Sub ActivateMdiChildForm(ByVal child As Form) Implements IMdiParentBridge.ActivateMdiChildForm
        If child IsNot Nothing Then child.Activate()
    End Sub

    Private Sub SyncSnapToGridState(ByVal enabled As Boolean)
        Dim activeView As Cls_Viewport = Me._mdiAction.GetActiveMdiViewport()
        If activeView IsNot Nothing Then
            activeView.IsGridSnappingEnabled = enabled
        End If
    End Sub

#End Region

#Region "Form Initialization Lifecycle"

    ''' <summary>
    ''' Maps UI component actions directly to the central MDI action controller pipeline.
    ''' FIXED: Replaced legacy lambda closures with decoupled, type-explicit method routing listeners.
    ''' </summary>
    Private Sub WireCoreActionHandlers()
        If Me._WorkspaceLayout IsNot Nothing Then
            AddHandler Me._WorkspaceLayout.MyMenu.CommandExecuted, AddressOf Me.OnLayoutCommandIntercepted
            AddHandler Me._WorkspaceLayout.MyToolbar.CommandExecuted, AddressOf Me.OnLayoutCommandIntercepted
            AddHandler Me._WorkspaceLayout.MyMenu.WindowMenuOpeningRequested, AddressOf Me.OnWindowMenuOpeningRequested
        End If
    End Sub

#End Region

#Region "Decoupled Event Interception Routes"

    Private Sub OnLayoutCommandIntercepted(ByVal sender As Object, ByVal action As CommandAction)
        If Me._mdiAction IsNot Nothing Then
            If action = CommandAction.NewDocument Then
                Me._mdiAction.ExecuteNewDocumentTransaction()
            Else
                Me._mdiAction.ProcessUiCommand(action)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Handles MDI window dropdown item updates programmatically before the strip unfolds.
    ''' </summary>
    Private Sub OnWindowMenuOpeningRequested(ByVal sender As Object, ByVal e As EventArgs)
        Dim activeView As Cls_Viewport = Me._mdiAction.GetActiveMdiViewport()

        ' Synchronize menu checked states with internal parameters before rendering
        If activeView IsNot Nothing Then
            Me._WorkspaceLayout.MyToolbar.SynchronizeToolbarGridStates(activeView.IsGridVisible, activeView.IsGridSnappingEnabled)
        End If
    End Sub

#End Region

End Class

'' Target File: frmMain.vb
'' Project Namespace: mdi_test
'' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
'' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

'Imports System
'Imports System.Drawing
'Imports System.Windows.Forms

'Public NotInheritable Class frmMain
'    Inherits Form
'    Implements IMdiParentBridge

'#Region "Explorer"

'    Public Sub RegisterMdiChild(ByVal child As Form) Implements IMdiParentBridge.RegisterMdiChild
'        If child Is Nothing Then Return

'        child.MdiParent = Me
'        AddHandler child.FormClosed, AddressOf Me.Diagnostics_OnMdiChildFormClosed
'        Diagnostics.Debug.WriteLine($"[CAD TEST - REGISTER] Attached liveness hooks to Sheet: Title='{child.Text}'")

'        child.Show()

'        ' Clean, instant workspace refresh since the child handles its own lifecycle events
'        Me._WorkspaceLayout.MyExplorer.RefreshHierarchyTree(Me._mdiAction.ProjectManager)
'    End Sub

'    Private Sub Diagnostics_OnMdiChildFormClosed(ByVal sender As Object, ByVal e As FormClosedEventArgs)

'        Dim targetForm As Form = TryCast(sender, Form)
'        If targetForm Is Nothing Then Return

'        Diagnostics.Debug.WriteLine($"[CAD TEST - PHASE 2 CLOSED] Form completely destroyed for '{targetForm.Text}'. Rebuilding Explorer Tree Views...")

'        ' Force the explorer panels to synchronize exactly what remains alive in the container collections
'        Me._WorkspaceLayout.MyExplorer.RefreshHierarchyTree(Me._mdiAction.ProjectManager)
'        Me._WorkspaceLayout.MyExplorer.SynchronizeWorkspaceTree(Me)

'    End Sub



'    Private Sub frmMain_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
'        Me.IsMdiContainer = True
'        Me.Text = "Authoritative Object-Oriented MDI Frame Layout Shell [v6]"
'        Me.Size = New Size(1024, 768)
'        Me.StartPosition = FormStartPosition.CenterScreen

'        Diagnostics.Debug.WriteLine($"[BOOT TRACE 1] frmMain Handle Created: {Me.IsHandleCreated} | Total Controls: {Me.Controls.Count}")

'        Dim docManager As New Cls_Project_Document_Manager()
'        Me._mdiAction = New Cls_MDI_Action(Me, docManager)

'        Me._WorkspaceLayout = New Cls_Layout(Me)

'        Me.WireCoreActionHandlers()
'        Me._WorkspaceLayout.RefreshLayoutTheme()

'        ' Force immediate tree initialization tracking
'        Me._WorkspaceLayout.MyExplorer.SynchronizeWorkspaceTree(Me)
'    End Sub

'#End Region


'    Private _WorkspaceLayout As Cls_Layout
'    Private _mdiAction As Cls_MDI_Action
'    Private _isSnapToGridEnabled As Boolean = False

'#Region "IMdiParentBridge Explicit Property Implementation"

'    Public Property IsSnapToGridEnabled() As Boolean Implements IMdiParentBridge.IsSnapToGridEnabled
'        Get
'            Return Me._isSnapToGridEnabled
'        End Get
'        Set(ByVal value As Boolean)
'            Me._isSnapToGridEnabled = value
'            Me.SyncSnapToGridState(value)
'        End Set
'    End Property

'    Public ReadOnly Property WorkspaceLayout() As Cls_Layout Implements IMdiParentBridge.WorkspaceLayout
'        Get
'            Return Me._WorkspaceLayout
'        End Get
'    End Property

'    Public ReadOnly Property ActiveMdiForm() As Form Implements IMdiParentBridge.ActiveMdiForm
'        Get
'            Return Me.ActiveMdiChild
'        End Get
'    End Property

'    Public ReadOnly Property MdiChildrenForms() As Form() Implements IMdiParentBridge.MdiChildrenForms
'        Get
'            Return Me.MdiChildren
'        End Get
'    End Property

'    Public ReadOnly Property MdiAction() As Cls_MDI_Action Implements IMdiParentBridge.MdiAction
'        Get
'            Return Me._mdiAction
'        End Get
'    End Property

'    Public ReadOnly Property Explorer() As Cls_Explorer Implements IMdiParentBridge.Explorer
'        Get
'            ' Clean routing to the layout engine's explorer instance to fulfill interface contract
'            Return Me._WorkspaceLayout.MyExplorer
'        End Get
'    End Property


'    Private Sub OnLayoutCommandIntercepted(ByVal sender As Object, ByVal action As CommandAction)
'            If Me._mdiAction IsNot Nothing Then
'                ' Direct, single pathway execution pass
'                If action = CommandAction.NewDocument Then
'                    Me._mdiAction.ExecuteNewDocumentTransaction()
'                Else
'                    Me._mdiAction.ProcessUiCommand(action)
'                End If
'            End If
'        End Sub

'#End Region

'#Region "IMdiParentBridge Explicit Method Routing"

'    Public Sub ActivateMdiChildForm(ByVal child As Form) Implements IMdiParentBridge.ActivateMdiChildForm
'        If child IsNot Nothing Then child.Activate()
'    End Sub

'    Private Sub SyncSnapToGridState(ByVal enabled As Boolean)
'        Dim activeView As Cls_Viewport = Me._mdiAction.GetActiveMdiViewport()
'        If activeView IsNot Nothing Then
'            activeView.IsGridSnappingEnabled = enabled
'        End If
'    End Sub

'#End Region

'#Region "Form Initialization Lifecycle"

'    ''' <summary>
'    ''' Maps UI component actions directly to the central MDI action controller pipeline.
'    ''' FIXED: Replaced legacy lambda closures with decoupled, type-explicit method routing listeners.
'    ''' </summary>
'    Private Sub WireCoreActionHandlers()

'        If Me._WorkspaceLayout IsNot Nothing Then
'            ' Bind the menu and toolbar pipeline events cleanly
'            AddHandler Me._WorkspaceLayout.MyMenu.CommandExecuted, AddressOf Me.OnLayoutCommandIntercepted
'            AddHandler Me._WorkspaceLayout.MyToolbar.CommandExecuted, AddressOf Me.OnLayoutCommandIntercepted

'            ' Bind window dropdown list refresh handlers
'            AddHandler Me._WorkspaceLayout.MyMenu.WindowMenuOpeningRequested, AddressOf Me.OnWindowMenuOpeningRequested
'        End If

'    End Sub

'#End Region

'#Region "Decoupled Event Interception Routes"

'    ''' <summary>
'    ''' Handles MDI window dropdown item updates programmatically before the strip unfolds.
'    ''' </summary>
'    Private Sub OnWindowMenuOpeningRequested(ByVal sender As Object, ByVal e As EventArgs)
'        Dim activeView As Cls_Viewport = Me._mdiAction.GetActiveMdiViewport()

'        ' Synchronize menu checked states with internal parameters before rendering
'        If activeView IsNot Nothing Then
'            Me._WorkspaceLayout.MyToolbar.SynchronizeToolbarGridStates(activeView.IsGridVisible, activeView.IsGridSnappingEnabled)
'        End If
'    End Sub

'#End Region

'End Class

