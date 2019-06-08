using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;
using Size = System.Windows.Size;

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
        
        public static void ExportToPng(string path, InkCanvas  canvas)
        {
            if (path == null) return;
            var size = new Size(canvas.ActualWidth, canvas.ActualWidth);
            // Measure the canvas
            canvas.Measure(size);
            // Create a render bitmap and push the canvas to it
            var renderBitmap = new RenderTargetBitmap((int)size.Width, (int)size.Height,96d,96d, PixelFormats.Pbgra32);
            renderBitmap.Render(canvas);
            // Create a file stream for saving image
            using (var outStream = new FileStream(path, FileMode.Create))
            {
                // Use png encoder for our data
                var encoder = new PngBitmapEncoder();
                // push the rendered bitmap to it
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                // save the data to the stream
                encoder.Save(outStream);
            }
        }

        public static void ExportToPng(string path, Image image)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)image.Source));
            if (File.Exists(path))
                return;
            using (var stream = new FileStream(path, FileMode.Create))
            {
                encoder.Save(stream);
                stream.Close();
            }
        }
    }
}