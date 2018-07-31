using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkinsonsKinectApplication.MathUtilities
{
    class Position_3D
    {
        private double x { get; set; }
        private double y { get; set; }
        private double z { get; set; }

        public Position_3D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public double magnitude()
        {
            return Math.Sqrt((x * x) + (y * y) + (z * z));
        }

        public double dot(Position_3D v)
        {
            return x * v.x + y * v.y + z * v.z;
        }

        public Position_3D cross(Position_3D v)
        {
            return new Position_3D(y * v.z - z * v.y, z * v.x - x * v.z, x * v.y - y * v.x);
        }

        public double distance(Position_3D v)
        {
            return Math.Sqrt(Math.Pow(v.x - x, 2) + Math.Pow(v.y - y, 2) + Math.Pow(v.z - z, 2));
        }

    }
}
