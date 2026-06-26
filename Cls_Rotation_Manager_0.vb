' Target File: Cls_Rotation_Manager.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Drawing
Imports System.Windows.Forms

Public NotInheritable Class Cls_Rotation_Manager

    ' Target File: Cls_Rotation_Manager.vb (Strategy Aligned Angular Computation)
    ' Project Namespace: mdi_test
    ' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.



    ''' <summary>
    ''' Computes the exact relative angular delta from the initial click anchor centered on the shape.
    ''' </summary>
    Public Sub ExecuteRotationStep(ByVal worldMouse As PointF, ByVal worldAnchor As PointF, ByVal shape As Cls_Base_Shape)
            If shape Is Nothing Then Exit Sub
            Dim center As PointF = shape.CalculateGeometricCenter()

            ' 1. Derive trigonometry vectors relative to the shape center anchor point
            Dim baseRad As Double = Math.Atan2(Convert.ToDouble(worldAnchor.Y - center.Y), Convert.ToDouble(worldAnchor.X - center.X))
            Dim currentRad As Double = Math.Atan2(Convert.ToDouble(worldMouse.Y - center.Y), Convert.ToDouble(worldMouse.X - center.X))

            ' 2. Calculate the raw distance angle gap in absolute degrees
            Dim deltaDegrees As Double = (currentRad - baseRad) * (180.0 / Math.PI)
            Dim proposedAngle As Single = shape.RotationAngle + Convert.ToSingle(deltaDegrees)

            ' Normalize within our strict 0-360 positive degree bounding ring
            If proposedAngle < 0.0F Then proposedAngle += 360.0F
            If proposedAngle >= 360.0F Then proposedAngle -= 360.0F

            ' 3. Clamp precisely to professional 45-degree schematic increments
            Dim snappedAngle As Single = Convert.ToSingle(Math.Round(proposedAngle / 45.0F) * 45.0F)
            shape.RotationAngle = If(snappedAngle >= 360.0F, 0.0F, snappedAngle)
        End Sub



    '''' <summary>
    '''' Computes the exact relative angular delta from the initial click anchor and logs telemetry.
    '''' </summary>
    'Public Sub ExecuteRotationStep(ByVal worldMouse As PointF, ByVal worldAnchor As PointF, ByVal shape As Cls_Base_Shape)
    '    If shape Is Nothing Then Exit Sub
    '    Dim center As PointF = shape.CalculateGeometricCenter()

    '    ' 1. Derive trigonometry vectors relative to the shape center anchor point
    '    Dim baseRad As Double = Math.Atan2(Convert.ToDouble(worldAnchor.Y - center.Y), Convert.ToDouble(worldAnchor.X - center.X))
    '    Dim currentRad As Double = Math.Atan2(Convert.ToDouble(worldMouse.Y - center.Y), Convert.ToDouble(worldMouse.X - center.X))

    '    ' 2. Calculate the raw distance angle gap in absolute degrees
    '    Dim deltaDegrees As Double = (currentRad - baseRad) * (180.0 / Math.PI)

    '    ' 3. Round cleanly to the nearest 45-degree layout intersection lane
    '    Dim snappedDelta As Single = Convert.ToSingle(Math.Round(deltaDegrees / 45.0) * 45.0)

    '    Diagnostics.Debug.WriteLine($" -> Math: Raw Delta={deltaDegrees:F1}°, Snapped Delta={snappedDelta:F1}° | Base Angle: {shape.RotationAngle:F1}°")

    '    If snappedDelta <> 0.0F Then
    '        Dim nextAngle As Single = shape.RotationAngle + snappedDelta

    '        ' Normalize within our strict 0-360 positive degree bounding ring
    '        If nextAngle < 0.0F Then nextAngle += 360.0F
    '        If nextAngle >= 360.0F Then nextAngle -= 360.0F

    '        shape.RotationAngle = nextAngle
    '        Diagnostics.Debug.WriteLine($"[SUCCESS] Committed Shape Rotation Angle = {shape.RotationAngle:F1}°")
    '    End If
    'End Sub

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
        End Sub



    '''' <summary>
    '''' Computes the exact relative angular delta from the initial click anchor centered on the shape.
    '''' STRATEGY ALIGNED: Free from nested local coordinate matrix mutations.
    '''' </summary>
    'Public Sub ExecuteRotationStep(ByVal worldMouse As PointF, ByVal worldAnchor As PointF, ByVal shape As Cls_Base_Shape)
    '        If shape Is Nothing Then Exit Sub
    '        Dim center As PointF = shape.CalculateGeometricCenter()

    '        ' 1. Derive the absolute base angle from the initial drag click landing position
    '        Dim baseRad As Double = Math.Atan2(Convert.ToDouble(worldAnchor.Y - center.Y), Convert.ToDouble(worldAnchor.X - center.X))

    '        ' 2. Derive the current angle relative to the active cursor track position
    '        Dim currentRad As Double = Math.Atan2(Convert.ToDouble(worldMouse.Y - center.Y), Convert.ToDouble(worldMouse.X - center.X))

    '        ' 3. Calculate the true vector distance gap difference in degrees
    '        Dim deltaDegrees As Double = (currentRad - baseRad) * (180.0 / Math.PI)
    '        Dim proposedAngle As Single = shape.RotationAngle + Convert.ToSingle(deltaDegrees)

    '        ' Normalize tracking bounds to maintain a strict positive 0-360 system loop
    '        If proposedAngle < 0.0F Then proposedAngle += 360.0F
    '        If proposedAngle >= 360.0F Then proposedAngle -= 360.0F

    '        ' 4. Clamp precisely to professional 45-degree schematic increments
    '        Dim snappedAngle As Single = Convert.ToSingle(Math.Round(proposedAngle / 45.0F) * 45.0F)
    '        shape.RotationAngle = If(snappedAngle >= 360.0F, 0.0F, snappedAngle)
    '    End Sub

    'Public Sub DrawRotationOverlayMetrics(ByVal g As Graphics, ByVal zoom As Single, ByVal shape As Cls_Base_Shape)
    '            If g Is Nothing OrElse shape Is Nothing Then Exit Sub
    '            Dim center As PointF = shape.CalculateGeometricCenter()
    '            Dim radius As Single = 12.0F / zoom
    '            Dim crossLength As Single = radius + (6.0F / zoom)

    '            Using pivotPen As New Pen(Color.OrangeRed, 1.5F / zoom)
    '                g.DrawEllipse(pivotPen, center.X - radius, center.Y - radius, radius * 2.0F, radius * 2.0F)
    '                g.DrawLine(pivotPen, center.X - crossLength, center.Y, center.X + crossLength, center.Y)
    '                g.DrawLine(pivotPen, center.X, center.Y - crossLength, center.X, center.Y + crossLength)
    '            End Using
    '        End Sub


    Private _pivotCenter As PointF
    Private _currentAngle As Single
    Private _isRotating As Boolean = False

    Public ReadOnly Property IsRotating As Boolean
        Get
            Return _isRotating
        End Get
    End Property
    Public ReadOnly Property PivotCenter As PointF
        Get
            Return _pivotCenter
        End Get
    End Property
    Public ReadOnly Property CurrentAngle As Single
        Get
            Return _currentAngle
        End Get
    End Property

    ''' <summary>
    ''' Computes trigonometric angle differences relative to the shape center vector.
    ''' </summary>
    Public Sub ExecuteRotationStep(ByVal worldMouse As PointF, ByVal shape As Cls_Base_Shape)
        If shape Is Nothing Then Exit Sub

        Dim center As PointF = shape.CalculateGeometricCenter()

        ' Perform double-precision arc-tangent coordinate delta tracking
        Dim deltaX As Double = Convert.ToDouble(worldMouse.X - center.X)
        Dim deltaY As Double = Convert.ToDouble(worldMouse.Y - center.Y)

        Dim radians As Double = Math.Atan2(deltaY, deltaX)
        Dim degrees As Double = radians * (180.0 / Math.PI)

        ' Clamp the calculated angular metrics to positive 360-degree boundary systems
        If degrees < 0.0 Then
            degrees += 360.0
        End If

        ' Snap directly to 45-degree rotation increments to keep schematic layouts square
        Dim snappedAngle As Single = Convert.ToSingle(Math.Round(degrees / 45.0) * 45.0)
        shape.RotationAngle = If(snappedAngle >= 360.0F, 0.0F, snappedAngle)
    End Sub
    'zzzzzzzzzzzzzzz




    '''' <summary>
    '''' Paints rotation tracker rings and string text metrics centered on the shape's raw boundaries.
    '''' </summary>
    'Public Sub DrawRotationOverlayMetrics(ByVal g As Graphics, ByVal zoom As Single, ByVal shape As Cls_Base_Shape)
    '    If g Is Nothing OrElse shape Is Nothing Then Exit Sub

    '    ' 1. Calculate the true world center point directly from the raw unrotated layout envelope bounds
    '    Dim center As PointF = shape.CalculateGeometricCenter()

    '    ' 2. Establish zoom-invariant vector sizing metrics
    '    Dim radius As Single = 12.0F / zoom
    '    Dim crossLength As Single = radius + (6.0F / zoom)

    '    Using pivotPen As New Pen(Color.OrangeRed, 1.5F / zoom)
    '        ' Draw the crosshair target right on top of the raw shape center point
    '        g.DrawEllipse(pivotPen, center.X - radius, center.Y - radius, radius * 2.0F, radius * 2.0F)
    '        g.DrawLine(pivotPen, center.X - crossLength, center.Y, center.X + crossLength, center.Y)
    '        g.DrawLine(pivotPen, center.X, center.Y - crossLength, center.X, center.Y + crossLength)
    '    End Using

    '    ' 3. Render real-time angle text strings offset safely from the center vector point
    '    Me.PaintRotationAngleTextString(g, zoom, center, shape.RotationAngle)
    'End Sub

    Private Sub PaintRotationAngleTextString(ByVal g As Graphics, ByVal zoom As Single, ByVal center As PointF, ByVal angle As Single)
        Using textBrush As New SolidBrush(Color.OrangeRed)
            ' Draw the text label string close to the shape center footprint
            Dim textX As Single = center.X + (18.0F / zoom)
            Dim textY As Single = center.Y - (22.0F / zoom)

            ' Use the ambient container formatting rules to output the text string representation
            g.DrawString($"{angle:F0}°", SystemFonts.DefaultFont, textBrush, textX, textY)
        End Using
    End Sub
    'xxxxxxxxxxxxxxxx




    'Public ReadOnly Property TargetShape As Cls_Base_Shape
    '    Get
    '        Return _targetShape
    '    End Get
    'End Property
    Public Sub StartRotation(shape As Cls_Base_Shape)
        If shape Is Nothing Then Exit Sub
        _isRotating = True
        _pivotCenter = shape.CalculateGeometricCenter()
        _currentAngle = shape.RotationAngle
    End Sub

    Public Sub TerminateRotation()
        _isRotating = False
    End Sub
    ' Target File: Cls_Rotation_Manager.vb
    ' Project Namespace: mdi_test
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    ''' <summary>
    ''' Computes the spatial angular delta from the geometric center to the world cursor point,
    ''' updating the shape's model property natively.
    ''' </summary>
    Public Sub ProcessRotation(worldMousePos As PointF, shape As Cls_Base_Shape, statusBar As Cls_StatusBar)
        If Not _isRotating OrElse shape Is Nothing Then Exit Sub

        ' 1. Pull the static pivot center right from the shape model's geometric center
        Dim center As PointF = shape.CalculateGeometricCenter()

        ' 2. Calculate the raw arc tangent delta vectors relative to the center origin
        Dim dx As Double = worldMousePos.X - center.X
        Dim dy As Double = worldMousePos.Y - center.Y

        ' 3. Extract the clean degree calculation value and normalize it safely
        Dim calculatedAngle As Single = CSng(Math.Atan2(dy, dx) * 180.0 / Math.PI)
        If calculatedAngle < 0.0F Then calculatedAngle += 360.0F

        ' 4. FIXED: Assign the final computed angle straight back to the shape model property natively
        _currentAngle = calculatedAngle
        shape.RotationAngle = calculatedAngle
    End Sub

    'Public Sub ProcessRotation(worldMousePos As PointF, statusBar As Cls_StatusBar)
    '    If Not _isRotating Then Exit Sub
    '    ' Calculate the delta angle from the pivot center to the cursor position
    '    Dim dx As Double = worldMousePos.X - _pivotCenter.X
    '    Dim dy As Double = worldMousePos.Y - _pivotCenter.Y
    '    _currentAngle = CSng(Math.Atan2(dy, dx) * 180.0 / Math.PI)
    '    If _currentAngle < 0.0F Then _currentAngle += 360.0F
    'End Sub

    '''' <summary>
    '''' Draws the rotation tracking indicators natively inside a pre-transformed graphics matrix context.
    '''' </summary>
    'Public Sub DrawRotationOverlayMetrics(g As Graphics, zoom As Single, shape As Cls_Base_Shape)
    '    If shape Is Nothing Then Exit Sub
    '    Dim config As Cls_Configuration_Manager = Cls_Configuration_Manager.Instance

    '    ' FIXED: Center the crosshair directly at the local center, since the matrix handles the rotation angle
    '    Dim center As PointF = shape.CalculateGeometricCenter()
    '    Using pivotPen As New Pen(Color.OrangeRed, 1.5! / zoom)
    '        Dim r As Single = 8.0F / zoom
    '        g.DrawEllipse(pivotPen, center.X - r, center.Y - r, r * 2.0F, r * 2.0F)
    '        g.DrawLine(pivotPen, center.X - r - 4.0F / zoom, center.Y, center.X + r + 4.0F / zoom, center.Y)
    '        g.DrawLine(pivotPen, center.X, center.Y - r - 4.0F / zoom, center.X, center.Y + r + 4.0F / zoom)
    '    End Using

    '    Using textBrush As New SolidBrush(Color.OrangeRed)
    '        g.DrawString($"{shape.RotationAngle:F1}°", config.UiFont, textBrush, New PointF(center.X + (15.0F / zoom), center.Y - (25.0F / zoom)))
    '    End Using
    'End Sub


End Class


