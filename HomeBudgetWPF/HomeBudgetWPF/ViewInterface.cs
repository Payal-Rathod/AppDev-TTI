using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudgetWPF
{
    interface ViewInterface
    {
        void ShowError(string msg);
        void Refresh();
        void OpenFile();
        void CloseFile();
        void ShowAdded();
        void ShowUserHistory();
        void RecentlyOpened();
        void ShowDatabase();
        void AddCategory();
        void ShowCategories();
        void Cancel();
    }
}
