using ParkinsonsKinectApplication.KinectModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkinsonsKinectApplication.MathUtilities
{
    class CovarianceMatrix
    {
        private SkeletonJointData[][] matrix;

        public CovarianceMatrix() {

        }

        public CovarianceMatrix(SkeletonJointData[][] matrix)
        {
            this.matrix = matrix;
        }

    }
}
