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

namespace SaveManager
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        public event EventHandler printFile;
        public event EventHandler openFile;
        public event EventHandler newFile;
        public event EventHandler back;
        public event EventHandler export;

        public Menu()
        {
            InitializeComponent();
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            if (openFile != null) openFile(this, EventArgs.Empty);
            Console.WriteLine("open");
        }

        private void NewFile(object sender, RoutedEventArgs e)
        {
            if (newFile != null) newFile(this, EventArgs.Empty);
            Console.WriteLine("New");
        }

        private void PrintFile(object sender, RoutedEventArgs e)
        {
            if (printFile != null) printFile(this, EventArgs.Empty);
            Console.WriteLine("print");
        }

        private void ExportFile(object sender, RoutedEventArgs e)
        {
            if (export != null) export(this, EventArgs.Empty);
            Console.WriteLine("export");
        }

        private void BackFile(object sender, RoutedEventArgs e)
        {
            if (back != null) back(this, EventArgs.Empty);
            Console.WriteLine("back");
        }
    }
}
