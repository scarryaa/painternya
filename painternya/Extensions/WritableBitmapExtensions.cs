using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace painternya.Extensions;

public static class WriteableBitmapExtensions
{
    public static void Clear(this WriteableBitmap bitmap, Avalonia.Media.Color color)
    {
        using var context = bitmap.Lock();
        int numPixels = context.Size.Width * context.Size.Height;
        int[] colorData = new int[numPixels];
        int colorValue = color.ToArgb();  // Assumes bitmap is in ARGB format
        for (int i = 0; i < numPixels; i++)
        {
            colorData[i] = colorValue;
        }
        Marshal.Copy(colorData, 0, context.Address, numPixels);
    }
    
    public static void SetPixel(this WriteableBitmap bitmap, int x, int y, Avalonia.Media.Color color)
    {
        using var context = bitmap.Lock();
        context.SetPixel(x, y, color);
    }
    
    public static void SetPixels(this WriteableBitmap bitmap, Dictionary<Point, Color> pixels)
    {
        using var context = bitmap.Lock();
        foreach (var pixel in pixels)
        {
            var (x, y) = pixel.Key;
            var color = pixel.Value;
            context.SetPixel((int)x, (int)y, color);
        }
    }

    
    public static void Clone(this WriteableBitmap destination, WriteableBitmap source)
    {
        if (source.PixelSize != destination.PixelSize)
        {
            throw new ArgumentException("Source and destination bitmaps must have the same dimensions.");
        }

        using var destContext = destination.Lock();
        using var sourceContext = source.Lock();

        var sizeInBytes = sourceContext.RowBytes * sourceContext.Size.Height;
        unsafe
        {
            Buffer.MemoryCopy(sourceContext.Address.ToPointer(), destContext.Address.ToPointer(), sizeInBytes, sizeInBytes);
        }
    }
    
    public static void Merge(this WriteableBitmap destination, WriteableBitmap source)
    {
        if (source.PixelSize != destination.PixelSize)
        {
            throw new ArgumentException("Source and destination bitmaps must have the same dimensions.");
        }

        using var destContext = destination.Lock();
        using var sourceContext = source.Lock();

        var sizeInBytes = sourceContext.RowBytes * sourceContext.Size.Height;
        unsafe
        {
            byte* destAddress = (byte*)destContext.Address.ToPointer();
            byte* sourceAddress = (byte*)sourceContext.Address.ToPointer();
            for (int i = 0; i < sizeInBytes; i++)
            {
                if (sourceAddress[i] != 0)
                {
                    destAddress[i] = sourceAddress[i];
                }
            }
        }
    }
}
