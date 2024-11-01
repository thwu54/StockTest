 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using ZedGraph;
using TicTacTec.TA.Library;
using YahooFinanceApi;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using Microsoft.Data.Analysis;
using System.Net.Http;
namespace My
{
    public delegate void MyEvent<T>(object sender, T args);
    class MyGraphic
    {
        protected string description;
        protected string title;
        protected ICollection types;
        //######屬性
        private bool myIsShowVolume = false;
        public bool IsShowVolume
        {
            get { return myIsShowVolume ; }
            set { myIsShowVolume =   value; }
        }

        private bool myIsShowPanel2 = true;
        public bool IsShowPanel2
        {
            get { return myIsShowPanel2; }
            set { myIsShowPanel2 = value; }
        }
        //######屬性
        private ZedGraphControl control;
         
        public MyGraphic(string description, string title, DemoType type)
        { 
            Init(description, title, types);
        } 

        public static DataTable GetDataSource(string StockNo, string Range)
        {
            string sql = "select Dates,[Open],[High],[Low],[Close],[Volume]  FROM  [StockData] where stockno='" + StockNo + "'AND [Open] IS NOT NULL   and dates>getdate()-" + (int.Parse(Range) ).ToString() + " order by dates";
            DataTable lTable = MyDatas.lData.FineDataTable(sql);
            lTable= ResampleWeekly(lTable);
             
            Task  task = Task.Run(async () => await GetKD());
            
            return lTable;
        }
        public static async System.Threading.Tasks.Task  GetKD()
        {
            //var opens = new double[] { 395, 450, 487.5, 486, 488, 476.5, 466.5, 454, 449.5, 468, 506, 542, 532, 544, 514, 504, 520, 513, 517, 533, 530, 533, 519, 505, 500 };
            //var highs = new double[] { 442, 494, 498, 508, 489, 481.5, 469.5, 462.5, 459.5, 509, 508, 543, 546, 546, 525, 525, 526, 518, 539, 538, 535, 533, 520, 510, 502 };
            //var lows = new double[] { 394, 441.5, 476, 473, 467, 465.5, 455, 442.5, 448.5, 467.5, 499.5, 521, 522, 516, 505, 504, 511, 504, 510, 524, 526, 510, 509, 489, 495 };
            //var closes = new double[] { 441.5, 487, 498, 492.5, 481.5, 471, 455, 448.5, 458.5, 500, 503, 542, 545, 518, 511, 516, 513, 518, 539, 533, 531, 516, 511, 502, 496 };

            var history = await Yahoo.GetHistoricalAsync("2330.TW", new DateTime(2023, 4, 1), DateTime.Today, Period.Daily);

            //var history = await Yahoo.GetHistoricalAsync("2330.TW", DateTime.Now.AddDays(-30));


            var high = history.Select(x => (double)x.High).ToArray();
            var low = history.Select(x => (double)x.Low).ToArray();
            var close = history.Select(x => (double)x.Close).ToArray();
            int outBegIdx, outNBElement;
            double[] outSlowK = new double[history.Count], outSlowD = new double[history.Count];
            Core.Stoch(0, history.Count - 1, high, low, close, 14, 3, Core.MAType.Sma, 3, Core.MAType.Sma, out outBegIdx, out outNBElement, outSlowK, outSlowD);
            for (int i = 0; i < outNBElement; i++)
            {
                Console.WriteLine($"SlowK: {outSlowK[i]}, SlowD: {outSlowD[i]}");
            }
        }
        
        //public static void GetKD()
        //{
        //    int startIdx, endIdx;
        //    float[] high = new float[] { 10.0f, 12.0f  };
        //    float[] low = new float[] { 9.0f, 10.0f };
        //    float[] close = new float[] { 9.5f, 11.5f  };
        
        //    double[] outJ = new double[close.Length];
        //        outJ[i] = 3 * outSlowK[i] - 2 * outSlowD[i];
        //    }
        //}

