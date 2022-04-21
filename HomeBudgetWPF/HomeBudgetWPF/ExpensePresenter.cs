using Budget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudgetWPF
{
    class ExpensePresenter
    {
        private readonly AddExpenseInterface view;
        private HomeBudget homeBudget;
        private Categories cats;
        private Expenses expenses;
        private string filepath;
        private bool newDb;
        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <param name="v">View Interface</param>
        public ExpensePresenter(AddExpenseInterface v, string filename)
        {
            view = v;
            homeBudget = new HomeBudget(filename, "", newDb = false);
            cats = homeBudget.categories;
            expenses = homeBudget.expenses;
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
            //view.ShowAdded(description);
            //view.Refresh();
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
        public void UpdateExpense(Budget.BudgetItem item)
        {
            expenses.UpdateProperties(item.ExpenseID, item.Date, item.ShortDescription, item.Amount, item.CategoryID);
        }
    }
}
