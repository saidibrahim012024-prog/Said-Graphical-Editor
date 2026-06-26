' Target File: Str_Telemetry_Payload.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Value Type Struct

Public Structure Str_Telemetry_Payload

    Public TotalSheets As Integer
    Public State As ViewportState
    Public TotalObjects As Integer
    Public SelectedObjects As Integer
    Public ZoomVal As Single
    Public Viewoffset As PointF
    Public ScreenPoint As PointF
    Public WorldPoint As PointF

End Structure

