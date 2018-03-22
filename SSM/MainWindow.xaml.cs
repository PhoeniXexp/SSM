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
                       
            _contextMenu = new System.Windows.Forms.ContextMenu();

            _contextMenu.MenuItems.Add("Выход", new EventHandler(_exitMenu));        

            ni = new NotifyIcon()
            {
                Icon = Properties.Resources.PictureWF,
                Visible = true,
                ContextMenu = _contextMenu,
                Text = "ScreenShot Maker"
            };
            this.Hide();
            this.WindowState = WindowState.Minimized;
            
            KBDHook.LocalHook = false;
            KBDHook.InstallHook();

            KBDHook.KeyDown += new KBDHook.HookKeyPress(Hooks_KeyDown);
            KBDHook.KeyUp += new KBDHook.HookKeyPress(Hooks_KeyUp);

            this.Closed += (s, e) =>
            {
                KBDHook.UnInstallHook();
            };
        }

        NotifyIcon ni;
        System.Windows.Forms.ContextMenu _contextMenu;
        private bool ctrl_key = false;
        private bool bsmooth = false;

        private Smooth sm = new Smooth();

        private void Hooks_KeyUp(LLKHEventArgs e)
        {
            if (e.Keys == Keys.RControlKey) ctrl_key = false;
        }

        private void Hooks_KeyDown(LLKHEventArgs e)
        {
            if (e.Keys == Keys.RControlKey) ctrl_key = true;

            if (e.Keys == Keys.PrintScreen)
                if (!bsmooth)
                {
                    if (ctrl_key)
                        smooth();
                    else
                        screen();
                }

            if(bsmooth)
                if (e.Keys == Keys.Escape)
                {
                    sm.Close();
                    bsmooth = false;
                }
        }

        private void smooth()
        {
            bsmooth = true;

            sm = new Smooth();
            sm.Top = 0;
            sm.Left = 0;
            sm.Height = SystemInformation.VirtualScreen.Height;
            sm.Width = SystemInformation.VirtualScreen.Width;
            sm.Show();
        }

        private void _exitMenu(object sender, EventArgs e)
        {
            Stop();
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

        private void Window_Closed(object sender, EventArgs e)
        {
            Stop();
        }

        private void Stop()
        {
            ni.Dispose();
            System.Windows.Application.Current.Shutdown();
        }
    }
}
