using System;
using System.Threading;
using System.ComponentModel;

namespace LabmanGrinderApp
{
    class Dispenser : INotifyPropertyChanged
    {
        private Vial inputVial;
        private Vial outputVial;
        private int targetWeight;
        private int waste = 0;
        private bool inputVialLoaded = false;
        private bool outputVialLoaded = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public Vial InputVial
        {
            get { return inputVial; }
            private set
            {
                inputVial = value;
                OnPropertyChanged("InputVial");
            }
        }

        public Vial OutputVial
        {
            get { return outputVial; }
            private set
            {
                outputVial = value;
                OnPropertyChanged("OutputVial");
            }
        }

        public bool InputVialLoaded
        {
            get { return inputVialLoaded; }
            set
            {
                inputVialLoaded = value;
                OnPropertyChanged("InputVialLoaded");
            }
        }
        public bool OutputVialLoaded
        {
            get { return outputVialLoaded; }
            set
            {
                outputVialLoaded = value;
                OnPropertyChanged("OutputVialLoaded");
            }
        }
     
        public int TargetOutputWeight
        {
            get
            {
                return targetWeight;
            }
            set
            {
                targetWeight = value;
                OnPropertyChanged("TargetOutputWeight");
            }
        }

        public int Waste
        {
            get { return waste; }
            set
            {
                waste = value;
                OnPropertyChanged("Waste");
            }
        }

        public string Status { get; set; }

        public void LoadInputVial(Vial v)
        {
            if (InputVial == null)
            {
                InputVial = v;
                InputVial.Location = "Dispenser";
                InputVialLoaded = true;    
            }
            else
            {
                throw new InvalidOperationException("Dispenser already contains input vial");
            }
        }

        public Vial UnloadInputVial()
        {
            Vial v = InputVial;
            v.Location = "Arm";
            InputVialLoaded = false;
            InputVial = null;
            return v;
        }

        public void LoadOutputVial(Vial v)
        {
            if (OutputVial == null)
            {
                OutputVial = v;
                OutputVial.Location = "Dispenser";
                OutputVialLoaded = true;
            }
            else
            {
                throw new InvalidOperationException("Dispenser already contains output vial");
            }
        }

        public Vial UnloadOutputVial()
        {
            Vial v = OutputVial;
            v.Location = "Arm";
            OutputVial = null;
            OutputVialLoaded = false;
            return v;
        }

        public void Dispense()
        {
            if (InputVial == null)
            {
                throw new InvalidOperationException("No input vial in dispenser");
            }
            if (OutputVial == null)
            {
                throw new InvalidOperationException("No output vial in dispenser");
            }
            if (TargetOutputWeight == 0)
            {
                throw new InvalidOperationException("Desired output weight not set");
            }

            Status = "Dispensing";
            for (int i = 0; i < targetWeight; i++)
            {
                if (InputVial.Weight == 0)
                {
                    break;
                }
                InputVial.Weight -= 1;
                OnPropertyChanged("InputVial");
                OutputVial.Weight += 1;
                OnPropertyChanged("OutputVial");
                Thread.Sleep(100);
            }
            Status = "Complete";
        }

        public void DisposeWaste()
        {
            while (InputVial.Weight > 0)
            {
                InputVial.Weight -= 1;
                OnPropertyChanged("InputVial");
                Waste += 1;
                OnPropertyChanged("Waste");
                Thread.Sleep(100);
            }
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
