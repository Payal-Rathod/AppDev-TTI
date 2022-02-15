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
//            string cs = $"Data Source=abc.sqlite; Foreign Keys=1";
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

            // If there was a database open before, close it and release the lock
            CloseDatabaseAndReleaseFile();

            // your code
            Database.newDatabase(GetSolutionDir() + "\\" + filename);
            var cmd = new SQLiteCommand(Database.dbConnection);

            //Creates Categorytypes table
            cmd.CommandText = @"CREATE TABLE categoryTypes(categoryTypeId INTEGER PRIMARY KEY, 
            description TEXT)";

            //Creates Category table
            cmd.CommandText = @"CREATE TABLE Category(categoryId INTEGER PRIMARY KEY,
            description TEXT, TypeId INTEGER FOREIGN KEY)";

            //Creates Expense table
            cmd.CommandText = @"CREATE TABLE Expense(expenseId INTEGER PRIMARY KEY,  Date TEXT,
            description TEXT, amount DOUBLE, categoryID INTEGER FOREIGN KEY)";

            // DO NOT FORGET TO BIND PARAMETERS/VALUES OR IT'S A 0.
            // after each commadn, execute3 query and dispose of it to "clean". explanations are given by Sandy in comments ^.
            

            
            cmd.ExecuteNonQuery();
            cmd.Dispose();
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

        static public String GetSolutionDir()
        {

            // this is valid for C# .Net Foundation (not for C# .Net Core)
            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
        }
    }

}
