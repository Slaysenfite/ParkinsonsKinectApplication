using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Linq;
using Microsoft.Kinect;
using System.Windows;
using ParkinsonsKinectApplication.Algorithms;
using ParkinsonsKinectApplication.KinectModule;

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
            KinectHandler kh = new KinectHandler();

        }

        

        private void btnClassify_Click(object sender, RoutedEventArgs e)
        {
            txtApp.Text = knn.kNNMethod();
        }
    }
}
