
' Target File: Cls_Layout.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Windows.Forms

Public NotInheritable Class Cls_Layout

    ' THE SINGLE AGGREGATED PIPELINE CHANNELS REQUIRED BY THE ARCHITECTURE CORE
    Public Event ActionPipelineTriggered As EventHandler(Of CommandAction)
    Public Event ToolPipelineTriggered As EventHandler(Of CanvasTool)

#Region "Exposed Control Infrastructure Components"

    Private ReadOnly _myMenu As Cls_Menu
    Private ReadOnly _myToolbar As Cls_Toolbar
    Private ReadOnly _myStatusBar As Cls_StatusBar
    Private ReadOnly _myToolBox As Cls_Toolbox
    'Private ReadOnly _myExplorer As Cls_Explorer
    Private ReadOnly _parentForm As frmMain
    Private ReadOnly _myDocManager As Cls_Project_Document_Manager
    ' Add this backing field and public read-only property to Cls_Layout.vb:
    Private ReadOnly _myInspector As Cls_Inspector

    Public ReadOnly Property MyInspector() As Cls_Inspector
        Get
            Return Me._myInspector
        End Get
    End Property



    Public ReadOnly Property MyMenu() As Cls_Menu
        Get
            Return Me._myMenu
        End Get
    End Property
    Public ReadOnly Property MyToolbar() As Cls_Toolbar
        Get
            Return Me._myToolbar
        End Get
    End Property
    Public ReadOnly Property MyStatusBar() As Cls_StatusBar
        Get
            Return Me._myStatusBar
        End Get
    End Property
    Public ReadOnly Property MyToolBox() As Cls_Toolbox
        Get
            Return Me._myToolBox
        End Get
    End Property
    'Public ReadOnly Property MyExplorer() As Cls_Explorer
    '    Get
    '        Return Me._myExplorer
    '    End Get
    'End Property
    Public ReadOnly Property MyDocManager() As Cls_Project_Document_Manager
        Get
            Return Me._myDocManager
        End Get
    End Property

#End Region

    Public Sub New(ByVal parent As frmMain)
        If parent Is Nothing Then Throw New ArgumentNullException(NameOf(parent))
        Me._parentForm = parent

        ' AUTHORITATIVE STATE HOOK: Instantiate a single, persistent database channel
        Me._myDocManager = New Cls_Project_Document_Manager()

        Me._myToolBox = New Cls_Toolbox()
        ' Me._myExplorer = New Cls_Explorer(Me._myDocManager)
        Me._myMenu = New Cls_Menu()
        Me._myToolbar = New Cls_Toolbar()
        Me._myStatusBar = New Cls_StatusBar()
        Me._myInspector = New Cls_Inspector(parent)
        Me.WireCoreLayoutEvents()
        Me.AssembleLayout()
    End Sub

    Private Sub WireCoreLayoutEvents()
        ' Bind menu and toolbar commands cleanly to single-point entry tracks
        AddHandler Me._myMenu.CommandExecuted, AddressOf Me.OnMenuOrToolbarActionTriggered
        AddHandler Me._myToolbar.CommandExecuted, AddressOf Me.OnMenuOrToolbarActionTriggered

        ' Bind custom toolbox selections cleanly to the active tool stream channel
        AddHandler Me._myToolBox.ToolSelected, AddressOf Me.OnToolboxSelectionTriggered
    End Sub

    Public Sub AssembleLayout()

        'Me._myExplorer.Dock = DockStyle.Right
        Me._myToolBox.Panel.Dock = DockStyle.Left

        Me._parentForm.Controls.Add(Me._myInspector.Panel)
        ' Me._parentForm.Controls.Add(Me._myExplorer)
        Me._parentForm.Controls.Add(Me._myToolBox.Panel)

        ' FIXED: Add the backing framework strip property cleanly to the parent controls collection
        Me._parentForm.Controls.Add(Me._myStatusBar.Strip)
        Me._parentForm.Controls.Add(Me._myToolbar.Strip)
        Me._parentForm.Controls.Add(Me._myMenu.MenuStripControl)

        Me._myMenu.MenuStripControl.SendToBack()
        Me._myToolbar.Strip.SendToBack()
        Me._parentForm.MainMenuStrip = Me._myMenu.MenuStripControl
    End Sub

#Region "Decoupled Event Forwarding Routes"

    Private Sub OnMenuOrToolbarActionTriggered(ByVal sender As Object, ByVal action As CommandAction)
        ' Bubble up the command action payload straight through the unified pipeline channel
        RaiseEvent ActionPipelineTriggered(Me, action)
    End Sub

    Private Sub OnToolboxSelectionTriggered(ByVal sender As Object, ByVal e As ToolSelectedEventArgs)
        If e IsNot Nothing Then
            ' Bubble up the selected canvas tool identifier type-safely
            RaiseEvent ToolPipelineTriggered(Me, e.SelectedTool)
        End If
    End Sub

#End Region

#Region "Structural Control Placements"

    Public Sub RefreshLayoutTheme()
        Me._myToolBox.ApplyCurrentTheme()
        '  Me._myExplorer.ApplyCurrentTheme()

        Me._myMenu.MenuStripControl.Invalidate()
        Me._myToolbar.Strip.Invalidate()
        Me._myStatusBar.Strip.Invalidate()
    End Sub

#End Region

End Class

'' Target File: Cls_Layout.vb
'' Project Namespace: mdi_test
'' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
'' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

