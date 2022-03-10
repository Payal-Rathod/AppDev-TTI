using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Dynamic;
using System.Data.SQLite;

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
        private Categories _categories;
        private Expenses _expenses;
        private SQLiteConnection db;

        // ====================================================================
        // Properties
        // ===================================================================

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


        public HomeBudget(String databaseFile, String XMLFile, bool newDB = false)
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
            // create the _expense object
            _expenses = new Expenses(Database.dbConnection);

            db = Database.dbConnection;
        }

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
        /// //Connections to a database and fills the tables for categories, categorytypes and expenses
        /// string folder = TestConstants.GetSolutionDir();
        /// string inFile = TestConstants.GetSolutionDir() + "\\" + testInputFile;
        /// String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
        /// String messyDB = $"{folder}\\messy.db";
        /// System.IO.File.Copy(goodDB, messyDB, true);
        /// HomeBudget homeBudget = new HomeBudget(messyDB, inFile, false);
        /// List<Expense> listExpenses = homeBudget.expenses.List();
        /// List<Category> listCategories = homeBudget.categories.List();
        /// 
        /// //Calls function
        /// List<BudgetItem> budgetItems =  homeBudget.GetBudgetItems(null,null,false,9);
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
        /// // print important information
        /// foreach (var bi in budgetItems)
        ///  {
        ///      Console.WriteLine ( 
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
            DateTime realStart = Start ?? new DateTime(1900, 1, 1);
            DateTime realEnd = End ?? new DateTime(2500, 1, 1);

            String startString = realStart.ToString("yyyy-MM-dd");
            String endString = realEnd.ToString("yyyy-MM-dd");

            var cmd = new SQLiteCommand(db);

            cmd.CommandText = "Select c.Id, e.Id, e.Date, c.Description, e.Description, e.Amount from categories c inner join expenses e on c.Id = e.CategoryId where e.Date >= @Start and e.Date <= @End";
            cmd.Parameters.AddWithValue("@Start", startString);
            cmd.Parameters.AddWithValue("@End", endString);
            cmd.Prepare();
            cmd.ExecuteNonQuery(); 

            var rdr = cmd.ExecuteReader();
            List<BudgetItem> newList = new List<BudgetItem>();
            double total = 0;

            while (rdr.Read())
            {
                if (FilterFlag && CategoryID != rdr.GetInt32(0))
                {
                    continue;
                }
                // keep track of running totals
                total = total + rdr.GetDouble(5);

                newList.Add(new BudgetItem
                {
                    CategoryID = rdr.GetInt32(0),
                    ExpenseID = rdr.GetInt32(1),
                    ShortDescription = rdr.GetString(4),
                    Date = rdr.GetDateTime(2),
                    Amount = rdr.GetDouble(5),
                    Category = rdr.GetString(3),
                    Balance = total
                });

       
            }

            cmd.Dispose();

            return newList;


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
        /// string folder = TestConstants.GetSolutionDir();
        /// string inFile = TestConstants.GetSolutionDir() + "\\" + testInputFile;
        /// String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
        /// String messyDB = $"{folder}\\messy.db";
        /// System.IO.File.Copy(goodDB, messyDB, true);
        /// HomeBudget homeBudget = new HomeBudget(messyDB, inFile, false);
        ///
        /// //Calls function
        /// List<BudgetItemsByMonth> budgetItemsByMonth = homeBudget.GetBudgetItemsByMonth(null, null, false, 9);
        ///
        ///     
        /// //print important information
        /// foreach (var bi in budgetItemsByMonth)
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
            DateTime realStart = Start ?? new DateTime(1900, 1, 1);
            DateTime realEnd = End ?? new DateTime(2500, 1, 1);

            String startString = realStart.ToString("yyyy-MM-dd");
            String endString = realEnd.ToString("yyyy-MM-dd");

            // -----------------------------------------------------------------------
            // Group by year/month
            // -----------------------------------------------------------------------

            var cmd = new SQLiteCommand(db);

            cmd.CommandText = "Select c.Id, e.Id, e.Date, c.Description, e.Description, e.Amount, substr(Date, 1, 7) from categories c inner join expenses e on c.Id = e.CategoryId  where e.Date >= @Start and e.Date <= @End group by substr(Date, 1, 7)";
            cmd.Parameters.AddWithValue("@Start", startString);
            cmd.Parameters.AddWithValue("@End", endString);
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            var rdr = cmd.ExecuteReader();

            List<BudgetItemsByMonth> summary = new List<BudgetItemsByMonth>();

            while (rdr.Read())
            {
                var cmd2 = new SQLiteCommand(db);
                cmd2.CommandText = "Select c.Id, e.Id, e.Date, c.Description, e.Description, e.Amount, substr(Date, 1, 7) from categories c inner join expenses e on c.Id = e.CategoryId where  substr(Date,1,7) = @Month";
                cmd2.Parameters.AddWithValue("@Month", rdr.GetString(6));
                cmd2.Prepare();
                cmd2.ExecuteNonQuery();
                
                var rdr2 = cmd2.ExecuteReader();
                double total = 0;

                List<BudgetItem> details = new List<BudgetItem>();

                while (rdr2.Read())
                {
                    if (FilterFlag && CategoryID != rdr2.GetInt32(0))
                    {
                        continue;
                    }

                    total += rdr2.GetDouble(5);

                    details.Add(new BudgetItem
                    {
                        CategoryID = rdr2.GetInt32(0),
                        ExpenseID = rdr2.GetInt32(1),
                        ShortDescription = rdr2.GetString(4),
                        Date = rdr2.GetDateTime(2),
                        Amount = rdr2.GetDouble(5),
                        Category = rdr2.GetString(3),
                        Balance = total
                    });
                }

                if (details.Count != 0)
                {
                    summary.Add(new BudgetItemsByMonth
                    {
                        Month = rdr.GetString(2).Split('-')[0] + "/" + rdr.GetString(2).Split('-')[1],
                        Details = details,
                        Total = total
                    });
                }

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
        /// string folder = TestConstants.GetSolutionDir();
        /// string inFile = TestConstants.GetSolutionDir() + "\\" + testInputFile;
        /// String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
        /// String messyDB = $"{folder}\\messy.db";
        /// System.IO.File.Copy(goodDB, messyDB, true);
        /// HomeBudget homeBudget = new HomeBudget(messyDB, inFile, false);
        /// int maxRecords = TestConstants.budgetItemsByCategory_MaxRecords;
        /// BudgetItemsByCategory firstRecord = TestConstants.budgetItemsByCategory_FirstRecord;
        /// 
        /// //Calls function
        /// List<BudgetItemsByCategory> budgetItemsByCategory = homeBudget.GeBudgetItemsByCategory(null, null, false, 9);
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
            DateTime realStart = Start ?? new DateTime(1900, 1, 1);
            DateTime realEnd = End ?? new DateTime(2500, 1, 1);

            String startString = realStart.ToString("yyyy-MM-dd");
            String endString = realEnd.ToString("yyyy-MM-dd");


            // -----------------------------------------------------------------------
            // Group by category
            // -----------------------------------------------------------------------

            var cmd = new SQLiteCommand(db);

            cmd.CommandText = "Select c.Id, e.Id, e.Date, c.Description, e.Description, e.Amount, substr(Date, 1, 7) from categories c inner join expenses e on c.Id = e.CategoryId  where e.Date >= @Start and e.Date <= @End group by c.Id order by c.Description";
            cmd.Parameters.AddWithValue("@Start", startString);
            cmd.Parameters.AddWithValue("@End", endString);
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            var rdr = cmd.ExecuteReader();

            List<BudgetItemsByCategory> summary = new List<BudgetItemsByCategory>();

            while (rdr.Read())
            {
                var cmd2 = new SQLiteCommand(db);
                cmd2.CommandText = "Select c.Id, e.Id, e.Date, c.Description, e.Description, e.Amount, substr(Date, 1, 7) from categories c inner join expenses e on c.Id = e.CategoryId where c.Id = @Category and e.Date >= @Start and e.Date <= @End";
                cmd2.Parameters.AddWithValue("@Category", rdr.GetInt32(0));
                cmd2.Parameters.AddWithValue("@Start", startString);
                cmd2.Parameters.AddWithValue("@End", endString);
                cmd2.Prepare();
                cmd2.ExecuteNonQuery();

                var rdr2 = cmd2.ExecuteReader();
                double total = 0;

                List<BudgetItem> details = new List<BudgetItem>();

                while (rdr2.Read())
                {
                    if (FilterFlag && CategoryID != rdr2.GetInt32(0))
                    {
                        continue;
                    }

                    total += rdr2.GetDouble(5);

                    details.Add(new BudgetItem
                    {
                        CategoryID = rdr2.GetInt32(0),
                        ExpenseID = rdr2.GetInt32(1),
                        ShortDescription = rdr2.GetString(4),
                        Date = rdr2.GetDateTime(2),
                        Amount = rdr2.GetDouble(5),
                        Category = rdr2.GetString(3),
                        Balance = total
                    });
                }

                if (details.Count != 0)
                {
                    summary.Add(new BudgetItemsByCategory
                    {
                        Category = rdr.GetString(3),
                        Details = details,
                        Total = total
                    });
                }

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
