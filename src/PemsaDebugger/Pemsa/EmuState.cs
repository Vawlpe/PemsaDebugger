namespace Pemsa;

public struct EmuState
{
    public IntPtr EmuHandle;
    public string CartName;
    public GraphicsState Graphics;
    public GuiState Gui;

    // Nested Structs
    public struct GraphicsState
    {
        public IntPtr SurfaceTexturePtr;
        public uint[] SurfaceTextureData;
        public Veldrid.Texture SurfaceTexture;
    }

    public struct GuiState
    {
        public float ZoomMultiplier;
        public float ZoomMultiplierMin;
        public float ZoomMultiplierMax;
        public float ZoomMultiplierStep;
    }
    public struct InputState
    {
        
    }
}

