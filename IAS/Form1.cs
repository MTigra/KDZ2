using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassLib;
using ExcelDataReader;
using Excel = Microsoft.Office.Interop.Excel;

namespace IAS
{

    public partial class Form1 : Form
    {
        List<string> header = new List<string>();
        DataTable dt = new DataTable();
        //List<Hotel> Hotels= new List<Hotel>(200);
        BindingList<Hotel> bl = new BindingList<Hotel>();
        BindingList<Geo> bG = new BindingList<Geo>();
        public Form1()
        {
            
            InitializeComponent();
            

        }




        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //dataGridView1.DataSource = bl;
                string filepath = openFileDialog1.FileName;
                //Excel.Application xlapp = new Excel.Application();
                //Excel.Workbook xlWorkBook = xlapp.Workbooks.Open(filepath);
                //Excel.Worksheet worksheet = xlWorkBook.Sheets[1];
                //Excel.Range xlRange = worksheet.UsedRange;
                using (var stream = File.Open(filepath, FileMode.Open, FileAccess.ReadWrite))
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
                            while (reader.Read() && !IsEmptyRow(reader))
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
                                    bl.Add(new Hotel(new Address(reader.GetString(5)), new Address(reader.GetString(6)), reader.GetString(7),
                                        reader.GetString(8), reader.GetString(9), reader.GetString(10), reader.GetString(17)));
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

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if ((dataGridView1.Rows[e.RowIndex].DataBoundItem != null) && (dataGridView1.Columns[e.ColumnIndex].DataPropertyName.Contains(".")))
            {
                e.Value = BindProperty(
                    dataGridView1.Rows[e.RowIndex].DataBoundItem,
                    dataGridView1.Columns[e.ColumnIndex].DataPropertyName
                );
            }
        }

        private string BindProperty(object property, string propertyName)
        {
            string retValue = "";

            if (propertyName.Contains("."))
            {
                PropertyInfo[] arrayProperties;
                string leftPropertyName;

                leftPropertyName = propertyName.Substring(0, propertyName.IndexOf("."));
                arrayProperties = property.GetType().GetProperties();

                foreach (PropertyInfo propertyInfo in arrayProperties)
                {
                    if (propertyInfo.Name == leftPropertyName)
                    {
                        retValue = BindProperty(
                            propertyInfo.GetValue(property, null),
                            propertyName.Substring(propertyName.IndexOf(".") + 1));
                        break;
                    }
                }
            }
            else
            {
                Type propertyType;
                PropertyInfo propertyInfo;

                propertyType = property.GetType();
                propertyInfo = propertyType.GetProperty(propertyName);
                retValue = propertyInfo.GetValue(property, null).ToString();
            }

            return retValue;
        }


    }
}
