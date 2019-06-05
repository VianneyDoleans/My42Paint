using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
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

        public void DrawLine(Point start, Point end, InkCanvas drawingSheet, bool preview = false)
        {
            var color = DefineColor(preview);
            var tag = preview;
            if(!preview)
                Debug.WriteLine("je passe ici");
            else
            {
                Debug.WriteLine("je passe la");
            }
            
            var line = new Line
            {
                Stroke = color,
                StrokeThickness = _strokeThickness,
                X1 = start.X,
                X2 = end.X,
                Y1 = start.Y - 50,
                Y2 = end.Y - 50,
                Tag = tag
            };
            drawingSheet.Children.Add(line);
        }
        
        public void DrawRectangle(Point start, Point end, InkCanvas drawingSheet, bool preview = false)
        {
            var color = DefineColor(preview);
            var tag = preview;
            
            var rectangle = new Rectangle
            {
                Stroke = color,
                StrokeThickness = _strokeThickness,
                Tag = tag
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
        
        public void DrawEllipse(Point start, Point end, InkCanvas drawingSheet, bool preview = false)
        {
            var color = DefineColor(preview);
            var tag = preview;
            
            var ellipse = new Ellipse
            {
                Stroke = color,
                StrokeThickness = _strokeThickness,
                Height = 10,
                Width = 10,
                Tag = tag
            };
            if (end.X >= start.X)
            {
                ellipse.SetValue(InkCanvas.LeftProperty, start.X);
                ellipse.Width = end.X - start.X;
            }
            else
            {
                ellipse.SetValue(InkCanvas.LeftProperty, end.X);
                ellipse.Width = start.X - end.X;
            }
            if (end.Y >= start.Y)
            {
                ellipse.SetValue(InkCanvas.TopProperty, start.Y - 50);
                ellipse.Height = end.Y - start.Y;
            }
            else
            {
                ellipse.SetValue(InkCanvas.TopProperty, end.Y - 50);
                ellipse.Height = start.Y - end.Y;
            }
            drawingSheet.Children.Add(ellipse);
        }

        private SolidColorBrush DefineColor(bool preview)
        {
            return preview ? new SolidColorBrush(_color) { Opacity = 0.2 } : new SolidColorBrush(_color);
        }
    }
}