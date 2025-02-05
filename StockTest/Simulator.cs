using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.Analysis;
using Newtonsoft.Json;
using FastMember;
using My;
using System.Threading;
using ZedGraph;
using System.IO;
using Microsoft.Spark.Sql.Types;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System.Data.SqlClient;
using System.Security;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace StockTest
{
    public partial class Simulator : Form
    {
        public Simulator()
        {
            InitializeComponent();
        }
        //事件事件事件事件事件事件事件事件事件事件事件事件事件事件事件事件事件事件事件事件事件事件事件事件
        private void collapsePanel2_MenuDoubleClick(object sender, EventArgs e)
        {
            TreeView tree = sender as TreeView;
            if (tree == null)
                return;
            var node = tree.SelectedNode;
            if (node == null)
                return;
            //只对无子菜单的菜单弹出窗口
            if (node.Nodes.Count == 0)
            {
                if (node.Tag.ToString() == "StockNo")
                {
                    
                    lGraphic.IsBBANDS = true;
                    lGraphic.IsMACD = true;
                    lGraphic.IsShowPanel2 = false;
                    ShowStock(node.Text);
                    node.BackColor = Color.Pink; 
                }
                if (node.Tag.ToString() == "Graphic") {
                    if (node.Text.ToString() == "hidden")
                    {
                        lGraphic.ZedGraphControl.Visible = false;
                    }
                    else if (node.Text.ToString() == "volume")
                    {
                        lGraphic.IsShowPanel2 = true;
                        lGraphic.FirstDraw();
                    }
                    else if (node.Text.ToString() == "hiddenV")
                    {
                        lGraphic.IsShowPanel2 = false;
                        lGraphic.FirstDraw();
                    }
                    else if (node.Text.ToString() == "KD")
                    {
                        lGraphic.IsSTOCH = !lGraphic.IsSTOCH;
                        ShowStock(node.Text);
                    }
                } 

            }
        }
        private bool IsMenu(string StockNo)
        {
            foreach (MenuData item in this.collapsePanel2.Menus)
            {
                if (item.Name == StockNo)
                    return true;
            }
            return false;
        }
        private void AddMenu(string StockNo)
        {
            if (!IsMenu(StockNo))
            {
                this.collapsePanel2.Menus.Add(new MenuData() { Id = this.collapsePanel2.Menus[this.collapsePanel2.Menus.Count - 1].Id + 1, ParentId = 1, Name = StockNo, Path = "StockNo", BackColor = Color.White }); this.collapsePanel2.InitMenus();
                
            }
        }
        private void AddMenu(int ParentId,string Menun,string Tag)
        {
            if (!IsMenu(Menun))
            {
                this.collapsePanel2.Menus.Add(new MenuData() { Id = this.collapsePanel2.Menus[this.collapsePanel2.Menus.Count - 1].Id + 1, ParentId = ParentId, Name = Menun, Path = Tag, BackColor = Color.White  }); this.collapsePanel2.InitMenus();
            }
        }
        private void Simulator_Load(object sender, EventArgs e)
        {


            List<MenuData> menuList = new List<MenuData>()
            {
                new MenuData(){ Id=1,ParentId=null,Name="標的",Path="it", BackColor=Color.White },
                new MenuData(){ Id=2,ParentId=null,Name="條件",Path="condition" , BackColor=Color.White},
                new MenuData(){ Id=3,ParentId=null,Name="組合條件",Path="conditions" , BackColor=Color.White},
                new MenuData(){ Id=4,ParentId=null,Name="現有標的",Path="current" , BackColor=Color.White},
                new MenuData(){ Id=5,ParentId=null,Name="關注",Path="favorite" , BackColor=Color.White},
                new MenuData(){ Id=6,ParentId=null,Name="圖型設定",Path="Graphic" , BackColor=Color.White},
                //new MenuData(){ Id=4,ParentId=1,Name="show",Path="show", BackColor=Color.White },
                //new MenuData(){ Id=5,ParentId=1,Name="hidden",Path="hidden", BackColor=Color.White },
                //new MenuData(){ Id=6,ParentId=1,Name="volume",Path="volume" , BackColor=Color.White},
                //new MenuData(){ Id=7,ParentId=1,Name="hiddenV",Path="hiddenV" , BackColor=Color.White},
                //new MenuData(){ Id=8,ParentId=1,Name="KD",Path="KD", BackColor=Color.White },
               
            };
            

            if (this.collapsePanel2 == null)
                this.collapsePanel2 = new CollapsePanel();
            this.collapsePanel2.Menus.AddRange(menuList);
            this.collapsePanel2.InitMenus();
            List<string> ConditionList = ConditionALL.GetConditionList();
            foreach (string item in ConditionList)
            {
                AddMenu(2, item, "Condition");
            }
            AddMenu(1, "法人Buy","SelectStock");
            AddMenu(1, "大量", "SelectStock");
            AddMenu(1, "價格", "SelectStock");

            AddMenu(6, "hidden", "Graphic");
            AddMenu(6, "volume", "Graphic");
            AddMenu(6, "hiddenV", "Graphic");
            AddMenu(6, "KD", "Graphic");

            collapsePanel2.AllowDrop = true;//可以當拖曳來源
            string StockList = iniFile.iniReadValue("StockNo", "StockList");
            if(StockList != "")
            {
                string[] split = { "," };
                string[] StockNos = StockList.Split(new string[]  { "," }, StringSplitOptions.None);
                foreach (var item in StockNos)
                {
                    AddMenu(5,item.ToString(),"StockNo");
                }
            }
            //this.splitContainer2.Panel1.Controls.Add(collapsePanel1);
            //this.splitContainer2.SplitterDistance    = collapsePanel1.Width-50;
            tabControl2.SelectTab(1);
            InitGraphic();
            RefreshTradeList();
            //lGraphic.Update("2344", "90");
        }
        private MyGraphic lGraphic;
        private bool IsinitGraphic = false;
        private void InitGraphic()
        {
            IsinitGraphic = true;
            lGraphic = new MyGraphic("", "", DemoType.Self);
            this.splitContainer2.Panel2.Controls.Clear();
            this.splitContainer2.Panel2.Controls.Add(lGraphic.ZedGraphControl);
            lGraphic.ZedGraphControl.IsEnableWheelZoom = false;//滾輪放大縮小
            lGraphic.ZedGraphControl.IsEnableZoom = false;
            lGraphic.ZedGraphControl.Width = this.splitContainer2.Panel2.Width;
            lGraphic.ZedGraphControl.Height = this.splitContainer2.Panel2.Height;
            lGraphic.ZedGraphControl.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

            lGraphic.DragDrop += LGraphic_DragDrop;

        }
        private void LGraphic_DragDrop(object sender, DragEventArgs e)
        {
            string str = (string)e.Data.GetData(DataFormats.StringFormat);
            if (str == "KD")
            {
                STOCH lStoch = new STOCH();
                lGraphic.AddIndex(lStoch);
                lGraphic.UpdateIndex();
            }
        }
        private void splitContainer2_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }
        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            ////splitContainer1.SplitterDistance = splitContainer1.SplitterDistance ==0 ?  50 : 0;
            //if(splitContainer1.SplitterDistance == 1)
            //    splitContainer1.SplitterDistance = 100;
            //else
            //    splitContainer1.SplitterDistance = 1;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            splitContainer1.SplitterDistance = 1;
        }
        private void splitContainer1_Click(object sender, EventArgs e)
        {
            if (splitContainer1.SplitterDistance > 10)
                splitContainer1.SplitterDistance = 1;
            else
                splitContainer1.SplitterDistance = 100;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (splitContainer1.SplitterDistance != 15)
                splitContainer1.SplitterDistance = 15;
            else
                splitContainer1.SplitterDistance = 100;
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (splitContainer2.SplitterDistance != 15)
                splitContainer2.SplitterDistance = 15;
            else
                splitContainer2.SplitterDistance = 100;
        }
        private void splitContainer2_Panel2_DragDrop(object sender, DragEventArgs e)
        {
            string ss = "";
        }
        private Dictionary<string, My.Signal> TradeLog = new Dictionary<string, My.Signal>();
        private void button3_Click(object sender, EventArgs e)
        {
            string[] files = FunLog.GetFolderFile("database\\" + DateTime.Now.ToString("yyyyMMdd"));
            foreach (string item in files)
            {
                string[] parts = item.Split('_');
                if (parts.Length > 0)
                {
                    string stockno = parts[0];
                    DateTime sDate = new DateTime(int.Parse(parts[2].Substring(0, 4)), 1, 1);
                    DateTime eDate = new DateTime(int.Parse(parts[3].Substring(0, 4)), 1, 1);
                    string filename = stockno + "_" + parts[1] + "_" + sDate.ToString("yyyyMMdd") + "_" + eDate.ToString("yyyyMMdd");
                    DataTable Table = MyData.JsonToDataTable("database\\fix\\" + filename);
                    MyBackTest.DataTableToCsv(Table, "database\\"  + filename + ".csv");
                }
            }
        }
        private void button4_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }
        private void button3_MouseDown(object sender, MouseEventArgs e)
        {
            button3.DoDragDrop(button3.Text, DragDropEffects.Move);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            DataTable lTable = MyData.JsonToDataTable("STOCHWEEKLY");

            DataTable lTable2 = lTable.Clone();
            foreach (DataRow item in lTable.Rows)
            {
                if ((long)item["TradeCount"] >= 10)
                    lTable2.ImportRow(item);
            }
            lTable = MyData.GetTableAvg(lTable2);
            lTable = MyData.Sort(lTable, "Kelly", false);
            dataGridView1.DataSource = lTable;
            //MyData.DataTableToJson(lTable, "StochWeekly");
        }
        private void splitContainer2_Panel2_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }
        private void button5_Click(object sender, EventArgs e)
        {
            splitContainer1.SplitterDistance = 900;
        }
        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                string value = selectedRow.Cells[2].Value.ToString();
                List<Trade> list = GALog[value];

                //Dictionary<string, BBSignal> lTable = MyData.JsonToObj<Dictionary<string, BBSignal>>("BBondsWeeklylog");
                //BBSignal lKDSingal = lTable[value];

                DataTable dataTable = new DataTable();
                using (var reader = ObjectReader.Create(list))
                {
                    dataTable.Load(reader);
                }

                dataGridView1.DataSource = dataTable;
            }

        }
        private void button6_Click(object sender, EventArgs e)
        {
            DataTable lTable = MyData.JsonToDataTable("StochWeeklyFilter");

            DataTable lTable2 = lTable.Clone();
            foreach (DataRow item in lTable.Rows)
            {
                if ((long)item["TradeCount"] >= 10)
                    lTable2.ImportRow(item);
            }
            lTable2 = MyData.Sort(lTable2, "Kelly", false);
            lTable = MyData.GetTableAvg(lTable2);
            dataGridView1.DataSource = lTable;
        }
        private void button7_Click(object sender, EventArgs e)
        {
            MessageBox.Show( MyBackTest.GetSimStock(msg.Text));
        }
        private void button8_Click(object sender, EventArgs e)
        {
            //BBANDS lBBANDS = new BBANDS();
            //DataTable lTable = MyGraphic.GetDataTable("2344", lBBANDS, "");
            //dataGridView1.DataSource = lTable;

            DataTable lTable = MyData.CreateTable("StockNo,TradeCount,WinFisrtRate,WinRate,WinLoseRate,Kelly,KeepTimeRange", "string,int,double,double,double,double,int");
            ArrayList Stocks = MyDatas.lData.GetStockList();

            if (msg.Text != "")
            {
                BBANDS lIndex = new BBANDS();
                DataFrame df = MyGraphic.GetData(msg.Text, "");
                BBSignal Signal = new BBSignal(msg.Text);
                Signal.Init(df, new string[] { "" });
                ArrayList lList = Signal.profit();
                List<Trade> lTrades = Signal.Trades;

                MyData.ObjToJson(Signal, "BBondsWeeklylog1");
                BBSignal lTablxe = MyData.JsonToObj<BBSignal>("BBondsWeeklylog1");
                return;
            }
            List<Trade> Trades = new List<Trade>();
            foreach (string stockno in Stocks)
            {
                BBANDS lIndex = new BBANDS();
                DataFrame df = MyGraphic.GetData(stockno, "");
                BBSignal Signal = new BBSignal(stockno);
                Signal.Init(df, new string[] { "" });
                ArrayList lList = Signal.profit();
                Trades.AddRange(Signal.Trades);
                DataRow lRow = lTable.NewRow();
                lRow.ItemArray = lList.ToArray();
                if ((int)lRow[1] != 0)
                {
                    TradeLog.Add(stockno, Signal);
                    lTable.Rows.Add(lRow);
                }
                msg.Text = stockno;
                Application.DoEvents();
            }
            DataRow lRowAdd = lTable.NewRow();
            lRowAdd.ItemArray = BBSignal.profit(Trades).ToArray();
            lTable.Rows.Add(lRowAdd);

            MyData.DataTableToJson(lTable, "BBondsWeeklyFilter");
            MyData.ObjToJson<Dictionary<string, My.Signal>>(TradeLog, "BBondsWeeklylog");
            //lTable = MyData.GetTableAvg(lTable);
            dataGridView1.DataSource = lTable;
        }
        private void button4_Click_1(object sender, EventArgs e)
        {
            DataTable lTable = MyData.JsonToDataTable("BBondsWeeklyFilter");

            DataTable lTable2 = lTable.Clone();
            foreach (DataRow item in lTable.Rows)
            {
                if ((long)item["TradeCount"] >= 10)
                    lTable2.ImportRow(item);
            }
            lTable2 = MyData.Sort(lTable2, "Kelly", false);
            lTable = MyData.GetTableAvg(lTable2);
            dataGridView1.DataSource = lTable;
        }
        private void button9_Click(object sender, EventArgs e)
        {
            Dictionary<string, BBSignal> lTable = MyData.JsonToObj<Dictionary<string, BBSignal>>("BBondsWeeklylog");
            DataTable dataTableAll = new DataTable();
            foreach (string stockno in lTable.Keys)
            {
                BBSignal lKDSingal = lTable[stockno];
                DataTable dataTable = new DataTable();

                using (var reader = ObjectReader.Create(lKDSingal.Trades))
                {
                    dataTable.Load(reader);
                }
                dataTable.Columns.Add("StockNo", typeof(string));
                foreach (DataRow row in dataTable.Rows)
                {
                    row["StockNo"] = stockno;
                }
                dataTableAll.Merge(dataTable);

            }
            dataTableAll.Columns.Add("StockName", typeof(string));
            Dictionary<string, string[]> StockName = MyDatas.lData.GetStockListHash();
            foreach (DataRow item in dataTableAll.Rows)
            {
                item["StockName"] = StockName[item["StockNo"].ToString()][0];
            }


            dataTableAll = MyData.Sort(dataTableAll, "_Date1", false);
            //foreach (DataRow item in dataTableAll.Rows)
            //{

            //}


            dataGridView1.DataSource = dataTableAll;
        }
        private (List<Trade>, double) GetUnitTradeLog(string stock, string Period, string Unit, int sYear, int YearCount)
        {
            string[] numberStrings = Unit.Split(',');
            List<int> GAUNIT = new List<int>();
            foreach (string numStr in numberStrings)
            {
                GAUNIT.Add(int.Parse(numStr));
            }
            STOCH lStoch = new STOCH();
            DateTime sDate = new DateTime(sYear, 3, 1);
            DateTime eDate = new DateTime(sYear + YearCount, 12, 31);
            DataFrame df = MyGraphic.GetDateData(stock, Period, sDate, eDate);
            GASignal GASiganals = new GASignal(msg.Text);
            double fitness = GASiganals.GenProfit(GAUNIT, df);
            return (GASiganals.Trades, fitness);
        }
        private void GetOneJob(string gaunit)
        {
            DateTime sDate = new DateTime(start_year, 1, 1);
            DateTime eDate = new DateTime(start_year + (int)yearRange.Value, 1, 1);
            DataFrame df = MyGraphic.GetDateData(msg.Text, txtPeriod.Text.ToString(), sDate, eDate);

            GASignal GASiganals = new GASignal(msg.Text);
            List<int> GAUNIT = gaunit.Split(',').Select(int.Parse).ToList();
            double fitness = GASiganals.GenProfit(GAUNIT, df);
            dataGridView1.DataSource = MyData.Trade2Table(GASiganals.Trades);

            string datastr = "LastGain:" + GASiganals.LastGain.ToString() + "  LastMoney:" + (GASiganals.LastMoney / 10000).ToString() + "  DropDown:" + GASiganals.DropDown.ToString();
            MSG2.Text = datastr;
            GetMaxDrowDown();
            return;
        }
        private void button10_Click(object sender, EventArgs e)
        {
            if (msg.Text != "")
            {
                GetOneJob(gaunit.Text);
            }
        }
        private IDictionary<string, List<Trade>> GALog = new Dictionary<string, List<Trade>>();
        DataTable gaTable = MyData.CreateTable("GACount,MaxFetness,MaxUnit,MaxGeneration,Period,Seconds", "string,string,string,string,string,string");
        List<int> OptimalConfig = new List<int>();
        DataFrame Optimaldf = new DataFrame();
        private void GAJOB(object _count)
        {
            int count = (int)_count;
            string type = "TX";
            Perioda Period = Perioda.W;
            STOCH lStoch = new STOCH();
            DateTime sDate = new DateTime(2019, 1, 1);
            DateTime eDate = new DateTime(2023, 1, 1);
            DataFrame df = MyGraphic.GetDateData(type, Period.ToString(), sDate, eDate);
            Optimaldf = df;
            GASignal GASiganals = new GASignal(type);
            DateTime lTime1 = DateTime.Now;
            ArrayList lLIst = GASiganals.GetGAOptimal(df, int.Parse(count.ToString()));
            if (lLIst[1] != null)
            {
                string result = string.Join(",", (List<int>)lLIst[1]);
                string sql = "insert into [Anlysis_Unit] (Unit,Period,GainRate,date1,date2,TradeCount,unitcount,DropDown,LastMoney) values('" + result + "','" + Period.ToString() + "'," + GASiganals.GainRate.ToString() + ",'" + sDate.ToString("yyyy/MM/dd") + "','" + eDate.ToString("yyyy/MM/dd") + "'," + GASiganals.TradeCount.ToString() + "," + count + "," + GASiganals.DropDown.ToString() + "," + GASiganals.LastMoney.ToString() + ")";
                MyDatas.lData.SqlCmdExec(sql);
            }

        }
        private void button11_Click(object sender, EventArgs e)
        {
            if (msg.Text != "")
            {
                STOCH lStoch = new STOCH();
                DateTime sDate = new DateTime(2019, 1, 1);
                DataFrame df = MyGraphic.GetDateData(msg.Text, txtPeriod.Text.Trim(), sDate, sDate.AddYears(4));
                Optimaldf = df;
                GASignal GASiganals = new GASignal(msg.Text);
                DateTime lTime1 = DateTime.Now;
                ArrayList lLIst = GASiganals.GetGAOptimal(df, int.Parse(txtGACount.Text));
                double Seconds = (DateTime.Now - lTime1).TotalSeconds;
                string result = string.Join(",", (List<int>)lLIst[1]);
                OptimalConfig = (List<int>)lLIst[1];
                string[] add = new string[] { txtGACount.Text, lLIst[0].ToString(), result, lLIst[2].ToString(), txtPeriod.Text, Seconds.ToString() };
                gaTable.Rows.Add(add);
                GALog.Add(result, (List<Trade>)lLIst[3]);

                dataGridView1.DataSource = gaTable;
                return;
            }
        }
        private void button12_Click(object sender, EventArgs e)
        {
            //DataTable lTable = MyData.CreateTable("StockNo,TradeCount,WinFisrtRate,WinRate,WinLoseRate,Kelly,KeepTimeRange", "string,int,double,double,double,double,int");
            //ArrayList Stocks = MyDatas.lData.GetStockList();
            //foreach (string stockno in Stocks)
            //{
            //    BBANDS lIndex = new BBANDS();
            //    DataFrame df = MyGraphic.GetData(stockno , txtPeriod.Text.Trim());
            //}
            //BBANDS lIndex = new BBANDS();
            //DataFrame df = MyGraphic.GetData(msg.Text, lIndex, "W-SAT");
            //dataGridView1.DataSource = df;

            MyGraphic.GetData(msg.Text, txtPeriod.Text.Trim());


        }
        private void button13_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = MyGraphic.GetDataTable(msg.Text, txtPeriod.Text.Trim());
        }
        private void button14_Click(object sender, EventArgs e)
        {
            int maxWorkerThreads, maxIOThreads;
            ThreadPool.GetMaxThreads(out maxWorkerThreads, out maxIOThreads);
            MSG2.Text = "maxWorkerThreads=" + maxWorkerThreads + "   maxIOThreads" + maxIOThreads;
        }
        private void button16_Click(object sender, EventArgs e)
        {
            DataTable dt = MyGraphic.GetDataTable(msg.Text, txtPeriod.Text.Trim());
            DataTable AnalaysisTable = My.rb.rb1(dt, new int[] { 1, 2, 5, 10, 20 }, new int[] { 10 });
            MyData.DataTableToCsv(AnalaysisTable, "dataMy.csv");
            string xx = MyData.GetFit(AnalaysisTable);
            string ss = xx;
            dataGridView1.DataSource = AnalaysisTable;
            My.MLClass.Fit(AnalaysisTable);
        }
        private void button17_Click(object sender, EventArgs e)
        {
            DataTable dt = MyData.StockDataM();
            string data = "";
            StringBuilder csvContent = new System.Text.StringBuilder();
            csvContent.AppendLine("<DATE>   <TIME>  <OPEN>  <HIGH>  <LOW>   <CLOSE> <TICKVOL>   <VOL>   <SPREAD>");
            foreach (DataRow item in dt.Rows)
            {
                csvContent.AppendLine(DateTime.Parse(item["Dates"].ToString()).ToString("yyyy.MM.dd") + "   " + DateTime.Parse(item["Dates"].ToString()).ToString("HH:mm:ss") + "  " + item["Open"].ToString() + " " + item["High"].ToString() + " " + item["Low"].ToString() + "  " + item["Close"].ToString() + "    " + item["vol"].ToString() + "  " + item["vol"].ToString() + "  1");

            }
            FunLog.WriteFile("TX.csv", csvContent.ToString());
        }
        private void GetMaxDrowDown()
        {
            DataTable lTable = (DataTable)dataGridView1.DataSource;
            int gain = 0;
            DataRow Add = lTable.NewRow();
            int maxGain = 0;
            int maxDrow = 0;
            foreach (DataRow item in lTable.Rows)
            {

                gain += (int)double.Parse(item[8].ToString());
                if (gain > maxGain)
                    maxGain = gain;
                int Drow = maxGain - gain;
                if (Drow > maxDrow)
                    maxDrow = Drow;
                for (int i = 0; i < lTable.Columns.Count; i++)
                {
                    string t1 = lTable.Columns[i].DataType.Name;
                    if (t1 == "Double")
                    {
                        if (Add[i].ToString() == "")
                            Add[i] = double.Parse(item[i].ToString());
                        else
                            Add[i] = double.Parse(Add[i].ToString()) + double.Parse(item[i].ToString());
                    }
                    else if (t1 == "Int32")
                    {
                        if (Add[i].ToString() == "")
                            Add[i] = int.Parse(item[i].ToString());
                        else
                            Add[i] = int.Parse(Add[i].ToString()) + int.Parse(item[i].ToString());
                    }
                    else if (t1 == "TradeType")
                        Add[i] = TradeType.None;
                    else if (t1 == "Boolean")
                        Add[i] = true;
                }
            }

            for (int i = 0; i < lTable.Columns.Count; i++)
            {
                string t1 = lTable.Columns[i].DataType.Name;
                if (t1 == "DateTime")
                    Add[i] = DateTime.Now;
                else if (t1 != "Double" && t1 != "Int32" && t1 != "TradeType" && t1 != "Boolean")
                    Add[i] = t1;
            }
            Add[7] = maxDrow;
            try
            {
                lTable.Rows.Add(Add);
                dataGridView1.DataSource = lTable;
            }
            catch (Exception)
            {

            }

        }
        private void button18_Click(object sender, EventArgs e)
        {

        }
        private void button19_Click(object sender, EventArgs e)
        {
            DateTime sTime = DateTime.Now;
            string sql = "select distinct  Period from Anlysis_Unit ";
            ArrayList period = MyDatas.lData.FineDataArrayListV(sql);
            List<string[]> UnixAll = new List<string[]>();
            int combinationsNeeded = 1000;
            foreach (string period1 in period)
            {
                sql = "SELECT  top(5) unit,INDEXNO  FROM [Anlysis_Unit] where period='" + period1 + "' order by GainRate desc";
                DataTable dt = MyDatas.lData.FineDataTable(sql);
                foreach (DataRow unit1 in dt.Rows)
                {
                    UnixAll.Add(new string[] { period1, unit1[0].ToString(), unit1[1].ToString() });
                }
            }

            Random random = new Random();
            HashSet<string> uniqueCombinations = new HashSet<string>();
            while (uniqueCombinations.Count < combinationsNeeded)
            {
                List<int> selectedNumbers = new List<int>();
                while (selectedNumbers.Count < random.Next(2, 6)) // Randomly choose between 2 to 5 numbers
                {
                    int randomNumber = random.Next(UnixAll.Count);
                    if (!selectedNumbers.Contains(randomNumber))
                    {
                        selectedNumbers.Add(randomNumber);
                    }
                }
                selectedNumbers.Sort();
                string combinationString = string.Join(",", selectedNumbers);
                if (!uniqueCombinations.Contains(combinationString))
                {
                    uniqueCombinations.Add(combinationString);
                }
            }

            List<TagData> maxUnit = new List<TagData>();

            foreach (string Indexs in uniqueCombinations)
            {
                List<Trade> TradeLog = new List<Trade>();
                string[] Indexa = Indexs.Split(',');

                foreach (string item in Indexa)
                {
                    int index = int.Parse(item);
                    string[] unit = UnixAll[index];
                    var TradeResault = GetUnitTradeLog("TX", unit[0], unit[1], 2019, 4);
                    TradeLog.AddRange(TradeResault.Item1);

                }
                List<Trade> sortedTradeLog = TradeLog.OrderBy(Trade => Trade._Date2).ToList();
                DateTime sDate3 = new DateTime(2019, 3, 1);
                DateTime eDate3 = new DateTime(2023, 1, 1);
                DataFrame df3 = MyGraphic.GetDateData("TX", "", sDate3, eDate3);

                double[] DropDown = GASignal.GetDropDown("TX", sortedTradeLog, 2, df3);

                maxUnit.Add(new TagData(Indexs, DropDown[0] / DropDown[1], DropDown[2], DropDown[0]));


            }
            TagData maxTagData = maxUnit.OrderByDescending(x => x.Value3).FirstOrDefault();
            string[] Strategys = maxTagData.Text.Split(',');
            List<Trade> TradeLog2 = new List<Trade>();
            foreach (string item in Strategys)
            {
                int index = int.Parse(item);
                string[] unit = UnixAll[index];
                var TradeResault = GetUnitTradeLog("TX", unit[0], unit[1], 2023, 2);
                TradeLog2.AddRange(TradeResault.Item1);
            }
            List<Trade> sortedTradeLog2 = TradeLog2.OrderBy(Trade => Trade._Date2).ToList();
            DateTime sDate1 = new DateTime(2019, 3, 1);
            DateTime eDate1 = new DateTime(2023, 1, 1);
            DataFrame df2 = MyGraphic.GetDateData("TX", "", sDate1, eDate1);
            double[] DropDown2 = GASignal.GetDropDown("TX", sortedTradeLog2, 2, df2);

            string msg = DropDown2[2].ToString() + "--" + maxTagData.Value3 + "--" + maxTagData.Value + "--" + maxTagData.Text + "--" + (DateTime.Now - sTime).TotalSeconds.ToString();
            MessageBox.Show(msg);
        }
        private void dataGridView2_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView2.SelectedRows[0];
                string value = selectedRow.Cells[0].Value.ToString();
                GetOneJob(value);
            }
        }
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        public void RefreshTradeList()
        {
            string sql = "select unit,gainrate,lastmoney,tradecount from Anlysis_Unit order by lastmoney desc";
            dataGridView2.DataSource = MyDatas.lData.FineDataTable(sql);
        }
        private void button20_Click(object sender, EventArgs e)
        {
            RefreshTradeList();
        }
        public int start_year = 2015;
        private void r2015_CheckedChanged(object sender, EventArgs e)
        {
            if (r2015.Checked)
                start_year = 2015;
            else if (r2016.Checked)
                start_year = 2016;
            else if (r2017.Checked)
                start_year = 2017;
            else if (r2018.Checked)
                start_year = 2018;
            else if (r2019.Checked)
                start_year = 2019;
            else if (r2020.Checked)
                start_year = 2020;
            else if (r2021.Checked)
                start_year = 2021;
            else if (r2022.Checked)
                start_year = 2022;
            else if (r2023.Checked)
                start_year = 2023;
            else if (r2024.Checked)
                start_year = 2024;
        }                   
        private void ShowStock(string StockNo)
        {
            lGraphic.ZedGraphControl.Visible = true; 
            lGraphic.Update(StockNo, "90");
            if(txtinfo.CurrentDate>DateTime.Now)
                lGraphic.MoveLastDate(DateTime.Now.AddDays(-330).ToString("yyyy-MM-dd")); 
            else
                lGraphic.MoveLastDate(txtinfo.CurrentDate.ToString("yyyy-MM-dd"));
        }
        private void txtStock_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                ShowStock(txtStock.Text);
                AddMenu(5,txtStock.Text, "StockNo");

                string StockList = iniFile.iniReadValue("StockNo", "StockList");
                if (StockList == "")
                    iniFile.iniWriteValue("StockNo", "StockList", txtStock.Text);
                else
                {
                    string[] StockLists = StockList.Split(new string[] { "," }, StringSplitOptions.None);
                    bool IsExist = false;
                    foreach (string item in StockLists)
                    {
                        if (item == txtStock.Text){
                            IsExist= true;
                            break;
                        }
                    }
                    if (!IsExist)
                    {
                        iniFile.iniWriteValue("StockNo", "StockList", StockList + "," + txtStock.Text);
                    }
                } 
            }
        }
        
        private void button15_Click(object sender, EventArgs e)
        {
            lGraphic.MoveNext();
            txtinfo.SetData( lGraphic.GetCurrentRow());
            refreshGain();
        }
        private void button21_Click(object sender, EventArgs e)
        {
            txtinfo.SetData("O", "1955");
        }
        public TradeManager lTradeManager = new TradeManager();

        public void refreshGain()
        {
            double ResultClose = lTradeManager.GetClosePriceALL();
            txtinfo.SetData("Already", ResultClose.ToString());//全部以平
            double[] ResultNonClose = lTradeManager.GetNonClosePrice(lGraphic.StockNo, txtinfo.CurrentPrice);//單一未平
            txtinfo.SetData("Non", ResultNonClose[0].ToString() + "-" + ResultNonClose[1].ToString());//全部未平
            double NonAll = lTradeManager.GetNonALL(txtinfo.CurrentDate);
            txtinfo.SetData("NonAll", NonAll.ToString());
        }
        private void btnBuy_Click(object sender, EventArgs e)
        {
            lTradeManager.DoTrade(lGraphic.StockNo,txtinfo.CurrentDate, TradeType.Buy, txtinfo.CurrentPrice);
            refreshGain();
        }
        private void btnSell_Click(object sender, EventArgs e)
        {
            lTradeManager.DoTrade(lGraphic.StockNo, txtinfo.CurrentDate, TradeType.Sell, txtinfo.CurrentPrice);
            refreshGain();
        }

        private void button21_Click_1(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                openFileDialog.Title = "Select a CSV file";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;

                    try
                    {
                        DataTable dataTable = CsvToDataTable(filePath);
                        
                        // For demonstration: bind the DataTable to a DataGridView
                        dataGridView3.DataSource = dataTable;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}");
                    }
                }
            }
        }
        string[] split = { "\",\"" }; 
        private DataTable CsvToDataTable(string filePath)
        {
            DataTable dataTable = new DataTable();
            string[] names = {"SerialNumber", "OpenDate", "SecurityName", "SecurityCode", "Market", "MarketType", "AuctionMethod",
"BidStartDate", "BidEndDate", "AuctionShares", "MinBidPrice", "MinBidUnit",
"MaxBidUnit", "DepositPercentage", "BidProcessingFee", "AllocationDate",
"Underwriter", "TotalWinningAmount", "WinningFeeRate", "TotalValidBids",
"ValidBidVolume", "MinWinningPrice", "MaxWinningPrice", "WeightedAvgWinningPrice",
"IssuePrice", "AuctionCancellation","LastPrice"};
           
            using (StreamReader reader = new StreamReader(filePath, System.Text.Encoding.Default))
            {
                bool MarketType = true;
                int rowno = 0;

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    if (line == null) continue;

                    string[] values = line.Split(split,StringSplitOptions.None);

                    if (rowno==1)
                    { 
                        foreach (string column in names)
                        {
                            dataTable.Columns.Add(column.Trim());
                        }

                    }
                    else if (rowno > 1)
                    {
                        // Add rows to the DataTable
                        if (values.Length == 25)
                        {
                            List<string> list = new List<string>(values);
                            list.Insert(5, "");
                            values = list.ToArray();
                        }
                        dataTable.Rows.Add(values);
                    }
                    rowno++;
                }
            }
            dataTable.Columns.RemoveAt(0);
            for (int i = dataTable.Rows.Count - 1; i >= 0; i--)
            {
                
                // 获取第一列的值（索引为 0）
                if (dataTable.Rows[i][0].ToString() == "")
                {
                    dataTable.Rows.RemoveAt(i); // 删除该行
                }
            }
            int lastColumnIndex = dataTable.Columns.Count - 2;
            foreach (DataRow row in dataTable.Rows)
            {
                row[lastColumnIndex] = row[lastColumnIndex].ToString().Replace(",", "").Replace("\"", ""); // 替换为空字符串
            }

            MyDatas.lData.InsertData(dataTable,"Stockauction");
            return dataTable;
        }

        private void button22_Click(object sender, EventArgs e)
        {
            string sql = "select * from StockAuction where len(securitycode)=4";
            DataTable dataTable = MyDatas.lData.FineDataTable(sql);
            foreach (DataRow item in dataTable.Rows)
            {
                string BidEndDate = item["BidEndDate"].ToString();
                string AllocationDate = item["AllocationDate"].ToString(); ;
                string SecurityCode = item["SecurityCode"].ToString();
                string Market = item["Market"].ToString();
               
                sql = "SELECT  [Open]   FROM [MyAnalysis].[dbo].[StockData] where stockno='"+ SecurityCode + "' and dates <='" + DateTime.Parse(BidEndDate).ToString("yyyy-MM-dd") + "' order by dates desc";
                string LastPrice = MyDatas.lData.FineDataField(sql);

                sql = "SELECT  [Close]   FROM [MyAnalysis].[dbo].[StockData] where stockno='" + SecurityCode + "' and dates >='" + DateTime.Parse(AllocationDate).ToString("yyyy-MM-dd") + "' order by dates  ";
                string LastPrice2 = MyDatas.lData.FineDataField(sql);

                //sql = "update StockAuction set LastPrice='"+ LastPrice + "',AllocationPrice='" + LastPrice2 + "' where SecurityCode='" + SecurityCode + "' and Market='" + Market +"'";
                //MyDatas.lData.SqlCmdExec(sql);
            }
        }

        private   void button23_Click(object sender, EventArgs e)
        {
              GetStockClass.GetYahooStockData("6026,TW");
        }

        private void button24_Click(object sender, EventArgs e)
        {
            DataTable dt = MyDatas.lData.GetXmlTable("Stockauction");
            DataRow[] selectedRows = dt.Select("Market in('初上櫃','櫃檯買賣') and LastPrice<>'' and Underwriter not like '%台新%'","BidEndDate");
            dt = selectedRows.CopyToDataTable();
            dt.Columns.Add("P1", typeof(decimal)); // 添加新欄位
            dt.Columns.Add("P2", typeof(decimal)); // 添加新欄位
            dt.Columns.Add("P3", typeof(decimal)); // 添加新欄位
            dt.Columns.Add("Sum", typeof(string)); // 添加新欄位
            foreach (DataRow row in dt.Rows)
            {
                try
                {
                    // 嘗試將 MinWinningPrice 欄位轉換為 decimal
                    row["P1"] = Convert.ToDecimal(row["MinWinningPrice"].ToString().Replace(",",""));
                    row["P2"] = Convert.ToDecimal(row["LastPrice"].ToString().Replace(",", ""));
                    row["P3"] = Convert.ToDecimal(row["AllocationPrice"].ToString().Replace(",", ""));
                    if((double.Parse(row["P2"].ToString()))*0.83 > double.Parse(row["P1"].ToString()))
                    {
                        row["Sum"]= "Y";
                    }


                }
                catch (FormatException)
                {
                    // 如果轉換失敗（例如非數字格式），這裡會捕捉到
                    Console.WriteLine($"無法轉換 '{row["MinWinningPrice"]}' 為數字");
                }
            }
            dt.Columns.Remove("MinWinningPrice");
            dt.Columns.Remove("LastPrice");
            dt.Columns.Remove("AllocationPrice");
            //selectedRows = dt.Select("P2*0.83> P1", "BidEndDate");
            //dt = selectedRows.CopyToDataTable();
            //AllocationDate  BidEndDate AllocationPrice MinWinningPrice
            dataGridView3.DataSource= dt;
        }
    }
}
