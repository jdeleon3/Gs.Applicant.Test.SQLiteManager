using Gs.Applicant.Test.SQLite;
using Gs.Applicant.Test.SQLite.Manager.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gs.Applicant.Test.SQLite.Manager.Presenter
{
    public class SQLiteManagerPresenter
    {
        private ISQLiteManagerView view;
        private IDataManager model;
        private SQLiteManagerPresenter()
        { }

        public SQLiteManagerPresenter(ISQLiteManagerView view, IDataManager model)
        {
            this.view = view;
            this.model = model;
        }

        public void LoadDatabase()
        {
            model.ChangeDatabase(view.DBConnectionString);
            view.tableList = model.GetDatabaseTableListing();
        }

        public void CreateAndLoadDatabase()
        {
            model.CreateDatabase(view.DBName);
            LoadDatabase();
        }

        public void CreateNewTable()
        {
            model.CreateTable(view.NewTableName, view.NewTableColumns.ToArray());
            
        }

        public void LoadTable()
        {
            model.CurrentTableName = view.SelectedTableName;
            view.CurrentTableRows = model.GetAllRows();
        }

        public void UpdateRow(int i)
        {
            model.UpdateRow(view.CurrentTableRows.Rows[i]);
        }

        public void SaveNewRow()
        {
            view.CurrentTableRows.Rows[view.SelectedRow]["rowid"] = model.InsertRow(view.CurrentTableRows.Rows[view.SelectedRow]);
        }

        public void DeleteRow()
        {
            int id = (int)view.CurrentTableRows.Rows[view.SelectedRow]["rowid"];
            if( id > 0)
             model.RemoveRow(id);
        }

    }
}
