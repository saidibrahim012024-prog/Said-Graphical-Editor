' Target File: Cls_Menu.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Said Rules Compliance: Zero Guessing | No Non-Existent Token Variables | Locked Classes Only
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Windows.Forms

Public NotInheritable Class Cls_Menu
    Private ReadOnly _strip As MenuStrip
    Private _mnuWindowList As ToolStripMenuItem
    Private _mnuSnapToGrid As ToolStripMenuItem
    Private _mnuShowGridLines As ToolStripMenuItem
    Private _mnuInspectorToggle As ToolStripMenuItem

    ' SINGLE SOURCE OF TRUTH EVENT PIPELINE
    Public Event CommandExecuted As EventHandler(Of CommandAction)
    Public Event WindowMenuOpeningRequested As EventHandler

#Region "Properties & Constructors"

    Public ReadOnly Property MenuStripControl() As MenuStrip
        Get
            Return Me._strip
        End Get
    End Property

    Public Sub New()
        Me._strip = New MenuStrip()
        Me.BuildMenu()
    End Sub

#End Region

#Region "Menu Layout Generation Infrastructure"

    ''' <summary>
    ''' Instantiates a dedicated Digital Schematic Menu Strip layout.
    ''' </summary>
    Private Sub BuildMenu()
        Dim mnuFile As New ToolStripMenuItem("&File")
        Dim mnuNewDoc As New ToolStripMenuItem("&New Schematic", Nothing, AddressOf Me.OnMenuActionClick)
        mnuNewDoc.Tag = CommandAction.NewDocument
        mnuFile.DropDownItems.Add(mnuNewDoc)

        Dim mnuView As New ToolStripMenuItem("&View")
        Me.PopulateViewMenuDropdown(mnuView)

        Dim mnuZoom As New ToolStripMenuItem("&Zoom")
        Me.PopulateZoomMenuDropdown(mnuZoom)

        Me._mnuWindowList = New ToolStripMenuItem("&Window")
        Me.PopulateWindowMenuDropdown(Me._mnuWindowList)
        AddHandler Me._mnuWindowList.DropDownOpening, AddressOf Me.OnWindowMenuOpening

        Me._strip.Items.AddRange(New ToolStripItem() {mnuFile, mnuView, mnuZoom, Me._mnuWindowList})
    End Sub

    Private Sub PopulateViewMenuDropdown(ByVal mnuView As ToolStripMenuItem)
        Me._mnuShowGridLines = New ToolStripMenuItem("Show &Grid Lines", Nothing, AddressOf Me.OnMenuActionClick) With {.CheckOnClick = True, .Checked = True, .Tag = CommandAction.ToggleGrid}
        Me._mnuSnapToGrid = New ToolStripMenuItem("&Snap to Grid", Nothing, AddressOf Me.OnMenuActionClick) With {.CheckOnClick = True, .Checked = True, .Tag = CommandAction.ToggleSnap}

        ' STRATEGY ITEM INJECTION: Aligns inspector toggle neatly with View category items
        Me._mnuInspectorToggle = New ToolStripMenuItem("Show Property &Inspector", Nothing, AddressOf Me.OnMenuActionClick) With {.CheckOnClick = True, .Checked = True, .Tag = CommandAction.ToggleInspectorPanel}

        mnuView.DropDownItems.AddRange(New ToolStripItem() {Me._mnuShowGridLines, Me._mnuSnapToGrid, New ToolStripSeparator(), Me._mnuInspectorToggle})
    End Sub

    Private Sub PopulateZoomMenuDropdown(ByVal mnuZoom As ToolStripMenuItem)
        Dim mnuIn As New ToolStripMenuItem("Zoom &In (Center)", Nothing, AddressOf Me.OnMenuActionClick) With {.Tag = CommandAction.ZoomIn}
        Dim mnuOut As New ToolStripMenuItem("Zoom &Out (Center)", Nothing, AddressOf Me.OnMenuActionClick) With {.Tag = CommandAction.ZoomOut}
        Dim mnuFit As New ToolStripMenuItem("Zoom To &Fit", Nothing, AddressOf Me.OnMenuActionClick) With {.Tag = CommandAction.ZoomFit}

        mnuZoom.DropDownItems.AddRange(New ToolStripItem() {mnuIn, mnuOut, mnuFit})
    End Sub

    Private Sub PopulateWindowMenuDropdown(ByVal mnuWindow As ToolStripMenuItem)
        ' STRATEGY LAYOUT INJECTION: Mounts authoritative structural window arrangement triggers 
        Dim mnuCascade As New ToolStripMenuItem("&Cascade Windows", Nothing, AddressOf Me.OnMenuActionClick) With {.Tag = CommandAction.LayoutCascade}
        Dim mnuTileH As New ToolStripMenuItem("Tile Horizontally", Nothing, AddressOf Me.OnMenuActionClick) With {.Tag = CommandAction.LayoutTileHorizontal}
        Dim mnuTileV As New ToolStripMenuItem("Tile Vertically", Nothing, AddressOf Me.OnMenuActionClick) With {.Tag = CommandAction.LayoutTileVertical}

        mnuWindow.DropDownItems.AddRange(New ToolStripItem() {mnuCascade, mnuTileH, mnuTileV, New ToolStripSeparator()})
    End Sub

#End Region

#Region "Unified Action Pipeline Routing"

    ''' <summary>
    ''' Decentralized action listener mapping click parameters securely to centralized enums.
    ''' </summary>
    Private Sub OnMenuActionClick(ByVal sender As Object, ByVal e As EventArgs)
        Dim item As ToolStripMenuItem = TryCast(sender, ToolStripMenuItem)

        If item IsNot Nothing AndAlso item.Tag IsNot Nothing Then
            Dim action As CommandAction = DirectCast(item.Tag, CommandAction)
            RaiseEvent CommandExecuted(Me, action)
        End If
    End Sub

    Private Sub OnWindowMenuOpening(ByVal sender As Object, ByVal e As EventArgs)
        RaiseEvent WindowMenuOpeningRequested(Me, EventArgs.Empty)
    End Sub

