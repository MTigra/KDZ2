using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClassLib;

namespace ContactTest
{
    [TestClass]
    public class ContactTest
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        // unit test code 
        [TestMethod]
        public void CorrectMatch() {
            // arrange
            
            Contact account = new Contact("+79296169700","55","5557","9987");
            // act
            //  account.Phone = "49857564668888";
            // assert
            
            
           // Assert.AreEqual("(929) 616-97-00",account.Phone);
           }

        //unit test method 
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Contact_WhenIncorrectPhone_ShouldThrowFormatException() {
            //  arrange
        //    double beginningBalance = 11.99;
            //double debitAmount = -100.00;
            Contact account = new Contact("+79296169700", "55", "5557", "9987");
          //  account.Phone = "49857564668888"; // assert is handled by ExpectedException 
        }  


    }
}
