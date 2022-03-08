﻿using System;
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
    // CLASS: expenses
    //        - A collection of expense items,
    //        - Read / write to file
    //        - etc
    // ====================================================================

    /// <summary>
    /// Expenses class that is used to manage a list of Expense objects.
    /// </summary>
    public class Expenses
    {
        private int counterId = 1;
    
        private SQLiteConnection db;

        public Expenses(SQLiteConnection con)
        {
            db = con;

            var cmd = new SQLiteCommand(db);
            cmd.CommandText = "Select Id, Date, Description, Amount, CategoryId from expenses";
            var checkDB = cmd.ExecuteScalar();
            if (checkDB != null)
            {
                cmd = new SQLiteCommand("Select MAX(Id) from expenses", db);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.Dispose();
                counterId = count+1;
            }
        }


        // ====================================================================
        // Add expense
        // ====================================================================

        /// <summary>
        /// Adds a new Expense object in the list of Expense. The id number is generated by default.
        /// </summary>
        /// <param name="date">The date of the expense.</param>
        /// <param name="category">The category of the expense.</param>
        /// <param name="amount">The amount of the expense.</param>
        /// <param name="description">The description of the expense.</param>
        ///<example>
        /// The example shown below shows the usage of this method: 
        /// 
        /// <code>
        /// <![CDATA[
        /// Expenses e = new Expenses();
        /// 
        /// //Adds a new Expense object to the list of Expense.
        /// e.Add(1/10/2018 12:00:00 AM, 10, 10, "hat (on credit)");
        /// 
        /// //Loops through the list of Expense that was populated by the constructor and that had a expense added to it. 
        /// //Also outputs the description of each expense to the console.
        /// foreach (Expense expense in e.List()){
        ///     Console.WriteLine(enxpense.description);
        /// }
        /// 
        /// ]]>
        /// </code>
        /// </example>
        public void Add(DateTime date, int category, Double amount, String description)
        {
            var cmd = new SQLiteCommand(db);


            cmd.CommandText = "INSERT INTO expenses(Id, Date, Description, Amount, CategoryId) VALUES (@Id, @Date, @Description, @Amount, @CategoryId)";

            cmd.Parameters.AddWithValue("@Id", counterId++);
            cmd.Parameters.AddWithValue("@Date", date);
            cmd.Parameters.AddWithValue("@Description", description);
            cmd.Parameters.AddWithValue("@Amount", amount);
            cmd.Parameters.AddWithValue("@CategoryId", category);

            cmd.Prepare();
            cmd.ExecuteNonQuery();

        }

        // ====================================================================
        // Delete expense
        // ====================================================================

        /// <summary>
        /// Deletes the Expense that matches the id given in the parameter.
        /// </summary>
        /// <param name="Id">The id number of the expense that needs to be deleted.</param>
        ///<example>
        /// The example shown below shows the usage of this method: 
        /// 
        /// <code>
        /// <![CDATA[
        /// Expenses e = new Expenses();
        /// 
        /// //Removes a Category object from the list of Category.
        /// e.Delete(1);
        /// 
        /// //Loops through the list of Expense that was populated by the constructor by default and that had a expense removed from it.
        /// //Also outputs the description of each expense to the console.
        /// foreach (Expense expense in e.List()){
        ///     Console.WriteLine(expense.description);
        /// }
        /// 
        /// ]]>
        /// </code>
        /// </example>
        public void Delete(int Id)
        {
            var cmd = new SQLiteCommand(db);
            cmd.CommandText = "Select Id from expenses where Id = @Id";
            cmd.Parameters.AddWithValue("@Id", Id);
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            var checkDB = cmd.ExecuteScalar();

            if (checkDB != null)
            {
                cmd = new SQLiteCommand(db);

                cmd.CommandText = "DELETE FROM expenses WHERE Id = @Id";
                cmd.Parameters.AddWithValue("@Id", Id);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }

        }

        // ====================================================================
        // Return list of expenses
        // Note:  make new copy of list, so user cannot modify what is part of
        //        this instance
        // ====================================================================

        /// <summary>
        /// Returns a list of Expenses that contains Expense objects.
        /// </summary>
        /// <returns>A list of Expense Objects.</returns>
        ///<example>
        /// The example shown below shows the usage of this method: 
        /// 
        /// <code>
        /// <![CDATA[
        /// Expenses e = new Expenses();
        /// 
        /// //Loops through the list of Expense that was populated by the constructor and outputs the description of each expense to the console.
        /// foreach (Expense expense in e.List()){
        ///     Console.WriteLine(expense.description);
        /// }
        /// 
        /// ]]>
        /// </code>
        /// </example>
        public List<Expense> List()
        {
            List<Expense> newList = new List<Expense>();
            var cmd = new SQLiteCommand(db);


            cmd.CommandText = "Select Id, Date, Description, Amount, CategoryId from expenses";
            var rdr = cmd.ExecuteReader();

            // loop
            while (rdr.Read())
            {
                int id = rdr.GetInt32(0);
                DateTime date = rdr.GetDateTime(1);
                string descr = rdr.GetString(2);
                double amount = rdr.GetDouble(3);
                int categoryId = rdr.GetInt32(4);
                newList.Add(new Expense(id, date, categoryId, amount, descr));
            }

            cmd.Dispose();

            return newList;
        }
    }
}

