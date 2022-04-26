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

namespace HomeBudgetWPF
{
    /// <summary>
    /// Interaction logic for AddExpense.xaml
    /// </summary>
    public partial class AddExpense : Window, AddExpenseInterface
    {
        List<Budget.Category> catsList;
        static int previousIndexSelected = -1;
        static DateTime previousDateSelected = DateTime.Today;
        string filepath;
        ComboBox categoriesDropDown;

        ExpensePresenter presenter;
        DataGrid myDataGrid;

        /// <summary>
        /// Gets datagrid, filename and categoriesdropdown values and initializes window.
        /// </summary>
        /// <param name="dataGrid">The datagrid in the main window.</param>
        /// <param name="filename">The filepath of the database.</param>
        /// <param name="catDropDown">The categories drop down list in the main window</param>
        public AddExpense(DataGrid dataGrid, string filename, ComboBox catDropDown)
        {
            InitializeComponent();
            myDataGrid = dataGrid;
            presenter = new ExpensePresenter(this, filename); //Opens database connection
            presenter.openDatabase(filename);
            filepath = filename;
            categoriesDropDown = catDropDown;

            catsList = presenter.getCategoriesList();  //Gets categories list
            CategoriesDropDown.ItemsSource = catsList; //Populates drop down list with categories list
            DateTimePicker1.SelectedDate = previousDateSelected; //Selected date is the same as the previous expense
            CategoriesDropDown.SelectedIndex = previousIndexSelected; //Selected category is the same as the previous expense
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
                Category CategoryWindow = new Category(CategoriesDropDown.Text, filepath, CategoriesDropDown);
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

                int index = CategoriesDropDown.SelectedIndex;

                previousIndexSelected = index;
                previousDateSelected = date;

                presenter.AddExpense(date, index, amount, desc);

                myDataGrid.ItemsSource = presenter.GetBudgetItemsList(); //Updates datagrid

                categoriesDropDown.ItemsSource = presenter.getCategoriesList(); //Updates drop down list

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


    }
}
