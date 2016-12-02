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

        public PlotWindow(List<double> YVector)
        {
            this.YVector = YVector;
            InitializeComponent();

            DrawAxes(YVector.Count);
            DrawGraphPoints();      
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

            XAxisEndLabel.Content = (YVectorSize*2).ToString() + " sek.";
            XAxisMiddleLabel.Content = (YVectorSize).ToString() + " sek.";
            YAxisTopLabel.Content = "30 sek.";
        }

        private void DrawGraphPoints()
        {
            var xSeparation = 640/(double) YVector.Count;
            int YVectorIndex = 0;

            for(double i = 30; i < 665; i += xSeparation) // 665 żeby wyeliminować błędy zaokrąglania (zamiast 670 na końcu byłoby 669.9999992 i powodowałoby Index out of bounds)
            {
                Ellipse point = new Ellipse();
                point.Height = 5;
                point.Width = 5;

                point.Fill = Brushes.Red;

                canvas.Children.Add(point);

                Canvas.SetLeft(point, i);
                Canvas.SetTop(point, 30 + (30 - YVector[YVectorIndex++]) * (640 / 30));
            }
        }
    }
}
