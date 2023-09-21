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
            ViewModel.InvalidateRequested += InvalidateCanvas;
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
        
        private void InvalidateCanvas()
        {
            InvalidateVisual();
        }
    }
}