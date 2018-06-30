using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkinsonsKinectApplication.KinectModule
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string connectionIDValue;

        public string ConnectionID
        {
            get
            {
                return this.connectionIDValue;
            }
            set
            {
                if (this.connectionIDValue != value)
                {
                    this.connectionIDValue = value;
                    this.OnNotifyPropertyChange("ConnectionID");
                }
            }
        }

        public void OnNotifyPropertyChange(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs
                (propertyName));
            }
        }

    }
}
