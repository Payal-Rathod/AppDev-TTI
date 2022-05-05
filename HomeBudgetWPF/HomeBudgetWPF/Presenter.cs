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
        private static HomeBudget homeBudget;
        private static Categories cats;
        private static Expenses expenses;
        private static string filepath;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <param name="v">View Interface</param>
        public Presenter(ViewInterface v)
        {
            view = v;
            NewDatabase();
        }
        public List<object> DataSource { get; set; }

        /// <summary>
        /// Logic for finding a database file.
        /// </summary>
        public void NewDatabase()
        {
            view.ShowDatabase("Open a database file in the file tab");
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
        }

        /// <summary>
        /// Opens existing file in the new application
        /// </summary>
        public void OpenFile()
        {
            view.OpenFile();
        }

        /// <summary>
        /// Opens new file in the application
        /// </summary>
        public void NewFile()
        {
            view.NewFile();
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

        /// <summary>
        /// Gets budget items list from homebudget and returns it
        /// </summary>
        /// <param name="startDate">The start date of the items.</param>
        /// <param name="endDate">The end date of the items</param>
        /// <param name="filterFlag">The filter flag for showing only one category</param>
        /// <param name="categoryId">The category id for filter flag</param>
        /// <returns>List of budget items</returns>
        public List<Budget.BudgetItem> GetBudgetItemsList(DateTime? startDate, DateTime? endDate, bool filterFlag, int categoryId)
        {
            OpenDatabase(filepath, false);

            view.InitializeDataGrid();

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

        /// <summary>
        /// Gets budget items list by month from homebudget and returns it
        /// </summary>
        /// <param name="startDate">The start date of the items.</param>
        /// <param name="endDate">The end date of the items</param>
        /// <param name="filterFlag">The filter flag for showing only one category</param>
        /// <param name="categoryId">The category id for filter flag</param>
        /// <returns>List of budget items by month</returns>
        public List<Budget.BudgetItemsByMonth> GetBudgetItemsListByMonth(DateTime? startDate, DateTime? endDate, bool filterFlag, int categoryId)
        {
            OpenDatabase(filepath, false);

            view.InitializeDataGridByMonth();

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

        /// <summary>
        /// Gets budget items list by category from homebudget and returns it
        /// </summary>
        /// <param name="startDate">The start date of the items.</param>
        /// <param name="endDate">The end date of the items</param>
        /// <param name="filterFlag">The filter flag for showing only one category</param>
        /// <param name="categoryId">The category id for filter flag</param>
        /// <returns>List of budget items by category</returns>
        public List<Budget.BudgetItemsByCategory> GetBudgetItemsListByCategory(DateTime? startDate, DateTime? endDate, bool filterFlag, int categoryId)
        {
            OpenDatabase(filepath, false);

            view.InitializeDataGridByCategory();

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

        /// <summary>
        /// Gets budget items list by category and month from homebudget and returns it
        /// </summary>
        /// <param name="startDate">The start date of the items.</param>
        /// <param name="endDate">The end date of the items</param>
        /// <param name="filterFlag">The filter flag for showing only one category</param>
        /// <param name="categoryId">The category id for filter flag</param>
        /// <returns>List of budget items by category and month</returns>
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

            view.InitializeDataGridByMonthAndCategory(items);

            return items;
        }

        /// <summary>
        /// Deletes an expense with a given id
        /// </summary>
        /// <param name="expenseId">Id of the expense to delete</param>
        public void DeleteExpense(int expenseId)
        {
            OpenDatabase(filepath, false);
            expenses = homeBudget.expenses;
            expenses.Delete(expenseId);
        }

        /// <summary>
        /// Gets updated categories list from database.
        /// </summary>
        /// <returns>List of categpries of homebudget</returns>
        public List<Budget.Category> getCategoriesList()
        {
            OpenDatabase(filepath, false);
            cats = homeBudget.categories;
            return cats.List();
        }

        public List<String> getCategoriesListInString()
        {
            List<String> catsName = new List<String>();
            OpenDatabase(filepath, false);
            cats = homeBudget.categories;
            foreach(Budget.Category cat in cats.List())
            {
                catsName.Add(cat.Description);
            }

            return catsName;
        }
    }
}