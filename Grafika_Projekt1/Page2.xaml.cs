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
        int r = 0, g = 0, b = 0;
        float c = 0, m = 0, y = 0, k = 1;
        bool draw = true;

        public Page2()
        {
            InitializeComponent();
            BLabel_rgb.Text = "" + b.ToString();
            GLabel_rgb.Text = "" + g.ToString();
            RLabel_rgb.Text = "" + r.ToString();
            CLabel_cmyk.Text = "" + c.ToString();
            MLabel_cmyk.Text = "" + m.ToString();
            YLabel_cmyk.Text = "" + y.ToString();
            KLabel_cmyk.Text = "" + k.ToString();
            draw = true;
            SetCubeColor();
        }

        private void SetCubeColor()
        {
            int red=256, blue=0;
            Bitmap resultBitmap = new Bitmap(256, 256);


            for (int i = 0; i < resultBitmap.Height; i++)
            {
                red--;
                blue = 0;
                for (int j = 0; j < resultBitmap.Width; j++)
                {
                    resultBitmap.SetPixel(j, i, System.Drawing.Color.FromArgb(red, 255, blue));
                    blue++;
                }
            }

           // rectangle.Fill = new ImageBrush { ImageSource = BitmapToImage(resultBitmap) };
           fBrush.ImageSource = BitmapToImage(resultBitmap);

            int green = -1;
            blue = 0;


            for (int i = 0; i < resultBitmap.Height; i++)
            {
                green++;
                blue = 0;
                for (int j = 0; j < resultBitmap.Width; j++)
                {
                    resultBitmap.SetPixel(j, i, System.Drawing.Color.FromArgb(255,green, blue));
                    blue++;
                }
            }

            // rectangle.Fill = new ImageBrush { ImageSource = BitmapToImage(resultBitmap) };
            tpBrush.ImageSource = BitmapToImage(resultBitmap);

            green = 256;
            red = 0;


            for (int i = 0; i < resultBitmap.Height; i++)
            {
                green--;
                red = 0;
                for (int j = 0; j < resultBitmap.Width; j++)
                {
                    resultBitmap.SetPixel(j, i, System.Drawing.Color.FromArgb(red, green, 255));
                    red++;
                }
            }

            // rectangle.Fill = new ImageBrush { ImageSource = BitmapToImage(resultBitmap) };
            bBrush.ImageSource = BitmapToImage(resultBitmap);
            //CubeBrush.ImageSou

            green = -1;
            red = 0;


            for (int i = 0; i < resultBitmap.Height; i++)
            {
                green++;
                red = 0;
                for (int j = 0; j < resultBitmap.Width; j++)
                {
                    resultBitmap.SetPixel(j, i, System.Drawing.Color.FromArgb(red, green, 0));
                    red++;
                }
            }

            // rectangle.Fill = new ImageBrush { ImageSource = BitmapToImage(resultBitmap) };
            lbBrush.ImageSource = BitmapToImage(resultBitmap);

            green = 256;
            blue = 0;


            for (int i = 0; i < resultBitmap.Height; i++)
            {
                green--;
                blue = 0;
                for (int j = 0; j < resultBitmap.Width; j++)
                {
                    resultBitmap.SetPixel(j, i, System.Drawing.Color.FromArgb(0, green, blue));
                    blue++;
                }
            }

            // rectangle.Fill = new ImageBrush { ImageSource = BitmapToImage(resultBitmap) };
            nBrush.ImageSource = BitmapToImage(resultBitmap);

            red = -1;
            blue = 0;


            for (int i = 0; i < resultBitmap.Height; i++)
            {
                red++;
                blue = 0;
                for (int j = 0; j < resultBitmap.Width; j++)
                {
                    resultBitmap.SetPixel(j, i, System.Drawing.Color.FromArgb(red, 0, blue));
                    blue++;
                }
            }

            // rectangle.Fill = new ImageBrush { ImageSource = BitmapToImage(resultBitmap) };
            CubeBrush.ImageSource = BitmapToImage(resultBitmap);
        }

        public BitmapImage BitmapToImage(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }

        public Bitmap ImageToBitmap(BitmapSource bitmapSource)
        {
            if (bitmapSource != null)
            {
                var width = bitmapSource.PixelWidth;
                var height = bitmapSource.PixelHeight;
                var stride = width * ((bitmapSource.Format.BitsPerPixel + 7) / 8);
                var memoryBlockPointer = Marshal.AllocHGlobal(height * stride);
                bitmapSource.CopyPixels(new Int32Rect(0, 0, width, height), memoryBlockPointer, height * stride, stride);
                var bitmap = new Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, memoryBlockPointer);
                return bitmap;
            }
            else
                return null;
        }

        #region RGB&CMYK

        public void RGBToCMYK()
        {
            float red = (float)r/255,
                  green = (float)g/255,
                  blue = (float)b/255;

            k = (float)Math.Round(Math.Min(Math.Min(1 - red, 1 - green), 1 - blue), 2);
            c = (float)Math.Round(Math.Max((1 - red - k) / (1 - k),0),2);
            m = (float)Math.Round(Math.Max((1 - green - k) / (1 - k),0),2);
            y = (float)Math.Round(Math.Max((1 - blue - k) / (1 - k),0),2);
            
           // cScroll.Value = c * 100;
            //mScroll.Value = m * 100;
            //yScroll.Value = y * 100;
            //kScroll.Value = k * 100;
        }

        public void CMYKToRGB()
        {
            r = (int)((1 - Math.Min(1, c * (1 - k) + k)) * 255);
            g = (int)((1 - Math.Min(1, m * (1 - k) + k)) * 255);
            b = (int)((1 - Math.Min(1, y * (1 - k) + k)) * 255);
        }

        public void SetRGBValues()
        {
            BLabel_rgb.Text = b.ToString();
            GLabel_rgb.Text = g.ToString();
            RLabel_rgb.Text = r.ToString();

            rScroll.Value = r;
            gScroll.Value = g;
            bScroll.Value = b;
            //ColorView.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)r, (byte)g, (byte)b));
        }

        public void SetCMYKValues()
        {
           CLabel_cmyk.Text = "" + Math.Round(c,2).ToString();
           MLabel_cmyk.Text = "" + Math.Round(m, 2).ToString();
           YLabel_cmyk.Text = "" + Math.Round(y, 2).ToString();
           KLabel_cmyk.Text = "" + Math.Round(k, 2).ToString();
           
           cScroll.Value = Math.Round(c, 2) * 100;
           mScroll.Value = Math.Round(m, 2) * 100;
           yScroll.Value = Math.Round(y, 2) * 100;
           kScroll.Value = Math.Round(k, 2) * 100;
            //ColorView.Background = new SolidColorBrush(System.Windows.Media.Color.);  //?
        }

        private void ConvertFromRgb()
        {
            draw = false;
            RGBToCMYK();
            SetCMYKValues();
            FillRectangle((byte)r, (byte)g, (byte)b);
            draw = true;
        }

        private void ConvertFromCmyk()
        {
            draw = false;
            CMYKToRGB();
            SetRGBValues();
            FillRectangle((byte)r, (byte)g, (byte)b);
            draw = true;
        }
        #endregion

        #region TextChanged
        private void RLabel_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (draw)
                {
                    r = Int32.Parse(RLabel_rgb.Text);
                    g = Int32.Parse(GLabel_rgb.Text);
                    b = Int32.Parse(BLabel_rgb.Text);
                    ConvertFromRgb();
                }
                
            }
            catch
            {

            }
        }

        private void GLabel_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (draw)
                {
                    r = Int32.Parse(RLabel_rgb.Text);
                    g = Int32.Parse(GLabel_rgb.Text);
                    b = Int32.Parse(BLabel_rgb.Text);
                    ConvertFromRgb();
                }
            }
            catch
            {

            }
           
        }

        private void BLabel_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (draw)
                {
                    r = Int32.Parse(RLabel_rgb.Text);
                    g = Int32.Parse(GLabel_rgb.Text);
                    b = Int32.Parse(BLabel_rgb.Text);
                    ConvertFromRgb();
                }
            }
            catch
            {

            }
        }

        private void CLabel_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (draw)
                {
                    c = float.Parse(CLabel_cmyk.Text);
                    m = float.Parse(MLabel_cmyk.Text);
                    y = float.Parse(YLabel_cmyk.Text);
                    k = float.Parse(KLabel_cmyk.Text);
                    ConvertFromCmyk();
                }
            }
            catch
            {

            }
           
        }

        private void MLabel_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (draw)
                {
                    c = float.Parse(CLabel_cmyk.Text);
                    m = float.Parse(MLabel_cmyk.Text);
                    y = float.Parse(YLabel_cmyk.Text);
                    k = float.Parse(KLabel_cmyk.Text);
                    ConvertFromCmyk();
                }
             
            }
            catch
            {

            }
        }

        private void YLabel_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (draw)
                {
                    c = float.Parse(CLabel_cmyk.Text);
                    m = float.Parse(MLabel_cmyk.Text);
                    y = float.Parse(YLabel_cmyk.Text);
                    k = float.Parse(KLabel_cmyk.Text);
                    ConvertFromCmyk();
                }
            }
            catch
            {

            }
        }

        private void KLabel_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (draw)
                {
                    c = float.Parse(CLabel_cmyk.Text);
                    m = float.Parse(MLabel_cmyk.Text);
                    y = float.Parse(YLabel_cmyk.Text);
                    k = float.Parse(KLabel_cmyk.Text);
                    ConvertFromCmyk();
                }
            }
            catch
            {

            }
        }

        #endregion

        #region ScrollValueChanged
        private void rScroll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RLabel_rgb.Text = rScroll.Value.ToString();
        }

        private void gScroll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GLabel_rgb.Text = gScroll.Value.ToString();
        }

        private void bScroll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            BLabel_rgb.Text = bScroll.Value.ToString();
        }

        private void cScroll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CLabel_cmyk.Text = (cScroll.Value / 100).ToString();
        }

        private void mScroll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MLabel_cmyk.Text = (mScroll.Value / 100).ToString();
        }

        private void yScroll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            YLabel_cmyk.Text = (yScroll.Value / 100).ToString();
        }

        private void kScroll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            KLabel_cmyk.Text = (kScroll.Value / 100).ToString();
        }
        #endregion


        private void ScrollBar_ValueChanged_Vertical(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Cube.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1,0,0), vScroll.Value));
        }

        private void front_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ScrollBar_ValueChanged_Horizontal(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Cube.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), hScroll.Value));
        }
        
        private void colorPicker_MouseDown(object sender, Syncfusion.Windows.Tools.Controls.SelectedBrushChangedEventArgs e)
        {
            r = (int)colorPicker.Color.R;
            RLabel_rgb.Text = r.ToString();
            rScroll.Value = r;
            g = (int)colorPicker.Color.G;
            GLabel_rgb.Text = g.ToString();
            gScroll.Value = g;
            b = (int)colorPicker.Color.B;
            BLabel_rgb.Text = b.ToString();
            bScroll.Value = b;
            Console.WriteLine(r + " " + g + " " + b);
            FillRectangle((byte)r, (byte)g, (byte)b);
            //SetRGBValues();
            RGBToCMYK();
            SetCMYKValues();
            HEXLabel.Text = string.Format("#{0:X2}{1:X2}{2:X2}", r, g, b).ToString();
        }

        private void FillRectangle(byte r, byte g, byte b)
        {
            rectangle.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)r, (byte)g, (byte)b));
            HEXLabel.Text = string.Format("#{0:X2}{1:X2}{2:X2}", r, g, b).ToString();
        }

        private void Button_Click_Back(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Page1());
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