        public static DataTable Resample(DataTable dataTable, TimeSpan resampleInterval)
        {
            // 建立新的 DataTable 物件，用於儲存重採樣後的資料
            DataTable resampledTable = new DataTable();

            // 複製原始 DataTable 的結構到新的 DataTable 中
            foreach (DataColumn column in dataTable.Columns)
            {
                resampledTable.Columns.Add(column.ColumnName, column.DataType);
            }

            // 依照時間日期欄位分組，並計算每小時的平均值
            var groupedData = dataTable.AsEnumerable()
                .GroupBy(row => new DateTime(row.Field<DateTime>("Date").Year, row.Field<DateTime>("Date").Month, row.Field<DateTime>("Date").Day, row.Field<DateTime>("Date").Hour / resampleInterval.Hours * resampleInterval.Hours, 0, 0))
                .Select(group =>
                {
                    var newRow = resampledTable.NewRow();
                    foreach (DataColumn column in resampledTable.Columns)
                    {
                        if (column.ColumnName == "Date")
                        {
                            newRow[column.ColumnName] = group.Key;
                        }
                        else
                        {
                            newRow[column.ColumnName] = group.Average(row => Convert.ToDouble(row[column.ColumnName]));
                        }
                    }
                    return newRow;
                });

            // 將重採樣後的資料加入到新的 DataTable 中
            foreach (DataRow row in groupedData)
            {
                resampledTable.Rows.Add(row);
            }

            // 回傳重採樣後的 DataTable
            return resampledTable;
        }
        public static DataTable ResampleWeekly(DataTable dataTable)
        {

            DataTable resampledTable = new DataTable();

            // 複製原始 DataTable 的結構到新的 DataTable 中
            foreach (DataColumn column in dataTable.Columns)
            {
                resampledTable.Columns.Add(column.ColumnName, column.DataType);
            }
            var result = dataTable.AsEnumerable().GroupBy(row => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(row.Field<DateTime>("Dates"), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)).Select(group =>
            {
                var newRow = resampledTable.NewRow();
                newRow["Dates"] = group.Max(row => row.Field<DateTime>("Dates"));
                newRow["Open"] = group.First().Field<double>("Open");
                newRow["Close"] = group.Last().Field<double>("Close");
                newRow["Low"] = group.Min(row => row.Field<double>("Low"));
                newRow["High"] = group.Max(row => row.Field<double>("High"));
                newRow["Volume"] = group.Sum(row => row.Field<double>("Volume"));
                return newRow;
            });

            foreach (var item in result)
            {
                resampledTable.Rows.Add(item); 
            }
            return resampledTable;
        }

        public void savePic()
        {
           
            Bitmap bitmap = new Bitmap(this.ZedGraphControl.Width, this.ZedGraphControl.Height); 
            Graphics graphics = Graphics.FromImage(bitmap);
            RectangleF rect = new RectangleF(0, 0, this.ZedGraphControl.Width, this.ZedGraphControl.Height);
            Image img = this.ZedGraphControl.GetImage();
            graphics.DrawImage(img, rect); 
             
            bitmap.Save("image.png", ImageFormat.Png);

            // 釋放資源
            graphics.Dispose();
            bitmap.Dispose();
         

             
        }

        private void Init(string description, string title, ICollection types)
        {
            this.description = description;
            this.title = title;
            this.types = types;

            control = new ZedGraphControl();
            control.IsAntiAlias = true;
            //control2 = new ZedGraphControl();
            //control2.IsAntiAlias = true;
            control.MouseMove += Control_MouseMove;
            control.MouseDownEvent += Control_MouseDownEvent;
            control.MouseUpEvent += Control_MouseUpEvent;
            control.MouseWheel += Control_MouseWheel;
            control.AllowDrop = true;
            control.DragDrop += Control_DragDrop;
            control.DragEnter += Control_DragEnter;
            if (_StockNo!="")
                SetSecond(new string[] { "upperband", "middleband", "lowerband" });
        }

        private void Control_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            e.Effect = System.Windows.Forms.DragDropEffects.Move;
        }

