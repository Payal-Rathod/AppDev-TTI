using Budget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudgetWPF
{
    class UpdatePresenter
    {
        private readonly AddExpenseInterface view;
        private HomeBudget homeBudget;
        private Categories cats;
        private Expenses expenses;
        private string filepath;
        bool newDb;

        /// <summary>
        /// Gets interface and filename value.
        /// </summary>
        /// <param name="v">Interface for update expense</param>
        /// <param name="filename">filename of the database</param>
        public UpdatePresenter(AddExpenseInterface v, string filename)
        {
            view = v;
            homeBudget = new HomeBudget(filename, "", newDb = false);
            filepath = filename;
            openDatabase(filename);
        }

        /// <summary>
        /// Opens new database connecton 
        /// </summary>
        /// <param name="filename">Filepath to database file</param>
        public void openDatabase(string filename)
        {
            homeBudget = new HomeBudget(filename, "", newDb = false);
            cats = homeBudget.categories;
            expenses = homeBudget.expenses;
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
        /// <returns>Categories list of homebudget</returns>
        public List<Budget.Category> getCategoriesList()
        {
            openDatabase(filepath);
            return cats.List();
        }

        /// <summary>
        /// Gets budget items list.
        /// </summary>
        /// <returns>list of budget items</returns>
        public List<Budget.BudgetItem> GetBudgetItemsList()
        {
            openDatabase(filepath);
            List<Budget.BudgetItem> items = homeBudget.GetBudgetItems(DateTime.MinValue, DateTime.MaxValue, false, -1);
            foreach (BudgetItem item in items)
            {
                if (item.CategoryID == 8 || item.CategoryID == 15)
                    continue;
                item.Amount *= -1;
            }
            return items;
        }

        /// <summary>
        /// Updates expense item given.
        /// </summary>
        /// <param name="item">The expense item that needs to be updated</param>
        public void UpdateExpense(Budget.BudgetItem item)
        {
            openDatabase(filepath);
            expenses.UpdateProperties(item.ExpenseID, item.Date, item.ShortDescription, item.Amount, item.CategoryID);
            view.ShowAdded(item.ShortDescription);
        }
    }
}
