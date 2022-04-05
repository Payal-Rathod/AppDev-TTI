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
        private readonly ViewInterface view;
        private HomeBudget homeBudget;
        private Categories cats;
        private Expenses expenses;
        public Presenter(ViewInterface v)
        {
            view = v;
        }
        public void openDatabase(string filename, bool newDb)
        {
            homeBudget = new HomeBudget(filename, "", newDb);
            cats = homeBudget.categories;
            expenses = homeBudget.expenses;
        }

        public void addExpenses(DateTime date, string category, Double amount, String description) {
            // Get id of category?
            // Not sure how to get the connection or if we have to query.
            var cmd = new SQLiteCommand();

            cmd.CommandText = "SELECT Id from categories WHERE Description = @Description";
            cmd.Parameters.AddWithValue("@Description", description);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            expenses.List();
            var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                expenses.Add(date, rdr.GetInt32(0), amount, description);
            }

            cmd.Dispose();
                
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
