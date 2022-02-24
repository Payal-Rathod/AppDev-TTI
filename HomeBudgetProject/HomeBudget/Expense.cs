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
    // CLASS: Expense
    //        - An individual expens for budget program
    // ====================================================================

    /// <summary>
    /// Expense class is used to manage, store and access different properties of this object
    /// </summary>
    public class Expense
    {
        private Double amount;
        // ====================================================================
        // Properties
        // ====================================================================

        /// <summary>
        /// Getter property of the id of the expense.
        /// </summary>
        /// <value>The <c>Id</c> property represents a unique id number of the expense.</value>
        public int Id { get; }

        /// <summary>
        /// Getter property of the date of the expense.
        /// </summary>
        /// <value>The <c>Date</c> property represents the date when the expense was made.</value>
        public DateTime Date { get;  }

        /// <summary>
        /// Automatically implemented property of the amounts of the expense.
        /// </summary>
        /// <value>The <c>Amount</c> property represents the amount of money spent on this expense.</value>
        public Double Amount { get; set; }

        /// <summary>
        /// Automatically implemented property of the description of the expense.
        /// </summary>
        /// <value>The <c>Description</c> property represents a brief description (name) of the expense</value>
        public String Description { get; set; }

        /// <summary>
        /// Automatically implemented property of the category of the expense.
        /// </summary>
        /// <value>The <c>Category</c> property represents the category of this expense.</value>
        public int Category { get; set; }

        // ====================================================================
        // Constructor
        //    NB: there is no verification the expense category exists in the
        //        categories object
        // ====================================================================

        /// <summary>
        /// Constructor that iniializes the properties of this class according to the 5 parameters received.
        /// </summary>
        /// <param name="id">A unique id number of the expense</param>
        /// <param name="date">The date when the expense was made.</param>
        /// <param name="category">The category of the expense.</param>
        /// <param name="amount">The amount of money spent on this expense.</param>
        /// <param name="description">A brief description (name) of the expense.</param>
        public Expense(int id, DateTime date, int category, Double amount, String description)
        {
            this.Id = id;
            this.Date = date;
            this.Category = category;
            this.Amount = amount;
            this.Description = description;
        }

        // ====================================================================
        // Copy constructor - does a deep copy
        // ====================================================================

        /// <summary>
        /// Constructor that initializes the properties of this class according to a Expense object received as parameter.
        /// </summary>
        /// <param name="obj">An Expense object that will initialize the properties of this class.</param>
        public Expense (Expense obj)
        {
            this.Id = obj.Id;
            this.Date = obj.Date;
            this.Category = obj.Category;
            this.Amount = obj.Amount;
            this.Description = obj.Description;
           
        }
    }
}
