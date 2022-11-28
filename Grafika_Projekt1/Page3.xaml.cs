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

namespace Grafika_Projekt1
{
    /// <summary>
    /// Logika interakcji dla klasy Page3.xaml
    /// </summary>
    public partial class Page3 : Page
    {
        Path bezierCurve;
        Geometry curve;
        List<Point> startPoints = new List<Point>();//PointCollection startPoints = new PointCollection();
        List<Point> points = new List<Point>(); // PointCollection points = new PointCollection();
        List<Point> polygonPoints = new List<Point>();
        double pX, pY, p1X, p1Y, p2X, p2Y, p3X, p3Y, p4X, p4Y, t, xf, yf;
        int degree;
        bool Dragging = false, Resizing = false;
        string SelectedFigure;

        public Page3()
        {
            InitializeComponent();
        }

        private void Krzywa_Click(object sender, RoutedEventArgs e)
        {
            startPoints.Clear();
            SelectedFigure = "Curve";
            Krzywa.Background = Brushes.Chocolate;
            Wielokąt.Background = Brushes.LightGray;
            Okręg.Background = Brushes.LightGray;
            StopieńKrzywej.Visibility = Visibility.Visible;
            Kąty.Visibility = Visibility.Hidden;
            //LineVisibility();
        }

        private void Wielokat_Click(object sender, RoutedEventArgs e)
        {
            polygonPoints.Clear();
            SelectedFigure = "Polygon";
            Wielokąt.Background = Brushes.Chocolate;
            Krzywa.Background = Brushes.LightGray;
            Okręg.Background = Brushes.LightGray;
            StopieńKrzywej.Visibility = Visibility.Hidden;
            Kąty.Visibility = Visibility.Visible;
            //RectangleVisibility();
        }

        private void Circle_Click(object sender, RoutedEventArgs e)
        {
            SelectedFigure = "Circle";
            Okręg.Background = Brushes.Chocolate;
            Wielokąt.Background = Brushes.LightGray;
            Krzywa.Background = Brushes.LightGray;
            //CircleVisibility();
        }
        private void MouseDown_Event(object sender, MouseButtonEventArgs e)
        {
            if (Draw.IsChecked == true && SelectedFigure == "Curve")
            {
                pointLabel.Visibility = Visibility.Hidden;
                pointX.Visibility = Visibility.Hidden;
                pointY.Visibility = Visibility.Hidden;
                EditButton.Visibility = Visibility.Hidden;
                pX = e.GetPosition(canvas).X;
                pY = e.GetPosition(canvas).Y;

                Console.WriteLine("Punkt wejsc: " + pX + ", " + pY);
                startPoints.Add(new Point(pX, pY));
                canvas.Children.Clear();
                DrawStartPoints();

                if (startPoints.Count() > 2)
                {
                    points.Clear();
                    CountPoints();

                    var curve = points.ToArray();
                    for (int i = 0; i < curve.Length - 1; i++)
                    {
                        DrawLine(curve[i], curve[i + 1]);
                    }
                }

            }
            else if (Draw.IsChecked == true && SelectedFigure == "Polygon")
            {
                pointLabel.Visibility = Visibility.Hidden;
                pointX.Visibility = Visibility.Hidden;
                pointY.Visibility = Visibility.Hidden;
                EditButton.Visibility = Visibility.Hidden;
                pX = e.GetPosition(canvas).X;
                pY = e.GetPosition(canvas).Y;
                Console.WriteLine("Punkt wejsc: " + pX + ", " + pY);
                polygonPoints.Add(new Point(pX, pY));

                if (polygonPoints.ToArray().Length == Int32.Parse(Stopień.Text))
                {
                    DrawPolygon();
                    polygonPoints.Clear();
                }

                else
                {
                    DrawPoints();
                }

                //canvas.Children.Clear();

            }
            else if (Resize.IsChecked == true)
            {
                xf = e.GetPosition(canvas).X;
                yf = e.GetPosition(canvas).Y;
                pointLabel.Visibility = Visibility.Visible;
                pointX.Visibility = Visibility.Visible;
                pointX.Text = xf.ToString();
                pointY.Visibility = Visibility.Visible;
                pointY.Text =yf.ToString();
                EditButton.Visibility = Visibility.Hidden;
                AddButton.Visibility = Visibility.Hidden;
                ScaleButton.Visibility = Visibility.Visible;
                sLabel.Visibility = Visibility.Visible;
                s.Visibility = Visibility.Visible;


            }
        }

