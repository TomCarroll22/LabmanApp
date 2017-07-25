using System;
using System.Linq;

namespace LabmanGrinderApp
{
    class OutputVialRack : VialRack
    {     
        public OutputVialRack()
        {
            for (int i = 0; i < 96; i++)
            {
                Vials.Add(new Vial(0));
            }
        }

        public Vial GetEmptyVial()
        {
            Vial ev = (from v in Vials
                       where v.Weight == 0
                       select v).FirstOrDefault();

            if (ev != null)
            {
                Vials.Remove(ev);
                OnPropertyChanged("VialsInRack");
                OnPropertyChanged("CombinedWeightOfVialContents");
                OnPropertyChanged("EmptyVialsInRack");
                return ev;
            }
            else
            {
                throw new InvalidOperationException("No empty vials in rack");
            }
        }



    }
}
