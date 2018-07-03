using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkinsonsKinectApplication
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string connectionIDValue;
        private string sensorStatusValue;
        private string deviceIdValue;
        private bool isColorStreamEnabledValue;
        private bool isDepthStreamEnabledValue;
        private bool isSkeletonStreamEnabledValue;
        private bool isFrameRateEnabledValue;
        private bool isFrameNumberEnabledValue;
        private bool isGrayScaleEnabledValue;
        private int frameRateValue;
        private int frameNumberValue;
        private ColorImageFormat currentImageFormatValue;
        private ObservableCollection<ColorImageFormat> colorImageFormatvalue;
        private int sensorAngleValue;
        private bool canStartValue;
        private bool canStopValue;

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


        public string DeviceID
        {
            get
            {
                return this.deviceIdValue;
            }

            set
            {
                if (this.deviceIdValue != value)
                {
                    this.deviceIdValue = value;
                    this.OnNotifyPropertyChange("DeviceID");
                }
            }
        }

        public string SensorStatus
        {
            get
            {
                return this.sensorStatusValue;
            }

            set
            {
                if (this.sensorStatusValue != value)
                {
                    this.sensorStatusValue = value;
                    this.OnNotifyPropertyChange("SensorStatus");
                }
            }
        }

        public bool IsColorStreamEnabled
        {
            get
            {
                return this.isColorStreamEnabledValue;
            }

            set
            {
                if (this.isColorStreamEnabledValue != value)
                {
                    this.isColorStreamEnabledValue = value;
                    this.OnNotifyPropertyChange("IsColorStreamEnabled");
                }
            }
        }

        public bool IsDepthStreamEnabled
        {
            get
            {
                return this.isDepthStreamEnabledValue;
            }

            set
            {
                if (this.isDepthStreamEnabledValue != value)
                {
                    this.isDepthStreamEnabledValue = value;
                    this.OnNotifyPropertyChange("IsDepthStreamEnabled");
                }
            }
        }

        public bool IsSkeletonStreamEnabled
        {
            get
            {
                return this.isSkeletonStreamEnabledValue;
            }

            set
            {
                if (this.isSkeletonStreamEnabledValue != value)
                {
                    this.isSkeletonStreamEnabledValue = value;
                    this.OnNotifyPropertyChange("IsSkeletonStreamEnabled");
                }
            }
        }

        public int SensorAngle
        {
            get
            {
                return this.sensorAngleValue;
            }

            set
            {
                if (this.sensorAngleValue != value)
                {
                    this.sensorAngleValue = value;
                    this.OnNotifyPropertyChange("SensorAngle");
                }
            }
        }

        public bool CanStart
        {
            get
            {
                return this.canStartValue;
            }

            set
            {
                if (this.canStartValue != value)
                {
                    this.canStartValue = value;
                    this.OnNotifyPropertyChange("CanStart");
                }
            }
        }

        public bool CanStop
        {
            get
            {
                return this.canStopValue;
            }

            set
            {
                if (this.canStopValue != value)
                {
                    this.canStopValue = value;
                    this.OnNotifyPropertyChange("CanStop");
                }
            }
        }
        public bool IsFrameRateEnabled
        {
            get
            {
                return this.isFrameRateEnabledValue;
            }

            set
            {
                if (this.isFrameRateEnabledValue != value)
                {
                    this.isFrameRateEnabledValue = value;
                    this.OnNotifyPropertyChange("IsFrameRateEnabled");
                }
            }
        }

        public int FrameRate
        {
            get
            {
                return this.frameRateValue;
            }

            set
            {
                if (this.frameRateValue != value)
                {
                    this.frameRateValue = value;
                    this.OnNotifyPropertyChange("FrameRate");
                }
            }
        }

        public bool IsFrameNumberEnabled
        {
            get
            {
                return this.isFrameNumberEnabledValue;
            }

            set
            {
                if (this.isFrameNumberEnabledValue != value)
                {
                    this.isFrameNumberEnabledValue = value;
                    this.OnNotifyPropertyChange("IsFrameNumberEnabled");
                }
            }
        }

        public int FrameNumber
        {
            get
            {
                return this.frameNumberValue;
            }

            set
            {
                if (this.frameNumberValue != value)
                {
                    this.frameNumberValue = value;
                    this.OnNotifyPropertyChange("FrameNumber");
                }
            }
        }

        public bool IsGrayScaleEnabled
        {
            get
            {
                return this.isGrayScaleEnabledValue;
            }

            set
            {
                if (this.isGrayScaleEnabledValue != value)
                {
                    this.isGrayScaleEnabledValue = value;
                    this.OnNotifyPropertyChange("IsGrayScaleEnabled");
                }
            }
        }
        public ObservableCollection<ColorImageFormat> ColorImageFormats
        {
            get
            {
                this.colorImageFormatvalue = new ObservableCollection<ColorImageFormat>();
                foreach (ColorImageFormat colorImageFormat in Enum.GetValues(typeof(ColorImageFormat)))
                {
                    this.colorImageFormatvalue.Add(colorImageFormat);
                }

                return this.colorImageFormatvalue;
            }
        }

        /// <summary>
        /// Gets or sets the current image format.
        /// </summary>
        /// <value>
        /// The current image format.
        /// </value>
        public ColorImageFormat CurrentImageFormat
        {
            get
            {
                return this.currentImageFormatValue;
            }

            set
            {
                if (this.currentImageFormatValue != value)
                {
                    this.currentImageFormatValue = value;
                    this.OnNotifyPropertyChange("CurrentImageFormat");
                }
            }
        }

        public void OnNotifyPropertyChange(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
