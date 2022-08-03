using System.Runtime.InteropServices;
using ImGuiNET;
using Veldrid;

namespace Pemsa;

public class GraphicsUnit
{
    public void Flip()
    {
        var ram = Native.GetRam(Program._pemsaEmuState.EmuHandle);
        var screenColor = new int[16];
        for (var i = 0; i < 16; i++)
            screenColor[i] = Native.GetScreenColor(Program._pemsaEmuState.EmuHandle, i);

        for (var i = 0; i < 0x2000; i++)
        {
            var val = Marshal.ReadByte(ram + 0x6000, i);
            var rv = screenColor[val & 0x0f];
            var lv = screenColor[val >> 4];

            Program._pemsaEmuState.Graphics.SurfaceTextureData[i * 2 + 0] = (rv & 128) != 0 ? Palette.alternate[rv & 0b1111111] : Palette.standard[rv];
            Program._pemsaEmuState.Graphics.SurfaceTextureData[i * 2 + 1] = (lv & 128) != 0 ? Palette.alternate[lv & 0b1111111] : Palette.standard[lv];
        }
    }

    public void Render()
    {
        Program._gd.UpdateTexture<uint>(
            Program._pemsaEmuState.Graphics.SurfaceTexture,
            Program._pemsaEmuState.Graphics.SurfaceTextureData,
            0, 0, 0,
            128, 128,
            1, 0, 0
        );
        Program._pemsaEmuState.Graphics.SurfaceTexturePtr = Program._controller.GetOrCreateImGuiBinding(Program._gd.ResourceFactory, Program._pemsaEmuState.Graphics.SurfaceTexture);
    }

    public void CreateSurface()
    {
        Program._pemsaEmuState.Graphics.SurfaceTextureData = new uint[0x2000 * 2];
        Program._pemsaEmuState.Graphics.SurfaceTexture = Program._gd.ResourceFactory.CreateTexture(new TextureDescription(
            128, 128,
            1,1,1,
            PixelFormat.R8_G8_B8_A8_UNorm,
            TextureUsage.Sampled,
            TextureType.Texture2D
        ));
    }

    public int GetFps() => (int)(1f / ImGui.GetIO().DeltaTime);
}