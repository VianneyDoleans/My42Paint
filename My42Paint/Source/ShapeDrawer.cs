using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace My42Paint.Source
{
    public class ShapeDrawer
    {
        private Color _color; public Color Color { set => _color = value; }
        private int _strokeThickness = 2; public int StrokeThickness {  set => _strokeThickness = value; }

        public ShapeDrawer(Color color)
        {
            _color = color;
        }

        private void DrawPreviewLine(Point start, Point end, Brush color, InkCanvas drawingSheet)
        {
            var line = new Line
            {
                Stroke = color,
                StrokeThickness = _strokeThickness,
                X1 = start.X,
                X2 = end.X,
                Y1 = start.Y - 50,
                Y2 = end.Y - 50,
                Tag = true
            };
            drawingSheet.Children.Add(line);
        }

        private void DrawInkCanvasLine(Point start, Point end, InkCanvas drawingSheet)
        {
            var pts = new StylusPointCollection();
            pts.Add(new StylusPoint(start.X, start.Y - 50));
            pts.Add(new StylusPoint(end.X, end.Y - 50));

            var s = new Stroke(pts);
            s.DrawingAttributes.Color = _color;

            drawingSheet.Strokes.Add(s);
        }

        public void DrawLine(Point start, Point end, InkCanvas drawingSheet, bool preview = false)
        {
            var color = DefineColor(preview);
            if (preview)
                DrawPreviewLine(start, end, color, drawingSheet);
            else
                DrawInkCanvasLine(start, end, drawingSheet);
        }


        private void DrawPreviewRectangle(Point start, Point end, Brush color, InkCanvas drawingSheet)
        {
            var rectangle = new Rectangle
            {
                Stroke = color,
                StrokeThickness = _strokeThickness,
                Tag = true
            };
            if (end.X >= start.X)
            {
                rectangle.SetValue(InkCanvas.LeftProperty, start.X);
                rectangle.Width = end.X - start.X;
            }
            else
            {
                rectangle.SetValue(InkCanvas.LeftProperty, end.X);
                rectangle.Width = start.X - end.X;
            }
            if (end.Y >= start.Y)
            {
                rectangle.SetValue(InkCanvas.TopProperty, start.Y - 50);
                rectangle.Height = end.Y - start.Y;
            }
            else
            {
                rectangle.SetValue(InkCanvas.TopProperty, end.Y - 50);
                rectangle.Height = start.Y - end.Y;
            }
            drawingSheet.Children.Add(rectangle);
        }

        private void DrawInkCanvasRectangle(Point start, Point end, InkCanvas drawingSheet)
        {
            var pts = new StylusPointCollection();
            pts.Add(new StylusPoint(start.X, start.Y - 50));
            pts.Add(new StylusPoint(end.X, start.Y - 50));
            pts.Add(new StylusPoint(end.X, end.Y - 50));
            pts.Add(new StylusPoint(start.X, end.Y - 50));
            pts.Add(new StylusPoint(start.X, start.Y - 50));
            
            var s = new Stroke(pts);
            s.DrawingAttributes.Color = _color;

            drawingSheet.Strokes.Add(s);
        }
        
        public void DrawRectangle(Point start, Point end, InkCanvas drawingSheet, bool preview = false)
        {
            var color = DefineColor(preview);
            if (preview)
                DrawPreviewRectangle(start, end, color, drawingSheet);
            else
                DrawInkCanvasRectangle(start, end, drawingSheet);
        }
        
        private SolidColorBrush DefineColor(bool preview)
        {
            return preview ? new SolidColorBrush(_color) { Opacity = 0.2 } : new SolidColorBrush(_color);
        }
    }
}