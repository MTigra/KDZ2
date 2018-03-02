using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib
{
    public class Address
    {
        private string adress = "";
        private int index;
        private string city;
        private string street;
        private string house;
        private string corpus;
        private string stroenie;
        private string pomeshenie;

        public Address(string str)
        {
            adress = str;
        }


        private void ParseAdress(string address)
        {
            string[] adressArr = address.Split(',');

        }

        public override string ToString()
        {
            return adress;
        }
    }
}
