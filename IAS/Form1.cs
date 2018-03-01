using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using  ClassLib;
using ExcelDataReader;
using Excel = Microsoft.Office.Interop.Excel;

namespace IAS
{

    public partial class Form1 : Form
    {
        List<string> header = new List<string>();
            DataTable dt = new DataTable();
        //List<Hotel> Hotels= new List<Hotel>(200);
        BindingList<Hotel> bl=new BindingList<Hotel>();
        BindingList<Geo> bG = new BindingList<Geo>();
        public Form1()
        {
            InitializeComponent();

        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //dataGridView1.DataSource = bl;
                string filepath = openFileDialog1.FileName;
                //Excel.Application xlapp = new Excel.Application();
                //Excel.Workbook xlWorkBook = xlapp.Workbooks.Open(filepath);
                //Excel.Worksheet worksheet = xlWorkBook.Sheets[1];
                //Excel.Range xlRange = worksheet.UsedRange;
                  using (var stream = File.Open(filepath, FileMode.Open, FileAccess.Read))
                {

                    // Auto-detect format, supports:
                    //  - Binary Excel files (2.0-2003 format; *.xls)
                    //  - OpenXml Excel files (2007 format; *.xlsx)
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {

                        // Choose one of either 1 or 2:

                        // 1. Use the reader methods
                        do
                        {
                            bool first = true;
                            while (reader.Read()&& !IsEmptyRow(reader))
                            {
                                if (first)
                                {
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        header.Add(Convert.ToString(reader.GetValue(i)));
                                       // dataGridView1.Columns.Add(header[i],header[i]);
                                    }
                                    
                                    first = false;
                                    continue;
                                }   

                                for (int i = 0; i < reader.ResultsCount; i++)
                                {
                                    bl.Add(new Hotel(new Adress(reader.GetString(5)),new Adress(reader.GetString(6)),reader.GetString(7),
                                        reader.GetString(8),reader.GetString(9),reader.GetString(10),reader.GetString(17)));
                                }
                            }
                        } while (reader.NextResult());

                        
                    }
                }

                  //а тут не работает, проблема в том что походу дела он не знает как представить втаблице такие объекты к
                dataGridView1.DataSource = bl;
                dataGridView1.Refresh();
                

            }
            else
            {
                return;
            }
        }


        private bool IsEmptyRow(IExcelDataReader reader)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetValue(i) != null)
                    return false;
            }
            return true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //It Works
            bG.Add(new Geo("{type=Point, coordinates=[37.74551, 55.4409]}"));
            bG.Add(new Geo("{type=Point, coordinates=[34.14551, 12.7409]}"));
            bG.Add(new Geo("{type=Point, coordinates=[77.24551, 85.714409]}"));
            dataGridView1.DataSource = bG;
        }
    }
}
