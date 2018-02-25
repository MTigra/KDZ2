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
        private List<int> phones;
        private List<int> faxes;
        private List<string> email;
        private List<string> webSite;


        private int ParsePhoneOrFax(string number)
        {
            

            //Pattern of regExp match phone numbers
            string pattern = @"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?(\d{3})[\- ]?(\d{2})[\- ]?(\d{2})$";
            Regex rex = new Regex(pattern);
            

            if (rex.IsMatch(number))
            {
               
                var group = rex.Match((number)).Groups;
                int res = int.Parse(group[3].Value + group[4] + group[5] + group[6]);

                return res;
            }
            else
            {
                throw new FormatException("Номер телефона введен неверно. Формат ввода: (код города) ХХХ-ХХ-ХХ");
            }
        }

        private List<int> GetPhoneFaxList(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;
            List<int> numberList = new List<int>();
            string[] lines = line.Split(';');
            for (int i = 0; i < lines.Length; i++)
            {
                numberList.Add(ParsePhoneOrFax(lines[i]));
            }
            return numberList;

        }

        //TODO: process data before set in field
        public List<int> Phones
        {
            get
            {
                return phones;
            }

            private set { phones = value; }
        }

        public List<int> Faxes
        {
            get
            {
                return faxes;
            }

            private set
            {
                faxes = value;
            }
        }

        public List<string> Email
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

        public List<string> WebSite
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

        private List<string> GetEmailList(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;
            List<string> emailList = new List<string>();
            string[] lines = line.Split(';');
            for (int i = 0; i < lines.Length; i++)
            {
                //Todo:Add checking correct email
                emailList.Add(lines[i]);
            }
            return emailList;
        }

        private List<string> GetWebSiteList(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;
            List<string> websiteList = new List<string>();
            string[] lines = line.Split(';');
            for (int i = 0; i < lines.Length; i++)
            {
                //Todo:Add checking correct website
                websiteList.Add(lines[i]);
            }
            return websiteList;
        }

        public Contact(string phone, string fax, string email, string webSite)
        {
            Phones = GetPhoneFaxList(phone);
            Faxes = GetPhoneFaxList(fax);
            Email = GetEmailList(email);
            WebSite = GetWebSiteList(webSite);
        }
    }
}