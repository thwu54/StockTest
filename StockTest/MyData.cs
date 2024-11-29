using System;
using System.Data;
using System.Data.OleDb;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;
using Microsoft.Data.Analysis;
//for SQLite app config 須設定
//<startup useLegacyV2RuntimeActivationPolicy="true">
//  <supportedRuntime version="v4.0"/>
//</startup>
using System.Net.Http;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using System.Linq;
using FastMember;
using System.Text;
using System.Web.Script.Serialization;
using StockTest;
namespace My
{
    /// <summary>
    /// MyOracle 的摘要描述。
    /// </summary>
    /// 
    public class TagData
    {
        public TagData(string Tag , double Data,List<Trade> _TradeLog,List<string> _TradeType)
        {
            Text = Tag;
            Value = Data;
            TradeLog = _TradeLog;
            TradeType = _TradeType;
        }
        public TagData(string Tag, double Data, double _Value2)
        {
            Text = Tag;
            Value = Data;
            Value2 = _Value2; 
        }
        /// <summary>
        ///Text=unit value=gain/drop value2=drop value3=lastMoney
        /// </summary>
        /// <returns>使用默认值初始化的 MyObject 实例。</returns>
        public TagData(string Tag, double Data, double _Value2, double _Value3)
        {
            Text = Tag;
            Value = Data;
            Value2 = _Value2;
            Value3 = _Value3;
        }
        public List<string> TradeType = new List<string>();
        public List<Trade> TradeLog = new List<Trade>();
        public double Value { get; set; }
        public double Value2 { get; set; }
        public double Value3 { get; set; }
        public string Text { get; set; }
    }

    public class MyData
    {
        public event EventHandler ConnectError; //宣告不用傳回參數之事件
        private string lConnstr = "";
        private SqlDataAdapter SqlDataAdapter1 = new SqlDataAdapter();
        private OleDbDataAdapter OleDbDataAdapter1 = new OleDbDataAdapter();
        private SqlConnection SqlConnection1 = new SqlConnection();

        //srcConnect="Provider=MSDAORA;User ID="+id1.Text+";Data Source="+Source1.Text+";Password="+pwd1.Text;
        //srcConnect="provider=Microsoft.Jet.OLEDB.4.0; data source=" + path1.Text + "; Extended Properties=Excel 8.0;";
        //dstConnect="Provider=SQLOLEDB;User ID=sa;Initial Catalog=Db1;PWD=pwd;Data Source=Db1;";


        public MyData(string Connstr)
        {
            lConnstr = Connstr;
            //if (!FunLog.IsFileExist(Connstr))
            //{
            //    SQLiteConnection.CreateFile(Connstr);
            //}
            //lConnstr = "Data source=" + Connstr;
        }

        public static void DataTableToJson(DataTable dataTable, string fileName)
        {
            string Pathfile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string AllPath = Pathfile.Substring(0, Pathfile.LastIndexOf("\\"));
            FunLog.checkFolderExistAll(AllPath+ "\\"+fileName); 
            string json = JsonConvert.SerializeObject(dataTable, Formatting.Indented);
            File.WriteAllText(fileName, json);
        }

        public static void ObjToJson<T>(T dataTable , string fileName)
        {
            //string Pathfile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            //string AllPath = Pathfile.Substring(0, Pathfile.LastIndexOf("\\")); 
            //string lLogName = AllPath + "\\" + fileName;

            string json = JsonConvert.SerializeObject(dataTable, Formatting.Indented);
            File.WriteAllText(fileName, json);
        }

        public static T  JsonToObj<T>(string filePath)
        {
            string json = File.ReadAllText(filePath);
            T dataTable = JsonConvert.DeserializeObject<T>(json);
            return dataTable;
        }

        public static DataTable JsonToDataTable(string filePath)
        {
            string json = File.ReadAllText(filePath);
            DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(json);
            return dataTable;
        }

        public static DataTable Trade2Table(List<Trade> lTrades)
        {
            DataTable dataTable = new DataTable();
            using (var reader = ObjectReader.Create(lTrades))
            {
                
                dataTable.Load(reader);
            }
            return dataTable;
        }

