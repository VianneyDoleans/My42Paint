using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace My42Paint.Source
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private ShapeDrawer _shapeDrawer;
        private InkCanvasTools _currentInkCanvasTools = InkCanvasTools.Brush;
        private Point _start;
        private Point _end;
        private static readonly Color StartingColor = Colors.Black;
        private Color _prevColor = StartingColor;
        
        private enum InkCanvasTools
        {
            Brush,
            Eraser,
            Select,
            ColorPicker,
            Rectangle,
            Line
        }

        public MainWindow()
        {
            InitializeComponent();
            _shapeDrawer = new ShapeDrawer(StartingColor);
            ColorPicker.SelectedColor = StartingColor;
        }

        private void BlackAndWhiteFilter_OnClick(object sender, RoutedEventArgs e)
        {
            var sc = DrawingSheet.Strokes;
            foreach (Stroke stroke in sc)
            {
                var color = stroke.DrawingAttributes.Color;
                var grayscale = (color.R + color.G + color.B) / 3.0;
                if (grayscale < 128)
                    stroke.DrawingAttributes.Color = Colors.Black;
                else
                    stroke.DrawingAttributes.Color = Colors.White;
            }
        }

        private void GrayscaleFilter_OnClick(object sender, RoutedEventArgs e)
        {
            var sc = DrawingSheet.Strokes;
            foreach (var stroke in sc)
            {
                var color = stroke.DrawingAttributes.Color;
                var grayscale = (color.R + color.G + color.B) / 3.0;
                color.R = (byte)grayscale;
                color.G = (byte)grayscale;
                color.B = (byte)grayscale;
                stroke.DrawingAttributes.Color = color;
            }
        }

        private void SepiaFilter_OnClick(object sender, RoutedEventArgs e)
        {
            var sc = DrawingSheet.Strokes;
            foreach (var stroke in sc)
            {
                var color = stroke.DrawingAttributes.Color;
                var newColor = new Color
                {
                    R = Math.Min((byte)((color.R * 0.393) + (color.G * 0.769) + (color.B * 0.189)), (byte)255),
                    G = Math.Min((byte)((color.R * 0.349) + (color.G * 0.686) + (color.B * 0.168)), (byte)255),
                    B = Math.Min((byte)((color.R * 0.272) + (color.G * 0.534) + (color.B * 0.131)), (byte)255),
                    A = color.A
                };
                stroke.DrawingAttributes.Color = newColor;
            }
        }

        private void InvertFilter_OnClick(object sender, RoutedEventArgs e)
        {
            var sc = DrawingSheet.Strokes;
            foreach (var stroke in sc)
            {
                var color = stroke.DrawingAttributes.Color;
                color.R = (byte)(255 - color.R);
                color.G = (byte)(255 - color.G);
                color.B = (byte)(255 - color.B);
                stroke.DrawingAttributes.Color = color;
            }
        }

        private void BrushButton_OnClick(object sender, RoutedEventArgs e)
        {
            _currentInkCanvasTools = InkCanvasTools.Brush;
            DrawingSheet.EditingMode = InkCanvasEditingMode.Ink;
        }

        private void EraserButton_OnClick(object sender, RoutedEventArgs e)
        {
            _currentInkCanvasTools = InkCanvasTools.Eraser;
            DrawingSheet.EditingMode = InkCanvasEditingMode.EraseByPoint;
        }

        private void ColorPickerButton_OnClick(object sender, RoutedEventArgs e)
        {
            _currentInkCanvasTools = InkCanvasTools.ColorPicker;
            DrawingSheet.EditingMode = InkCanvasEditingMode.GestureOnly;
        }

        private void LineButton_OnClick(object sender, RoutedEventArgs e)
        {
            _currentInkCanvasTools = InkCanvasTools.Line;
            DrawingSheet.EditingMode = InkCanvasEditingMode.GestureOnly;
        }

        private void RectangleButton_OnClick(object sender, RoutedEventArgs e)
        {
            _currentInkCanvasTools = InkCanvasTools.Rectangle;
            DrawingSheet.EditingMode = InkCanvasEditingMode.GestureOnly;
        }

        private void ColorPicker_OnSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (!ColorPicker.SelectedColor.HasValue || DrawingSheet == null) return;
            DrawingSheet.DefaultDrawingAttributes.Color = ColorPicker.SelectedColor.GetValueOrDefault(Colors.Red);
            _shapeDrawer.Color = ColorPicker.SelectedColor.GetValueOrDefault(Colors.Red);
        }

        private void DrawingSheet_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _start = e.GetPosition(this);

            // Hide cursor when not using brush
            if (_currentInkCanvasTools != InkCanvasTools.Brush)
            {
                _prevColor = DrawingSheet.DefaultDrawingAttributes.Color;
                DrawingSheet.DefaultDrawingAttributes.Color = Colors.Transparent;
            }
        }

        private void DrawingSheet_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            RemoveTmpDrawOnCanvas();
            DrawOnCanvas();

            if (_currentInkCanvasTools == InkCanvasTools.ColorPicker)
            {
                var sc = DrawingSheet.Strokes;
                foreach (var stroke in sc)
                {
                    var pts = stroke.StylusPoints;
                    foreach (var stylusPoint in pts)
                    {
                        if ((int)_end.X == (int)stylusPoint.X && (int)(_end.Y - 50) == (int)stylusPoint.Y)
                        {
                            ColorPicker.SelectedColor = stroke.DrawingAttributes.Color;
                            return;
                        }
                    }
                }
            }

            // Reset color when not using brush
            if (_currentInkCanvasTools != InkCanvasTools.Brush)
                DrawingSheet.DefaultDrawingAttributes.Color = _prevColor;
        }
        
        private void DrawingSheet_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            _end = e.GetPosition(this);
            RemoveTmpDrawOnCanvas();
            DrawOnCanvas(true);
        }

        private void RemoveTmpDrawOnCanvas()
        {
            var toRemove = new List<UIElement>();
            
            foreach (UIElement drawingSheetChild in DrawingSheet.Children)
            {
                Shape shape = null;
                try
                {
                    shape = (Shape)drawingSheetChild;
                }
                catch (Exception) { 
                    // ignored
                }
                if (shape != null && (bool)shape.Tag)
                    toRemove.Add(drawingSheetChild);
            }
            foreach (var uiElement in toRemove)
                DrawingSheet.Children.Remove(uiElement);
            toRemove.Clear();
        }

        private void DrawOnCanvas(bool preview = false)
        {
            switch (_currentInkCanvasTools)
            {
                case InkCanvasTools.Line:
                    _shapeDrawer.DrawLine(_start, _end, DrawingSheet, preview);
                    break;
                case InkCanvasTools.Rectangle:
                    _shapeDrawer.DrawRectangle(_start, _end, DrawingSheet, preview);
                    break;
                default:
                    return;
            }
        }

        private void Select_OnClick(object sender, RoutedEventArgs e)
        {
            _currentInkCanvasTools = InkCanvasTools.Select;
            DrawingSheet.EditingMode = InkCanvasEditingMode.Select;
        }

     
        private void LoadImage(object sender, RoutedEventArgs e)
        {
            var image = new Image
            {
                Source = new BitmapImage(new Uri(@"../../Assets/Images/ak47.jpg", UriKind.Relative))
            };
            DrawingSheet.Children.Add(image);
        }

        private void Export_OnClick(object sender, RoutedEventArgs e)
        {
            Tools.ExportToPng(new Uri(@"../../test/test.png", UriKind.Relative), DrawingSheet);
        }
    }
}