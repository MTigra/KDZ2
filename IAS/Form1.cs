using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
using ClosedXML.Excel;


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
                try
                {
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
                                        bl.Add(new Hotel(new Address(reader.GetString(5)),
                                            new Address(reader.GetString(6)),
                                            reader.GetString(0), reader.GetString(1),
                                            reader.GetString(7), reader.GetString(2), reader.GetString(3),
                                            reader.GetString(4), reader.GetString(11), reader.GetString(12),
                                            reader.GetString(13), reader.GetString(14), reader.GetString(15),
                                            reader.GetString(16),
                                            reader.GetString(8), reader.GetString(9), reader.GetString(10),
                                            reader.GetString(17)));
                                    }
                                }
                            } while (reader.NextResult());
                        }
                    }
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            else
            {
                return;
            }
            //а тут не работает, проблема в том что походу дела он не знает как представить втаблице такие объекты к
            dataGridView1.DataSource = bl;
            dataGridView1.Refresh();


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
            bl.OrderBy(x => x.Address.ToString());
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

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {

                string path = saveFileDialog1.FileName;
                var aa = new ClosedXML.Excel.XLWorkbook();


                System.Diagnostics.Stopwatch sw = new Stopwatch();
                sw.Start();
                var ws = aa.AddWorksheet(ToDataTable(dataGridView1), "name");
                ws.Tables.First().ShowAutoFilter = false;
                ws.Tables.First().Theme = XLTableTheme.None;
                ws.Style = XLWorkbook.DefaultStyle;
                aa.SaveAs(path);

                sw.Stop();
                MessageBox.Show(sw.Elapsed.ToString());
            }
        }

        private DataTable ToDataTable(DataGridView dataGridView)
        {

            var dt = new DataTable();
            foreach (DataGridViewColumn dataGridViewColumn in dataGridView.Columns)
            {
                if (dataGridViewColumn.Visible)
                {
                    dt.Columns.Add(dataGridViewColumn.HeaderText);
                }
            }
            string[] cell = new string[dataGridView.Columns.Count];
            foreach (DataGridViewRow dataGridViewRow in dataGridView.Rows)
            {
                for (int i = 0; i < dataGridViewRow.Cells.Count; i++)
                {
                    cell[i] = dataGridViewRow.Cells[i].FormattedValue.ToString();
                }
                dt.Rows.Add(cell);
            }
            return dt;
        }

    }
}
