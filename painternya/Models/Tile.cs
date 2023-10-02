using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using painternya.Extensions;

namespace painternya.Models;

public class Tile
{
    public int X { get; set; }
    public int Y { get; set; }
    public WriteableBitmap Bitmap { get; set; }
    public WriteableBitmap LowDetail { get; set; }
    public bool Dirty { get; set; }
    public bool IsVisible { get; set; }
    public int Width => Bitmap.PixelSize.Width;
    public int Height => Bitmap.PixelSize.Height;
    
    public Tile(int width, int height)
    {
        Bitmap = new WriteableBitmap(new PixelSize(width, height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Unpremul);
        LowDetail = new WriteableBitmap(new PixelSize(width / 10, height / 10), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Unpremul);
        using (var context = Bitmap.Lock())
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    context.SetPixel(x, y, Colors.Transparent);
                }
            }
        }
        
        IsVisible = true;
        Dirty = false;
    }
}