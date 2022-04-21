using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudgetWPF
{
    interface AddExpenseInterface
    {
        void ShowError(string msg);
        void Refresh();
        void CancelExpense();
    }
}
