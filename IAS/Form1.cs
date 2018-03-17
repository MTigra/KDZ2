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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using BindingFiltering;
using ClassLib;
using ExcelDataReader;
using Excel = Microsoft.Office.Interop.Excel;
using ClosedXML.Excel;
using ExcelDataReader.Exceptions;


namespace IAS
{

    public partial class Form1 : Form
    {

        //List<string> header = new List<string>();
        // DataTable dt = new DataTable();

        FilteredBindingList<Hotel> bl = new FilteredBindingList<Hotel>();

        public Form1()
        {
            InitializeComponent();

        }


        /// <summary>
        /// Откртие файла
        /// </summary>
        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {

            dataGridView1.AutoGenerateColumns = false;


            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                string filepath = openFileDialog1.FileName;
                try
                {
                    using (var stream = File.Open(filepath, FileMode.Open, FileAccess.ReadWrite))
                    {

                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            bl.Clear();
                            do
                            {
                                bool first = true;
                                while (reader.Read() && !IsEmptyRow(reader))
                                {
                                    if (first)
                                    {
                                        int i = 0;
                                        foreach (DataGridViewColumn col in dataGridView1.Columns)
                                        {
                                            if (col.HeaderText == "Index") continue;

                                            if (col.HeaderText != "Index" && reader.GetString(i) != col.HeaderText)
                                            {
                                                MessageBox.Show(
                                                    "Данный файл нельзя открыть. Откройте таблицу про гостиницы.");
                                                return;
                                            }
                                            i++;
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
                            button1.Visible = true;
                            button2.Visible = true;
                        }
                    }
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (HeaderException ex)
                {
                    MessageBox.Show("Данный файл невозможно открыть. Вероятно, он поврежден.");
                }

            }
            else
            {
                return;
            }

            BindingSource bs = new BindingSource();
            bs.DataSource = bl;
            dataGridView1.DataSource = bs;
        }


        /// <summary>
        /// проверяет пустая ли строка в таблице Excel
        /// </summary>
        private bool IsEmptyRow(IExcelDataReader reader)
        {
            for (var i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetValue(i) != null)
                    return false;
            }
            return true;
        }


        /// <summary>
        /// Обрабатывает клик на кнопку "Фильтр"
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {

            if (button1.Text == "Сбросить Фильтр")
            {
                FilterForm a = new FilterForm(dataGridView1);
                a.ShowDialog();
                a.Close();
                button1.Text = "Фильтр";
                return;
            }
            if (button1.Text == "Фильтр")
            {
                FilterForm a = new FilterForm(dataGridView1);
                a.ShowDialog();
                button1.Text = "Сбросить Фильтр";
            }
        }

        /// <summary>
        /// Обрабатывает событие, когда Datagridview начинает форматировать отображение ячейки
        /// </summary>
        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

            try
            {
                if ((dataGridView1.Rows[e.RowIndex].DataBoundItem != null) &&
                    (dataGridView1.Columns[e.ColumnIndex].DataPropertyName.Contains(".")))
                {
                    e.Value = BindProperty(
                        dataGridView1.Rows[e.RowIndex].DataBoundItem,
                        dataGridView1.Columns[e.ColumnIndex].DataPropertyName
                    );
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Возвращает значение, содержащееся в свойстве через Рефлексию
        /// </summary>
        /// <param name="property"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
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
                retValue = propertyInfo.GetValue(property, null) == null ? string.Empty : propertyInfo.GetValue(property, null).ToString();

            }

            return retValue;
        }

        /// <summary>
        /// Сохранение в файл
        /// </summary>
        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {

                string path = saveFileDialog1.FileName;
                var aa = new ClosedXML.Excel.XLWorkbook();

                var ws = aa.AddWorksheet(ToDataTable(dataGridView1), "name");
                ws.Tables.First().ShowAutoFilter = false;
                ws.Tables.First().Theme = XLTableTheme.None;
                ws.Style = XLWorkbook.DefaultStyle;
                aa.SaveAs(path);
                MessageBox.Show("Файл сохранен.");
            }
        }

        /// <summary>
        /// переводит Datagridview в DataTable, чтобы сохранить данные в файл
        /// </summary>
        private DataTable ToDataTable(DataGridView dataGridView)
        {

            var dt = new DataTable();
            foreach (DataGridViewColumn dataGridViewColumn in dataGridView.Columns)
            {
                if (dataGridViewColumn.Visible)
                {
                    //колонка индекс не должна быть в Excel.
                    if (dataGridViewColumn.HeaderText != "Index")
                    {
                        dt.Columns.Add(dataGridViewColumn.HeaderText);
                    }
                }
            }
            string[] cell = new string[dataGridView.Columns.Count - 1];
            foreach (DataGridViewRow dataGridViewRow in dataGridView.Rows)
            {
                int k = 0;
                for (int i = 0; i < dataGridViewRow.Cells.Count; i++)
                {
                    if (dataGridViewRow.Cells[i].OwningColumn.Name != "Index")
                    {
                        cell[k] = dataGridViewRow.Cells[i].FormattedValue.ToString();
                        k++;
                    }
                }
                dt.Rows.Add(cell);
            }
            return dt;
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            var a = e.Context;
            var colidx = e.ColumnIndex;
            var d = e.RowIndex;
            var ex = e.Exception.StackTrace;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

        }

        /// <summary>
        /// Устанавливает значение в свойство объекта, используя рефлексию
        /// </summary>
        private void SetProperty(object property, string propertyName, string val)
        {

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
                        SetProperty(
                            propertyInfo.GetValue(property, null),
                            propertyName.Substring(propertyName.IndexOf(".") + 1), val);
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
                if (propertyInfo.CanWrite)
                {
                    propertyInfo.SetValue(property, val);
                }
            }
        }

