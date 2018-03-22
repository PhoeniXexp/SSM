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
using System.Windows.Shapes;

namespace SSM
{
    /// <summary>
    /// Логика взаимодействия для Smooth.xaml
    /// </summary>
    public partial class Smooth : Window
    {
        public Smooth()
        {
            InitializeComponent();
        }

        private bool _canDraw;
        private double _startX, _startY;

        public Point p1;
        public Point p2;

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _canDraw = true;

            Point m = e.GetPosition(null);
            _startX = m.X;
            _startY = m.Y;
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _canDraw = false;

            p1.X = (double)rect.GetValue(Canvas.LeftProperty);
            p1.Y = (double)rect.GetValue(Canvas.TopProperty);

            p2.X = rect.Width;
            p2.Y = rect.Height;

            rect.Visibility = Visibility.Collapsed;

            this.Close();
        }

        private void sm_window_MouseMove(object sender, MouseEventArgs e)
        {
            Point m = e.GetPosition(null);

            if (!_canDraw) return;

            double x = Math.Min(_startX, m.X);
            double y = Math.Min(_startY, m.Y);

            double mx = Math.Max(_startX, m.X);
            double my = Math.Max(_startY, m.Y);

            rect.SetValue(Canvas.LeftProperty, x);
            rect.SetValue(Canvas.TopProperty, y);

            rect.Width = Math.Abs(m.X - _startX);
            rect.Height = Math.Abs(m.Y - _startY);

            if (rect.Visibility != Visibility.Visible)
                rect.Visibility = Visibility.Visible;
        }

    }
}
