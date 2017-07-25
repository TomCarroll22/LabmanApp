using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace LabmanGrinderApp
{
    abstract class VialRack : INotifyPropertyChanged
    {
        public List<Vial> Vials = new List<Vial>();
        public event PropertyChangedEventHandler PropertyChanged;
        private int vialCapacity = 96;
      
        public int VialsInRack
        {
            get
            {
                return Vials.Count();
            }
        }

        public int EmptyVialsInRack
        {
            get
            {
                int e = (from v in Vials
                         where v.Weight == 0
                         select v).Count();

                return e;
            }
        }

        public int CombinedWeightOfVialContents
        {
            get
            {
                int w = (from v in Vials
                         select v.Weight).Sum();
                return w;
            }
        }

        public int VialCapacity
        {
            get { return vialCapacity; }
        }

        public void AddToRack(Vial v)
        {
            if (Vials.Count() >= vialCapacity)
            {
                throw new InvalidOperationException("No space in rack");
            }
            Vials.Add(v);
            OnPropertyChanged("VialsInRack");
            OnPropertyChanged("CombinedWeightOfVialContents");
            OnPropertyChanged("EmptyVialsInRack");         
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
