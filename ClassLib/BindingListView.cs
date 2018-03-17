using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using ClassLib;

namespace BindingFiltering
{

    public class FilteredBindingList<T> : BindingList<T>, IBindingListView
    {

        public FilteredBindingList()
        { }

        private List<T> originalListValue = new List<T>();
        public List<T> OriginalList
        {
            get
            { return originalListValue; }
        }


        #region Sorting
        private IEnumerable<T> items;
        bool isSortedValue;
        ListSortDirection sortDirectionValue;
        PropertyDescriptor sortPropertyValue;

        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        protected override bool IsSortedCore
        {
            get { return isSortedValue; }
        }

        protected override PropertyDescriptor SortPropertyCore
        {
            get { return sortPropertyValue; }
        }

        protected override ListSortDirection SortDirectionCore
        {
            get { return sortDirectionValue; }
        }

        /// <summary>
        /// Переопредленный метод для сортировки
        /// </summary>
        /// <param name="property"></param>
        /// <param name="direction"></param>
        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
        {
            sortPropertyValue = property;
            sortDirectionValue = direction;

            items = null;
            if (direction == ListSortDirection.Ascending)
            {
                items = this.OrderBy(item => property.GetValue(item));
            }
            else
            {
                items = this.OrderByDescending(item => property.GetValue(item));
            }
            this.RaiseListChangedEvents = false;
            ResetItems(items.ToList());
            this.RaiseListChangedEvents = true;
            isSortedValue = true;
            if (String.IsNullOrEmpty(Filter))
                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        /// <summary>
        /// Метод для очистки данных от лишних(не соответствующих фильтру значений)
        /// </summary>
        /// <param name="items"></param>
        private void ResetItems(IEnumerable<T> items)
        {
            this.Clear();
            this.ClearItems();

            foreach (var item in items)
            {
                this.Add(item);
            }
        }

        #endregion Sorting


        #region AdvancedSorting
        public bool SupportsAdvancedSorting
        {
            get { return false; }
        }
        public ListSortDescriptionCollection SortDescriptions
        {
            get { return null; }
        }

        public void ApplySort(ListSortDescriptionCollection sorts)
        {
            throw new NotSupportedException();
        }

        #endregion AdvancedSorting

        #region Filtering

        public bool SupportsFiltering
        {
            get { return true; }
        }

        public void RemoveFilter()
        {
            if (Filter != null) Filter = null;
        }

        private string filterValue = null;

        /// <summary>
        /// Свойство возвращающее или устанавливающее значение фильтра.
        /// </summary>
        public string Filter
        {
            get
            {
                return filterValue;
            }
            set
            {
                if (filterValue == value) return;

                //если пустая строка или не соответсвует формату
                if (!string.IsNullOrEmpty(value) &&
                    !Regex.IsMatch(value,
                    BuildRegExForFilterFormat(), RegexOptions.Singleline))
                    throw new ArgumentException("Filter is not in " +
                          "the format: propName[<>=]'value'.");

                //запрещаем вызывать событие изменения данных в списке, во избежание добавления ненужных данных в список
                RaiseListChangedEvents = false;

                // Пустой фильтр == отмена фильтра
                if (string.IsNullOrEmpty(value))
                    ResetList();
                else
                {
                    int count = 0;
                    string[] matches = value.Split(new string[] { " AND " },
                        StringSplitOptions.RemoveEmptyEntries);

                    while (count < matches.Length)
                    {
                        string filterPart = matches[count].ToString();

                        
                        if (!String.IsNullOrEmpty(filterValue)
                                && !value.Contains(filterValue))
                            ResetList();

                        // Обработать фильтр и применить его
                        SingleFilterInfo filterInfo = ParseFilter(filterPart);
                        ApplyFilter(filterInfo);
                        count++;
                    }
                }
                //Обновим список после применения фильтра
                filterValue = value;
                RaiseListChangedEvents = true;
                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
        }


        /// <summary>
        /// возвращает регулярное выражение для проверки филььтра
        /// </summary>
        /// <returns></returns>
        public static string BuildRegExForFilterFormat()
        {
            StringBuilder regex = new StringBuilder();

            
            regex.Append(@"\[?[\w\s]+\]?\s?");

            //операторы 
            regex.Append(@"[><=]");

            
            regex.Append(@"\s?'?.+'?");

            return regex.ToString();
        }

        /// <summary>
        /// Сбрасывает все изменения в классе до исходных
        /// </summary>
        private void ResetList()
        {
            this.ClearItems();
            foreach (T t in originalListValue)
                this.Items.Add(t);
            if (IsSortedCore)
                ApplySortCore(SortPropertyCore, SortDirectionCore);
        }


        protected override void OnListChanged(ListChangedEventArgs e)
        {
           
            if (e.ListChangedType == ListChangedType.Reset)
            {
                if (Filter == null || Filter == "")
                    AllowNew = true;
                else
                    AllowNew = false;
            }
           
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                OriginalList.Add(this[e.NewIndex]);
                if (!String.IsNullOrEmpty(Filter)) 
                {
                    string cachedFilter = this.Filter;
                    this.Filter = "";
                    this.Filter = cachedFilter;
                }
            }
           if (e.ListChangedType == ListChangedType.ItemDeleted)
                OriginalList.RemoveAt(e.NewIndex);

            base.OnListChanged(e);
        }

        /// <summary>
        /// Применяет фильтр
        /// </summary>
        /// <param name="filterParts"></param>
        internal void ApplyFilter(SingleFilterInfo filterParts)
        {
            List<T> results;

            Type interfaceType;
            PropertyDescriptor filterPropDesc = null;
            if (filterParts.PropName.Contains("."))
            {
                filterPropDesc = TypeDescriptor.GetProperties(typeof(T))[
                    filterParts.PropName.Substring(0, filterParts.PropName.IndexOf('.'))];

                interfaceType =
                    filterPropDesc.GetChildProperties()[
                            filterParts.PropName.Substring(filterParts.PropName.IndexOf('.') + 1)].PropertyType
                        .GetInterface("IComparable");

            }
            else
            {

                //проверим реализует ли фильтруемое значенеи IComparable
                interfaceType =
                    TypeDescriptor.GetProperties(typeof(T))[filterParts.PropName].PropertyType
                        .GetInterface("IComparable");
            }
            if (interfaceType == null)
                throw new InvalidOperationException("Filtered property" +
                                                    " must implement IComparable.");

            results = new List<T>();

            //проверяем каждое значнеи относитлеьно фильтра и добавляем в список
            foreach (T item in this.ToArray())
            {
                if (filterParts.PropName.Contains("."))
                {
                    if (filterPropDesc.GetValue(item) != null)
                    {
                        object obj = filterPropDesc.GetValue(item);
                        if (obj != null)
                        {
                            IComparable compareValue =
                                filterParts.PropDesc.GetValue(obj) as IComparable;
                            int result =
                                compareValue.CompareTo(filterParts.CompareValue);
                            if (filterParts.OperatorValue ==
                                FilterOperator.EqualTo && result == 0)
                                results.Add(item);
                            if (filterParts.OperatorValue ==
                                FilterOperator.GreaterThan && result > 0)
                                results.Add(item);
                            if (filterParts.OperatorValue ==
                                FilterOperator.LessThan && result < 0)
                                results.Add(item);
                        }
                    }
                }
                else
                {
                    if (filterParts.PropDesc.GetValue(item) != null)
                    {
                        IComparable compareValue =
                            filterParts.PropDesc.GetValue(item) as IComparable;
                        int result =
                            compareValue.CompareTo(filterParts.CompareValue);
                        if (filterParts.OperatorValue ==
                            FilterOperator.EqualTo && result == 0)
                            results.Add(item);
                        if (filterParts.OperatorValue ==
                            FilterOperator.GreaterThan && result > 0)
                            results.Add(item);
                        if (filterParts.OperatorValue ==
                            FilterOperator.LessThan && result < 0)
                            results.Add(item);
                    }
                }
            }
            this.ClearItems();
                foreach (T itemFound in results)
                    this.Add(itemFound);
            
        }

        /// <summary>
        /// Считывает фильтр
        /// </summary>
        /// <param name="filterPart"></param>
        /// <returns></returns>
        internal SingleFilterInfo ParseFilter(string filterPart)
        {
            SingleFilterInfo filterInfo = new SingleFilterInfo();
            filterInfo.OperatorValue = DetermineFilterOperator(filterPart);

            string[] filterStringParts =
                filterPart.Split(new char[] { (char)filterInfo.OperatorValue });

            filterInfo.PropName =
                filterStringParts[0].Replace("[", "").
                Replace("]", "").Replace(" AND ", "").Trim();

            PropertyDescriptor filterPropDesc;
            //Get the property descriptor for the filter property name STEP - 1
            if (filterInfo.PropName.Contains("."))
            {
                filterPropDesc = TypeDescriptor.GetProperties(typeof(T))[
                   filterInfo.PropName.Substring(0, filterInfo.PropName.IndexOf('.'))];
                filterPropDesc = filterPropDesc.GetChildProperties()[filterInfo.PropName.Substring(filterInfo.PropName.IndexOf('.') + 1)];

            }
            else
            {
                filterPropDesc =
                   TypeDescriptor.GetProperties(typeof(T))[filterInfo.PropName];
            }

            // Convert the filter compare value to the property type.
            if (filterPropDesc == null)
                throw new InvalidOperationException("Specified property to " +
                    "filter " + filterInfo.PropName +
                    " on does not exist on type: " + typeof(T).Name);

            filterInfo.PropDesc = filterPropDesc;

            string comparePartNoQuotes = StripOffQuotes(filterStringParts[1]);
            try
            {
                TypeConverter converter =
                    TypeDescriptor.GetConverter(filterPropDesc.PropertyType);
                filterInfo.CompareValue =
                    converter.ConvertFromString(comparePartNoQuotes);
            }
            catch (NotSupportedException)
            {
                throw new InvalidOperationException("Specified filter" +
                    "value " + comparePartNoQuotes + " can not be converted" +
                    "from string. Implement a type converter for " +
                    filterPropDesc.PropertyType.ToString());
            }
            return filterInfo;
        }

        /// <summary>
        /// определяет оператор из строки и возвращает его в виде Enum
        /// </summary>
        /// <param name="filterPart"></param>
        /// <returns></returns>
        internal FilterOperator DetermineFilterOperator(string filterPart)
        {
            
            if (Regex.IsMatch(filterPart, "[^>^<]="))
                return FilterOperator.EqualTo;
            else if (Regex.IsMatch(filterPart, "<[^>^=]"))
                return FilterOperator.LessThan;
            else if (Regex.IsMatch(filterPart, "[^<]>[^=]"))
                return FilterOperator.GreaterThan;
            else
                return FilterOperator.None;
        }

        /// <summary>
        /// Избавляется от кавычек в фильтре
        /// </summary>
        /// <param name="filterPart"></param>
        /// <returns></returns>
        internal static string StripOffQuotes(string filterPart)
        {
            // избавляемся от кавычек
            if (Regex.IsMatch(filterPart, "'.+'"))
            {
                int quote = filterPart.IndexOf('\'');
                filterPart = filterPart.Remove(quote, 1);
                quote = filterPart.LastIndexOf('\'');
                filterPart = filterPart.Remove(quote, 1);
                filterPart = filterPart.Trim();
            }
            return filterPart;
        }


        #endregion Filtering
    }

    public struct SingleFilterInfo
    {
        internal string PropName;
        internal PropertyDescriptor PropDesc;
        internal Object CompareValue;
        internal FilterOperator OperatorValue;
    }

    

    /// <summary>
    /// перечисление в котором возможные оператор фильтра
    /// </summary>
    public enum FilterOperator
    {
        EqualTo = '=',
        LessThan = '<',
        GreaterThan = '>',
        None = ' '
    }
}