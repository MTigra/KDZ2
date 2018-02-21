using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                phone = value;
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
            this.phone = phone;
            this.fax = fax;
            this.email = email;
            this.webSite = webSite;
        }
    }
}