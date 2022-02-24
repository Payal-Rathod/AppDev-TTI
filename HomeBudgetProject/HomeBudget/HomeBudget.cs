using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Dynamic;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================


namespace Budget
{
    // ====================================================================
    // CLASS: HomeBudget
    //        - Combines categories Class and expenses Class
    //        - One File defines Category and Budget File
    //        - etc
    // ====================================================================

    /// <summary>
    /// Homebudget Class used to manage Categories and Expenses.
    /// </summary>
    public class HomeBudget
    {
        private string _FileName;
        private string _DirName;
        private Categories _categories;
        private Expenses _expenses;

        // ====================================================================
        // Properties
        // ===================================================================

        // Properties (location of files etc)

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

        /// <summary>
        /// Getter property that returns a path which includes a directory name and file name. If either of those 2 properties are null,
        /// this property will also return null.
        /// </summary>
        /// <value>The <c>PathName</c> property represents a filepath according to the file name and directory name</value>
        public String PathName
        {
            get
            {
                if (_FileName != null && _DirName != null)
                {
                    return Path.GetFullPath(_DirName + "\\" + _FileName);
                }
                else
                {
                    return null;
                }
            }
        }

        // Properties (categories and expenses object)

        /// <summary>
        /// Getter porperty that returns a Categories object.
        /// </summary>
        /// <value>The <c>categories</c> property represents an instance of the class <c>Categories</c></value>
        /// <remarks>The type value of this property is another class <see cref="Categories"/>.</remarks>
        public Categories categories { get { return _categories; } }

        /// <summary>
        /// Getter porperty that returns an Expenses object.
        /// </summary>
        /// <value>The <c>expenses</c> property represents an instance of the class <c>Expenses</c></value>
        /// <remarks>The type value of this property is another class <see cref="Expenses"/>.</remarks>
        public Expenses expenses { get { return _expenses; } }

        // -------------------------------------------------------------------
        // Constructor (new... default categories, no expenses)
        // -------------------------------------------------------------------

        // -------------------------------------------------------------------
        // Constructor (existing budget ... must specify file)
        // -------------------------------------------------------------------
        /// <summary>
        /// Constructor that initializes the private data fields and reads from a file received as parameter.
        /// </summary>
        /// <param name="budgetFileName">Budget file to read data from.</param>
        /// <exception cref="FileNotFoundException">Thrown when the file path does not exist. Exception thrown in <see cref="BudgetFiles.VerifyReadFromFileName"/>.</exception>
        /// <remarks>The constructor uses the <see cref="ReadFromFile"/> method to read the file</remarks>
        public HomeBudget(String budgetFileName)
        {
            _categories = new Categories();
            _expenses = new Expenses();
            ReadFromFile(budgetFileName);
        }

        public HomeBudget(String databaseFile, String expensesXMLFile, bool newDB = false)
        {
            // if database exists, and user doesn't want a new database, open existing DB
            if (!newDB && File.Exists(databaseFile))
            {
                Database.existingDatabase(databaseFile);
            }
            // file did not exist, or user wants a new database, so open NEW DB
            else
            {
                Database.newDatabase(databaseFile);
                newDB = true;
            }
            // create the category object
            _categories = new Categories(Database.dbConnection, newDB);
            // create the _expenses course
            _expenses = new Expenses();
            _expenses.ReadFromFile(expensesXMLFile);
        }

        #region OpenNewAndSave
        // ---------------------------------------------------------------
        // Read
        // Throws Exception if any problem reading this file
        // ---------------------------------------------------------------

