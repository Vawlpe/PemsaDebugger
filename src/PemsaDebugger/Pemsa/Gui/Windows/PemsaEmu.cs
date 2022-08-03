using System.Numerics;
using System.Reflection;
using ImGuiNET;

namespace Pemsa.Gui;

public partial class Windows
{
    public static void PemsaEmu(ref EmuState State)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
        ImGui.PushStyleColor(ImGuiCol.WindowBg, 0xFF_000000);

        ImGui.SetNextWindowPos(ImGui.GetIO().DisplaySize / 2 - ((new Vector2(128, 128) * State.Gui.ZoomMultiplier) + new Vector2(0, ImGui.GetFontSize() + ImGui.GetStyle().FramePadding.Y * 2) / 2), ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowSize((new Vector2(128, 128) * State.Gui.ZoomMultiplier) + new Vector2(0, ImGui.GetFontSize() + ImGui.GetStyle().FramePadding.Y * 2), ImGuiCond.Always); 

        if (!ImGui.Begin($"PEMSA Emulator (zoom x{State.Gui.ZoomMultiplier})###PEMSAEmuWindow",
            ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            return;
        
        State.Gui.EmuPos = ImGui.GetWindowPos();
        State.Gui.EmuSize = ImGui.GetWindowSize();
        State.Gui.EmuContentSize = ImGui.GetContentRegionAvail();
        State.Gui.EmuFocused = ImGui.IsWindowFocused();

        if (ImGui.IsWindowDocked())
            State.Gui.ZoomMultiplierMax = (float)Math.Round(Math.Min(State.Gui.EmuContentSize.X / 128, State.Gui.EmuContentSize.Y / 128), 1);
        else
            State.Gui.ZoomMultiplierMax = 6f;
        State.Gui.ZoomMultiplier = Math.Clamp(State.Gui.ZoomMultiplier, State.Gui.ZoomMultiplierMin, State.Gui.ZoomMultiplierMax);

        if (State.IsRunning)
        {
            ImGui.SetCursorPos(ImGui.GetCursorPos() + State.Gui.EmuContentSize / 2 - ((new Vector2(128, 128) * State.Gui.ZoomMultiplier) / 2));
            State.Gui.EmuContentPos = ImGui.GetCursorPos();
            ImGui.Image(State.Graphics.SurfaceTexturePtr, new Vector2(128, 128) * State.Gui.ZoomMultiplier);
        }
        else
        {
            ImGui.PushStyleColor(ImGuiCol.Text, 0xFF_00AAFF);
            ImGui.SetCursorPos(new(
                (State.Gui.EmuSize.X - ImGui.CalcTextSize("PEMSA Emulator (not running)").X) / 2,
                (State.Gui.EmuSize.Y / 2) - (ImGui.GetFontSize() * 2)
            ));
            ImGui.Text("PEMSA Emulator");
            ImGui.SameLine();
            ImGui.PushStyleColor(ImGuiCol.Text, 0xFF_55C8FF);
            ImGui.Text("(not running)");
            ImGui.PushStyleColor(ImGuiCol.Text, 0xFF_00007F);
            ImGui.SetCursorPosX((State.Gui.EmuSize.X - ImGui.CalcTextSize("Please load a PEMSA/PICO-8 cartridge").X) / 2);
            ImGui.Text("Please load a PEMSA/PICO-8 cartridge");
            ImGui.PopStyleColor(3);
        }

        ImGui.End();

        ImGui.PopStyleVar();
        ImGui.PopStyleColor();

//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        ImGui.SetNextWindowSize(new(250, State.Gui.EmuSize.Y), ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowPos(State.Gui.EmuPos - new Vector2(255, 0), ImGuiCond.FirstUseEver);

        if (!ImGui.Begin("Controls###PEMSAEmuCtrlWindow"))
            return;

        ImGui.BeginDisabled(true);
        if (ImGui.Button("Reset"))
            Native.ResetEmulator(State.EmuHandle);

        ImGui.SameLine();
        if (ImGui.Button("Stop"))
            Native.StopEmulator(State.EmuHandle);
        ImGui.EndDisabled();

        ImGui.SameLine();
        ImGui.Text("Zoom:");

        ImGui.SetNextItemWidth(ImGui.GetWindowWidth() - 150);
        ImGui.SameLine();
        ImGui.SliderFloat("##PEMSAEmuZoom", ref State.Gui.ZoomMultiplier, State.Gui.ZoomMultiplierMin, State.Gui.ZoomMultiplierMax, "x%.1f");

        ImGui.BeginDisabled(State.IsRunning);
        if (ImGui.Button("Load Cart"))
        {
            Native.LoadCart(State.EmuHandle, Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "carts", State.CartName));
            State.IsRunning = true;
        }
            
        ImGui.SameLine();
        ImGui.SetNextItemWidth(ImGui.GetWindowWidth() - 90);
        if (ImGui.InputTextWithHint("", "cartName.p8", ref State.CartName, 1024, ImGuiInputTextFlags.EnterReturnsTrue))
        {
            Native.LoadCart(State.EmuHandle, Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "carts", State.CartName));
            State.IsRunning = true;
        }
        ImGui.EndDisabled();

        ImGui.End();
//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        ImGui.SetNextWindowSize(new(250, State.Gui.EmuSize.Y), ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowPos(State.Gui.EmuPos + new Vector2(State.Gui.EmuSize.X + 5, 0), ImGuiCond.FirstUseEver);

        if (!ImGui.Begin("Debug###PEMSAEmuDebugWindow"))
            return;
            
        ImGui.Text($"Cart: {State.CartName}");
        ImGui.Text($"Mouse pos: ({State.Input.MousePosition.X}, {State.Input.MousePosition.Y})");
        ImGui.Text($"Mouse buttons: 0b{(State.Input.MouseButtonsMask & 4) >> 2}{(State.Input.MouseButtonsMask & 2) >> 1}{State.Input.MouseButtonsMask & 1}");

        Widgets.InputDisplay(1, State.Input.ButtonsDown[0, 0], State.Input.ButtonsDown[1, 0], State.Input.ButtonsDown[2, 0], State.Input.ButtonsDown[3, 0], State.Input.ButtonsDown[4, 0], State.Input.ButtonsDown[5, 0]);
        Widgets.InputDisplay(2, State.Input.ButtonsDown[0, 1], State.Input.ButtonsDown[1, 1], State.Input.ButtonsDown[2, 1], State.Input.ButtonsDown[3, 1], State.Input.ButtonsDown[4, 1], State.Input.ButtonsDown[5, 1]);

        ImGui.End();
    }
}

