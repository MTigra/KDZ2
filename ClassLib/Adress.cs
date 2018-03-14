using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib
{
    public class Address
    {
        private string fullAddress;
        private int index;
        private string city;
        private string street;
        private string house;
        private string corpus;
        private string stroenie;
        private string pomeshenie;

        public Address(string str)
        {
            fullAddress = string.IsNullOrWhiteSpace(str) ? null : str;
        }


        private void ParseAdress(string address)
        {
            string[] adressArr = address.Split(',');

        }

        public string FullAddress
        {
            get { return fullAddress; }
            set
            {
               fullAddress= string.IsNullOrWhiteSpace(value) ? null : value;
            }
        }

        public string Index {
            get { return index.ToString(); }
        }
    }
}
