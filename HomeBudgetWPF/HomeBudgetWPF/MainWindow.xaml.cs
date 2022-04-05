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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Budget;
using Microsoft.Win32;

namespace HomeBudgetWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ViewInterface
    {
        Presenter presenter;
        string filename;
        bool newDb;
        Category window;
        List<Budget.Category> catsList;

        private List<Budget.Category> categoriesList = new List<Budget.Category>();

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            presenter = new Presenter(this);
           

            // NO BINDING.          

        }
        private void openFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
            catsList = presenter.getCategoriesList();
            foreach (Budget.Category c in catsList)
            {
                CategoriesDropDown.Items.Add(c);
            }
        }

        private void AddExpenses_Click(object sender, RoutedEventArgs e)
        {
            // Input validation.
            if (DateTimePicker1.SelectedDate.Value == null)
            {
                MessageBox.Show("Please select a date!");
            }

            else if (Amount.Text == "" || Amount.Text == "Amount")
            {
                MessageBox.Show("Please enter an amount!");
            }

            else if (Desc.Text == "" || Desc.Text == "Description")
            {
                MessageBox.Show("Please enter a description!");
            }

            else if (CategoriesDropDown.SelectedIndex == -1)
            {
                MessageBox.Show("Please enter a category from the list, or create a new one!");
            }

            else
            {
                DateTime date = DateTimePicker1.SelectedDate.Value;

                int amount = Int32.Parse(Amount.Text);

                string desc = Desc.Text;

                string category = CategoriesDropDown.SelectedItem.ToString();

                catsList = presenter.getCategoriesList();
                int index;

                for (int i = 0; i < catsList.Count(); i++)
                {
                    if (catsList[i].Description == category)
                    {
                        index = i;
                        presenter.addExpenses(date, index, amount, desc);
                    }
                }             

                MessageBox.Show(date.ToString("yyyy-MM-dd") + "\n" + amount + "\n" + desc + "\n" + category);

                // Clear fields except Category and Date.
                Refresh();          

            }
        }

        public void Cancel()
        {
            MessageBox.Show("Your entries will be cleared.", "Configuration", MessageBoxButton.OK, MessageBoxImage.Warning);

            // Clear fields.
            DateTimePicker1.SelectedDate = null;
            Amount.Text = "Enter amount";
            Desc.Text = "Enter description";
            CategoriesDropDown.SelectedItem = null;
        }

        public void CloseFile()
        {
            throw new NotImplementedException();
        }

        public void OpenFile()
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Filter = "Database file (.db)|*.db";
            Nullable<bool> result = openFileDlg.ShowDialog();
            if (result == true)
            {
                FileNameTextBox.Text = openFileDlg.FileName;
                filename = FileNameTextBox.Text;
            }

            presenter.openDatabase(filename, newDb = true);
        }

        public void RecentlyOpened()
        {
            throw new NotImplementedException();
        }

        public void Refresh()
        {
            // Clear fields except Date and Category.
            Amount.Text = "Enter amount";
            Desc.Text = "Enter description";
        }

        public void ShowAdded()
        {
            catsList = presenter.getCategoriesList();

            MessageBox.Show("Added " + catsList.Last(), "Configuration", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowCategories()
        {
            throw new NotImplementedException();
        }

        public void ShowDatabase()
        {
            throw new NotImplementedException();
        }

        public void ShowError(string msg)
        {
            throw new NotImplementedException();
        }

        public void ShowUserHistory()
        {
            throw new NotImplementedException();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Open_Category_Window(object sender, RoutedEventArgs e)
        {
            window = new Category(presenter, this);
            window.Show();

        }

        private void Amount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void CategoriesDropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Budget.Category selectedItem = CategoriesDropDown.SelectedItem as Budget.Category;
            ComboBoxItem desc = new ComboBoxItem();
            desc.Content = selectedItem.Description;
        }

        // Closing the application.
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("If you close this window without saving, your changes will be lost.\nSave?", "Save file", MessageBoxButton.YesNo, MessageBoxImage.Hand) == MessageBoxResult.Yes)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.DefaultExt = ".db";
                dlg.Filter = "Database file (.db)|*.db";

                if (dlg.ShowDialog() == true)
                {
                    string filename = dlg.FileName;
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

        private void Desc_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Desc.Text == "Description")
            {
                Desc.Text = "";
            }            
        }
    }
}
