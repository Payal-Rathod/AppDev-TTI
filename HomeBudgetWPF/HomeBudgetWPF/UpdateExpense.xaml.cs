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
    public partial class UpdateExpense : Window, AddExpenseInterface
    {
        List<Budget.Category> catsList;
        UpdatePresenter presenter;
        Budget.BudgetItem item;
        DataGrid myDataGrid;
        string filepath;
        ComboBox catsDropDown;
        Budget.BudgetItem selectedItemDataGrid;
        String windowColorMode;

        /// <summary>
        /// Gets selected item, datagrid, filename and categories drop down values and initializes window.
        /// </summary>
        /// <param name="selectedItem"></param>
        /// <param name="datagrid"></param>
        /// <param name="filename"></param>
        /// <param name="catdropdown"></param>
        public UpdateExpense(Budget.BudgetItem selectedItem, DataGrid datagrid, string filename, ComboBox catdropdown, object colorMode)
        {
            InitializeComponent();
            windowColorMode = colorMode as String;

            GenerateWindowColors();

            presenter = new UpdatePresenter(this, filename);
            filepath = filename;
            catsDropDown = catdropdown;

            catsList = presenter.getCategoriesList(); //gets categories list
            CategoriesDropDown.ItemsSource = catsList;  //Updates categories list

            item = selectedItem;
            myDataGrid = datagrid;
            selectedItemDataGrid = selectedItem;

            PopulateItemInForm();
        }

        public void GenerateWindowColors()
        {
            if (windowColorMode == "Light Mode")
            {
                Color color = (Color)ColorConverter.ConvertFromString("#0C6291");

                var brush = new SolidColorBrush(color);

                mainGridUpdateExpense.Background = Brushes.Black;

                Amount.Background = Brushes.DarkGray;
                Desc.Background = Brushes.DarkGray;
                DateTimePicker1.Background = Brushes.DarkGray;

                updateExpense_btn.Background = brush;
                updateExpense_btn.Foreground = Brushes.DarkGray;

                cancelExpense_btn.Background = brush;
                cancelExpense_btn.Foreground = Brushes.DarkGray;

                updateExpense_btn.BorderBrush = brush;
                cancelExpense_btn.BorderBrush = brush;

                DateTimePicker1.BorderBrush = brush;

                CategoriesDropDown.Background = brush;
                //categoriesDropDown.Foreground = Brushes.DarkGray;
            }
            else
            {
                Color color = (Color)ColorConverter.ConvertFromString("#C9E4E7");
                Color darkBlue = (Color)ColorConverter.ConvertFromString("#0C6291");

                mainGridUpdateExpense.Background = Brushes.White;

                var brush = new SolidColorBrush(color);
                var blueBrush = new SolidColorBrush(darkBlue);
                Amount.Background = Brushes.White;
                Desc.Background = Brushes.White;
                DateTimePicker1.Background = Brushes.White;

                updateExpense_btn.Background = blueBrush;
                updateExpense_btn.Foreground = Brushes.White;

                cancelExpense_btn.Background = blueBrush;
                cancelExpense_btn.Foreground = Brushes.White;

                DateTimePicker1.BorderBrush = blueBrush;

                CategoriesDropDown.Background = brush;
                //categoriesDropDown.Foreground = Brushes.DarkGray;
            }

        }

        /// <summary>
        /// Populates items in form with the existing info
        /// </summary>
        public void PopulateItemInForm()
        {
            Amount.Text = item.Amount.ToString();
            if (Amount.Text.Contains("-"))
                Amount.Text = Amount.Text.Split('-')[1];
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
            //myDataGrid.SelectedItem = item;
            int index = myDataGrid.SelectedIndex;

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

            catsDropDown.ItemsSource = presenter.getCategoriesList(); //updates drop down list

            myDataGrid.ItemsSource = presenter.GetBudgetItemsList(); //updates datagrid items

            myDataGrid.SelectedIndex = index;

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
                //New category window if new category is written in drop down list
                Category CategoryWindow = new Category(CategoriesDropDown.Text, filepath, CategoriesDropDown, windowColorMode);
                CategoryWindow.Show();
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

        /// <summary>
        /// Shows message that expense was updated.
        /// </summary>
        /// <param name="desc">The updated expense's description</param>
        public void ShowAdded(string desc)
        {
            MessageBox.Show(desc + " updated");
        }
    }
}
