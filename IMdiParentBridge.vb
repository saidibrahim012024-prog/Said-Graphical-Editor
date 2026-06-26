' Target File: IMdiParentBridge.vb
Imports System.Windows.Forms

Public Interface IMdiParentBridge



    ' ReadOnly Property Explorer() As Cls_Explorer
    ReadOnly Property WorkspaceLayout As Cls_Layout
    ReadOnly Property ActiveMdiForm As Form
    ReadOnly Property MdiChildrenForms As Form()

    ' FIXED: Expose the central action controller handle to the global interface contract
    ReadOnly Property MdiAction As Cls_MDI_Action


    Sub ActivateMdiChildForm(child As Form)

    Property IsSnapToGridEnabled() As Boolean
    Sub RegisterMdiChild(ByVal child As Form)
End Interface