'Imports System
'Imports System.Windows.Forms

'Public NotInheritable Class Cls_Layout


'    ' THE SINGLE AGGREGATED PIPELINE CHANNELS REQUIRED BY THE ARCHITECTURE CORE
'    Public Event ActionPipelineTriggered As EventHandler(Of CommandAction)
'    Public Event ToolPipelineTriggered As EventHandler(Of CanvasTool)

'#Region "Exposed Control Infrastructure Components"

'    Private ReadOnly _myMenu As Cls_Menu
'    Private ReadOnly _myToolbar As Cls_Toolbar
'    Private ReadOnly _myStatusBar As Cls_StatusBar
'    Private ReadOnly _myToolBox As Cls_Toolbox
'    Private ReadOnly _myExplorer As Cls_Explorer
'    Private ReadOnly _parentForm As frmMain

'    Public ReadOnly Property MyMenu As Cls_Menu
'        Get
'            Return _myMenu
'        End Get
'    End Property
'    Public ReadOnly Property MyToolbar As Cls_Toolbar
'        Get
'            Return _myToolbar
'        End Get
'    End Property
'    Public ReadOnly Property MyStatusBar As Cls_StatusBar
'        Get
'            Return _myStatusBar
'        End Get
'    End Property
'    Public ReadOnly Property MyToolBox As Cls_Toolbox
'        Get
'            Return _myToolBox
'        End Get
'    End Property
'    Public ReadOnly Property MyExplorer As Cls_Explorer
'        Get
'            Return _myExplorer
'        End Get
'    End Property

'#End Region

'    Public Sub New(ByVal parent As frmMain)
'        If parent Is Nothing Then Throw New ArgumentNullException(NameOf(parent))
'        Me._parentForm = parent
'        Dim manager As Cls_Project_Document_Manager = _parentForm.MdiAction.ProjectManager

'        Me._myToolBox = New Cls_Toolbox()
'        Me._myExplorer = New Cls_Explorer(manager)
'        Me._myMenu = New Cls_Menu()
'        Me._myToolbar = New Cls_Toolbar()

'        ' FIXED: Instantiate your production custom class instead of the bare framework strip
'        Me._myStatusBar = New Cls_StatusBar()

'        Me.WireCoreLayoutEvents()
'        Me.AssembleLayout()
'    End Sub


'    ''' <summary>
'    ''' Maps layout menu, ribbon, and panel changes up to the parent container switchboard.
'    ''' FIXED: Replaced legacy lambda closures with decoupled, type-explicit method routing listeners.
'    ''' </summary>
'    Private Sub WireCoreLayoutEvents()
'        ' Bind menu and toolbar commands cleanly to single-point entry tracks
'        AddHandler Me._myMenu.CommandExecuted, AddressOf Me.OnMenuOrToolbarActionTriggered
'        AddHandler Me._myToolbar.CommandExecuted, AddressOf Me.OnMenuOrToolbarActionTriggered

'        ' Bind custom toolbox selections cleanly to the active tool stream channel
'        AddHandler Me._myToolBox.ToolSelected, AddressOf Me.OnToolboxSelectionTriggered
'    End Sub


'    ''' <summary>
'    ''' Assembles and docks user controls explicitly inside the parent form container window.
'    ''' </summary>
'    Public Sub AssembleLayout()
'        Me._myExplorer.Panel.Dock = DockStyle.Right
'        Me._myToolBox.Panel.Dock = DockStyle.Left

'        Me._parentForm.Controls.Add(Me._myExplorer.Panel)
'        Me._parentForm.Controls.Add(Me._myToolBox.Panel)
'        ' FIXED: Add the backing framework strip property cleanly to the parent controls collection
'        Me._parentForm.Controls.Add(Me._myStatusBar.Strip)
'        Me._parentForm.Controls.Add(Me._myToolbar.Strip)
'        Me._parentForm.Controls.Add(Me._myMenu.MenuStripControl)

'        Me._myMenu.MenuStripControl.SendToBack()
'        Me._myToolbar.Strip.SendToBack()
'        Me._parentForm.MainMenuStrip = Me._myMenu.MenuStripControl
'    End Sub

'#Region "Decoupled Event Forwarding Routes"

'    Private Sub OnMenuOrToolbarActionTriggered(ByVal sender As Object, ByVal action As CommandAction)
'        ' Bubble up the command action payload straight through the unified pipeline channel
'        RaiseEvent ActionPipelineTriggered(Me, action)
'    End Sub

'    Private Sub OnToolboxSelectionTriggered(ByVal sender As Object, ByVal e As ToolSelectedEventArgs)
'        If e IsNot Nothing Then
'            ' Bubble up the selected canvas tool identifier type-safely
'            RaiseEvent ToolPipelineTriggered(Me, e.SelectedTool)
'        End If
'    End Sub

'#End Region

'#Region "Structural Control Placements"



'    ''' <summary>
'    ''' Invalidates view regions dynamically to flush style modifications to screen glass.
'    ''' </summary>
'    Public Sub RefreshLayoutTheme()
'        Me._myToolBox.ApplyCurrentTheme()
'        Me._myExplorer.ApplyCurrentTheme()

'        Me._myMenu.MenuStripControl.Invalidate()
'        Me._myToolbar.Strip.Invalidate()
'        Me._myStatusBar.Strip.Invalidate()
'    End Sub

'#End Region
'End Class
