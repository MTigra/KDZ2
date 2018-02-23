using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClassLib
{
    public class Contact
    {
        private string phone;
        private string fax;
        private string email;
        private string webSite;

        
      

    //TODO: process data before set in field
    public string Phone
        {
            get
            {
                return phone;
            }

            set
            {
                //Allow user to delete phone
                if (String.IsNullOrWhiteSpace(value))
                    phone = "";
                
                //Pattern of regExp match phone numbers
                string pattern = @"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?(\d{3})[\- ]?(\d{2})[\- ]?(\d{2})$";
                Regex rex = new Regex(pattern);
                string res = "";

                if (rex.IsMatch(value))
                {
                    var group = rex.Match((value)).Groups;
                    
                    res = string.Format($"({group[3]}) {group[4]}-{group[5]}-{group[6]}");
                    phone = res;
                }
                else
                {
                    throw new FormatException("Номер телефона введен неверно. Формат ввода: (код города) ХХХ-ХХ-ХХ");
                }
                
            }
        }

        public string Fax
        {
            get
            {
                return fax;
            }

            set
            {
                fax = value;
            }
        }

        public string Email
        {
            get
            {
                return email;
            }

            set
            {
                email = value;
            }
        }

        public string WebSite
        {
            get
            {
                return webSite;
            }

            set
            {
                webSite = value;
            }
        }

        public Contact(string phone, string fax, string email, string webSite)
        {
            Phone = phone;
            this.fax = fax;
            this.email = email;
            this.webSite = webSite;
        }
    }
}