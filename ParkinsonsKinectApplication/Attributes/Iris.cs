using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkinsonsKinectApplication
{
    public class Iris
    {
        private double sepal_length;
        private double sepal_width;
        private double petal_length;
        private double petal_width;
        private String iris_class;
        private int class_num;
        private double[] data;

        public Iris()
        {
            sepal_width = 0;
            sepal_length = 0;
            petal_width = 0;
            petal_length = 0;
            class_num = -1;
            iris_class = "";
            data = null;
        }

        public Iris(double sl, double sw, double pl, double pw, String ic)
        {
            sepal_length = sl;
            sepal_width = sw;
            petal_length = pl;
            petal_width = pw;
            iris_class = ic;
            if (ic.Equals("Iris-setosa")) { class_num = 0;}
            else if (ic.Contains("Iris-versicolor")) { class_num = 1;}
            else if (ic.Contains("Iris-virginica")) { class_num = 2;}
            else { class_num = -1;}
            data = new double[] {sl, sw, pl, pw};

        }

        public double getSLength()
        {
            return sepal_length;
        }
        public double getPLength()
        {
            return petal_length;
        }

        public double getSWidth()
        {
            return sepal_width;
        }
        public double getPWidth()
        {
            return petal_width;
        }

        public String getIClass()
        {
            return iris_class;
        }

        public int getClassNum()
        {
            return class_num;
        }

        public double[] getDataArray()
        {
            return data;
        }
    }
}
