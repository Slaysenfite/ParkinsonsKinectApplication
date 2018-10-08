using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkinsonsKinectApplication.Utilities
{
    class Experiment
    {
        private String description;
        private String code;

        public Experiment(String description)
        {
            this.description = description;

            if (description.Equals("Front view gait sequence")) this.code = "frontv";
            else if (description.Equals("Back view gait sequence")) this.code = "backv";
            else if (description.Equals("Side view, left facing gait sequence")) this.code = "leftv";
            else if (description.Equals("Side view, right facing gait sequence")) this.code = "rightv";
            else this.code = "other";
        }

        public String getCode()
        {
            return this.code;
        }
    }
}
