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
        private Adress adress;
        /// <summary>
        /// Represnts a legal adress of this <see cref="T:ClassLib.Hotel" />
        /// </summary>
        private Adress legalAdress;
        /// <summary>
        /// Represents a contacts of this <see cref="T:ClassLib.Hotel" />
        /// </summary>
        private Contact contacts;

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
        public Hotel(Adress adress, Adress legalAdress, string phone,string fax,string email, string webSite, string geotype, float x, float y)
        {
            this.adress = adress;
            this.legalAdress = legalAdress;
            contacts=new Contact(phone,fax,email,webSite);
        }

    }
}