        public event MyEvent<System.Windows.Forms.DragEventArgs> DragDrop;
        private void Control_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            DragDrop(this, e);
        }

        private void Control_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int gap = 10;
            if (IsDrop)
                gap = 100;

            if (e.Delta < 0)
            {
                Move(gap);
            }
            else
            {
                if((int)this.ZedGraphControl.GraphPane.XAxis.Scale.Min- gap >= 0)
                    Move(-1* gap);
            }
            drawMouseMove(sender, e);
        }

        private bool Control_MouseUpEvent(ZedGraphControl sender, System.Windows.Forms.MouseEventArgs e)
        {
            IsDrop = false;
            return true;
        }
        private int orgX1 = 0;
        private int orgX2 = 0;
        private bool Control_MouseDownEvent(ZedGraphControl sender, System.Windows.Forms.MouseEventArgs e)
        {
            IsDrop = true;
            //RectangleF rect = control.MasterPane[0].Chart.Rect;
            //if (rect.Contains(e.Location))
            //{
            //    double x = 0;
            //    double y = 0;
            //    PointF mousePt = new PointF(e.X, e.Y);
            //    ((ZedGraphControl)sender).MasterPane[0].ReverseTransform(mousePt, out x, out y);
            //    x = Math.Round(x, 0, MidpointRounding.AwayFromZero);
            //    if (x - 1 <= spl.Count - 1)
            //    {
            //        ClickX = (int)x;
            //    }
            //    orgX1=(int)this.ZedGraphControl.GraphPane.XAxis.Scale.Min ;
            //    orgX2= (int)this.ZedGraphControl.GraphPane.XAxis.Scale.Max  ;
            //}
            return true;
        }

        private bool IsDrop=false;
        private int ClickX = 0;
         
        private void drawMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            using (Graphics gc = control.CreateGraphics())
            using (Pen pen = new Pen(Color.Green))
            {
                //設置畫筆的寬度
                pen.Width = 1;
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
                RectangleF rect = control.MasterPane[0].Chart.Rect;

                //確保在畫圖區域
                if (rect.Contains(e.Location))
                {
                    control.Refresh();
                    PointF mousePt = new PointF(e.X, e.Y);
                    double x = 0;
                    double y = 0;
                    //圖形數值=>資料數值 
                    ((ZedGraphControl)sender).MasterPane[0].ReverseTransform(mousePt, out x, out y);
                    x = Math.Round(x, 0, MidpointRounding.AwayFromZero);
                    //資料數值=>圖形數值
                    //畫豎線主圖
                    PointF PointF2 = ((ZedGraphControl)sender).MasterPane[0].GeneralTransform(x, 30, CoordType.AxisXYScale);
                    gc.DrawLine(pen, PointF2.X, rect.Top, PointF2.X, rect.Bottom);
                    //畫豎線附圖

                    if (IsShowVolume)
                    {
                        PointF PointF3 = ((ZedGraphControl)sender).MasterPane[1].GeneralTransform(x, 30, CoordType.AxisXYScale);
                        RectangleF rect2 = control.MasterPane[1].Chart.Rect;
                        gc.DrawLine(pen, PointF3.X, rect2.Top, PointF3.X, rect2.Bottom);
                    }

                    if (x - 1 <= spl.Count - 1 && spl.Count>0)
                    {
                        StockPt pt = (StockPt)spl[(int)x - 1];
                        string Close = Math.Round(pt.Close, 2).ToString();
                        string Volume = Math.Round(pt.Vol, 2).ToString();
                        
                        //畫橫線 IN CLOSE
                        PointF PointFY = ((ZedGraphControl)sender).MasterPane[0].GeneralTransform(30, pt.Close, CoordType.AxisXYScale);

                        gc.DrawLine(pen, rect.Left, PointFY.Y, rect.Right, PointFY.Y);
                        DateTime lTime = DateTime.FromOADate(pt.Date);
                        System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 10);//
                        System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);

                        gc.DrawString(lTime.ToString("yyyy/MM/dd"), drawFont, drawBrush, rect.X + 20, rect.Y + 5);
                        gc.DrawString(Close, drawFont, drawBrush, rect.X + 110, rect.Y + 5);

                        if (IsShowVolume)
                        {
                            RectangleF rect2 = control.MasterPane[1].Chart.Rect;
                            gc.DrawString(Volume, drawFont, drawBrush, rect2.X + 20, rect2.Y + 5);
                        }

                    }

                }
            }
        }
        private void Control_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            drawMouseMove(sender, e);
        }

        public PaneBase Pane { get { return control.GraphPane; } }

        public MasterPane MasterPane { get { return control.MasterPane; } }

        public GraphPane GraphPane { get { return control.GraphPane; } }
        
        //virtual 是 被複寫的方法
        public virtual string Description { get { return description; } }

        public virtual string Title { get { return title; } }

        public virtual ICollection Types { get { return types; } }
        public List<Index> Indexs = new List<Index>();
        public void AddIndex(Index lIndex)
        {
            Indexs.Add(lIndex);
        }

        private ArrayList _SecondIndex = new ArrayList();

        public bool IsSecond = true;
        //附圖欄位
        private void SetSecond(string[] SecondColumn)
        {
            foreach (string item in SecondColumn)
            {
                _SecondIndex.Add(item);
                PointPairList list = new PointPairList();
                _SecondData.Add(item, list);
            }
        }

        private int _GraphicRange = 0;
        private StockPointList spl = new StockPointList();
        private DataTable _lTable = null;
        private DataFrame df = null;
        private IDictionary<string, DataRow> _IndexData = new Dictionary<string, DataRow>();
        private IDictionary<string, PointPairList> _SecondData = new Dictionary<string, PointPairList>();
        //###################################################################################################
        private void setData(DataTable lTable)
        {
            _lTable = lTable; 
            DataFrame df = MyData.table_df(lTable);
            foreach (DataRow item in _lTable.Rows)
            {
                if(!_IndexData.Keys.Contains(item["date"].ToString()))
                    _IndexData.Add(item["date"].ToString(), item);

            }
        }

        private void setData_df(DataFrame df)
        {
            _lTable = MyData.df_dt(df); 
            
            foreach (DataRow item in _lTable.Rows)
            {
                if (!_IndexData.Keys.Contains(item["date"].ToString()))
                    _IndexData.Add(item["date"].ToString(), item);

            }
        }

        public void DrawTrade2(GraphPane myPane, Trade lTrade)
        {
            Trade lTrade2= new Trade();
            
            lTrade2._Type= lTrade._Type== TradeType.Buy? TradeType.Sell: TradeType.Buy;
            lTrade2._Date1 = lTrade._Date2;
            DrawTrade(myPane, lTrade,lTrade._Point1);
            DrawTrade(myPane, lTrade2, lTrade._Point2);
            
        }
        public void DrawTrade(GraphPane myPane,Trade lTrade,double Point)
        {
            int i = 0;
            double bx = 0;
            double by = 0;
            double bdrawy = 0;
            foreach (DataRow lRow in _lTable.Rows)
            {
                i++;
                //附圖指標 
                if (((DateTime)lRow[0]).Date== lTrade._Date1)
                {
                    bx = i;
                    if (lTrade._Type == TradeType.Sell)
                        bdrawy = double.Parse(lRow["high"].ToString())  ;
                    else
                        bdrawy = double.Parse(lRow["low"].ToString());
                    break;
                } 
            }
            by = Point;
            double texty = bdrawy * 1.05; 
            if (lTrade._Type == TradeType.Buy)
                texty = bdrawy * 0.9;

            TextObj text = new TextObj( by.ToString()  , bx, texty);

            text.Location.AlignH = AlignH.Center;
            text.Location.AlignV = AlignV.Bottom;
            text.FontSpec.Fill.IsVisible = false;
            text.FontSpec.Border.IsVisible = false;
            myPane.GraphObjList.Add(text);
            double y1 = 0;
            double y2 = 0;
            Color ColorType = Color.Red;
            if (lTrade._Type== TradeType.Sell)
            {
                y1 = bdrawy * 1.05;
                y2 = bdrawy * 1.03;
                ColorType = Color.Green;
            }
            else
            {
                y1 = bdrawy * 0.95;
                y2 = bdrawy * 0.97;
            }

            ArrowObj arrow = new ArrowObj(ColorType, 12, bx, y1, bx, y2);
            arrow.Location.CoordinateFrame = CoordType.AxisXYScale;
            arrow.Line.Width = 2.0F;
            myPane.GraphObjList.Add(arrow);
        }
        public void FirstDraw()
        {
            MasterPane myMaster = this.MasterPane;
            myMaster.PaneList.Clear();

            //附圖clear
            foreach (string item in _SecondData.Keys)
            {
                _SecondData[item].Clear();
            }
            // First day is jan 1st
            GraphPane myPane = new GraphPane(); 
            GraphPane myPane2 = new GraphPane();
            //myPane.Title.Text = "/*Japanese Candlestick Chart Demo*/";
            myPane.XAxis.Title.Text = "Date";
            //myPane.YAxis.Title.Text = "Price";
            myPane.YAxis.Scale.MagAuto = false;
            myPane.YAxis.Scale.FontSpec.Size = 8;
            myPane.XAxis.Scale.FontSpec.Size = 8;
            myPane.Margin.Right = 0;
            myPane2.XAxis.Title.Text = "Date";
            myPane2.YAxis.Title.Text = "Volume";
            myPane2.YAxis.Scale.MagAuto = false;
            spl.Clear();
            //
            //附圖量
            PointPairList list = new PointPairList();
            //附圖量
            //畫訊號
            int i = 0;
            string TradePoint = "0";
            string TradeDate = "2021-10-11";
            foreach (DataRow lRow in _lTable.Rows)
            {
                i++;
                XDate xDate = new XDate((DateTime.Parse(lRow["date"].ToString())).ToOADate());
                double x = xDate.XLDate;
                double close = double.Parse(lRow["close"].ToString());
                double hi = double.Parse(lRow["high"].ToString());
                double low = double.Parse(lRow["low"].ToString());
                double open = double.Parse(lRow["open"].ToString());
                double volume = double.Parse(lRow["volume"].ToString());
                StockPt pt = new StockPt(x, hi, low, open, close, volume);
                spl.Add(pt);
                //附圖指標取值
                foreach (string item in _SecondData.Keys)
                {
                    double y = lRow[item] is DBNull ? double.NaN: double.Parse(lRow[item].ToString());
                    _SecondData[item].Add(x, y);
                } 
                list.Add(i, volume);
                if (i == 40)
                {
                    TradePoint = Math.Round(close, 2).ToString();
                    TradeDate = (DateTime.Parse(lRow["date"].ToString())).ToString("yyyy-MM-dd");
                }
            }


            //畫訊號
            if (_Log.Count != 0)
            {
                for (int j = -0; j < _Log.Count; j++)
                {
                    Trade Log = _Log[j]; 
                    DrawTrade2(myPane, Log); 

                }
            }
            //畫volume
            //BarItem myVolumeBar = myPane2.AddBar("volume", list, Color.Blue);
            //畫volume

            //畫主圖
            //StockPt pt2 = (StockPt)spl[0];            
            JapaneseCandleStickItem myCurve = myPane.AddJapaneseCandleStick("trades", spl);
            //myCurve.Stick.IsAutoSize = true;
            myCurve.Stick.Color = Color.Blue;
            myCurve.Stick.RisingFill = new Fill(Color.Red);

            

            foreach (Index item in Indexs)
            {
                if(item.PanelNo=="2" || item.PanelNo == "3")
                    item.Draw(ref myPane2, _lTable); 
                else
                    item.Draw(ref myPane, _lTable);
            }
            
            double gap = 0.5;
            double width = _GraphicRange;
            if (spl.Count > width)
            {
                myPane.XAxis.Scale.Max = width + gap;
                myPane2.XAxis.Scale.Max = width + gap;
            }
            // Use DateAsOrdinal to skip weekend gaps
            myPane.XAxis.Type = AxisType.DateAsOrdinal;
            myPane2.XAxis.Type = AxisType.DateAsOrdinal;
            myPane.XAxis.Scale.MajorUnit = DateUnit.Day;
            myPane.XAxis.Scale.MajorStep = 7; 
            myPane.XAxis.Scale.Format = "MM/dd";
            myPane2.XAxis.Scale.Format = "MM/dd"; 
            myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45.0f);
            myPane.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45.0f);
             
            myPane2.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45.0f);
            myPane2.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45.0f);
            //BarItem myVolume = myPane2.AddBar("volume", list, Color.Blue);


            this.MasterPane.Add(myPane);
            if(myIsShowPanel2)
                this.MasterPane.Add(myPane2);

            
            
            this.ZedGraphControl.MasterPane.SetLayout(this.ZedGraphControl.CreateGraphics(), PaneLayout.SingleColumn);
            this.ZedGraphControl.AxisChange(); 
            this.ZedGraphControl.Invalidate(); 
        }
        //###################################################################################################
        public string _StockNo = "";
        private string _Range = "";//圖寬
        private string _Pre = "";// 
        //新增屬性

        public string StockNo
        {
            get { return _StockNo; } set { _StockNo = value; }
        }   
        private Dictionary<string, DataTable> stockdata = new Dictionary<string, DataTable>();

        public static DataFrame GetData(string StockNo , string Resample)
        {
            if (FunLog.checkFileExist2("database\\" + DateTime.Now.ToString("yyyyMMdd") + "\\", StockNo + "_" + Resample))
            {
                DataTable Table = MyData.JsonToDataTable("database\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + StockNo + "_" + Resample);
                return MyData.table_df(Table);
            }
            else
            {
                string url = "";
                //url = "http://127.0.0.1:800/Talib/GetHistroy/?Resample=W-SAT&stockno=2330&start=2023-05-20&Indexs=MACD_BBANDS_STOCH_RSI_SMA,SMA,SMA&paras=_20,2.1,2.1_5,3,3_14_5_10_20";
                DataTable Table = MyData.GetTableByWebApi("http://127.0.0.1:800/Talib/GetHistroy/?Resample=" + Resample + "&stockno=" + StockNo + "&start=2019-01-01&end=2023-01-01&Indexs=MACD_BBANDS_STOCH_RSI_SMA_SMA_SMA&paras=_20,2.1,2.1_5,3,3_14_5_10_20");
                DataFrame df = MyData.table_df(Table);
                MyData.DataTableToJson(Table, "database\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + StockNo+"_"+ Resample);
                return df;
            }
        }

        public static Dictionary<string, DataFrame> dfData = new Dictionary<string, DataFrame>();

        public static DataFrame GetDateData(string StockNo, string Resample,DateTime Date1,DateTime Date2)
        {
            if (Resample == "H4")
                Resample = "4H";
            string filename = StockNo + "_" + Resample + "_" + Date1.ToString("yyyyMMdd") + "_" + Date2.ToString("yyyyMMdd");
            if (dfData.ContainsKey(filename))
                return dfData[filename];
            if (FunLog.checkFileExist2("database\\" + DateTime.Now.ToString("yyyyMMdd") + "\\", filename))
            {
                DataTable Table = MyData.JsonToDataTable("database\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + filename);
                if (!dfData.ContainsKey(filename))
                    dfData.Add(filename, MyData.table_df(Table)); 
                return dfData[filename];
            }
            else
            {
                string url = "";
                string Date2str = "";
                if (Date2 != Date1)
                    Date2str = Date2.ToString("yyyy-MM-dd");
                //url = "http://127.0.0.1:800/Talib/GetHistroy/?Resample=W-SAT&stockno=2330&start=2023-05-20&Indexs=MACD_BBANDS_STOCH_RSI_SMA,SMA,SMA&paras=_20,2.1,2.1_5,3,3_14_5_10_20";
                DataTable Table = MyData.GetTableByWebApi("http://127.0.0.1:800/Talib/GetHistroy/?Resample=" + Resample + "&stockno=" + StockNo + "&start=" + Date1.ToString("yyyy-MM-dd") + "&end=" + Date2str + "&Indexs=MACD_BBANDS_STOCH_RSI_SMA_SMA_SMA&paras=_20,2.1,2.1_5,3,3_14_5_10_20");
                DataFrame df = MyData.table_df(Table);
                MyData.DataTableToJson(Table, "database\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + filename);
                return df;
            }
        }

        public static DataFrame GetData2(string StockNo, string Resample)
        {
            if (FunLog.checkFileExist2("database\\" + DateTime.Now.ToString("yyyyMMdd") + "\\", StockNo + "_" + Resample))
            {
                DataTable Table = MyData.JsonToDataTable("database\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + StockNo + "_" + Resample);
                return MyData.table_df(Table);
            }
            else
            {
                string url = "";
                //url = "http://127.0.0.1:800/Talib/GetHistroy/?Resample=W-SAT&stockno=2330&start=2023-05-20&Indexs=MACD_BBANDS_STOCH_RSI_SMA,SMA,SMA&paras=_20,2.1,2.1_5,3,3_14_5_10_20";
                DataTable Table = MyData.GetTableByWebApi("http://127.0.0.1:800/Talib/GetHistroy/?Resample=" + Resample + "&stockno=" + StockNo + "&start=2023-01-01&Indexs=MACD_BBANDS_STOCH_RSI_SMA_SMA_SMA&paras=_20,2.1,2.1_5,3,3_14_5_10_20");
                DataFrame df = MyData.table_df(Table);
                MyData.DataTableToJson(Table, "database\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + StockNo + "_" + Resample);
                return df;
            }
        }

        public static DataTable GetDataTable(string StockNo,  string Resample)
        {
            if (Resample == "T")
            {

            }


            if (FunLog.checkFileExist2("database\\" + DateTime.Now.ToString("yyyyMMdd") + "\\", StockNo + "_" + Resample))
            {
                DataTable Table = MyData.JsonToDataTable("database\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + StockNo + "_" + Resample);
                return Table;
            }
            else
            {
                string url = "";
                url = "http://127.0.0.1:800/Talib/GetHistroy/?Resample=W-SAT&stockno=2330&start=2023-05-20&Indexs=MACD_BBANDS_STOCH&paras=_20,2.1,2.1_5,3,3";
                DataTable Table = MyData.GetTableByWebApi("http://127.0.0.1:800/Talib/GetHistroy/?Resample=" + Resample + "&stockno=" + StockNo + "&start=2020-01-01&Indexs=MACD_BBANDS_STOCH&paras=_20,2.1,2.1_5,3,3");
                DataFrame df = MyData.table_df(Table);
                MyData.DataTableToJson(Table, "database\\" + DateTime.Now.ToString("yyyyMMdd") + "\\" + StockNo + "_" + Resample);
                return Table;
            }
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

        public virtual void Update(string StockNo,string Range)
        {
            _StockNo = StockNo;
            _Range = Range;
            string para = "";
            string index = "";
            foreach (Index item in Indexs)
            {
                if (index == "")
                    index = item.Name;
                else
                    index+="," + item.Name;
                if (para == "")
                    para = item.para;
                else
                    index += "_" + item.para;
            }
            DataFrame df = MyGraphic.GetDateData(_StockNo, "W-SAT", DateTime.Parse("2020/01/01"), DateTime.Parse("2025/01/01"));
            setData_df(df);
            //DataTable Table = MyData.GetTableByWebApi("http://127.0.0.1:800/Talib/GetHistroy/?stockno=" + _StockNo + "&start=2020-01-01&Indexs=" + index + "&paras=" + para);
            //DataTable Table = MyData.GetTableByWebApi("http://127.0.0.1:8000/GetHistroy/?stockno=" + _StockNo + "&start=2020-01-01&Indexs=" + index + "&paras=" + para);
            
            _GraphicRange = int.Parse(Range); 
            FirstDraw();
        }

        public void UpdateIndex( )
        { 
            string para = "";
            string index = "";
            foreach (Index item in Indexs)
            {
                if (index == "")
                    index = item.Name;
                else
                    index += "," + item.Name;
                if (para == "")
                    para = item.para;
                else
                    index += "_" + item.para;
            }
            DataTable Table = MyData.GetTableByWebApi("http://127.0.0.1:800/Talib/GetHistroy/?stockno=" + _StockNo + "&start=2020-01-01&Indexs=" + index + "&paras=" + para);
            //DataTable Table = MyData.GetTableByWebApi("http://127.0.0.1:8000/GetHistroy/?stockno=" + _StockNo + "&start=2020-01-01&Indexs=" + index + "&paras=" + para);
            setData(Table); 
            FirstDraw();
        }

        private int LogNo = 0;
        private List<Trade> _Log = new List<Trade>();   
        public void ShowTrade(List<Trade> Log)
        {
            LogNo = 0;
            string StockNo = "TX";
            if (_StockNo != StockNo)
            {
                _StockNo = StockNo;
                 
                _Log = Log;
                if (!stockdata.ContainsKey(StockNo))
                {
                    stockdata.Add(StockNo, MyDatas.GetIndex1(StockNo));
                }
                _lTable = stockdata[_StockNo];
                //string[] paras = Log[LogNo]["para"].ToString().Split(parasplit, StringSplitOptions.None); 
                //DataRow Log1 = _Log[LogNo];
                string StartDate = Log[0]._Date1.ToString();
                string EndDate = Log[0]._Date2.ToString();
                int[] Scale = GetScale(StartDate, EndDate);
                _Pre = "20";
                _GraphicRange = Scale[1] - Scale[0] + int.Parse(_Pre) + 30; 
                FirstDraw();
                
            }
            MoveTrade();
        }
        string[] parasplit = { "," };
        public void MoveTrade()
        { 
             
            string StartDate = _Log[LogNo]._Date1.ToString();   
            string EndDate = _Log[LogNo]._Date2.ToString();
            int[] Scale = GetScale(StartDate, EndDate);
            Move(Scale[0]- int.Parse(_Pre), Scale[1]+30);
            LogNo++;
            if (LogNo >= _Log.Count)
                LogNo = 0;
        }

        public int[] GetScale(string StartDate, string EndDate)
        {
            int[] Scale = { 0, 10 };
            for(int i=0;i< _lTable.Rows.Count; i++)
            {
                if (_lTable.Rows[i]["Dates"].ToString() == StartDate)
                    Scale[0] = i;
                if (_lTable.Rows[i]["Dates"].ToString() == EndDate)
                {
                    Scale[1] = i;
                    break;
                }
            }
            return Scale;
        }

        public void Move(int Min,int Max)
        {
            //double Min = this.ZedGraphControl.GraphPane.XAxis.Scale.Min;
            //double Max = this.ZedGraphControl.GraphPane.XAxis.Scale.Max;
            //double MinorStep = this.ZedGraphControl.GraphPane.XAxis.Scale.MinorStep;
            //double MajorStep = this.ZedGraphControl.GraphPane.XAxis.Scale.MajorStep;
            this.ZedGraphControl.GraphPane.XAxis.Scale.Min = Min;
            this.ZedGraphControl.GraphPane.XAxis.Scale.Max = Max;
            if (this.ZedGraphControl.MasterPane.PaneList.Count > 1)
            {
                this.ZedGraphControl.MasterPane[1].XAxis.Scale.Min = Min;
                this.ZedGraphControl.MasterPane[1].XAxis.Scale.Max = Max;
            }
            double[] MaxMin = GetMaxMin(this.ZedGraphControl.GraphPane.XAxis.Scale.Min, this.ZedGraphControl.GraphPane.XAxis.Scale.Max);
            double gap = MaxMin[0] - MaxMin[1];
            this.ZedGraphControl.GraphPane.YAxis.Scale.Max = MaxMin[0] + gap * 0.1;
            this.ZedGraphControl.GraphPane.YAxis.Scale.Min = MaxMin[1] - gap * 0.1;
            if (this.ZedGraphControl.MasterPane.PaneList.Count > 1)
            {
                if (MaxMin[3] != 0)
                {
                    this.ZedGraphControl.MasterPane[1].YAxis.Scale.Max = MaxMin[2];
                    this.ZedGraphControl.MasterPane[1].YAxis.Scale.Min = MaxMin[3];
                }
                if (MaxMin[5] != 0)
                {
                    this.ZedGraphControl.MasterPane[1].Y2Axis.Scale.Max = MaxMin[4];
                    this.ZedGraphControl.MasterPane[1].Y2Axis.Scale.Min = MaxMin[5];
                } 
            }
            this.ZedGraphControl.AxisChange();
            this.ZedGraphControl.Invalidate();
        }

        public void Move(int gaps)
        { 
            double _gap = (double)gaps;
            this.ZedGraphControl.GraphPane.XAxis.Scale.Min += _gap;
            this.ZedGraphControl.GraphPane.XAxis.Scale.Max += _gap;
            if (this.ZedGraphControl.MasterPane.PaneList.Count > 1)
            {
                this.ZedGraphControl.MasterPane[1].XAxis.Scale.Min = this.ZedGraphControl.GraphPane.XAxis.Scale.Min;
                this.ZedGraphControl.MasterPane[1].XAxis.Scale.Max = this.ZedGraphControl.GraphPane.XAxis.Scale.Max;
            }
            double[] MaxMin = GetMaxMin(this.ZedGraphControl.GraphPane.XAxis.Scale.Min, this.ZedGraphControl.GraphPane.XAxis.Scale.Max);
            double gap = MaxMin[0] - MaxMin[1];
            this.ZedGraphControl.GraphPane.YAxis.Scale.Max = MaxMin[0] + gap * 0.1;
            this.ZedGraphControl.GraphPane.YAxis.Scale.Min = MaxMin[1] - gap * 0.1;
            if (this.ZedGraphControl.MasterPane.PaneList.Count > 1)
            {
                if (MaxMin[2] != 0)
                {
                    this.ZedGraphControl.MasterPane[1].YAxis.Scale.Max = MaxMin[2];
                    this.ZedGraphControl.MasterPane[1].YAxis.Scale.Min = MaxMin[3];
                }
                if (MaxMin[4] != 0)
                {
                    this.ZedGraphControl.MasterPane[1].Y2Axis.Scale.Max = MaxMin[4];
                    this.ZedGraphControl.MasterPane[1].Y2Axis.Scale.Min = MaxMin[5];
                }
            }
            this.ZedGraphControl.AxisChange();
            this.ZedGraphControl.Invalidate();
        }
        public  string CurrentDate="";
        public int CurrentIndex = 0;
        public DataRow GetCurrentRow()
        { 
            if(CurrentIndex >= _lTable.Rows.Count)
                CurrentIndex = _lTable.Rows.Count - 1;
            return _lTable.Rows[CurrentIndex];
        }
        public void MoveNext()
        {
            double Min = this.ZedGraphControl.GraphPane.XAxis.Scale.Min;
            MoveByIndex(Min+1);
        }

        public void MoveByIndex(double Min)
        {
            double width = this.ZedGraphControl.GraphPane.XAxis.Scale.Max - this.ZedGraphControl.GraphPane.XAxis.Scale.Min;

            this.ZedGraphControl.GraphPane.XAxis.Scale.Min = Min;
            this.ZedGraphControl.GraphPane.XAxis.Scale.Max = Min + width  ;
            CurrentIndex = (int)(Min + width - 0.5-1);
            if (this.ZedGraphControl.MasterPane.PaneList.Count > 1)
            {
                this.ZedGraphControl.MasterPane[1].XAxis.Scale.Min += 1;
                this.ZedGraphControl.MasterPane[1].XAxis.Scale.Max += 1;
            }

            double[] MaxMin = GetMaxMin(this.ZedGraphControl.GraphPane.XAxis.Scale.Min, this.ZedGraphControl.GraphPane.XAxis.Scale.Max);
            double gap = MaxMin[0] - MaxMin[1];
            this.ZedGraphControl.GraphPane.YAxis.Scale.Max = MaxMin[0] + gap * 0.1;
            this.ZedGraphControl.GraphPane.YAxis.Scale.Min = MaxMin[1] - gap * 0.1;
            if (this.ZedGraphControl.MasterPane.PaneList.Count > 1)
            {
                if (MaxMin[2] != 0)
                {
                    this.ZedGraphControl.MasterPane[1].YAxis.Scale.Max = MaxMin[2];
                    this.ZedGraphControl.MasterPane[1].YAxis.Scale.Min = MaxMin[3];
                }
                if (MaxMin[4] != 0)
                {
                    this.ZedGraphControl.MasterPane[1].Y2Axis.Scale.Max = MaxMin[4];
                    this.ZedGraphControl.MasterPane[1].Y2Axis.Scale.Min = MaxMin[5];
                }
            }
            this.ZedGraphControl.AxisChange();
            this.ZedGraphControl.Invalidate();
        }
        //2021-10-01
        public void MoveDate(string Date)
        {
            int Point = 0;
            for (int i = 0; i < _lTable.Rows.Count; i++)
            {
                DateTime lDate = (DateTime)_lTable.Rows[i]["Date"];
                if (lDate.ToString("yyyy-MM-dd").CompareTo(Date)>=0)
                {
                    Point = i;
                    break;
                }
            }
            MoveByIndex(Point+0.5); 
        }

        public void MoveLastDate(string Date)
        {
            double width = this.ZedGraphControl.GraphPane.XAxis.Scale.Max - this.ZedGraphControl.GraphPane.XAxis.Scale.Min;

            int Point = 0;
            for (int i = 0; i < _lTable.Rows.Count; i++)
            {
                DateTime lDate = (DateTime)_lTable.Rows[i]["Date"];
                if (lDate.ToString("yyyy-MM-dd").CompareTo(Date) >= 0)
                {
                    Point = i;
                    break;
                }
            }
            MoveByIndex(Point + 0.5- width);
        }

        private double[] GetMaxMin(double start, double end)
        {
            double Max = 0;
            double Min = 100000000;
            double Vol = 0;
            for (double i = start; i <= end; i++)
            {
                if (i <= spl.Count - 1)
                {
                    StockPt item = (StockPt)spl[(int)i];
                    if (item.High > Max)
                        Max = item.High;
                    if (item.Low < Min)
                        Min = item.Low;
                    if (item.Vol > Vol)
                        Vol = item.Vol;
                }
            }
            double Max2 = 0;
            double Min2 = 100000000;
            foreach (Index item in Indexs)
            {
                double[] MaxMin = item.GetMaxMin(start, end);
                if (item.PanelNo == "2") {                   
                    if (MaxMin[0] > Max2)
                        Max2 = MaxMin[0];
                    if (MaxMin[1] < Min2)
                        Min2 = MaxMin[1];
                }
            }
            double Max3 = 0;
            double Min3 = 100000000;
            foreach (Index item in Indexs)
            {
                double[] MaxMin = item.GetMaxMin(start, end);
                if (item.PanelNo == "3")
                {
                    if (MaxMin[0] > Max3)
                        Max3 = MaxMin[0];
                    if (MaxMin[1] < Min3)
                        Min3 = MaxMin[1];
                }
            }

            return new double[]{ Max,Min, Max2, Min2, Max3, Min3 };
        }

        public ZedGraphControl ZedGraphControl
        {
            get { return control; }
        }

         
    }
}
