using System.Reflection;
using System.Runtime.InteropServices;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using ImGuiNET;

namespace Pemsa;
class Program
{
    public static Sdl2Window _window;
    public static GraphicsDevice _gd;
    public static CommandList _cl;
    public static Gui.ImGuiController _controller;

    // UI state
    private static Vector3 _clearColor = new Vector3(0.45f, 0.55f, 0.6f);
    private static bool _showDemoWindow = false;
    public static EmuState _pemsaEmuState = new EmuState
    {
        EmuHandle = IntPtr.Zero,
        CartName = "celeste.p8",
        IsRunning = false,
        Gui = new EmuState.GuiState
        {
            ZoomMultiplier = 2.5f,
            ZoomMultiplierMin = 2f,
            ZoomMultiplierMax = 6f
        },
        Graphics = new EmuState.GraphicsState
        {
            SurfaceTexturePtr = IntPtr.Zero,
            SurfaceTextureData = new uint[0x2000 * 2],
            SurfaceTexture = null
        },
        Input = new EmuState.InputState
        {
            ButtonsDown = new bool[EmuState.InputState.PlayerCount, EmuState.InputState.ButtonCount],
            ButtonsPressed = new bool[EmuState.InputState.PlayerCount, EmuState.InputState.ButtonCount],
            MousePosition = new(0, 0),
            MouseButtonsMask = 0,
            IsAnyKeyDown = false,
            LastKeyDown = String.Empty
        }
    };
    public static GraphicsUnit _gfxUnit = new();
    public static InputUnit _inputUnit = new();
    static void Main(string[] args)
    {
        // Create window, GraphicsDevice, and all resources necessary
        VeldridStartup.CreateWindowAndGraphicsDevice(
            new WindowCreateInfo(50, 50, 1280, 720, WindowState.Normal, "PEMSA DEBUGGER"),
            new GraphicsDeviceOptions(true, null, true, ResourceBindingModel.Improved, true, true),
            out _window,
            out _gd
        );

        _window.Resized += () =>
        {
            _gd.MainSwapchain.Resize((uint)_window.Width, (uint)_window.Height);
            _controller.WindowResized(_window.Width, _window.Height);
        };

        _cl = _gd.ResourceFactory.CreateCommandList();
        _controller = new Gui.ImGuiController(_gd, _gd.MainSwapchain.Framebuffer.OutputDescription, _window.Width, _window.Height);

        _pemsaEmuState.EmuHandle = Native.AllocateEmulator(
            _gfxUnit.Flip,
            _gfxUnit.CreateSurface,
            _gfxUnit.GetFps,
            _gfxUnit.Render,

            _inputUnit.IsButtonDown,
            _inputUnit.IsButtonPressed,
            _inputUnit.UpdateInput,
            _inputUnit.GetMouseX,
            _inputUnit.GetMouseY,
            _inputUnit.GetMouseMask,
            _inputUnit.ReadKey,
            _inputUnit.HasKey,
            _inputUnit.ResetInput,
            _inputUnit.GetClipboardText,

            false
        );

        // Main application loop
        while (_window.Exists)
        {
            InputSnapshot snapshot = _window.PumpEvents();

            if (!_window.Exists)
                break;

            _inputUnit.Update(ref _pemsaEmuState);
            _controller.Update(2f / 60f, snapshot);
            if (_pemsaEmuState.IsRunning)
                Native.UpdateEmulator(_pemsaEmuState.EmuHandle, ImGui.GetIO().DeltaTime/2);

            SubmitUI();

            _cl.Begin();
            _cl.SetFramebuffer(_gd.MainSwapchain.Framebuffer);
            _cl.ClearColorTarget(0, new RgbaFloat(_clearColor.X, _clearColor.Y, _clearColor.Z, 1f));

            if (_pemsaEmuState.IsRunning)
                Native.Render(_pemsaEmuState.EmuHandle);
            _controller.Render(_gd, _cl);

            _cl.End();

            _gd.SubmitCommands(_cl);
            _gd.SwapBuffers(_gd.MainSwapchain);
        }

        // Clean up
        Native.FreeEmulator(_pemsaEmuState.EmuHandle);
        _gd.WaitForIdle();
        _controller.Dispose();
        _cl.Dispose();
        _gd.Dispose();
    }

    private static void SubmitUI()
    {
        ImGui.DockSpaceOverViewport(ImGui.GetMainViewport());
        if (ImGui.BeginMainMenuBar())
        {
            ImGui.MenuItem("Show ImGui Demo", "", ref _showDemoWindow);
            ImGui.EndMainMenuBar();
        }

        Gui.Windows.PemsaEmu(ref _pemsaEmuState);

        if (_showDemoWindow)
            ImGui.ShowDemoWindow(ref _showDemoWindow);
    }
}
