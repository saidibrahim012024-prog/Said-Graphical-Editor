' Target File: Cls_Toolbar.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Said Rules Compliance: Zero Guessing | No Non-Existent Token Variables | Locked Classes Only
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Drawing
Imports System.Windows.Forms

Public NotInheritable Class Cls_Toolbar
    Private ReadOnly _strip As ToolStrip

    ' SINGLE SOURCE OF TRUTH EVENT PIPELINE
    Public Event CommandExecuted As EventHandler(Of CommandAction)

    Private _btnSnapToggle As ToolStripButton
    Private _btnGridVisibleToggle As ToolStripButton
    Private _btnInspectorToggle As ToolStripButton ' <--- RESTORED STRATEGY BACKING FIELD

#Region "Properties & Constructors"

    Public ReadOnly Property Strip() As ToolStrip
        Get
            ' BINDING ALIGNMENT: Returns the underlying strip instance explicitly
            Return Me._strip
        End Get
    End Property

    Public Sub New()
        Me._strip = New ToolStrip()
        Me.BuildToolbar()
    End Sub

#End Region

#Region "Toolbar Component Layout Generation"

    ''' <summary>
    ''' Instantiates a high-utility professional CAD toolbar control layout ribbon.
    ''' </summary>
    Private Sub BuildToolbar()
        With Me._strip
            .GripStyle = ToolStripGripStyle.Hidden
            .ShowItemToolTips = True
        End With

        Dim btnNew As New ToolStripButton("&New File", SystemIcons.Application.ToBitmap(), AddressOf Me.OnToolbarItemClick)
        btnNew.ToolTipText = "Create a New Schematic Drafting MDI Document Window"
        btnNew.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
        btnNew.Tag = CommandAction.NewDocument

        Me._strip.Items.Add(btnNew)
        Me._strip.Items.Add(New ToolStripSeparator())

        Me.InjectZoomActionControls()
        Me.AppendGridControlActions()
        Me.AppendInspectorPanelActions() ' <--- INJECT NEW STRATEGY WORKER
        Me.AppendDiagnosticsTestControl()
    End Sub

    Private Sub InjectZoomActionControls()
        Dim btnIn As New ToolStripButton("➕ Zoom In", Nothing, AddressOf Me.OnToolbarItemClick) With {.ToolTipText = "Zoom In Window Center", .Tag = CommandAction.ZoomIn}
        Dim btnOut As New ToolStripButton("➖ Zoom Out", Nothing, AddressOf Me.OnToolbarItemClick) With {.ToolTipText = "Zoom Out Window Center", .Tag = CommandAction.ZoomOut}
        Dim btnFit As New ToolStripButton("🔍 Zoom Fit", Nothing, AddressOf Me.OnToolbarItemClick) With {.ToolTipText = "Fit All Placed Gates to Screen Frame", .Tag = CommandAction.ZoomFit}

        Me._strip.Items.AddRange(New ToolStripItem() {btnIn, btnOut, btnFit})
    End Sub

    Private Sub AppendGridControlActions()
        Me._btnGridVisibleToggle = New ToolStripButton("🌐 Show Grid", Nothing, AddressOf Me.OnToolbarItemClick) With {
            .ToolTipText = "Show/Hide Active Viewport drafting grid lines", .CheckOnClick = True, .Checked = True, .DisplayStyle = ToolStripItemDisplayStyle.ImageAndText, .Tag = CommandAction.ToggleGrid
        }
        Me._btnSnapToggle = New ToolStripButton("🧲 Snap Grid", Nothing, AddressOf Me.OnToolbarItemClick) With {
            .ToolTipText = "Toggle 20px cell clamping engine tracking on/off", .CheckOnClick = True, .Checked = True, .DisplayStyle = ToolStripItemDisplayStyle.ImageAndText, .Tag = CommandAction.ToggleSnap
        }

        Me._strip.Items.AddRange(New ToolStripItem() {New ToolStripSeparator(), Me._btnGridVisibleToggle, Me._btnSnapToggle})
    End Sub

    Private Sub AppendInspectorPanelActions()
        ' OPTIMIZED BUTTON HOOK: Uses CheckOnClick to support active state synchronization
        Me._btnInspectorToggle = New ToolStripButton("📋 Inspector", Nothing, AddressOf Me.OnToolbarItemClick) With {
            .ToolTipText = "Toggle the layout visibility state of the property inspector panel",
            .CheckOnClick = True, .Checked = True, .DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
            .Tag = CommandAction.ToggleInspectorPanel
        }
        Me._strip.Items.AddRange(New ToolStripItem() {New ToolStripSeparator(), Me._btnInspectorToggle})
    End Sub

    Private Sub AppendDiagnosticsTestControl()
        Dim btnTest As New ToolStripButton("🧪 Run CAD Test", Nothing, AddressOf Me.OnToolbarItemClick) With {
            .ToolTipText = "Execute 5-Step Simulation and Sizing Matrix Test Harness", .DisplayStyle = ToolStripItemDisplayStyle.ImageAndText, .ForeColor = Color.DarkRed, .Tag = CommandAction.ExecuteStressTest
        }
        Me._strip.Items.AddRange(New ToolStripItem() {New ToolStripSeparator(), btnTest})
    End Sub

#End Region

#Region "Toolbar State Updates & Routing Interfaces"

    ''' <summary>
    ''' Decoupled action router extracting button tag objects into type-explicit commands.
    ''' </summary>
    Private Sub OnToolbarItemClick(ByVal sender As Object, ByVal e As EventArgs)
        Dim btn As ToolStripButton = TryCast(sender, ToolStripButton)

        If btn IsNot Nothing AndAlso btn.Tag IsNot Nothing Then
            Dim action As CommandAction = DirectCast(btn.Tag, CommandAction)
            RaiseEvent CommandExecuted(Me, action)
        End If
    End Sub

    ''' <summary>
    ''' Public synchronization channel to lock button checkmarks to internal system properties.
    ''' </summary>
    Public Sub SynchronizeToolbarGridStates(ByVal gridVisible As Boolean, ByVal snapEnabled As Boolean)
        If Me._btnGridVisibleToggle IsNot Nothing Then
            Me._btnGridVisibleToggle.Checked = gridVisible
        End If
        If Me._btnSnapToggle IsNot Nothing Then
            Me._btnSnapToggle.Checked = snapEnabled
        End If
    End Sub

#End Region
End Class


'Public NotInheritable Class Cls_Toolbar
'    Private ReadOnly _strip As ToolStrip



'    ' SINGLE SOURCE OF TRUTH EVENT PIPELINE
'    Public Event CommandExecuted As EventHandler(Of CommandAction)

'    Private _btnSnapToggle As ToolStripButton
'    Private _btnGridVisibleToggle As ToolStripButton

'#Region "Properties & Constructors"

'    Public ReadOnly Property Strip() As ToolStrip
'        Get
'            Return Me._strip
'        End Get
'    End Property

'    Public Sub New()
'        Me._strip = New ToolStrip()
'        Me.BuildToolbar()
'    End Sub

'#End Region

'#Region "Toolbar Component Layout Generation"

'    ''' <summary>
'    ''' Instantiates a high-utility professional CAD toolbar control layout ribbon.
'    ''' </summary>
'    Private Sub BuildToolbar()
'        With Me._strip
'            .GripStyle = ToolStripGripStyle.Hidden
'            .ShowItemToolTips = True
'        End With

