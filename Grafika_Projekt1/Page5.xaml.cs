using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace Grafika_Projekt1
{
    /// <summary>
    /// Logika interakcji dla klasy Page5.xaml
    /// </summary>
    public partial class Page5 : Page
    {
        Uri filePath;
        BitmapImage loadedImage;
        BitmapImage filteredImage;
        BitmapImage toSave;
        Bitmap copy;

        public Page5()
        {
            InitializeComponent();
        }

        #region Plik
        private void Wczytaj_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Wybierz obraz";
            openFileDialog.Filter = "BMP|*.bmp|GIF|*.gif|JPG|*.jpg;*.jpeg|PNG|*.png|TIFF|*.tif;*.tiff|"
       + "All Graphics Types|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff";

            if (openFileDialog.ShowDialog() == true)
            {
                filePath = new Uri(openFileDialog.FileName);

            }
            loadedImage = new BitmapImage(filePath);
            //image.Source = loadedImage;
            Binaryzacja();
        }

        private void Zapisz_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "BMP|*.bmp|GIF|*.gif|JPG|*.jpg;*.jpeg|PNG|*.png|TIFF|*.tif;*.tiff|"
                    + "All Graphics Types|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff";

            if (saveFileDialog.ShowDialog() == true)
            {
                FileStream saveStream = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate);
                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(filteredImage));
                encoder.Save(saveStream);
                saveStream.Close();
            }
        }
        #endregion

        #region Converter
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

        private void BitmapCopy()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(loadedImage));
                enc.Save(ms);
                Bitmap loaded = new Bitmap(ms);
                copy = new Bitmap(loaded);
            }
        }
        #endregion

        public void Binaryzacja()
        {
            if (loadedImage != null)
            {
                BitmapCopy();

                var data = copy.LockBits(new System.Drawing.Rectangle(0, 0, copy.Width, copy.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb
                );

                var copyData = new byte[data.Stride * data.Height];

                Marshal.Copy(data.Scan0, copyData, 0, copyData.Length);
                // Przerzuci z Bitmapy do tablicy

                for (int i = 0; i < copyData.Length; i += 3)
                {
                    byte r = copyData[i + 0];
                    byte g = copyData[i + 1];
                    byte b = copyData[i + 2];
                    byte mean = (byte)((r + g + b) / 3);
                    copyData[i + 0] =
                    copyData[i + 1] =
                    copyData[i + 2] = mean > 128
                        ? byte.MaxValue
                        : byte.MinValue;
                }

                Marshal.Copy(copyData, 0, data.Scan0, copyData.Length);
                // Przerzuci z tablicy do Bitmapy

                copy.UnlockBits(data);
                loadedImage = BitmapToImage(copy);
                toSave = loadedImage;
                image.Source = loadedImage;
            }
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
