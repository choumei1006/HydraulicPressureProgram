using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CreepRateApp.Core
{
    public static class DevexpressTools
    {
        public static System.Collections.IList GetGridViewFilteredAndSortedData(DevExpress.XtraGrid.Views.Grid.GridView view)
        {
            return view.DataController.GetAllFilteredAndSortedRows();
        }
    }
}
