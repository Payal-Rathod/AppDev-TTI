using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Threading;
using System.IO;



// ===================================================================
// Very important notes:
// ... To keep everything working smoothly, you should always
//     dispose of EVERY SQLiteCommand even if you recycle a 
//     SQLiteCommand variable later on.
//     EXAMPLE:
//            Database.newDatabase(GetSolutionDir() + "\\" + filename);
//            var cmd = new SQLiteCommand(Database.dbConnection);
//            cmd.CommandText = "INSERT INTO categoryTypes(Description) VALUES('Whatever')";
//            cmd.ExecuteNonQuery();
//            cmd.Dispose();
//
// ... also dispose of reader objects
//
// ... by default, SQLite does not impose Foreign Key Restraints
//     so to add these constraints, connect to SQLite something like this:
//            string cs = $"Data Source=abc.sqlite; Foreign Keys=1;";
//            var con = new SQLiteConnection(cs);
//
// ===================================================================


namespace Budget
{
    public class Database
    {

        public static SQLiteConnection dbConnection { get { return _connection; } }
        private static SQLiteConnection _connection;

        // ===================================================================
        // create and open a new database
        // ===================================================================
        public static void newDatabase(string filename)
        {
            String connection_string = $"Data Source={filename}; Foreign Keys=1;";
            _connection = new SQLiteConnection(connection_string);
            _connection.Open();


            var cmd = new SQLiteCommand(Database.dbConnection);
 
            cmd.CommandText = "DROP TABLE IF EXISTS categoryTypes";
            cmd.ExecuteNonQuery();
            //cmd.Dispose();

            //Creates Categorytypes table
            cmd.CommandText = @"CREATE TABLE categoryTypes(categoryTypeId INTEGER PRIMARY KEY, 
            description TEXT)";
            cmd.ExecuteNonQuery();
            //cmd.Dispose();

            cmd.CommandText = "DROP TABLE IF EXISTS categories";
            cmd.ExecuteNonQuery();
            //cmd.Dispose();

            //Creates Category table
            cmd.CommandText = @"CREATE TABLE categories(categoryId INTEGER PRIMARY KEY,
            description TEXT, TypeId INTEGER, FOREIGN KEY(TypeId) REFERENCES categoryTypes(Id))";

            cmd.ExecuteNonQuery();

            //cmd.Dispose();


            cmd.CommandText = "DROP TABLE IF EXISTS expenses";
            cmd.ExecuteNonQuery();
            //cmd.Dispose();
            //Creates Expense table
            cmd.CommandText = @"CREATE TABLE expenses(expenseId INTEGER PRIMARY KEY, Date TEXT,
            description TEXT, amount DOUBLE, CategoryId INTEGER, FOREIGN KEY(categoryId) REFERENCES categories(categoryId))";
            cmd.ExecuteNonQuery();

            //cmd.CommandText = "INSERT INTO categories(categoryId, description, TypeId) VALUES(1, 'hi', 1)";
            cmd.Dispose();

            // DO NOT FORGET TO BIND PARAMETERS/VALUES OR IT'S A 0.
            // after each commadn, execute3 query and dispose of it to "clean". explanations are given by Sandy in comments ^.

        }

       // ===================================================================
       // open an existing database
       // ===================================================================
       public static void existingDatabase(string filename)
        {

            CloseDatabaseAndReleaseFile();

            // your code
        }

       // ===================================================================
       // close existing database, wait for garbage collector to
       // release the lock before continuing
       // ===================================================================
        static public void CloseDatabaseAndReleaseFile()
        {
            if (Database.dbConnection != null)
            {
                // close the database connection
                Database.dbConnection.Close();
                

                // wait for the garbage collector to remove the
                // lock from the database file
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

     }

}
