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
        private MainWindowViewModel viewModel;
        private KinectHandler kinectHandler;

        public MainWindow()
        {
            InitializeComponent();
            knn = new KNearestNeighbour();
            this.Loaded += this.MainWindow_Loaded;
            this.viewModel = new MainWindowViewModel();
            this.DataContext = this.viewModel;
            kinectHandler = new KinectHandler();
        }

        private void SetKinectInfo()
        {
            if (this.kinectHandler.getSensor() != null)
            {
                this.viewModel.ConnectionID = (this.kinectHandler.getSensor().DeviceConnectionId);
                // Set other property values
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
