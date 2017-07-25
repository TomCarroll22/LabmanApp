using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabmanGrinderApp
{
    class Vial
    {
        private int weight;

        public Vial(int weightOfContents)
        {
            weight = weightOfContents;
        }

        public int Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        public string Location { get; set; }
    }
}
