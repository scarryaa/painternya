using Avalonia.Media.Imaging;

namespace painternya.Extensions;

public static class WriteableBitmapExtensions
{
    public static void Clear(this WriteableBitmap bitmap, Avalonia.Media.Color color)
    {
        using var context = bitmap.Lock();
        for (int y = 0; y < context.Size.Height; y++)
        {
            for (int x = 0; x < context.Size.Width; x++)
            {
                context.SetPixel(x, y, color);
            }
        }
    }
    
    public static void SetPixel(this WriteableBitmap bitmap, int x, int y, Avalonia.Media.Color color)
    {
        using var context = bitmap.Lock();
        context.SetPixel(x, y, color);
    }
}
