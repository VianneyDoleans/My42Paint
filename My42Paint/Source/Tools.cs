using System.Drawing;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;

namespace My42Paint.Source
{
    public static class Tools
    {
        public static void LoadImage(InkCanvas drawingSheet, MainWindow mainWindow)
        {
            var thread = new Thread(() =>
            {
                var image = System.Drawing.Image.FromFile(@"C:\Users\Clement\Pictures\1427428321_large.jpg");
                var bitmap = new Bitmap(image);
                var strokeCollection = new StrokeCollection();
                for (var i = 0; i < bitmap.Width; i++)
                {
                    for (var j = 0; j < bitmap.Height; j++)
                    {
                        var pxl = bitmap.GetPixel(i, j);
                        var stylusPointCollection = new StylusPointCollection();
                        stylusPointCollection.Add(new StylusPoint(i + 100, j + 100));
                        var stroke = new Stroke(stylusPointCollection);
                        stroke.DrawingAttributes.Color = System.Windows.Media.Color.FromArgb(pxl.A, pxl.R, pxl.G, pxl.B);
                        strokeCollection.Add(stroke);
                    }
                }
                mainWindow.Dispatcher.Invoke(() => { drawingSheet.Strokes.Add(strokeCollection);});
            });
            thread.Start();
        }
    }
}