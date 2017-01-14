﻿using System;
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

namespace AISDE_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Player simulator;
        double SimulationTime = 500;

        public MainWindow()
        {
            InitializeComponent();
            simulator = new Player();
            simulator.LogCreated += UpdateLogWindow;
            simulator.Simulate(SimulationTime);
        }

        private void UpdateLogWindow(object sender, LogEventArgs e)
        {
            LogTextBox.Text += e.Message + '\n';
        }

        private void PlotButton_Click(object sender, RoutedEventArgs e)
        {
            PlotWindow w = new PlotWindow(simulator.YGraphValues, SimulationTime);
            w.Show();
        }
    }
}
