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

        /// <summary>
        /// Парсит Телефон или Факс, возвращая числовое представление
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static long ParsePhoneOrFax(string number)
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
        /// <summary>
        /// получает список номеров телефонов  или факсов из строки
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
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
        /// <summary>
        /// проверяет возможно ли распарсить строку
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
       static public bool CanParse(string s)
        {
            try
            {
                ParsePhoneOrFax(s);
            }
            catch (FormatException)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Получает или задает строкове представление списка факсов
        /// </summary>
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
            set
            {
                Faxes = GetPhoneFaxList(value);

            }
        }

        /// <summary>
        /// получает или задает строковое представление списка номеров телефонов
        /// </summary>
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

            }
            //todo: Add setter
        }

        /// <summary>
        /// получает или задает список телефонов
        /// </summary>
        private List<long> Phones
        {
            get
            {
                return phones;
            }

             set { phones = value; }
        }
        /// <summary>
        /// получает или задает список факсов
        /// </summary>
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
        /// <summary>
        /// Получает или задает список email
        /// </summary>
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

        /// <summary>
        /// получает или задает строкове представление списка Email
        /// </summary>
        public string Emails
        {
            get
            {

                string s = "";
                if (Email == null) return s;
                for (int i = 0; i < Email.Count; i++)
                {
                    s += Email[i];
                    if (i != Email.Count - 1) s += ";";
                }
                return s;
            }
            set
            {
                Email = GetEmailList(value);

            }
        }

        /// <summary>
        /// получает или задает строковое представление списка сайтов
        /// </summary>
        public string WebSites
        {
            get
            {
                string s = "";
                if (WebSiteList == null) return s;
                for (int i = 0; i < WebSiteList.Count; i++)
                {
                    s += WebSiteList[i];
                    if (i != WebSiteList.Count - 1) s += ";";
                }
                return s;
            }
            set
            {
                WebSiteList = GetWebSiteList(value);
             
            }
        }
        /// <summary>
        /// Получает список сайтов
        /// </summary>
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
       
        /// <summary>
        /// Получает список почт из строки
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private List<string> GetEmailList(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;
            List<string> emailList = new List<string>();
            string[] lines = line.Split(';');
            for (int i = 0; i < lines.Length; i++)
            {
                
                emailList.Add(lines[i].Replace(" ",""));
            }
            return emailList;
        }
        /// <summary>
        /// получает список вебсайтов из строки
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private List<string> GetWebSiteList(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;
            List<string> websiteList = new List<string>();
            string[] lines = line.Split(';');
            for (int i = 0; i < lines.Length; i++)
            {
               
                websiteList.Add(lines[i].Replace(" ", ""));
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