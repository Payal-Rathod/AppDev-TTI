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
        public bool calledCancel;
        public bool calledDarkMode;
        public bool calledDisableBtnAndInput;
        public bool calledEnableBtnAndInput;
        public bool calledLightMode;
        public bool calledRefresh;
        public bool calledOpenFile;
        public bool calledShowAdded;
        public bool calledShowDatabase;
        public bool calledShowError;

        public void Cancel()
        {
            calledCancel = true;
        }

        public void DarkMode()
        {
            calledDarkMode = true;
        }

        public void DisableBtnAndInput()
        {
            calledDisableBtnAndInput = true;
        }

        public void EnableBtnAndInput()
        {
            calledEnableBtnAndInput = true;
        }

        public void LightMode()
        {
            calledLightMode = true;
        }

        public void OpenFile()
        {
            calledOpenFile = true;
        }

        public void Refresh()
        {
            calledRefresh = true;
        }

        public void ShowAdded(string desc)
        {
            calledShowAdded = true;
        }

        public void ShowDatabase(string filename)
        {
            calledShowDatabase = true;
        }

        public void ShowError(string msg)
        {
            calledShowError = true;
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
            view.calledRefresh = false;
            view.calledDisableBtnAndInput = false;
            view.calledShowDatabase = false;

            //Act
            p.NewDatabase();

            //Assert
            Assert.True(view.calledRefresh);
            Assert.True(view.calledShowDatabase);
            Assert.True(view.calledDisableBtnAndInput);
        }


        [Fact]
        public void TestOPenDatabaseDoesNotCrash()
        {
            //Arrange
            TestView view = new TestView();
            Presenter p = new Presenter(view);
            view.calledEnableBtnAndInput = false;
            string directory = Directory.GetCurrentDirectory() + "\\..\\..\\..";
            string filename = directory + "\\" + "test.db";
            bool newDb = false;

            //Act
            p.OpenDatabase(filename, newDb);

            //Assert
            Assert.True(view.calledEnableBtnAndInput);
        }

        [Fact]
        public void TestAddExpenseDoesNotCrash()
        {
            //Arrange
            TestView view = new TestView();
            Presenter p = new Presenter(view);
            view.calledShowAdded = false;
            view.calledRefresh = false;
            string directory = Directory.GetCurrentDirectory() + "\\..\\..\\..";
            string filename = directory + "\\" + "test.db";
            bool newDb = false;
            p.OpenDatabase(filename, newDb);

            //Act
            p.AddExpense(DateTime.Today, 1, 10, "test");

            //Assert
            Assert.True(view.calledShowAdded);
            Assert.True(view.calledRefresh);
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
        public void TestAddCategoryDoesNotCrash()
        {
            //Arrange
            TestView view = new TestView();
            Presenter p = new Presenter(view);
            string directory = Directory.GetCurrentDirectory() + "\\..\\..\\..";
            string filename = directory + "\\" + "test.db";
            bool newDb = false;
            p.OpenDatabase(filename, newDb);
            List<Budget.Category> catsBefore = new List<Budget.Category>();
            List<Budget.Category> catsAfter = new List<Budget.Category>();

            Budget.Category newCat;

            //Act
            catsBefore = p.getCategoriesList();
            newCat = p.AddCategory("Car", Budget.Category.CategoryType.Expense);
            catsAfter = p.getCategoriesList();

            //Assert
            Assert.Equal(catsAfter.Count, catsBefore.Count+1);
        }
    }
 }
