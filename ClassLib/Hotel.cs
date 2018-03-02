using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib
{
    /// <summary>
    /// Represents a Hotel with some information about it. 
    /// </summary>
    public class Hotel
    {
        /// <summary>
        /// Repreents a de-facto adress of this <see cref="T:ClassLib.Hotel" />
        /// </summary>
        private Address address;
        /// <summary>
        /// Represnts a legal adress of this <see cref="T:ClassLib.Hotel" />
        /// </summary>
        private Address legalAddress;
        /// <summary>
        /// Represents a contacts of this <see cref="T:ClassLib.Hotel" />
        /// </summary>
        private Contact contacts;


        /// <summary>
        /// Represents a contacts of this <see cref="T:ClassLib.Hotel" />
        /// </summary>
        public Contact Contacts
        {
            get { return contacts; }
            set { contacts = value; }
        }

        private Geo geo;
        /// <summary>
        /// Initialize new instance of class <see cref="T:ClassLib.Hotel" /> with parameters.
        /// </summary>
        /// <param name="adress">Instance of Class Adress. De facto adress.</param>
        /// <param name="legalAdress">Instance of Class Adress. Legal adress</param>
        /// <param name="phone">String that represnts phone. If there are a few phone numbers, use ";" as separator. </param>
        /// <param name="fax">String that represents fax</param>
        /// <param name="email">String that represents E-mail</param>
        /// <param name="webSite">String that represents website</param>
        /// <param name="geotype">string that represents type of X,Y coordinates</param>
        /// <param name="x">Coordinate X</param>
        /// <param name="y">Coordinate y</param>
        public Hotel(Address adress, Address legalAdress, string phone, string fax, string email, string webSite, string geo)
        {
            this.Geo = new Geo(geo);
            this.address = adress;
            this.LegalAdress = legalAdress;
            Contacts = new Contact(phone, fax, email, webSite);
        }

        public Address Adress
        {
            get { return address; }
            set { address = value; }
        }

        /// <summary>
        /// Represnts a legal adress of this <see cref="T:ClassLib.Hotel" />
        /// </summary>
        public Address LegalAdress
        {
            get { return legalAddress; }
            set { legalAddress = value; }
        }

        

        public Geo Geo
        {
            get { return geo; }
            set { geo = value; }
        }
    }
}