'        ' Instantiate standard document management option button control
'        Dim btnNew As New ToolStripButton("&New File", SystemIcons.Application.ToBitmap(), AddressOf Me.OnToolbarItemClick)
'        btnNew.ToolTipText = "Create a New Schematic Drafting MDI Document Window"
'        btnNew.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
'        btnNew.Tag = CommandAction.NewDocument

'        Me._strip.Items.Add(btnNew)
'        Me._strip.Items.Add(New ToolStripSeparator())

'        Me.InjectZoomActionControls()
'        Me.AppendGridControlActions()
'        Me.AppendDiagnosticsTestControl()
'    End Sub

'    Private Sub InjectZoomActionControls()
'        Dim btnIn As New ToolStripButton("➕ Zoom In", Nothing, AddressOf Me.OnToolbarItemClick) With {.ToolTipText = "Zoom In Window Center", .Tag = CommandAction.ZoomIn}
'        Dim btnOut As New ToolStripButton("➖ Zoom Out", Nothing, AddressOf Me.OnToolbarItemClick) With {.ToolTipText = "Zoom Out Window Center", .Tag = CommandAction.ZoomOut}
'        Dim btnFit As New ToolStripButton("🔍 Zoom Fit", Nothing, AddressOf Me.OnToolbarItemClick) With {.ToolTipText = "Fit All Placed Gates to Screen Frame", .Tag = CommandAction.ZoomFit}

