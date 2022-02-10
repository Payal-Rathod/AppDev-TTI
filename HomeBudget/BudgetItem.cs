using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Budget
{
    // ====================================================================
    // CLASS: BudgetItem
    //        A single budget item, includes Category and Expense
    // ====================================================================

    /// <summary>
    /// BudgetItem class is used to store and access all the properties of the budget.
    /// </summary>
    public class BudgetItem
    {
        /// <summary>
        /// Automatically implemented property to store the ID number of the item's category.
        /// </summary>
        /// <value>The <c>CategoryID</c> property represents the category id of the item.</value>
        public int CategoryID { get; set; }

        /// <summary>
        /// Automatically implemented property to store the ID number of the item's Expense.
        /// </summary>
        /// <value>The <c>ExpenseID</c> property represents the expense id of the item.</value>
        public int ExpenseID { get; set; }

        /// <summary>
        /// Automatically implemented property to store the date.
        /// </summary>
        /// <value>The <c>Date</c> property represents the datetime of the item.</value>
        public DateTime Date { get; set; }

        /// <summary>
        /// Automatically implemented property to store the Category of the item.
        /// </summary>
        /// <value>The <c>Category</c> property represents the category of the item.</value>
        public String Category { get; set; }

        /// <summary>
        /// Automatically implemented property to store a small description of the item.
        /// </summary>
        /// <value>The <c>ShortDescription</c> property represents a brief description (name) of the item</value>
        public String ShortDescription { get; set; }

        /// <summary>
        /// Automatically implemented property to store the amount value of the item.
        /// </summary>
        /// <value>The <c>Amount</c> property represents the cost of the item</value>
        public Double Amount { get; set; }

        /// <summary>
        /// Automatically implemented property to store the balance.
        /// </summary>
        /// <value>The <c>Balance</c> property represents the total left according to the amount spent or received</value>
        public Double Balance { get; set; }

    }
    
    /// <summary>
    /// BudgetItemsByMonth class is used to store and access Bugdet item properties grouped by month.
    /// </summary>
    public class BudgetItemsByMonth
    {
        /// <summary>
        /// Automatically implemented property to store the month.
        /// </summary>
        /// <value>The <c>Month</c> property represents the month of the date of the budget item.</value>
        public String Month { get; set; }

        /// <summary>
        /// Automatically implemented property to store the Details of the items.
        /// </summary>
        /// <value>The <c>Details</c> property represents the list of items and properties of the budget item..</value>
        public List<BudgetItem> Details { get; set; }

        /// <summary>
        /// Automatically implemented property to store the total.
        /// </summary>
        /// <value>The <c>Total</c> property represents the balance afte the amount has been added or removed from the previous balance.</value>
        public Double Total { get; set; }
    }

    /// <summary>
    /// BudgetItemsByCategory class is used to store and access Bugdet item properties grouped by Category.
    /// </summary>
    public class BudgetItemsByCategory
    {
        /// <summary>
        /// Automatically implemented property to store the categore.
        /// </summary>
        /// <value>The <c>Category</c> property represents the category of the budget item.</value>
        public String Category { get; set; }

        /// <summary>
        /// Automatically implemented property to store the Details of the items.
        /// </summary>
        /// <value>The <c>Details</c> property represents the list of items and properties of the budget item..</value>
        public List<BudgetItem> Details { get; set; }

        /// <summary>
        /// Automatically implemented property to store the total.
        /// </summary>
        /// <value>The <c>Total</c> property represents the balance afte the amount has been added or removed from the previous balance.</value>
        public Double Total { get; set; }

    }


}
