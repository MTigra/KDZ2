using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClassLib
{
    public class Address
    {
        private string fullAddress;
        private int index;


        Regex rex = new Regex(@"\b[\d]{6}\b");
        public Address(string str)
        {
            fullAddress = string.IsNullOrWhiteSpace(str) ? null : str;
            index = GetIndex(fullAddress);
        }


        private int GetIndex(string address)
        {

            int indx = 0;
            if (!string.IsNullOrWhiteSpace(address))
            {

                Match match = rex.Match(address);
                if (!string.IsNullOrWhiteSpace(match.Value))
                {
                    indx = int.Parse(match.Value);
                }
            }
            return indx;
        }

        public string FullAddress
        {
            get { return fullAddress; }
            set
            {
                fullAddress = string.IsNullOrWhiteSpace(value) ? null : value;
            }
        }

        public string Index
        {
            get { return index.ToString("000000"); }
        }
    }
}
