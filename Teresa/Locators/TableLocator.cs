using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Teresa
{
    public class TableLocator : Locator
    {
        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }
        public List<string> Headers { get; private set; }
        public bool WithHeaders { get; private set; }

        public IWebElement CellOf(int rowIndex, int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= ColumnCount)
                throw new Exception("Column Index ranges from 0 to " + (ColumnCount - 1));
            if (rowIndex < 0 || rowIndex >= RowCount)
                throw new Exception("Row Index ranges from 0 to " + (RowCount - 1));
            if (rowIndex == 0 && !WithHeaders)
                throw new Exception("No headers (rowIndex=0) defined for this table.");

            return FindElement().FindElementByCss(
                string.Format("tr:nth-of-type({0}) {1}:nth-of-type({2})", 
                    rowIndex + 1,
                    rowIndex == 0 ? "th" : "td",
                    columnIndex)
                );
        }

        public IWebElement CellOf(int rowIndex, string headerKeyword)
        {
            int columnIndex = Headers.FindIndex(s => s.Contains(headerKeyword));
            return CellOf(rowIndex, columnIndex);
        }

        public IWebElement CellOf(string headerKeyword, Func<string, bool> predicate)
        {
            int columnIndex = Headers.FindIndex(s => s.Contains(headerKeyword));
            var allRows = FindElement().FindElementsByCss("tr").ToList();
            if (WithHeaders)
            {
                allRows.RemoveAt(0);
            }

            foreach (var row in allRows)
            {
                var td = ((IWebElement)row).FindElementByCss(string.Format("td:nth-of-type({0})", columnIndex + 1));
                if (predicate(td.Text))
                    return row;
            }
            return null;
        }

        public override IWebElement FindElement(Func<IWebElement, bool> filters = null, int waitInMills = DefaultWaitToFindElement)
        {
            IWebElement lastFound = lastFoundElement;
            IWebElement result = base.FindElement(filters, waitInMills);
            if (result == null)
                throw new NoSuchElementException();
            
            if (result != lastFound)
            {
                var rows = result.FindElementsByCss("tr").ToList();
                IWebElement firstRow = rows.FirstOrDefault();
                if (firstRow==null)
                    throw new NoSuchElementException("Failed to found table rows.");
                var headers = firstRow.FindElementsByCss("th").ToList();
                RowCount = rows.Count();
                if (headers.Count() != 0)
                {
                    WithHeaders = true;
                    ColumnCount = headers.Count();
                    Headers = headers.Select(x => x.Text).ToList();
                }
                else
                {
                    RowCount += 1;
                    WithHeaders = false;
                    Headers = null;
                    firstRow = rows.FirstOrDefault();
                    ColumnCount = firstRow == null ? 0 : firstRow.FindElementsByCss("td").Count();
                }
            }
            return result;
        }

        public TableLocator(Enum identifier, Fragment parent)
            : base(identifier, parent)
        {}
    }
}
