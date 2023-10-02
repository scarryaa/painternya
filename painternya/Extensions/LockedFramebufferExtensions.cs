using System;
using Avalonia.Platform;

namespace painternya.Extensions;

public static class LockedFramebufferExtensions
{
    public static unsafe void SetPixel(this ILockedFramebuffer context, int x, int y, Avalonia.Media.Color color)
    {
        byte* address = (byte*)context.Address.ToPointer() + y * context.RowBytes + x * 4;
        *(address) = color.B;
        *(address + 1) = color.G;
        *(address + 2) = color.R;
        *(address + 3) = color.A;
    }
}