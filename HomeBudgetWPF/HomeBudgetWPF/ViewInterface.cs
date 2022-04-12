using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudgetWPF
{
    public interface ViewInterface
    {
        void ShowError(string msg);
        void Refresh();
        void OpenFile();
        void ShowAdded(string desc);
        void ShowDatabase(string filename);
        void LightMode();
        void DarkMode();
        void CancelExpense();
        void EnableBtnAndInput();
        void DisableBtnAndInput();
    }
}
