using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using painternya.Models;
using painternya.ViewModels;
using DrawingContext = Avalonia.Media.DrawingContext;

namespace painternya.Controls
{
    public partial class Canvas : UserControl
    {
        private const int TileSize = 128;
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
            
            for (var x = 0; x < ViewModel.TilesX; x++)
            {
                for (var y = 0; y < ViewModel.TilesY; y++)
                {
                    var tile = ViewModel.DrawingContext.GetTile(x, y);

                    if (tile is not { Dirty: true, IsVisible: true }) continue;
                    
                    var destRect = new Rect(x * TileSize, y * TileSize, TileSize, TileSize);
                    context.DrawImage(tile.Bitmap, sourceRect: new Rect(0, 0, TileSize, TileSize), destRect: destRect);
                }
            }
        }
        
        public void InvalidateCanvas()
        {
            InvalidateVisual();
        }
    }
}