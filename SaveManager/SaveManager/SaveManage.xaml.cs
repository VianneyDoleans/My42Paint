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

        public event EventHandler<EventArgs> saveFile;
        public event EventHandler<EventArgs> openFile;
        public event EventHandler<EventArgs> newFile;
        public event EventHandler<EventArgs> back;
        public event EventHandler<EventArgs> export;

        public SaveManage()
        {
            InitializeComponent();
            ReadFileSaves("C:/Users/vianneydoleans/Documents/SaveManager-My42Paint/SaveManager/saves.txt");
            localMenu.newFile += new EventHandler(NewFile);
            localMenu.openFile += new EventHandler(OpenFile);
            localMenu.back += new EventHandler(btnOpenScreen1_Clicked);
            localMenu.export += new EventHandler(Export);
            localMenu.saveFile += new EventHandler(SaveFile);
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
            /*Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = openFileDlg.ShowDialog();
            if (result == true)
            {
                String path = openFileDlg.FileName;
            }*/
            openFile?.Invoke(this, e);
            Console.WriteLine("parent open");
        }

        private void NewFile(object sender, EventArgs e)
        {
            Console.WriteLine("parent New");
            newFile?.Invoke(this, e);
        }

        public void SaveFile(object sender, EventArgs e)
        {
            Console.WriteLine("parent save");
            saveFile?.Invoke(this, e);
        }

        private void Export(object sender, EventArgs e)
        {
            Console.WriteLine("parent export");
            export?.Invoke(this, e);
        }

        public event EventHandler<EventArgs> OpenScreen1;

        private void btnOpenScreen1_Clicked(object sender, EventArgs e)
        {
            if (OpenScreen1 != null)
            {
                OpenScreen1(this, new EventArgs());
            }
        }
    }
}