        /// <summary>
        /// Reads from a file if it exists and stores the information from the files to the Categories and Expenses lists. This is possible by veryfing the existence of the file
        /// with the <see cref="BudgetFiles.VerifyReadFromFileName"/> method, reading the file, getting the path of the filenames, and reading from the file with the
        /// <see cref="Categories.ReadFromFile"/> method and <see cref="Expenses.ReadFromFile"/> method. This method also saves the directory and file name for later usage.
        /// </summary>
        /// <param name="budgetFileName">File name that will be read from</param>
        /// <exception cref="Exception">Thrown when there was an error reading the budget info</exception>
        /// <exception cref="FileNotFoundException">Thrown when file path does not exist. Exception thrown in <see cref="BudgetFiles.VerifyReadFromFileName"/> method.</exception>
        /// <exception cref="Exception">thrown when there was an error reading from the XML file. Exception thrown in <see cref="Categories.ReadFromFile"/> and <see cref="Expenses.ReadFromFile"/>.</exception>
        /// <example>
        /// For all examples below, assume the budgetFileName contains the following elements:
        /// 
        ///<code>
        ///test_categories.cats
        ///test_expenses.exps
        ///</code>
        /// <b>Reading from the BugdetFileName</b>
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget();
        /// 
        /// //Reads from filename which has the two file namess
        /// budget.ReadFromFile(filename);
        /// ]]>
        /// </code>
        /// </example>
        public void ReadFromFile(String budgetFileName)
        {
            // ---------------------------------------------------------------
            // read the budget file and process
            // ---------------------------------------------------------------
            try
            {
                // get filepath name (throws exception if it doesn't exist)
                budgetFileName = BudgetFiles.VerifyReadFromFileName(budgetFileName, "");
                
                // If file exists, read it
                string[] filenames = System.IO.File.ReadAllLines(budgetFileName);

                // ----------------------------------------------------------------
                // Save information about budget file
                // ----------------------------------------------------------------
                string folder = Path.GetDirectoryName(budgetFileName);
                _FileName = Path.GetFileName(budgetFileName);

                // read the expenses and categories from their respective files
                //_categories.ReadFromFile(folder + "\\" + filenames[0]);
                _expenses.ReadFromFile(folder + "\\" + filenames[1]);

                // Save information about budget file
                _DirName = Path.GetDirectoryName(budgetFileName);
                _FileName = Path.GetFileName(budgetFileName);
            }

            // ----------------------------------------------------------------
            // throw new exception if we cannot get the info that we need
            // ----------------------------------------------------------------
            catch (Exception e)
            {
                throw new Exception("Could not read budget info: \n" + e.Message);
            }

        }

        // ====================================================================
        // save to a file
        // saves the following files:
        //  filepath_expenses.exps  # expenses file
        //  filepath_categories.cats # categories files
        //  filepath # a file containing the names of the expenses and categories files.
        //  Throws exception if we cannot write to that file (ex: invalid dir, wrong permissions)
        // ====================================================================

        /// <summary>
        /// Writes to a file if it exists, constructs file names for expenses and categories, saves categories and expenses list to a file each and saves the files to the budget file.
        /// This is possible by verifying if you can write to the file with the <see cref="BudgetFiles.VerifyWriteToFileName"/> method, creating file names for expenses and categories in the
        /// filepath, saving the files to those filepaths with the <see cref="Expenses.SaveToFile"/> and <see cref="Categories.SaveToFile"/> methods and writing the file names in the budget file.
        /// This method also saves the directory and file name for later usage.
        /// </summary>
        /// <param name="filepath">File path for budget file.</param>
        /// <exception cref="Exception">Thrown when <c>filepath</c> does not exist or when <c>filepath</c> is read-only meaning that you cannot write in the file. Thrown in <see cref="BudgetFiles.VerifyWriteToFileName"/> method.</exception>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget();
        /// 
        /// //Saves expenses and categories filenames in budget file.
        /// budget.SaveToFile(filename);
        /// ]]>
        /// </code>
        /// </example>
        public void SaveToFile(String filepath)
        {

            // ---------------------------------------------------------------
            // just in case filepath doesn't exist, reset path info
            // ---------------------------------------------------------------
            _DirName = null;
            _FileName = null;

            // ---------------------------------------------------------------
            // get filepath name (throws exception if we can't write to the file)
            // ---------------------------------------------------------------
            filepath = BudgetFiles.VerifyWriteToFileName(filepath, "");

            String path = Path.GetDirectoryName(Path.GetFullPath(filepath));
            String file = Path.GetFileNameWithoutExtension(filepath);
            String ext = Path.GetExtension(filepath);

            // ---------------------------------------------------------------
            // construct file names for expenses and categories
            // ---------------------------------------------------------------
            String expensepath = path + "\\" + file + "_expenses" + ".exps";
            String categorypath = path + "\\" + file + "_categories" + ".cats";

            // ---------------------------------------------------------------
            // save the expenses and budgets into their own files
            // ---------------------------------------------------------------
            _expenses.SaveToFile(expensepath);
           // _categories.SaveToFile(categorypath);

            // ---------------------------------------------------------------
            // save filenames of expenses and categories to budget file
            // ---------------------------------------------------------------
            string[] files = { Path.GetFileName(categorypath), Path.GetFileName(expensepath) };
            System.IO.File.WriteAllLines(filepath, files);

            // ----------------------------------------------------------------
            // save filename info for later use
            // ----------------------------------------------------------------
            _DirName = path;
            _FileName = Path.GetFileName(filepath);
        }
        #endregion OpenNewAndSave

