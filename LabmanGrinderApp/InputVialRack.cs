using System;
using System.Linq;

namespace LabmanGrinderApp
{
    class InputVialRack : VialRack
    {
        private int inputVialWeight = 100;

        public InputVialRack()
        {
            for (int i = 0; i < 96; i++)
            {
                Vials.Add(new Vial(inputVialWeight));
            }
        }

        public int InputVialWeight
        {
            get { return inputVialWeight; }
        }

        public Vial GetFullVial()
        {
            Vial fv = (from v in Vials
                       where v.Weight > 0
                       select v).FirstOrDefault();
            if (fv != null)
            {
                Vials.Remove(fv);
                OnPropertyChanged("VialsInRack");
                OnPropertyChanged("CombinedWeightOfVialContents");
                OnPropertyChanged("EmptyVialsInRack");
                return fv;
            }
            else
            {
                throw new InvalidOperationException("All input vials are empty");
            }
        }

        public int CountOfFullVials()
        {
            int cnt = (from v in Vials
                       where v.Weight > 0
                       select v).Count();
            return cnt;
        }

    }
}
