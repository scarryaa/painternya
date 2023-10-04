using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.IO;
using Avalonia.Input;
using SkiaSharp;
using Svg.Skia;

namespace painternya.Controls
{
    public partial class ColorWheel : UserControl
    {
        private SKSvg _svg;
        private Rect _arrowIconBounds;
        private Rect[] palletteRects;
        private Color[] paletteColors;

        public static readonly StyledProperty<Color> PrimaryColorProperty =
            AvaloniaProperty.Register<ColorWheel, Color>(nameof(PrimaryColor));

        public static readonly StyledProperty<Color> SecondaryColorProperty =
            AvaloniaProperty.Register<ColorWheel, Color>(nameof(SecondaryColor));

        public Color PrimaryColor
        {
            get => GetValue(PrimaryColorProperty);
            set => SetValue(PrimaryColorProperty, value);
        }

        public Color SecondaryColor
        {
            get => GetValue(SecondaryColorProperty);
            set => SetValue(SecondaryColorProperty, value);
        }

        public ColorWheel()
        {
            _svg = new SKSvg();
            _svg.Load("../../../Assets/swap_arrows.svg");
            
            PrimaryColor = Colors.Black;
            SecondaryColor = Colors.White;

            this.GetObservable(PrimaryColorProperty).Subscribe(_ => InvalidateVisual());
            this.GetObservable(SecondaryColorProperty).Subscribe(_ => InvalidateVisual());
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            var size = Bounds.Size;
            var radius = 70;
            var center = new Point(size.Width / 2 + 15, size.Height / 2);

            for (double angle = 0; angle < 360; angle += 0.5)
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

            DrawColorBox(context, Colors.Black, new Rect(center.X - 92, center.Y - 60, 24, 24));
            DrawColorBox(context, Colors.White, new Rect(center.X - 91, center.Y - 59, 22, 22));
            DrawColorBox(context, SecondaryColor, new Rect(center.X - 90, center.Y - 58, 20, 20));

            DrawColorBox(context, Colors.Black, new Rect(center.X - 102, center.Y - 70, 24, 24));
            DrawColorBox(context, Colors.White, new Rect(center.X - 101, center.Y - 69, 22, 22));
            DrawColorBox(context, PrimaryColor, new Rect(center.X - 100, center.Y - 68, 20, 20));
            
            var skBitmap = new SKBitmap(35, 25);
            using var skCanvas = new SKCanvas(skBitmap);
            skCanvas.DrawPicture(_svg.Picture);

            // Convert the SKBitmap to an Avalonia bitmap
            using var memoryStream = new MemoryStream();
            using (var skImage = SKImage.FromBitmap(skBitmap))
            {
                var skData = skImage.Encode(SKEncodedImageFormat.Png, 100);
                skData.SaveTo(memoryStream);
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            var avaloniaBitmap = new Avalonia.Media.Imaging.Bitmap(memoryStream);

            // Draw the Avalonia bitmap onto the DrawingContext
            var sourceRect = new Rect(0, 0, avaloniaBitmap.PixelSize.Width, avaloniaBitmap.PixelSize.Height);
            var destRect = new Rect(center.X - 80, center.Y - 90, 25, 25);
            _arrowIconBounds = destRect;
            context.DrawImage(avaloniaBitmap, sourceRect, destRect);

            var hsv = RgbToHsv(PrimaryColor);
            var hue = hsv.Hue;
            var angleInRadians = hue * Math.PI / 180;
            var cursorX = center.X + radius * Math.Cos(angleInRadians);
            var cursorY = center.Y + radius * Math.Sin(angleInRadians);

            context.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Black, 2),
                new Rect(cursorX - 5, cursorY - 5, 10, 10));
            
            DrawColorPalette(context, center.X - 100, center.Y + radius + 5); 
        }
        
