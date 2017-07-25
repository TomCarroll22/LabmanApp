using System;
using System.Threading;
using System.ComponentModel;

namespace LabmanGrinderApp
{
    class Grinder : INotifyPropertyChanged
    {
        private Vial currentVial;
        private string status = "Idle";
        public event PropertyChangedEventHandler PropertyChanged;

        public Vial CurrentVial
        {
            get { return currentVial; }
            private set { currentVial = value; }
        }

        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }

        public void LoadVial(Vial vial)
        {
            if (currentVial != null)
            {
                throw new InvalidOperationException("Grinder already contains vial");
            }
            CurrentVial = vial;
            CurrentVial.Location = "Grinder";
            Status = "Vial loaded";
        }

        public Vial UnloadVial()
        {
            Vial v = currentVial;
            v.Location = "Arm";
            currentVial = null;
            Status = "Idle";
            return v;
        }

        public void Grind()
        {
            if (CurrentVial == null)
            {
                throw new InvalidOperationException("No vial in grinder");
            }
            Status = "Grinding";
            Thread.Sleep(3000);
            Status = "Grind Complete";
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
