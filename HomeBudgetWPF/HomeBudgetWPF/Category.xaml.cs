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
using System.Windows.Shapes;

namespace HomeBudgetWPF
{
    /// <summary>
    /// Interaction logic for Category.xaml
    /// </summary>
    public partial class Category : Window
    {
        Presenter presenter;
        MainWindow mainWindow;
        public Category(Presenter mainPresenter, MainWindow main)
        {
            InitializeComponent();
            mainWindow = main;
            presenter = mainPresenter;

            // Dropdown for types.
            foreach (Budget.Category.CategoryType type in Enum.GetValues(typeof(Budget.Category.CategoryType)))
            {
                categoryType.Items.Add(type);
            }
        }

        private void Add_Category_Click(object sender, RoutedEventArgs e)
        {
            // We display the added category in drop down list.
            // We add it to categories list.

            Budget.Category.CategoryType tmp;
            Budget.Category category;

            // Validation.
            if (categoryName.Text == "" || categoryName.Text == "Enter name")
            {
                MessageBox.Show("Please enter a description!");
            }

            else if (categoryName.Text.Any(char.IsDigit))
            {
                MessageBox.Show("Your description contains a number!");
            }
            else if (categoryType.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a category type!");
            }

            else
            {
                // Get type.
                foreach (Budget.Category.CategoryType type in Enum.GetValues(typeof(Budget.Category.CategoryType)))
                {
                    if (Enum.TryParse(categoryType.Text, out tmp))
                    {
                        category = presenter.addCategory(categoryName.Text, tmp);

                        // Updating.
                        mainWindow.CategoriesDropDown.Items.Add(category);

                    }
                }

                mainWindow.ShowAdded();
                this.Close();
            }
        }

        private void categoryName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (categoryName.Text == "Enter name")
            {
                categoryName.Text = "";
            }
        }
    }
}
