using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
        List<Budget.Category> catsList;

        private List<Budget.Category> categoriesList = new List<Budget.Category>();

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            presenter = new Presenter(this);
            DateTimePicker1.SelectedDate = DateTime.Today;

            Application.Current.MainWindow.FontFamily = new FontFamily("Cambria");

        }
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();

            catsList = presenter.getCategoriesList();
            foreach (Budget.Category c in catsList)
            {
                CategoriesDropDown.Items.Add(c);
            }
            CategoriesDropDown.DisplayMemberPath = "Description";
        }
        public void OpenFile()
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Filter = "Database file (.db)|*.db";
            Nullable<bool> result = openFileDlg.ShowDialog();
            if (result == true)
            {
                ShowDatabase(System.IO.Path.GetFileName(openFileDlg.FileName));
                fileName = openFileDlg.FileName;
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

                presenter.AddExpense(date, index, amount, desc);
            }
        }
        public void Cancel()
        {
            MessageBox.Show("Your entries will be cleared.", "Configuration", MessageBoxButton.OK, MessageBoxImage.Warning);

            // Clear fields.
            DateTimePicker1.SelectedDate = null;
            Amount.Text = "Amount";
            Desc.Text = "Description";
            CategoriesDropDown.SelectedItem = null;
        }

        public void CloseFile()
        {
            throw new NotImplementedException();
        }

        public void RecentlyOpened()
        {
            throw new NotImplementedException();
        }
        public void DisableBtnAndInput()
        {
            addExpense_btn.IsEnabled = false;
            cancelExpense_btn.IsEnabled = false;
            Desc.IsEnabled = false;
            Amount.IsEnabled = false;
            CategoriesDropDown.IsEnabled = false;
            DateTimePicker1.IsEnabled = false;
        }
        public void EnableBtnAndInput()
        {
            addExpense_btn.IsEnabled = true;
            cancelExpense_btn.IsEnabled = true;
            Desc.IsEnabled = true;
            Amount.IsEnabled = true;
            CategoriesDropDown.IsEnabled = true;
            DateTimePicker1.IsEnabled = true;
        }

        public void Refresh()
        {
            // Clear fields except Date and Category.
            Amount.Text = "Amount";
            Desc.Text = "Description";
        }

        public void ShowAdded(string desc)
        {
            MessageBox.Show("Added " + desc, "Configuration", MessageBoxButton.OK, MessageBoxImage.Information);

        }
        public void ShowDatabase(string fileName)
        {
            FileNameTextBox.Text = fileName;
        }

        public void ShowError(string msg)
        {
            MessageBox.Show(msg);
        }

        public void ShowUserHistory()
        {
            throw new NotImplementedException();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
        private void CancelExpenses_Click(object sender, RoutedEventArgs e)
        {
            presenter.ClearFields();
        }
        private void ColorMode_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            presenter.ChangeColorMode(btn.Content.ToString());
        }

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

            Amount.Background = Brushes.White;
            Desc.Background = Brushes.White;
            DateTimePicker1.Background = Brushes.White;
            Header.Foreground = blueBrush;
            Header.Foreground = blueBrush;

            addExpense_btn.Background = blueBrush;
            addExpense_btn.Foreground = Brushes.White;

            cancelExpense_btn.Background = blueBrush;
            cancelExpense_btn.Foreground = Brushes.White;

            DateTimePicker1.BorderBrush = blueBrush;
        }

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

            Amount.Background = Brushes.DarkGray;
            Desc.Background = Brushes.DarkGray;
            DateTimePicker1.Background = Brushes.DarkGray;
            Header.Foreground = brush;

            addExpense_btn.Background = brush;
            addExpense_btn.Foreground = Brushes.DarkGray;

            cancelExpense_btn.Background = brush;
            cancelExpense_btn.Foreground = Brushes.DarkGray;

            addExpense_btn.BorderBrush = brush;
            cancelExpense_btn.BorderBrush = brush;

            DateTimePicker1.BorderBrush = brush;
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
                CategoriesDropDown.Items.Add(cat);
                CategoriesDropDown.Text = "";
            }
        }
    }
}