        #region GetList



        // ============================================================================
        // Get all expenses list
        // NOTE: VERY IMPORTANT... budget amount is the negative of the expense amount
        // Reasoning: an expense of $15 is -$15 from your bank account.
        // ============================================================================

        /// <summary>
        /// Gets Budget items by querying a list of categories and expenses within the start and end datetime, finding the total costs and returning the items. This is possible by
        /// creating a combined list of expenses and categories with the <see cref="Categories.List"/> and <see cref="Expenses.List"/> methods within the datetime received, finding the
        /// total which is represented as the balance after every item on the list, and finally returning the items. This method is used for getting a list of items within a certain time in
        /// order of datetime while keeping track of the balance (total). 
        /// </summary>
        /// <param name="Start">Start of datetime where the list would begin.</param>
        /// <param name="End">End of datetime where the list would end.</param>
        /// <param name="FilterFlag">filter outs unwanted categories if it is true.</param>
        /// <param name="CategoryID">Id number of the category.</param>
        /// <returns>list of BudgetItems.</returns>
        /// <example>
        /// 
        /// For all examples below, assume the budget file contains the
        /// following elements:
        /// 
        /// <code>
        /// Cat_ID  Expense_ID  Date                    Description                    Cost
        ///    10       1       1/10/2018 12:00:00 AM   Clothes hat (on credit)         10
        ///     9       2       1/11/2018 12:00:00 AM   Credit Card hat                -10
        ///    10       3       1/10/2019 12:00:00 AM   Clothes scarf(on credit)        15
        ///     9       4       1/10/2020 12:00:00 AM   Credit Card scarf              -15
        ///    14       5       1/11/2020 12:00:00 AM   Eating Out McDonalds            45
        ///    14       7       1/12/2020 12:00:00 AM   Eating Out Wendys               25
        ///    14      10       2/1/2020 12:00:00 AM    Eating Out Pizza                33.33
        ///     9      13       2/10/2020 12:00:00 AM   Credit Card mittens            -15
        ///     9      12       2/25/2020 12:00:00 AM   Credit Card Hat                -25
        ///    14      11       2/27/2020 12:00:00 AM   Eating Out Pizza                33.33
        ///    14       9       7/11/2020 12:00:00 AM   Eating Out Cafeteria            11.11
        /// </code>
        /// 
        /// <b>Getting a list of ALL budget items.</b>
        /// 
        /// <code>
        /// <![CDATA[
        ///  HomeBudget budget = new HomeBudget();
        ///  
        /// //Reads from the budget file.
        ///  budget.ReadFromFile(filename);
        ///  
        ///  // Get a list of all budget items
        ///  var budgetItems = budget.GetBudgetItems(null, null, false, 0);
        ///            
        ///  // print important information
        ///  foreach (var bi in budgetItems)
        ///  {
        ///      Console.WRiteLine ( 
        ///          String.Format("{0} {1,-20}  {2,8:C} {3,12:C}", 
        ///             bi.Date.ToString("yyyy/MMM/dd"),
        ///             bi.ShortDescription,
        ///             bi.Amount, bi.Balance)
        ///       );
        ///  }
        ///
        /// ]]>
        /// </code>
        /// 
        /// Sample output containing the date, description, amount and balance:
        /// The list is sorted by datetime unlike before.
        /// <code>
        /// 2018-Jan-10 hat (on credit)        -$10.00      -$10.00
        /// 2018-Jan-11 hat                     $10.00        $0.00
        /// 2019-Jan-10 scarf(on credit)      -$15.00      -$15.00
        /// 2020-Jan-10 scarf                   $15.00        $0.00
        /// 2020-Jan-11 McDonalds              -$45.00      -$45.00
        /// 2020-Jan-12 Wendys                 -$25.00      -$70.00
        /// 2020-Feb-01 Pizza                  -$33.33     -$103.33
        /// 2020-Feb-10 mittens                 $15.00      -$88.33
        /// 2020-Feb-25 Hat                     $25.00      -$63.33
        /// 2020-Feb-27 Pizza                  -$33.33      -$96.66
        /// 2020-Jul-11 Cafeteria              -$11.11     -$107.77
        /// </code>
        /// </example>
        public List<BudgetItem> GetBudgetItems(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // ------------------------------------------------------------------------
            // return joined list within time frame
            // ------------------------------------------------------------------------
            Start = Start ?? new DateTime(1900, 1, 1);
            End = End ?? new DateTime(2500, 1, 1);

            var query =  from c in _categories.List()
                        join e in _expenses.List() on c.Id equals e.Category
                        where e.Date >= Start && e.Date <= End
                        select new { CatId = c.Id, ExpId = e.Id, e.Date, Category = c.Description, e.Description, Amount = -e.Amount};

            // ------------------------------------------------------------------------
            // create a BudgetItem list with totals,
            // ------------------------------------------------------------------------
            List<BudgetItem> items = new List<BudgetItem>();
            Double total = 0;

            foreach (var e in query.OrderBy(q => q.Date))
            {
                // filter out unwanted categories if filter flag is on
                if (FilterFlag && CategoryID != e.CatId)
                {
                    continue;
                }

                // keep track of running totals
                total = total - e.Amount;
                items.Add(new BudgetItem
                {
                    CategoryID = e.CatId,
                    ExpenseID = e.ExpId,
                    ShortDescription = e.Description,
                    Date = e.Date,
                    Amount = -e.Amount,
                    Category = e.Category,
                    Balance = total
                });
            }

            return items;
        }

