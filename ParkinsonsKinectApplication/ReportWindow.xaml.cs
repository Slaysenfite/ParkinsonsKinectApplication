using ParkinsonsKinectApplication.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public ReportWindow()
        {
            InitializeComponent();
        }

        private int generateTrainingData()
        {
            return FileUtilities.pythonGenTrainingSet(FileUtilities.RELATIVE_PATH + "PythonScripts//GenerateTrainingSet.py");
        }

        private void response(int val)
        {
            if (val == 0)
            {
                MessageBox.Show("New training set created");
                ((MainWindow)Application.Current.MainWindow).txtOutToUser.Text += "New training set created";
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
            String userID = ((MainWindow)Application.Current.MainWindow).txtSubjectName.Text;
        }
    }
}