        public static DataTable GetTableAvg(DataTable lTable)
        {
            DataRow lRow = lTable.NewRow();
            foreach (DataColumn item in lTable.Columns)
            {
                Type colType = GetColumnType(lTable, item.ColumnName);
                if (colType == typeof(string))
                    lRow[item.ColumnName] = "";
                else
                {
                    if(colType== typeof(int))
                        lRow[item.ColumnName] =(int)CalculateColumnAverage(lTable, item.ColumnName, colType);
                    else if (colType == typeof(double))
                        lRow[item.ColumnName] = (double)CalculateColumnAverage(lTable, item.ColumnName, colType);
                    else
                        lRow[item.ColumnName] = CalculateColumnAverage(lTable, item.ColumnName, colType);
                }
            }
            lTable.Rows.Add(lRow);
            return lTable;
        }

        public static double CalculateColumnAverage(DataTable dataTable, string columnName,Type colType)
        {
            var columnValues = dataTable.AsEnumerable()
         .Select(row => Convert.ChangeType(row.Field<object>(columnName), colType));

            double average = columnValues.Average(value => Convert.ToDouble(value));

            double sum = 0;
            double count = 0;
            foreach (DataRow item in dataTable.Rows)
            {
                sum += double.Parse(item[columnName].ToString());
                count++;
            }
            double average2 = sum / count;
            return average2;
        }

        public static double CalculateColumnAverage(DataTable dataTable, string columnName)
        {
            double average = dataTable.AsEnumerable()
                .Select(row => row.Field<double>(columnName))
                .Average();

            return average;
        }

        public static Type GetColumnType(DataTable dataTable, string columnName)
        {
            DataColumn column = dataTable.Columns[columnName];
            return column.DataType;
        }

        public static DataTable Sort(DataTable lTable,string columnName,bool ascending)
        {  
            DataView dataView = new DataView(lTable);
            dataView.Sort = columnName + (ascending ? " ASC" : " DESC");
            return   dataView.ToTable(); // 將排序後的結果轉回 DataTable
        }

        public ArrayList GetStockList()
        { 
            string sql = "select distinct stockno from [StockData] where Dates >(SELECT DATEADD(DAY, -10,MAX( [Dates]))  FROM  [StockData]) order by stockno";
            sql= @"select a.* from (
select stockno,avg([close]*Volume/1000) sums,avg(Volume/1000) v from [StockData] 
where Dates >(SELECT DATEADD(DAY, -10,MAX( [Dates]))  FROM  [StockData]) 
and [open]>0     group by stockno --order by stockno  
)  a where   sums>50000  order by sums ";
            return this.FineDataArrayListV(sql);
        }

        public double GetStockReal(string stockno)
        {
            string sql = "select price from  StockReal where createtime>getdate()-5 AND stockno='" + stockno+  "' order by createtime desc";
            return double.Parse(MyDatas.lData.FineDataField(sql));
        }

        public Dictionary<string,string[]> GetStockListHash()
        {
            string sql = "SELECT [StockNo],[StockName],[StockType]  FROM [MyAnalysis].[dbo].[StockList]";
            DataTable lTable = this.FineDataTable(sql);
            Dictionary<string, string[]> stockName = new Dictionary<string, string[]>();
            foreach (DataRow item in lTable.Rows)
            {
                stockName.Add(item[0].ToString(), new string[] { item[1].ToString() ,item[2].ToString() });
            }
            return stockName;
        }

        public static DataFrame table_df  (DataTable dt)
        {

            DataFrame df = new DataFrame();
            
            foreach (DataColumn item in dt.Columns)
            {
                if(item.ColumnName== "date")
                {
                   
                    var dateColumn = dt.AsEnumerable().Select(row => DateTime.Parse(row.Field<string>("date"))).ToArray();

                    var dateDataFrameColumn = new PrimitiveDataFrameColumn<DateTime>(item.ColumnName, dateColumn);
                    df.Columns.Add(dateDataFrameColumn);
                }
                else
                {
                    //var dateColumn = dt.AsEnumerable().Select(row => double.Parse(row.Field<string>(item.ColumnName))).ToArray();
                    var dateColumn = dt.AsEnumerable().Select(row => row.Field<string>(item.ColumnName) != null ? double.Parse(row.Field<string>(item.ColumnName)) : 0).ToArray();

                    var dateDataFrameColumn = new PrimitiveDataFrameColumn<double>(item.ColumnName, dateColumn);
                    df.Columns.Add(dateDataFrameColumn);
                }

            }
           
            return df;
        }

