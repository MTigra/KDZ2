using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib
{
    public class Adress
    {
        private string adress = "";

        public Adress(string str)
        {
            adress = str;
        }

        public override string ToString()
        {
            return adress;
        }
    }
}
