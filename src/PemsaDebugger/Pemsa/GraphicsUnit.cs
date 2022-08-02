using System.Runtime.InteropServices;
using ImGuiNET;
using Veldrid;

namespace Pemsa;

public class GraphicsUnit
{
    EmuState State;

    public GraphicsUnit(EmuState state) { State = state; }
    public void Flip()
    {
        var ram = Native.GetRam(State.EmuHandle);
        var screenColor = new int[16];
        for (var i = 0; i < 16; i++)
            screenColor[i] = Native.GetScreenColor(State.EmuHandle, i);

        for (var i = 0; i < 0x2000; i++)
        {
            var val = Marshal.ReadByte(ram + i + 0x6000);
            var rv = screenColor[val & 0x0f];
            var lv = screenColor[val >> 4];

            State.Graphics.SurfaceTextureData[i * 2 + 0] = (rv & 128) != 0 ? Palette.alternate[rv & 0b1111111] : Palette.standard[rv];
            State.Graphics.SurfaceTextureData[i * 2 + 1] = (lv & 128) != 0 ? Palette.alternate[lv & 0b1111111] : Palette.standard[lv];
        }
    }

    public void Render()
    {
        Program._gd.UpdateTexture<uint>(
            State.Graphics.SurfaceTexture,
            State.Graphics.SurfaceTextureData,
            0, 0, 0,
            128, 128,
            1, 0, 0
        );
        State.Graphics.SurfaceTexturePtr = Program._controller.GetOrCreateImGuiBinding(Program._gd.ResourceFactory, State.Graphics.SurfaceTexture);
    }

    public void CreateSurface()
    {
        State.Graphics.SurfaceTextureData = new uint[0x2000 * 2];
        State.Graphics.SurfaceTexture = Program._gd.ResourceFactory.CreateTexture(new TextureDescription(
            128, 128,
            1,1,1,
            PixelFormat.R8_G8_B8_A8_UNorm,
            TextureUsage.Sampled,
            TextureType.Texture2D
        ));
    }

    public int GetFps() => (int)(1f / ImGui.GetIO().DeltaTime);
}