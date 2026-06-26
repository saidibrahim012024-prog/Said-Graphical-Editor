' Target File: Cls_Grid_Manager.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Drawing

Public NotInheritable Class Cls_Grid_Manager
    Private _isVisible As Boolean = True
    Private _isSnapEnabled As Boolean = True

#Region "Properties"

    Public Property IsVisible() As Boolean
        Get
            Return Me._isVisible
        End Get
        Set(ByVal value As Boolean)
            Me._isVisible = value
        End Set
    End Property

    Public Property IsSnapEnabled() As Boolean
        Get
            Return Me._isSnapEnabled
        End Get
        Set(ByVal value As Boolean)
            Me._isSnapEnabled = value
        End Set
    End Property

#End Region

    Public Function SnapPoint(ByVal rawWorldPoint As PointF) As PointF
        ' Instantly pass original vectors through if snapping checks are bypassed
        If Not Me._isSnapEnabled Then Return rawWorldPoint

        Dim snappedX As Single = Convert.ToSingle(Math.Round(Convert.ToDouble(rawWorldPoint.X) / 20.0) * 20.0)
        Dim snappedY As Single = Convert.ToSingle(Math.Round(Convert.ToDouble(rawWorldPoint.Y) / 20.0) * 20.0)
        Return New PointF(snappedX, snappedY)
    End Function

    Public Sub Render(ByVal g As Graphics, ByVal zoom As Single, ByVal visibleBounds As RectangleF)
        If g Is Nothing OrElse Not Me._isVisible Then Exit Sub

        Dim gridSpacing As Single = 20.0F
        Dim startX As Single = Convert.ToSingle(Math.Floor(visibleBounds.Left / gridSpacing) * gridSpacing)
        Dim startY As Single = Convert.ToSingle(Math.Floor(visibleBounds.Top / gridSpacing) * gridSpacing)

        Using gridPen As New Pen(Color.FromArgb(50, 50, 50), 1.0F / zoom)
            ' Draw complete vertical cell lines across view boundaries
            For x As Single = startX To visibleBounds.Right Step gridSpacing
                g.DrawLine(gridPen, x, visibleBounds.Top, x, visibleBounds.Bottom)
            Next
            ' Draw complete horizontal cell lines across view boundaries
            For y As Single = startY To visibleBounds.Bottom Step gridSpacing
                g.DrawLine(gridPen, visibleBounds.Left, y, visibleBounds.Right, y)
            Next
        End Using
    End Sub
End Class
