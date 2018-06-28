using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace ParkinsonsKinectApplication.KinectModule
{
    class KinectHandler
    {
        private readonly KinectSensor sensor;

        public KinectHandler()
        {
            if (deviceConnectionTest())
            {
                sensor = KinectSensor.KinectSensors[0];
                Console.WriteLine(sensor.UniqueKinectId);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("No Kinect device connected", "Device Connection Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
    }
}
