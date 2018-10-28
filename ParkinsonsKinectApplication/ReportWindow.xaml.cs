using ParkinsonsKinectApplication.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ParkinsonsKinectApplication
{
    /// <summary>
    /// Interaction logic for ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : Window
    {
        String[] labels; 

        public ReportWindow()
        {
            InitializeComponent();
            labels = new String[3];
            labels[0] = "None";
            labels[1] = "Regular";
            labels[2] = "Severe";
        }

        private int generateTrainingData()
        {
            return FileUtilities.pythonGenTrainingSet(FileUtilities.RELATIVE_PATH + "PythonScripts//AutomatedCovarianceScript.py");
        }

        private void response(int val)
        {
            if (val == 0)
            {
                MessageBox.Show("New training set created");
                ((MainWindow)Application.Current.MainWindow).txtOutToUser.Text += "New training set created.";
                busyIndicator.IsBusy = false;
            }
            if (val == -1)
            {
                MessageBox.Show("Unable to create new training set");
                ((MainWindow)Application.Current.MainWindow).txtOutToUser.Text += "New training set created";
                busyIndicator.IsBusy = false;
            }
        }

        private void btnTrain_Click(object sender, RoutedEventArgs e)
        {
            int val = -2;
            busyIndicator.IsBusy = true;
            var worker = new BackgroundWorker();
            worker.DoWork += (s, ev) => val = generateTrainingData();
            worker.RunWorkerCompleted += (s, ev) => response(val);
            worker.RunWorkerAsync();

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnClassify_Click(object sender, RoutedEventArgs e)
        {
            string userID = ((MainWindow)Application.Current.MainWindow).txtSubjectName.Text;
            if(userID == "")
            {
                MessageBox.Show("Please enter a subject name");
                return;
            }
            var userFiles = Directory.EnumerateFiles(FileUtilities.RELATIVE_PATH + "SkeletonJointFiles//", userID + "*.csv").ToList();
            if(userFiles.Count == 4)
            {
                int classified = FileUtilities.pythonClassifyUser(FileUtilities.RELATIVE_PATH + "PythonScripts//AutomatedCovarianceScript.py", userID);
                if (classified == 0 || classified == 1 || classified == 2)
                {
                    MessageBox.Show("Parkinsons Level of " + userID + ": " + labels[classified]);
                    ((MainWindow)Application.Current.MainWindow).txtOutToUser.Text += userID + " calssified with Parkinsons level: " + labels[classified];
                }
                else
                {
                    MessageBox.Show("Unable to classify" + userID);
                    ((MainWindow)Application.Current.MainWindow).txtOutToUser.Text += "Classification error";
                }
            }
            else
            {
                MessageBox.Show("Skeleton files for " + userID + " do not exist");
                ((MainWindow)Application.Current.MainWindow).txtOutToUser.Text += "Skeleton files for " + userID + " do not exist";
            }
        }
    }
}
