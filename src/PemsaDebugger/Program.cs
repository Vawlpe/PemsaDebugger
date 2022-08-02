using System.Reflection;
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
    private static EmuState _pemsaEmuState = new EmuState
    {
        EmuHandle = IntPtr.Zero,
        CartName = "celeste.p8",
        Gui = new EmuState.GuiState
        {
            ZoomMultiplier = 2.5f,
            ZoomMultiplierMin = 1.75f,
            ZoomMultiplierMax = 6f,
            ZoomMultiplierStep = 0.25f
        },
        Graphics = new EmuState.GraphicsState
        {
            SurfaceTexturePtr = IntPtr.Zero,
            SurfaceTextureData = new uint[0x2000 * 2],
            SurfaceTexture = null
        }
    };
    public static GraphicsUnit GfxUnit = new(_pemsaEmuState);
    public static InputUnit InpUnit = new(_pemsaEmuState);
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
            GfxUnit.Flip,
            GfxUnit.CreateSurface,
            GfxUnit.GetFps,
            GfxUnit.Render,

            InpUnit.IsButtonDown,
            InpUnit.IsButtonPressed,
            InpUnit.UpdateInput,
            InpUnit.GetMouseX,
            InpUnit.GetMouseY,
            InpUnit.GetMouseMask,
            InpUnit.ReadKey,
            InpUnit.HasKey,
            InpUnit.ResetInput,
            InpUnit.GetClipboardText,

            false
        );

        // Main application loop
        while (_window.Exists)
        {
            InputSnapshot snapshot = _window.PumpEvents();

            if (!_window.Exists)
                break;

            _controller.Update(1f / 60f, snapshot);

            SubmitUI();

            _cl.Begin();
            _cl.SetFramebuffer(_gd.MainSwapchain.Framebuffer);
            _cl.ClearColorTarget(0, new RgbaFloat(_clearColor.X, _clearColor.Y, _clearColor.Z, 1f));
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

        Gui.Windows.PemsaEmu(_pemsaEmuState);

        if (_showDemoWindow)
            ImGui.ShowDemoWindow(ref _showDemoWindow);

        ImGuiIOPtr io = ImGui.GetIO();
        io.DeltaTime = 2f;
    }
}
