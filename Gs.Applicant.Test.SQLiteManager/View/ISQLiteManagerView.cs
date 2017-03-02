using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gs.Applicant.Test.SQLite.Manager.View
{
    public interface ISQLiteManagerView
    {
        DataTable CurrentTableRows { get; set; }
        string DBConnectionString { get; }
        string DBName { get; set; }
        IList<Column> NewTableColumns { get; set; }
        string NewTableName { get; set; }
        int SelectedRow { get; set; }
        string SelectedTableName { get; set; }
        string[] tableList { get; set; }

        void Reset();
    }
}
