using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using painternya.Models;
using painternya.Services;
using painternya.ViewModels;
using DrawingContext = Avalonia.Media.DrawingContext;

namespace painternya.Controls
{
    public partial class Canvas : UserControl
    {
        private int _canvasWidth;
        private int _canvasHeight;
        private SolidColorBrush _darkSquareBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204));
        private SolidColorBrush _lightSquareBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        
        public CanvasViewModel ViewModel { get; set; }
        
        public static readonly DirectProperty<Canvas, int> CanvasWidthProperty =
            AvaloniaProperty.RegisterDirect<Canvas, int>("CanvasWidth", o => o.CanvasWidth, (o, v) => o.CanvasWidth = v);
        
        public int CanvasWidth
        {
            get => _canvasWidth;
            set => SetAndRaise(CanvasWidthProperty, ref _canvasWidth, value);
        }
        
        public static readonly DirectProperty<Canvas, int> CanvasHeightProperty =
            AvaloniaProperty.RegisterDirect<Canvas, int>("CanvasHeight", o => o.CanvasHeight, (o, v) => o.CanvasHeight = v);
        
        public int CanvasHeight
        {
            get => _canvasHeight;
            set => SetAndRaise(CanvasHeightProperty, ref _canvasHeight, value);
        }
        
        public Canvas()
        {
            DataContextChanged += OnDataContextChanged;
            InitializeComponent();
        }

        private void OnDataContextChanged(object? sender, EventArgs e)
        {
            if (DataContext is CanvasViewModel oldViewModel)
            {
                oldViewModel.InvalidateRequested -= InvalidateCanvas;
            }
            
            if (DataContext is CanvasViewModel newViewModel)
            {
                ViewModel = newViewModel;
                newViewModel.InvalidateRequested += InvalidateCanvas;
                InvalidateCanvas();
            }
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (ViewModel == null) return;

            // Draw checkerboard background
            DrawCheckerboardBackground(context);

            // Render all layers
            foreach (var layer in ViewModel.DrawingContext.LayerManager.GetAllLayers())
            {
                RenderLayer(layer, context);
            }

            RenderLayer(ViewModel.DrawingContext.LayerManager.PreviewLayer, context);
        }

        private void DrawCheckerboardBackground(DrawingContext context)
        {
            // Determine square size from canvas size
            int squareSize = (int)Width / 10;
            int horizontalSquares = (int)Math.Ceiling(Width / (double)squareSize);
            int verticalSquares = (int)Math.Ceiling(Height / (double)squareSize);
            var rect = new Rect(0, 0, squareSize, squareSize);

            for (int i = 0; i < horizontalSquares; i++)
            {
                for (int j = 0; j < verticalSquares; j++)
                {
                    var currentBrush = ((i + j) % 2 == 0) ? _lightSquareBrush : _darkSquareBrush;
                    rect = rect.WithX(i * squareSize).WithY(j * squareSize);
                    context.FillRectangle(currentBrush, rect);
                }
            }
        }
        
        public void InvalidateCanvas()
        {
            InvalidateVisual();
        }

        private void RenderLayer(Layer layer, DrawingContext context)
        {
            if (!layer.IsVisible) return;

            var offscreenBitmap = ViewModel.DrawingContext.OffscreenBitmap;
            if (offscreenBitmap != null)
            {
                Console.WriteLine("Rendering offscreen bitmap");
                var sourceRect = new Rect(0, 0, offscreenBitmap.PixelSize.Width, offscreenBitmap.PixelSize.Height);
                var destRect = new Rect(0, 0, CanvasWidth, CanvasHeight);  // Adjust these values as necessary
                context.DrawImage(offscreenBitmap, sourceRect, destRect);
            }
            else
            {
                foreach (var tile in layer.TileManager.GetAllTiles())
                {
                    if (!tile.Dirty || !tile.IsVisible) continue;
                    var sourceWidth = tile.Width;
                    var sourceHeight = tile.Height;

                    var destRect = new Rect(tile.X * TileManager.TileSize, tile.Y * TileManager.TileSize, sourceWidth, sourceHeight);
                    var sourceRect = new Rect(0, 0, sourceWidth, sourceHeight);

                    context.DrawImage(tile.Bitmap, sourceRect, destRect);
                }
            }
        }
    }
}