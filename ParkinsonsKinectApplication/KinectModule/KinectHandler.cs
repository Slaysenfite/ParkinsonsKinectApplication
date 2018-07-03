using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using KinectStatusNotifier;

namespace ParkinsonsKinectApplication.KinectModule
{
    class KinectHandler
    {
        private KinectSensor sensor;
        private StatusNotifier notifier;


        public KinectHandler()
        {
            if (deviceConnectionTest())
            {
                sensor = KinectSensor.KinectSensors[0];
                sensor = KinectSensor.KinectSensors.FirstOrDefault(sensorItem => sensorItem.Status == KinectStatus.Connected);
                Console.WriteLine(sensor.UniqueKinectId);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("No Kinect device connected", "Device Connection Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            notifier = new StatusNotifier();
        }

        private void Sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void startKinect()
        {
            if (sensor != null && !sensor.IsRunning)
            {
                sensor.Start();
                sensor.ColorStream.Enable();
                sensor.DepthStream.Enable();
                sensor.SkeletonStream.Enable();
            }
        }

        private void stopKinect()
        {
            if (this.sensor != null && this.sensor.IsRunning)
            {
                this.sensor.Stop();
            }
        }

        private bool deviceConnectionTest()
        {
            int deviceCount = KinectSensor.KinectSensors.Count;
            if (deviceCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void Kinects_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case KinectStatus.Connected:
                    // Device Connected;
                    break;
                case KinectStatus.Disconnected:
                    // Device DisConnected;
                    this.stopKinect();
                    break;
            }
        }

        private void StartKinectCam()
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                this.sensor = KinectSensor.KinectSensors.FirstOrDefault
                (sensorItem => sensorItem.Status == KinectStatus.Connected);
                this.startKinect();
                this.sensor.ColorStream.Enable();
                this.sensor.ColorFrameReady += Sensor_ColorFrameReady;
            }
            else
            {
                System.Windows.MessageBox.Show("No device is connected with system!");
            }
        }

        public KinectSensor getSensor()
        {
            return this.sensor;
        }

    }
}
