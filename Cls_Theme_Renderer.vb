' Target File: Cls_Theme_Renderer.vb
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable

Imports System.Drawing
Imports System.Windows.Forms

Public NotInheritable Class Cls_Theme_Renderer
    Inherits ToolStripProfessionalRenderer

    Public Sub New()
        MyBase.New(New Cls_Theme_Color_Table())
    End Sub

    ' Enforces uniform typography color management over all embedded menu text nodes
    Protected Overrides Sub OnRenderItemText(e As ToolStripItemTextRenderEventArgs)
        Dim config As Cls_Configuration_Manager = Cls_Configuration_Manager.Instance
        e.TextColor = config.PanelForeColor
        e.TextFont = config.UiFont
        MyBase.OnRenderItemText(e)
    End Sub
End Class

