using ParkinsonsKinectApplication.KinectModule;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        //public static String PYTHON_INTERPRETER_LOCATION = System.Windows.Forms.Application.StartupPath + "//python.exe";
        public const string PYTHON_INTERPRETER_LOCATION = @"C:\Users\wesse\Anaconda3\python.exe";
        public static String RELATIVE_PATH = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "..//..//");
        public static String FILE_HEADER = "Time,HipCenter_x,HipCenter_y,HipCenter_z,Spine_x,Spine_y,Spine_z," +
                                            "ShoulderCenter_x,ShoulderCenter_y,ShoulderCenter_z,Head_x,Head_y,Head_z," +
                                            "ShoulderLeft_x,ShoulderLeft_y,ShoulderLeft_z,ElbowLeft_x,ElbowLeft_y,ElbowLeft_z," +
                                            "WristLeft_x,WristLeft_y,WristLeft_z,HandLeft_x,HandLeft_y,HandLeft_z," +
                                            "ShoulderRight_x,ShoulderRight_y,ShoulderRight_z,ElbowRight_x,ElbowRight_y,ElbowRight_z," +
                                            "WristRight_x,WristRight_y,WristRight_z,HandRight_x,HandRight_y,HandRight_z," +
                                            "HipLeft_x,HipLeft_y,HipLeft_z,KneeLeft_x,KneeLeft_y,KneeLeft_z," +
                                            "AnkleLeft_x,AnkleLeft_y,AnkleLeft_z,FootLeft_x,FootLeft_y,FootLeft_z," +
                                            "HipRight_x,HipRight_y,HipRight_z,KneeRight_x,KneeRight_y,KneeRight_z," +
                                            "AnkleRight_x,AnkleRight_y,AnkleRight_z,FootRight_x,FootRight_y,FootRight_z";


        public static string generateUniqueFilename(String userID)
        {
            return string.Format(userID + "-{0}.csv", DateTime.Now.Ticks);
        }

        public List<SkeletonJointData> populateSkeletonJointLists(String filename)
        {

            return null;
        }

        public static String pythonSkeletonEnrollment(String pyScript, String userFile)
        {
            ProcessStartInfo pyProcInfo = new ProcessStartInfo(PYTHON_INTERPRETER_LOCATION);
            pyProcInfo.CreateNoWindow = true;
            pyProcInfo.UseShellExecute = false;
            pyProcInfo.RedirectStandardOutput = true;
            pyProcInfo.Arguments = pyScript;
            Process pyProc = new Process();
            pyProc.StartInfo = pyProcInfo;
            pyProc.Start();

            StreamReader pyProcReader = pyProc.StandardOutput;
            return pyProcReader.ReadLine();
        }
    }
}