        private void DrawPolygon()
        {
            Polygon polygon = new Polygon();
            polygon.Stroke = System.Windows.Media.Brushes.Black;
            //polygon.Fill
            polygon.StrokeThickness = 2;
            PointCollection points = new PointCollection();
            foreach (Point p in polygonPoints)
            {
                points.Add(p);
                //canvas.Children.Remove();
            }
            polygon.Points = points;
            polygon.MouseLeftButtonDown += ChildMouseDown_Event;
            canvas.Children.Add(polygon);
        }

        private void DrawPoints()
        {
            foreach (Point point in polygonPoints)
            {
                Ellipse p = new Ellipse();
                p.StrokeThickness = 4;
                p.Stroke = System.Windows.Media.Brushes.Black;
                double r = 2;
                p.Height = r * 2;
                p.Width = p.Height;
                Canvas.SetLeft(p, point.X - r);
                Canvas.SetTop(p, point.Y - r);
                canvas.Children.Add(p);
            }
        }


        FrameworkElement figure;

        private void ChildMouseDown_Event(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                figure = sender as FrameworkElement;
                if (figure.GetType().Equals(typeof(Polygon)))
                {
                    pX = e.GetPosition(sender as FrameworkElement).X;
                    pY = e.GetPosition(sender as FrameworkElement).Y;
                    if (Drag.IsChecked == true)
                    {
                        Dragging = true;
                    }
                    else if (Resize.IsChecked == true)
                    {
                        Resizing = true;
                        SelectedFigure = "Polygon";
                        Polygon polygon = (Polygon)figure;
                        //RectangleVisibility();
                        if (polygon.Points.Count() >= 4)
                        {
                            P1X.Text = polygon.Points[0].X.ToString();
                            P1Y.Text = polygon.Points[0].Y.ToString();
                            P2X.Text = polygon.Points[1].X.ToString();
                            P2Y.Text = polygon.Points[1].Y.ToString();
                            P3X.Text = polygon.Points[2].X.ToString();
                            P3Y.Text = polygon.Points[2].Y.ToString();
                            P4X.Text = polygon.Points[3].X.ToString();
                            P4Y.Text = polygon.Points[3].Y.ToString();
                        }
                        else if(polygon.Points.Count() == 3)
                        {
                            P1X.Text = polygon.Points[0].X.ToString();
                            P1Y.Text = polygon.Points[0].Y.ToString();
                            P2X.Text = polygon.Points[1].X.ToString();
                            P2Y.Text = polygon.Points[1].Y.ToString();
                            P3X.Text = polygon.Points[2].X.ToString();
                            P3Y.Text = polygon.Points[2].Y.ToString();
                        }
                        else if (polygon.Points.Count() == 2)
                        {
                            P1X.Text = polygon.Points[0].X.ToString();
                            P1Y.Text = polygon.Points[0].Y.ToString();
                            P2X.Text = polygon.Points[1].X.ToString();
                            P2Y.Text = polygon.Points[1].Y.ToString();
                        }

                    }
                }
                else if (Edit.IsChecked == true)
                {
                    SelectedFigure = "Curve";
                    Dragging = true;
                    pX = e.GetPosition(canvas).X;
                    pY = e.GetPosition(canvas).Y;
                    //Console.WriteLine(pX + "  " + pY);

                    pointLabel.Visibility = Visibility.Visible;
                    pointX.Visibility = Visibility.Visible;
                    pointY.Visibility = Visibility.Visible;
                    EditButton.Visibility = Visibility.Visible;
                    Point[] tmp = startPoints.ToArray();
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        if ((tmp[i].X >= pX - 4 && tmp[i].X <= pX + 4) && (tmp[i].Y >= pY - 4 && tmp[i].Y <= pY + 4))
                        {
                            pointX.Text = tmp[i].X.ToString();
                            pointY.Text = tmp[i].Y.ToString();
                        }
                    }
                }

            }
        }

        private void MouseMove_Event(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && (Dragging || Resizing))
            {
                double pX0 = e.GetPosition(figure).X;
                double pY0 = e.GetPosition(figure).Y;

                if (SelectedFigure == "Curve")
                {
                    pX0 = e.GetPosition(canvas).X;
                    pY0 = e.GetPosition(canvas).Y;

                    Point[] tmp = startPoints.ToArray();
                    startPoints.Clear();
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        if ((tmp[i].X >= pX - 4 && tmp[i].X <= pX + 4) && (tmp[i].Y >= pY - 4 && tmp[i].Y <= pY + 4))
                        {
                            tmp[i].X = pX0;
                            tmp[i].Y = pY0;
                        }
                        startPoints.Add(new Point(tmp[i].X, tmp[i].Y));
                    }


                    pX = pX0;
                    pY = pY0;
                    UpdateCurve();
                }

                else if (SelectedFigure == "Polygon")
                {
                    Polygon polygon = (Polygon)figure;
                    if (Resizing)
                    {
                        var points = polygon.Points.ToArray();
                        for(int i=0; i< points.Length; i++)
                        {
                            //Console.WriteLine("zmieniono");
                            Point p = new Point(points[i].X + (pX0 - pX), points[i].Y + (pY0-pY));
                            //points[i].X = pX0;
                            //points[i].Y = pY0;
                            polygon.Points[i]=p;
                        }
                    }
                }

                pX = pX0;
                pY = pY0;
            }

        }

        private void UpdateCurve()
        {
            canvas.Children.Clear();
            DrawStartPoints();
            points.Clear();
            CountPoints();

            var curve = points.ToArray();
            for (int i = 0; i < curve.Length - 1; i++)
            {
                DrawLine(curve[i], curve[i + 1]);
            }
        }

        private void Scale_Click(object sender, RoutedEventArgs e)
        {
            var scale = Double.Parse(s.Text);

            Polygon polygon = (Polygon)figure;
            var points = polygon.Points.ToArray();
            for (int i = 0; i < points.Length; i++)
            {
                var x_new = xf + (points[i].X - xf) * scale;
                var y_new = yf + (points[i].Y - yf) * scale;
                Point p = new Point(x_new, y_new);
               polygon.Points[i] = p;
                
            }
        }

        private void MouseUp_Event(object sender, MouseButtonEventArgs e)
        {
            Dragging = false;
        }

        private void Draw_Click(object sender, RoutedEventArgs e)
        {
            startPoints.Clear();
            degree = Int32.Parse(Stopień.Text);    //stopien krzywej = liczba punktow kontrolnych -1

            if ((P1X.Text != "") && (P1Y.Text != ""))
            {
                p1X = double.Parse(P1X.Text);
                p1Y = double.Parse(P1Y.Text);
                startPoints.Add(new Point(p1X, p1Y));

                if ((P1X.Text != "") && (P1Y.Text != ""))
                {
                    p2X = double.Parse(P2X.Text);
                    p2Y = double.Parse(P2Y.Text);
                    startPoints.Add(new Point(p2X, p2Y));

                    if ((P3X.Text != "") && (P3Y.Text != ""))
                    {
                        p3X = double.Parse(P3X.Text);
                        p3Y = double.Parse(P3Y.Text);
                        startPoints.Add(new Point(p3X, p3Y));

                        if ((P4X.Text != "") && (P4Y.Text != ""))
                        {
                            p4X = double.Parse(P4X.Text);
                            p4Y = double.Parse(P4Y.Text);
                            startPoints.Add(new Point(p4X, p4Y));
                        }
                    }
                }
            }

            if (SelectedFigure == "Polygon")
            {
                if (degree > startPoints.Count())
                {
                    Random rnd = new Random();
                    for (int i = startPoints.Count(); i < degree; i++)
                    {
                        startPoints.Add(new Point(rnd.Next(0, 900), rnd.Next(0, 600)));
                    }
                }
                polygonPoints.Clear();
                foreach(Point p in startPoints)
                {
                    polygonPoints.Add(p);   
                }
                DrawPolygon();
            }

            else if(SelectedFigure == "Curve")
            {
                if (degree > startPoints.Count())
                {
                    Random rnd = new Random();
                    for (int i = startPoints.Count(); i <= degree; i++)
                    {
                        startPoints.Add(new Point(rnd.Next(0, 900), rnd.Next(0, 600)));
                    }
                }
                UpdateCurve();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Point[] tmp = startPoints.ToArray();

            if ((P1X.Text != "") && (P1Y.Text != ""))
            {
                tmp[0].X = double.Parse(P1X.Text);
                tmp[0].Y = double.Parse(P1Y.Text);
            }

            else if ((P1X.Text != "") && (P1Y.Text != "") && (tmp.Length >= 2))
            {
                tmp[1].X = double.Parse(P2X.Text);
                tmp[1].Y = double.Parse(P2Y.Text);
            }

            else if ((P3X.Text != "") && (P3Y.Text != "") && (tmp.Length >= 3))
            {
                tmp[2].X = double.Parse(P3X.Text);
                tmp[2].Y = double.Parse(P3Y.Text);
            }

            else if ((P4X.Text != "") && (P4Y.Text != "") && (tmp.Length >= 4))
            {
                tmp[3].X = double.Parse(P4X.Text);
                tmp[3].Y = double.Parse(P4Y.Text);
            }

            for (int i = 0; i < tmp.Length; i++)
            {
                startPoints.Add(new Point(tmp[i].X, tmp[i].Y));
            }
            UpdateCurve();

        }

        private void DrawLine(Point a, Point b)
        {
            Line line = new Line();
            line.StrokeThickness = 2;
            line.Stroke = System.Windows.Media.Brushes.Green;
            line.X1 = a.X;
            line.X2 = b.X;
            line.Y1 = a.Y;
            line.Y2 = b.Y;
            canvas.Children.Add(line);
        }

        private void EditPoint_Click(object sender, RoutedEventArgs e)
        {
            Point[] tmp = startPoints.ToArray();
            startPoints.Clear();
            for (int i = 0; i < tmp.Length; i++)
            {
                if ((tmp[i].X >= pX - 4 && tmp[i].X <= pX + 4) && (tmp[i].Y >= pY - 4 && tmp[i].Y <= pY + 4))
                {
                    tmp[i].X = double.Parse(pointX.Text);
                    tmp[i].Y = double.Parse(pointY.Text);
                }
                startPoints.Add(new Point(tmp[i].X, tmp[i].Y));
            }


            pX = double.Parse(pointX.Text);
            pY = double.Parse(pointY.Text);
            UpdateCurve();
        }

        private void AddPoint_Click(object sender, RoutedEventArgs e)
        {
            startPoints.Add(new Point(double.Parse(addX.Text), double.Parse(addY.Text)));
            UpdateCurve();
        }

        private void DrawStartPoints()
        {
            foreach (Point point in startPoints)
            {
                Ellipse p = new Ellipse();
                p.StrokeThickness = 4;
                p.Stroke = System.Windows.Media.Brushes.Yellow;
                double r = 4;
                p.Height = r * 2;
                p.Width = p.Height;
                Canvas.SetLeft(p, point.X - r);
                Canvas.SetTop(p, point.Y - r);
                p.MouseLeftButtonDown += ChildMouseDown_Event;
                canvas.Children.Add(p);
            }

            for (int i = 0; i < startPoints.Count() - 1; i++)
            {
                Line line = new Line();
                line.StrokeThickness = 1;
                line.Stroke = System.Windows.Media.Brushes.Yellow;
                line.X1 = startPoints[i].X;
                line.X2 = startPoints[i + 1].X;
                line.Y1 = startPoints[i].Y;
                line.Y2 = startPoints[i + 1].Y;
                canvas.Children.Add(line);
            }
        }

        private void CountPoints()
        {
            degree = startPoints.Count() - 1;
            for (t = 0.0; t <= 1.0; t += 0.05) //krok =0.05
            {
                double xPoint = 0;
                double yPoint = 0;

                for (int i = 0; i <= (startPoints.Count() - 1); i++)
                {
                    double Newton = countNewton(degree, i);
                    double ti = Math.Pow(t, i);
                    double lti = Math.Pow((1 - t), (degree - i));
                    xPoint += Newton * ti * lti * startPoints[i].X;
                    yPoint += Newton * ti * lti * startPoints[i].Y;
                }
                points.Add(new Point(xPoint, yPoint));
            }

        }

        private double countNewton(int a, int b)
        {
            return silnia(a) / (silnia(b) * silnia(a - b));
        }

        private static int silnia(int n)
        {
            if (n < 1) return 1;
            else return n * silnia(n - 1);
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            points.Clear();
            startPoints.Clear();
        }

        private void Button_Click_Back(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Page1());
        }

        private void Button_New_Page(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Page2());
        }
    }
}
