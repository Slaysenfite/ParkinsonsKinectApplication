using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualBasic.FileIO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace ParkinsonsKinectApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum Classification_Classes { setosa, versicolor, virginica };

        private static String output;

        public const String Filename = "irisdata.txt";
        public MainWindow()
        {
            InitializeComponent();

            
        }

        public void kNNMethod()
        {
            output = "Begin k-NN classification demo";

            List<Iris> trainData = readData(Filename);

            int numFeatures = 4;  // predictor variables
            int numClasses = 3;   // 0, 1, 2

            double[] unknown = new double[] { 5.25, 1.75, 5.2, 3.8 };

            output += "\nClassifying item with predictor values:  5.25 1.75 5.2 3.8";

            int k = 1;
            output += "\nK = 1";

            int predicted = Classify(unknown, trainData, numClasses, k);
            output += "\nPredicted class = " + predicted;

            k = 4;
            output += "\n\n With k = 4";
            predicted = Classify(unknown, trainData, numClasses, k);
            output += "\nPredicted class = " + predicted;

            output += "\nEnd k-NN demo";

            txtApp.Text = output;
        }

        public List<Iris> readData(String Filename)
        {
            using (var reader = new StreamReader(@Filename))
            {
                List<Iris> irisList = new List<Iris>();
                while (!reader.EndOfStream)
                {
                    String line = reader.ReadLine();
                    String[] values = line.Split(',').ToArray();

                    double sl = double.Parse(values[0], CultureInfo.InvariantCulture);
                    double sw = double.Parse(values[1], CultureInfo.InvariantCulture);
                    double pl = double.Parse(values[2], CultureInfo.InvariantCulture);
                    double pw = double.Parse(values[3], CultureInfo.InvariantCulture);
                    String ic = values[4];

                    irisList.Add(new Iris(sl, sw, pl, pw, ic));
                }
                return irisList;
            }

        }

        public static int Classify(double[] unknown, List<Iris> trainData, int numClasses, int k)
        {
            int n = trainData.Count;
            IndexAndDistance[] info = new IndexAndDistance[n];
            for (int i = 0; i < n; ++i)
            {
                IndexAndDistance curr = new IndexAndDistance();
                double dist = Distance(unknown, trainData[i].getDataArray());
                curr.i = i;
                curr.dist = dist;
                info[i] = curr;
            }

            Array.Sort(info);  // sort by distance
            output += "\nNearest  /  Distance  / Class";
            output += "\n==============================";
            for (int i = 0; i < k; ++i)
            {
                int c = trainData.ElementAt(info[i].i).getClassNum();
                string dist = info[i].dist.ToString("F3");
                output += "\n( " + trainData.ElementAt(info[i].i).getPLength() + trainData.ElementAt(info[i].i).getSWidth() + trainData.ElementAt(info[i].i).getPLength() + trainData.ElementAt(info[i].i).getPWidth() + " ) : "
                    + dist + "        " + c;
            }
            int result = Vote(info, trainData, numClasses, k);  // k nearest classes
            return result;
        }

        static int Vote(IndexAndDistance[] info, List<Iris> trainData, int numClasses, int k)
        {
            int[] votes = new int[numClasses];
            for (int j = 0; j < k; ++j)  // just first k nearest
            {
                int idx = info[j].i;  // which item
                Boolean b = trainData.ElementAt(j).getIClass().Equals("Iris-setosa");
                String o = trainData.ElementAt(j).getSLength() + "," + trainData.ElementAt(j).getSWidth() + "," 
                           + trainData.ElementAt(j).getPLength() + "," + trainData.ElementAt(j).getPWidth() + "," 
                           + trainData.ElementAt(j).getIClass() + " || " + trainData.ElementAt(j).getClassNum();
                int c = trainData.ElementAt(j).getClassNum();
                ++votes[c];
            }
            int mostVotes = 0;
            int classWithMostVotes = 0;
            for (int j = 0; j < numClasses; ++j)
            {
                if (votes[j] > mostVotes)
                {
                    mostVotes = votes[j];
                    classWithMostVotes = j;
                }
            }

            return classWithMostVotes;
        }


        static double Distance(double[] unknown, double[] data)
        {
            double sum = 0.0;
            for (int i = 0; i < unknown.Length; ++i)
                sum += (unknown[i] - data[i]) * (unknown[i] - data[i]);
            return Math.Sqrt(sum);
        }

        private void btnClassify_Click(object sender, RoutedEventArgs e)
        {
            kNNMethod();
        }
    }
}
