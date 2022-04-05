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
        void CloseFile();
        void ShowAdded(DateTime date, int amount, string desc, string category);
        void ShowUserHistory();
        void RecentlyOpened();
        void ShowDatabase();
<<<<<<< HEAD
        void ShowCategories();
=======
        void LightMode();
        void DarkMode();
>>>>>>> 58c5c8dd26076fa3053fed78922dc4a110df7220
        void Cancel();
    }
}
