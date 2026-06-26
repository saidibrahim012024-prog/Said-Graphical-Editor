' Target File: Cls_Configuration_Manager.vb
' Project Namespace: New_Said_Graphics_Library
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable

Imports System
Imports System.Drawing
Imports System.Windows.Forms

Public NotInheritable Class Cls_Configuration_Manager

    ' Thread-safe Singleton Instance Initialization
    Private Shared ReadOnly _instance As New Lazy(Of Cls_Configuration_Manager)(Function() New Cls_Configuration_Manager())

    ' Private Theme Backing Fields
    Private _currentTheme As VisualTheme = VisualTheme.Light
    Private _panelBackColor As Color
    Private _panelForeColor As Color
    Private _controlBorderColor As Color
    Private _headerFont As Font
    Private _uiFont As Font

    ' Public Structural Properties
    Public Shared ReadOnly Property Instance As Cls_Configuration_Manager
        Get
            Return _instance.Value
        End Get
    End Property

    Public ReadOnly Property PanelBackColor As Color
        Get
            Return _panelBackColor
        End Get
    End Property
    Public ReadOnly Property PanelForeColor As Color
        Get
            Return _panelForeColor
        End Get
    End Property
    Public ReadOnly Property ControlBorderColor As Color
        Get
            Return _controlBorderColor
        End Get
    End Property
    Public ReadOnly Property HeaderFont As Font
        Get
            Return _headerFont
        End Get
    End Property
    Public ReadOnly Property UiFont As Font
        Get
            Return _uiFont
        End Get
    End Property
    Public ReadOnly Property CurrentTheme As VisualTheme
        Get
            Return _currentTheme
        End Get
    End Property

    ' Supported Layout Themes Enumeration
    Public Enum VisualTheme
        Light
        Dark
    End Enum

    ' Enforce Private Constructor for Singleton Pattern Integrity
    Private Sub New()
        InitializeThemeProfiles()
    End Sub

    ' Defines and loads the layout specifications
    Public Sub ApplyTheme(theme As VisualTheme)
        _currentTheme = theme
        InitializeThemeProfiles()
    End Sub

    Private Sub InitializeThemeProfiles()
        ' Enforce system-wide typography settings
        _headerFont = New Font("Segoe UI", 9.0!, FontStyle.Bold)
        _uiFont = New Font("Segoe UI", 9.0!, FontStyle.Regular)

        ' Process conditional theme color configurations
        If _currentTheme = VisualTheme.Dark Then
            _panelBackColor = Color.FromArgb(45, 45, 48)
            _panelForeColor = Color.FromArgb(241, 241, 241)
            _controlBorderColor = Color.FromArgb(63, 63, 70)
        Else
            _panelBackColor = SystemColors.ControlLight
            _panelForeColor = SystemColors.ControlText
            _controlBorderColor = Color.DarkGray
        End If
    End Sub

End Class

