using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Gs.Applicant.Test.SQLite
{
    public class SQLiteConnector : IDisposable, IDataManager
    {
        private SQLiteConnection connection;
        public string CurrentTableName { get; set; }

        public SQLiteConnector()
        {
            connection = new SQLiteConnection();
        }

        public SQLiteConnector(string connectionString)
        {
            connection = new SQLiteConnection(connectionString);            
        }

        public void CreateDatabase(string name)
        {
            ValidateDatabaseName(name);
            SQLiteConnection.CreateFile(name);
        }

        public void ChangeDatabase(string connectionString)
        {
            Close();
            connection = new SQLiteConnection(connectionString);
        }


        public void CreateTable(string name, Column[] props)
        {
            validateConnection();
            ValidateColumns(props);
            try
            {
                Open();
                CreateTableQuery(name, props);
                CurrentTableName = name;
            }
            finally
            {
                Close();
            }
        }

        public DataTable GetAllRows()
        {
            validateConnection();
            validateTableName();
            try
            {
                Open();
                return QueryForAllRows();
            }
            finally
            {
                Close();
            }
        }

        public string[] GetDatabaseTableListing()
        {
            validateConnection();
            try
            {
                Open();
                return QueryForTableList();
            }
            finally
            {
                Close();
            }
        }

        public DataTable GetRow(int id)
        {
            validateConnection();
            validateTableName();
            try
            {
                Open();
                return QueryForRow(id);
            }
            finally
            {
                Close();
            }
        }

        public int InsertRow(DataRow row)
        {
            validateConnection();
            validateTableName();
            try
            {
                Open();
                return QueryToInsertRow(row);
            }
            finally
            {
                Close();
            }
        }

        public void RemoveRow(int id)
        {
            validateConnection();
            validateTableName();
            try
            {
                Open();
                QueryToDeleteRow(id);
            }
            finally
            {
                Close();
            }
        }

        public void UpdateRow(DataRow row)
        {
            validateConnection();
            validateTableName();
            try
            {
                Open();
                QueryToUpdateRow(row);
            }
            finally
            {
                Close();
            }
        }

        private void CreateTableQuery(string name, Column[] props)
        {
            //TODO: Need to Sanitize all inputs for SQL injection
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("CREATE TABLE if not exists {0} (", name);

            for (int i = 0; i < props.Length; ++i)
            {
                if (i != 0)
                    sb.Append(",");
                sb.AppendFormat("{0} {1}", props[i].GetColumnNameNoSpaces(), props[i].ColumnType.GetStringValue());
            }
            sb.Append(")");

            string sql = sb.ToString();
            var cmd = new SQLiteCommand(sql, connection);
            cmd.ExecuteNonQuery();
        }

        private string[] QueryForTableList()
        {
            string sql = "SELECT NAME from sqlite_master WHERE NAME not like 'sqlite_%' and NAME not like 'IPK_%' and NAME not like 'IFK_%';";
            var cmd = new SQLiteCommand(sql, connection);
            var reader = cmd.ExecuteReader();
            List<string> tableNames = new List<string>();
            while (reader.Read())
            {
                tableNames.Add(reader["NAME"] as string);
            }
            return tableNames.ToArray();
        }

        private DataTable QueryForAllRows()
        {
            //TODO:  Need to sanatize table name since it's publicly accessible
            string sql = string.Format("SELECT * FROM {0}", CurrentTableName);
            var cmd = new SQLiteCommand(sql, connection);
            var reader = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            return dt;
        }

        private DataTable QueryForRow(int id)
        {
            string sql = string.Format("SELECT * FROM {0} WHERE rowid = @id", CurrentTableName);
            var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id", id);
            var reader = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            return dt;
        }

        private void QueryToDeleteRow(int id)
        {
            string sql = string.Format("DELETE FROM {0} WHERE rowid = @id", CurrentTableName);
            var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        private int QueryToInsertRow(DataRow row)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder valuesBuilder = new StringBuilder();
            SQLiteParameter[] parameters = new SQLiteParameter[row.ItemArray.Length];
            sb.AppendFormat("INSERT INTO {0}(", CurrentTableName);
            valuesBuilder.Append(" VALUES(");
            for (int i = 0; i < row.ItemArray.Length; ++i)
            {
                if (i != 0)
                {
                    sb.Append(",");
                    valuesBuilder.Append(",");
                }
                sb.Append(row.Table.Columns[i].ColumnName);
                valuesBuilder.AppendFormat("@{0}", i);
                parameters[i] = new SQLiteParameter(string.Format("@{0}", i), row.ItemArray[i]);
            }
            sb.Append(")");
            valuesBuilder.Append(")");

            string sql = sb.Append(valuesBuilder.ToString()).ToString();
            var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddRange(parameters);
            cmd.ExecuteNonQuery();
            return GetLastRowId();
        }

        private void QueryToUpdateRow(DataRow row)
        {
            StringBuilder sb = new StringBuilder();
            SQLiteParameter[] parameters = new SQLiteParameter[row.ItemArray.Length+1];
            sb.AppendFormat("UPDATE {0} SET ", CurrentTableName);
            for (int i = 0; i < row.ItemArray.Length; ++i)
            {
                if (i != 0)
                {
                    sb.Append(",");
                }
                sb.AppendFormat("{0} = @{1}", row.Table.Columns[i].ColumnName, i);
                parameters[i] = new SQLiteParameter(string.Format("@{0}", i), row.ItemArray[i]);
            }
            sb.Append(" WHERE rowid = @id");
            parameters[row.ItemArray.Length] = new SQLiteParameter("@id", row[GetIdColumnName(row.Table.TableName)]);

            string sql = sb.ToString();
            var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddRange(parameters);
            cmd.ExecuteNonQuery();
        }

        private int GetLastRowId()
        {
            string sql = "SELECT last_insert_rowid()";
            var cmd = new SQLiteCommand(sql, connection);
            return (int)cmd.ExecuteScalar();
        }

        private static void ValidateColumns(Column[] props)
        {
            if (props.Count(x => string.IsNullOrEmpty(x.Name)) > 0)
            {
                throw new ArgumentException("Column Names cannot be empty.");
            }
        }

        private static void ValidateDatabaseName(string name)
        {
            if (System.IO.File.Exists(name))
            {
                throw new ArgumentException("Database already exists.");
            }
            if (!name.ToUpper().EndsWith(".SQLITE"))
            {
                throw new ArgumentException("Name must be of type sqlite.");
            }
        }

        private void Open()
        {
            connection.Open();
        }

        private void validateConnection()
        {
            if (connection == null)
            {
                throw new NullReferenceException("Database Connection is null.");
            }
        }

        private void validateTableName()
        {
            if (string.IsNullOrEmpty(CurrentTableName))
            {
                throw new NullReferenceException("CurrentTableName cannot be null or empty.");
            }
        }

        private void Close()
        {
            if (connection != null)
            {
                if (connection.State != System.Data.ConnectionState.Closed)
                    connection.Close();
            }
        }

        private void DisposeConnection()
        {
            if (connection != null)
            {
                Close();
                connection.Dispose();
                connection = null;
            }
        }
        public static string GetIdColumnName(string table)
        {
            return string.Format("{0}Id", table);
        }

        void IDisposable.Dispose()
        {
            DisposeConnection();
        }
    }
}