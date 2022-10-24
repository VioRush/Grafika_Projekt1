using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace Grafika_Projekt1
{
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    public partial class Page2 : Page
    {
        int r, g, b;

        public Page2()
        {
            InitializeComponent();
            HEXLabel.Text = "rabotajet";
            ambientLight.Color = System.Windows.Media.Color.FromRgb(255,0,0);
        }

        private void Button_Click_Back(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Page1());
        }

        private void ScrollBar_ValueChanged_Vertical(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Cube.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1,0,0), vScroll.Value));
        }

        private void ScrollBar_ValueChanged_Horizontal(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Cube.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), hScroll.Value));
        }

        private void colorPicker_MouseDown(object sender, Syncfusion.Windows.Tools.Controls.SelectedBrushChangedEventArgs e)
        {
            r = colorPicker.Color.R;
            g = colorPicker.Color.G;
            b = colorPicker.Color.B;
            rectangle.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)r, (byte)g, (byte)b));
            //CLabel.Text = string.Format("#{0:X2}{1:X2}{2:X2}", r, g, b);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
