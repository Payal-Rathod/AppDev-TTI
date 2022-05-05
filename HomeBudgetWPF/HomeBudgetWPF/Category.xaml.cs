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
    public partial class Category : Window, AddCategoryInterface
    {
        CategoryPresenter presenter;
        ComboBox categoriesDropDown;
        string windowColorMode;

        /// <summary>
        /// Gets category, filename and categoriedropdown values and initializes window
        /// </summary>
        /// <param name="category">The ategory that was typed in the drop down list</param>
        /// <param name="filename">The filepath of the database</param>
        /// <param name="catDropDown">The drop down list of categories in expenses window</param>
        public Category(string category, string filename, ComboBox catDropDown, object colorMode)
        {
            InitializeComponent();

            windowColorMode = colorMode as string;

            presenter = new CategoryPresenter(this, filename);
            presenter.openDatabase(filename);


            categoryName.Text = category;
            categoriesDropDown = catDropDown;

            // Adds all category types to the drop down list
            foreach (Budget.Category.CategoryType type in Enum.GetValues(typeof(Budget.Category.CategoryType)))
            {
                categoryType.Items.Add(type);
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

        private void Add_Category_Click(object sender, RoutedEventArgs e)
        {
            // We display the added category in drop down list.
            // We add it to categories list.

            Budget.Category.CategoryType tmp;
            Budget.Category category;

            // Validation.
            if (categoryName.Text == "" || categoryName.Text == "Enter name")
            {
               ShowError("Please enter a description!");
            }

            else if (categoryName.Text.Any(char.IsDigit))
            {
                ShowError("Your description contains a number!");
            }
            else if (categoryType.SelectedIndex == -1)
            {
                ShowError("Please select a category type!");
            }

            else
            {
                // Get type.
                if (Enum.TryParse(categoryType.Text, out tmp))
                {
                    category = presenter.AddCategory(categoryName.Text, tmp);

                }

                //Updates drop down list in previous window.
                categoriesDropDown.ItemsSource = presenter.getCategoriesList();

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