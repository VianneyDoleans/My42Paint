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
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Path = System.IO.Path;
using XamlReader = System.Windows.Markup.XamlReader;
using XamlWriter = System.Windows.Markup.XamlWriter;
using System.Windows.Media.Animation;

namespace My42Paint.Source
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        Paint Paint;
        SaveManager.SaveManage SaveManage;

        public MainWindow()
        {
            InitializeComponent();
            Paint = new Paint();
            SaveManage = new SaveManager.SaveManage();
            Paint.OpenScreen1 += new EventHandler<EventArgs>(MyUserControl_OpenScreen1);
            SaveManage.OpenScreen1 += new EventHandler<EventArgs>(MyUserControl_OpenScreen2);
            SaveManage.openFile += new EventHandler<EventArgs>(open);
            SaveManage.newFile += new EventHandler<EventArgs>(newFile);
            SaveManage.saveFile += new EventHandler<EventArgs>(save);
            SaveManage.export += new EventHandler<EventArgs>(Export);

            brdMain.Child = Paint;
        }

        void Export(object sender, EventArgs e)
        {
            Paint.Export();
        }

        void newFile(object sender, EventArgs e)
        {
            Paint.NewPaint();
        }

        void open(object sender, EventArgs e)
        {
            Paint.Load();
        }

        void save(object sender, EventArgs e)
        {
            Paint.Save();
        }

        void MyUserControl_OpenScreen1(object sender, EventArgs e)
        {
            brdMain.Child = SaveManage;
        }

        void MyUserControl_OpenScreen2(object sender, EventArgs e)
        {
            brdMain.Child = Paint;
        }
    }
}