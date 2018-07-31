using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ParkinsonsKinectApplication.KinectModule.KinectReplaySystem
{
    public class ReplaySkeletonFrame : ReplayFrame
    {
        public Tuple<float, float, float, float> FloorClipPlane { get; private set; }
        public Skeleton[] Skeletons { get; private set; }
        public ReplaySkeletonFrame(SkeletonFrame frame)
        {
            FloorClipPlane = frame.FloorClipPlane;
            FrameNumber = frame.FrameNumber;
            TimeStamp = frame.Timestamp;
            Skeleton[] skeletons = new Skeleton[frame.SkeletonArrayLength];
            frame.CopySkeletonDataTo(skeletons); 
        }
        public ReplaySkeletonFrame()
        {
        }
        internal override void CreateFromReader(BinaryReader reader)
        {
            TimeStamp = reader.ReadInt64();
            FloorClipPlane = new Tuple<float, float, float, float>(
            reader.ReadSingle(), reader.ReadSingle(),
            reader.ReadSingle(), reader.ReadSingle());
            FrameNumber = reader.ReadInt32();
            BinaryFormatter formatter = new BinaryFormatter();
            Skeletons = (Skeleton[])formatter.Deserialize(reader.BaseStream);
        }
        public static implicit operator ReplaySkeletonFrame(SkeletonFrame frame)
        {
            return new ReplaySkeletonFrame(frame);
        }
    }
}
