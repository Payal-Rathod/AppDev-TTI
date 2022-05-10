using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace EnterpriseBudget
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ViewInterface
    {
        private const int NUMFILESRECENT = 3;
        MainPresenter presenter;
        public string fileName;
        public string filePath;
        public bool filterFlag = false;
        public DateTime? startDate = DateTime.MinValue;
        public DateTime? endDate = DateTime.MaxValue;
        public int filterCategoryId = -1;
        public string filePathString;
        bool newDb;
        List<Budget.Category> catsList;
        List<String> recentlyOpenedFile = new List<String>();
        AddExpense AddWindow;
        public static string appDataPath = Environment.GetEnvironmentVariable("APPDATA");
        /// <summary>
        /// Initializes application and Presenter.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            presenter = new MainPresenter(this);

            Application.Current.MainWindow.FontFamily = new FontFamily("Cambria");

            ShowRecentlyOpened();

            InitializeDataGrid();
        }

        /// <summary>
        /// Shows the pie chart
        /// </summary>
        public void ShowChart()
        {
            List<Dictionary<string, object>> myItems = presenter.GetBudgetItemsListByMonthAndCategory(startDate, endDate, filterFlag, filterCategoryId);
            List<Object> theObjects = myItems.Cast<object>().ToList();

            ViewExpenses.ItemsSource = myItems;

            presenter.DataSource = theObjects;

            List<String> test = new List<string>();
            chartView.InitializeByCategoryAndMonthDisplay(presenter.getCategoriesListInString());
            chartView.DataSource = theObjects;
        }

        /// <summary>
        /// Initializes grid for getbudgetitems list
        /// </summary>
        public void InitializeDataGrid()
        {
            ViewExpenses.Columns.Clear();

            // create columns
            var col1 = new DataGridTextColumn();
            col1.Header = "Date";
            col1.Binding = new Binding("Date");
            ViewExpenses.Columns.Add(col1);
            var col2 = new DataGridTextColumn();
            col2.Header = "Category";
            col2.Binding = new Binding("Category");
            ViewExpenses.Columns.Add(col2);
            var col3 = new DataGridTextColumn();
            col3.Header = "Description";
            col3.Binding = new Binding("ShortDescription");
            ViewExpenses.Columns.Add(col3);
            var col4 = new DataGridTextColumn();
            col4.Header = "Amount";
            col4.Binding = new Binding("Amount");
            ViewExpenses.Columns.Add(col4);
            var col5 = new DataGridTextColumn();
            col4.Binding.StringFormat = "F2";

            Style s = new Style();
            s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty,
                                    TextAlignment.Right));
            col4.CellStyle = s;


            col5.Header = "Balance";
            col5.Binding = new Binding("Balance");
            ViewExpenses.Columns.Add(col5);
            col5.Binding.StringFormat = "F2";

        }

        /// <summary>
        /// Initializes grid for getbudgetitemsbymonth list
        /// </summary>
        public void InitializeDataGridByMonth()
        {
            ViewExpenses.Columns.Clear();

            // create columns
            var col1 = new DataGridTextColumn();
            col1.Header = "Month";
            col1.Binding = new Binding("Month");
            ViewExpenses.Columns.Add(col1);
            var col2 = new DataGridTextColumn();
            col2.Header = "Total";
            col2.Binding = new Binding("Total");
            ViewExpenses.Columns.Add(col2);

            col2.Binding.StringFormat = "F2";

            Style s = new Style();
            s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty,
                                    TextAlignment.Right));
            col2.CellStyle = s;


        }

        /// <summary>
        /// Initializes grid for getbudgetitemsbycategory list
        /// </summary>
        public void InitializeDataGridByCategory()
        {
            ViewExpenses.Columns.Clear();

            // create columns
            var col1 = new DataGridTextColumn();
            col1.Header = "Category";
            col1.Binding = new Binding("Category");
            ViewExpenses.Columns.Add(col1);
            var col2 = new DataGridTextColumn();
            col2.Header = "Total";
            col2.Binding = new Binding("Total");
            ViewExpenses.Columns.Add(col2);

            col2.Binding.StringFormat = "F2";

            Style s = new Style();
            s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty,
                                    TextAlignment.Right));
            col2.CellStyle = s;
        }

        /// <summary>
        /// Initializes grid for getbudgetitemsbycategoryandmonth list
        /// </summary>
        public void InitializeDataGridByMonthAndCategory(List<Dictionary<string, object>> items)
        {
            ViewExpenses.Columns.Clear();

            foreach (string key in items[items.Count-1].Keys) //Goes through each key values
            {
                if (key.Split(':')[0] == "details")
                {
                    continue;
                }
                var column = new DataGridTextColumn();
                column.Header = key;
                column.Binding = new Binding($"[{key}]");

                if (key.Split(':')[0] == "Total")
                {
                    column.Binding.StringFormat = "F2";

                    Style s = new Style();
                    s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty,
                                            TextAlignment.Right));
                    column.CellStyle = s;
                }

                ViewExpenses.Columns.Add(column);
            }
        }
        // =====================================================================================
        // VIEW INTERFACE
        // =====================================================================================

        /// <summary>
        /// Opens file from File Explorer, only .db files.
        /// </summary>
        public void OpenFile()
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Filter = "Database file (.db)|*.db";
            Nullable<bool> result = openFileDlg.ShowDialog();
            if (result == true)
            {
                ShowDatabase(openFileDlg.FileName);
                fileName = openFileDlg.FileName;
                //MessageBox.Show(File.ReadLines(filePathString).Count() + "");
                if (recentlyOpenedFile.Contains(fileName))
                {

                }
                else if (recentlyOpenedFile.Count == NUMFILESRECENT)
                {
                    recentlyOpenedFile.RemoveAt(0);
                    recentlyOpenedFile.Add(fileName);
                    File.WriteAllLines(filePathString, recentlyOpenedFile);

                }
                else
                {
                    recentlyOpenedFile.Add(fileName);
                    File.WriteAllLines(filePathString, recentlyOpenedFile);
                }

            }
            else
            {
                return;
            }

            if (new FileInfo(fileName).Length == 0)
            {
                newDb = true;
            }
            else
            {
                newDb = false;
            }

            presenter.OpenDatabase(fileName, newDb);
            
            ViewExpenses.ItemsSource =  presenter.GetBudgetItemsList(null, null, false, -1);

            CategoriesDropDown.ItemsSource = presenter.getCategoriesList();

            recentlyOpened.Items.Clear();

            chartView.Visibility = Visibility.Hidden;


            for (int i = 0; i < recentlyOpenedFile.Count; i++)
            {
                MenuItem file = new MenuItem();
                file.Header = recentlyOpenedFile[i];
                recentlyOpened.Items.Add(file);

                file.Click += OpenRecent_Click;
            }
        }

        /// <summary>
        /// Opens databse connection to a new db file
        /// </summary>
        public void NewFile()
        {
            SaveFileDialog saveFileDlg = new SaveFileDialog();
            saveFileDlg.Filter = "Database file (.db)|*.db";

            if (saveFileDlg.ShowDialog() == true)
            {
                ShowDatabase(saveFileDlg.FileName);
                fileName = saveFileDlg.FileName;

                if (recentlyOpenedFile.Contains(fileName))
                {

                }
                else if (recentlyOpenedFile.Count == NUMFILESRECENT)
                {
                    recentlyOpenedFile.RemoveAt(0);
                    recentlyOpenedFile.Add(fileName);
                    File.WriteAllLines(filePathString, recentlyOpenedFile);

                }
                else
                {
                    recentlyOpenedFile.Add(fileName);
                    File.WriteAllLines(filePathString, recentlyOpenedFile);
                }
            }
            else
            {
                return;
            }

            presenter.OpenDatabase(fileName, newDb = true);

            ViewExpenses.ItemsSource = presenter.GetBudgetItemsList(null, null, false, -1);

            CategoriesDropDown.ItemsSource = presenter.getCategoriesList();

            recentlyOpened.Items.Clear();

            for (int i = 0; i < recentlyOpenedFile.Count; i++)
            {
                MenuItem file = new MenuItem();
                file.Header = recentlyOpenedFile[i];

                recentlyOpened.Items.Add(file);

                file.Click += OpenRecent_Click;
            }

            chartView.Visibility = Visibility.Hidden;

        }

        /// <summary>
        /// Shows database from file.
        /// </summary>
        /// <param name="fileName">String value of file name.</param>
        public void ShowDatabase(string fileName)
        {
            FileNameTextBox.Text = fileName;
        }

        /// <summary>
        /// Light color mode.
        /// </summary>
        public void LightMode()
        { 
            theme.Content = "Dark Mode";
            theme.Foreground = Brushes.White;
            theme.Background = Brushes.Black;
            FileNameTextBox.Foreground = Brushes.Black;

            Color color = (Color)ColorConverter.ConvertFromString("#C9E4E7");
            Color darkBlue = (Color)ColorConverter.ConvertFromString("#0C6291");

            var brush = new SolidColorBrush(color);
            var blueBrush = new SolidColorBrush(darkBlue);

            mainGrid.Background = brush;
            menu.Background = brush;

            Header.Foreground = blueBrush;
            Header.Foreground = blueBrush;

        }
        /// <summary>
        /// Dark color mode.
        /// </summary>
        public void DarkMode()
        {
            
            Color color = (Color)ColorConverter.ConvertFromString("#0C6291");

            var brush = new SolidColorBrush(color);

            theme.Content = "Light Mode";
            theme.Foreground = Brushes.Black;
            theme.Background = Brushes.White;
            mainGrid.Background = Brushes.Black;
            menu.Background = Brushes.Black;
            FileNameTextBox.Foreground = brush;
            filterCheck.Foreground = brush;
            monthCheck.Foreground = brush;
            categoryCheck.Foreground = brush;
            Header.Foreground = brush;
        }

        // =====================================================================================
        // EVENT HANDLERS
        // =====================================================================================
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            presenter.OpenFile();
        }

        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            presenter.NewFile();
        }
       
        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ColorMode_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            presenter.ChangeColorMode(btn.Content.ToString());
        }      

        private void AddExpense_Click(object sender, RoutedEventArgs e)
        {
            AddWindow = new AddExpense(ViewExpenses, fileName, CategoriesDropDown, theme.Content);
            AddWindow.Show();

            ViewExpenses.ItemsSource = presenter.GetBudgetItemsList(startDate, endDate, filterFlag, filterCategoryId);
        }

        private void StartDateTimePicker1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            startDate = StartDateTimePicker1.SelectedDate;
            ViewExpenses.ItemsSource = presenter.GetBudgetItemsList(startDate, endDate, filterFlag, filterCategoryId);
        }

        private void EndDateTimePicker1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            endDate = EndDateTimePicker1.SelectedDate;
            ViewExpenses.ItemsSource = presenter.GetBudgetItemsList(startDate, endDate, filterFlag, filterCategoryId);
        }

        private void CategoriesDropDown_TextChanged(object sender, MouseButtonEventArgs e)
        {
            if (CategoriesDropDown.Text == "Select or type a category")
            {
                CategoriesDropDown.Text = "";
            }
        }

        private void filterCheck_Click(object sender, RoutedEventArgs e)
        {
            chartView.Visibility = Visibility.Hidden;
            showChart.Visibility = Visibility.Hidden;
            monthCheck.IsChecked = false;
            categoryCheck.IsChecked = false;

            if (filterCheck.IsChecked == true)
            {

                filterCategoryId = CategoriesDropDown.SelectedIndex;
                filterFlag = true;
                ViewExpenses.ItemsSource = presenter.GetBudgetItemsList(startDate, endDate, filterFlag, filterCategoryId);
            }
            else
            {
                filterCategoryId = -1;
                filterFlag = false;
                ViewExpenses.ItemsSource = presenter.GetBudgetItemsList(startDate, endDate, filterFlag, filterCategoryId);
            }
        }

        private void MonthCategoryCheck_Click(object sender, RoutedEventArgs e)
        {
            if (monthCheck.IsChecked == true && categoryCheck.IsChecked == false)
            {
                ViewExpenses.ItemsSource = presenter.GetBudgetItemsListByMonth(startDate, endDate, filterFlag, filterCategoryId);
                showChart.Visibility = Visibility.Hidden;
                chartView.Visibility = Visibility.Hidden;
            }
            else if (monthCheck.IsChecked == true && categoryCheck.IsChecked == true)
            {
                List<Dictionary<string, object>> items = presenter.GetBudgetItemsListByMonthAndCategory(startDate, endDate, filterFlag, filterCategoryId);
                ViewExpenses.ItemsSource = items;
                showChart.Visibility = Visibility.Visible;

            }
            else if (monthCheck.IsChecked == false && categoryCheck.IsChecked == true)
            {
                ViewExpenses.ItemsSource = presenter.GetBudgetItemsListByCategory(startDate, endDate, filterFlag, filterCategoryId);
                showChart.Visibility = Visibility.Hidden;
                chartView.Visibility = Visibility.Hidden;
            }
            else
            {
                ViewExpenses.ItemsSource = presenter.GetBudgetItemsList(startDate, endDate, filterFlag, filterCategoryId);
                showChart.Visibility = Visibility.Hidden;
                chartView.Visibility = Visibility.Hidden;

            }

        }

        private void close_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            int windowActiveCount = 0;
            foreach (var Window in App.Current.Windows)
            {
                windowActiveCount++;
            }
            if (windowActiveCount > 3)
            {
                if (MessageBox.Show("There are unsaved changes.\nExit?", "Add expense", MessageBoxButton.YesNo, MessageBoxImage.Hand) == MessageBoxResult.Yes)
                {
                    App.Current.Shutdown();
                }
            }
            else
            {
                App.Current.Shutdown();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int windowActiveCount = 0;
            foreach (var Window in App.Current.Windows)
            {
                windowActiveCount++;
            }
            if (windowActiveCount > 2)
            {
                if (MessageBox.Show("There are unsaved changes.\nExit?", "Add expense", MessageBoxButton.YesNo, MessageBoxImage.Hand) == MessageBoxResult.Yes)
                {
                    App.Current.Shutdown();
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                App.Current.Shutdown();
            }
        }

        int selectedCouter = 0;

        private void enter_Click(object sender, RoutedEventArgs e)
        {
            if (searchBox.Text == "")
            {
                MessageBox.Show("Please enter a value in the search bar");
            }
            else
            {
                string searchBoxValue = searchBox.Text.ToLower();
                var items = ViewExpenses.ItemsSource as List<Budget.BudgetItem>;

                var item = items.FindAll(it => it.ShortDescription.ToLower().Contains(searchBoxValue) || it.Amount.ToString().Contains(searchBoxValue) || it.Balance.ToString().Contains(searchBoxValue) || it.Category.ToLower().Contains(searchBoxValue) || it.Date.ToString().Contains(searchBoxValue) || it.CategoryID.ToString().Contains(searchBoxValue) || it.ExpenseID.ToString().Contains(searchBoxValue));

                showResults.Text = item.Count + " results found";

                if (item.Count == 0)
                {
                    MessageBox.Show("No match found");
                    return;
                }
                else if (selectedCouter < item.Count)
                {
                    ViewExpenses.SelectedItem = item[selectedCouter];
                }
                else
                {
                    selectedCouter = 0;
                    ViewExpenses.SelectedItem = item[selectedCouter];
                }

                ViewExpenses.ScrollIntoView(item[selectedCouter]);
                selectedCouter++;
            }

        }

        private void RecentlyOpened_Click(object sender, RoutedEventArgs e)
        {
        }

        private void OpenRecent_Click(object sender, RoutedEventArgs e)
        {
            chartView.Visibility = Visibility.Hidden;

            MenuItem file = sender as MenuItem;
            string path = file.Header.ToString();
            presenter.OpenDatabase(path, false);
            ShowDatabase(path);

            ViewExpenses.ItemsSource = presenter.GetBudgetItemsList(null, null, false, -1);

            CategoriesDropDown.ItemsSource = presenter.getCategoriesList();

        }

        public void ShowRecentlyOpened()
        {
            string pathString = System.IO.Path.Combine(appDataPath, "Budget");

            if (!System.IO.Directory.Exists(pathString))
            {
                System.IO.Directory.CreateDirectory(pathString);
            }

            filePathString = System.IO.Path.Combine(pathString, "RecentlyOpenedDBFiles.txt");

            if (!System.IO.File.Exists(filePathString))
            {
                System.IO.File.CreateText(filePathString);
            }
            for (int i = 0; i < File.ReadLines(filePathString).Count(); i++)
            {
                recentlyOpenedFile.Add(File.ReadAllLines(filePathString)[i]);
            }
        }

        private void showChart_Click(object sender, RoutedEventArgs e)
        {
            chartView.Visibility = Visibility.Visible;
            ShowChart();
        }
    }
}
