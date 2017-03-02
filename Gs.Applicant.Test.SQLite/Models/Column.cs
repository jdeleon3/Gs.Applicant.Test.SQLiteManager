namespace Gs.Applicant.Test.SQLite
{
    public class Column
    {
        public string Name { get; set; }
        public ColumnDataType ColumnType { get; set; }



        public string GetColumnNameNoSpaces()
        {
                if (string.IsNullOrEmpty(Name))
                    return string.Empty;
                return Name.Replace(' ', '_');
        }
    }

    public enum ColumnDataType
    {
        INTEGER,
        BLOB,
        NUMERIC,
        REAL,
        TEXT
    }
    
}