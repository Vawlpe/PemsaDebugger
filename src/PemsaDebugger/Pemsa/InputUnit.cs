using System.Runtime.InteropServices;
using ImGuiNET;
using Veldrid;

namespace Pemsa;

public class InputUnit
{
    EmuState State;

    public InputUnit(EmuState state) { State = state; }
    public bool IsButtonDown(int i, int p)
    {
        Console.WriteLine($"IsButtonDown: {i}, {p}");
        return false;
    }
    public bool IsButtonPressed(int i, int p)
    {
        Console.WriteLine($"IsButtonPressed: {i}, {p}");
        return false;
    }
    public void UpdateInput()
    {
        Console.WriteLine("UpdateInput");
        return;
    }
    public int GetMouseX()
    {
        Console.WriteLine("GetMouseX");
        return 0;
    }
    public int GetMouseY()
    {
        Console.WriteLine("GetMouseY");
        return 0;
    }
    public int GetMouseMask()
    {
        Console.WriteLine("GetMouseMask");
        return 0;
    }
    public string ReadKey()
    {
        Console.WriteLine("ReadKey");
        return String.Empty;
    }
    public bool HasKey()
    {
        Console.WriteLine("HasKey");
        return false;
    }
    public void ResetInput()
    {
        Console.WriteLine("ResetInput");
        return;
    }
    public string GetClipboardText()
    {
        Console.WriteLine("GetClipboardText");
        return String.Empty;
    }
}