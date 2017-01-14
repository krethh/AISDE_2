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

namespace AISDE_2
{
    /// <summary>
    /// Interaction logic for PlotWindow.xaml
    /// </summary>
    public partial class PlotWindow : Window
    {
        public List<double> YVector { get; set; }
        /// <summary>
        /// Długość wektora x (w sekundach), potrzebne do skalowania osi.
        /// </summary>
        public double TimeVectorLength { get; set; }

        public PlotWindow(List<double> YVector, double SimulationTime)
        {
            this.YVector = YVector;
            this.TimeVectorLength = SimulationTime;
            InitializeComponent();

            DrawAxes(YVector.Count);
            DrawGraph();      
        }

        /// <summary>
        /// Rysuje osie układu współrzędnych i dobiera podziałkę na podstawie ilości punktów do przedstawienia
        /// </summary>
        /// <param name="YVectorSize"></param>
        private void DrawAxes(double YVectorSize)
        {
            Line yAxis = new Line();
            yAxis.X1 = 30;
            yAxis.Y1 = 670;
            yAxis.X2 = 30;
            yAxis.Y2 = 30;

            yAxis.Stroke = Brushes.Black;
            yAxis.StrokeThickness = 2;

            canvas.Children.Add(yAxis);

            Line xAxis = new Line();
            xAxis.X1 = 30;
            xAxis.Y1 = 670;
            xAxis.X2 = 670;
            xAxis.Y2 = 670;

            xAxis.Stroke = Brushes.Black;
            xAxis.StrokeThickness = 2;

            canvas.Children.Add(xAxis);

            for (int i = 110; i <= 670; i += 80)
            {
                Line segment = new Line();
                segment.X1 = i;
                segment.X2 = i;
                segment.Y1 = 650;
                segment.Y2 = 690;

                segment.Stroke = Brushes.Black;
                segment.StrokeThickness = 1;
                canvas.Children.Add(segment);
            }

            for (int i = 30; i <= 670; i += 80)
            {
                Line segment = new Line();
                segment.X1 = 50;
                segment.X2 = 10;
                segment.Y1 = i;
                segment.Y2 = i;

                segment.Stroke = Brushes.Black;
                segment.StrokeThickness = 1;
                canvas.Children.Add(segment);
            }

            XAxisEndLabel.Content = (TimeVectorLength).ToString() + " sek.";
            XAxisMiddleLabel.Content = (TimeVectorLength / 2).ToString() + " sek.";
            YAxisTopLabel.Content = "30 sek.";
        }

        private void DrawGraph()
        {
            var xSeparation = 640/(double) YVector.Count;
            int YVectorIndex = 0;

            for(double i = 30; i < 665; i += xSeparation) // 665 żeby wyeliminować błędy zaokrąglania (zamiast 670 na końcu byłoby 669.9999992 i powodowałoby Index out of bounds)
            {
                Ellipse point = new Ellipse();
                point.Height = 2;
                point.Width = 2;

                point.Fill = Brushes.Transparent;

                canvas.Children.Add(point);

                Canvas.SetLeft(point, i);
                Canvas.SetTop(point, 30 + (30 - YVector[YVectorIndex++]) * (640 / 30));
            }

            /// rysuje linie wykresu
            int n = canvas.Children.Count - 1;

            for (int i = 0; i < n; i++)
            {
                if (canvas.Children[i] as Ellipse == null)
                    continue;

                var point1 = canvas.Children[i] as Ellipse;
                var point2 = canvas.Children[i + 1] as Ellipse;

                Line line = new Line();
                line.X1 = Canvas.GetLeft(point1);
                line.Y1 = Canvas.GetTop(point1);

                line.X2 = Canvas.GetLeft(point2);
                line.Y2 = Canvas.GetTop(point2);
                line.Stroke = Brushes.Red;
                line.StrokeThickness = 1;
                canvas.Children.Add(line);
            }
        }
    }
}
