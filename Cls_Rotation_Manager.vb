' Target File: Cls_Rotation_Manager.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Drawing

Public NotInheritable Class Cls_Rotation_Manager

    ''' <summary>
    ''' Computes the relative angle between the initial baseline anchor and current cursor track.
    ''' </summary>
    Public Sub ExecuteRotationStep(ByVal worldMouse As PointF, ByVal worldAnchor As PointF, ByVal shape As Cls_Base_Shape)
        If shape Is Nothing Then Exit Sub
        Dim center As PointF = shape.CalculateGeometricCenter()

        ' Calculate trigonometric angles relative to the true geometric midpoint
        Dim baseRad As Double = Math.Atan2(Convert.ToDouble(worldAnchor.Y - center.Y), Convert.ToDouble(worldAnchor.X - center.X))
        Dim currentRad As Double = Math.Atan2(Convert.ToDouble(worldMouse.Y - center.Y), Convert.ToDouble(worldMouse.X - center.X))

        ' Derive total distance angle delta gap in degrees
        Dim deltaDegrees As Double = (currentRad - baseRad) * (180.0 / Math.PI)
        Dim proposedAngle As Single = shape.RotationAngle + Convert.ToSingle(deltaDegrees)

        ' Clamp and normalize vectors strictly to our 0-360 positive degree boundary ring
        If proposedAngle < 0.0F Then proposedAngle += 360.0F
        If proposedAngle >= 360.0F Then proposedAngle -= 360.0F

        ' Snap directly to professional 45-degree layout increments
        Dim snappedAngle As Single = Convert.ToSingle(Math.Round(proposedAngle / 45.0F) * 45.0F)
        shape.RotationAngle = If(snappedAngle >= 360.0F, 0.0F, snappedAngle)
    End Sub

    ''' <summary>
    ''' Renders unrotated target ring crosshairs centered directly on the midpoint center point.
    ''' </summary>
    Public Sub DrawRotationOverlayMetrics(ByVal g As Graphics, ByVal zoom As Single, ByVal shape As Cls_Base_Shape)
        If g Is Nothing OrElse shape Is Nothing Then Exit Sub
        Dim center As PointF = shape.CalculateGeometricCenter()
        Dim radius As Single = 12.0F / zoom
        Dim crossLength As Single = radius + (6.0F / zoom)

        Using pivotPen As New Pen(Color.OrangeRed, 1.5F / zoom)
            g.DrawEllipse(pivotPen, center.X - radius, center.Y - radius, radius * 2.0F, radius * 2.0F)
            g.DrawLine(pivotPen, center.X - crossLength, center.Y, center.X + crossLength, center.Y)
            g.DrawLine(pivotPen, center.X, center.Y - crossLength, center.X, center.Y + crossLength)
        End Using

        Me.PaintRotationAngleTextString(g, zoom, center, shape.RotationAngle)
    End Sub

    Private Sub PaintRotationAngleTextString(ByVal g As Graphics, ByVal zoom As Single, ByVal center As PointF, ByVal angle As Single)
        Using textBrush As New SolidBrush(Color.OrangeRed)
            Dim textX As Single = center.X + (18.0F / zoom)
            Dim textY As Single = center.Y - (22.0F / zoom)

            g.DrawString($"{angle:F0}°", SystemFonts.DefaultFont, textBrush, textX, textY)
        End Using
    End Sub
End Class
