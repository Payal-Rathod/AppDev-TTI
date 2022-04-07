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
        public Presenter(ViewInterface v)
        {
            view = v;
            NewDatabase();
        }
        public void NewDatabase()
        {
            view.Refresh();
            view.ShowDatabase("Open a database file in the file tab");
            view.DisableBtnAndInput();
        }
        public void OpenDatabase(string filename, bool newDb)
        {
            homeBudget = new HomeBudget(filename, "", newDb);
            cats = homeBudget.categories;
            expenses = homeBudget.expenses;
            view.EnableBtnAndInput();
        }

        public void AddExpense(DateTime date, int category, double amount, string description) 
        {
            expenses.Add(date, category, amount, description);
            view.ShowAdded(description);
            view.Refresh();
        }
        public void ChangeColorMode(string colorMode)
        {
            if (colorMode == "Dark Mode")
                view.DarkMode();
            else
                view.LightMode();
        }
        public void ClearFields()
        {
            view.Cancel();
        }

        public List<Budget.Category> getCategoriesList()
        {
            return cats.List();
        }

        public Budget.Category AddCategory(String name, Budget.Category.CategoryType type)
        {
            cats.Add(name, type);
            return cats.List().Last();
        }
    }
}