        // ============================================================================
        // Group all expenses month by month (sorted by year/month)
        // returns a list of BudgetItemsByMonth which is 
        // "year/month", list of budget items, and total for that month
        // ============================================================================

        /// <summary>
        /// Gets Budget items by month by first getting the budget items, grouping them by month, determining the total and creating a new object with the info.
        /// This is possible by using the method <see cref="GetBudgetItems"/> first, then grouping all the items according to their month and year, finding the total for each item in the list
        /// according to the amount and creating a <see cref="BudgetItemsByMonth"/> object for each item to add in the summary which will be returned in the end.
        /// </summary>
        /// <param name="Start">Start of datetime where the list would begin.</param>
        /// <param name="End">End of datetime where the list would end.</param>
        /// <param name="FilterFlag">filter outs unwanted categories if it is true.</param>
        /// <param name="CategoryID">Id number of the category.</param>
        /// <returns>list of BudgetItems explaining the summary.</returns>
        /// <example>
        /// 
        /// For all examples below, assume the budget file contains the
        /// following elements:
        /// 
        /// <code>
        /// Cat_ID  Expense_ID  Date                    Description                    Cost
        ///    10       1       1/10/2018 12:00:00 AM   Clothes hat (on credit)         10
        ///     9       2       1/11/2018 12:00:00 AM   Credit Card hat                -10
        ///    10       3       1/10/2019 12:00:00 AM   Clothes scarf(on credit)        15
        ///     9       4       1/10/2020 12:00:00 AM   Credit Card scarf              -15
        ///    14       5       1/11/2020 12:00:00 AM   Eating Out McDonalds            45
        ///    14       7       1/12/2020 12:00:00 AM   Eating Out Wendys               25
        ///    14      10       2/1/2020 12:00:00 AM    Eating Out Pizza                33.33
        ///     9      13       2/10/2020 12:00:00 AM   Credit Card mittens            -15
        ///     9      12       2/25/2020 12:00:00 AM   Credit Card Hat                -25
        ///    14      11       2/27/2020 12:00:00 AM   Eating Out Pizza                33.33
        ///    14       9       7/11/2020 12:00:00 AM   Eating Out Cafeteria            11.11
        /// </code>
        /// 
        /// <b>Getting a list of budget items by MONTH.</b>
        /// 
        /// <code>
        /// <![CDATA[
        ///  HomeBudget budget = new HomeBudget();
        ///  
        /// //Reads from the budget file.
        ///  budget.ReadFromFile(filename);
        ///  
        /// //Get a list of all budget items
        ///  var budgetItems = budget.GetBudgetItemsByMonth(null, null, false, 0);
        ///     
        /// //print important information
        /// foreach (var bi in budgetItems)
        ///    {
        ///      Console.WriteLine(
        ///         String.Format("{0} {1,-20}  {2,8:C}", 
        ///             bi.Month.ToString(),
        ///              bi.Details[0].Category,
        ///              bi.Total)
        ///       );
        /// }
        ///
        /// ]]>
        /// </code>
        /// 
        /// Sample output containing the date, category of the first item in details and total:
        /// The list is sorted by datetime unlike before.
        /// <code>
        /// 2018/01 Clothes                  $0.00
        /// 2019/01 Clothes                -$15.00
        /// 2020/01 Credit Card            -$55.00
        /// 2020/02 Eating Out             -$26.66
        /// 2020/07 Eating Out             -$11.11
        /// </code>
        /// </example>
        public List<BudgetItemsByMonth> GetBudgetItemsByMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // -----------------------------------------------------------------------
            // get all items first
            // -----------------------------------------------------------------------
            List<BudgetItem> items = GetBudgetItems(Start, End, FilterFlag, CategoryID);

