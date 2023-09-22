using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using Avalonia;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using painternya.Interfaces;
using painternya.Models;
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
        private readonly Subject<Unit> _horizontalOffsetChangedSubject = new Subject<Unit>();
        private readonly Subject<Unit> _verticalOffsetChangedSubject = new Subject<Unit>();
        private readonly DrawingContext _drawingContext;
        private double _offsetX;
        private double _offsetY;
        private ITool _currentTool = new PencilTool();
        
        public DrawingContext DrawingContext => _drawingContext;
        
        public int TilesX => _drawingContext.TilesX;
        public int TilesY => _drawingContext.TilesY;
        
        public ITool CurrentTool
        {
            get => _currentTool;
            set
            {
                if (_currentTool != value)
                {
                    _currentTool = value;
                    this.RaisePropertyChanged();
                }
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
        public event Action? InvalidateRequested;
        
        public IObservable<Unit> OffsetChanged => 
            _horizontalOffsetChangedSubject.Merge(_verticalOffsetChangedSubject);
        
        public CanvasViewModel() {}
        
        public CanvasViewModel(int canvasWidth, int canvasHeight)
        {
            _horizontalOffsetChangedSubject
                .Merge(_verticalOffsetChangedSubject)
                .Throttle(TimeSpan.FromMilliseconds(100))
                .ObserveOn(AvaloniaScheduler.Instance)
                .Subscribe(_ => InvalidateRequested?.Invoke());
            
            _drawingContext = new DrawingContext(this, canvasWidth, canvasHeight);
            PointerMovedCommand = ReactiveCommand.Create<Point>(HandlePointerMoved);
            PointerPressedCommand = ReactiveCommand.Create<Point>(HandlePointerPressed);
            
            CanvasWidth = canvasWidth;
            CanvasHeight = canvasHeight;

            _drawingContext.DrawingChanged
                .ObserveOn(AvaloniaScheduler.Instance)
                .Subscribe(_ => InvalidateRequested?.Invoke());

        }

        private void HandlePointerPressed(Point point)
        {
            _currentTool.OnPointerPressed(_drawingContext, point);
            _lastPoint = point;
        }
        
        private void HandlePointerMoved(Point point)
        {
            _currentTool.OnPointerMoved(_drawingContext, point);
            _lastPoint = point;
        }
    }
}