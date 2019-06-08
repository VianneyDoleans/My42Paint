
using System;
using System.Windows;
using System.Windows.Controls;

namespace SaveManager
{
    /// <summary>
    /// Interaction logic for Save.xaml
    /// </summary>
    public partial class Save : UserControl
    {
        public event EventHandler del;

        private void delete(object sender, RoutedEventArgs e)
        {
            if (del != null) del(this, EventArgs.Empty);
            Console.WriteLine("delete");
        }

        public Save()
        {
            InitializeComponent();
        }
    }
}