'        Me._strip.Items.AddRange(New ToolStripItem() {btnIn, btnOut, btnFit})
'    End Sub

'    Private Sub AppendGridControlActions()
'        Me._btnGridVisibleToggle = New ToolStripButton("🌐 Show Grid", Nothing, AddressOf Me.OnToolbarItemClick) With {
'            .ToolTipText = "Show/Hide Active Viewport drafting grid lines", .CheckOnClick = True, .Checked = True, .DisplayStyle = ToolStripItemDisplayStyle.ImageAndText, .Tag = CommandAction.ToggleGrid
'        }
'        Me._btnSnapToggle = New ToolStripButton("🧲 Snap Grid", Nothing, AddressOf Me.OnToolbarItemClick) With {
'            .ToolTipText = "Toggle 20px cell clamping engine tracking on/off", .CheckOnClick = True, .Checked = True, .DisplayStyle = ToolStripItemDisplayStyle.ImageAndText, .Tag = CommandAction.ToggleSnap
'        }

'        Me._strip.Items.AddRange(New ToolStripItem() {New ToolStripSeparator(), Me._btnGridVisibleToggle, Me._btnSnapToggle})
'    End Sub

'    Private Sub AppendDiagnosticsTestControl()
'        Dim btnTest As New ToolStripButton("🧪 Run CAD Test", Nothing, AddressOf Me.OnToolbarItemClick) With {
'            .ToolTipText = "Execute 5-Step Simulation and Sizing Matrix Test Harness", .DisplayStyle = ToolStripItemDisplayStyle.ImageAndText, .ForeColor = Color.DarkRed, .Tag = CommandAction.ExecuteStressTest
'        }
'        Me._strip.Items.AddRange(New ToolStripItem() {New ToolStripSeparator(), btnTest})
'    End Sub

'#End Region

'#Region "Toolbar State Updates & Routing Interfaces"

'    ''' <summary>
'    ''' Decoupled action router extracting button tag objects into type-explicit commands.
'    ''' </summary>
'    Private Sub OnToolbarItemClick(ByVal sender As Object, ByVal e As EventArgs)
'        Dim btn As ToolStripButton = TryCast(sender, ToolStripButton)

'        ' Ensure target tag structures possess valid commands before triggering parent threads
'        If btn IsNot Nothing AndAlso btn.Tag IsNot Nothing Then
'            Dim action As CommandAction = DirectCast(btn.Tag, CommandAction)
'            RaiseEvent CommandExecuted(Me, action)
'        End If
'    End Sub

'    ''' <summary>
'    ''' Public synchronization channel to lock button checkmarks to internal system properties.
'    ''' </summary>
'    Public Sub SynchronizeToolbarGridStates(ByVal gridVisible As Boolean, ByVal snapEnabled As Boolean)
'        If Me._btnGridVisibleToggle IsNot Nothing Then
'            Me._btnGridVisibleToggle.Checked = gridVisible
'        End If
'        If Me._btnSnapToggle IsNot Nothing Then
'            Me._btnSnapToggle.Checked = snapEnabled
'        End If
'    End Sub

'#End Region
'End Class

'' Target File: Cls_Toolbar.vb
'' Project Namespace: mdi_test
'' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
'' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

'Imports System
'Imports System.Drawing
'Imports System.Windows.Forms

'Public NotInheritable Class Cls_Toolbar
'    Private ReadOnly _strip As ToolStrip

