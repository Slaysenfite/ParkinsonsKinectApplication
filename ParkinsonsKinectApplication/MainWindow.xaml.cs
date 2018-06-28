using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Windows;
using ParkinsonsKinectApplication.Algorithms;

namespace ParkinsonsKinectApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KNearestNeighbour knn;
        public MainWindow()
        {
            InitializeComponent();
            knn = new KNearestNeighbour();
        }

        

        private void btnClassify_Click(object sender, RoutedEventArgs e)
        {
            txtApp.Text = knn.kNNMethod();
        }
    }
}
