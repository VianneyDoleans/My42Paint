using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SaveManage : UserControl
    {
        String selectedFile;


        public SaveManage()
        {
            InitializeComponent();
            ReadFileSaves("C:/Users/vianneydoleans/Documents/SaveManager-My42Paint/SaveManager/saves.txt");
            localMenu.printFile += new EventHandler(PrintFile);
            localMenu.newFile += new EventHandler(NewFile);
            localMenu.openFile += new EventHandler(OpenFile); 
        }


        public void ReadFileSaves(string pathFile)
        {
            Console.WriteLine("read file");
            using (StreamReader file = new StreamReader(pathFile))
            {
                string ln;

                while ((ln = file.ReadLine()) != null)
                {
                    addItem(ln);
                }
                file.Close();
            }
        }

        public void addItem(string path)
        {
            Save save = new Save();

            save.name.Content = System.IO.Path.GetFileNameWithoutExtension(path);
            save.image.Source = new BitmapImage(new Uri(path, UriKind.Absolute));
            SavesGrid.Children.Add(save);
        }

        protected void OpenFile(object sender, EventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = openFileDlg.ShowDialog();
            if (result == true)
            {
                String path = openFileDlg.FileName;
            }

            Console.WriteLine("parent open");
        }

        private void NewFile(object sender, EventArgs e)
        {
            Console.WriteLine("parent New");
        }

        private void PrintFile(object sender, EventArgs e)
        {
            /*PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(selectedFile, "My First Print Job");
            }*/
        }

    }
}
