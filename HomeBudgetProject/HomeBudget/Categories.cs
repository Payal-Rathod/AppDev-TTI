using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Data.SQLite;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Budget
{
    // ====================================================================
    // CLASS: categories
    //        - A collection of category items,
    //        - Read / write to file
    //        - etc
    // ====================================================================

    /// <summary>
    /// Categories class is used to manage a list of Category objects.
    /// </summary>
    public class Categories
    {
        private SQLiteConnection db;
        private int counterId = 0;

        // ====================================================================
        // Constructor
        // ====================================================================

        /// <summary>
        /// Default constructor that calls the <see cref="SetCategoriesToDefaults"/> method to add categories to a list.
        /// </summary>
        public Categories()
        {
            SetCategoriesToDefaults();
        }


        /// <summary>
        /// Constructor that connects to a database (new or existing) and sets categories to default if it is a new database.
        /// </summary>
        /// <param name="con">Connection of the database</param>
        /// <param name="newDb">If it is a new database</param>
        public Categories(SQLiteConnection con, bool newDb)
        {
            db = con;

            if (newDb)
            {
                var cmd = new SQLiteCommand(db);

                cmd.CommandText = "INSERT INTO categoryTypes(Id, Description) VALUES (@Id, @Description)";
                cmd.Parameters.AddWithValue("@Id", (int)Category.CategoryType.Expense + 1);
                cmd.Parameters.AddWithValue("@Description", "Expense");
                cmd.Prepare();
                cmd.ExecuteNonQuery();


                cmd.CommandText = "INSERT INTO categoryTypes(Id, Description) VALUES (@Id, @Description)";
                cmd.Parameters.AddWithValue("@Id", (int)Category.CategoryType.Savings + 1);
                cmd.Parameters.AddWithValue("@Description", "Savings");
                cmd.Prepare();
                cmd.ExecuteNonQuery();


                cmd.CommandText = "INSERT INTO categoryTypes(Id, Description) VALUES (@Id, @Description)";
                cmd.Parameters.AddWithValue("@Id", (int)Category.CategoryType.Income + 1);
                cmd.Parameters.AddWithValue("@Description", "Income");
                cmd.Prepare();
                cmd.ExecuteNonQuery();


                cmd.CommandText = "INSERT INTO categoryTypes(Id, Description) VALUES (@Id, @Description)";
                cmd.Parameters.AddWithValue("@Id", (int)Category.CategoryType.Credit + 1);
                cmd.Parameters.AddWithValue("@Description", "Credit");
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                SetCategoriesToDefaults();
            }
            else
            {
                var cmd = new SQLiteCommand("Select MAX(Id) from categories", db);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.Dispose();
                counterId = count+1;
            } 

        }
        // ====================================================================
        // get a specific category from the list where the id is the one specified
        // ====================================================================

        /// <summary>
        /// Searches for a category of a specific ID and returns the category of that id. This method is used to get info about a category according to its ID and 
        /// this is possible by searching for an element that matches the ID and returns the first occurrence of the match. Before calling this method, it is 
        /// necessary to already have a category list from which the id must match. 
        /// </summary>
        /// <param name="i">The number of the ID of the category to find</param>
        /// <returns>The first occurence of a matching ID number</returns>
        /// <exception cref="Exception">Thrown when the ID number given in the parameter cannot be found from the list of categories</exception>
        public Category GetCategoryFromId(int i)
        {
            var cmd = new SQLiteCommand(db);

            cmd.CommandText = "SELECT Id, Description, TypeId from categories where Id = @Id";
            cmd.Parameters.AddWithValue("@Id", i);
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            var rdr = cmd.ExecuteReader();
            int id = 0, type = 0;
            String descr = "";
            while (rdr.Read())
            {
                id = rdr.GetInt32(0);
                descr = rdr.GetString(1);
                type = rdr.GetInt32(2);
            }
            Category c = new Category(id, descr, (Category.CategoryType)type - 1);

            cmd.Dispose();

            return c;
        }


        // ====================================================================
        // set categories to default
        // ====================================================================

        /// <summary>
        /// Sets the Category list to default by adding some Category objects to it. This is used to populate the Category list and it is possible to do so
        /// by adding each object to the list with the method <see cref="Add"/>. 
        /// </summary>
        /// <example>
        /// The example shown below shows the usage of this method: 
        /// 
        /// <code>
        /// <![CDATA[
        /// Categories c = new Categories();
        /// 
        /// //Loops through the list of Category that was populated by the constructor by default. 
        /// //Also outputs the description of each category to the console.
        /// foreach (Category category in c.List()){
        ///     Console.WriteLine(category.description);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public void SetCategoriesToDefaults()
        {
            try
            {
                counterId = 0;
                // ---------------------------------------------------------------
                // reset any current categories,
                // ---------------------------------------------------------------
                var cmd = new SQLiteCommand(db);

                cmd.CommandText = "DELETE FROM categories";
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                // ---------------------------------------------------------------
                // Add Defaults
                // ---------------------------------------------------------------
                Add("Utilities", Category.CategoryType.Expense);
                Add("Rent", Category.CategoryType.Expense);
                Add("Food", Category.CategoryType.Expense);
                Add("Entertainment", Category.CategoryType.Expense);
                Add("Education", Category.CategoryType.Expense);
                Add("Miscellaneous", Category.CategoryType.Expense);
                Add("Medical Expenses", Category.CategoryType.Expense);
                Add("Vacation", Category.CategoryType.Expense);
                Add("Credit Card", Category.CategoryType.Credit);
                Add("Clothes", Category.CategoryType.Expense);
                Add("Gifts", Category.CategoryType.Expense);
                Add("Insurance", Category.CategoryType.Expense);
                Add("Transportation", Category.CategoryType.Expense);
                Add("Eating Out", Category.CategoryType.Expense);
                Add("Savings", Category.CategoryType.Savings);
                Add("Income", Category.CategoryType.Income);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        // ====================================================================
        // Add category
        // ====================================================================

        /// <summary>
        /// Adds a new Category in the list of Category. The id number is generated by default.
        /// </summary>
        /// <param name="desc">The description of the category.</param>
        /// <param name="type">The type of the Category</param>
        ///         ///<example>
        /// The example shown below shows the usage of this method: 
        /// 
        /// <code>
        /// <![CDATA[
        /// Categories c = new Categories();
        /// 
        /// //Adds a new Category object to the list of Category.
        /// c.Add("Car", Category.CategoryType.Expense);
        /// 
        /// //Loops through the list of Category that was populated by the constructor and that had a category added to it. 
        /// //Also outputs the description of each category to the console.
        /// foreach (Category category in c.List()){
        ///     Console.WriteLine(category.description);
        /// }
        /// 
        /// ]]>
        /// </code>
        /// </example>
        public void Add(String desc, Category.CategoryType type)
        {

            var cmd = new SQLiteCommand(db);


            cmd.CommandText = "INSERT INTO categories(Id, Description, TypeId) VALUES (@Id, @Description, @TypeId)";
           
            cmd.Parameters.AddWithValue("@Id", counterId++);
            cmd.Parameters.AddWithValue("@Description", desc);
            cmd.Parameters.AddWithValue("@TypeId", (int) type + 1);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        // ====================================================================
        // Delete category
        // ====================================================================

        /// <summary>
        /// Deletes the Category that matches the id given in the parameter.
        /// </summary>
        /// <param name="Id">The id number of the category that needs to be deleted.</param>
        ///<example>
        /// The example shown below shows the usage of this method: 
        /// 
        /// <code>
        /// <![CDATA[
        /// Categories c = new Categories();
        /// 
        /// //Removes a Category object from the list of Category.
        /// c.Delete(1);
        /// 
        /// //Loops through the list of Category that was populated by the constructor by default and that had a category removed from it.
        /// //Also outputs the description of each category to the console.
        /// foreach (Category category in c.List()){
        ///     Console.WriteLine(category.description);
        /// }
        /// 
        /// ]]>
        /// </code>
        /// </example>
        public void Delete(int Id)
        {
            if (Id < counterId)
            {
                var cmd = new SQLiteCommand(db);

                cmd.CommandText = "DELETE FROM categories WHERE Id = @Id";
                cmd.Parameters.AddWithValue("@Id", Id);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }

        }

        // ====================================================================
        // Return list of categories
        // Note:  make new copy of list, so user cannot modify what is part of
        //        this instance
        // ====================================================================

        /// <summary>
        /// Returns a list of Categories that contains Category objects.
        /// </summary>
        /// <returns>A list of Categories.</returns>
        ///<example>
        /// The example shown below shows the usage of this method: 
        /// 
        /// <code>
        /// <![CDATA[
        /// Categories c = new Categories();
        /// 
        /// //Loops through the list of Category that was populated by the constructor and outputs the description of each category to the console.
        /// foreach (Category category in c.List()){
        ///     Console.WriteLine(category.description);
        /// }
        /// 
        /// ]]>
        /// </code>
        /// </example>
        public List<Category> List()
        {
            List<Category> newList = new List<Category>();
            var cmd = new SQLiteCommand(db);


            cmd.CommandText = "Select Id, Description, TypeId from categories";
            var rdr = cmd.ExecuteReader();

            // loop
            while (rdr.Read())
            {
                int id = rdr.GetInt32(0);
                string descr = rdr.GetString(1);
                int type = rdr.GetInt32(2);
                newList.Add(new Category(id, descr, (Category.CategoryType)(type - 1)));

            }
            
            cmd.Dispose();

            return newList;
        }
       
        /// <summary>
        /// Updates a category item given in the parameter
        /// </summary>
        /// <param name="id">Id of the category to update</param>
        /// <param name="desc">New decription of the category</param>
        /// <param name="type">New type of the category</param>
        public void UpdateProperties(int id, string desc, Category.CategoryType type)
        {
            var cmd = new SQLiteCommand(db);

            cmd.CommandText = "SELECT Id, Description, TypeId from categories where Id = @Id";
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            cmd.CommandText = "UPDATE categories SET Description = @Description, TypeId = @TypeId WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Description", desc);
            cmd.Parameters.AddWithValue("@TypeId", (int) type + 1);
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            cmd.Dispose();
        }

    }
}

