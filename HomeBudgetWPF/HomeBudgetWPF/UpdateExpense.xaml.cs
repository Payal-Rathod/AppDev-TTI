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
    /// Interaction logic for UpdateExpense.xaml
    /// </summary>
    public partial class UpdateExpense : Window
    {
        List<Budget.Category> catsList;
        Presenter presenter;
        Budget.BudgetItem item;
        DataGrid myDataGrid;

        public UpdateExpense(Presenter p, Budget.BudgetItem selectedItem, DataGrid datagrid)
        {
            InitializeComponent();
            presenter = p;
            catsList = p.getCategoriesList();

            CategoriesDropDown.ItemsSource = catsList;

            item = selectedItem;
            myDataGrid = datagrid;

            PopulateItemInForm();
        }

        public void PopulateItemInForm()
        {
            Amount.Text = item.Amount.ToString();
            Desc.Text = item.ShortDescription;
            CategoriesDropDown.SelectedIndex = item.CategoryID;
            DateTimePicker1.SelectedDate = item.Date;

        }

        private void cancelExpense_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void updateExpense_btn_Click(object sender, RoutedEventArgs e)
        {
            if (Amount.Text == item.Amount.ToString() && Desc.Text == item.ShortDescription && CategoriesDropDown.SelectedIndex == item.CategoryID && DateTimePicker1.SelectedDate == item.Date)
            {
                MessageBox.Show("No changes were made");
            }
            else
            {
                item.Amount = Convert.ToDouble(Amount.Text);
                item.ShortDescription = Desc.Text;
                item.Date = (DateTime)DateTimePicker1.SelectedDate;
                item.CategoryID = CategoriesDropDown.SelectedIndex;

                presenter.UpdateExpense(item);
            }

            myDataGrid.ItemsSource = presenter.GetBudgetItemsList();

            this.Close();

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
                Budget.Category cat = presenter.AddCategory(CategoriesDropDown.Text, Budget.Category.CategoryType.Expense);
                catsList.Add(cat);
                CategoriesDropDown.Text = "";
            }
        }
        private void Amount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        /// <summary>
        /// Shows errors from string.
        /// </summary>
        /// <param name="msg">String value of error message.</param>
        public void ShowError(string msg)
        {
            MessageBox.Show(msg);
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
        /// Shows latest added entry.
        /// </summary>
        /// <param name="desc">String description of entry.</param>
        public void ShowUpdated(string desc)
        {
            MessageBox.Show("Added " + desc, "Configuration", MessageBoxButton.OK, MessageBoxImage.Information);

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
        
    }
}
