using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace My42Paint.Window
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class Windows : UserControl
    {
        public Windows()
        {
            InitializeComponent();
        }

        public event EventHandler<EventArgs> OpenScreen1;
        public event EventHandler<EventArgs> OpenScreen2;

        private void btnOpenScreen1_Clicked(object sender, RoutedEventArgs e)
        {
            if (OpenScreen1 != null)
            {
                OpenScreen1(this, new EventArgs());
            }
        }

        private void btnOpenScreen2_Clicked(object sender, RoutedEventArgs e)
        {
            if (OpenScreen2 != null)
            {
                OpenScreen2(this, new EventArgs());
            }
        }
    }
}
