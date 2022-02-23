using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budget
{
    class Program
    {
        static void Main(string[] args)
        {
             HomeBudget budget = new HomeBudget();
              
            //Reads from the budget file.
              budget.ReadFromFile("./test.budget");
              
              // Get a list of all budget items
             var budgetItems = budget.GetBudgetDictionaryByCategoryAndMonth(null, null, false, 0);
                   
             // print important information
             foreach (Dictionary<string, object> bi in budgetItems)
             {

                bi.TryGetValue("January", out object des);
                 Console.WriteLine(des);
           }


        }
    }
}
