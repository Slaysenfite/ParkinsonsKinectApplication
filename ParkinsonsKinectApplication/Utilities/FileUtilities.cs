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
        public const string PYTHON_INTERPRETER_LOCATION = @"C:\Users\user\Anaconda3\python.exe";
        public static String RELATIVE_PATH = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "..//..//");
        public static String FILE_HEADER = "HipCenterXPosition, HipCenterYPosition, HipCenterZPosition, SpineXPosition, SpineYPosition, " +
                                             "SpineZPosition, ShoulderCenterXPosition, ShoulderCenterYPosition, ShoulderCenterZPosition, HeadXPosition, HeadYPosition, " +
                                             "HeadZPosition, ShoulderLeftXPosition,ShoulderLeftYPosition, ShoulderLeftZPosition, ElbowLeftXPosition, ElbowLeftYPosition, " +
                                             "ElbowLeftZPosition, WristLeftXPosition, WristLeftYPosition, WristLeftZPosition, HandLeftXPosition, HandLeftYPosition, " +
                                             "HandLeftZPosition, ShoulderRightXPosition, ShoulderRightYPosition, ShoulderRightZPosition, ElbowRightXPosition, ElbowRightYPosition, " +
                                             "ElbowRightZPosition, WristRightXPosition, WristRightYPosition, WristRightZPosition, HandRightXPosition, HandRightYPosition, " +
                                             "HandRightZPosition, HipLeftXPosition, HipLeftYPosition, HipLeftZPosition, KneeLeftXPosition, KneeLeftYPosition, KneeLeftZPosition, " +
                                             "AnkleLeftXPosition, AnkleLeftYPosition, AnkleLeftZPosition, FootLeftXPosition, FootLeftYPosition, FootLeftZPosition, HipRightXPosition, " +
                                             "HipRightYPosition, HipRightZPosition, KneeRightXPosition, KneeRightYPosition, KneeRightZPosition, AnkleRightXPosition, " +
                                             "AnkleRightYPosition, AnkleRightZPosition, FootRightXPosition, FootRightYPosition, FootRightZPosition, drop_this_col";


        public static string generateUniqueFilename(String userID, String code)
        {
            return string.Format(userID + "-{0}.csv", code + "-" + DateTime.Now.Ticks);
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

        public static int pythonGenTrainingSet(String pyScript)
        {
            ProcessStartInfo pyProcInfo = new ProcessStartInfo(PYTHON_INTERPRETER_LOCATION);
            pyProcInfo.CreateNoWindow = true;
            pyProcInfo.UseShellExecute = false;
            pyProcInfo.RedirectStandardOutput = true;
            pyProcInfo.Arguments = pyScript + " " 
                + RELATIVE_PATH + " "
                + RELATIVE_PATH + "ParkinsonsDetectionDataset//" + " "
                + RELATIVE_PATH + "TrainingData//";
            Process pyProc = new Process();
            pyProc.StartInfo = pyProcInfo;
            pyProc.Start();

            StreamReader pyProcReader = pyProc.StandardOutput;
            String returned = pyProcReader.ReadLine();
            Console.WriteLine(returned);
            if (returned.Contains("DONE"))
                return 0;
            else return -1;
        }

        public static int pythonClassifyUser(String pyScript, String userID)
        {
            ProcessStartInfo pyProcInfo = new ProcessStartInfo(PYTHON_INTERPRETER_LOCATION);
            pyProcInfo.CreateNoWindow = true;
            pyProcInfo.UseShellExecute = false;
            pyProcInfo.RedirectStandardOutput = true;
            pyProcInfo.Arguments = pyScript + " " + userID;
            Process pyProc = new Process();
            pyProc.StartInfo = pyProcInfo;
            pyProc.Start();

            StreamReader pyProcReader = pyProc.StandardOutput;
            String returned = pyProcReader.ReadLine();
            return Convert.ToInt32(returned);
        }
    }
}
