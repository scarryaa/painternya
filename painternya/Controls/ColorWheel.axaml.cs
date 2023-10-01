using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using Avalonia.Input;

namespace painternya.Controls
{
    public partial class ColorWheel : Control
    {
        public static readonly StyledProperty<Color> SelectedColorProperty =
            AvaloniaProperty.Register<ColorWheel, Color>(nameof(SelectedColor));

        public Color SelectedColor
        {
            get => GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }
        
        public override void Render(DrawingContext context)
        {
            base.Render(context);
            
            var size = Bounds.Size;
            var radius = Math.Min(size.Width, size.Height) / 2;
            var center = new Point(size.Width / 2, size.Height / 2);
            
            for (double angle = 0; angle < 360; angle += 1)
            {
                var color = HsvToRgb(angle, 1, 1);
                
                var brush = new SolidColorBrush(color);
                
                var x = center.X + radius * Math.Cos(angle * Math.PI / 180);
                var y = center.Y + radius * Math.Sin(angle * Math.PI / 180);
                
                context.DrawLine(
                    new Pen(brush),
                    center,
                    new Point(x, y)
                );
            }
        }
        
        private Color HsvToRgb(double h, double s, double v)
        {
            int hi = Convert.ToInt32(Math.Floor(h / 60)) % 6;
            double f = h / 60 - Math.Floor(h / 60);

            byte p = Convert.ToByte(255 * v * (1 - s));
            byte q = Convert.ToByte(255 * v * (1 - f * s));
            byte t = Convert.ToByte(255 * v * (1 - (1 - f) * s));
            byte v255 = Convert.ToByte(255 * v);

            return hi switch
            {
                0 => Color.FromArgb(255, v255, t, p),
                1 => Color.FromArgb(255, q, v255, p),
                2 => Color.FromArgb(255, p, v255, t),
                3 => Color.FromArgb(255, p, q, v255),
                4 => Color.FromArgb(255, t, p, v255),
                _ => Color.FromArgb(255, v255, p, q),
            };
        }
        
        private bool _isPressed = false;

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            _isPressed = true;
            UpdateSelectedColor(e.GetPosition(this));
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);
            if (_isPressed)
            {
                UpdateSelectedColor(e.GetPosition(this));
            }
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);
            _isPressed = false;
        }

        private void UpdateSelectedColor(Point position)
        {
            var center = new Point(Bounds.Width / 2, Bounds.Height / 2);
            var dx = position.X - center.X;
            var dy = position.Y - center.Y;
            
            var distanceSquared = dx * dx + dy * dy;
            var radiusSquared = Math.Pow(Math.Min(Bounds.Width, Bounds.Height) / 2, 2);
            if (distanceSquared > radiusSquared)
                return;
                
            var angle = Math.Atan2(dy, dx);
            var hue = (angle * (180 / Math.PI) + 360) % 360;  // Normalize to [0, 360)
            
            var saturation = Math.Sqrt(distanceSquared) / Math.Sqrt(radiusSquared);
            
            var value = 1.0;
            
            var color = HsvToRgb(hue, saturation, value);
            
            SelectedColor = color;
        }
    }
}