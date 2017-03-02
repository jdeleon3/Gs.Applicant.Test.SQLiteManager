using System.Data;

namespace Gs.Applicant.Test.SQLite
{
    public interface IDataManager
    {
        string CurrentTableName { get; set; }
        void CreateDatabase(string name);

        void ChangeDatabase(string connectionString);

        void CreateTable(string name, Column[] props);

        string[] GetDatabaseTableListing();

        int InsertRow(DataRow row);

        void UpdateRow(DataRow row);

        void RemoveRow(int id);

        DataTable GetRow(int id);

        DataTable GetAllRows();
    }
}