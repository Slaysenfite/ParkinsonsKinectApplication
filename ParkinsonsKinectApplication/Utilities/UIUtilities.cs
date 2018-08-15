using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkinsonsKinectApplication.Utilities
{
    class UIUtilities
    {
        public static bool validateTextBox(String text)
        {
            if (text == "")
            {
                return false;
            }
            else return true;
        }

        public static List<String> experimentType()
        {
            List<String> ret = new List<String>();
            ret.Add("Front view gait sequence");
            ret.Add("Back view gait sequence");
            ret.Add("Side view gait sequence");
            return ret;
        }
    }
}