        /// <summary>
        /// Обрабатывает событие, когда пользователь покидает режим правки datagridview
        /// </summary>
        private void dataGridView1_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if ((dataGridView1.Rows[e.RowIndex].DataBoundItem != null) && (dataGridView1.Columns[e.ColumnIndex].DataPropertyName.Contains(".")))
            {
                SetProperty(
                    dataGridView1.Rows[e.RowIndex].DataBoundItem,
                    dataGridView1.Columns[e.ColumnIndex].DataPropertyName, Convert.ToString(e.Value));
            }
            if (dataGridView1.Columns[e.ColumnIndex].DataPropertyName == "LegalAddress.FullAddress")
            {
                dataGridView1.Refresh();

            }
        }

        /// <summary>
        /// Обрабатывает клик на кнопку "Диаграмма"
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            ChartingForm chartingForm = new ChartingForm(dataGridView1);
            chartingForm.Show();
        }

        /// <summary>
        /// Срабатывает при завершении проверки на корректность
        /// </summary>
        private void dataGridView1_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.Rows[e.RowIndex].ErrorText = null;
        }

        /// <summary>
        /// Проверяет на корректность введенное значение пользователем в ячейку
        /// </summary>
        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            dataGridView1.Rows[e.RowIndex].ErrorText = "";
            if (dataGridView1.Rows[e.RowIndex].IsNewRow) { return; }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "geoData" && dataGridView1[e.ColumnIndex, e.RowIndex].IsInEditMode)
            {
                string val = e.FormattedValue.ToString();
                string pattern = @"^([a-zA-Z]+);\s*(\d+[,]?\d+|\d{1,3})?;\s*(\d+[,]?\d+|\d{1,3})?$";
                Regex rex = new Regex(pattern);
                MatchCollection matches = rex.Matches(val);
                if (matches.Count <= 0)
                {
                    e.Cancel = true;
                    dataGridView1.Rows[e.RowIndex].ErrorText =
                        "Ошибка при вводе. Введите геоданные в формате: \"ТипТочки; xx,xx; yy,yy\"";
                }
            }

            if ((dataGridView1.Columns[e.ColumnIndex].Name == "ContactPhone" ||
                 dataGridView1.Columns[e.ColumnIndex].Name == "Fax") &&
                dataGridView1[e.ColumnIndex, e.RowIndex].IsInEditMode)
            {
                string val = e.FormattedValue.ToString();
                string[] lines = val.Split(';');
                for (int i = 0; i < lines.Length; i++)
                {
                    if (!Contact.CanParse(lines[i]))
                    {
                        e.Cancel = true;
                        dataGridView1.Rows[e.RowIndex].ErrorText =
                            "Ошибка при вводе. Номер телефона и Факс необходимо вводить в таком формате: \"(XXX) XXX-XX-XX\" либо \"XXXXXXXXXX\"\n" +
                            "Если желаете ввести несколько номеров, то вводите через \";\" без пробела";
                        return;

                    }
                }

            }
        }

        /// <summary>
        /// Открытие справки
        /// </summary>
        private void справкаToolStripButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("КДЗ по Программированию: Иформационно-справочная система о гостиницах." +
                            "Работу Выполнил Мартиросян Т.О. студент 1 курса ПИ НИУ ВШЭ ");
        }

        /// <summary>
        /// Клик на кнопку выход
        /// </summary>
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

