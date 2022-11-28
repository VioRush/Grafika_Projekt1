using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Grafika_Projekt1
{
    /// <summary>
    /// Interaction logic for Page4.xaml
    /// </summary>
    public partial class Page4 : Page
    {
        BitmapImage loadedImage;
        private System.Windows.Point origin;
        private System.Windows.Point start;
        private double scale;
        private string lastLoadedPath;
        private const int BLACK = (255 << 24) | (0 << 16) | (0 << 8) | 0,
                          WHITE = (255 << 24) | (255 << 16) | (255 << 8) | 255;

        int CompressionLevel;
        int[] histogram = null;

        public Page4()
        {
            InitializeComponent();
        }

        #region Plik

        private void Wczytaj_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Wczytaj";
            dialog.InitialDirectory = lastLoadedPath == null ?
                Directory.GetCurrentDirectory() :
                Path.GetDirectoryName(lastLoadedPath);
            dialog.Filter = "Image files|*.ppm;*.jpeg;*.jpg";
            dialog.FilterIndex = 1;
            if (dialog.ShowDialog() != true) return;
            var path = dialog.FileName;
            lastLoadedPath = path;
            LoadFile(path);
            ResetTransform();
            loadedImage = new BitmapImage(new Uri(dialog.FileName));
        }

        private void Zapisz()
        {
            if (Image.Source == null)
            {
                System.Windows.MessageBox.Show("Aby zapisać, wczytaj obraz.", "Brak obrazu!");

            }
            else if (Image.Source != null)
            {
                Microsoft.Win32.SaveFileDialog save = new Microsoft.Win32.SaveFileDialog();
                save.Title = "Zapisywanie jako";
                save.Filter = "JPEG (*.jpeg)|*.jpeg";
                save.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                JpegBitmapEncoder encoder;

                if (save.ShowDialog() == true)
                {
                    string ext = System.IO.Path.GetExtension(save.FileName);
                    FileStream filestream = new FileStream(save.FileName, FileMode.Create);

                    switch (ext)
                    {
                        case ".jpeg":
                            encoder = new JpegBitmapEncoder();
                            encoder.QualityLevel = CompressionLevel;
                            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)Image.Source));
                            encoder.Save(filestream);
                            break;
                        case null:
                            Console.WriteLine("Błąd zapisu pliku");
                            break;
                    }
                    filestream.Close();
                }
            }

        }

        private void WysokaJakość_Click(object sender, RoutedEventArgs e)
        {
            CompressionLevel = 99;
            Zapisz();
        }

        private void ŚredniaJakość_Click(object sender, RoutedEventArgs e)
        {
            CompressionLevel = 50;
            Zapisz();
        }

        private void NiskaJakość_Click(object sender, RoutedEventArgs e)
        {
            CompressionLevel = 1;
            Zapisz();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void LoadFile(string path)
        {
            if (!File.Exists(path))
            { MessageBox.Show($"Plik {path} nie istnieje."); return; }
            using (FileStream fs = new FileStream(path, FileMode.Open,
                FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan))
            {
                var ext = Path.GetExtension(path);
                string errMsg = $"Błąd podczas wczytywania pliku {path}.";
                /*if (ext == ".ppm")
                {
                    WriteableBitmap bmp = new MemoryBufferPPM(fs).DecodeFile();
                    if (bmp == null) MessageBox.Show(errMsg);
                    else SetImageSource(bmp);
                }
                else */
                if (ext == ".jpg" || ext == ".jpeg")
                {
                    try
                    {
                        // nie dajemy BitmapCreateOptions.PreservePixelFormat, bo wtedy dekoder tworzy dec.Frames[0] jako bitmapę z 24 (zamiast 32) bitami na piksel
                        var dec = new JpegBitmapDecoder(fs, BitmapCreateOptions.None,
                            BitmapCacheOption.OnLoad);
                        // poniższy kod nie wykona się, gdy konstruktor JpegBitmapDecoder wyrzuci wyjątek
                        // var bmp = new WriteableBitmap(dec.Frames[0]); - nie działa, bo tak stworzonego WriteableBitmap nie można edytować poprzez BackBuffer
                        var frm = dec.Frames[0];
                        int wid = frm.PixelWidth, hei = frm.PixelHeight;
                        var bmp = new WriteableBitmap(wid, hei, 96, 96, PixelFormats.Bgra32, null);
                        // kopiujemy obraz zdekodowany z JPG do WriteableBitmap, aby później móc go szybko edytować
                        var rect = new Int32Rect(0, 0, wid, hei);
                        int stride = wid * frm.Format.BitsPerPixel / 8, size = stride * hei;
                        frm.CopyPixels(rect, bmp.BackBuffer, size, stride);
                        bmp.Lock();
                        bmp.AddDirtyRect(rect);
                        bmp.Unlock();
                        SetImageSource(bmp);
                    }
                    catch (Exception ex)
                    { MessageBox.Show(errMsg + " (" + ex.Message + ")"); }
                }
                else MessageBox.Show("Format nieobsługiwany.");
            }
        }

        private void LoadLast_Click(object sender, RoutedEventArgs e)
        {
            if (lastLoadedPath != null) LoadFile(lastLoadedPath);
        }

        #endregion

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

        public void Histogram(BitmapImage image)
        {
            Bitmap copy;
            using (MemoryStream ms = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(image));
                enc.Save(ms);
                Bitmap loaded = new Bitmap(ms);
                copy = new Bitmap(loaded);
            }

            var data = copy.LockBits(
                new System.Drawing.Rectangle(0, 0, copy.Width, copy.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb
            );
            var copyData = new byte[data.Stride * data.Height];

            Marshal.Copy(data.Scan0, copyData, 0, copyData.Length);

            histogram = new int[256];
            foreach (byte i in copyData)
            {
                ++histogram[i];
            }

            double max = histogram.Max();

            for (int i = 0; i < histogram.Length; i++)
            {
                histogram[i] = (int)(histogram[i] / max * data.Height);
            }

            copyData = new byte[copyData.Length];
            for (int i = 0; i < copyData.Length; i++)
            {
                copyData[i] = 255;
            }

            for (int i = 0; i < histogram.Length; i++)
            {
                for (int j = 0; j < histogram[i]; j++)
                {
                    int index = i * 3 + (data.Height - 1 - j) * data.Stride;

                    copyData[index + 0] =
                    copyData[index + 1] =
                    copyData[index + 2] = 0;
                }
            }

            Marshal.Copy(copyData, 0, data.Scan0, copyData.Length);
            // Przerzuci z tablicy do Bitmapy

            copy.UnlockBits(data);
        }

        private void ResetTransform()
        {
            Image.RenderTransform = Transform.Identity;
            scale = 1;
        }

        private void SetImageSource(WriteableBitmap bmp)
        {
            int bmpWid = bmp.PixelWidth, bmpHei = bmp.PixelHeight;
            double imgWid, imgHei;
            if (bmpWid > Image.MaxWidth)
            {
                imgWid = Image.MaxWidth;
                imgHei = (imgWid * bmpHei) / bmpWid;
                if (imgHei > Image.MaxHeight)
                {
                    imgHei = Image.MaxHeight;
                    imgWid = (imgHei * bmpWid) / bmpHei;
                }
            }
            else
            {
                imgWid = bmpWid;
                imgHei = (imgWid * bmpHei) / bmpWid;
                if (imgHei > Image.MaxHeight)
                {
                    imgHei = Image.MaxHeight;
                    imgWid = (imgHei * bmpWid) / bmpHei;
                }
            }
            Image.Width = imgWid;
            Image.Height = imgHei;
            Image.Source = bmp;
        }

        private void PerformEqualization()
        {
            Histogram(loadedImage);
            double[] dystrybuanta = new double[256];
            byte[] LUT = new byte[256];
            double minVal = 0;
            double suma = 0;
            int k = 0;

            for (int i = 0; i < 256; i++)
            {
                k += histogram[i];
            }
            Console.WriteLine(k);

            for (int i = 0; i < 256; i++)
            {
                suma += histogram[i];
                dystrybuanta[i] = suma / k;
            }

            for (int i = 0; i < 256; i++)  //pierwsza niezer wartosc dystrybuanty
            {
                if (dystrybuanta[i] != 0)
                {
                    minVal = dystrybuanta[i];
                    break;
                }
            }

            for (int i = 0; i <= 255; i++)
            {
                LUT[i] = (byte)(((dystrybuanta[i] - minVal) / (1 - minVal)) * 255.0);
            }

            Bitmap copy;
            using (MemoryStream ms = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(loadedImage));
                enc.Save(ms);
                Bitmap loaded = new Bitmap(ms);
                copy = new Bitmap(loaded);
            }

            var data = copy.LockBits(
                new System.Drawing.Rectangle(0, 0, copy.Width, copy.Height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb
            );
            var copyData = new byte[data.Stride * data.Height];

            Marshal.Copy(data.Scan0, copyData, 0, copyData.Length);

            for (int i = 0; i < copyData.Length; i++)
            {
                copyData[i] = LUT[copyData[i]];
            }

            Marshal.Copy(copyData, 0, data.Scan0, copyData.Length);
            copy.UnlockBits(data);
            BitmapImage im = BitmapToImage(copy);
            Image.Source = im;
            /*
            var bmp = (WriteableBitmap)Image.Source;
            int wid = bmp.PixelWidth, hei = bmp.PixelHeight;
            bmp.Lock();
            unsafe
            {
                int[] rCnt = new int[256], gCnt = new int[256], bCnt = new int[256];
                int* p = (int*)bmp.BackBuffer;
                // wyznaczamy liczbę wystąpień każdej z 256 wartości koloru R, G, B
                for (int y = 0; y < hei; ++y)
                    for (int x = 0; x < wid; ++x)
                    {
                        int argb = *p;
                        ++rCnt[(argb >> 16) & 255];
                        ++gCnt[(argb >> 8) & 255];
                        ++bCnt[argb & 255];
                        ++p;
                    }
                // wyznaczamy dystrybuanty wartości kolorów R, G, B (skumulowane liczby wystąpień)
                for (int v = 1; v <= 255; ++v)
                {
                    rCnt[v] = rCnt[v - 1] + rCnt[v];
                    gCnt[v] = gCnt[v - 1] + gCnt[v];
                    bCnt[v] = bCnt[v - 1] + bCnt[v];
                }
                int[] rDist = rCnt, gDist = gCnt, bDist = bCnt;
                // wyznaczamy minimalne wartości dystrybuant R, G, B
                int rDistMin = 0, gDistMin = 0, bDistMin = 0;
                // nie trzeba sprawdzać wszystkich wartości dystrybuant, bo zawsze minimum jest na początku kumulacji (pominąwszy zera)
                for (int v = 0; v <= 255; ++v)
                    if (rDist[v] > 0) { rDistMin = rDist[v]; break; }
                for (int v = 0; v <= 255; ++v)
                    if (gDist[v] > 0) { gDistMin = gDist[v]; break; }
                for (int v = 0; v <= 255; ++v)
                    if (bDist[v] > 0) { bDistMin = bDist[v]; break; }
                const string err = "Wszystkie piksele mają wartość 0 koloru ";
                if (rDistMin == 0) { MessageBox.Show(err + "czerwonego"); return; }
                if (gDistMin == 0) { MessageBox.Show(err + "zielonego"); return; }
                if (bDistMin == 0) { MessageBox.Show(err + "niebieskiego"); return; }
                int widHei = wid * hei;
                int rDiv = widHei - rDistMin, gDiv = widHei - gDistMin, bDiv = widHei - bDistMin;
                double rMult = 255.0 / rDiv, gMult = 255.0 / gDiv, bMult = 255.0 / bDiv; // L - 1 = 255
                for (int v = 0; v <= 255; ++v)
                {
                    rDist[v] = (int)Math.Round((rDist[v] - rDistMin) * rMult); // & 255
                    gDist[v] = (int)Math.Round((gDist[v] - gDistMin) * gMult); // & 255
                    bDist[v] = (int)Math.Round((bDist[v] - bDistMin) * bMult); // & 255
                }
                // mapujemy oryginalne wartości kolorów pikseli na nowe odpowiadające im wartości
                p = (int*)bmp.BackBuffer;
                for (int y = 0; y < hei; ++y)
                    for (int x = 0; x < wid; ++x)
                    {
                        int argb = *p;
                        *p = (255 << 24) |
                            (rDist[(argb >> 16) & 255] << 16) |
                            (gDist[(argb >> 8) & 255] << 8) |
                            bDist[argb & 255];
                        ++p;
                    }
            }
            bmp.AddDirtyRect(new Int32Rect(0, 0, wid, hei));
            bmp.Unlock();*/
        }

        public Bitmap SkalaSzarosci(Bitmap bitmap)
        {
            Bitmap source = bitmap;
            for (var i = 0; i < source.Width; i++)
            {
                for (var j = 0; j < source.Height; j++)
                {
                    System.Drawing.Color pixel = source.GetPixel(i, j);
                    int r, g, b;

                    r = pixel.R;
                    g = pixel.G;
                    b = pixel.B;
                    r = (int)(0.21 * r + 0.71 * g + 0.07 * b);

                    source.SetPixel(i, j, System.Drawing.Color.FromArgb(r, r, r));
                }
            }

            return source;
        }

        private void Bin_Checked(object sender, RoutedEventArgs e)
        {
            ThresholdLabel.Visibility = Visibility.Collapsed;
            ThresholdTextBox.Visibility = Visibility.Collapsed;
            if (sender == ManualRadioButton)
            {
                ThresholdLabel.Visibility = Visibility.Visible;
                ThresholdLabel.Content = "Podaj próg:";
                ThresholdTextBox.Visibility = Visibility.Visible;
                return;
            }
            if (sender == PercentBlackSelectionRadioButton)
            {
                ThresholdLabel.Visibility = Visibility.Visible;
                ThresholdLabel.Content = "Procent czarnych pikseli:";
                ThresholdTextBox.Visibility = Visibility.Visible;
                return;
            }
        }

        private void PerformNormalization_Click(object sender, RoutedEventArgs e)
        {
            if (lastLoadedPath != null) LoadFile(lastLoadedPath);

            var bmp = Image.Source as WriteableBitmap;
            if (bmp == null) return;

            if (StretchingRadioButton.IsChecked == true)
                PerformStretching();
            else if (EqualizationRadioButton.IsChecked == true)
                PerformEqualization();
        }

        private void PerformStretching()
        {
            var bmp = (WriteableBitmap)Image.Source;
            int wid = bmp.PixelWidth, hei = bmp.PixelHeight;
            bmp.Lock();
            unsafe
            {
                int* p = (int*)bmp.BackBuffer;
                byte rMin = (byte)(*p >> 16), gMin = (byte)(*p >> 8), bMin = (byte)(*p);
                byte rMax = rMin, gMax = gMin, bMax = bMin;

                for (int y = 0; y < hei; ++y)
                    for (int x = 0; x < wid; ++x)
                    {
                        byte r = (byte)(*p >> 16), g = (byte)(*p >> 8), b = (byte)(*p);
                        if (r < rMin) rMin = r;
                        else if (r > rMax) rMax = r;
                        if (g < gMin) gMin = g;
                        else if (g > gMax) gMax = g;
                        if (b < bMin) bMin = b;
                        else if (b > bMax) bMax = b;
                        ++p;
                    }
                p = (int*)bmp.BackBuffer;
                int rDiv = rMax - rMin, gDiv = gMax - gMin, bDiv = bMax - bMin;
                const string err = " ma we wszystkich pikselach taką samą wartość.";
                if (rDiv == 0) { MessageBox.Show("Kolor czerwony" + err); return; }
                if (gDiv == 0) { MessageBox.Show("Kolor zielony" + err); return; }
                if (bDiv == 0) { MessageBox.Show("Kolor niebieski" + err); return; }
                double rMult = 255.0 / rDiv, gMult = 255.0 / gDiv, bMult = 255.0 / bDiv;
                for (int y = 0; y < hei; ++y)
                    for (int x = 0; x < wid; ++x)
                    {
                        double r = (byte)(*p >> 16), g = (byte)(*p >> 8), b = (byte)(*p);
                        r = (r - rMin) * rMult;
                        g = (g - gMin) * gMult;
                        b = (b - bMin) * bMult;
                        *p = (255 << 24) | ((int)r << 16) | ((int)g << 8) | (int)b;
                        ++p;
                    }
            }

            bmp.AddDirtyRect(new Int32Rect(0, 0, wid, hei));
            bmp.Unlock();
        }

        private void PerformBinarization_Click(object sender, RoutedEventArgs e)
        {
            if (lastLoadedPath != null) LoadFile(lastLoadedPath);

            var bmp = Image.Source as WriteableBitmap;
            if (bmp == null) return;
            if (MeanIterativeSelectionRadioButton.IsChecked == true)
                PerformMeanIterativeSelection();
            else if (EntropySelectionRadioButton.IsChecked == true)
                PerformEntropySelection();
            else if (MinimumErrorRadioButton.IsChecked == true)
                PerformMinimumError();
            else if (FuzzyMinimumErrorRadioButton.IsChecked == true)
                PerformFuzzyMinimumError();
            else
            {
                if (ManualRadioButton.IsChecked == true)
                {
                    if (!int.TryParse(ThresholdTextBox.Text, out int thr))
                    { MessageBox.Show("Podaj próg będący liczbą całkowitą."); return; }
                    PerformManual(thr);
                }
                else if (PercentBlackSelectionRadioButton.IsChecked == true)
                {
                    if (!double.TryParse(ThresholdTextBox.Text, out double per) || per < 0 || per > 1)
                    {
                        MessageBox.Show("Podaj procent czarnych pikseli z przedziału (0,1).");
                        return;
                    }
                    PerformPercentBlackSelection(per);
                }
            }
        }

        private void PerformManual(int threshold)
        {
            var bmp = (WriteableBitmap)Image.Source;
            var grayBuf = ToGrayscaleBuffer(bmp);
            Binarize(grayBuf, threshold);
        }

        private void PerformPercentBlackSelection(double percent)
        {
            var bmp = (WriteableBitmap)Image.Source;
            int wid = bmp.PixelWidth, hei = bmp.PixelHeight;
            var grayBuf = ToGrayscaleBuffer(bmp);
            var ch = ComputeHistogram(grayBuf, wid, hei);
            int width_height = wid * hei;
            int i;
            int accum = 0;

            for (i = 0; i <= 255; ++i)
            {
                accum += ch[i];
                if ((double)accum >= width_height * percent) break;
            }

            Binarize(grayBuf, i);
        }

        private void PerformMeanIterativeSelection()
        {
            var bmp = (WriteableBitmap)Image.Source;
            int wid = bmp.PixelWidth, hei = bmp.PixelHeight;
            var grayBuf = ToGrayscaleBuffer(bmp);
            var h = ComputeHistogram(grayBuf, wid, hei);

            const int max_grey = 255;
            byte Tk = 0, TkPrev;
            int[] accumH = new int[h.Length];
            accumH[0] = h[0];

            for (int v = 1; v <= 255; ++v)
                accumH[v] = accumH[v - 1] + h[v];
            do
            {
                TkPrev = Tk;
                int TbNum = 0;
                for (int i = 0; i <= TkPrev; ++i)
                    TbNum += i * h[i];
                int TwNum = 0;
                for (int j = TkPrev + 1; j <= max_grey; ++j)
                    TwNum += j * h[j];
                int TbDen = accumH[TkPrev];
                int TwDen = accumH[max_grey] - accumH[TkPrev];
                double Tb = TbNum / (double)TbDen, Tw = TwNum / (double)TwDen;
                Tk = (byte)((Tb + Tw) / 2.0);
            } while (Tk != TkPrev);

            Binarize(grayBuf, Tk);
        }

        private void PerformEntropySelection()
        {
            var bmp = (WriteableBitmap)Image.Source;
            int wid = bmp.PixelWidth, hei = bmp.PixelHeight;
            var grayBuf = ToGrayscaleBuffer(bmp);
            var h = ComputeHistogram(grayBuf, wid, hei);
            double[] fhistogram = new double[h.Length];
            int widHei = wid * hei;

            for (int i = 0; i < fhistogram.Length; ++i)
                fhistogram[i] = h[i] / (double)widHei;

            int threshold = 127;
            double maxsum = double.MinValue, f = 0, Pt = 0;
            double maxlow = fhistogram[0], maxhigh = 0, Ht = 0, HT = 0;

            for (int i = 0; i < 256; i++)
                HT -= fhistogram[i] * Log2(fhistogram[i]);
            for (int i = 0; i < 256; i++)
            {
                Pt += fhistogram[i];

                if (fhistogram[i] > maxlow)
                    maxlow = fhistogram[i];
                maxhigh = i < 255 ? fhistogram[i + 1] : fhistogram[i];

                for (int j = i + 2; j < 256; j++)
                    if (fhistogram[j] > maxhigh)
                        maxhigh = fhistogram[j];

                Ht -= (fhistogram[i] * Log2(fhistogram[i]));
                f = Ht * Log2(Pt) / (HT * Log2(maxlow)) + (1 - Ht / HT) * Log2(1 - Pt) / Log2(maxhigh);

                if (f > maxsum)
                {
                    maxsum = f;
                    threshold = i;
                }
            }

            Binarize(grayBuf, threshold);
        }

        private void PerformMinimumError()
        {
            var bmp = (WriteableBitmap)Image.Source;
            int wid = bmp.PixelWidth, hei = bmp.PixelHeight;
            var grayBuf = ToGrayscaleBuffer(bmp);
            var histogram = ComputeHistogram(grayBuf, wid, hei);

            int threshold = 127;
            double minvalue = double.MaxValue;
            double J, P1, P2, s1, s2, fv, u1, u2, Pi1, Pi2;
            P1 = P2 = Pi1 = Pi2 = 0;
            int v;

            for (int i = 0; i < 256; i++)
            {
                v = histogram[i];
                P2 += v;
                v *= i;
                Pi2 += v;
            }
            for (int i = 0; i < 256; i++)
            {
                v = histogram[i];
                P1 += v;
                P2 -= v;
                v *= i;
                Pi1 += v;
                Pi2 -= v;
                u1 = P1 > 0 ? Pi1 / P1 : 0;
                u2 = P2 > 0 ? Pi2 / P2 : 0;
                s1 = 0;
                if (P1 > 0)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        fv = j - u1;
                        s1 += fv * fv * histogram[j];
                    }
                    s1 /= P1;
                }
                s2 = 0;
                if (P2 > 0)
                {
                    for (int j = i + 1; j < 256; j++)
                    {
                        fv = j - u2;
                        s2 += fv * fv * histogram[j];
                    }
                    s2 /= P2;
                }
                J = 1 + 2 * P1 * (Log(s1) - Log(P1) + P2 * (Log(s2) - Log(P2)));
                if (J < minvalue)
                {
                    threshold = i;
                    minvalue = J;
                }
            }
            Binarize(grayBuf, threshold);
        }

        private void PerformFuzzyMinimumError()
        {
            var bmp = (WriteableBitmap)Image.Source;
            int wid = bmp.PixelWidth, hei = bmp.PixelHeight;
            var grayBuf = ToGrayscaleBuffer(bmp);
            var histogram = ComputeHistogram(grayBuf, wid, hei);

            double[] fhistogram = new double[histogram.Length];
            int widHei = wid * hei;

            for (int i = 0; i < fhistogram.Length; ++i)
                fhistogram[i] = histogram[i] / (double)widHei;

            int threshold = 127;
            double mu0, mu1, e, mine = double.MaxValue;
            double max = 0, min = 255;

            for (int i = 0; i < 256; i++)
            {
                if (histogram[i] > 0)
                {
                    if (i > max)
                        max = i;
                    if (i < min)
                        min = i;
                }
            }

            double C = max - min;

            for (int t = 0; t < 255; t++)
            {
                mu0 = 0;
                double c = 0;
                for (int i = 0; i <= t; i++)
                {
                    mu0 += i * fhistogram[i];
                    c += fhistogram[i];
                }
                mu0 /= c;
                mu1 = 0;
                c = 0;
                for (int i = t + 1; i < 256; i++)
                {
                    mu1 += i * fhistogram[i];
                    c += fhistogram[i];
                }
                mu1 /= c;
                e = 0;
                for (int i = 0; i <= t; i++)
                    e += Shannon(C / (C + Math.Abs(i - mu0))) * histogram[i];
                for (int i = t + 1; i < 256; i++)
                    e += Shannon(C / (C + Math.Abs(i - mu1))) * histogram[i];
                e /= widHei;
                if (e < mine)
                {
                    threshold = t;
                    mine = e;
                }
            }
            Binarize(grayBuf, threshold);
        }

        private byte[] ToGrayscaleBuffer(WriteableBitmap bmp)
        {
            int wid = bmp.PixelWidth, hei = bmp.PixelHeight;
            byte[] buf = new byte[wid * hei];
            int i = 0;
            unsafe
            {
                int* p = (int*)bmp.BackBuffer;
                for (int y = 0; y < hei; ++y)
                    for (int x = 0; x < wid; ++x)
                    {
                        int argb = *p;
                        byte r = (byte)((argb >> 16) & 255),
                             g = (byte)((argb >> 8) & 255),
                             b = (byte)(argb & 255);
                        byte max;
                        max = r > g ? r : g;
                        if (b > max) max = b;
                        buf[i++] = max;
                        ++p;
                    }
            }
            return buf;
        }

        private void Binarize(byte[] grayscaleBuffer, int threshold)
        {
            var bmp = (WriteableBitmap)Image.Source;
            int wid = bmp.PixelWidth, hei = bmp.PixelHeight;
            int i = 0;
            bmp.Lock();
            unsafe
            {
                int* p = (int*)bmp.BackBuffer;
                for (int y = 0; y < hei; ++y)
                    for (int x = 0; x < wid; ++x)
                    {
                        if (grayscaleBuffer[i] <= threshold) *p = BLACK;
                        else *p = WHITE;
                        ++i;
                        ++p;
                    }
            }
            bmp.AddDirtyRect(new Int32Rect(0, 0, wid, hei));
            bmp.Unlock();
        }

        private int[] ComputeHistogram(byte[] grayscaleBuffer, int width, int height)
        {
            int[] hist = new int[256];
            int i = 0;

            for (int y = 0; y < height; ++y)
                for (int x = 0; x < width; ++x)
                    ++hist[grayscaleBuffer[i++]];
            return hist;
        }

        private double Log2(double d)
        {
            if (d == 0) return double.MinValue;
            return Math.Log(d, 2);
        }

        private double Log(double f)
        {
            if (f <= 0) return 0;
            return Math.Log(f);
        }

        private double Shannon(double x)
        {
            return -x * Log(x) - (1 - x) * Log(1 - x);
        }

        private void Button_Bezier(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Page3());
        }
    }
}