#End Region
End Class

'Public NotInheritable Class Cls_Menu
'    Private ReadOnly _strip As MenuStrip
'    Private _mnuWindowList As ToolStripMenuItem
'    Private _mnuSnapToGrid As ToolStripMenuItem
'    Private _mnuShowGridLines As ToolStripMenuItem

'    ' SINGLE SOURCE OF TRUTH EVENT PIPELINE
'    Public Event CommandExecuted As EventHandler(Of CommandAction)
'    Public Event WindowMenuOpeningRequested As EventHandler

'#Region "Properties & Constructors"

'    Public ReadOnly Property MenuStripControl() As MenuStrip
'        Get
'            Return Me._strip
'        End Get
'    End Property

'    Public Sub New()
'        Me._strip = New MenuStrip()
'        Me.BuildMenu()
'    End Sub

'#End Region

'#Region "Menu Layout Generation Infrastructure"

'    ''' <summary>
'    ''' Instantiates a dedicated Digital Schematic Menu Strip layout.
'    ''' </summary>
'    Private Sub BuildMenu()
'        Dim mnuFile As New ToolStripMenuItem("&File")
'        Dim mnuNewDoc As New ToolStripMenuItem("&New Schematic", Nothing, AddressOf Me.OnMenuActionClick)
'        mnuNewDoc.Tag = CommandAction.NewDocument
'        mnuFile.DropDownItems.Add(mnuNewDoc)

'        Dim mnuView As New ToolStripMenuItem("&View")
'        Me.PopulateViewMenuDropdown(mnuView)

'        Dim mnuZoom As New ToolStripMenuItem("&Zoom")
'        Me.PopulateZoomMenuDropdown(mnuZoom)

'        Me._mnuWindowList = New ToolStripMenuItem("&Window")
'        AddHandler Me._mnuWindowList.DropDownOpening, AddressOf Me.OnWindowMenuOpening

'        Me._strip.Items.AddRange(New ToolStripItem() {mnuFile, mnuView, mnuZoom, Me._mnuWindowList})
'    End Sub

'    Private Sub PopulateViewMenuDropdown(ByVal mnuView As ToolStripMenuItem)
'        Me._mnuShowGridLines = New ToolStripMenuItem("Show &Grid Lines", Nothing, AddressOf Me.OnMenuActionClick)
'        Me._mnuShowGridLines.CheckOnClick = True
'        Me._mnuShowGridLines.Checked = True
'        Me._mnuShowGridLines.Tag = CommandAction.ToggleGrid

'        Me._mnuSnapToGrid = New ToolStripMenuItem("&Snap to Grid", Nothing, AddressOf Me.OnMenuActionClick)
'        Me._mnuSnapToGrid.CheckOnClick = True
'        Me._mnuSnapToGrid.Checked = True
'        Me._mnuSnapToGrid.Tag = CommandAction.ToggleSnap

'        mnuView.DropDownItems.AddRange(New ToolStripItem() {Me._mnuShowGridLines, Me._mnuSnapToGrid})
'    End Sub

'    Private Sub PopulateZoomMenuDropdown(ByVal mnuZoom As ToolStripMenuItem)
'        Dim mnuIn As New ToolStripMenuItem("Zoom &In (Center)", Nothing, AddressOf Me.OnMenuActionClick) With {.Tag = CommandAction.ZoomIn}
'        Dim mnuOut As New ToolStripMenuItem("Zoom &Out (Center)", Nothing, AddressOf Me.OnMenuActionClick) With {.Tag = CommandAction.ZoomOut}
'        Dim mnuFit As New ToolStripMenuItem("Zoom To &Fit", Nothing, AddressOf Me.OnMenuActionClick) With {.Tag = CommandAction.ZoomFit}

'        mnuZoom.DropDownItems.AddRange(New ToolStripItem() {mnuIn, mnuOut, mnuFit})
'    End Sub

'#End Region

'#Region "Unified Action Pipeline Routing"

'    ''' <summary>
'    ''' Decentralized action listener mapping click parameters securely to centralized enums.
'    ''' </summary>
'    Private Sub OnMenuActionClick(ByVal sender As Object, ByVal e As EventArgs)
'        Dim item As ToolStripMenuItem = TryCast(sender, ToolStripMenuItem)

