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
        public event EventHandler<EventArgs> saveFile;
        public event EventHandler<EventArgs> openFile;
        public event EventHandler<EventArgs> newFile;
        public event EventHandler<EventArgs> back;
        public event EventHandler<EventArgs> export;
        public Dictionary<EventHandler<EventArgs>, String> saves;

        public SaveManage()
        {
            InitializeComponent();
            saves = new Dictionary<EventHandler<EventArgs>, string>();
            ReadFileSaves();
            localMenu.newFile += new EventHandler(NewFile);
            localMenu.openFile += new EventHandler(OpenFile);
            localMenu.back += new EventHandler(btnOpenScreen1_Clicked);
            localMenu.export += new EventHandler(Export);
            localMenu.saveFile += new EventHandler(SaveFile);
        }


        public void ReadFileSaves()
        {
            SavesGrid.Children.Clear();
           // TODO : change path here
           String pathFile = "C:/Users/vianneydoleans/Documents/My42Paint/SaveManager/saves.txt";
            Console.WriteLine("read file");
            using (StreamReader file = new StreamReader(pathFile))
            {
                string ln;

                while ((ln = file.ReadLine()) != null)
                {
                    if (ln != null || ln != "\n")
                        addItem(ln);
                }
                file.Close();
            }
        }

        public void delete(object sender, EventArgs e)
        {

        }

        public void addItem(string path)
        {
            try
            {
                Save save = new Save();
                save.name.Content = path;//System.IO.Path.GetFileNameWithoutExtension(path);
                save.image.Source = new BitmapImage(new Uri(path, UriKind.Absolute));
                SavesGrid.Children.Add(save);
            }
            catch(Exception)
            {

            }
            //save.del += new EventHandler(delete, path);
            //saves.Add(, save);
        }

        protected void OpenFile(object sender, EventArgs e)
        {
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
            ReadFileSaves();
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
