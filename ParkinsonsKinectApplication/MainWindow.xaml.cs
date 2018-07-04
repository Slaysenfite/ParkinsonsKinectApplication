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
        private byte[] pixelDataColor { get; set; }
        byte[] depth32;
        short[] pixelDataDepth;
        DepthImageFrame frame;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(this.MainWindow_Loaded);
            Unloaded += new RoutedEventHandler(MainWindow_Unloaded);
        }

        protected void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                kinectHandler = new KinectHandler();
                this.kinectHandler.getSensor().Start();

                this.kinectHandler.getSensor().ColorStream.Disable();
                if (!this.kinectHandler.getSensor().ColorStream.IsEnabled)
                {
                    this.kinectHandler.getSensor().ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                    this.kinectHandler.getSensor().ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(sensor_ColorFrameReady);
                }
                this.kinectHandler.getSensor().DepthStream.Disable();
                if (!this.kinectHandler.getSensor().DepthStream.IsEnabled)
                {
                    this.kinectHandler.getSensor().DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    kinectHandler.getSensor().DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(sensor_DepthFrameReady);
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("No Kinect device connected", "Device Connection Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthimageFrame = e.OpenDepthImageFrame())
            {
                if (depthimageFrame == null)
                {
                    return;
                }
                short[] pixelData = new short[depthimageFrame.PixelDataLength];
                int stride = depthimageFrame.Width * 2;
                depthimageFrame.CopyPixelDataTo(pixelData);
                depthImageControl.Source = BitmapSource.Create(depthimageFrame.
                Width, depthimageFrame.Height, 96, 96, PixelFormats.Gray16, null,
                pixelData, stride);
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
                    this.pixelDataColor = new byte[imageFrame.PixelDataLength];
                    // Copy the pixel data
                    imageFrame.CopyPixelDataTo(this.pixelDataColor);
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
                    pixelDataColor,
                    stride);
                }
            }
        }

        private void btnStopKinect_Click(object sender, RoutedEventArgs e)
        {
            this.kinectHandler.stopKinect();
        }
    }
}
