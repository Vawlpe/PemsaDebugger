using System.Numerics;
using System.Reflection;
using ImGuiNET;

namespace Pemsa.Gui;

public partial class Windows
{
    public static void PemsaEmu(EmuState State)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowTitleAlign, new Vector2(0.5f, 0.5f));
        ImGui.SetNextWindowSize(new Vector2(128 * State.Gui.ZoomMultiplier, 140 * State.Gui.ZoomMultiplier) , ImGuiCond.Always);
        ImGui.SetNextWindowPos(ImGui.GetIO().DisplaySize / 2 - (new Vector2(128 * State.Gui.ZoomMultiplier, 140 * State.Gui.ZoomMultiplier) / 2), ImGuiCond.Always);
        if (!ImGui.Begin($"PEMSA Emulator (zoom x{State.Gui.ZoomMultiplier})##PEMSAEmuWindow", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse))
            return;
        
        var pos = ImGui.GetWindowPos();
        var siz = ImGui.GetWindowSize();

        Native.UpdateEmulator(State.EmuHandle, ImGui.GetIO().DeltaTime);
        Native.Render(State.EmuHandle);
        
        ImGui.Image(State.Graphics.SurfaceTexturePtr, new(128 * State.Gui.ZoomMultiplier, 128 * State.Gui.ZoomMultiplier));

        ImGui.PopStyleVar(2);
        ImGui.End();

//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        ImGui.SetNextWindowSize(new Vector2(128 * State.Gui.ZoomMultiplier, 25), ImGuiCond.Always);
        ImGui.SetNextWindowPos(pos + new Vector2(0, siz.Y + 5), ImGuiCond.Always);
        if (!ImGui.Begin("##PEMSAEmuCtrlWindow", ImGuiWindowFlags.NoDecoration))
            return;

        if (ImGui.Button("Reset"))
            Native.ResetEmulator(State.EmuHandle);

        ImGui.SameLine();
        if (ImGui.Button("Stop"))
            Native.StopEmulator(State.EmuHandle);

        ImGui.SameLine();
        ImGui.InvisibleButton("##PEMSAEmuCtrlSpacing", new Vector2(-50,0)); ImGui.SameLine();

        ImGui.BeginDisabled(State.Gui.ZoomMultiplier >= State.Gui.ZoomMultiplierMax);
        if (ImGui.Button("+", new Vector2(-25, 0)))
            State.Gui.ZoomMultiplier += State.Gui.ZoomMultiplierStep;
        ImGui.EndDisabled();

        ImGui.SameLine();

        ImGui.BeginDisabled(State.Gui.ZoomMultiplier <= State.Gui.ZoomMultiplierMin);
        if (ImGui.Button("-", new Vector2(-1, 0)) && State.Gui.ZoomMultiplier > State.Gui.ZoomMultiplierMin)
            State.Gui.ZoomMultiplier -= State.Gui.ZoomMultiplierStep;
        ImGui.EndDisabled();

        ImGui.End();
//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        ImGui.SetNextWindowSize(new Vector2(128 * State.Gui.ZoomMultiplier, 25), ImGuiCond.Always);
        ImGui.SetNextWindowPos(pos + new Vector2(0, siz.Y + 40), ImGuiCond.Always);
        if (!ImGui.Begin("##PEMSAEmuCartWindow", ImGuiWindowFlags.NoDecoration))
            return;
        
        if (ImGui.Button("Load Cart"))
            Native.LoadCart(State.EmuHandle, Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "carts", State.CartName));
            
        ImGui.SameLine();
        ImGui.SetNextItemWidth(ImGui.GetWindowWidth() - 90);
        if (ImGui.InputTextWithHint("", "cartName.p8", ref State.CartName, 1024, ImGuiInputTextFlags.EnterReturnsTrue))
            Native.LoadCart(State.EmuHandle, Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "carts", State.CartName));

        ImGui.End();
    }
}