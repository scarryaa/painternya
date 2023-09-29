using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
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
    public class CanvasViewModel : ViewModelBase, IOffsetObserver
    {
        private Point _lastPoint;
        private int _canvasHeight;
        private int _canvasWidth;
        private readonly Subject<Unit> _horizontalOffsetChangedSubject = new();
        private readonly Subject<Unit> _verticalOffsetChangedSubject = new();
        private readonly DrawingContext _drawingContext;
        private double _offsetX;
        private double _offsetY;
        private Vector _offset;
        private static int _globalCurrentToolSize = 4;
        private ITool _currentTool;
        
        private ITool _pencil = new PencilTool(_globalCurrentToolSize);
        private ITool _eraser = new EraserTool(_globalCurrentToolSize);
        private ITool _brush = new BrushTool(_globalCurrentToolSize);
        public ITool Pencil => _pencil;
        public ITool Eraser => _eraser;
        public ITool Brush => _brush;
        
        public Layer ActiveLayer => _drawingContext.LayerManager.ActiveLayer;
        public DrawingContext DrawingContext => _drawingContext;
        
        public int GlobalCurrentToolSize
        {
            get => _globalCurrentToolSize;
            set
            {
                _globalCurrentToolSize = value;
                _currentTool.Size = value;
                this.RaisePropertyChanged();
            }
        }
        
        public List<ITool> Tools => new() { Pencil, Eraser, Brush };
        public ITool? CurrentTool
        {
            get => _currentTool;
            set
            {
                _currentTool = value;
                if (_currentTool != null) _currentTool.Size = _globalCurrentToolSize;
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
        
        public CanvasViewModel(LayerManager layerManager, int canvasWidth, int canvasHeight)
        {
            _drawingContext = new DrawingContext(layerManager, this, canvasWidth, canvasHeight);
            CurrentTool = Pencil;
            _horizontalOffsetChangedSubject
                .Merge(_verticalOffsetChangedSubject)
                .Throttle(TimeSpan.FromMilliseconds(100))
                .ObserveOn(AvaloniaScheduler.Instance)
                .Subscribe(_ => InvalidateRequested?.Invoke());
            
            MessagingService.Instance.Subscribe((message, data) =>
            {
                if (message == MessageType.LayerRemoved)
                {
                    InvalidateRequested?.Invoke();
                }
            });
            
            PointerMovedCommand = ReactiveCommand.Create<Point>(HandlePointerMoved);
            PointerPressedCommand = ReactiveCommand.Create<Point>(HandlePointerPressed);
            PointerReleasedCommand = ReactiveCommand.Create<Point>(HandlePointerReleased);
            
            CanvasWidth = canvasWidth;
            CanvasHeight = canvasHeight;

            _drawingContext.DrawingChanged
                .ObserveOn(AvaloniaScheduler.Instance)
                .Subscribe(_ => InvalidateRequested?.Invoke());
        }
        
        public void SelectTool(string tool)
        {
            switch (tool)
            {
                case "Pencil":
                    CurrentTool = Pencil;
                    break;
                case "Eraser":
                    CurrentTool = Eraser;
                    break;
                case "Brush":
                    CurrentTool = Brush;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tool), tool, null);
            }
        }

        private void HandlePointerPressed(Point point)
        {
            _currentTool.OnPointerPressed(_drawingContext.LayerManager, _drawingContext, point, CurrentTool.Size);
            _lastPoint = point;
        }
        
        private void HandlePointerMoved(Point point)
        {
            _currentTool.OnPointerMoved(_drawingContext, point, CurrentTool.Size);
            _lastPoint = point;
        }
        
        private void HandlePointerReleased(Point point)
        {
            _currentTool.OnPointerReleased(_drawingContext.LayerManager, _drawingContext, point);
            _lastPoint = point;
        }
    }
}