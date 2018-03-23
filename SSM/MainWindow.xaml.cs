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
using System.IO;
using System.Diagnostics;

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

            second_start();
                       
            _contextMenu = new System.Windows.Forms.ContextMenu();

            _contextMenu.MenuItems.Add("Настройки", new EventHandler(new_settings));
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

            init_settings();

            ni.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(open_folder);
        }

        NotifyIcon ni;
        System.Windows.Forms.ContextMenu _contextMenu;
        private bool ctrl_key = false;
        private bool bsmooth = false;

        private List<string> _settings = new List<string>();

        private Smooth sm = new Smooth();

        private void second_start()
        {
            Process proc = Process.GetCurrentProcess();
            int curProc = proc.Id;

            Process[] procs = Process.GetProcessesByName("SSM");
            foreach (Process pr in procs)
            {
                if (pr.Id != curProc)
                {
                    if (pr.MainModule.FileVersionInfo.FileDescription == "SSM")
                    {
                        Process.GetCurrentProcess().Kill();
                    }
                }
            }
        }

        private void open_folder(object sender, EventArgs e)
        {
            Process.Start(_settings[1]);
        }

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

        private void new_settings(object sender, EventArgs e)
        {
            Settings sett = new Settings(_settings);
            sett.ShowDialog();

            _settings = sett.settings;

            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SSM\\";
            string file = path + "settings.ini";

            using (StreamWriter writer = new StreamWriter(file))
            {
                writer.WriteLine("MultiDisplay={0}", (_settings[0] == "1") ? "True" : "False");
                writer.WriteLine("Path={0}", _settings[1]);
                writer.WriteLine("AutoBoot={0}", (_settings[2] == "1") ? "True" : "False");
            }
        }

        private void init_settings()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SSM\\";
            string file = path + "settings.ini";
            _settings = new List<string>();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);

                using (StreamWriter writer = new StreamWriter(file))
                {
                    writer.WriteLine("MultiDisplay=True");
                    writer.WriteLine("Path=" + Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\Screenshots");
                    writer.WriteLine("AutoBoot=False");
                }
            }

            using (StreamReader reader = new StreamReader(file))
            {
                string line;
                line = reader.ReadLine();
                line = (line.Split('=')[1] == "True") ? "1" : "0";
                _settings.Add(line);

                line = reader.ReadLine();
                _settings.Add(line.Split('=')[1]);

                line = reader.ReadLine();
                line = (line.Split('=')[1] == "True") ? "1" : "0";
                _settings.Add(line);
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
            sm.ShowDialog();

            string path = _settings[1];
            string name = path + "\\ss-" + DateTime.Now.ToString("yyyy.MM.dd-hh.mm.ss");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            save((int)sm.p2.X, (int)sm.p2.Y, (int)sm.p1.X, (int)sm.p1.Y, name);

            bsmooth = false;
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

            string path = _settings[1];
            string name = path + "\\ss-" + DateTime.Now.ToString("yyyy.MM.dd-hh.mm.ss");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            save(screenWidth, screenHeight, screenLeft, screenTop, name);

            if (_settings[0] == "1")
            {
                if (SystemInformation.MonitorCount == 2)
                {
                    if (screenWidth == 3840)
                    {
                        int screenTop2 = 0;
                        int screenHeight2 = screenHeight;

                        if (screenHeight == 1090)
                        {
                            screenHeight2 = 1080;
                            screenTop2 = 10;
                        }

                        save(1920, screenHeight2, screenLeft, screenTop, name, "-1");
                        save(1920, screenHeight2, 1920, screenTop2, name, "-2");

                    }
                }
            }
        }

        private void save(int width, int height, int x, int y, string name, string dn = "")
        {
            using (Bitmap bmp = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    String filename = name + dn + ".png";
                    Opacity = .0;
                    g.CopyFromScreen(x, y, 0, 0, bmp.Size);
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
