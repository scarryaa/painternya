using System;
using Avalonia.Media.Imaging;

namespace painternya.Services;

public class ThumbnailCapturer
{
    private readonly TileManager _tileManager;
    private readonly int _canvasWidth;
    private readonly int _canvasHeight;

    public ThumbnailCapturer(TileManager tileManager, int canvasWidth, int canvasHeight)
    {
        _tileManager = tileManager ?? throw new ArgumentNullException(nameof(tileManager));
        _canvasWidth = canvasWidth;
        _canvasHeight = canvasHeight;
    }

    public RenderTargetBitmap CaptureThumbnail()
    {
        return _tileManager.CaptureThumbnail();
    }
}
