using My42Paint.Source;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Path = System.IO.Path;
using XamlReader = System.Windows.Markup.XamlReader;
using XamlWriter = System.Windows.Markup.XamlWriter;

namespace My42Paint
{
    /// <summary>
    /// Interaction logic for Paint.xaml
    /// </summary>
    public partial class Paint : System.Windows.Controls.UserControl
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

        public Paint()
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
                catch (Exception)
                {
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
            var openFile = new OpenFileDialog();
            openFile.Multiselect = false;
            openFile.CheckFileExists = true;
            openFile.Filter = @"PNG Image (*.png)|*.png|JPEG Image (*.jpg)|*.jpg";
            openFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            openFile.ShowDialog();
            if (openFile.FileName == null || openFile.FileName.Equals(""))
                return;
            var image = new Image
            {
                Source = new BitmapImage(new Uri(openFile.FileName, UriKind.Absolute))
            };
            DrawingSheet.Children.Add(image);
        }


        private void export()
        {
            var fileDialog = new SaveFileDialog();
            fileDialog.Filter = @"PNG Image (*.png)|*.png";
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            fileDialog.ShowDialog();
            if (fileDialog.FileName == null || fileDialog.FileName.Equals(""))
                return;
            Tools.ExportToPng(fileDialog.FileName, DrawingSheet);
        }

        private void Export_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void SaveChild(string directoryPath, string directoryName)
        {
            DrawingSheet.UpdateLayout();
            var fsPos = new StreamWriter(directoryPath + "\\" + directoryName + "_pos.txt");
            for (var i = 0; i < DrawingSheet.Children.Count; i++)
            {
                Tools.ExportToPng(directoryPath + "\\" + directoryName + "_child" + i + ".png", DrawingSheet.Children[i] as Image);
                var pos = directoryName + "_child" + i + " " + InkCanvas.GetTop(DrawingSheet.Children[i]) + " " + InkCanvas.GetLeft(DrawingSheet.Children[i]);
                fsPos.WriteLine(pos);
            }
            fsPos.Close();
        }


        private void save()
        {
            var dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = true;
            var res = dialog.ShowDialog();
            string dialogSelectedPath = null;
            if (res == System.Windows.Forms.DialogResult.OK)
                dialogSelectedPath = dialog.SelectedPath;
            if (dialogSelectedPath == null)
                return;
            var directoryName = Path.GetFileName(Path.GetDirectoryName(dialogSelectedPath));
            var fs = File.Open(dialogSelectedPath + "\\" + directoryName + "_ink.xaml", FileMode.Create);
            XamlWriter.Save(DrawingSheet, fs);
            fs.Close();
            DrawingSheet.UpdateLayout();
            SaveChild(dialogSelectedPath, directoryName);
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void Print_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private static void ErasePosTxtFromList(List<string> filesName)
        {
            string txtFile = null;
            foreach (var fileName in filesName)
            {
                if (!fileName.Contains("_pos.txt") || !Path.GetExtension(fileName).Equals(".txt")) continue;
                txtFile = fileName;
                break;
            }
            filesName.Remove(txtFile);
        }

        private void LoadChild(List<string> filesName)
        {
            string fileTxt = null;
            foreach (var fileName in filesName)
            {
                if (!fileName.Contains("_pos.txt") || !Path.GetExtension(fileName).Equals(".txt")) continue;
                fileTxt = fileName;
                break;
            }
            if (fileTxt == null)
                return;
            var file = new StreamReader(fileTxt);
            ErasePosTxtFromList(filesName);
            foreach (var filename in filesName)
            {
                if (filename.Contains(".png") && Path.GetExtension(filename).Equals(".png"))
                {
                    var positions = file.ReadLine();
                    if (positions != null)
                    {
                        var pos = positions.Split(' ');
                        var childSaved = new Image
                        {
                            Source = new BitmapImage(new Uri(filename, UriKind.Absolute))
                        };
                        childSaved.SetValue(InkCanvas.TopProperty, Convert.ToDouble(pos[1]));
                        childSaved.SetValue(InkCanvas.LeftProperty, Convert.ToDouble(pos[2]));
                        DrawingSheet.Children.Add(childSaved);
                    }
                }
            }
            file.Close();
        }

        private void LoadInkCanvas(string inkFileName, List<string> filesName)
        {
            var fs = File.Open(inkFileName, FileMode.Open, FileAccess.Read);
            var savedCanvas = XamlReader.Load(fs) as InkCanvas;
            fs.Close();
            if (savedCanvas != null)
                DrawingSheet.Strokes.Add(savedCanvas.Strokes);
            filesName.Remove(inkFileName);
        }

        private void load()
        {
            var dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = false;
            var res = dialog.ShowDialog();
            string directoryName = null;
            if (res == System.Windows.Forms.DialogResult.OK)
                directoryName = dialog.SelectedPath;
            if (directoryName == null)
                return;
            var filesName = new List<string>(Directory.GetFiles(directoryName));
            string inkFileName = null;

            foreach (var fileName in filesName)
            {
                if (!fileName.Contains("_ink.xaml") || !Path.GetExtension(fileName).Equals(".xaml")) continue;
                inkFileName = fileName;
                break;
            }
            if (inkFileName == null) return;
            LoadInkCanvas(inkFileName, filesName);
            LoadChild(filesName);
        }

        private void Load_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void NewPaint()
        {
            DrawingSheet.Children.Clear();
            DrawingSheet.Strokes.Clear();
            BrushButton.IsChecked = true;
            _currentInkCanvasTools = InkCanvasTools.Brush;
            DrawingSheet.EditingMode = InkCanvasEditingMode.Ink;
        }

        private void New_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void SaveManager_OnClick(object sender, RoutedEventArgs e)
        {

        }

        public event EventHandler<EventArgs> OpenScreen1;

        private void btnOpenScreen1_Clicked(object sender, RoutedEventArgs e)
        {
            if (OpenScreen1 != null)
            {
                OpenScreen1(this, new EventArgs());
            }
        }
    }

}
