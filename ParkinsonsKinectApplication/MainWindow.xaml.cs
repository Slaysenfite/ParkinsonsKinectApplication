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
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Threading;

namespace ParkinsonsKinectApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectHandler kinectHandler;
        private byte[] pixelDataColor { get; set; }
        public bool IsRecordingStarted { get; set; }
        private ObservableCollection<SkeletonInfo> skeletonCollection = new ObservableCollection<SkeletonInfo>();
        private Skeleton skeleton;
        private Boolean isCapturingJointData = false;
        private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();




        public MainWindow()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(this.MainWindow_Loaded);
            Unloaded += new RoutedEventHandler(MainWindow_Unloaded);
            btnCaptureStop.IsEnabled = false;
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
                    this.kinectHandler.getSensor().DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(sensor_DepthFrameReady);
                }
                this.kinectHandler.getSensor().SkeletonStream.Disable();
                if (!this.kinectHandler.getSensor().SkeletonStream.IsEnabled)
                {
                    this.kinectHandler.getSensor().SkeletonStream.Enable();
                    this.kinectHandler.getSensor().SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);
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

        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            progressBar1.Value = 0;
            skeletonCanvas.Children.Clear();
            Brush brush = new SolidColorBrush(Colors.Red);
            Skeleton[] skeletons = null;
            SkeletonFrame frame;
            using (frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    skeletons = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletons);
                }
            }

            if (skeletons == null) return;

            skeleton = (from trackSkeleton in skeletons
                        where trackSkeleton.TrackingState == SkeletonTrackingState.Tracked
                        select trackSkeleton).FirstOrDefault();

            if (skeleton == null)
                return;

            if (this.IsRecordingStarted && skeletonCollection.Count <= 1000)
            {
                skeletonCollection.Add(new SkeletonInfo { FrameID = frame.FrameNumber, Skeleton = skeleton });
            }

            int trackedSkeleton = skeleton.Joints.Where(item => item.TrackingState == JointTrackingState.Tracked).Count();
            progressBar1.Value = trackedSkeleton;

            if(isCapturingJointData) captureJointData(skeleton);
            DrawDefaultSkeleton();
        }

        private void captureJointData(Skeleton skeleton)
        {
            //string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"SkeletonJointFiles\test.txt");
            String jointData = "";
            //StreamWriter sw = new StreamWriter(path);
            if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
            {
                foreach (Joint joint in skeleton.Joints)
                {
                    jointData += joint.JointType.ToString() + ":" + joint.Position.X + "," + joint.Position.Y + "," + joint.Position.Z;
                    Console.Write(jointData);
                }
                try
                {
                    WriteToFileThreadSafe(jointData, "C:\\development\\ParkinsonsKinectApplication\\ParkinsonsKinectApplication\\SkeletonJointFiles\\test.txt");
                }
                catch(System.IO.IOException e)
                {
                    Console.Write("Shit has occured: " + e);
                }
            }
        }

        public void WriteToFileThreadSafe(string text, string path)
        {
            // Set Status to Locked
            _readWriteLock.EnterWriteLock();
            try
            {
                //create file if it does not exist
                if (!File.Exists(path))
                {
                    FileStream fs = File.Create(path);
                }
                // Append text to the file
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(text);
                    sw.Close();
                }
            }
            finally
            {
                // Release lock
                _readWriteLock.ExitWriteLock();
            }
        }

            private void DrawDefaultSkeleton()
        {
            DrawSpine();
            DrawLeftArm();
            DrawRightArm();
            DrawLeftLeg();
            DrawRightLeg();
        }

        private void DrawHeadShoulder()
        {
            drawBone(skeleton.Joints[JointType.Head], skeleton.Joints[JointType.ShoulderCenter]);
        }
        private void DrawSpine()
        {
            drawBone(skeleton.Joints[JointType.Head], skeleton.Joints[JointType.ShoulderCenter]);
            drawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.Spine]);
        }
        private void DrawLeftArm()
        {
            drawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderLeft]);
            drawBone(skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.ElbowLeft]);
            drawBone(skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.WristLeft]);
            drawBone(skeleton.Joints[JointType.WristLeft], skeleton.Joints[JointType.HandLeft]);

        }
        private void DrawRightArm()
        {
            drawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderRight]);
            drawBone(skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.ElbowRight]);
            drawBone(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.WristRight]);
            drawBone(skeleton.Joints[JointType.WristRight], skeleton.Joints[JointType.HandRight]);
        }
        private void DrawLeftLeg()
        {
            drawBone(skeleton.Joints[JointType.Spine], skeleton.Joints[JointType.HipCenter]);
            drawBone(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipLeft]);
            drawBone(skeleton.Joints[JointType.HipLeft], skeleton.Joints[JointType.KneeLeft]);
            drawBone(skeleton.Joints[JointType.KneeLeft], skeleton.Joints[JointType.AnkleLeft]);
            drawBone(skeleton.Joints[JointType.AnkleLeft], skeleton.Joints[JointType.FootLeft]);
        }
        private void DrawRightLeg()
        {
            drawBone(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipRight]);
            drawBone(skeleton.Joints[JointType.HipRight], skeleton.Joints[JointType.KneeRight]);
            drawBone(skeleton.Joints[JointType.KneeRight], skeleton.Joints[JointType.AnkleRight]);
            drawBone(skeleton.Joints[JointType.AnkleRight], skeleton.Joints[JointType.FootRight]);
        }
        void drawBone(Joint trackedJoint1, Joint trackedJoint2)
        {
            Line bone = new Line();
            bone.Stroke = Brushes.Red;
            bone.StrokeThickness = 3;
            Point joint1 = this.ScalePosition(trackedJoint1.Position);
            bone.X1 = joint1.X;
            bone.Y1 = joint1.Y;
          
            Point mappedPoint1 = this.ScalePosition(trackedJoint1.Position);
            Rectangle r = new Rectangle(); r.Height = 10; r.Width = 10;
            r.Fill = Brushes.Azure;
            Canvas.SetLeft(r, mappedPoint1.X - 2);
            Canvas.SetTop(r, mappedPoint1.Y - 2);
            skeletonCanvas.Children.Add(r);

            Point joint2 = this.ScalePosition(trackedJoint2.Position);
            bone.X2 = joint2.X;
            bone.Y2 = joint2.Y;

            Point mappedPoint2 = this.ScalePosition(trackedJoint2.Position);

            if (LeafJoint(trackedJoint2))
            {
                Rectangle r1 = new Rectangle(); r1.Height = 10; r1.Width = 10;
                r1.Fill = Brushes.Red;
                Canvas.SetLeft(r1, mappedPoint2.X - 2);
                Canvas.SetTop(r1, mappedPoint2.Y - 2);
                skeletonCanvas.Children.Add(r1);
            }

            if (LeafJoint(trackedJoint2))
            {
                Point mappedPoint = this.ScalePosition(trackedJoint2.Position);
                TextBlock textBlock = new TextBlock();
                textBlock.Foreground = Brushes.Black;
                Canvas.SetLeft(textBlock, mappedPoint.X + 5);
                Canvas.SetTop(textBlock, mappedPoint.Y + 5);
                skeletonCanvas.Children.Add(textBlock);
            }

            skeletonCanvas.Children.Add(bone);
        }

        private bool LeafJoint(Joint j2)
        {
            if (j2.JointType == JointType.HandRight || j2.JointType == JointType.HandLeft || j2.JointType == JointType.FootLeft || j2.JointType == JointType.FootRight)
            {
                return true;
            }
            return false;
        }

        private Point ScalePosition(SkeletonPoint skeletonPoint)
        {
            DepthImagePoint depthPoint = this.kinectHandler.getSensor().CoordinateMapper.MapSkeletonPointToDepthPoint(skeletonPoint, DepthImageFormat.Resolution320x240Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

        private void DrawSkeleton(Skeleton skeleton)
        {

            drawBone(skeleton.Joints[JointType.Head], skeleton.Joints[JointType.ShoulderCenter]);
            drawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.Spine]);

            drawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderLeft]);
            drawBone(skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.ElbowLeft]);
            drawBone(skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.WristLeft]);
            drawBone(skeleton.Joints[JointType.WristLeft], skeleton.Joints[JointType.HandLeft]);

            drawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderRight]);
            drawBone(skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.ElbowRight]);
            drawBone(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.WristRight]);
            drawBone(skeleton.Joints[JointType.WristRight], skeleton.Joints[JointType.HandRight]);

            drawBone(skeleton.Joints[JointType.Spine], skeleton.Joints[JointType.HipCenter]);
            drawBone(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipLeft]);
            drawBone(skeleton.Joints[JointType.HipLeft], skeleton.Joints[JointType.KneeLeft]);
            drawBone(skeleton.Joints[JointType.KneeLeft], skeleton.Joints[JointType.AnkleLeft]);
            drawBone(skeleton.Joints[JointType.AnkleLeft], skeleton.Joints[JointType.FootLeft]);

            drawBone(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipRight]);
            drawBone(skeleton.Joints[JointType.HipRight], skeleton.Joints[JointType.KneeRight]);
            drawBone(skeleton.Joints[JointType.KneeRight], skeleton.Joints[JointType.AnkleRight]);
            drawBone(skeleton.Joints[JointType.AnkleRight], skeleton.Joints[JointType.FootRight]);
        }

        private void btnCaptureStart_Click(object sender, RoutedEventArgs e)
        {
            isCapturingJointData = true;
            btnCaptureStart.IsEnabled = false;
            btnCaptureStop.IsEnabled = true;

        }

        private void btnCaptureStop_Click(object sender, RoutedEventArgs e)
        {
            isCapturingJointData = false;
            btnCaptureStart.IsEnabled = true;
            btnCaptureStop.IsEnabled = false;
        }
    }
}
