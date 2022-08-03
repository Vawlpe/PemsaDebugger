using System.Runtime.InteropServices;
using ImGuiNET;
using Veldrid;

namespace Pemsa;

public class InputUnit
{
    public bool IsButtonDown(int i, int p) => Program._pemsaEmuState.Input.ButtonsDown[i, p];
    public bool IsButtonPressed(int i, int p) => Program._pemsaEmuState.Input.ButtonsPressed[i, p];
    public void UpdateInput() => Console.WriteLine("UpdateInput"); // What's this for? it onlt seems to be called once on startup.
    public int GetMouseX() => (int)Program._pemsaEmuState.Input.MousePosition.X;
    public int GetMouseY() => (int)Program._pemsaEmuState.Input.MousePosition.Y;
    public int GetMouseMask() => Program._pemsaEmuState.Input.MouseButtonsMask;
    public string ReadKey() => Program._pemsaEmuState.Input.LastKeyDown;
    public bool HasKey() => Program._pemsaEmuState.Input.IsAnyKeyDown;
    public void ResetInput() => Program._pemsaEmuState.Input = new EmuState.InputState();
    public string GetClipboardText() => ImGui.GetClipboardText();

    public void Update(ref EmuState State)
    {
        if (!State.Gui.EmuFocused)
            return;
        
        State.Input.ButtonsDown[0, 0] = ImGui.IsKeyDown(ImGuiKey.LeftArrow);
        State.Input.ButtonsDown[1, 0] = ImGui.IsKeyDown(ImGuiKey.RightArrow);
        State.Input.ButtonsDown[2, 0] = ImGui.IsKeyDown(ImGuiKey.UpArrow);
        State.Input.ButtonsDown[3, 0] = ImGui.IsKeyDown(ImGuiKey.DownArrow);
        State.Input.ButtonsDown[4, 0] = ImGui.IsKeyDown(ImGuiKey.Z) || ImGui.IsKeyDown(ImGuiKey.C) || ImGui.IsKeyDown(ImGuiKey.N);
        State.Input.ButtonsDown[5, 0] = ImGui.IsKeyDown(ImGuiKey.X) || ImGui.IsKeyDown(ImGuiKey.V) || ImGui.IsKeyDown(ImGuiKey.M);

        State.Input.ButtonsDown[0, 1] = ImGui.IsKeyDown(ImGuiKey.S);
        State.Input.ButtonsDown[1, 1] = ImGui.IsKeyDown(ImGuiKey.F);
        State.Input.ButtonsDown[2, 1] = ImGui.IsKeyDown(ImGuiKey.E);
        State.Input.ButtonsDown[3, 1] = ImGui.IsKeyDown(ImGuiKey.D);
        State.Input.ButtonsDown[4, 1] = ImGui.IsKeyDown(ImGuiKey.LeftShift) || ImGui.IsKeyDown(ImGuiKey.Tab);
        State.Input.ButtonsDown[5, 1] = ImGui.IsKeyDown(ImGuiKey.A)         || ImGui.IsKeyDown(ImGuiKey.Q);

        State.Input.ButtonsPressed[0, 0] = ImGui.IsKeyPressed(ImGuiKey.LeftArrow);
        State.Input.ButtonsPressed[1, 0] = ImGui.IsKeyPressed(ImGuiKey.RightArrow);
        State.Input.ButtonsPressed[2, 0] = ImGui.IsKeyPressed(ImGuiKey.UpArrow);
        State.Input.ButtonsPressed[3, 0] = ImGui.IsKeyPressed(ImGuiKey.DownArrow);
        State.Input.ButtonsPressed[4, 0] = ImGui.IsKeyPressed(ImGuiKey.Z) || ImGui.IsKeyPressed(ImGuiKey.C) || ImGui.IsKeyPressed(ImGuiKey.N);
        State.Input.ButtonsPressed[5, 0] = ImGui.IsKeyPressed(ImGuiKey.X) || ImGui.IsKeyPressed(ImGuiKey.V) || ImGui.IsKeyPressed(ImGuiKey.M);

        State.Input.ButtonsPressed[0, 1] = ImGui.IsKeyPressed(ImGuiKey.S);
        State.Input.ButtonsPressed[1, 1] = ImGui.IsKeyPressed(ImGuiKey.F);
        State.Input.ButtonsPressed[2, 1] = ImGui.IsKeyPressed(ImGuiKey.E);
        State.Input.ButtonsPressed[3, 1] = ImGui.IsKeyPressed(ImGuiKey.D);
        State.Input.ButtonsPressed[4, 1] = ImGui.IsKeyPressed(ImGuiKey.LeftShift) || ImGui.IsKeyPressed(ImGuiKey.Tab);
        State.Input.ButtonsPressed[5, 1] = ImGui.IsKeyPressed(ImGuiKey.A)         || ImGui.IsKeyPressed(ImGuiKey.Q);

        var mp = ImGui.GetMousePos() - State.Gui.EmuPos - State.Gui.EmuContentPos;
        mp /= State.Gui.ZoomMultiplier;
        mp.X = (float)Math.Floor(Math.Clamp(mp.X, 0, 127));
        mp.Y = (float)Math.Floor(Math.Clamp(mp.Y, 0, 127));
        State.Input.MousePosition = mp;

        State.Input.MouseButtonsMask = 0;
        State.Input.MouseButtonsMask |= ImGui.IsMouseDown(ImGuiMouseButton.Left)   ? 1 : 0;
        State.Input.MouseButtonsMask |= ImGui.IsMouseDown(ImGuiMouseButton.Right)  ? 2 : 0;
        State.Input.MouseButtonsMask |= ImGui.IsMouseDown(ImGuiMouseButton.Middle) ? 4 : 0;
    }
}