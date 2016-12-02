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
            InitializeComponent();
                       
            var XStep = YVector.Count / 700;

            for(int i = 0; i < YVector.Count; i ++)
            {
                Ellipse point = new Ellipse();
                point.Width = 2;
                point.Height = 2;

                point.Fill = Brushes.Red;

                Canvas.SetLeft(point, i * XStep + 20);
                Canvas.SetTop(point, 700);

                canvas.Children.Add(point);
            }
        }
    }
}
