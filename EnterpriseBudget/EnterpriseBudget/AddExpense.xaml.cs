using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EnterpriseBudget
{
    /// <summary>
    /// Interaction logic for AddExpense.xaml
    /// </summary>
    public partial class AddExpense : Window, AddExpenseInterface
    {
        List<Budget.Category> catsList;
        static int previousIndexSelected = -1;
        static string prevDesc = "";
        static DateTime previousDateSelected = DateTime.Today;
        string filepath;
        ComboBox categoriesDropDown;

        ExpensePresenter presenter;
        DataGrid myDataGrid;

        string windowColorMode;

        /// <summary>
        /// Gets datagrid, filename and categoriesdropdown values and initializes window.
        /// </summary>
        /// <param name="dataGrid">The datagrid in the main window.</param>
        /// <param name="filename">The filepath of the database.</param>
        /// <param name="catDropDown">The categories drop down list in the main window</param>
        public AddExpense(DataGrid dataGrid, string filename, ComboBox catDropDown, object colormode)
        {
            InitializeComponent();
            windowColorMode = colormode as String;
            GenerateWindowColors();

            myDataGrid = dataGrid;
            presenter = new ExpensePresenter(this, filename); //Opens database connection
            presenter.openDatabase(filename);
            filepath = filename;
            categoriesDropDown = catDropDown;

            actions.Text = prevDesc + " added to Expenses";

            catsList = presenter.getCategoriesList();  //Gets categories list
            CategoriesDropDown.ItemsSource = catsList; //Populates drop down list with categories list
            DateTimePicker1.SelectedDate = previousDateSelected; //Selected date is the same as the previous expense
            CategoriesDropDown.SelectedIndex = previousIndexSelected; //Selected category is the same as the previous expense
        }

        public void GenerateWindowColors()
        {
            if (windowColorMode == "Light Mode")
            {
                Color color = (Color)ColorConverter.ConvertFromString("#0C6291");

                var brush = new SolidColorBrush(color);

                mainGridAddExpense.Background = Brushes.Black;

                Amount.Background = Brushes.DarkGray;
                Desc.Background = Brushes.DarkGray;
                DateTimePicker1.Background = Brushes.DarkGray;

                addExpense_btn.Background = brush;
                addExpense_btn.Foreground = Brushes.DarkGray;

                cancelExpense_btn.Background = brush;
                cancelExpense_btn.Foreground = Brushes.DarkGray;

                addExpense_btn.BorderBrush = brush;
                cancelExpense_btn.BorderBrush = brush;

                DateTimePicker1.BorderBrush = brush;

                CategoriesDropDown.Background = brush;
                //categoriesDropDown.Foreground = Brushes.DarkGray;

                last_action.Foreground = Brushes.DarkGray;
                last_action.Background = Brushes.Black;

                actions.Foreground = Brushes.DarkGray;
                actions.Background = Brushes.Black;


            }
            else
            {

                Color color = (Color)ColorConverter.ConvertFromString("#C9E4E7");
                Color darkBlue = (Color)ColorConverter.ConvertFromString("#0C6291");

                mainGridAddExpense.Background = Brushes.White;

                var brush = new SolidColorBrush(color);
                var blueBrush = new SolidColorBrush(darkBlue);
                Amount.Background = Brushes.White;
                Desc.Background = Brushes.White;
                DateTimePicker1.Background = Brushes.White;

                addExpense_btn.Background = blueBrush;
                addExpense_btn.Foreground = Brushes.White;

                cancelExpense_btn.Background = blueBrush;
                cancelExpense_btn.Foreground = Brushes.White;

                DateTimePicker1.BorderBrush = blueBrush;

                CategoriesDropDown.Background = brush;
                //categoriesDropDown.Foreground = Brushes.DarkGray;

                last_action.Foreground = blueBrush;
                last_action.Background = Brushes.White;

                actions.Foreground = blueBrush;
                actions.Background = Brushes.White;
            }

        }

        private void CategoriesDropDown_TextChanged(object sender, MouseButtonEventArgs e)
        {
            if (CategoriesDropDown.Text == "Select or type a category")
            {
                CategoriesDropDown.Text = "";
            }
        }
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                //New category window shown if a category is typed in drop down list
                Category CategoryWindow = new Category(CategoriesDropDown.Text, filepath, CategoriesDropDown, windowColorMode);
                CategoryWindow.Show();
            }
        }

        private void Amount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }


        // Closing the application.
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ((Amount.Text == "" || Amount.Text == "Amount") && (Desc.Text == "" || Desc.Text == "Description"))
            {
                e.Cancel = false;
            }
            else
            {
                if (MessageBox.Show("If you close this window without adding your expense, your changes will be lost.\nExit?", "Add expense", MessageBoxButton.YesNo, MessageBoxImage.Hand) == MessageBoxResult.Yes)
                {
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
        private void Amount_TextChanged(object sender, MouseButtonEventArgs e)
        {
            if (Amount.Text == "Amount")
            {
                Amount.Text = "";
            }
        }

        private void Desc_TextChanged(object sender, MouseButtonEventArgs e)
        {
            if (Desc.Text == "Description")
            {
                Desc.Text = "";
            }
        }

        private void AddExpenses_Click(object sender, RoutedEventArgs e)
        {
            // Input validation.
            if (DateTimePicker1.SelectedDate.HasValue == false)
            {
                ShowError("Please select a date!");
            }

            else if (Amount.Text == "" || Amount.Text == "Amount")
            {
                ShowError("Please enter an amount!");
            }

            else if (Desc.Text == "" || Desc.Text == "Description")
            {
                ShowError("Please enter a description!");
            }

            else if (CategoriesDropDown.SelectedIndex == -1)
            {
                ShowError("Please enter a category from the list, or create a new one!");
            }

            else
            {
                DateTime date = DateTimePicker1.SelectedDate.Value;

                int amount = int.Parse(Amount.Text);

                string desc = Desc.Text;

                prevDesc = desc;

                int index = CategoriesDropDown.SelectedIndex;

                previousIndexSelected = index;
                previousDateSelected = date;

                presenter.AddExpense(date, index, amount, desc);

                myDataGrid.ItemsSource = presenter.GetBudgetItemsList(); //Updates datagrid

                categoriesDropDown.ItemsSource = presenter.getCategoriesList(); //Updates drop down list



                actions.Text = prevDesc + " added to Expenses";

                this.Close();
            }
        }

        /// <summary>
        /// Shows error message to the user
        /// </summary>
        /// <param name="msg">The error message details</param>
        public void ShowError(string msg)
        {
            MessageBox.Show(msg);
        }

        public void ShowAdded(string desc)
        {
            MessageBox.Show(desc + " added");
        }

        /// <summary>
        /// Clears fields except Date and Category.
        /// </summary>
        public void Refresh()
        {
            Amount.Text = "Amount";
            Desc.Text = "Description";
        }

        /// <summary>
        /// Cancels expense entry and clears fields from user input.
        /// </summary>
        public void CancelExpense()
        {
            MessageBox.Show("Your entries will be cleared.", "Configuration", MessageBoxButton.OK, MessageBoxImage.Warning);

            // Clear fields.
            DateTimePicker1.SelectedDate = null;
            Amount.Text = "Amount";
            Desc.Text = "Description";
            CategoriesDropDown.SelectedItem = null;
        }
        private void CancelExpenses_Click(object sender, RoutedEventArgs e)
        {
            presenter.ClearFields();
        }

        private void CategoriesDropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox catComboBox = (ComboBox)sender;
            var selectedItem = catComboBox.SelectedItem;
            limit.Text = selectedItem + "";
        }
    }
}
