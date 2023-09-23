using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using painternya.Extensions;

namespace painternya.Models;

public class Tile
{
    public WriteableBitmap Bitmap { get; }
    public bool Dirty { get; set; }
    public bool IsVisible { get; set; }
    
    public Tile(int width, int height)
    {
        Bitmap = new WriteableBitmap(new PixelSize(width, height), new Vector(96, 96), PixelFormat.Bgra8888);
        using (var context = Bitmap.Lock())
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    context.SetPixel(x, y, Colors.White);
                }
            }
        }
        
        IsVisible = true;
        Dirty = true;
    }
}