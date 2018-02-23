using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using  ClassLib;
namespace IAS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Contact ac = new Contact("49857564668888", "55", "5557", "9987");

        }
    }
}