        public static DataTable df_dt(DataFrame df)
        {

            DataTable dt = new DataTable();

            // 添加列名稱到 DataTable
            foreach (var column in df.Columns)
            {
                dt.Columns.Add(column.Name, column.DataType);
            }
            for (int row = 0; row < df.Rows.Count; row++)
            {
                DataRow dataRow = dt.NewRow();
                for (int col = 0; col < df.Columns.Count; col++)
                {
                    dataRow[col] = df.Rows[row][col];//  df[col][row];
                }
                dt.Rows.Add(dataRow);
            }
            return dt;
        }

        public static string ConvertDataTableToJson(DataTable dataTable)
        {
            string json = string.Empty;

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                json = Newtonsoft.Json.JsonConvert.SerializeObject(dataTable, Newtonsoft.Json.Formatting.Indented);
            }

            return json;
        }

        public static bool HttpWebRequest_Post(string url, ref string data)
        {           
            try
            {
                //TODO:序列化
                var jsonBytes = Encoding.UTF8.GetBytes(data);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Http.Post;
                request.ContentType = "application/json";
                request.ContentLength = jsonBytes.Length;
                request.Timeout = 30000;
                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(jsonBytes, 0, jsonBytes.Length);
                    requestStream.Flush();
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        data = reader.ReadToEnd();
                        return true;
                    }
                }
            }
            catch (WebException ex)
            {
                using (WebResponse response = ex.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    using (Stream rb_data = response.GetResponseStream())
                    using (var reader = new StreamReader(rb_data))
                    {
                        data = reader.ReadToEnd();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                data = "HttpWebRequest_Post Error.Url:" + url + ".Error Message:" + ex.Message;
                return false;
            }
            return false;
        }
        public static bool GetWebAPI(string url, ref string result)
        {
            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = 30000;
            bool IsOk = true;
            // 取得回應資料
            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        result = sr.ReadToEnd();
                    }
                }
            }
            catch (System.Exception ex)
            {
                result = ex.Message;
                IsOk = false;
            }
            return IsOk;

        }

        public static bool GetWebAPI2(string url, ref string result)
        {
            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Timeout = 30000;
            bool IsOk = true;
            // 取得回應資料
            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        result = sr.ReadToEnd();
                    }
                }
            }
            catch (System.Exception ex)
            {
                result = ex.Message;
                IsOk = false;
            }
            return IsOk;

        }
        public static string GetWebAPIDataframe(string url)
        {
            string data = "";
            bool IsOk = GetWebAPI(url, ref data);
            return data;
        }

        public static string ConvertDataTabletoString(DataTable dt)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);
        }

        public static string GetFit(DataTable dt)
        {
            string url = "http://127.0.0.1:8000/GetFit/";

            string data = ConvertDataTabletoString(dt);
            bool IsOk = HttpWebRequest_Post(url, ref data);
              
            Dictionary<string, object> dicData = new Dictionary<string, object>();
            //string json = JsonConvert.SerializeObject(dt, Formatting.Indented);
            
            return data;
        }
        public static void DataTableToCsv(DataTable dataTable,string fileName)
        {
            // 使用 StringWriter 來建立 CSV 格式的字串
            StringWriter csvString = new StringWriter();

            // 寫入表頭
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                csvString.Write(dataTable.Columns[i].ColumnName);
                // 檢查是否是表頭的最後一個元素，如果不是，則寫入逗號
                if (i < dataTable.Columns.Count - 1)
                {
                    csvString.Write(",");
                }
            }
            csvString.WriteLine();

            // 寫入資料
            foreach (DataRow row in dataTable.Rows)
            {

                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    csvString.Write(row.ItemArray[i].ToString());
                    // 檢查是否是該行的最後一個元素，如果不是，則寫入逗號
                    if (i < row.ItemArray.Length - 1)
                    {
                        csvString.Write(",");
                    }
                }
                csvString.WriteLine();
            }
            File.WriteAllText(fileName, csvString.ToString()); 
        }
        public static DataTable GetTableByWebApi(string url)
        {
            string json = MyData.GetWebAPIDataframe(url);
            Dictionary<string, object> values = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            DataTable lTable2 = MyData.CreateTable(4);
            foreach (object item in values.Values)
            {
                Dictionary<string, string> values2 = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.ToString());
                string colstr = "";
                foreach (string col in values2.Keys)
                {
                    if (colstr != "")
                        colstr += ",";
                    colstr += col;
                }
                lTable2 = MyData.CreateTableByColumn(colstr);
                break;
            }
            foreach (object item in values.Values)
            {
                Dictionary<string, string> values2 = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.ToString());
                DataRow AddRow = lTable2.NewRow();
                foreach (string col in values2.Keys)
                {
                    AddRow[col] = values2[col];
                }
                lTable2.Rows.Add(AddRow);
            }
            return lTable2;
        }

        public DataTable FineDataTable(string lSql)
        {
            DataTable lTable = new DataTable();
            bool IsOk = false;
            int i = 1;
            while (!IsOk && i < 10)
            {
                i++;
                try
                {
                    SqlConnection SqlConnection1 = new SqlConnection(lConnstr);
                    SqlDataAdapter SqlDataAdapter1 = new SqlDataAdapter();
                    SqlConnection1.Open();
                    SqlDataAdapter1.SelectCommand = new SqlCommand(lSql, SqlConnection1);
                    SqlDataAdapter1.Fill(lTable);
                    SqlConnection1.Close();
                    IsOk = true;
                }
                catch (System.Exception ex1)
                {
                    FunLog.WriteLog(ex1.Message + "sql" + lSql, "conn_error");
                    IsOk = false;
                }
            }
            return lTable;
        }
        public string FineDataField(string sql)
        {
            string field = "";
            try
            {
                DataTable lTable = FineDataTable(sql);
                if(lTable!=null && lTable.Rows.Count!=0){
                    field = lTable.Rows[0][0].ToString();
                }
            }
            catch (System.Exception ex1)
            {
                FunLog.WriteLog(ex1.Message, "conn_error");
            }
            return field;
        }

        public static string DateTime2SQL(DateTime InDate)
        {
            return InDate.Year + "/" + InDate.Month.ToString().PadLeft(2, '0') + "/" + InDate.Day.ToString().PadLeft(2, '0') + " " + InDate.Hour.ToString().PadLeft(2, '0') + ":" + InDate.Minute.ToString().PadLeft(2, '0') + ":" + InDate.Second.ToString().PadLeft(2, '0');
        }

        public static string DateTime2Oracle(DateTime InDate)
        {
            return "to_date('"+ InDate.Year+"/"+InDate.Month.ToString().PadLeft(2, '0')+"/"+ InDate.Day.ToString().PadLeft(2, '0')+" "+InDate.Hour.ToString().PadLeft(2, '0')+":"+InDate.Minute.ToString().PadLeft(2, '0')+":"+InDate.Second.ToString().PadLeft(2, '0')+"','yyyy/mm/dd hh24:mi:ss')";
        }

        public bool SqlCmdExec(string sql)
        {
            bool IsOk = false;
            SqlConnection SqlConnection1 = new SqlConnection(lConnstr);

            try
            {

                SqlConnection1.Open();
                SqlCommand lCommand = new SqlCommand();
                lCommand.CommandText = sql;
                lCommand.Connection = SqlConnection1;
                lCommand.CommandTimeout = 0;
                int returnNo = lCommand.ExecuteNonQuery();
                IsOk = true;

            }
            catch (System.Exception ex1)
            {

                if(ex1.Message.IndexOf("deadlocked")>-1){

                    SqlCmdExec(sql);
                }
                else if (ex1.Message.IndexOf("無法評估") < 0)
                {
                    FunLog.WriteLog(ex1.Message + "=> " + sql, "SqlCmdExec");
                    //throw;
                }
                
            }
            try
            {
                SqlConnection1.Close();
            }
            catch (System.Exception ex1)
            {
                FunLog.WriteLog(ex1.Message + "=> " + sql, "SqlCmdExec");
            }
            return IsOk;

        }
        //public bool SqlCmdExec(string sql)
        //{
        //    bool IsOk = false;
        //    sqlite_connect = new SQLiteConnection(lConnstr);
            
        //    try
        //    {

        //        sqlite_connect.Open();// Open 
        //        sqlite_cmd = sqlite_connect.CreateCommand();
        //        sqlite_cmd.CommandText = sql;
        //        sqlite_cmd.ExecuteNonQuery();
        //        sqlite_connect.Close();
        //        IsOk = true;
                
        //    }
        //    catch (System.Exception ex1)
        //    {
        //        FunLog.WriteLog(ex1.Message + "=> " + sql, "SqlCmdExec");
        //    }
        //    try
        //    {
        //        sqlite_connect.Close();
        //    }
        //    catch (System.Exception ex1)
        //    {
        //        FunLog.WriteLog(ex1.Message + "=> " + sql, "SqlCmdExec");
        //    }
            
        //    return IsOk;
            
        //}
		public bool SqlCmdExec(string sql,string Type) {
            return SqlCmdExec(sql);
		}

		public bool FineDataRow(string sql,ref DataRow lData) {		
			bool IsFine=false;			
			try {			
				DataTable lTable=FineDataTable(sql);
				DataRow[] lfoundRows =lTable.Select() ;					
				if(lfoundRows.GetUpperBound(0) > -1 ) {
					lData=lfoundRows[0];
					IsFine=true;
				}
				else{
					lData=null;						
				}
			}
			catch(System.Exception   ex1) {
				
			}    
			return IsFine;
		}
        public string[] FineFields(string sql)
        {
            string[] Fields= new string[10];
            try
            {
                DataTable lTable = FineDataTable(sql);
                if(lTable.Rows.Count>0){
                    Fields=new string[lTable.Columns.Count];
                    for(int i=0 ;i< lTable.Columns.Count;i++){
                        Fields[i] = lTable.Rows[0][i].ToString();
                    }
                }else{
                    return null;
                }
                return Fields;
            }
            catch (System.Exception ex1)
            {

            }
            return null;
        }

        public static DataTable CreateTableByColumn(string Rows)
        {
            DataTable lTable = new DataTable("New");
            string[] cols = Rows.Split(',');

            for (int i = 0; i <= cols.GetUpperBound(0); i++)
            {
                DataColumn column = new DataColumn(cols[i], typeof(System.String));
                lTable.Columns.Add(column);
            }

            return lTable;
        }

        public static DataTable CreateTable(string json)
        {
            DataTable lTable = new DataTable("New");
            IDictionary<string, object> dict = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);
            foreach (string key2 in dict.Keys)
            {
                IDictionary<string, object> dict2 = JsonConvert.DeserializeObject<IDictionary<string, object>>(dict[key2].ToString());
                foreach (string key in dict2.Keys)
                {
                    if (key == "stockno")
                    {
                        DataColumn column = new DataColumn(key, typeof(System.String));
                        lTable.Columns.Add(column);
                    }
                    else if (key.ToUpper() == "DATE" || key.ToUpper() == "DATES")
                    {
                        DataColumn column = new DataColumn(key, typeof(System.DateTime));
                        lTable.Columns.Add(column);
                    }
                    else
                    {
                        DataColumn column = new DataColumn(key, typeof(float));
                        lTable.Columns.Add(column);
                    }
                }
                break;
            }



            foreach (string key in dict.Keys)
            {
                IDictionary<string, object> DateData = JsonConvert.DeserializeObject<IDictionary<string, object>>(dict[key].ToString());
                DataRow addRow = lTable.NewRow();
                foreach (string column in DateData.Keys)
                {
                    if (column.ToUpper() == "DATE" || column.ToUpper() == "DATES")
                    {
                        addRow[column] = DateTime.Parse(DateData[column].ToString());
                    }
                    else if (column == "stockno")
                    {
                        addRow[column] = DateData[column].ToString();
                    }
                    else
                    {
                        if (DateData[column] == null)
                            addRow[column] = DBNull.Value;
                        else
                            addRow[column] = float.Parse(DateData[column].ToString());
                    }

                }

                lTable.Rows.Add(addRow);


            }
            return lTable;
        }
        public static DataTable CreateTable(string Rows, string type)
        {
            DataTable lTable = new DataTable("New");
            string[] cols = Rows.Split(',');
            string[] types = type.Split(',');

            for (int i = 0; i <= cols.GetUpperBound(0); i++)
            {
                if (types[i] == "string")
                {
                    DataColumn column = new DataColumn(cols[i], typeof(System.String));
                    lTable.Columns.Add(column);
                }
                else if (types[i] == "int")
                {
                    DataColumn column = new DataColumn(cols[i], typeof(System.Int32));
                    lTable.Columns.Add(column);
                }
                else if (types[i] == "double")
                {
                    DataColumn column = new DataColumn(cols[i], typeof(System.Double));
                    lTable.Columns.Add(column);
                }
                else if (types[i] == "DateTime")
                {
                    DataColumn column = new DataColumn(cols[i], typeof(System.DateTime));
                    lTable.Columns.Add(column);
                }
            }

            return lTable;
        }

        public static DataTable CreateTable(int cols)
        {
            DataTable lTable = new DataTable("Table");             
            for (int i = 0; i < cols; i++)
            {
                DataColumn column = new DataColumn("Row" + i.ToString(), typeof(System.String));
                lTable.Columns.Add(column);
            }
            return lTable;
        } 
        public ArrayList FineDataArrayList(string sql)
        {
            ArrayList lList = new ArrayList();
            try
            {
                DataTable lTable = FineDataTable(sql);                
                if (lTable.Rows.Count > 0)
                {
                    for (int i = 0; i < lTable.Columns.Count; i++) {
                        lList.Add(lTable.Rows[0][i].ToString());
                    }
                }
            }
            catch (System.Exception ex1)
            {

            }
            return lList;
        }
        public ArrayList FineDataArrayListV(string sql)
        {
            ArrayList lList = new ArrayList();
            try
            {
                DataTable lTable = FineDataTable(sql);
                foreach (DataRow lRow in lTable.Rows) {
                    lList.Add(lRow[0].ToString());
                }            
            }
            catch (System.Exception ex1)
            {

            }
            return lList;
        }

        public ArrayList FineOneFieldArrayList(string sql)
        {
            ArrayList lList = new ArrayList();
            try
            {
                DataTable lTable = FineDataTable(sql);
                foreach (DataRow lRow in lTable.Rows)
                {
                    lList.Add(lRow[0].ToString());
                }      
            }
            catch (System.Exception ex1)
            {

            }
            return lList;
        }

        public string[] FineOneFieldData(string sql)
        {
            string[] Fields = new string[1];
            try
            {
                DataTable lTable = FineDataTable(sql);
                Fields = new string[lTable.Rows.Count];
                for (int i = 0; i < lTable.Rows.Count; i++)
                {
                    Fields[i] = lTable.Rows[i][0].ToString();
                }
            }
            catch (System.Exception ex1)
            {

            }
            return Fields;
        }

