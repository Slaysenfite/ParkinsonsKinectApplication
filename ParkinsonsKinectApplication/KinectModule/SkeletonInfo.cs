using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkinsonsKinectApplication.KinectModule
{
    class SkeletonInfo
    {
        public int FrameID { get; set; }
        public Skeleton Skeleton { get; set; }
    }
}
