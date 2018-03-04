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
        public Hotel(Address address, Address legalAddress, string id, string fullName, string phone, string globalId,
            string Admarea, string district, string categorization,string certificateNumber, string certificateIssueDate,
            string numberInFederalList,string certificateValidity, string nameOfAccreditedOrg,string fax, string email, string webSite, string geo)
        {
            this.ID=int.Parse(id);
            this.FullName = fullName;
            this.GlobalId = int.Parse(globalId);
            this.AdmArea = Admarea;
            this.District = district;
            this.Categorization = categorization;
            this.CertificateNumber = certificateNumber;
            if (String.IsNullOrWhiteSpace(certificateIssueDate)) this.CertificateIssueDate = null;
            else
            {
                this.CertificateIssueDate = DateTime.Parse(certificateIssueDate);
            }
            var s = numberInFederalList.Trim();
            this.NumberInFederalList = int.Parse(s);
            if (String.IsNullOrWhiteSpace(certificateValidity)) this.CertificateValidity = null;
            else
            {
                this.CertificateValidity = DateTime.Parse(certificateValidity);
            }
            
            this.NameOfAccreditedOrganization = nameOfAccreditedOrg;
            this.GeoData = new Geo(geo);
            this.Address = address;
            this.LegalAddress = legalAddress;
            Contacts = new Contact(phone, fax, email, webSite);
        }

        public string CertificateNumber { get; set; }

        public string Categorization { get; set; }

        public DateTime? CertificateIssueDate { get; set; }

        public int NumberInFederalList { get; set; }

        public DateTime? CertificateValidity { get; set; }

        public int GlobalId { get; set; }

        public Address Address
        {
            get { return address; }
            set { address = value; }
        }

        /// <summary>
        /// Represnts a legal adress of this <see cref="T:ClassLib.Hotel" />
        /// </summary>
        public Address LegalAddress
        {
            get { return legalAddress; }
            set { legalAddress = value; }
        }

        public int ID { get; set; }

        public Geo GeoData
        {
            get { return geo; }
            set { geo = value; }
        }

        public string FullName { get;  set; }
        public string AdmArea { get;  set; }
        public string District { get;  set; }
        public string NameOfAccreditedOrganization { get; set; }
    }
}
