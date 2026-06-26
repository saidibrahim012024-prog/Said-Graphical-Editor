' Target File: Cls_StatusBar.vb
' Project Namespace: mdi_test
' Architecture Standard: VB.NET | Option Strict On | Explicit Binding | NotInheritable
' Said Rules Compliance: Zero Guessing | No Non-Existent Token Variables | Locked Classes Only
' Strict Execution Rule: Maximum 25 lines of operational code per subroutine/function.

Imports System
Imports System.Drawing
Imports System.Windows.Forms

Public NotInheritable Class Cls_StatusBar

    Private ReadOnly _strip As StatusStrip
    Private ReadOnly _lblMetrics As ToolStripStatusLabel
    Private ReadOnly _lblSheets As ToolStripStatusLabel
    Private ReadOnly _lblSelection As ToolStripStatusLabel
    Private ReadOnly _lblZoom As ToolStripStatusLabel
    Private ReadOnly _lblCoordinates As ToolStripStatusLabel

#Region "Properties & Constructors"

    Public ReadOnly Property Strip() As StatusStrip
        Get
            Return Me._strip
        End Get
    End Property

    Public Sub New()
        Me._strip = New StatusStrip()
        Me._lblMetrics = New ToolStripStatusLabel("Objects: 0") With {.Spring = True, .TextAlign = ContentAlignment.MiddleLeft}
        Me._lblSheets = New ToolStripStatusLabel("Sheets: 0") With {.Spring = True, .TextAlign = ContentAlignment.MiddleLeft}
        Me._lblSelection = New ToolStripStatusLabel("Selection: None") With {.Width = 150, .AutoSize = False, .TextAlign = ContentAlignment.MiddleLeft}
        Me._lblZoom = New ToolStripStatusLabel("Zoom: 100%") With {.Width = 100, .AutoSize = False, .TextAlign = ContentAlignment.MiddleCenter}
        Me._lblCoordinates = New ToolStripStatusLabel("X: 0.0, Y: 0.0") With {.Width = 240, .AutoSize = False, .TextAlign = ContentAlignment.MiddleRight}

        Me._strip.Items.AddRange(New ToolStripItem() {Me._lblMetrics, Me._lblSheets, Me._lblSelection, Me._lblZoom, Me._lblCoordinates})
    End Sub

#End Region

#Region "Thread-Safe UI Telemetry Invokers"

    Public Sub UpdateStatusMetrics(ByVal telemetryPayload As String)
        If String.IsNullOrEmpty(telemetryPayload) Then Return

        If Me._strip.InvokeRequired Then
            Me._strip.BeginInvoke(New Action(Of String)(AddressOf UpdateStatusMetrics), telemetryPayload)
            Return
        End If

        Me._lblSheets.Text = telemetryPayload : Me._strip.Invalidate()
    End Sub

    Public Sub SetStatus(ByVal text As String)
        If Me._strip.InvokeRequired Then
            Me._strip.BeginInvoke(New Action(Of String)(AddressOf SetStatus), text)
            Return
        End If

        Me._lblMetrics.Text = If(String.IsNullOrEmpty(text), "System Ready", text)
    End Sub

    ''' <summary>
    ''' High-performance structured endpoint. Formats raw values from your revised struct.
    ''' </summary>
    Public Sub UpdateTelemetryHUD(ByVal payload As Str_Telemetry_Payload)
        If Me._strip.InvokeRequired Then
            Me._strip.BeginInvoke(New Action(Of Str_Telemetry_Payload)(AddressOf UpdateTelemetryHUD), payload)
            Return
        End If

        ' Render state and vector geometry details cleanly on the UI thread
        Me._lblMetrics.Text = $"State: {payload.State.ToString()} | Objects: {payload.TotalObjects}"
        Me._lblSheets.Text = $"Active Sheets: {payload.TotalSheets}"
        Me._lblSelection.Text = $"Selected Count: {payload.SelectedObjects}"
        Me._lblZoom.Text = $"Zoom: {CInt(payload.ZoomVal * 100)}%"
        Me._lblCoordinates.Text = $"SCR:({CInt(payload.ScreenPoint.X)},{CInt(payload.ScreenPoint.Y)}) | WLD:({payload.WorldPoint.X:F1},{payload.WorldPoint.Y:F1})"
    End Sub

#End Region
End Class
