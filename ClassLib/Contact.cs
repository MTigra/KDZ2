using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClassLib
{
    public class Contact
    {
        private List<long> phones;
        private List<long> faxes;
        private List<string> email;
        private List<string> webSite;


        private long ParsePhoneOrFax(string number)
        {
            

            //Pattern of regExp match phone numbers
            string pattern = @"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?(\d{3})[\- ]?(\d{2})[\- ]?(\d{2})$";
            Regex rex = new Regex(pattern);
            

            if (rex.IsMatch(number))
            {
               
                var group = rex.Match((number)).Groups;
                string s = group[3].Value.Trim(')', '(',' ') + group[4].Value + group[5].Value + group[6].Value;
                long res = long.Parse(s);

                return res;
            }
            else
            {
                throw new FormatException("Номер телефона введен неверно. Формат ввода: (код города) ХХХ-ХХ-ХХ");
            }
        }

        private List<long> GetPhoneFaxList(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;
            List<long> numberList = new List<long>();
            string[] lines = line.Split(';');
            for (int i = 0; i < lines.Length; i++)
            {
                numberList.Add(ParsePhoneOrFax(lines[i]));
            }
            return numberList;

        }

        public string Faxs
        {
            get
            {
                string s = string.Empty;
                if (Faxes == null) return s;
                for (int i = 0; i < Faxes.Count; i++)
                {
                    
                    s += Faxes[i].ToString("(###) ###-##-##") ;
                    if (i != Faxes.Count - 1) s += ";";
                }
                return s;
            }
        }

        public string ContactPhone
        {
            get
            {
               
                string s = string.Empty;
                if (Phones == null) return s;
                for (int i = 0; i < Phones.Count; i++)
                {

                    s += Phones[i].ToString("(###) ###-##-##");
                    if (i != Phones.Count - 1) s += ";";
                }
                return s;
            }
            set
            {
                Phones=  GetPhoneFaxList(value);
                //string s = value;
                //string ret = "";
                //for (int i = 0; i < s.Length; i++)
                //{
                //    if (char.IsDigit(s[i])) ret += s[i];
                //}
                //Phones.Add(ret);

            }
            //todo: Add setter
        }

        
        private List<long> Phones
        {
            get
            {
                return phones;
            }

             set { phones = value; }
        }

        private List<long> Faxes
        {
            get
            {
                return faxes;
            }

             set
            {
                faxes = value;
            }
        }

        private List<string> Email
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

        public string Emails
        {
            get
            {

                string s = "";
                if (Email == null) return s;
                for (int i = 0; i < Email.Count; i++)
                {
                    s += Email[i];
                }
                return s;
            }
        }

        public string WebSites
        {
            get
            {
                string s = "";
                if (WebSiteList == null) return s;
                for (int i = 0; i < WebSiteList.Count; i++)
                {
                    s += WebSiteList[i];
                }
                return s;
            }
            set
            {
                WebSiteList = GetWebSiteList(value);
             
            }
        }

        private List<string> WebSiteList
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
            WebSiteList = GetWebSiteList(webSite);
        }
    }
}