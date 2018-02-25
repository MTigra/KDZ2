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
        private int index;
        private string city;
        private string street;
        private string house;
        private string corpus;
        private string stroenie;
        private string pomeshenie;

        public Adress(string str)
        {
            adress = str;
        }


        private void ParseAdress(string adress)
        {
            string[] adressArr = adress.Split(',');

        }

        public override string ToString()
        {
            return adress;
        }
    }
}
