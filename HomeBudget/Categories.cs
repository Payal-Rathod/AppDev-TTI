using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Budget
{
    // ====================================================================
    // CLASS: categories
    //        - A collection of category items,
    //        - Read / write to file
    //        - etc
    // ====================================================================

    /// <summary>
    /// Categories class is used to manage a list of Category objects.
    /// </summary>
    public class Categories
    {
        private static String DefaultFileName = "budgetCategories.txt";
        private List<Category> _Cats = new List<Category>();
        private string _FileName;
        private string _DirName;

        // ====================================================================
        // Properties
        // ====================================================================

        /// <summary>
        /// Property to store and access the file name.
        /// </summary>
        /// <value>The <c>FileName</c> property represents the name of the file.</value>
        public String FileName { get { return _FileName; } }

        /// <summary>
        /// Property to store and access the directory name.
        /// </summary>
        /// <value>The <c>DirName</c> property represents the name of the directory.</value>
        public String DirName { get { return _DirName; } }

        // ====================================================================
        // Constructor
        // ====================================================================

        /// <summary>
        /// Default constructor that calls the <see cref="SetCategoriesToDefaults"/> method to add categories to a list.
        /// </summary>
        public Categories()
        {
            SetCategoriesToDefaults();
        }

        // ====================================================================
        // get a specific category from the list where the id is the one specified
        // ====================================================================

        /// <summary>
        /// Searches for a category of a specific ID and returns the category of that id. This method is used to get info about a category according to its ID and 
        /// this is possible by searching for an element that matches the ID and returns the first occurrence of the match. Before calling this method, it is 
        /// necessary to already have a category list from which the id must match. 
        /// </summary>
        /// <param name="i">The number of the ID of the category to find</param>
        /// <returns>The first occurence of a matching ID number</returns>
        /// <exception cref="Exception">Thrown when the ID number given in the parameter cannot be found from the list of categories</exception>
        public Category GetCategoryFromId(int i)
        {
            Category c = _Cats.Find(x => x.Id == i);
            if (c == null)
            {
                throw new Exception("Cannot find category with id " + i.ToString());
            }
            return c;
        }

        // ====================================================================
        // populate categories from a file
        // if filepath is not specified, read/save in AppData file
        // Throws System.IO.FileNotFoundException if file does not exist
        // Throws System.Exception if cannot read the file correctly (parsing XML)
        // ====================================================================

        /// <summary>
        /// Reads from a file if it exists and populates the categories accordingly. This method is used to stores values for properties of Category by 
        /// reading its values from a file. This is possible with a method which makes the file an XML document to be able to read and 
        /// add data to the Category properties.  This method will also save the directory and file name for later usage. 
        /// The file path, if any, needs to be valid in order to read from it.
        /// </summary>
        /// <param name="filepath">The file path from which the method will read from. If it is not provided (null), its value will be set by default.</param>
        /// <exception cref="FileNotFoundException">Thrown when <c>FilePath</c> does not exist. Exception in <see cref="BudgetFiles.VerifyReadFromFileName"/> method</exception>
        /// <exception cref="Exception">thrown when there was an error reading from the XML file.</exception>
        /// <example>
        /// The example shown below shows the usage of this method: 
        /// 
        /// <code>
        /// <![CDATA[
        /// Categories c = new Categories();
        /// 
        /// 
        /// //Calls method to read from filepath and store data in the categories property.
        /// c.ReadFromFile(String filepath);
        /// 
        /// //Loops through the list of Category that was populated by the previous method and outputs the description of each category to the console.
        /// foreach (Category category in c.List()){
        ///     Console.WriteLine(category.description);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public void ReadFromFile(String filepath = null)
        {

            // ---------------------------------------------------------------
            // reading from file resets all the current categories,
            // ---------------------------------------------------------------
            _Cats.Clear();

            // ---------------------------------------------------------------
            // reset default dir/filename to null 
            // ... filepath may not be valid, 
            // ---------------------------------------------------------------
            _DirName = null;
            _FileName = null;

            // ---------------------------------------------------------------
            // get filepath name (throws exception if it doesn't exist)
            // ---------------------------------------------------------------
            filepath = BudgetFiles.VerifyReadFromFileName(filepath, DefaultFileName);

            // ---------------------------------------------------------------
            // If file exists, read it
            // ---------------------------------------------------------------
            _ReadXMLFile(filepath);
            _DirName = Path.GetDirectoryName(filepath);
            _FileName = Path.GetFileName(filepath);
        }

        // ====================================================================
        // save to a file
        // if filepath is not specified, read/save in AppData file
        // ====================================================================

        /// <summary>
        /// Saves the Category objects and properties by writing them to a file. This method is used to save the Category objects from a list to a file.
        /// This is possible by evaluating every Category object properties from a list and writing them to a XML file. It is necessary for the file path, if any, given in the 
        /// parameter to be valid in order to write to it. Also, the Category list should not be empty since no data will be saved in the file.
        /// </summary>
        /// <param name="filepath">The file path from which the method will write to. If it is not provided (null), its value will be set by default.</param>
        /// <exception cref="Exception">Thrown when <c>FilePath</c> does not exist or when <c>FilePath</c> is read-only meaning that you cannot write in the file. Exception in 
        /// <see cref="BudgetFiles.VerifyWriteToFileName"/> method.</exception>
        /// <example>
        /// The example shown below shows the usage of this method: 
        /// 
        /// <code>
        /// <![CDATA[
        /// Categories c = new Categories();
        /// 
        /// //Calls method to write to file in filepath.
        /// c.SaveToFile(String filepath);
        /// 
        /// //Calls method to read from filepath and store data in the categories property to be able to see the output gotten from the previous method.
        /// c.ReadFromFile(String filepath);
        /// 
        /// //Loops through the list of Category that was populated by the previous method and outputs the description of each category to the console.
        /// foreach (Category category in c.List()){
        ///     Console.WriteLine(category.description);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public void SaveToFile(String filepath = null)
        {
            // ---------------------------------------------------------------
            // if file path not specified, set to last read file
            // ---------------------------------------------------------------
            if (filepath == null && DirName != null && FileName != null)
            {
                filepath = DirName + "\\" + FileName;
            }

            // ---------------------------------------------------------------
            // just in case filepath doesn't exist, reset path info
            // ---------------------------------------------------------------
            _DirName = null;
            _FileName = null;

            // ---------------------------------------------------------------
            // get filepath name (throws exception if it doesn't exist)
            // ---------------------------------------------------------------
            filepath = BudgetFiles.VerifyWriteToFileName(filepath, DefaultFileName);

            // ---------------------------------------------------------------
            // save as XML
            // ---------------------------------------------------------------
            _WriteXMLFile(filepath);

            // ----------------------------------------------------------------
            // save filename info for later use
            // ----------------------------------------------------------------
            _DirName = Path.GetDirectoryName(filepath);
            _FileName = Path.GetFileName(filepath);
        }

        // ====================================================================
        // set categories to default
        // ====================================================================

        /// <summary>
        /// Sets the Category list to default by adding some Category objects to it. This is used to populate the Category list and it is possible to do so
        /// by adding each object to the list with the method <see cref="Add"/>. 
        /// </summary>
        /// <example>
        /// The example shown below shows the usage of this method: 
        /// 
        /// <code>
        /// <![CDATA[
        /// Categories c = new Categories();
        /// 
        /// //Loops through the list of Category that was populated by the constructor by default. 
        /// //Also outputs the description of each category to the console.
        /// foreach (Category category in c.List()){
        ///     Console.WriteLine(category.description);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public void SetCategoriesToDefaults()
        {
            // ---------------------------------------------------------------
            // reset any current categories,
            // ---------------------------------------------------------------
            _Cats.Clear();

            // ---------------------------------------------------------------
            // Add Defaults
            // ---------------------------------------------------------------
            Add("Utilities", Category.CategoryType.Expense);
            Add("Rent", Category.CategoryType.Expense);
            Add("Food", Category.CategoryType.Expense);
            Add("Entertainment", Category.CategoryType.Expense);
            Add("Education", Category.CategoryType.Expense);
            Add("Miscellaneous", Category.CategoryType.Expense);
            Add("Medical Expenses", Category.CategoryType.Expense);
            Add("Vacation", Category.CategoryType.Expense);
            Add("Credit Card", Category.CategoryType.Credit);
            Add("Clothes", Category.CategoryType.Expense);
            Add("Gifts", Category.CategoryType.Expense);
            Add("Insurance", Category.CategoryType.Expense);
            Add("Transportation", Category.CategoryType.Expense);
            Add("Eating Out", Category.CategoryType.Expense);
            Add("Savings", Category.CategoryType.Savings);
            Add("Income", Category.CategoryType.Income);

        }

        // ====================================================================
        // Add category
        // ====================================================================
        private void Add(Category cat)
        {
            _Cats.Add(cat);
        }

        /// <summary>
        /// Adds a new Category in the list of Category. The id number is generated by default.
        /// </summary>
        /// <param name="desc">The description of the category.</param>
        /// <param name="type">The type of the Category</param>
        ///         ///<example>
        /// The example shown below shows the usage of this method: 
        /// 
        /// <code>
        /// <![CDATA[
        /// Categories c = new Categories();
        /// 
        /// //Adds a new Category object to the list of Category.
        /// c.Add("Car", Category.CategoryType.Expense);
        /// 
        /// //Loops through the list of Category that was populated by the constructor and that had a category added to it. 
        /// //Also outputs the description of each category to the console.
        /// foreach (Category category in c.List()){
        ///     Console.WriteLine(category.description);
        /// }
        /// 
        /// ]]>
        /// </code>
        /// </example>

        public void Add(String desc, Category.CategoryType type)
        {

            int new_num = 1;
            if (_Cats.Count > 0)
            {
                new_num = (from c in _Cats select c.Id).Max();
                new_num++;
            }
            _Cats.Add(new Category(new_num, desc, type));
        }

        // ====================================================================
        // Delete category
        // ====================================================================

        /// <summary>
        /// Deletes the Category that matches the id given in the parameter.
        /// </summary>
        /// <param name="Id">The id number of the category that needs to be deleted.</param>
        ///<example>
        /// The example shown below shows the usage of this method: 
        /// 
        /// <code>
        /// <![CDATA[
        /// Categories c = new Categories();
        /// 
        /// //Removes a Category object from the list of Category.
        /// c.Delete(1);
        /// 
        /// //Loops through the list of Category that was populated by the constructor by default and that had a category removed from it.
        /// //Also outputs the description of each category to the console.
        /// foreach (Category category in c.List()){
        ///     Console.WriteLine(category.description);
        /// }
        /// 
        /// ]]>
        /// </code>
        /// </example>
        public void Delete(int Id)
        {
            int i = _Cats.FindIndex(x => x.Id == Id);
            _Cats.RemoveAt(i);
        }

        // ====================================================================
        // Return list of categories
        // Note:  make new copy of list, so user cannot modify what is part of
        //        this instance
        // ====================================================================

        /// <summary>
        /// Returns a list of Categories that contains Category objects.
        /// </summary>
        /// <returns>A list of Categories.</returns>
        ///<example>
        /// The example shown below shows the usage of this method: 
        /// 
        /// <code>
        /// <![CDATA[
        /// Categories c = new Categories();
        /// 
        /// //Loops through the list of Category that was populated by the constructor and outputs the description of each category to the console.
        /// foreach (Category category in c.List()){
        ///     Console.WriteLine(category.description);
        /// }
        /// 
        /// ]]>
        /// </code>
        /// </example>
        public List<Category> List()
        {
            List<Category> newList = new List<Category>();
            foreach (Category category in _Cats)
            {
                newList.Add(new Category(category));
            }
            return newList;
        }

        // ====================================================================
        // read from an XML file and add categories to our categories list
        // ====================================================================
        private void _ReadXMLFile(String filepath)
        {

            // ---------------------------------------------------------------
            // read the categories from the xml file, and add to this instance
            // ---------------------------------------------------------------
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filepath);

                foreach (XmlNode category in doc.DocumentElement.ChildNodes)
                {
                    String id = (((XmlElement)category).GetAttributeNode("ID")).InnerText;
                    String typestring = (((XmlElement)category).GetAttributeNode("type")).InnerText;
                    String desc = ((XmlElement)category).InnerText;

                    Category.CategoryType type;
                    switch (typestring.ToLower())
                    {
                        case "income":
                            type = Category.CategoryType.Income;
                            break;
                        case "expense":
                            type = Category.CategoryType.Expense;
                            break;
                        case "credit":
                            type = Category.CategoryType.Credit;
                            break;
                        default:
                            type = Category.CategoryType.Expense;
                            break;
                    }
                    this.Add(new Category(int.Parse(id), desc, type));
                }

            }
            catch (Exception e)
            {
                throw new Exception("ReadXMLFile: Reading XML " + e.Message);
            }

        }


        // ====================================================================
        // write all categories in our list to XML file
        // ====================================================================
        private void _WriteXMLFile(String filepath)
        {
            try
            {
                // create top level element of categories
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<Categories></Categories>");

                // foreach Category, create an new xml element
                foreach (Category cat in _Cats)
                {
                    XmlElement ele = doc.CreateElement("Category");
                    XmlAttribute attr = doc.CreateAttribute("ID");
                    attr.Value = cat.Id.ToString();
                    ele.SetAttributeNode(attr);
                    XmlAttribute type = doc.CreateAttribute("type");
                    type.Value = cat.Type.ToString();
                    ele.SetAttributeNode(type);

                    XmlText text = doc.CreateTextNode(cat.Description);
                    doc.DocumentElement.AppendChild(ele);
                    doc.DocumentElement.LastChild.AppendChild(text);

                }

                // write the xml to FilePath
                doc.Save(filepath);

            }
            catch (Exception e)
            {
                throw new Exception("_WriteXMLFile: Reading XML " + e.Message);
            }

        }

    }
}

