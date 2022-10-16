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

        public MainWindow()
        {
            InitializeComponent();
        }

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
                            if(X1 == line.X1)
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
                            if(Canvas.GetLeft(circle) < Canvas.GetLeft(circle) - (X1 - X2))
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
            rectangle.Height = Math.Max(Y1 - Y2, Y2 -Y1);
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
            circle.Height = r*2;
            circle.Width = circle.Height;
            Canvas.SetLeft(circle, X1 - r);
            Canvas.SetTop(circle, Y1 - r);
            circle.MouseLeftButtonDown += ChildMouseDown_Event;
            canvas.Children.Add(circle);
        }
    }

}
