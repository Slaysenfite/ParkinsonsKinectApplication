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

        public static string generateUniqueFilename(String userID)
        {
            return string.Format(userID + "-{0}.txt", DateTime.Now.Ticks);
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
