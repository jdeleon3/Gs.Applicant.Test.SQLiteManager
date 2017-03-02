using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Gs.Applicant.Test.SQLite
{
    public static class ModelExtensions
    {
        public static string GetStringValue(this ColumnDataType ctype)
        {
            return typeof(ColumnDataType).GetEnumName(ctype);
        }

        //public static DataRow ToDataRow(this Row r)
        //{
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("rowid");
        //    var cols = from header in r.CellHeaders
        //               select new DataColumn(header);
        //    dt.Columns.AddRange(cols.ToArray());
        //    DataRow dr = dt.NewRow();
        //    dr["rowid"] = r.Id;
        //    foreach (var cell in r.Cells)
        //    {
        //        dr[cell.Header] = cell.Value;
        //    }
        //    return dr;
        //}

        //public static DataTable ToDataTable(this Row r)
        //{
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("rowid");
        //    var cols = from header in r.CellHeaders
        //               select new DataColumn(header);
        //    dt.Columns.AddRange(cols.ToArray());
        //    DataRow dr = dt.NewRow();
        //    dr["rowid"] = r.Id;
        //    foreach (var cell in r.Cells)
        //    {
        //        dr[cell.Header] = cell.Value;
        //    }
        //    dt.Rows.Add(dr);
        //    return dt;
        //}

        //public static DataTable ToDataTable(this Row[] rows)
        //{
        //    if (rows.Length == 0)
        //        return new DataTable();

        //    DataTable dt = rows[0].ToDataTable();
        //    for (int i = 1; i < rows.Length; ++i)
        //    {
        //        DataRow dr = dt.NewRow();
        //        dr["rowid"] = rows[i].Id;
        //        foreach (var cell in rows[i].Cells)
        //        {
        //            dr[cell.Header] = cell.Value;
        //        }
        //        dt.Rows.Add(dr);
        //    }
        //    return dt;
        //}

        //public static Row[] ToRowArray(this DataTable dt)
        //{
        //    Row[] rows = new Row[dt.Rows.Count];
        //    for (int i = 0; i < dt.Rows.Count; ++i)
        //    {
        //        rows[i] = new Row()
        //        {
        //            Id = dt.Columns.Contains("rowid") ? (int)dt.Rows[i]["rowid"] : -1
        //        };
        //        rows[i].CellHeaders = new string[dt.Columns.Count];
        //        rows[i].Cells = new Cell[dt.Columns.Count];
        //        for (int j = 0; j < dt.Columns.Count; ++j)
        //        {
        //            string header = dt.Columns[j].ColumnName;
        //            rows[i].CellHeaders[j] = header;
        //            rows[i].Cells[j] = new Cell()
        //            {
        //                Header = header,
        //                Value = dt.Rows[i][header]
        //            };
        //        }
        //    }
        //    return rows;
        //}
    }
}
