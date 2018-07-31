using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParkinsonsKinectApplication.MathUtilities;

namespace ParkinsonsKinectApplication.KinectModule
{
    class SkeletonJointData
    {
        private Position_3D HipCenter { get; }
        private Position_3D Spine { get; }
        private Position_3D ShoulderCenter { get; }
        private Position_3D Head { get; }
        private Position_3D ShoulderLeft { get; }
        private Position_3D ElbowLeft { get; }
        private Position_3D WristLeft { get; }
        private Position_3D HandLeft { get; }
        private Position_3D ShoulderRight { get; }
        private Position_3D ElbowRight { get; }
        private Position_3D WritstRight { get; }
        private Position_3D HandRight { get; }
        private Position_3D HipLeft { get; }
        private Position_3D KneeLeft { get; }
        private Position_3D AnkleLeft { get; }
        private Position_3D FootLeft { get; }
        private Position_3D HipRight { get; }
        private Position_3D KneeRight { get; }
        private Position_3D AnkleRight { get; }
        private Position_3D FootRight { get; }

        public SkeletonJointData() { }
 
        public SkeletonJointData(Position_3D _1, Position_3D _2, Position_3D _3, Position_3D _4, Position_3D _5,
            Position_3D _6, Position_3D _7, Position_3D _8, Position_3D _9, Position_3D _10,
            Position_3D _11, Position_3D _12, Position_3D _13, Position_3D _14, Position_3D _15,
            Position_3D _16, Position_3D _17, Position_3D _18, Position_3D _19, Position_3D _20)
        {
            this.HipCenter = _1;
            this.Spine = _2;
            this.ShoulderCenter = _3;
            this.Head = _4;
            this.ShoulderLeft = _5;
            this.ElbowLeft = _6;
            this.WristLeft = _7;
            this.HandLeft = _8;
            this.ShoulderRight = _9;
            this.ElbowRight = _10;
            this.WritstRight = _11;
            this.HandRight = _12;
            this.HipLeft = _13;
            this.KneeLeft = _14;
            this.AnkleLeft = _15;
            this.FootLeft = _16;
            this.HipRight = _17;
            this.KneeRight = _18;
            this.AnkleRight = _19;
            this.AnkleLeft = _20;
        }

    }
}
