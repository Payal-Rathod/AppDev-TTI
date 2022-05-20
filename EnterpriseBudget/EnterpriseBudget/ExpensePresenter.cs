using Budget;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseBudget
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
            filepath = filename;
            view.Refresh();
            openDatabase(filename);
        }

        public void openDatabase(string filename)
        {
            homeBudget = new HomeBudget(filename, newDb = false);
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
            openDatabase(filepath);
            expenses.Add(date, category+1, amount, description);
            view.ShowAdded(description);
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
        /// Gets budget items list in homebudget and returns the list.
        /// </summary>
        /// <returns>The list of budget items</returns>
        public List<Budget.BudgetItem> GetBudgetItemsList()
        {
            List<Budget.BudgetItem> items = homeBudget.GetBudgetItems(DateTime.MinValue, DateTime.MaxValue, false, -1);
            /*foreach (BudgetItem item in items)
            {
                if (item.CategoryID == 8 || item.CategoryID == 15)
                    continue;
                item.Amount *= -1;
            }*/
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

        /// <summary>
        /// Deletes an expense with a given id.
        /// </summary>
        /// <param name="expenseId">Id of the expense to delete</param>
        public void DeleteExpense(int expenseId)
        {
            openDatabase(filepath);
            expenses.Delete(expenseId);
        }

        public double GetLimit(int catId)
        {
            //Employees table
            SqlCommand verifyUser = Model.Connection.cnn.CreateCommand();

            verifyUser.CommandText = "SELECT limit FROM budgetCategoryLimits WHERE catId = @catId and deptId = @deptId";
            verifyUser.Parameters.AddWithValue("@catId", catId+1);
            verifyUser.Parameters.AddWithValue("@deptId", 1);

            var rdr = verifyUser.ExecuteReader();

            double limit = 0;

            if (rdr.HasRows)
            {

                while (rdr.Read())
                {
                    limit = rdr.GetDouble(0);
                }
            }

            verifyUser.Dispose();
            rdr.Close();

            return limit;
        }
    }
}