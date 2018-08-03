using ParkinsonsKinectApplication.KinectModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkinsonsKinectApplication.Utilities
{
    class FileUtilities
    {
        private const int X_COORDINATE_INDEX = 0;
        private const int Y_COORDINATE_INDEX = 1;
        private const int Z_COORDINATE_INDEX = 2;
        public static string generateUniqueFilename(String userID)
        {
            return string.Format(userID + "-{0}.txt", DateTime.Now.Ticks);
        }

        public List<SkeletonJointData> populateSkeletonJointLists(String filename)
        {

            return null;
        }
    }
}
