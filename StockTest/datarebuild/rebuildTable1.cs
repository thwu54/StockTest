using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My
{
    class rb
    {
        public static DataTable rb1(DataTable dt,int[] Pre,int[] Next) {
            DataTable lTable = new DataTable();
            for (int i = 1; i <= dt.Rows.Count; i++)
            {
                // 新增一行
                DataRow newRow = lTable.NewRow(); 
                lTable.Rows.Add(newRow);
            }
            //lTable.Columns.Add("Close");
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    // 新增一行
            //    lTable.Rows[i]["Close" ] = double.Parse(dt.Rows[i]["Close"].ToString());
            //}
            for (int j = 0; j < Pre.Length; j++)
            {
                lTable.Columns.Add("X"+j.ToString(), typeof(double));
                for (int i = Pre[j]; i < dt.Rows.Count; i++)
                {
                    double currentCellValue = double.Parse(dt.Rows[i]["Close"].ToString());
                    double previousCellValue = double.Parse(dt.Rows[i - Pre[j]]["Close"].ToString());
                    lTable.Rows[i]["X" + j.ToString()] = currentCellValue - previousCellValue;
                }
            }

            for (int j = 0; j < Next.Length; j++)
            {
                lTable.Columns.Add("Next" + j.ToString(), typeof(double));
                for (int i = 0; i <= dt.Rows.Count - Next[j]-1; i++)
                {
                    double currentCellValue = double.Parse(dt.Rows[i]["Close"].ToString());
                    double NextCellValue = double.Parse(dt.Rows[i + Next[j]]["Close"].ToString());
                    lTable.Rows[i]["Next" + j.ToString()] = NextCellValue - currentCellValue ;
                }
            }
            RemoveRowsWithNull(ref lTable);
            return lTable;
        }


        static void RemoveRowsWithNull(ref DataTable dataTable)
        {
            foreach (DataColumn column in dataTable.Columns)
            {
                // 使用Select方法選擇包含空值的行
                //DataRow[] rowsToRemove = dataTable.Select($"{column.ColumnName} IS NULL OR {column.ColumnName} = ''");

                // 或者使用 Convert.IsDBNull 判斷 DBNull.Value
                DataRow[] rowsToRemove = dataTable.AsEnumerable().Where(row => row.IsNull(column))
            .ToArray();
                // 移除選擇的行
                foreach (DataRow row in rowsToRemove)
                {
                    dataTable.Rows.Remove(row);
                }
            }
        }
    }
}
