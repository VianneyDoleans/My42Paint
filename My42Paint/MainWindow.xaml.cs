using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace My42Paint
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point currentPoint = new Point();    
        public MainWindow()
        {
            InitializeComponent();
        }

        private new void MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                currentPoint = e.GetPosition(this);
        }

        private new void MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            var line = new Line
            {
                Stroke = SystemColors.WindowFrameBrush,
                X1 = currentPoint.X,
                Y1 = currentPoint.Y,
                X2 = e.GetPosition(this).X,
                Y2 = e.GetPosition(this).Y
            };
            currentPoint = e.GetPosition(this);
            DrawingSheet.Children.Add(line);
            Debug.WriteLine(DrawingSheet.Children.Count);
        }
    }
}
