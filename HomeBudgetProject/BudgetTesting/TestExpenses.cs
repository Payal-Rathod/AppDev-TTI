using System;
using Xunit;
using System.IO;
using System.Collections.Generic;
using Budget;
using System.Data.SQLite;

namespace BudgetCodeTests
{
    [Collection("Sequential")]
    public class TestExpenses
    {
        int numberOfExpensesInFile = TestConstants.numberOfExpensesInFile;
        String testInputFile = TestConstants.testExpensesInputFile;
        int maxIDInExpenseFile = TestConstants.maxIDInExpenseFile;
        Expense firstExpenseInFile = new Expense(1, new DateTime(2021, 1, 10), 10, 12, "hat (on credit)");


        // ========================================================================

        [Fact]
        public void ExpensesObject_New()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\newDB.db";
            Database.newDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;


            // Act
            Categories categories = new Categories(conn, true);
            Expenses expenses = new Expenses(conn);

            // Assert 
            Assert.IsType<Expenses>(expenses);

        }

        //==========================================================================

        [Fact]
        public void ExpensesMethod_ReadFromDatabase_ValidateCorrectDataWasRead()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;


            // Act
            Categories categories = new Categories(conn, false);
            Expenses expenses = new Expenses(conn);
            List<Expense> list = expenses.List();
            Expense firstExpense = list[0];

            // Assert
            Assert.Equal(numberOfExpensesInFile, list.Count);
            Assert.Equal(firstExpenseInFile.Id, firstExpense.Id);
            Assert.Equal(firstExpenseInFile.Description, firstExpense.Description);

        }

        // ========================================================================

        [Fact]
        public void ExpensesMethod_List_ReturnsListOfExpenses()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\{TestConstants.testDBInputFile}";
            Database.existingDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;
            Categories categories = new Categories(conn, false);
            Expenses expenses = new Expenses(conn);

            // Act
            List<Expense> list = expenses.List();

            // Assert
            Assert.Equal(numberOfExpensesInFile, list.Count);

        }

        // ========================================================================

        [Fact]
        public void ExpensesMethod_Add()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Categories categories = new Categories(conn, false);
            Expenses expenses = new Expenses(conn);
            string descr = "New Expense";
            int type = 2;
            double amount = -20;
            DateTime date = DateTime.Now;

            // Act
            expenses.Add(date, type, amount, descr);
            List<Expense> expensesList = expenses.List();
            int sizeOfList = expenses.List().Count;

            // Assert
            Assert.Equal(numberOfExpensesInFile + 1, sizeOfList);
            Assert.Equal(descr, expensesList[sizeOfList - 1].Description);
            Assert.Equal(type, expensesList[sizeOfList - 1].Category);
            Assert.Equal(amount, expensesList[sizeOfList - 1].Amount);
            Assert.Equal(date, expensesList[sizeOfList - 1].Date);
        }

        // ========================================================================

        [Fact]
        public void ExpensesMethod_Delete()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Categories categories = new Categories(conn, false);
            Expenses expenses = new Expenses(conn);
            int IdToDelete = 3;

            // Act
            expenses.Delete(IdToDelete);
            List<Expense> expensesList = expenses.List();
            int sizeOfList = expensesList.Count;

            // Assert
            Assert.Equal(numberOfExpensesInFile - 1, sizeOfList);
            Assert.False(expensesList.Exists(e => e.Id == IdToDelete), "correct Expense item deleted");

        }

        // ========================================================================

        [Fact]
        public void ExpensesMethod_Delete_InvalidIDDoesntCrash()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messyDB";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Categories categories = new Categories(conn, false);
            Expenses expenses = new Expenses(conn);
            int IdToDelete = 9999;
            int sizeOfList = expenses.List().Count;

            // Act
            try
            {
                expenses.Delete(IdToDelete);
                Assert.Equal(sizeOfList, expenses.List().Count);
            }

            // Assert
            catch
            {
                Assert.True(false, "Invalid ID causes Delete to break");
            }
        }

        // ========================================================================

        [Fact]
        public void ExpensesMethod_UpdateExpenses()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messyDB";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Categories categories = new Categories(conn, false);
            Expenses expenses = new Expenses(conn);
            String newDescr = "Presents";
            double newAmount = -10;
            DateTime newDate = DateTime.MinValue;
            int newCategory = 2;
            int id = 3;

            // Act
            expenses.UpdateProperties(id, newDate, newDescr, newAmount, newCategory);
            List<Expense> expensesList = expenses.List();

            // Assert 
            Assert.True(expensesList.Exists(e => e.Description == newDescr), "Expense item updated");
        }
    }
}

