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
           

        }
        
    }
}
