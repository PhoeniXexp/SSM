﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SSM
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings(List<string> param)
        {
            InitializeComponent();
            settings = param;
            _settings = new List<string>(param);

            checkbox2.IsChecked = (settings[1] == "1") ? true : false;
            textbox.Text = settings[0];
        }

        public List<string> settings;
        private List<string> _settings;

        private void btn_obzor_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    textbox.Text = fbd.SelectedPath;
                }
            }

            settings[0] = textbox.Text;
        }

        private void checkbox2_Click(object sender, RoutedEventArgs e)
        {
            settings[1] = (checkbox2.IsChecked == true) ? "1" : "0";
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            bool b = (checkbox2.IsChecked == true) ? true : false;

            autorun(b);

            this.Close();
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            settings = _settings;
            this.Close();
        }

        private void autorun(bool inst)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SSM\\SSM.exe";

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

            bool find = false;

            foreach (string name in key.GetValueNames()) 
            {
                if (name == "SSM")
                    find = true;                
            }

            if (inst)
            {
                key.SetValue("SSM", path);
            }
            else
            {
                if (find)
                {
                    key.DeleteValue("SSM");
                }
            }

            key.Close();
        }
    }
}
