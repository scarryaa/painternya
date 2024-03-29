using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.ReactiveUI;
using Microsoft.VisualBasic;
using painternya.Interfaces;
using painternya.Models;
using painternya.Services;
using painternya.Tools;
using ReactiveUI;
using DrawingContext = painternya.Models.DrawingContext;

namespace painternya.ViewModels
{
    public class CanvasViewModel : ViewModelBase, IOffsetObserver, IDisposable
    {
        public Bitmap CurrentBitmap { get; set; }
        private Color _currentGlobalColor = Colors.Black;
        private readonly ThumbnailCapturer _thumbnailCapturer;
        private RenderTargetBitmap? _thumbnail;
        private readonly ToolManager _toolManager;
        private bool isActive = true;
        private int _canvasHeight;
        private int _canvasWidth;
        private readonly Subject<Unit> _horizontalOffsetChangedSubject = new();
        private readonly Subject<Unit> _verticalOffsetChangedSubject = new();
        private readonly DrawingContext _drawingContext;
        private double _offsetX;
        private double _offsetY;
        private Vector _offset;
        private static int _globalCurrentToolSize = 4;
        private Action<MessageType, object> _messageSubscription;
        private double _zoom = 1.0;
        
        public double Zoom
        {
            get => _zoom;
            set => this.RaiseAndSetIfChanged(ref _zoom, value);
        }
        
        public Layer ActiveLayer => _drawingContext.LayerManager.ActiveLayer;
        public DrawingContext DrawingContext => _drawingContext;
        public bool IsActive
        {
            get => isActive;
            set => this.RaiseAndSetIfChanged(ref isActive, value);
        }
        
        public Color CurrentGlobalColor
        {
            get => _currentGlobalColor;
            set
            {
                _currentGlobalColor = value;
                _drawingContext.CurrentColor = value;
                this.RaisePropertyChanged();
            }
        }
        
        public RenderTargetBitmap OffscreenBitmap => _drawingContext.OffscreenBitmap;
        
        public RenderTargetBitmap? Thumbnail
        {
            get => _thumbnail;
            set
            {
                if (value == null) return;
                _thumbnail = value;
                this.RaisePropertyChanged();
            }
        }
        
        public int GlobalCurrentToolSize
        {
            get => _globalCurrentToolSize;
            set
            {
                _globalCurrentToolSize = value;
                _toolManager.CurrentTool.Size = value;
                this.RaisePropertyChanged();
            }
        }
        
        public int CanvasWidth
        {
            get => _canvasWidth;
            set
            {
                _canvasWidth = value;
                this.RaisePropertyChanged();
            }
        }
        
        public int CanvasHeight
        {
            get => _canvasHeight;
            set
            {
                _canvasHeight = value;
                this.RaisePropertyChanged();
            }
        }

        public Vector Offset
        {
            get => _offset;
            set
            {
                _offset = value;
                this.RaisePropertyChanged();
            }
        }
        
        public double OffsetX
        {
            get => _offsetX;
            set
            {
                _offsetX = value;
                _horizontalOffsetChangedSubject.OnNext(Unit.Default);
            }
        }

        public double OffsetY
        {
            get => _offsetY;
            set
            {
                _offsetY = value;
                _verticalOffsetChangedSubject.OnNext(Unit.Default);
            }
        }
        
        public ICommand? PointerMovedCommand { get; set; }
        public ICommand? PointerPressedCommand { get; set; }
        public ICommand? PointerReleasedCommand { get; set; }
        public event Action? InvalidateRequested;
        
        public IObservable<Unit> OffsetChanged => 
            _horizontalOffsetChangedSubject.Merge(_verticalOffsetChangedSubject);
        
        public CanvasViewModel() {}
        
        public CanvasViewModel(ToolManager toolManager, LayerManager layerManager, int canvasWidth, int canvasHeight)
        {
            _toolManager = toolManager;
            _drawingContext = new DrawingContext(layerManager, this, canvasWidth, canvasHeight);
            _horizontalOffsetChangedSubject
                .Merge(_verticalOffsetChangedSubject)
                .Throttle(TimeSpan.FromMilliseconds(100))
                .ObserveOn(AvaloniaScheduler.Instance)
                .Subscribe(_ => InvalidateRequested?.Invoke());
            
            _messageSubscription = (message, data) =>
            {
                if ((message is MessageType.LayerRemoved or MessageType.LayerVisibilityChanged or MessageType.LayerAdded) && IsActive)
                {
                    Console.WriteLine("Invalidate requested " + message + ", " + data);
                    InvalidateRequested?.Invoke();
                }
            };
            
            MessagingService.Instance.Subscribe(_messageSubscription);
            
            PointerMovedCommand = ReactiveCommand.Create<Point>(HandlePointerMoved);
            PointerPressedCommand = ReactiveCommand.Create<Point>(HandlePointerPressed);
            PointerReleasedCommand = ReactiveCommand.Create<Point>(HandlePointerReleased);
            
            CanvasWidth = canvasWidth;
            CanvasHeight = canvasHeight;

            _drawingContext.DrawingChanged
                .ObserveOn(AvaloniaScheduler.Instance)
                .Subscribe(_ =>
                {
                    InvalidateRequested?.Invoke();
                });
        }

        public void CaptureThumbnail()
        {
            Thumbnail = _drawingContext.CaptureThumbnail();
        }
        
        private void HandlePointerPressed(Point point)
        {
            if (!isActive) return;
            _toolManager.CurrentTool.OnPointerPressed(_drawingContext.LayerManager, _drawingContext, point, _toolManager.CurrentTool.Size);
        }
        
        private void HandlePointerMoved(Point point)
        {
            if (!isActive) return;
            _toolManager.CurrentTool.OnPointerMoved(_drawingContext, point, _toolManager.CurrentTool.Size);
        }
        
        private void HandlePointerReleased(Point point)
        {
            if (!isActive) return;
            _toolManager.CurrentTool.OnPointerReleased(_drawingContext.LayerManager, _drawingContext, point);
            
            Task.Run(() =>
            {
                _drawingContext.CaptureThumbnail();
            });
        }
        
        public void SetBitmap(WriteableBitmap bitmap)
        {
            // Implement logic to update canvas drawing based on new bitmap.
            // This could involve rendering the bitmap to a layer, updating display, etc.
            CurrentBitmap = bitmap;
            
            _drawingContext.LayerManager.ActiveLayer.TileManager.SetBitmapToTile(0, 0, bitmap);
        }

        private void ReleaseUnmanagedResources() {}

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            MessagingService.Instance.Unsubscribe(_messageSubscription);
            _horizontalOffsetChangedSubject.Dispose();
            _verticalOffsetChangedSubject.Dispose();
            
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}