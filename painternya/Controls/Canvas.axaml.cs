using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using painternya.ViewModels;

namespace painternya.Controls
{
    public partial class Canvas : UserControl
    {
        private const int TileSize = 128;
        private CanvasViewModel ViewModel { get; set; }
        
        public Canvas()
        {
            InitializeComponent();
            ViewModel = new CanvasViewModel(1024, 1024);
            DataContext = ViewModel;
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            for (int x = 0; x < ViewModel.TilesX; x++)
            {
                for (int y = 0; y < ViewModel.TilesY; y++)
                {
                    var tile = ViewModel.GetTile(x, y);
                    if (tile.Dirty)
                    {
                        tile.Dirty = false;
                    }

                    var destRect = new Rect(x * TileSize, y * TileSize, TileSize, TileSize);
                    context.DrawImage(tile.Bitmap, sourceRect: new Rect(0, 0, TileSize, TileSize), destRect: destRect);
                }
            }
        }


        private void InputElement_OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                var point = e.GetPosition(this);
                ViewModel.SetPixel((int)point.X, (int)point.Y, Colors.Red);
                
                int tileX = (int)point.X / TileSize;
                int tileY = (int)point.Y / TileSize;
        
                MarkTileAsDirty(tileX, tileY);

                // If the drawing point is close to the tile edge, 
                // we'll mark neighboring tiles as dirty too.
                int pixelOffsetX = (int)point.X % TileSize;
                int pixelOffsetY = (int)point.Y % TileSize;
        
                if (pixelOffsetX < 2)
                {
                    MarkTileAsDirty(tileX - 1, tileY);
                }
                if (pixelOffsetX > TileSize - 2)
                {
                    MarkTileAsDirty(tileX + 1, tileY);
                }
                if (pixelOffsetY < 2)
                {
                    MarkTileAsDirty(tileX, tileY - 1);
                }
                if (pixelOffsetY > TileSize - 2)
                {
                    MarkTileAsDirty(tileX, tileY + 1);
                }

                InvalidateVisual();
            }
        }

        private void MarkTileAsDirty(int x, int y)
        {
            if (x >= 0 && x < ViewModel.TilesX && y >= 0 && y < ViewModel.TilesY)
            {
                ViewModel.GetTile(x, y).Dirty = true;
            }
        }

    }
}