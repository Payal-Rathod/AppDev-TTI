using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseBudget
{
    interface AddExpenseInterface
    {
        void ShowError(string msg);
        void Refresh();
        void CancelExpense();
        void ShowAdded(string desc);
    }
}
