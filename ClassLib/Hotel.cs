using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib
{
    public class Hotel
    {
        private Adress adress;
        private Adress legalAdress;
        private Contact contacts;
        public Hotel(Adress adress, Adress legalAdress, string phone,string fax,string email, string webSite, string geotype, float x, float y)
        {
            this.adress = adress;
            this.legalAdress = legalAdress;
            contacts=new Contact(phone,fax,email,webSite);
        }

    }
}
