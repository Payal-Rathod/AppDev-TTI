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
    // CLASS: Category
    //        - An individual category for budget program
    //        - Valid category types: Income, Expense, Credit, Saving
    // ====================================================================

    /// <summary>
    /// Category class is used to manage, store and access different properties of this object
    /// </summary>
    public class Category
    {
        // ====================================================================
        // Properties
        // ====================================================================

        /// <summary>
        /// Automatically implemented property of the id of the category.
        /// </summary>
        /// <value>The <c>Id</c> property represents a unique id number of the category</value>
        public int Id { get; set; }

        /// <summary>
        /// Automatically implemented property of description of the category.
        /// </summary>
        /// <value>The <c>Description</c> property represents a short description (name) of the category</value>
        public String Description { get; set; }

        /// <summary>
        /// Automatically implemented property of the type of the category.
        /// </summary>
        /// <value>The <c>Type</c> property represents the type of the category. This property's type represents an enum: <see cref="CategoryType"/>.</value>
        public CategoryType Type { get; set; }

        /// <summary>
        /// Enum used to define the category type for each category
        /// </summary>
        public enum CategoryType
        {
            /// <summary>
            /// Category type where money is received.
            /// </summary>
            Income,

            /// <summary>
            /// Category type where money is spent.
            /// </summary>
            Expense,

            /// <summary>
            /// Category type where the money is an an amount that the card issuer owes you.
            /// </summary>
            Credit,

            /// <summary>
            /// Category type where money is saved.
            /// </summary>
            Savings
        };

        // ====================================================================
        // Constructor
        // ====================================================================

        /// <summary>
        /// Constructor that iniializes the properties of this class according to the 3 parameters received.
        /// </summary>
        /// <param name="id">The id number of the category</param>
        /// <param name="description">A short description (name) of the categor</param>
        /// <param name="type">The type of the category</param>
        public Category(int id, String description, CategoryType type = CategoryType.Expense)
        {
            this.Id = id;
            this.Description = description;
            this.Type = type;
        }

        // ====================================================================
        // Copy Constructor
        // ====================================================================

        /// <summary>
        /// Constructor that initializes the properties of this class according to a Category object received as parameter.
        /// </summary>
        /// <param name="category">A category object that will initialize the properties of this class.</param>
        public Category(Category category)
        {
            this.Id = category.Id;;
            this.Description = category.Description;
            this.Type = category.Type;
        }
        // ====================================================================
        // String version of object
        // ====================================================================

        /// <summary>
        /// Returns the description of the object.
        /// </summary>
        /// <returns>A string that represents the description (name) of the Category</returns>
        /// <example>
        /// The example shown below shows the usage of this method: 
        /// 
        /// <code>
        /// <![CDATA[
        /// //Calls object where category is an object that is already initialized.
        /// Category c = new Category(category);
        /// 
        /// //Calls the method to output the description of the Category.
        /// Console.WriteLine(c.ToString());
        /// ]]>
        /// </code>
        /// </example>
        public override string ToString()
        {
            return Description;
        }

    }
}

