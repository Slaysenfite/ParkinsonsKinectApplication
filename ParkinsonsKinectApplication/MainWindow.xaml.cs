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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Forms;

namespace ParkinsonsKinectApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectHandler kinectHandler;
        private KNearestNeighbour knn;
        private byte[] pixelData { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(this.MainWindow_Loaded);
            Unloaded += new RoutedEventHandler(MainWindow_Unloaded);
        }

        protected void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Check if there kinect connected
            if (KinectSensor.KinectSensors.Count > 0)
            {
                kinectHandler = new KinectHandler();
                // start the kinectHandler.getSensor()
                this.kinectHandler.getSensor().Start();

                this.kinectHandler.getSensor().ColorStream.Disable();
                if (!this.kinectHandler.getSensor().ColorStream.IsEnabled)
                {
                    this.kinectHandler.getSensor().ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                    this.kinectHandler.getSensor().ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(sensor_ColorFrameReady);
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("No Kinect device connected", "Device Connection Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            kinectHandler.getSensor().Stop();
        }

        void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame imageFrame = e.OpenColorImageFrame())
            {
                // Check if the incoming frame is not null
                if (imageFrame == null)
                {
                    return;
                }
                else
                {
                    // Get the pixel data in byte array
                    this.pixelData = new byte[imageFrame.PixelDataLength];
                    // Copy the pixel data
                    imageFrame.CopyPixelDataTo(this.pixelData);
                    // Calculate the stride
                    int stride = imageFrame.Width * imageFrame.BytesPerPixel;
                    // assign the bitmap image source into image control
                    this.VideoControl.Source = BitmapSource.Create(
                    imageFrame.Width,
                    imageFrame.Height,
                    96,
                    96,
                    PixelFormats.Bgr32,
                    null,
                    pixelData,
                    stride);
                }
            }
        }
    }
}