public static partial class Widgets
{
    public static void InputDisplay(int P, bool L, bool R, bool U, bool D, bool X, bool O)
    {
        ImGui.Text($"P{P}: ");
        ImGui.SameLine();
        if (L) ImGui.PushStyleColor(ImGuiCol.Button, 0xFF_00AAFF);
        ImGui.ArrowButton("##PEMSAEmuDebugLbtn", ImGuiDir.Left);
        if (L) ImGui.PopStyleColor();
        ImGui.SameLine();
        if (R) ImGui.PushStyleColor(ImGuiCol.Button, 0xFF_00AAFF);
        ImGui.ArrowButton("##PEMSAEmuDebugRbtn", ImGuiDir.Right);
        if (R) ImGui.PopStyleColor();
        ImGui.SameLine();
        if (U) ImGui.PushStyleColor(ImGuiCol.Button, 0xFF_00AAFF);
        ImGui.ArrowButton("##PEMSAEmuDebugUbtn", ImGuiDir.Up);
        if (U) ImGui.PopStyleColor();
        ImGui.SameLine();
        if (D) ImGui.PushStyleColor(ImGuiCol.Button, 0xFF_00AAFF);
        ImGui.ArrowButton("##PEMSAEmuDebugDbtn", ImGuiDir.Down);
        if (D) ImGui.PopStyleColor();
        ImGui.SameLine();
        if (X) ImGui.PushStyleColor(ImGuiCol.Button, 0xFF_00AAFF);
        ImGui.Button("X##PEMSAEmuDebugXbtn");
        if (X) ImGui.PopStyleColor();
        ImGui.SameLine();
        if (O) ImGui.PushStyleColor(ImGuiCol.Button, 0xFF_00AAFF);
        ImGui.Button("O##PEMSAEmuDebugObtn");
        if (O) ImGui.PopStyleColor();
    }
}