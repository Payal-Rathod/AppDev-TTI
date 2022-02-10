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
    // CLASS: expenses
    //        - A collection of expense items,
    //        - Read / write to file
    //        - etc
    // ====================================================================

    /// <summary>
    /// Expenses class that is used to manage a list of Expense objects.
    /// </summary>
    public class Expenses
    {
        private static String DefaultFileName = "budget.txt";
        private List<Expense> _Expenses = new List<Expense>();
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
        // populate categories from a file
        // if filepath is not specified, read/save in AppData file
        // Throws System.IO.FileNotFoundException if file does not exist
        // Throws System.Exception if cannot read the file correctly (parsing XML)
        // ====================================================================

        /// <summary>
        /// Reads from a file if it exists and populates the expenses accordingly. This method is used to stores values for properties of Expense by 
        /// reading its values from a file. This is possible with a method which makes the file an XML document to be able to read and 
        /// add data to the Expense properties. This method will also save the directory and file name for later usage. 
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
        /// Expenses e = new Expenses();
        /// 
        /// 
        /// //Calls method to read from filepath and store data in the expenses property.
        /// e.ReadFromFile(String filepath);
        /// 
        /// //Loops through the list of Expense that was populated by the previous method and outputs the description of each expense to the console.
        /// foreach (Expense expense in e.List()){
        ///     Console.WriteLine(expense.description);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public void ReadFromFile(String filepath = null)
        {

            // ---------------------------------------------------------------
            // reading from file resets all the current expenses,
            // so clear out any old definitions
            // ---------------------------------------------------------------
            _Expenses.Clear();

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
            // read the expenses from the xml file
            // ---------------------------------------------------------------
            _ReadXMLFile(filepath);

            // ----------------------------------------------------------------
            // save filename info for later use?
            // ----------------------------------------------------------------
            _DirName = Path.GetDirectoryName(filepath);
            _FileName = Path.GetFileName(filepath);


        }

        // ====================================================================
        // save to a file
        // if filepath is not specified, read/save in AppData file
        // ====================================================================

        /// <summary>
        /// Saves the Expense objects and properties by writing them to a file. This method is used to save the Expense objects from a list to a file.
        /// This is possible by evaluating every Expense object properties from a list and writing them to a XML file. It is necessary for the file path, if any, given in the 
        /// parameter to be valid in order to write to it. Also, the Expense list should not be empty since no data will be saved in the file.
        /// </summary>
        /// <param name="filepath">The file path from which the method will write to. If it is not provided (null), its value will be set by default.</param>
        /// <exception cref="Exception">Thrown when <c>FilePath</c> does not exist or when <c>FilePath</c> is read-only meaning that you cannot write in the file. Exception in 
        /// <see cref="BudgetFiles.VerifyWriteToFileName"/> method.</exception>
        /// <example>
        /// The example shown below shows the usage of this method: 
        /// 
        /// <code>
        /// <![CDATA[
        /// Expenses e = new Expenses();
        /// 
        /// //Calls method to write to file in filepath.
        /// e.SaveToFile(String filepath);
        /// 
        /// //Calls method to read from filepath and store data in the expenses property to be able to see the output gotten from the previous method.
        /// e.ReadFromFile(String filepath);
        /// 
        /// //Loops through the list of Expense that was populated by the previous method and outputs the description of each expense to the console.
        /// foreach (Expense expense in e.List()){
        ///     Console.WriteLine(expense.description);
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
        // Add expense
        // ====================================================================
        private void Add(Expense exp)
        {
            _Expenses.Add(exp);
        }

        /// <summary>
        /// Adds a new Expense object in the list of Expense. The id number is generated by default.
        /// </summary>
        /// <param name="date">The date of the expense.</param>
        /// <param name="category">The category of the expense.</param>
        /// <param name="amount">The amount of the expense.</param>
        /// <param name="description">The description of the expense.</param>
        ///<example>
        /// The example shown below shows the usage of this method: 
        /// 
        /// <code>
        /// <![CDATA[
        /// Expenses e = new Expenses();
        /// 
        /// //Adds a new Expense object to the list of Expense.
        /// e.Add(1/10/2018 12:00:00 AM, 10, 10, "hat (on credit)");
        /// 
        /// //Loops through the list of Expense that was populated by the constructor and that had a expense added to it. 
        /// //Also outputs the description of each expense to the console.
        /// foreach (Expense expense in e.List()){
        ///     Console.WriteLine(enxpense.description);
        /// }
        /// 
        /// ]]>
        /// </code>
        /// </example>
        public void Add(DateTime date, int category, Double amount, String description)
        {
            int new_id = 1;

            // if we already have expenses, set ID to max
            if (_Expenses.Count > 0)
            {
                new_id = (from e in _Expenses select e.Id).Max();
                new_id++;
            }

            _Expenses.Add(new Expense(new_id, date, category, amount, description));

        }

        // ====================================================================
        // Delete expense
        // ====================================================================

        /// <summary>
        /// Deletes the Expense that matches the id given in the parameter.
        /// </summary>
        /// <param name="Id">The id number of the expense that needs to be deleted.</param>
        ///<example>
        /// The example shown below shows the usage of this method: 
        /// 
        /// <code>
        /// <![CDATA[
        /// Expenses e = new Expenses();
        /// 
        /// //Removes a Category object from the list of Category.
        /// e.Delete(1);
        /// 
        /// //Loops through the list of Expense that was populated by the constructor by default and that had a expense removed from it.
        /// //Also outputs the description of each expense to the console.
        /// foreach (Expense expense in e.List()){
        ///     Console.WriteLine(expense.description);
        /// }
        /// 
        /// ]]>
        /// </code>
        /// </example>
        public void Delete(int Id)
        {
            int i = _Expenses.FindIndex(x => x.Id == Id);
            _Expenses.RemoveAt(i);

        }

        // ====================================================================
        // Return list of expenses
        // Note:  make new copy of list, so user cannot modify what is part of
        //        this instance
        // ====================================================================

        /// <summary>
        /// Returns a list of Expenses that contains Expense objects.
        /// </summary>
        /// <returns>A list of Expense Objects.</returns>
        ///<example>
        /// The example shown below shows the usage of this method: 
        /// 
        /// <code>
        /// <![CDATA[
        /// Expenses e = new Expenses();
        /// 
        /// //Loops through the list of Expense that was populated by the constructor and outputs the description of each expense to the console.
        /// foreach (Expense expense in e.List()){
        ///     Console.WriteLine(expense.description);
        /// }
        /// 
        /// ]]>
        /// </code>
        /// </example>
        public List<Expense> List()
        {
            List<Expense> newList = new List<Expense>();
            foreach (Expense expense in _Expenses)
            {
                newList.Add(new Expense(expense));
            }
            return newList;
        }


        // ====================================================================
        // read from an XML file and add categories to our categories list
        // ====================================================================
        private void _ReadXMLFile(String filepath)
        {


            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filepath);

                // Loop over each Expense
                foreach (XmlNode expense in doc.DocumentElement.ChildNodes)
                {
                    // set default expense parameters
                    int id = int.Parse((((XmlElement)expense).GetAttributeNode("ID")).InnerText);
                    String description = "";
                    DateTime date = DateTime.Parse("2000-01-01");
                    int category = 0;
                    Double amount = 0.0;

                    // get expense parameters
                    foreach (XmlNode info in expense.ChildNodes)
                    {
                        switch (info.Name)
                        {
                            case "Date":
                                date = DateTime.Parse(info.InnerText);
                                break;
                            case "Amount":
                                amount = Double.Parse(info.InnerText);
                                break;
                            case "Description":
                                description = info.InnerText;
                                break;
                            case "Category":
                                category = int.Parse(info.InnerText);
                                break;
                        }
                    }

                    // have all info for expense, so create new one
                    this.Add(new Expense(id, date, category, amount, description));

                }

            }
            catch (Exception e)
            {
                throw new Exception("ReadFromFileException: Reading XML " + e.Message);
            }
        }


        // ====================================================================
        // write to an XML file
        // if filepath is not specified, read/save in AppData file
        // ====================================================================
        private void _WriteXMLFile(String filepath)
        {
            // ---------------------------------------------------------------
            // loop over all categories and write them out as XML
            // ---------------------------------------------------------------
            try
            {
                // create top level element of expenses
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<Expenses></Expenses>");

                // foreach Category, create an new xml element
                foreach (Expense exp in _Expenses)
                {
                    // main element 'Expense' with attribute ID
                    XmlElement ele = doc.CreateElement("Expense");
                    XmlAttribute attr = doc.CreateAttribute("ID");
                    attr.Value = exp.Id.ToString();
                    ele.SetAttributeNode(attr);
                    doc.DocumentElement.AppendChild(ele);

                    // child attributes (date, description, amount, category)
                    XmlElement d = doc.CreateElement("Date");
                    XmlText dText = doc.CreateTextNode(exp.Date.ToString());
                    ele.AppendChild(d);
                    d.AppendChild(dText);

                    XmlElement de = doc.CreateElement("Description");
                    XmlText deText = doc.CreateTextNode(exp.Description);
                    ele.AppendChild(de);
                    de.AppendChild(deText);

                    XmlElement a = doc.CreateElement("Amount");
                    XmlText aText = doc.CreateTextNode(exp.Amount.ToString());
                    ele.AppendChild(a);
                    a.AppendChild(aText);

                    XmlElement c = doc.CreateElement("Category");
                    XmlText cText = doc.CreateTextNode(exp.Category.ToString());
                    ele.AppendChild(c);
                    c.AppendChild(cText);

                }

                // write the xml to FilePath
                doc.Save(filepath);

            }
            catch (Exception e)
            {
                throw new Exception("SaveToFileException: Reading XML " + e.Message);
            }
        }

    }
}

