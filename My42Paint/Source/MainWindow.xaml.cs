using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace My42Paint.Source
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private ShapeDrawer _shapeDrawer;
        private Tools _currentTools = Tools.Brush;
        private Point _start;
        private Point _end;
        private Color _color = Colors.Blue;
        
        private enum Tools
        {
            Brush,
            Eraser,
            Rectangle,
            Ellipse,
            Line
        }

        public MainWindow()
        {
            InitializeComponent();
            _shapeDrawer = new ShapeDrawer(_color);
        }

        private void BrushButton_OnClick(object sender, RoutedEventArgs e)
        {
            _currentTools = Tools.Brush;
            DrawingSheet.EditingMode = InkCanvasEditingMode.Ink;
        }

        private void EraserButton_OnClick(object sender, RoutedEventArgs e)
        {
            _currentTools = Tools.Eraser;
            DrawingSheet.EditingMode = InkCanvasEditingMode.EraseByPoint;
        }

        private void EllipseButton_OnClick(object sender, RoutedEventArgs e)
        {
            _currentTools = Tools.Ellipse;
            DrawingSheet.EditingMode = InkCanvasEditingMode.GestureOnly;
        }

        private void LineButton_OnClick(object sender, RoutedEventArgs e)
        {
            _currentTools = Tools.Line;
            DrawingSheet.EditingMode = InkCanvasEditingMode.GestureOnly;
        }

        private void RectangleButton_OnClick(object sender, RoutedEventArgs e)
        {
            _currentTools = Tools.Rectangle;
            DrawingSheet.EditingMode = InkCanvasEditingMode.GestureOnly;
        }

        private void DrawingSheet_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _start = e.GetPosition(this); 

            // Hide cursor when not using brush
            if (_currentTools != Tools.Brush)
                DrawingSheet.DefaultDrawingAttributes.Color = Colors.Transparent;
        }

        private void DrawingSheet_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            RemoveTmpDrawOnCanvas();
            DrawOnCanvas();
            DrawingSheet.DefaultDrawingAttributes.Color = Colors.Black;
        }
        
        private void DrawingSheet_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _end = e.GetPosition(this);
                RemoveTmpDrawOnCanvas();
                DrawOnCanvas(true);
            }
        }

        private void RemoveTmpDrawOnCanvas()
        {
            var toRemove = new List<UIElement>();
            
            foreach (UIElement drawingSheetChild in DrawingSheet.Children)
            {
                var shape = (Shape)drawingSheetChild;
                if ((bool)shape.Tag)
                    toRemove.Add(drawingSheetChild);
            }

            foreach (var uiElement in toRemove)
                DrawingSheet.Children.Remove(uiElement);
            toRemove.Clear();
        }

        private void DrawOnCanvas(bool preview = false)
        {
            switch (_currentTools)
            {
                case Tools.Line:
                    _shapeDrawer.DrawLine(_start, _end, DrawingSheet, preview);
                    break;
                case Tools.Rectangle:
                    _shapeDrawer.DrawRectangle(_start, _end, DrawingSheet, preview);
                    break;
                case Tools.Ellipse:
                    _shapeDrawer.DrawEllipse(_start, _end, DrawingSheet, preview);
                    break;
                default:
                    return;
            }
        }
    }
}