'    ' SINGLE SOURCE OF TRUTH EVENT PIPELINE
'    Public Event CommandExecuted As EventHandler(Of CommandAction)

'    Private _btnSnapToggle As ToolStripButton
'    Private _btnGridVisibleToggle As ToolStripButton

'    Public ReadOnly Property Strip() As ToolStrip
'        Get
'            Return Me._strip
'        End Get
'    End Property

'    Public Sub New()
'        Me._strip = New ToolStrip()
'        Me.BuildToolbar()
'    End Sub

'    Private Sub BuildToolbar()
'        With Me._strip
'            .GripStyle = ToolStripGripStyle.Hidden
'            .ShowItemToolTips = True
'        End With

'        Dim btnNew As New ToolStripButton("&New", SystemIcons.Application.ToBitmap(), Sub(s, e) Me.RaiseCmd(CommandAction.NewMdiChild)) With {
'            .ToolTipText = "Create a New MDI Child Workplace Item (Ctrl+N)", .DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
'        }
'        Me._strip.Items.Add(btnNew)
'        Me._strip.Items.Add(New ToolStripSeparator())

'        Me.InjectZoomActionControls()
'        Me.AppendGridControlActions()
'        Me.AppendDiagnosticsTestControl()
'    End Sub

'    Private Sub InjectZoomActionControls()
'        Dim btnIn As New ToolStripButton("➕ Zoom In", Nothing, Sub(s, e) Me.RaiseCmd(CommandAction.ZoomIn)) With {.ToolTipText = "Zoom In Center"}
'        Dim btnOut As New ToolStripButton("➖ Zoom Out", Nothing, Sub(s, e) Me.RaiseCmd(CommandAction.ZoomOut)) With {.ToolTipText = "Zoom Out Center"}
'        Dim btnFit As New ToolStripButton("🔍 Zoom Fit", Nothing, Sub(s, e) Me.RaiseCmd(CommandAction.ZoomFit)) With {.ToolTipText = "Fit to Window"}

'        Me._strip.Items.AddRange(New ToolStripItem() {btnIn, btnOut, btnFit})
'    End Sub

'    Private Sub AppendGridControlActions()
'        Me._btnGridVisibleToggle = New ToolStripButton("🌐 Show Grid", Nothing, Sub(s, e) Me.RaiseCmd(CommandAction.ToggleGridVisibility)) With {
'            .ToolTipText = "Show/Hide Viewport Grid Lines", .CheckOnClick = True, .Checked = True, .DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
'        }
'        Me._btnSnapToggle = New ToolStripButton("🧲 Snap to Grid", Nothing, Sub(s, e) Me.RaiseCmd(CommandAction.ToggleSnapToGrid)) With {
'            .ToolTipText = "Toggle Cursor Snapping Accuracy", .CheckOnClick = True, .Checked = True, .DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
'        }

'        Me._strip.Items.AddRange(New ToolStripItem() {New ToolStripSeparator(), Me._btnGridVisibleToggle, Me._btnSnapToggle})
'    End Sub

'    Private Sub AppendDiagnosticsTestControl()
'        Dim btnTest As New ToolStripButton("🧪 Run CAD Test", Nothing, Sub(s, e) Me.RaiseCmd(CommandAction.RunDiagnosticsTest)) With {
'            .ToolTipText = "Execute 5-Step Boundary Matrix Test Harness", .DisplayStyle = ToolStripItemDisplayStyle.ImageAndText, .ForeColor = Color.DarkRed
'        }
'        Me._strip.Items.AddRange(New ToolStripItem() {New ToolStripSeparator(), btnTest})
'    End Sub

'    Public Sub SynchronizeToolbarGridStates(ByVal gridVisible As Boolean, ByVal snapEnabled As Boolean)
'        If Me._btnGridVisibleToggle IsNot Nothing Then Me._btnGridVisibleToggle.Checked = gridVisible
'        If Me._btnSnapToggle IsNot Nothing Then Me._btnSnapToggle.Checked = snapEnabled
'    End Sub

'    Private Sub RaiseCmd(ByVal action As CommandAction)
'        RaiseEvent CommandExecuted(Me, action)
'    End Sub
'End Class

