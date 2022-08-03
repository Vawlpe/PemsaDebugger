using System.Numerics;

namespace Pemsa;

public struct EmuState
{
    public IntPtr EmuHandle;
    public string CartName;
    public bool IsRunning;
    public GraphicsState Graphics;
    public InputState Input;
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
        public Vector2 EmuPos;
        public Vector2 EmuSize;
        public Vector2 EmuContentSize;
        public Vector2 EmuContentPos;
        public bool EmuFocused;
    }
    public struct InputState
    {
        public const int ButtonCount = 7;
		public const int PlayerCount = 8;

		public bool[,] ButtonsDown;
		public bool[,] ButtonsPressed;

        public Vector2 MousePosition;
		public int MouseButtonsMask;

		public bool IsAnyKeyDown;
		public string LastKeyDown;
    }
}

