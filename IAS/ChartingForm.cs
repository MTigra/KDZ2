using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ClassLib;

namespace IAS
{
    public partial class ChartingForm : Form
    {
        //по округам, районам, индексу, звездности
        private DataGridView dataGridView;

        public ChartingForm(DataGridView dgv)
        {
            dataGridView = dgv;
            InitializeComponent();
            PopulateComboBox();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataGridViewColumn col = comboBox1.SelectedItem as DataGridViewColumn;
            List<string> values = GetValuesOfColumn(col);
            List<int> counts = new List<int>();
            for (int i = 0; i < values.Count; i++)
            {
                counts.Add(CountOfSameCells(values[i], col));
            }

            //Prepare Chart
            chart1.Series[0].Points.Clear();
            chart1.Series[0].Name = col.HeaderText;
            chart1.Series[0].LabelAngle = 90;
          //  chart1.Series[0].IsXValueIndexed = true;
            for (int i = 0; i < values.Count; i++)
            {
                chart1.Series[0].Points.AddXY(values[i].ToString(), counts[i]);
            }

        }

        private void PopulateComboBox()
        {
            comboBox1.Items.AddRange(new DataGridViewColumn[]
            {
                dataGridView.Columns["AdmArea"],
                dataGridView.Columns["District"],
                dataGridView.Columns["Index"],
                dataGridView.Columns["Categorization"],
            });
            comboBox1.DisplayMember = "HeaderText";
        }

        /// <summary>
        /// Подсчитывает количество  одинаковых записей в оперделенном столбце
        /// </summary>
        private int CountOfSameCells(string val, DataGridViewColumn col)
        {
            int n = 0;
            for (int i = 0; i < dataGridView.Rows.Count - 1; i++)
            {
                if (dataGridView[col.Name, i].FormattedValue.ToString() == val) n++;
            }
            return n;
        }

        /// <summary>
        /// получает список значений в колонке
        /// </summary>
        private List<string> GetValuesOfColumn(DataGridViewColumn column)
        {

            BindingSource data = dataGridView.DataSource as BindingSource;
            DataGridViewColumn col = column;
            List<string> list = new List<string>(data.Count);

            
            foreach (Object item in data)
            {
                Object value = null;

                PropertyInfo[] properties = item.GetType().GetProperties(
                    BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo property in properties)
                {
                    if (String.Compare(col.DataPropertyName,
                            property.Name, true /*case insensitive*/,
                            System.Globalization.CultureInfo.InvariantCulture) == 0)
                    {
                        value = property.GetValue(item, null /*property index*/);
                        break;
                    }
                    else if (property.PropertyType == typeof(Address) ||
                             property.PropertyType == typeof(Contact) || property.PropertyType == typeof(Geo))
                    {
                        string propname = col.DataPropertyName.Substring(col.DataPropertyName.IndexOf('.') + 1);
                        if (string.Compare(col.DataPropertyName, property.Name + '.' + propname, true,
                                System.Globalization.CultureInfo.InvariantCulture) == 0)
                        {
                            object obj = property.GetValue(item, null);
                            Type propType = obj.GetType();
                            PropertyInfo pinf = propType.GetProperty(propname);
                            value = pinf.GetValue(obj, null) == null
                                ? string.Empty
                                : pinf.GetValue(obj, null).ToString();
                        }
                    }


                }


                
                if (!list.Contains(value))
                {
                    list.Add(value.ToString());
                }
            }
            return list;
        }
    }
}
