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
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace SSM
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            KBDHook.LocalHook = false;
            KBDHook.InstallHook();

            KBDHook.KeyDown += new KBDHook.HookKeyPress(Hooks_KeyDown);
            KBDHook.KeyUp += new KBDHook.HookKeyPress(Hooks_KeyUp);

            this.Closed += (s, e) =>
            {
                KBDHook.UnInstallHook();
            };
        }

        void Hooks_KeyUp(LLKHEventArgs e)
        {
            listbox.Items.Insert(0, e.ScanCode + " " + e.Keys + " " + e.IsPressed);
        }

        void Hooks_KeyDown(LLKHEventArgs e)
        {
            listbox.Items.Insert(0, e.ScanCode + " " + e.Keys + " " + e.IsPressed);
            if (e.Keys == Keys.PrintScreen)
            {
                screen();
            }
        }

        private void screen()
        {
            int screenLeft = SystemInformation.VirtualScreen.Left;
            int screenTop = SystemInformation.VirtualScreen.Top;
            int screenWidth = SystemInformation.VirtualScreen.Width;
            int screenHeight = SystemInformation.VirtualScreen.Height;

            string dn = "";
            string name = "ss-" + DateTime.Now.ToString("yyyy.MM.dd-hh.mm.ss");

            if (SystemInformation.MonitorCount == 2)
            {
                if (screenWidth == 3840)
                {
                    screenWidth = 1920;
                    int screenTop2 = screenTop;                   

                    if (screenHeight == 1090)
                    {
                        screenHeight = 1080;
                        screenTop2 = 10;
                    }
                    
                    using (Bitmap bmp = new Bitmap(1920, screenHeight))
                    {
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            String filename = name + "-2" + ".png";
                            Opacity = .0;
                            g.CopyFromScreen(1920, screenTop2, 0, 0, bmp.Size);
                            bmp.Save(filename, ImageFormat.Png);
                            Opacity = 1;
                        }
                    }

                    dn = "-1";
                }
            }

            using (Bitmap bmp = new Bitmap(screenWidth, screenHeight))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    String filename = name + dn + ".png";
                    Opacity = .0;
                    g.CopyFromScreen(screenLeft, screenTop, 0, 0, bmp.Size);
                    bmp.Save(filename, ImageFormat.Png);
                    Opacity = 1;
                }
            }

        }

    }
}
