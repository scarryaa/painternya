using System;
using Avalonia.Platform;

namespace painternya.Extensions;

public static class LockedFramebufferExtensions
{
    public static unsafe void SetPixel(this ILockedFramebuffer context, int x, int y, Avalonia.Media.Color color)
    {
        var pixelBytes = new byte[4] { color.B, color.G, color.R, color.A };

        byte* address = (byte*)context.Address.ToPointer() + y * context.RowBytes + x * 4;
        for (int i = 0; i < 4; i++)
        {
            *(address + i) = pixelBytes[i];
        }
    }
}