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

        public string Filter
        {
            get
            {
                return filterValue;
            }
            set
            {
                if (filterValue == value) return;

                // If the value is not null or empty, but doesn't
                // match expected format, throw an exception.
                if (!string.IsNullOrEmpty(value) &&
                    !Regex.IsMatch(value,
                    BuildRegExForFilterFormat(), RegexOptions.Singleline))
                    throw new ArgumentException("Filter is not in " +
                          "the format: propName[<>=]'value'.");

                //Turn off list-changed events.
                RaiseListChangedEvents = false;

                // If the value is null or empty, reset list.
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

                        // Check to see if the filter was set previously.
                        // Also, check if current filter is a subset of 
                        // the previous filter.
                        if (!String.IsNullOrEmpty(filterValue)
                                && !value.Contains(filterValue))
                            ResetList();

                        // Parse and apply the filter.
                        SingleFilterInfo filterInfo = ParseFilter(filterPart);
                        ApplyFilter(filterInfo);
                        count++;
                    }
                }
                // Set the filter value and turn on list changed events.
                filterValue = value;
                RaiseListChangedEvents = true;
                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
        }


        // Build a regular expression to determine if 
        // filter is in correct format.
        public static string BuildRegExForFilterFormat()
        {
            StringBuilder regex = new StringBuilder();

            // Look for optional literal brackets, 
            // followed by word characters or space.
            regex.Append(@"\[?[\w\s]+\]?\s?");

            // Add the operators: > < or =.
            regex.Append(@"[><=]");

            //Add optional space followed by optional quote and
            // any character followed by the optional quote.
            regex.Append(@"\s?'?.+'?");

            return regex.ToString();
        }

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
            // If the list is reset, check for a filter. If a filter 
            // is applied don't allow items to be added to the list.
            if (e.ListChangedType == ListChangedType.Reset)
            {
                if (Filter == null || Filter == "")
                    AllowNew = true;
                else
                    AllowNew = false;
            }
            // Add the new item to the original list.
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                OriginalList.Add(this[e.NewIndex]);
                if (!String.IsNullOrEmpty(Filter))
                //if (Filter == null || Filter == "")
                {
                    string cachedFilter = this.Filter;
                    this.Filter = "";
                    this.Filter = cachedFilter;
                }
            }
            // Remove the new item from the original list.
            if (e.ListChangedType == ListChangedType.ItemDeleted)
                OriginalList.RemoveAt(e.NewIndex);

            base.OnListChanged(e);
        }


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

                // Check to see if the property type we are filtering by implements
                // the IComparable interface.
                interfaceType =
                    TypeDescriptor.GetProperties(typeof(T))[filterParts.PropName].PropertyType
                        .GetInterface("IComparable");
            }
            if (interfaceType == null)
                throw new InvalidOperationException("Filtered property" +
                                                    " must implement IComparable.");

            results = new List<T>();

            // Check each value and add to the results list.
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

        internal FilterOperator DetermineFilterOperator(string filterPart)
        {
            // Determine the filter's operator.
            if (Regex.IsMatch(filterPart, "[^>^<]="))
                return FilterOperator.EqualTo;
            else if (Regex.IsMatch(filterPart, "<[^>^=]"))
                return FilterOperator.LessThan;
            else if (Regex.IsMatch(filterPart, "[^<]>[^=]"))
                return FilterOperator.GreaterThan;
            else
                return FilterOperator.None;
        }

        internal static string StripOffQuotes(string filterPart)
        {
            // Strip off quotes in compare value if they are present.
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

    

    // Enum to hold filter operators. The chars 
    // are converted to their integer values.
    public enum FilterOperator
    {
        EqualTo = '=',
        LessThan = '<',
        GreaterThan = '>',
        None = ' '
    }
}