        private void DrawColorPalette(DrawingContext context, double startX, double startY)
        {
            paletteColors = new[]
            {
                Colors.Black, Colors.Gray, Colors.Red, Colors.Orange, Colors.Yellow, Colors.Green, Colors.SkyBlue, Colors.Blue, Colors.DarkViolet, Colors.Pink, Colors.Violet,
                Colors.White, Colors.DarkGray, Colors.DarkRed, Colors.DarkOrange, Colors.DarkGoldenrod, Colors.DarkGreen, Colors.RoyalBlue, Colors.DarkBlue, Colors.Indigo, Colors.HotPink, Colors.BlueViolet
            };
            
            palletteRects = new Rect[paletteColors.Length];

            int colorsPerRow = 11;
            double rectSize = 15;
            double spacing = 0;

            for (int i = 0; i < paletteColors.Length; i++)
            {
                int row = i / colorsPerRow;
                int col = i % colorsPerRow;

                double x = startX + col * (rectSize + spacing);
                double y = startY + row * (rectSize + spacing);

                var rect = new Rect(x, y, rectSize, rectSize);
                palletteRects[i] = rect;
                var brush = new SolidColorBrush(paletteColors[i]);

                context.FillRectangle(brush, rect);
            }
        }

        private void DrawColorBox(DrawingContext context, Color color, Rect rect)
        {
            var brush = new SolidColorBrush(color);
            context.FillRectangle(brush, rect);
        }

        public static (double Hue, double Saturation, double Value) RgbToHsv(Color color)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double delta = max - min;

            double hue = 0;
            if (delta > 0)
            {
                if (max == r)
                {
                    hue = 60 * (((g - b) / delta) % 6);
                }
                else if (max == g)
                {
                    hue = 60 * (((b - r) / delta) + 2);
                }
                else if (max == b)
                {
                    hue = 60 * (((r - g) / delta) + 4);
                }
            }

            double saturation = max == 0 ? 0 : delta / max;
            double value = max;

            return (hue, saturation, value);
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
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                UpdateSelectedColor(e.GetPosition(this));
            }
            else if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
            {
                UpdateSelectedColor(e.GetPosition(this), false);
            }
            
            var position = e.GetPosition(this);
            if (_arrowIconBounds.Contains(position))
            {
                // If the SVG icon was clicked, swap the primary and secondary colors
                (PrimaryColor, SecondaryColor) = (SecondaryColor, PrimaryColor);
            }
            
            if (palletteRects != null)
            {
                for (int i = 0; i < palletteRects.Length; i++)
                {
                    if (palletteRects[i].Contains(position))
                    {
                        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                        {
                            PrimaryColor = paletteColors[i];
                        }
                        else if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
                        {
                            SecondaryColor = paletteColors[i];
                        }
                    }
                }
            }
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);
            if (_isPressed)
            {
                if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                {
                    UpdateSelectedColor(e.GetPosition(this));
                }
                else if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
                {
                    UpdateSelectedColor(e.GetPosition(this), false);
                }
            }
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);
            _isPressed = false;
        }

        private void UpdateSelectedColor(Point position, bool isPrimary = true)
        {
            var center = new Point(Bounds.Width / 2, Bounds.Height / 2);
            var dx = position.X - center.X;
            var dy = position.Y - center.Y;

            var distanceSquared = dx * dx + dy * dy;
            var radiusSquared = Math.Pow(Math.Min(Bounds.Width, Bounds.Height) / 2, 2);
            if (distanceSquared > radiusSquared)    
                return;

            var angle = Math.Atan2(dy, dx);
            var hue = (angle * (180 / Math.PI) + 360) % 360; // Normalize to [0, 360)

            var saturation = Math.Sqrt(distanceSquared) / Math.Sqrt(radiusSquared);

            var value = 1.0;

            var color = HsvToRgb(hue, saturation, value);

            if (isPrimary)
            {
                PrimaryColor = color;
            }
            else
            {
                SecondaryColor = color;
            }
        }
    }
}