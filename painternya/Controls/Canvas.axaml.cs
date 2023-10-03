using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using painternya.Models;
using painternya.Services;
using painternya.ViewModels;
using DrawingContext = Avalonia.Media.DrawingContext;

namespace painternya.Controls
{
    public partial class Canvas : UserControl
    {
        private WriteableBitmap _checkerboardBitmap;
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

        private async void DrawCheckerboardBackground(DrawingContext context)
        {
            if (_checkerboardBitmap == null)
            {
                int squareSize = (int)Width / 100;
                int horizontalSquares = (int)Math.Ceiling(Width / (double)squareSize);
                int verticalSquares = (int)Math.Ceiling(Height / (double)squareSize);
        
                var renderTargetBitmap = new RenderTargetBitmap(
                    new PixelSize(horizontalSquares * squareSize, verticalSquares * squareSize),
                    new Vector(96, 96));

                using (var bmpContext = renderTargetBitmap.CreateDrawingContext())
                {
                    var rect = new Rect(0, 0, squareSize, squareSize);

                    for (int i = 0; i < horizontalSquares; i++)
                    {
                        for (int j = 0; j < verticalSquares; j++)
                        {
                            var currentBrush = ((i + j) % 2 == 0) ? _lightSquareBrush : _darkSquareBrush;
                            rect = rect.WithX(i * squareSize).WithY(j * squareSize);
                            bmpContext.FillRectangle(currentBrush, rect);
                        }
                    }
                }
                
                using (var stream = new MemoryStream())
                {
                    renderTargetBitmap.Save(stream);
                    stream.Position = 0;
                    
                    _checkerboardBitmap = WriteableBitmap.Decode(stream);
                }
            }

            context.DrawImage(_checkerboardBitmap, new Rect(0, 0, _checkerboardBitmap.PixelSize.Width, _checkerboardBitmap.PixelSize.Height), new Rect(0, 0, Width, Height));
        }
        
        public void InvalidateCanvas()
        {
            InvalidateVisual();
        }

        private void RenderLayer(Layer layer, DrawingContext context)
        {
            if (!layer.IsVisible) return;

            var tiles = layer.TileManager.GetAllTiles();
            foreach (var kvp in tiles)
            {
                var tile = kvp.Value;
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