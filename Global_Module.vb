Public Module Global_Module



    ' Target File: Enums.vb
    ' Target File: Enums.vb
    ' Project Namespace: mdi_test
    ' Architecture Standard: VB.NET | Option Strict On | Explicit Binding

    Public Enum CanvasTool
        SelectPointer
        LogicGate
    End Enum

    Public Enum LogicGateType
        Buffer
        NotGate
        AndGate
        OrGate
        NandGate
        NorGate
        XorGate
        XnorGate
    End Enum





    Public Enum ResizeHandle
        None
        TopLeft
        TopCenter
        TopRight
        MiddleRight
        BottomRight
        BottomCenter
        BottomLeft
        MiddleLeft
    End Enum

    ' Target File: Enums.vb or Cls_Viewport.vb Namespace
    ' Target File: Enums.vb or Cls_Viewport.vb Namespace
    Public Enum ViewportState
        Idle
        Panning
        Drawing
        MarqueeSelecting
        Resizing
        Rotating
        GroupDragging ' <-- ADD THIS BLOCK LINE
    End Enum

    ' Target File: Enums.vb
    Public Enum CommandAction


        NewDocument
        CloseDocument
        ZoomIn
        ZoomOut
        ZoomFit
        ToggleGrid
        ToggleSnap
        DeleteSelection
        ExecuteStressTest


        ToggleInspectorPanel
        SwitchTheme

        LayoutCascade        ' <--- ADDED INFRASTRUCTURE TOKEN
        LayoutTileHorizontal ' <--- ADDED INFRASTRUCTURE TOKEN
        LayoutTileVertical   ' <--- ADDED INFRASTRUCTURE TOKEN
    End Enum




End Module
