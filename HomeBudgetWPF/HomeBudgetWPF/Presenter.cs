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
            view.Refresh();
            view.ShowDatabase("Open a database file in the file tab");
            view.DisableBtnAndInput();
        }
        public void openDatabase(string filename, bool newDb)
        {
            homeBudget = new HomeBudget(filename, "", newDb);
            cats = homeBudget.categories;
            expenses = homeBudget.expenses;
            view.EnableBtnAndInput();
        }

        public void addExpenses(DateTime date, int category, Double amount, String description) 
        {
            expenses.Add(date, category, amount, description);
            view.ShowAdded();
            view.Refresh();
        }
        public void ChangeColorMode(string mode)
        {
            if (mode == "Dark Mode")
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

        public Budget.Category addCategory(String name, Budget.Category.CategoryType type)
        {
            // This only works if you opened a file. 
            // Input validation.
            // Default file's cats if no database file has been selected.
            cats.Add(name, type);
            return cats.List().Last();
        }
    }
}
