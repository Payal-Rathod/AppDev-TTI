﻿using System;
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
        void ShowAdded();
        void ShowUserHistory();
        void RecentlyOpened();
        void ShowDatabase();
        void LightMode();
        void DarkMode();
        void Cancel();
    }
}
