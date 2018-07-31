using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkinsonsKinectApplication.KinectModule.KinectReplaySystem
{
    public abstract class ReplayFrame
    {
        public int FrameNumber { get; protected set; }
        public long TimeStamp { get; protected set; }
        internal abstract void CreateFromReader(BinaryReader reader);
    }
}
