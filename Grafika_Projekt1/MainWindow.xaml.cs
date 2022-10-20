﻿using Microsoft.Win32;
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
using Brushes = System.Windows.Media.Brushes;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace Grafika_Projekt1
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string SelectedFigure;
        int T;
        double X1, X2, Y1, Y2;
        bool FirstClick = true;
        bool Dragging = false, Resizing = false;
        int width = -1, height = -1, maxValue = -1;
        byte[] pixelBuffer;
        int r, g, b, ratio;

        Uri filePath;
        BitmapImage loadedImage;
        System.Windows.Point? lastCenterPositionOnTarget;
        int CompressionLevel;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Drawing
        private void LineVisibility()
        {
            WspK.Visibility = Visibility.Visible;
            EndX.Visibility = Visibility.Visible;
            EndY.Visibility = Visibility.Visible;
            Boki.Visibility = Visibility.Hidden;
            H.Visibility = Visibility.Hidden;
            W.Visibility = Visibility.Hidden;
            Promień.Visibility = Visibility.Hidden;
            R.Visibility = Visibility.Hidden;
            // ResizeButton.Visibility = Visibility.Hidden;
            //DrawButton.Visibility = Visibility.Visible;
        }

        private void RectangleVisibility()
        {
            WspK.Visibility = Visibility.Hidden;
            EndX.Visibility = Visibility.Hidden;
            EndY.Visibility = Visibility.Hidden;
            Boki.Visibility = Visibility.Visible;
            H.Visibility = Visibility.Visible;
            W.Visibility = Visibility.Visible;
            Promień.Visibility = Visibility.Hidden;
            R.Visibility = Visibility.Hidden;
            //ResizeButton.Visibility = Visibility.Hidden;
            //DrawButton.Visibility = Visibility.Visible;
        }

        private void CircleVisibility()
        {
            WspK.Visibility = Visibility.Hidden;
            EndX.Visibility = Visibility.Hidden;
            EndY.Visibility = Visibility.Hidden;
            Boki.Visibility = Visibility.Hidden;
            H.Visibility = Visibility.Hidden;
            W.Visibility = Visibility.Hidden;
            Promień.Visibility = Visibility.Visible;
            R.Visibility = Visibility.Visible;
            //    ResizeButton.Visibility = Visibility.Hidden;
            //  DrawButton.Visibility = Visibility.Visible;
        }

        private void Linia_Click(object sender, RoutedEventArgs e)
        {
            SelectedFigure = "Line";
            Linia.Background = Brushes.Chocolate;
            Prostokąt.Background = Brushes.LightGray;
            Okręg.Background = Brushes.LightGray;
            LineVisibility();
        }

        private void Rectangle_Click(object sender, RoutedEventArgs e)
        {
            SelectedFigure = "Rectangle";
            Prostokąt.Background = Brushes.Chocolate;
            Linia.Background = Brushes.LightGray;
            Okręg.Background = Brushes.LightGray;
            RectangleVisibility();
        }

        private void Circle_Click(object sender, RoutedEventArgs e)
        {
            SelectedFigure = "Circle";
            Okręg.Background = Brushes.Chocolate;
            Prostokąt.Background = Brushes.LightGray;
            Linia.Background = Brushes.LightGray;
            CircleVisibility();
        }

        private void Draw_Click(object sender, RoutedEventArgs e)
        {
            X1 = Double.Parse(StartX.Text);
            Y1 = Double.Parse(StartY.Text);
            T = int.Parse(Grubość.Text);
            switch (SelectedFigure)
            {
                case "Line":
                    X2 = Double.Parse(EndX.Text);
                    Y2 = Double.Parse(EndY.Text);
                    DrawLine();
                    break;
                case "Rectangle":
                    DrawRectangle();
                    break;
                case "Circle":
                    DrawCircle();
                    break;
            }
        }

        private void MouseDown_Event(object sender, MouseButtonEventArgs e)
        {
            if (SelectedFigure == null) return;
            if (Draw.IsChecked == true)
            {
                T = int.Parse(Grubość.Text);
                if (FirstClick && e.ButtonState == MouseButtonState.Pressed)
                {
                    X1 = e.GetPosition(canvas).X;
                    Y1 = e.GetPosition(canvas).Y;
                    FirstClick = false;
                }
                else
                {
                    X2 = e.GetPosition(canvas).X;
                    Y2 = e.GetPosition(canvas).Y;
                    switch (SelectedFigure)
                    {
                        case "Line":
                            DrawLine();
                            break;
                        case "Rectangle":
                            DrawRectangleByCoords();
                            break;
                        case "Circle":
                            DrawCircleByCoords();
                            break;
                    }
                    FirstClick = true;
                }
            }


        }

        FrameworkElement figure;

        private void ChildMouseDown_Event(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                figure = sender as FrameworkElement;
                X1 = e.GetPosition(sender as FrameworkElement).X;
                Y1 = e.GetPosition(sender as FrameworkElement).Y;
                if (Drag.IsChecked == true)
                {
                    Dragging = true;
                }
                else if (Resize.IsChecked == true)
                {
                    Resizing = true;
                    if (figure.GetType().Equals(typeof(Line)))
                    {
                        SelectedFigure = "Line";
                        Line line = (Line)figure;
                        LineVisibility();
                        StartX.Text = "" + line.X1;
                        StartY.Text = "" + line.Y1;
                        EndX.Text = "" + line.X2;
                        EndY.Text = "" + line.Y2;
                        Grubość.Text = "" + line.StrokeThickness;
                    }
                    else if (figure.GetType().Equals(typeof(Rectangle)))
                    {
                        SelectedFigure = "Rectangle";
                        Rectangle rectangle = (Rectangle)figure;
                        RectangleVisibility();
                        H.Text = "" + rectangle.Height;
                        W.Text = "" + rectangle.Width;
                        StartX.Text = "" + Canvas.GetLeft(rectangle);
                        StartY.Text = "" + Canvas.GetTop(rectangle);
                        Grubość.Text = "" + rectangle.StrokeThickness;
                    }
                    else if (figure.GetType().Equals(typeof(Ellipse)))
                    {
                        SelectedFigure = "Circle";
                        Ellipse circle = (Ellipse)figure;
                        CircleVisibility();
                        StartX.Text = "" + Canvas.GetLeft(circle);
                        StartY.Text = "" + Canvas.GetTop(circle);
                        R.Text = "" + circle.Height / 2;
                        Grubość.Text = "" + circle.StrokeThickness;
                    }
                }

            }
        }

        private void MouseMove_Event(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && (Dragging || Resizing))
            {
                X2 = e.GetPosition(figure).X;
                Y2 = e.GetPosition(figure).Y;
                Console.WriteLine(figure.GetType());
                if (figure.GetType().Equals(typeof(Line)))
                {
                    SelectedFigure = "Line";
                }
                else if (figure.GetType().Equals(typeof(Rectangle)))
                {
                    SelectedFigure = "Rectangle";
                }
                else if (figure.GetType().Equals(typeof(Ellipse)))
                {
                    SelectedFigure = "Circle";
                }

                switch (SelectedFigure)
                {
                    case "Line":
                        Line line = (Line)figure;
                        if (Dragging)
                        {
                            line.X1 += X2 - X1;
                            line.X2 += X2 - X1;
                            line.Y1 += Y2 - Y1;
                            line.Y2 += Y2 - Y1;
                        }
                        else if (Resizing)
                        {
                            if (X1 == line.X1)
                            {
                                line.X1 = X2;
                                line.Y1 = Y2;
                            }
                            else if (X1 == line.X2)
                            {
                                line.X2 = X2;
                                line.Y2 = Y2;
                            }
                        }
                        break;
                    case "Rectangle":
                        Rectangle rectangle = (Rectangle)figure;
                        if (Dragging)
                        {
                            Canvas.SetLeft(rectangle, Canvas.GetLeft(rectangle) - (X1 - X2));
                            Canvas.SetTop(rectangle, Canvas.GetTop(rectangle) - (Y1 - Y2));
                        }
                        else if (Resizing)
                        {
                            rectangle.Height += Math.Sqrt(Math.Pow((X1 - X2), 2) + Math.Pow((Y1 - Y2), 2));
                            rectangle.Width += Math.Sqrt(Math.Pow((X1 - X2), 2) + Math.Pow((Y1 - Y2), 2));
                            Canvas.SetLeft(rectangle, Canvas.GetLeft(rectangle) - (X1 - X2));
                            Canvas.SetTop(rectangle, Canvas.GetTop(rectangle) - (Y1 - Y2));
                        }
                        break;
                    case "Circle":
                        Ellipse circle = (Ellipse)figure;
                        if (Dragging)
                        {
                            Canvas.SetLeft(circle, Canvas.GetLeft(circle) - (X1 - X2));
                            Canvas.SetTop(circle, Canvas.GetTop(circle) - (Y1 - Y2));
                        }
                        else if (Resizing)
                        {
                            double r;
                            if (Canvas.GetLeft(circle) < Canvas.GetLeft(circle) - (X1 - X2))
                            {
                                r = circle.Height / 2 - Math.Sqrt(Math.Pow((X1 - X2), 2) + Math.Pow((Y1 - Y2), 2));
                            }
                            else r = circle.Height / 2 + Math.Sqrt(Math.Pow((X1 - X2), 2) + Math.Pow((Y1 - Y2), 2));

                            circle.Height = r * 2;
                            circle.Width = circle.Height;
                            Canvas.SetLeft(circle, Canvas.GetLeft(circle) - (X1 - X2));
                            Canvas.SetTop(circle, Canvas.GetTop(circle) - (Y1 - Y2));
                        }
                        break;
                }

                X1 = X2;
                Y1 = Y2;
            }
        }

        private void MouseUp_Event(object sender, MouseButtonEventArgs e)
        {
            Dragging = false;
        }

        private void DrawLine()
        {
            Line line = new Line();
            line.StrokeThickness = T;
            line.Stroke = System.Windows.Media.Brushes.Black;
            line.X1 = X1;
            line.X2 = X2;
            line.Y1 = Y1;
            line.Y2 = Y2;
            line.MouseLeftButtonDown += ChildMouseDown_Event;
            canvas.Children.Add(line);
        }

        private void Resize_Click(object sender, RoutedEventArgs e)
        {
            X1 = Double.Parse(StartX.Text);
            Y1 = Double.Parse(StartY.Text);

            T = int.Parse(Grubość.Text);
            switch (SelectedFigure)
            {
                case "Line":
                    Line line = (Line)figure;
                    X2 = Double.Parse(EndX.Text);
                    Y2 = Double.Parse(EndY.Text);
                    line.StrokeThickness = T;
                    line.Stroke = System.Windows.Media.Brushes.Black;
                    line.X1 = X1;
                    line.X2 = X2;
                    line.Y1 = Y1;
                    line.Y2 = Y2;
                    break;
                case "Rectangle":
                    Rectangle rectangle = (Rectangle)figure;
                    rectangle.StrokeThickness = T;
                    rectangle.Height = Double.Parse(H.Text);
                    rectangle.Width = Double.Parse(W.Text);
                    Canvas.SetLeft(rectangle, X1);
                    Canvas.SetTop(rectangle, Y1);
                    break;
                case "Circle":
                    Ellipse circle = (Ellipse)figure;
                    circle.StrokeThickness = T;
                    double r = Double.Parse(R.Text);
                    circle.Height = r * 2;
                    circle.Width = circle.Height;
                    Canvas.SetLeft(circle, X1 - r);
                    Canvas.SetTop(circle, Y1 - r);
                    break;
            }
        }

        private void DrawRectangle()
        {
            Rectangle rectangle = new Rectangle();
            rectangle.StrokeThickness = T;
            rectangle.Stroke = System.Windows.Media.Brushes.Black;
            rectangle.Height = Double.Parse(H.Text);
            rectangle.Width = Double.Parse(W.Text);
            Canvas.SetLeft(rectangle, X1);
            Canvas.SetTop(rectangle, Y1);
            rectangle.MouseLeftButtonDown += ChildMouseDown_Event;
            canvas.Children.Add(rectangle);
        }

        private void DrawRectangleByCoords()
        {
            Rectangle rectangle = new Rectangle();
            rectangle.StrokeThickness = T;
            rectangle.Stroke = System.Windows.Media.Brushes.Black;
            rectangle.Height = Math.Max(Y1 - Y2, Y2 - Y1);
            rectangle.Width = Math.Max(X1 - X2, X2 - X1);
            Canvas.SetLeft(rectangle, Math.Min(X1, X2));
            Canvas.SetTop(rectangle, Math.Min(Y1, Y2));
            rectangle.MouseLeftButtonDown += ChildMouseDown_Event;
            canvas.Children.Add(rectangle);
        }

        private void DrawCircle()
        {
            Ellipse circle = new Ellipse();
            circle.StrokeThickness = T;
            circle.Stroke = System.Windows.Media.Brushes.Black;
            double r = Double.Parse(R.Text);
            circle.Height = r * 2;
            circle.Width = circle.Height;
            Canvas.SetLeft(circle, X1 - r);
            Canvas.SetTop(circle, Y1 - r);
            circle.MouseLeftButtonDown += ChildMouseDown_Event;
            canvas.Children.Add(circle);
        }

        private void DrawCircleByCoords()
        {
            Ellipse circle = new Ellipse();
            circle.StrokeThickness = T;
            circle.Stroke = System.Windows.Media.Brushes.Black;
            double r = Math.Sqrt(Math.Pow((X1 - X2), 2) + Math.Pow((Y1 - Y2), 2));
            circle.Height = r * 2;
            circle.Width = circle.Height;
            Canvas.SetLeft(circle, X1 - r);
            Canvas.SetTop(circle, Y1 - r);
            circle.MouseLeftButtonDown += ChildMouseDown_Event;
            canvas.Children.Add(circle);
        }
        #endregion

        #region Plik
        private void Wczytaj_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Wybierz obraz";
            openFileDialog.Filter = "JPEG|*.jpeg|PPM|*.ppm";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = System.IO.Path.GetExtension(openFileDialog.FileName);
                if (filePath == ".ppm")
                {
                    OpenPPM(openFileDialog);
                }
                else if (filePath == ".jpeg")
                {
                    OpenJPEG(openFileDialog);
                }
                else
                {
                    komunikat.Text = "Podano niedopuszczalny format pliku!";
                }
                    
            }

            /* if (openFileDialog.ShowDialog() == true)
             {
                 filePath = new Uri(openFileDialog.FileName);
             }

             loadedImage = new BitmapImage(filePath);
             image.Source = loadedImage;*/
        }

        private void OpenJPEG(OpenFileDialog openFileDialog)
        {
            if (openFileDialog.ShowDialog() == true)
            {
                filePath = new Uri(openFileDialog.FileName);
            }

            loadedImage = new BitmapImage(filePath);
            image.Source = loadedImage;
        }

        private void OpenPPM(OpenFileDialog openFileDialog)
        {
            var file = File.ReadLines(openFileDialog.FileName);
            var format = file.First();

            int i = 1;
            r = -1; g = -1; b = -1;
            width = -1; height = -1; maxValue = -1 ;
            int index;
            List<String> values = new List<string>();
            String line;

            while (maxValue < 0)
            {
                line = file.ElementAt(i);

                if (line.Length <= 0)
                {
                    i++;
                    continue;
                }

                if ((index = line.IndexOf('#')) >= 0)  //usuwanie komentarzy
                {
                    Console.WriteLine(line);
                    line = line.Remove(index);
                }

                line = string.Join(" ", line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries));
                values = string.IsNullOrWhiteSpace(line) ? new List<string>() : line.Split(' ').ToList();

                if (values.Count() > 0 && width < 0)
                {
                    if (int.Parse(values[0]) > 0)
                    {
                        width = int.Parse(values[0]);
                    }
                    values.RemoveAt(0);
                }

                if (values.Count() > 0 && height < 0)
                {
                    if (int.Parse(values[0]) > 0)
                    {
                        height = int.Parse(values[0]);
                    }
                    values.RemoveAt(0);
                }

                if (values.Count() > 0 && maxValue < 0)
                {
                    if (int.Parse(values[0]) > 0)
                    {
                        maxValue = int.Parse(values[0]);
                    }
                    values.RemoveAt(0);
                }

                i++;
            }

            Console.WriteLine(height + "h");
            Console.WriteLine(width + "w");
            Console.WriteLine(maxValue + "m");

            pixelBuffer = new byte[width * 3 * height];
            int byteOffset = 0;
            Bitmap resultBitmap = new Bitmap(width, height);

           

          //  pixelBuffer = new byte[data.Stride * data.Height];

            if (format == "P6")
            {
      
                values = new List<string>();
                byte[] colorValues = new byte[9];

                var fileStream = File.OpenRead(openFileDialog.FileName);
                var streamReader = new StreamReader(fileStream, Encoding.UTF8);

                int chars = 0, lines = 0;
                while (lines < i)
                {
                    if ((char)streamReader.Read() == '\n')
                    {
                        lines++;
                    }
                    chars++;
                }

                fileStream.Seek(chars, SeekOrigin.Begin);

                while (fileStream.Read(colorValues, 0, colorValues.Length) > 0 && byteOffset < pixelBuffer.Length)
                {
                    values = colorValues.Select(j => j.ToString()).ToList();

                    while (values.Count > 0)
                    {
                        if (values.Count > 0 && r < 0)
                        {
                            if (int.Parse(values[0]) >= 0)
                            {
                                r = int.Parse(values[0]);
                            }
                            values.RemoveAt(0);
                        }

                        if (values.Count > 0 && g < 0)
                        {
                            if (int.Parse(values[0]) >= 0)
                            {
                                g = int.Parse(values[0]);
                            }
                            values.RemoveAt(0);
                        }

                        if (values.Count > 0 && b < 0)
                        {
                            if (int.Parse(values[0]) >= 0)
                            {
                                b = int.Parse(values[0]);
                            }
                            values.RemoveAt(0);
                        }

                        if (r >= 0 && g >= 0 && b >= 0)
                        {
                            Scaling();
                            if (byteOffset >= pixelBuffer.Length - 1) break;
                           // pixelBuffer[byteOffset] = (byte)b;
                           // pixelBuffer[byteOffset + 1] = (byte)g;
                           // pixelBuffer[byteOffset + 2] = (byte)r;
                            int x = Math.Min((byteOffset / 3) % width, width - 1);
                            int y = Math.Min(((byteOffset / 3) - (byteOffset / 3) % width) / width, height - 1);

                            resultBitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(r, g, b));
                            //pixelBuffer[byteOffset + 3] = 255;
                            r = -1;
                            g = -1;
                            b = -1;

                            byteOffset = byteOffset + 3;
                        }
                    }
                }

               // Marshal.Copy(pixelBuffer, 0, data.Scan0, pixelBuffer.Length);
                //resultBitmap.UnlockBits(data);
            }

            else if (format == "P3")
            {
                Console.WriteLine(file.Count());

                var fileStream = File.OpenRead(openFileDialog.FileName);
                var streamReader = new StreamReader(fileStream, Encoding.UTF8);
                char[] colors = new char[100];

                int chars = 0, lines = 0;
                while (lines < i)
                {
                    if ((char)streamReader.Read() == '\n')
                    {
                        lines++;
                    }
                    chars++;
                }

                fileStream.Seek(chars, SeekOrigin.Begin);
                List <string> f = file.Skip(i).ToList();
                //int index;
                // int el = 0;
               // file = file.Select(j => j.Remove(index).Where((index = j.IndexOf('#')) >= 0));
                   
                for (int el = 0; el < f.Count(); el++)
                {
                    if ((index = f.ElementAt(el).IndexOf('#')) >= 0)  //usuwanie komentarzy
                    {
                        var l = f.ElementAt(el);
                        var n = f.ElementAt(el).Remove(index);
                        Console.WriteLine(" n:" + f.ElementAt(el).Remove(index));
                        f[el] = f.ElementAt(el).Remove(index);
                    }
                }
                string test = String.Join(" ", f.Select(j => j.ToString()));
                Console.WriteLine("testowy:" + test);


                //while (!streamReader.EndOfStream) {

                  //  streamReader.ReadBlock(colors, 0, colors.Length);
                    //line = file.ElementAt(j);
                   // line = "";
                    //var new_line = new string(colors);
                    line = test;
                    
                    
                    line = string.Join(" ", line.Split(new char[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries));
                    values = string.IsNullOrWhiteSpace(line) ? new List<string>() : line.Split(' ').ToList();

                    Console.WriteLine(line);

                    while (values.Count > 0)
                    {
                        if (values.Count > 0 && r < 0)
                        {
                            if (int.Parse(values[0]) >= 0)
                            {
                                r = int.Parse(values[0]);
                            }
                            values.RemoveAt(0);
                        }

                        if (values.Count > 0 && g < 0)
                        {
                            if (int.Parse(values[0]) >= 0)
                            {
                                g = int.Parse(values[0]);
                            }
                            values.RemoveAt(0);
                        }

                        if (values.Count > 0 && b < 0)
                        {
                            if (int.Parse(values[0]) >= 0)
                            {
                                b = int.Parse(values[0]);
                            }
                            values.RemoveAt(0);
                        }

                        if (r >= 0 && g >= 0 && b >= 0)
                        {
                            Scaling();
                           // pixelBuffer[byteOffset] = (byte)b;
                            //pixelBuffer[byteOffset + 1] = (byte)g;
                            //pixelBuffer[byteOffset + 2] = (byte)r;
                            int x = Math.Min((byteOffset / 3) % width, width - 1);
                            int y = Math.Min(((byteOffset / 3) - (byteOffset / 3) % width) / width, height - 1);

                            resultBitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(r, g, b));
                            //pixelBuffer[byteOffset + 3] = 255;
                            r = -1;
                            g = -1;
                            b = -1;

                            byteOffset = byteOffset + 3;
                        }
                    }
               // }

               
            }

            loadedImage = BitmapToImage(resultBitmap);
            image.Source = loadedImage;
        }

        private void Scaling()
        {
            ratio = Math.Max(((int)maxValue / 255) - 1, 1);
            r /= ratio;
            g /= ratio;
            b /= ratio;
        }

        private void Zapisz()
        {
            if (image.Source == null)
            {
                System.Windows.MessageBox.Show("Aby zapisać, wczytaj obraz.", "Brak obrazu!");

            }
            else if (image.Source != null)
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
                            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)image.Source));
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
        #endregion

        #region Zoom
        private void zoom_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            if (loadedImage != null)
            {

                scaleTransform.ScaleX = e.NewValue;
                scaleTransform.ScaleY = e.NewValue;

                var centerOfViewport = new System.Windows.Point((int)(scrollViewer.ViewportWidth / 2),
                                                 (int)(scrollViewer.ViewportHeight / 2));
                lastCenterPositionOnTarget = scrollViewer.TranslatePoint(centerOfViewport, ImageGrid);
            }
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

        private void MouseDown_Event_Image(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                X1 = e.GetPosition(image).X;
                Y1 = e.GetPosition(image).Y;
                Bitmap bitmap = ImageToBitmap((BitmapSource)image.Source);
                komunikat.Text = bitmap.GetPixel((int)X1, (int)Y1).ToString();
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

        private void Button_Click_PPM_P3_P6(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Title = "Wybierz obraz";
            openFileDialog.Filter = "PPM|*.ppm";
            int byteOffset = 0;


            if (openFileDialog.ShowDialog() == true)
            {
                var line = File.ReadLines(openFileDialog.FileName);
                var p3_p6 = line.First();
                int i = -1;
                int max = 0;

                foreach (var originalLine in line)
                {
                    string new_line = originalLine;

                    i++;

                    if (i == 0) continue;
                    if (new_line.Length <= 0) continue;

                    var whiteSpace = new List<string>();
                    bool ws_bool = false;
                    bool boolean = true;



                    int hashtags;

                    if (boolean)
                    {
                        hashtags = new_line.IndexOf('#');
                        if (hashtags >= 0)
                            new_line.Remove(hashtags, new_line.Length - hashtags);

                        new_line = string.Join(" ", new_line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries));

                        whiteSpace = string.IsNullOrWhiteSpace(new_line) ? new List<string>() : new_line.Split(' ').ToList();

                        if (whiteSpace.Count > 0 && width < 0)
                        {
                            width = int.Parse(whiteSpace[0]);
                            whiteSpace.RemoveAt(0);
                        }

                        if (whiteSpace.Count > 0 && height < 0)
                        {
                            height = int.Parse(whiteSpace[0]);
                            whiteSpace.RemoveAt(0);
                        }

                        if (whiteSpace.Count > 0 && maxValue < 0)
                        {
                            max = int.Parse(whiteSpace[0]);
                            whiteSpace.RemoveAt(0);
                        }

                        if (width != null && height != null && maxValue != null)
                        {
                            double bitmap_width = canvas.ActualWidth;
                            double bitmap_height = canvas.ActualHeight;
                            double x = 96, y = 96;

                            pixelBuffer = new byte[(int)(width * 3 * height)];


                            boolean = false;
                            if (whiteSpace.Count > 0)
                                ws_bool = true;
                            else
                                continue;
                        }
                    }
                    if (!boolean || ws_bool)
                    {
                        if (p3_p6 == "P3")
                        {

                            var bytes = new List<byte>();
                            long counts = 0;

                            hashtags = new_line.IndexOf('#');
                            if (hashtags >= 0)
                                new_line = new_line.Remove(hashtags, new_line.Length - hashtags);

                            new_line = string.Join(" ", new_line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries));

                            if (!ws_bool)
                                whiteSpace = string.IsNullOrWhiteSpace(new_line) ? new List<string>() : new_line.Split(' ').ToList();

                            while (whiteSpace.Count > 0)
                            {
                                if (whiteSpace.Count > 0 && r < 0 )
                                {
                                    r = int.Parse(whiteSpace[0]);
                                    whiteSpace.RemoveAt(0);
                                }

                                if (whiteSpace.Count > 0 && r < 0)
                                {
                                    g = int.Parse(whiteSpace[0]);
                                    whiteSpace.RemoveAt(0);
                                }

                                if (whiteSpace.Count > 0 && r < 0)
                                {
                                    b = int.Parse(whiteSpace[0]);
                                    whiteSpace.RemoveAt(0);
                                }

                                if (r >= 0 && g >= 0 && b >= 0)
                                {
                                    int range = Math.Max(((int)maxValue / 255) - 1, 1);
                                    byte convertedR = (byte)(r / range);
                                    byte convertedG = (byte)(g / range);
                                    byte convertedB = (byte)(b / range);
                                    pixelBuffer[byteOffset] = (byte)convertedB;
                                    pixelBuffer[byteOffset + 1] = (byte)convertedG;
                                    pixelBuffer[byteOffset + 2] = (byte)convertedR;
                                    //pixelBuffer[byteOffset + 3] = 255;

                                    r = -1;
                                    g = -1;
                                    b = -1;
                                    byteOffset = byteOffset + 3;
                                }
                            }
                        }
                        else if (p3_p6 == "P6")
                        {
                            break;
                        }
                    }
                }
            }

            for(int k=0; k < pixelBuffer.Length; k++)
            {
                Console.WriteLine(pixelBuffer[k]);
            }
            Bitmap writableBitmap = new Bitmap(width, height);
            var resultData = writableBitmap.LockBits(new System.Drawing.Rectangle(0, 0,
                                    writableBitmap.Width, writableBitmap.Height),
                                       System.Drawing.Imaging.ImageLockMode.ReadWrite,
                                         System.Drawing.Imaging.PixelFormat.Format24bppRgb);


            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            writableBitmap.UnlockBits(resultData);
            loadedImage = BitmapToImage(writableBitmap);
            image.Source = BitmapToImage(writableBitmap);

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
