using System;
using Avalonia;
using Avalonia.Controls;
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
            
            // Render all layers
            foreach (var layer in ViewModel.DrawingContext.LayerManager.GetAllLayers())
            {
                RenderLayer(layer, context);
            }
        }
        
        public void InvalidateCanvas()
        {
            InvalidateVisual();
        }

        private void RenderLayer(Layer layer, DrawingContext context)
        {
            foreach (var tile in layer.TileManager.GetAllTiles())
            {
                Console.WriteLine(tile);
        
                var sourceWidth = tile.Width;
                var sourceHeight = tile.Height;

                var destRect = new Rect(tile.X * TileManager.TileSize, tile.Y * TileManager.TileSize, sourceWidth, sourceHeight);
                var sourceRect = new Rect(0, 0, sourceWidth, sourceHeight);

                context.DrawImage(tile.Bitmap, sourceRect, destRect);
            }
        }
    }
}