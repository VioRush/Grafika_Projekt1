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
        int[,] values;

        //thinning masks
        int[,] mask1 = { { 0, 0, 0 }, { -1, 1, -1 }, { 1, 1, 1 } };
        int[,] mask2 = { { 1, -1, 0 }, { 1, 1, 0 }, { 1, -1, 0 } };
        int[,] mask3 = { { 1, 1, 1 }, { -1, 1, -1 }, { 0, 0, 0 } };
        int[,] mask4 = { { 0, -1, 1 }, { 0, 1, 1 }, { 0, -1, 1 } };
        int[,] mask5 = { { -1, 0, 0 }, { 1, 1, 0 }, { -1, 1, -1 } };
        int[,] mask6 = { { -1, 1, -1 }, { 1, 1, 0 }, { -1, 0, 0 } };
        int[,] mask7 = { { -1, 1, -1 }, { 0, 1, 1 }, { 0, 0, -1 } };
        int[,] mask8 = { { 0, 0, -1 }, { 0, 1, 1 }, { -1, 1, -1 } };

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

        #region Filters
        private Bitmap Dylatacja(Bitmap copy)
        {
            int[,] mask = new int[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    mask[i, j] = 1;
                }
            }

            values = new int[copy.Height, copy.Width];
            int height = copy.Height;
            int width = copy.Width;
            var pom = new int[copy.Height, copy.Width];
            UstawJedynki(copy);

            for (int i = 1; i < height - 1; i++)
            {
                for (int j = 1; j < width - 1; j++)
                {
                    var liczba = 0;
                    for(int x = i - 1; x <= i + 1; x++)
                    {
                        for (int y = j - 1; y <= j + 1; y++)
                        {
                            if (values[x, y] == 1)
                            {
                                liczba++;
                            }
                        }
                    }
                    if (liczba >= 1) pom[i, j] = 1;
                    else pom[i, j] = 0;
                    
                }
            }

            copy = Pokoloruj(pom, copy);

            return copy;
           
        }

        private Bitmap Pokoloruj(int[,] pom, Bitmap copy)
        {
            for (int i = 0; i < copy.Height; i++)
            {
                for (int j = 0; j < copy.Width; j++)
                {
                    if (pom[i, j] == 1)
                    {
                        copy.SetPixel(j, i, System.Drawing.Color.Black);
                    }
                    else if (pom[i, j] == 0)
                    {
                        copy.SetPixel(j, i, System.Drawing.Color.White);
                    }
                }
            }

            return copy;
        }

        private Bitmap Otwarcie(Bitmap copy)
        {
            //Erozja();
            // Dylatacja();
            return copy;//Dylatacja(Erozja(copy));
        }

        private Bitmap Pocienianie(Bitmap copy)
        {
            List<int[,]> masks = new List<int[,]>();

            masks.Add(mask1);
            masks.Add(mask2);
            masks.Add(mask3);
            masks.Add(mask4);
            masks.Add(mask5);
            masks.Add(mask6);
            masks.Add(mask7);
            masks.Add(mask8);

            values = new int[copy.Height, copy.Width];
            int height = copy.Height;
            int width = copy.Width;
            UstawJedynki(copy);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Console.Write(values[i, j] + " ");
                }
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            var repeat = true;

            do
            {
                repeat = false;

                for (int m = 0; m < 8; m++)
                {
                    for (int i = 1; i < height - 1; i++)
                    {
                        for (int j = 1; j < width - 1; j++)
                        {
                            if (values[i, j] == 1)
                            {
                                var eq = true;
                                int rm = 0, cm = 0;
                                for(int r = i - 1; r <= i + 1; r++)
                                {
                                    cm = 0;
                                    for (int c = j - 1; c <= j + 1; c++)
                                    {
                                        if(values[r,c] != masks[m][rm, cm])
                                        {
                                            if (masks[m][rm, cm] >= 0)
                                            {
                                                Console.WriteLine("not eq: " + values[r, c] + "!=" + masks[m][rm, cm]);
                                                eq = false;
                                            }
                        
                                        }
                                        cm++;
                                    }
                                    rm++;
                                }

                                if (eq == true)
                                {
                                    Console.WriteLine("zmieniono");
                                    values[i, j] = 0;
                                    repeat = true;
                                }
                            }
                        }
                    }
                }

            } while (repeat);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Console.Write(values[i, j] + " ");
                }
                Console.WriteLine();
            }

            copy = Pokoloruj(values, copy);

            return copy;
        }

        #endregion

        private void Do_Click(object sender, RoutedEventArgs e)
        {
            if (DylatacjaRadioButton.IsChecked == true)
            {
                BitmapCopy();
                filteredImage = BitmapToImage(Dylatacja(copy));
                toSave = filteredImage;
                image2.Source = filteredImage;
            }
            else if (OtwarcieRadioButton.IsChecked == true)
            {
                BitmapCopy();
                filteredImage = BitmapToImage(Otwarcie(copy));
                toSave = filteredImage;
                image2.Source = filteredImage;
            }
            else if (PocienianieRadioButton.IsChecked == true)
            {
                BitmapCopy();
                filteredImage = BitmapToImage(Pocienianie(copy));
                toSave = filteredImage;
                image2.Source = filteredImage;
            }
        }

        private void UstawJedynki(Bitmap copy)
        {
            for (int i = 0; i < copy.Height; i++)
            {
                for (int j = 0; j < copy.Width; j++)
                {
                    System.Drawing.Color kolor = copy.GetPixel(j, i);
                    if (kolor.R == 0 && kolor.G == 0 && kolor.B == 0)
                    {
                        values[i, j] = 1;
                    }
                    else
                    {
                        values[i, j] = 0;
                    }
                }
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
