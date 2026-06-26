' Target File: Cls_Theme_Color_Table.vb
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable

Imports System.Drawing
Imports System.Windows.Forms

Public NotInheritable Class Cls_Theme_Color_Table
    Inherits ProfessionalColorTable

    Private ReadOnly Property Config As Cls_Configuration_Manager
        Get
            Return Cls_Configuration_Manager.Instance
        End Get
    End Property

    ' Overrides the master background container perimeter gradient boundaries
    Public Overrides ReadOnly Property ToolStripBorder As Color
        Get
            Return Config.ControlBorderColor
        End Get
    End Property
    Public Overrides ReadOnly Property ToolStripDropDownBackground As Color
        Get
            Return Config.PanelBackColor
        End Get
    End Property
    Public Overrides ReadOnly Property ImageMarginGradientBegin As Color
        Get
            Return Config.PanelBackColor
        End Get
    End Property
    Public Overrides ReadOnly Property ImageMarginGradientMiddle As Color
        Get
            Return Config.PanelBackColor
        End Get
    End Property
    Public Overrides ReadOnly Property ImageMarginGradientEnd As Color
        Get
            Return Config.PanelBackColor
        End Get
    End Property

    ' Overrides the standard strip backgrounds
    Public Overrides ReadOnly Property MenuStripGradientBegin As Color
        Get
            Return Config.PanelBackColor
        End Get
    End Property
    Public Overrides ReadOnly Property MenuStripGradientEnd As Color
        Get
            Return Config.PanelBackColor
        End Get
    End Property
    Public Overrides ReadOnly Property ToolStripGradientBegin As Color
        Get
            Return Config.PanelBackColor
        End Get
    End Property
    Public Overrides ReadOnly Property ToolStripGradientEnd As Color
        Get
            Return Config.PanelBackColor
        End Get
    End Property
    Public Overrides ReadOnly Property StatusStripGradientBegin As Color
        Get
            Return Config.PanelBackColor
        End Get
    End Property
    Public Overrides ReadOnly Property StatusStripGradientEnd As Color
        Get
            Return Config.PanelBackColor
        End Get
    End Property

    ' Overrides the selection focus highlight metrics
    Public Overrides ReadOnly Property MenuItemSelected As Color
        Get
            Return Config.ControlBorderColor
        End Get
    End Property
    Public Overrides ReadOnly Property MenuItemSelectedGradientBegin As Color
        Get
            Return Config.ControlBorderColor
        End Get
    End Property
    Public Overrides ReadOnly Property MenuItemSelectedGradientEnd As Color
        Get
            Return Config.ControlBorderColor
        End Get
    End Property
    Public Overrides ReadOnly Property MenuItemPressedGradientBegin As Color
        Get
            Return Config.PanelBackColor
        End Get
    End Property
    Public Overrides ReadOnly Property MenuItemPressedGradientEnd As Color
        Get
            Return Config.PanelBackColor
        End Get
    End Property
End Class