            // -----------------------------------------------------------------------
            // Group by year/month
            // -----------------------------------------------------------------------
            var GroupedByMonth = items.GroupBy(c => c.Date.Year.ToString("D4") + "/" + c.Date.Month.ToString("D2"));

            // -----------------------------------------------------------------------
            // create new list
            // -----------------------------------------------------------------------
            var summary = new List<BudgetItemsByMonth>();
            foreach (var MonthGroup in GroupedByMonth)
            {
                // calculate total for this month, and create list of details
                double total = 0;
                var details = new List<BudgetItem>();
                foreach (var item in MonthGroup)
                {
                    total = total + item.Amount;
                    details.Add(item);
                }

                // Add new BudgetItemsByMonth to our list
                summary.Add(new BudgetItemsByMonth
                {
                    Month = MonthGroup.Key,
                    Details = details,
                    Total = total
                });
            }

            return summary;
        }

        // ============================================================================
        // Group all expenses by category (ordered by category name)
        // ============================================================================

        /// <summary>
        /// Gets Budget items by category by first getting the budget items, grouping them by category, determining the total and creating a new object with the info.
        /// This is possible by using the method <see cref="GetBudgetItems"/> first, then grouping all the items according to their category, finding the total for each item in the list
        /// according to the amount and creating a <see cref="BudgetItemsByCategory"/> object for each item to add in the summary which will be returned in the end.
        /// </summary>
        /// <param name="Start">Start of datetime where the list would begin.</param>
        /// <param name="End">End of datetime where the list would end.</param>
        /// <param name="FilterFlag">filter outs unwanted categories if it is true.</param>
        /// <param name="CategoryID">Id number of the category.</param>
        /// <returns>list of BudgetItems explaining the summary.</returns>
        ///        /// <example>
        /// 
        /// For all examples below, assume the budget file contains the
        /// following elements:
        /// 
        /// <code>
        /// Cat_ID  Expense_ID  Date                    Description                    Cost
        ///    10       1       1/10/2018 12:00:00 AM   Clothes hat (on credit)         10
        ///     9       2       1/11/2018 12:00:00 AM   Credit Card hat                -10
        ///    10       3       1/10/2019 12:00:00 AM   Clothes scarf(on credit)        15
        ///     9       4       1/10/2020 12:00:00 AM   Credit Card scarf              -15
        ///    14       5       1/11/2020 12:00:00 AM   Eating Out McDonalds            45
        ///    14       7       1/12/2020 12:00:00 AM   Eating Out Wendys               25
        ///    14      10       2/1/2020 12:00:00 AM    Eating Out Pizza                33.33
        ///     9      13       2/10/2020 12:00:00 AM   Credit Card mittens            -15
        ///     9      12       2/25/2020 12:00:00 AM   Credit Card Hat                -25
        ///    14      11       2/27/2020 12:00:00 AM   Eating Out Pizza                33.33
        ///    14       9       7/11/2020 12:00:00 AM   Eating Out Cafeteria            11.11
        /// </code>
        /// 
        /// <b>Getting a list of budget items by CATEGORY</b>
        /// <code>
        /// <![CDATA[
        ///  HomeBudget budget = new HomeBudget();
        ///  
        /// //Reads from the budget file.
        ///  budget.ReadFromFile(filename);
        ///  
        /// //Get a list of all budget items
        ///  var budgetItems = budget.GetBudgetItemsByCategory(null, null, false, 0);
        ///     
        /// //print important information
        /// foreach (var bi in budgetItems)
        ///    {
        ///      Console.WriteLine(
        ///         String.Format("{0} {1,-20}  {2,8:C}", 
        ///             bi.Category.ToString(),
        ///              bi.Details[0].Date,
        ///              bi.Total)
        ///       );
        /// }
        ///
        /// ]]>
        /// </code>
        /// 
        /// Sample output containing the category, date of the first item in details and total:
        /// <code>
        /// Clothes 2018-01-10 12:00:00 AM   -$25.00
        /// Credit Card 2018-01-11 12:00:00 AM    $65.00
        /// Eating Out 2020-01-11 12:00:00 AM  -$147.77
        /// </code>
        /// </example>
        public List<BudgetItemsByCategory> GeBudgetItemsByCategory(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // -----------------------------------------------------------------------
            // get all items first
            // -----------------------------------------------------------------------
            List<BudgetItem> items = GetBudgetItems(Start, End, FilterFlag, CategoryID);

            // -----------------------------------------------------------------------
            // Group by Category
            // -----------------------------------------------------------------------
            var GroupedByCategory = items.GroupBy(c => c.Category);

            // -----------------------------------------------------------------------
            // create new list
            // -----------------------------------------------------------------------
            var summary = new List<BudgetItemsByCategory>();
            foreach (var CategoryGroup in GroupedByCategory.OrderBy(g => g.Key))
            {
                // calculate total for this category, and create list of details
                double total = 0;
                var details = new List<BudgetItem>();
                foreach (var item in CategoryGroup)
                {
                    total = total + item.Amount;
                    details.Add(item);
                }

                // Add new BudgetItemsByCategory to our list
                summary.Add(new BudgetItemsByCategory
                {
                    Category = CategoryGroup.Key,
                    Details = details,
                    Total = total
                });
            }

            return summary;
        }



