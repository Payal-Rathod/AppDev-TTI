using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private List<Budget.Category.CategoryType> categoriesList = new List<Budget.Category.CategoryType>();

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


           // Budget.HomeBudget homeBudget = new Budget.HomeBudget(openFileDlg.FileName);
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
    }
}
