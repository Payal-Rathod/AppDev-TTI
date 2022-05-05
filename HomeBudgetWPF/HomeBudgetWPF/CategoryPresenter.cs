using Budget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudgetWPF
{
    class CategoryPresenter
    {
        private readonly AddCategoryInterface view;
        private HomeBudget homeBudget;
        private Categories cats;
        private Expenses expenses;
        private string filepath;
        private bool newDb;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        /// <param name="v">View Interface</param>
        public CategoryPresenter(AddCategoryInterface v, string filename)
        {
            view = v;
            filepath = filename;
        }

        /// <summary>
        /// Opens database connection
        /// </summary>
        /// <param name="filename"></param>
        public void openDatabase(string filename)
        {
            homeBudget = new HomeBudget(filename, "", newDb = false);
            cats = homeBudget.categories;
            expenses = homeBudget.expenses;
        }

        /// <summary>
        /// Gets updated categories list from database.
        /// </summary>
        /// <returns>The category list of homebudget</returns>
        public List<Budget.Category> getCategoriesList()
        {
            openDatabase(filepath);
            cats = homeBudget.categories;
            return cats.List();
        }

        /// <summary>
        /// Adds a category to database.
        /// </summary>
        /// <param name="name">String name of category.</param>
        /// <param name="type">Type of category.</param>
        /// <returns>Last category of updated list of categories, which is the ID.</returns>
        public Budget.Category AddCategory(String name, Budget.Category.CategoryType type)
        {
            //openDatabase(filepath);
            cats.Add(name, type);
            return cats.List().Last();
        }
    }
}