//		OleDbConnection cn = new OleDbConnection();
//		cn.ConnectionString = sConnectionString;
//		cn.Open();
//
//		string sSQL = "SELECT * FROM [Sheet1$]";
//		OleDbCommand cmd = cn.CreateCommand();
//		cmd.CommandText = sSQL;
//		OleDbDataReader DR = cmd.ExecuteReader();
//		while(DR.Read())
//	{
//		Console.WriteLine(DR[0].ToString());
//	}
//
//	cn.Close();

        

		public bool IsData(string sql) 
		{		
			bool IsFine=false;			
			try 
			{			
				DataTable lTable=FineDataTable(sql);
				DataRow[] lfoundRows = lTable.Select() ;					
				if(lfoundRows.GetUpperBound(0) > -1 ) 
				{					
					IsFine=true;
				}				
			}
			catch(System.Exception   ex1) 
			{

			}    
			return IsFine;
		}

        public static void CreateDBTable(DataTable lTable, string TableName, string colName, string colType, bool Ishead)
        {
            string sql = "";
            string[] colNames = colName.Split(new string[] { "," }, StringSplitOptions.None);
            string[] colTypes = colType.Split(new string[] { "," }, StringSplitOptions.None);
            sql = " CREATE TABLE [dbo].[" + TableName + "]([IndexNo] [int] IDENTITY(1,1) NOT NULL";

            for (int i = 0; i < lTable.Columns.Count; i++)
            {
                if (colName == "")
                    sql += " ,[Col" + i.ToString() + "] ";
                else
                    sql += " ,[" + colNames[i].ToString() + "] [nvarchar](50) NULL ";
                if (colType == "")
                {
                    sql += " [nvarchar](50) NULL ";
                }
                else
                {
                    if (colTypes[i] == "int")
                        sql += " [int] NULL ";
                    else if (colTypes[i] == "string")
                        sql += " [nvarchar](50) NULL ";
                    else if (colTypes[i] == "datetime")
                        sql += " [datetime] NULL ";
                    else if (colTypes[i] == "float")
                        sql += " [float] NULL ";
                    else if (colTypes[i] == "bigint")
                        sql += " [bigint] NULL ";
                }
            }
            sql += ") ON [PRIMARY]";
            MyDatas.lData.SqlCmdExec(sql);
            for (int i = 0; i < lTable.Columns.Count; i++)
            {
                if (Ishead)
                {
                    string oldcol = "Col" + i.ToString();
                    if (colName != "")
                        oldcol = colNames[i].ToString();
                    AddDesc(TableName, oldcol, lTable.Rows[0][i].ToString());
                }
            } 
        }


        public static DataTable ArrayList2Table(ArrayList lList)
        {
            string[] fs=(string[])lList[0];
            
            DataTable lTable = MyData.CreateTable(fs.Length);
            foreach (string[] item in lList)
            {
                DataRow lRow = lTable.NewRow();
                for (int i = 0; i < fs.Length; i++)
                {
                    lRow[i] = item[i];
                }
                lTable.Rows.Add(lRow);
            }
            return lTable;
        }
        public static void DataTable2DB(DataTable lTable,string TableName,string colName,string colType,bool Ishead){
            if (lTable.Rows.Count < 1)
                return;
            string sql="";
            string[] colNames=colName.Split(new string[]{","}, StringSplitOptions.None);
            if (!HasTable(TableName))
                CreateDBTable(lTable,TableName,colName,colType,Ishead);
            Table2DB(lTable,TableName,Ishead);            
        }

        public static void Table2DB(DataTable lTable,string TableName,bool Ishead){
            DataTable colTable = GetTableColumn(TableName);
            int i = 0;
            int k = 0;
            string colNameStr = "";
            string[] colTypes = new string[colTable.Rows.Count-1];
            string[] colNames = new string[colTable.Rows.Count-1];
             string sql="";
            foreach (DataRow Row in colTable.Rows)
            {
                if (Row[0].ToString() != "IndexNo")
                {
                    if (colNameStr != "")
                        colNameStr += ",";
                    colNameStr +="["+ Row[0].ToString() + "]";
                    colTypes[i] = Row[1].ToString();
                    colNames[i] =Row[0].ToString();
                    i++;
                }
            }
            i=0;
            sql = "insert into " + TableName + "(" + colNameStr + ") values";
            string values = "";
            foreach (DataRow item in lTable.Rows)
            {
                if (Ishead && i == 0)
                {
                    i++;
                    continue;
                }
                if (k > 500 || i == lTable.Rows.Count-1)
                {
                    if(values!="")
                        MyDatas.lData.SqlCmdExec(sql+values);
                    k = 0;
                    values = "";
                    if (i == lTable.Rows.Count - 1)
                        break;
                }
                string valuestr = "";
                for (int j = 0; j < colTypes.Length; j++)
                {
                    if (valuestr != "")
                        valuestr += ",";
                    if (colTypes[j] == "datetime" || colTypes[j] == "nvarchar")
                        valuestr += "'" + item[j].ToString() + "'";
                    else
                        valuestr +=  item[j].ToString() ;
                }
                string sqls = "select * from " + TableName + " where " + colNames[0].ToString() + "='" + item[0].ToString() + "' and " + colNames[1].ToString() + "='" + item[1].ToString() + "'  and " + colNames[2].ToString() + "='" + item[2].ToString() + "'";
                if (!MyDatas.lData.IsData(sqls))
                {
                    if (values != "")
                        values += ",";
                    values+="(" + valuestr + ")";
                    k++;
                }
                i++;
            }
        }

        public static DataTable GetTableColumn(string TableName)
        {
            string sql = "SELECT COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + TableName + "'";
            return MyDatas.lData.FineDataTable(sql);
        }

        public static void AddDesc(string TableName, string Name, string Desc)
        {
            string sql = "EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'" + Desc + "' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'" + TableName + "', @level2type=N'COLUMN',@level2name=N'" + Name + "'";
            MyDatas.lData.SqlCmdExec(sql);
        }

        public static bool HasTable(string TableName){
            string sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES where Table_Name='"+TableName+"'";
            return MyDatas.lData.IsData(sql);
        }

        public static DataTable StockDataM()
        {
            string Date = My.iniFile.iniReadValue("System", "Export");
            string sql = "select * from StockDataM where Dates >'"+ Date + "' order by Dates ";
            DataTable lTable= MyDatas.lData.FineDataTable(sql);
            sql = "select Max(Dates) from StockDataM"; 
            My.iniFile.iniWriteValue("System", "Export", MyDatas.lData.FineDataField(sql));
            return lTable;
        }

    }
    //System.Configuration.ConfigurationManager.AppSettings["SiteUrl"].ToString();

    public static class MyDatas
    {
        public static DataTable GetIndex(string stockno, IndexClass index, string startdate)
        {
            if (startdate == "")
                startdate = "2021-08-04";
            string url = "http://localhost:800/Talib/GetAllIndex/?stockno=" + stockno + "&start=" + startdate + "&end=&Indexs=" + index._name + "&paras=" + index._para;
            Tuple<string, string> result = WebAPI.HttpWebRequest_Get<string>(url);
            if (result.Item1 == "")
            {
                DataTable lTable = MyData.CreateTable(result.Item2);
                return lTable;
            }
            return null;
        }

        public static DataTable GetIndexDB(string stockno, IndexClass index, string startdate)
        {
            if (startdate == "")
                startdate = "2021-08-04";
            string url = "http://localhost:800/Talib/GetAllIndex/?stockno=" + stockno + "&start=" + startdate + "&end=&Indexs=" + index._name + "&paras=" + index._para;
            Tuple<string, string> result = WebAPI.HttpWebRequest_Get<string>(url);
            if (result.Item1 == "")
            {
                DataTable lTable = MyData.CreateTable(result.Item2);
                return lTable;
            }
            return null;
        }

        public static DataTable GetIndex1(string stockno)
        {
            string sql = "select * from vw_StockData where stockno='" + stockno + "' order by dates";
            DataTable lTable = MyDatas.lData.FineDataTable(sql);
            return lTable;
        }

        public static Dictionary<string, DataTable> GetIndexAll(  IndexClass index)
        { 
            string url = "http://localhost:800/Talib/GetAllIndex/?start=&end=&Indexs=" + index._name + "&paras=" + index._para;
            Tuple<string, string> result = WebAPI.HttpWebRequest_Get<string>(url);
            if (result.Item1 == "")
            {
                DataTable lTable = MyData.CreateTable(result.Item2);
                Dictionary<string, DataTable> stockdata = new Dictionary<string, DataTable>();
                DataTable stocklist = lTable.DefaultView.ToTable(true, "stockno");
                foreach (DataRow item in stocklist.Rows)
                {
                    DataView dv = new DataView();
                    dv.Table = lTable;
                    dv.RowFilter = "stockno='" + item["stockno"].ToString() + "'";
                    DataTable nTable = dv.ToTable();
                    stockdata.Add(item["stockno"].ToString(), nTable);
                }
                return stockdata;
            }
            return null;
        }
        public static MyData lData = new MyData(ConfigurationSettings.AppSettings["DB"].ToString());
        public static MyStockData StockData = new MyStockData();
        //public static MyData lData = new MyData(iniFile.iniReadValue("Conn", "Conn1") + System.Environment.MachineName);
        //public static MyData lData = new MyData(iniFile.iniReadValue("Conn", "Conn1"));
        //iniFile.iniReadValue("Conn", "Conn1")
        //public static MyData lData2 = new MyData(iniFile.iniReadValue("Conn", "DB2"));
        //public static MyData lData = new MyData(ConfigurationSettings.AppSettings["DB1"].ToString());
      
        //public static MyData lData2 = new MyData(ConfigurationSettings.AppSettings["DB2"].ToString());
        //public static MyData lData3 = new MyData(ConfigurationSettings.AppSettings["DB3"].ToString());
    }
}