        // ============================================================================
        // Group all expenses by category and Month
        // creates a list of ExpandoObjects... which are objects that can have
        //   properties added to it on the fly.
        // ... for each element of the list (expenses by month), the ExpandoObject will have a property
        //     Month = (year/month) (string)
        //     Total = Double total for that month
        //     and for each category that had an entry in that month...
        //     1) Name of category , 
        //     2) and a property called "details: <name of category>" 
        //  
        // ... the last element of the list will contain an ExpandoObject
        //     with the properties for each category, equal to the totals for that
        //     category, and the name of the "Month" property will be "Totals"
        // ============================================================================

        /// <summary>
        /// Gets the budget dictionary by category and month by getting a list of budget items by month, looping over each month and then looping over each category, and 
        /// determining the total for each category. This method is used for combining th information of Category and Month budget items with dictionaries.
        /// </summary>
        /// <param name="Start">Start of datetime where the list would begin.</param>
        /// <param name="End">End of datetime where the list would end.</param>
        /// <param name="FilterFlag">filter outs unwanted categories if it is true.</param>
        /// <param name="CategoryID">Id number of the category.</param>
        /// <returns>list of dictionaries explaining the summary.</returns>
        ///         /// <summary>
        /// Gets Budget items by category by first getting the budget items, grouping them by category, determining the total and creating a new object with the info.
        /// This is possible by using the method <see cref="GetBudgetItems"/> first, then grouping all the items according to their category, finding the total for each item in the list
        /// according to the amount and creating a <see cref="BudgetItemsByCategory"/> object for each item to add in the summary which will be returned in the end.
        /// </summary>
        /// <param name="Start">Start of datetime where the list would begin.</param>
        /// <param name="End">End of datetime where the list would end.</param>
        /// <param name="FilterFlag">filter outs unwanted categories if it is true.</param>
        /// <param name="CategoryID">Id number of the category.</param>
        /// <returns>list of BudgetItems explaining the summary.</returns>
        ///  <example>
        /// 
        /// For all examples below, assume the budget file contains the
        /// following elements:
        /// 
        /// <code>
        /// Cat_ID  Expense_ID  Date                    Description                    Cost
        ///    10       1       1/10/2018 12:00:00 AM   Clothes hat (on credit)         10
        ///     9       2       1/11/2018 12:00:00 AM   Credit Card hat                -10
        ///    10       3       1/10/2019 12:00:00 AM   Clothes scarf(on credit)        15
        ///     9       4       1/10/2020 12:00:00 AM   Credit Card scarf              -15
        ///    14       5       1/11/2020 12:00:00 AM   Eating Out McDonalds            45
        ///    14       7       1/12/2020 12:00:00 AM   Eating Out Wendys               25
        ///    14      10       2/1/2020 12:00:00 AM    Eating Out Pizza                33.33
        ///     9      13       2/10/2020 12:00:00 AM   Credit Card mittens            -15
        ///     9      12       2/25/2020 12:00:00 AM   Credit Card Hat                -25
        ///    14      11       2/27/2020 12:00:00 AM   Eating Out Pizza                33.33
        ///    14       9       7/11/2020 12:00:00 AM   Eating Out Cafeteria            11.11
        /// </code>
        /// 
        /// <b>Getting a list of budget items by CATEGORY and MONTH</b>
        /// <code>
        /// <![CDATA[
        ///  HomeBudget budget = new HomeBudget();
        ///  
        /// //Reads from the budget file.
        ///  budget.ReadFromFile(filename);
        ///  
        /// //Get a list of all budget items
        ///  var budgetItems = budget.GetBudgetItemsByCategory(null, null, false, 0);
        /// ]]>
        /// </code>
        /// </example>
        public List<Dictionary<string,object>> GetBudgetDictionaryByCategoryAndMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // -----------------------------------------------------------------------
            // get all items by month 
            // -----------------------------------------------------------------------
            List<BudgetItemsByMonth> GroupedByMonth = GetBudgetItemsByMonth(Start, End, FilterFlag, CategoryID);

