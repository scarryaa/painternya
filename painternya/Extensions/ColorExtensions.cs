namespace painternya.Extensions;

public static class ColorExtensions
{
    public static int ToArgb(this Avalonia.Media.Color color)
    {
        return (color.A << 24) | (color.R << 16) | (color.G << 8) | color.B;
    }
}
