using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using Budget;

namespace HomeBudgetWPF
{
    public class Presenter
    {
        // We should use view.
        private readonly ViewInterface view;
        private HomeBudget homeBudget;
        private Categories cats;
        private Expenses expenses;
        private string filepath;
        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <param name="v">View Interface</param>
        public Presenter(ViewInterface v)
        {
            view = v;
            NewDatabase();
        }

        /// <summary>
        /// Logic for finding a database file.
        /// </summary>
        public void NewDatabase()
        {
            //view.Refresh();
            view.ShowDatabase("Open a database file in the file tab");
            //view.DisableBtnAndInput();
        }
        /// <summary>
        /// Opens a database file and creates categories and expenses from it.
        /// </summary>
        /// <param name="filename">String name of file path.</param>
        /// <param name="newDb">Boolean connection to database.</param>
        public void OpenDatabase(string filename, bool newDb)
        {
            homeBudget = new HomeBudget(filename, "", newDb);
            cats = homeBudget.categories;
            
            foreach(Expense exp in homeBudget.expenses.List())
            {
                if (exp.Category == 2)
                    continue;
                exp.Amount *= -1;
            }
            expenses = homeBudget.expenses;

            filepath = filename;
            //view.EnableBtnAndInput();
        }

        /// <summary>
        /// Changes color modes from Dark to Light and vice-versa to accomodate user's needs.
        /// </summary>
        /// <param name="colorMode">String value of button's content - Light or dark.</param>
        public void ChangeColorMode(string colorMode)
        {
            if (colorMode == "Dark Mode")
                view.DarkMode();
            else
                view.LightMode();
        }
        public List<Budget.BudgetItem> GetBudgetItemsList(DateTime? startDate, DateTime? endDate, bool filterFlag, int categoryId)
        {
            OpenDatabase(filepath, false);

            if (startDate == null)
            {
                startDate = DateTime.MinValue;
            }
            if(endDate == null)
            {
                endDate = DateTime.MaxValue;
            }
            List<Budget.BudgetItem> items = homeBudget.GetBudgetItems(startDate, endDate, filterFlag, categoryId);
            foreach(BudgetItem item in items)
            {
                if (item.CategoryID == 8 || item.CategoryID == 15)
                    continue;
                item.Amount *= -1;
            }
            return items;
        }

        public List<Budget.BudgetItemsByMonth> GetBudgetItemsListByMonth(DateTime? startDate, DateTime? endDate, bool filterFlag, int categoryId)
        {
            OpenDatabase(filepath, false);

            if (startDate == null)
            {
                startDate = DateTime.MinValue;
            }
            if (endDate == null)
            {
                endDate = DateTime.MaxValue;
            }
            List<Budget.BudgetItemsByMonth> items = homeBudget.GetBudgetItemsByMonth(startDate, endDate, filterFlag, categoryId);
            return items;
        }

        public List<Budget.BudgetItemsByCategory> GetBudgetItemsListByCategory(DateTime? startDate, DateTime? endDate, bool filterFlag, int categoryId)
        {
            OpenDatabase(filepath, false);

            if (startDate == null)
            {
                startDate = DateTime.MinValue;
            }
            if (endDate == null)
            {
                endDate = DateTime.MaxValue;
            }
            List<Budget.BudgetItemsByCategory> items = homeBudget.GeBudgetItemsByCategory(startDate, endDate, filterFlag, categoryId);
            return items;
        }

        public List<Dictionary<string,object>> GetBudgetItemsListByMonthAndCategory(DateTime? startDate, DateTime? endDate, bool filterFlag, int categoryId)
        {
            OpenDatabase(filepath, false);
            if (startDate == null)
            {
                startDate = DateTime.MinValue;
            }
            if (endDate == null)
            {
                endDate = DateTime.MaxValue;
            }
            List<Dictionary<string, object>> items = homeBudget.GetBudgetDictionaryByCategoryAndMonth(startDate, endDate, filterFlag, categoryId);
            return items;
        }
        public void DeleteExpense(int expenseId)
        {
            OpenDatabase(filepath, false);
            expenses = homeBudget.expenses;
            expenses.Delete(expenseId);
        }

        /// <summary>
        /// Gets updated categories list from database.
        /// </summary>
        /// <returns></returns>
        public List<Budget.Category> getCategoriesList()
        {
            cats = homeBudget.categories;
            return cats.List();
        }
    }
}