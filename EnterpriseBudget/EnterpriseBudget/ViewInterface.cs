using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseBudget
{
    public interface ViewInterface
    {
        void OpenFile();
        void NewFile();
        void ShowDatabase(string filename);
        void LightMode();
        void DarkMode();
        void InitializeDataGrid();
        void InitializeDataGridByMonth();
        void InitializeDataGridByCategory();
        void InitializeDataGridByMonthAndCategory(List<Dictionary<string, object>> items);
    }
}