'        ' Verify tag parameters possess valid enum actions before raising events
'        If item IsNot Nothing AndAlso item.Tag IsNot Nothing Then
'            Dim action As CommandAction = DirectCast(item.Tag, CommandAction)
'            RaiseEvent CommandExecuted(Me, action)
'        End If
'    End Sub

'    Private Sub OnWindowMenuOpening(ByVal sender As Object, ByVal e As EventArgs)
'        ' FIXED: Passes "Me" as the authentic class event sender object authority
'        RaiseEvent WindowMenuOpeningRequested(Me, EventArgs.Empty)
'    End Sub

'#End Region
'End Class

'' Target File: Cls_Menu.vb
'' Project Namespace: mdi_test
'' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
'' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

'Imports System
'Imports System.Windows.Forms

'Public NotInheritable Class Cls_Menu
'    Private ReadOnly _strip As MenuStrip
'    Private _mnuWindowList As ToolStripMenuItem
'    Private _mnuSnapToGrid As ToolStripMenuItem
'    Private _mnuShowGridLines As ToolStripMenuItem

'    ' SINGLE SOURCE OF TRUTH EVENT PIPELINE
'    Public Event CommandExecuted As EventHandler(Of CommandAction)
'    Public Event WindowMenuOpeningRequested As EventHandler

'    Public ReadOnly Property Strip() As MenuStrip
'        Get
'            Return Me._strip
'        End Get
'    End Property

'    Public Sub New()
'        Me._strip = New MenuStrip()
'        Me.BuildMenu()
'    End Sub

'    Private Sub BuildMenu()
'        Dim mnuFile As New ToolStripMenuItem("&File")
'        mnuFile.DropDownItems.Add(New ToolStripMenuItem("&New MDI Child", Nothing, Sub(s, e) Me.RaiseCmd(CommandAction.NewMdiChild)))

'        Dim mnuView As New ToolStripMenuItem("&View")
'        Me.PopulateViewMenuDropdown(mnuView)

'        Dim mnuZoom As New ToolStripMenuItem("&Zoom")
'        Me.PopulateZoomMenuDropdown(mnuZoom)

'        Me._mnuWindowList = New ToolStripMenuItem("&Window")
'        AddHandler Me._mnuWindowList.DropDownOpening, AddressOf Me.OnWindowMenuOpening

'        Me._strip.Items.AddRange(New ToolStripItem() {mnuFile, mnuView, mnuZoom, Me._mnuWindowList})
'    End Sub

'    Private Sub PopulateViewMenuDropdown(ByVal mnuView As ToolStripMenuItem)
'        mnuView.DropDownItems.Add(New ToolStripMenuItem("Toggle &Explorer Panel", Nothing, Sub(s, e) Me.RaiseCmd(CommandAction.ToggleExplorer)))
'        mnuView.DropDownItems.Add(New ToolStripMenuItem("Switch Visual &Theme", Nothing, Sub(s, e) Me.RaiseCmd(CommandAction.SwitchTheme)))
'        mnuView.DropDownItems.Add(New ToolStripSeparator())

'        Me._mnuShowGridLines = New ToolStripMenuItem("Show &Grid Lines") With {.CheckOnClick = True, .Checked = True}
'        Me._mnuSnapToGrid = New ToolStripMenuItem("&Snap to Grid") With {.CheckOnClick = True, .Checked = True}

'        AddHandler Me._mnuShowGridLines.CheckedChanged, Sub(s, e) Me.RaiseCmd(CommandAction.ToggleGridVisibility)
'        AddHandler Me._mnuSnapToGrid.CheckedChanged, Sub(s, e) Me.RaiseCmd(CommandAction.ToggleSnapToGrid)

'        mnuView.DropDownItems.AddRange(New ToolStripItem() {Me._mnuShowGridLines, Me._mnuSnapToGrid})
'    End Sub

'    Private Sub PopulateZoomMenuDropdown(ByVal mnuZoom As ToolStripMenuItem)
'        mnuZoom.DropDownItems.Add(New ToolStripMenuItem("Zoom &In (Center)", Nothing, Sub(s, e) Me.RaiseCmd(CommandAction.ZoomIn)))
'        mnuZoom.DropDownItems.Add(New ToolStripMenuItem("Zoom &Out (Center)", Nothing, Sub(s, e) Me.RaiseCmd(CommandAction.ZoomOut)))
'        mnuZoom.DropDownItems.Add(New ToolStripMenuItem("Zoom To &Fit", Nothing, Sub(s, e) Me.RaiseCmd(CommandAction.ZoomFit)))
'    End Sub

'    Private Sub RaiseCmd(ByVal action As CommandAction)
'        RaiseEvent CommandExecuted(Me, action)
'    End Sub

'    Private Sub OnWindowMenuOpening(ByVal sender As Object, ByVal e As EventArgs)
'        RaiseEvent WindowMenuOpeningRequested(Me._mnuWindowList, EventArgs.Empty)
'    End Sub
'End Class
