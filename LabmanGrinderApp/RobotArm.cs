using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace LabmanGrinderApp
{
    class RobotArm : INotifyPropertyChanged
    {
        private bool hasVial;
        private string status;
        public event PropertyChangedEventHandler PropertyChanged;

        public RobotArm()
        {
            Location = "inputRack";
            status = "Idle";
            HasVial = false;
        }

        public Vial CurrentVial { get; set; }

        public string Location { get; set; }

        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }

        public bool HasVial
        {
            get { return hasVial; }
            set
            {
                hasVial = value;
                OnPropertyChanged("HasVial");
            }
        }

        public void PickUpVial(Vial vial)
        {
            if (CurrentVial == null)
            {
                CurrentVial = vial;
                HasVial = true;
            }
            else
            {
                throw new InvalidOperationException("Arm is already holding vial");
            }
        }

        public Vial DropVial()
        {
            Vial v = CurrentVial;
            CurrentVial = null;
            HasVial = false;
            return v;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

    }
}
