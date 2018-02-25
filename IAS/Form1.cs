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
using Excel = Microsoft.Office.Interop.Excel;

namespace IAS
{
    public partial class Form1 : Form
    {
        List<Hotel> Hotels= new List<Hotel>(200);
        public Form1()
        {
            InitializeComponent();

        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filepath = openFileDialog1.FileName;
                Excel.Application xlapp = new Excel.Application();
                Excel.Workbook xlWorkBook = xlapp.Workbooks.Open(filepath);
                Excel.Worksheet worksheet = xlWorkBook.Sheets[1];
                Excel.Range xlRange = worksheet.UsedRange;

                for (int i = 1; i <= xlRange.Columns.Count; i++)
                {
                    for (int j = 1; j <= xlRange.Rows.Count; j++)
                    {
                       
                      //  xlRange.Cells[i, j].Value2.ToString();
                    }
                }

            }
            else
            {
                return;
            }
        }
    }
}
