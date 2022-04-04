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
            List<Budget.Category> catsList = presenter.getCategoriesList();
            foreach (Budget.Category c in catsList)
            {
                CategoriesDropDown.Items.Add(c);
            }
        }

        private void AddExpenses_Click(object sender, RoutedEventArgs e)
        {
            DateTime date = DateTimePicker1.SelectedDate.Value;

            int amount = Int32.Parse(Amount.Text);

            string desc = Desc.Text;

            string category = CategoriesDropDown.SelectedItem.ToString().Split(':')[1];

            presenter.addExpenses(date, category, amount, desc);

            MessageBox.Show(date.ToString("yyyy-MM-dd") +"\n"+ amount + "\n" + desc + "\n" + category);



        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public void CloseFile()
        {
            throw new NotImplementedException();
        }

        public void OpenFile()
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
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
            throw new NotImplementedException();
        }

        public void ShowAdded()
        {
            throw new NotImplementedException();
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
            Category window = new Category();
            window.Show();

        }

        private void Amount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void CategoriesDropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = CategoriesDropDown.SelectedIndex;
            Budget.Category selectedItem = CategoriesDropDown.SelectedItem as Budget.Category;
            ComboBoxItem desc = new ComboBoxItem();
            desc.Content = selectedItem.Description;
            

            ComboBoxItem cbi = desc;

            string selectedtext = cbi.Content.ToString();
        }
    }
}
