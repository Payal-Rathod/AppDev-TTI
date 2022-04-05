using System;
using System.Collections.Generic;
using System.IO;
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
        string fileName;
        bool newDb;
        Category window;
        List<Budget.Category> catsList;

        private List<Budget.Category> categoriesList = new List<Budget.Category>();

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            presenter = new Presenter(this);
          
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
                ShowError("Please select a date!");
            }

            else if (Amount.Text == "")
            {
                ShowError("Please enter an amount!");
            }

            else if (Desc.Text == "")
            {
                ShowError("Please enter a description!");
            }

            else if (CategoriesDropDown.SelectedItem == null)
            {
                ShowError("Please enter a category from the list, or create a new one!");
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

                ShowAdded(date, amount, desc, category);

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
                fileName = FileNameTextBox.Text;
            }

            if (new FileInfo(fileName).Length == 0)
            {
                newDb = true;
            }
            else
            {
                newDb = false;
            }

            presenter.openDatabase(fileName, newDb);
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

        public void ShowAdded(DateTime date, int amount, string desc, string category)
        {
            MessageBox.Show(date.ToString("yyyy-MM-dd") + "\n" + amount + "\n" + desc + "\n" + category);
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
            MessageBox.Show("Error! " +msg);
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
            if (CategoriesDropDown.SelectedIndex != -1)
                desc.Content = selectedItem.Description;
        }

        // Closing the application.
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("If you close this window without adding, your changes will be lost.\nSave?", "Add expense", MessageBoxButton.YesNo, MessageBoxImage.Hand) == MessageBoxResult.Yes)
            {

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

        private void newFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (DialogResult.HasValue == dialog.ShowDialog())
            {
                fileName = dialog.FileName;
            }
        }
        private void cancelExpenses_Click(object sender, RoutedEventArgs e)
        {
            Cancel();
        }
    }
}
