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
            view.Refresh();
            view.ShowDatabase("Open a database file in the file tab");
            view.DisableBtnAndInput();
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
            expenses = homeBudget.expenses;
            
            filepath = filename;
            view.EnableBtnAndInput();
        }
        /// <summary>
        /// Adds an expense from user input on GUI.
        /// </summary>
        /// <param name="date">DateTime value of expense date.</param>
        /// <param name="category">Category ID of expense.</param>
        /// <param name="amount">Amount double value of expense.</param>
        /// <param name="description">String description of expense.</param>
        public void AddExpense(DateTime date, int category, double amount, string description) 
        {
            expenses.Add(date, category, amount, description);
            view.ShowAdded(description);
            view.Refresh();
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
        /// Clears fields.
        /// </summary>
        public void ClearFields()
        {
            view.CancelExpense();
        }
        /// <summary>
        /// Gets updated categories list from database.
        /// </summary>
        /// <returns></returns>
        public List<Budget.Category> getCategoriesList()
        {
            return cats.List();
        }
        public List<Budget.BudgetItem> GetBudgetItemsList()
        {
            return homeBudget.GetBudgetItems(DateTime.MinValue, DateTime.MaxValue, false, -1);
        }
        /// <summary>
        /// Adds a category to database.
        /// </summary>
        /// <param name="name">String name of category.</param>
        /// <param name="type">Type of category.</param>
        /// <returns>Last category of updated list of categories, which is the ID.</returns>
        public Budget.Category AddCategory(String name, Budget.Category.CategoryType type)
        {
            cats.Add(name, type);
            return cats.List().Last();
        }

        public void DeleteExpense(int expenseId)
        {
            expenses.Delete(expenseId);
        }
    }
}