            // -----------------------------------------------------------------------
            // loop over each month
            // -----------------------------------------------------------------------
            var summary = new List<Dictionary<string, object>>();
            var totalsPerCategory = new Dictionary<String, Double>();

            foreach (var MonthGroup in GroupedByMonth)
            {
                // create record object for this month
                Dictionary<string, object> record = new Dictionary<string, object>();
                record["Month"] = MonthGroup.Month;
                record["Total"] = MonthGroup.Total;

                // break up the month details into categories
                var GroupedByCategory = MonthGroup.Details.GroupBy(c => c.Category);

                // -----------------------------------------------------------------------
                // loop over each category
                // -----------------------------------------------------------------------
                foreach (var CategoryGroup in GroupedByCategory.OrderBy(g => g.Key))
                {

                    // calculate totals for the cat/month, and create list of details
                    double total = 0;
                    var details = new List<BudgetItem>();

                    foreach (var item in CategoryGroup)
                    {
                        total = total + item.Amount;
                        details.Add(item);
                    }

                    // add new properties and values to our record object
                    record["details:" + CategoryGroup.Key] =  details;
                    record[CategoryGroup.Key] = total;

                    // keep track of totals for each category
                    if (totalsPerCategory.TryGetValue(CategoryGroup.Key, out Double CurrentCatTotal))
                    {
                        totalsPerCategory[CategoryGroup.Key] = CurrentCatTotal + total;
                    }
                    else
                    {
                        totalsPerCategory[CategoryGroup.Key] = total;
                    }
                }

                // add record to collection
                summary.Add(record);
            }
            // ---------------------------------------------------------------------------
            // add final record which is the totals for each category
            // ---------------------------------------------------------------------------
            Dictionary<string, object> totalsRecord = new Dictionary<string, object>();
            totalsRecord["Month"] = "TOTALS";

            foreach (var cat in categories.List())
            {
                try
                {
                    totalsRecord.Add(cat.Description, totalsPerCategory[cat.Description]);
                }
                catch { }
            }
            summary.Add(totalsRecord);


            return summary;
        }




        #endregion GetList

    }
}
