using HomeBudgetWPF;
using System;
using System.IO;
using Xunit;
using System.Data.SQLite;
using Budget;
using System.Collections.Generic;

namespace HomeBudgetWPFTest
{
    public class TestView : ViewInterface
    {
        public bool calledDarkMode;
        public bool calledLightMode;
        public bool calledNewFile;
        public bool calledOpenFile;
        public bool calledShowDatabase;
        public bool calledInitializeDataGrid;
        public bool calledInitializeDataGridByMonth;
        public bool calledInitializeDataGridByCategory;
        public bool calledInitializeDataGridByMonthAndCategory;


        public void DarkMode()
        {
            calledDarkMode = true;
        }

        public void LightMode()
        {
            calledLightMode = true;
        }

        public void NewFile()
        {
            calledNewFile = true;
        }

        public void OpenFile()
        {
            calledOpenFile = true;
        }

        public void ShowDatabase(string filename)
        {
            calledShowDatabase = true;
        }
        public void InitializeDataGrid()
        {
            calledInitializeDataGrid = true;
        }

        public void InitializeDataGridByMonth()
        {
            calledInitializeDataGridByMonth = true;
        }

        public void InitializeDataGridByCategory()
        {
            calledInitializeDataGridByCategory = true;
        }

        public void InitializeDataGridByMonthAndCategory(List<Dictionary<string, object>> items)
        {
            calledInitializeDataGridByMonthAndCategory = true;
        }

        [Fact]
        public void TestConstructor()
        {
            //Arrange
            TestView view = new TestView();

            //Act
            Presenter p = new Presenter(view);

            //Assert
            Assert.IsType<Presenter>(p);
        }

        [Fact]
        public void TestConstructorCalledNewDatabase()
        {
            //Arrange
            TestView view = new TestView();
            Presenter p = new Presenter(view);
            view.calledShowDatabase = false;

            //Act
            p.NewDatabase();

            //Assert
            Assert.True(view.calledShowDatabase);
        }


        [Fact]
        public void TestOpenDatabaseDoesNotCrash()
        {
            //Arrange
            TestView view = new TestView();
            Presenter p = new Presenter(view);
            string directory = Directory.GetCurrentDirectory() + "\\..\\..\\..";
            string filename = directory + "\\" + "test.db";
            bool newDb = false;

            //Act
            p.OpenDatabase(filename, newDb);
        }

        [Fact]
        public void TestDeleteExpenseDoesNotCrash()
        {
            //Arrange
            TestView view = new TestView();
            Presenter p = new Presenter(view);

            string directory = Directory.GetCurrentDirectory() + "\\..\\..\\..";
            string filename = directory + "\\" + "test.db";
            bool newDb = false;
            p.OpenDatabase(filename, newDb);

            //Act
            p.DeleteExpense(1);

        }

        [Fact]
        public void TestOpenFile()
        {
            //Arrange
            TestView view = new TestView();
            Presenter p = new Presenter(view);
            view.calledOpenFile = false;
            string directory = Directory.GetCurrentDirectory() + "\\..\\..\\..";
            string filename = directory + "\\" + "test.db";
            bool newDb = false;
            p.OpenDatabase(filename, newDb);

            //Act
            p.OpenFile();

            //Assert
            Assert.True(view.calledOpenFile);
        }

        [Fact]
        public void TestNewFile()
        {
            //Arrange
            TestView view = new TestView();
            Presenter p = new Presenter(view);

            string directory = Directory.GetCurrentDirectory() + "\\..\\..\\..";
            string filename = directory + "\\" + "test.db";
            bool newDb = false;
            view.calledNewFile = false;
            p.OpenDatabase(filename, newDb);

            //Act
            p.NewFile();

            //Assert
            Assert.True(view.calledNewFile);
        }

        [Fact]
        public void TestChangeColorModeDark()
        {
            //Arrange
            TestView view = new TestView();
            Presenter p = new Presenter(view);
            view.calledDarkMode = false;

            //Act
            p.ChangeColorMode("Dark Mode");

            //Assert
            Assert.True(view.calledDarkMode);
        }

        [Fact]
        public void TestGetCategoryList()
        {
            //Arrange
            TestView view = new TestView();
            Presenter p = new Presenter(view);
            string directory = Directory.GetCurrentDirectory() + "\\..\\..\\..";
            string filename = directory + "\\" + "testUnchanged.db";
            bool newDb = false;
            p.OpenDatabase(filename, newDb);
            List<Budget.Category> cats = new List<Budget.Category>();

            //Act
            cats = p.getCategoriesList();

            //Assert
            Assert.Equal(16, cats.Count);
        }


        [Fact]
        public void TestGetBudgetItemsList()
        {
            //Arrange
            TestView view = new TestView();
            Presenter p = new Presenter(view);
            string directory = Directory.GetCurrentDirectory() + "\\..\\..\\..";
            string filename = directory + "\\" + "testUnchanged.db";
            bool newDb = false;
            view.calledInitializeDataGrid = false;
            p.OpenDatabase(filename, newDb);

            //Act
            p.GetBudgetItemsList(null, null, false, -1);

            //Assert
            Assert.True(view.calledInitializeDataGrid = true);
        }

        [Fact]
        public void TestGetBudgetItemsListByMonth()
        {
            //Arrange
            TestView view = new TestView();
            Presenter p = new Presenter(view);
            string directory = Directory.GetCurrentDirectory() + "\\..\\..\\..";
            string filename = directory + "\\" + "testUnchanged.db";
            bool newDb = false;
            view.calledInitializeDataGridByMonth = false;
            p.OpenDatabase(filename, newDb);

            //Act
            p.GetBudgetItemsListByMonth(null, null, false, -1);

            //Assert
            Assert.True(view.calledInitializeDataGridByMonth = true);
        }

        [Fact]
        public void TestGetBudgetItemsListByCategory()
        {
            //Arrange
            TestView view = new TestView();
            Presenter p = new Presenter(view);
            string directory = Directory.GetCurrentDirectory() + "\\..\\..\\..";
            string filename = directory + "\\" + "testUnchanged.db";
            bool newDb = false;
            view.calledInitializeDataGridByCategory = false;
            p.OpenDatabase(filename, newDb);

            //Act
            p.GetBudgetItemsListByCategory(null, null, false, -1);

            //Assert
            Assert.True(view.calledInitializeDataGridByCategory = true);
        }

        [Fact]
        public void TestGetBudgetItemsListByMonthAndCategory()
        {
            //Arrange
            TestView view = new TestView();
            Presenter p = new Presenter(view);
            string directory = Directory.GetCurrentDirectory() + "\\..\\..\\..";
            string filename = directory + "\\" + "testUnchanged.db";
            bool newDb = false;
            view.calledInitializeDataGridByMonthAndCategory = false;
            p.OpenDatabase(filename, newDb);

            //Act
            p.GetBudgetItemsListByMonthAndCategory(null, null, false, -1);

            //Assert
            Assert.True(view.calledInitializeDataGridByMonthAndCategory = true);
        }

    }
 }
