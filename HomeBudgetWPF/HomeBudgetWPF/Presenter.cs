﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Budget;

namespace HomeBudgetWPF
{
    public class Presenter
    {
        private readonly ViewInterface view;
        private HomeBudget homeBudget;
        private Categories cats;
        private Expenses expenses;
        public Presenter(ViewInterface v)
        {
            view = v;
        }
        public void openDatabase(string filename, bool newDb)
        {
            homeBudget = new HomeBudget(filename, "", newDb);
            cats = homeBudget.categories;
            expenses = homeBudget.expenses;
        }
    }
}
