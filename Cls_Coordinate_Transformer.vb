' Target File: Cls_Coordinate_Transformer.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Drawing
Imports System.Drawing.Drawing2D

Public NotInheritable Class Cls_Coordinate_Transformer

    ' Target File: Cls_Coordinate_Transformer.vb (High-Precision Coordinate Projection)
    ' Explicit configuration locked for file scope | Option Strict On | Explicit Binding
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    ''' <summary>
    ''' Projects world space coordinate points back into a shape's localized, center-relative space.
    ''' FIXED: Applies vector translations manually to prevent world-to-local coordinate blackouts.
    ''' </summary>
    Public Function TransformWorldToLocalShapeSpace(ByVal worldPoint As PointF, ByVal shape As Cls_Base_Shape) As PointF
        If shape Is Nothing Then Return worldPoint

        ' 1. Translate the raw world point to be relative to the shape's geometric center (Origin 0,0)
        Dim center As PointF = shape.CalculateGeometricCenter()
        Dim localX As Double = Convert.ToDouble(worldPoint.X) - Convert.ToDouble(center.X)
        Dim localY As Double = Convert.ToDouble(worldPoint.Y) - Convert.ToDouble(center.Y)

        ' 2. If an orientation angle exists, apply the inverse trigonometric rotation matrix pass
        If shape.RotationAngle <> 0.0F Then
            Dim rad As Double = -Convert.ToDouble(shape.RotationAngle) * (Math.PI / 180.0)
            Dim cosTheta As Double = Math.Cos(rad)
            Dim sinTheta As Double = Math.Sin(rad)

            Dim rotatedX As Double = (localX * cosTheta) - (localY * sinTheta)
            Dim rotatedY As Double = (localX * sinTheta) + (localY * cosTheta)

            localX = rotatedX : localY = rotatedY
        End If

        ' Return the crisp, type-safe projected coordinate local point vector
        Return New PointF(Convert.ToSingle(localX), Convert.ToSingle(localY))
    End Function


    ' Target File: Cls_Coordinate_Transformer.vb (High-Precision Local Space Mapping Pass)
    ' Explicit configuration locked for file scope | Option Strict On | Explicit Binding
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    '''' <summary>
    '''' Projects raw world space coordinate points back into a shape's localized, unrotated space.
    '''' FIXED: Factors in camera pan matrix offsets dynamically to prevent panning-induced coordinate drift.
    '''' </summary>
    'Public Function TransformWorldToLocalShapeSpace(ByVal worldPoint As PointF, ByVal shape As Cls_Base_Shape) As PointF
    '    If shape Is Nothing Then Return worldPoint

    '    ' Build a fresh local matrix instance layer to perform coordinate projections
    '    Using matrix As New Matrix()
    '        ' Perform the inverse rotation relative to the shape's calculated world center
    '        If shape.RotationAngle <> 0.0F Then
    '            matrix.RotateAt(-shape.RotationAngle, shape.CalculateGeometricCenter(), MatrixOrder.Prepend)
    '        End If

    '        Dim points As PointF() = {worldPoint}
    '        matrix.TransformPoints(points)

    '        ' Return the crisp, unrotated coordinate vector isolated to the local origin space
    '        Return points(0)
    '    End Using
    'End Function



    Public Function CreateCameraMatrix(panOffset As PointF, zoomFactor As Single) As Matrix
        Dim matrix As New Matrix()
        matrix.Translate(panOffset.X, panOffset.Y, MatrixOrder.Prepend)
        matrix.Scale(zoomFactor, zoomFactor, MatrixOrder.Prepend)
        Return matrix
    End Function

    ' Target File: Cls_Coordinate_Transformer.vb (Refactored Matrix Assembly Pass)
    ' Explicit configuration locked for file scope | Option Strict On | Explicit Binding
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    ''' <summary>
    ''' Builds an isolated, high-precision transformation matrix tailored for a single component.
    ''' FIXED: Explicitly appends shape position center vectors to prevent DX/DY zero blackouts.
    ''' </summary>
    Public Function CreateLocalShapeMatrix(ByVal panOffset As PointF, ByVal zoomFactor As Single, ByVal shapeAngle As Single, ByVal shapeCenter As PointF) As Matrix
        ' 1. Start with a fresh, clean matrix footprint layout container
        Dim localMatrix As New Matrix()

        ' 2. FIRST: Shift by global panning offsets and current zoom factor rules
        localMatrix.Translate(panOffset.X, panOffset.Y, MatrixOrder.Append)
        localMatrix.Scale(zoomFactor, zoomFactor, MatrixOrder.Append)

        ' 3. FIXED: Explicitly translate the matrix plane straight to the gate's world position center coordinate!
        localMatrix.Translate(shapeCenter.X, shapeCenter.Y, MatrixOrder.Append)

        ' 4. Pivot the canvas plane by the custom orientation angle parameters if needed
        If shapeAngle <> 0.0F Then
            localMatrix.Rotate(shapeAngle, MatrixOrder.Append)
        End If

        Return localMatrix
    End Function

    'Public Function CreateLocalShapeMatrix(panOffset As PointF, zoomFactor As Single, shapeAngle As Single, shapeCenter As PointF) As Matrix
    '    Dim localMatrix As Matrix = CreateCameraMatrix(panOffset, zoomFactor)
    '    localMatrix.RotateAt(shapeAngle, shapeCenter, MatrixOrder.Prepend)
    '    Return localMatrix
    'End Function

    Public Function TransformScreenToWorld(screenPoint As Point, panOffset As PointF, zoomFactor As Single) As PointF
        Using matrix As Matrix = CreateCameraMatrix(panOffset, zoomFactor)
            If matrix.IsInvertible Then
                matrix.Invert()
                Dim points As PointF() = {New PointF(screenPoint.X, screenPoint.Y)}
                matrix.TransformPoints(points)
                Return points(0)
            End If
            Return New PointF(screenPoint.X, screenPoint.Y)
        End Using
    End Function

    ' FIXED: Isolated view boundary calculator offloaded straight to the central authority
    Public Function CalculateWorldBounds(panOffset As PointF, zoomFactor As Single, viewWidth As Integer, viewHeight As Integer) As RectangleF
        Using matrix As Matrix = CreateCameraMatrix(panOffset, zoomFactor)
            If matrix.IsInvertible Then
                matrix.Invert()
                Dim points As PointF() = {New PointF(0, 0), New PointF(Math.Max(1, viewWidth), Math.Max(1, viewHeight))}
                matrix.TransformPoints(points)
                Return New RectangleF(points(0).X, points(0).Y, points(1).X - points(0).X, points(1).Y - points(0).Y)
            End If
            Return New RectangleF(0, 0, Math.Max(1, viewWidth), Math.Max(1, viewHeight))
        End Using
    End Function

    ' FIXED: Bulletproof wrapper that executes shape rendering inside a protected matrix container
    Public Sub ExecuteSafeShapeRender(g As Graphics, baseCamera As Matrix, shape As Cls_Base_Shape)
        If shape Is Nothing Then Exit Sub
        Using localMatrix As Matrix = baseCamera.Clone()
            localMatrix.RotateAt(shape.RotationAngle, shape.CalculateGeometricCenter(), MatrixOrder.Prepend)
            g.Transform = localMatrix

            ' Draw the primitive shape primitive safely inside its isolated local coordinate context
            shape.ExecuteDrawGeometry(g)
        End Using
    End Sub

    ' Target File: Cls_Coordinate_Transformer.vb (Inverse Projection Segment)
    ' Explicit configuration locked for file scope | Option Strict On | Explicit Binding
    ' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

    '''' <summary>
    '''' Projects world coordinate points back into a shape's localized, unrotated space.
    '''' Centralizes geometry transformation operations and respects your 25-line limit.
    '''' </summary>
    'Public Function TransformWorldToLocalShapeSpace(ByVal worldPoint As PointF, ByVal shape As Cls_Base_Shape) As PointF
    '    If shape Is Nothing OrElse shape.RotationAngle = 0.0F Then
    '        Return worldPoint
    '    End If

    '    ' Instantiate a clean, temporary matrix isolated to the transformation pass
    '    Using matrix As New Matrix()
    '        ' Perform the inverse rotation relative to the shape's calculated center
    '        matrix.RotateAt(-shape.RotationAngle, shape.CalculateGeometricCenter(), MatrixOrder.Prepend)

    '        Dim points As PointF() = {worldPoint}
    '        matrix.TransformPoints(points)

    '        Return points(0)
    '    End Using
    'End Function

End Class
