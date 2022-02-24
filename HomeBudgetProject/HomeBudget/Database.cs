﻿using System;
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

            cmd.CommandText = "DROP TABLE IF EXISTS categories";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DROP TABLE IF EXISTS expenses";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DROP TABLE IF EXISTS categoryTypes";
            cmd.ExecuteNonQuery();

            //Creates Categorytypes table
            cmd.CommandText = @"CREATE TABLE categoryTypes(Id INTEGER PRIMARY KEY, 
            Description TEXT)";
            cmd.ExecuteNonQuery();

            //Creates Category table
            cmd.CommandText = @"CREATE TABLE categories(Id INTEGER PRIMARY KEY,
            Description TEXT, TypeId INTEGER, FOREIGN KEY(TypeId) REFERENCES categoryTypes(Id))";
            cmd.ExecuteNonQuery();

            //Creates Expense table
            cmd.CommandText = @"CREATE TABLE expenses(Id INTEGER PRIMARY KEY, Date TEXT,
            Description TEXT, Amount DOUBLE, CategoryId INTEGER, FOREIGN KEY(CategoryId) REFERENCES categories(Id))";
            cmd.ExecuteNonQuery();

            //cmd.CommandText = "INSERT INTO categoryTypes(Id, Description) VALUES(@Id, @Description)";
            //cmd.ExecuteNonQuery();

            //cmd.CommandText = "INSERT INTO categories(Id, Description, TypeId) VALUES(@Id, @Description, @TypeId)";
            //cmd.ExecuteNonQuery();

            //cmd.CommandText = "INSERT INTO expenses(Id, Date, Description, Amount, CategoryId) VALUES(@Id, @Date, @Description, @Amount, @CategoryId)";
            //cmd.ExecuteNonQuery();

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

            String connection_string = $"Data Source={filename}; Foreign Keys=1;";
            _connection = new SQLiteConnection(connection_string);
            _connection.Open();
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
