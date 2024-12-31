using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GroupLabel
{
    public partial class UserControl1: UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        Dictionary<string, Label> ShowData = new Dictionary<string, Label>();
         
        private string[] split = new string[] { "," };
        private string _Lables;
        [Category("Custom Properties")]
        [Description("Custom text property for the user control.")]
        public string Lables
        {
            get { return _Lables; }
            set
            {
                flowLayoutPanel1.Controls.Clear();
                ShowData.Clear();
                _Lables = value;
                if (value != "")
                {
                    string[] label = value.Split(split, StringSplitOptions.None);
                    foreach (string item in label)
                    {  
                        Label lbl = new Label();
                        lbl.Text = item;                        
                        lbl.AutoSize = true;
                        lbl.Font = new Font(lbl.Font.FontFamily, customFontSize);
                        flowLayoutPanel1.Controls.Add(lbl);
                        Label lbValue = new Label();
                        lbValue.Text = "";
                        lbValue.AutoSize = true;
                        lbValue.Font = new Font(lbValue.Font.FontFamily, customFontSize);
                        lbValue.ForeColor = customColor;
                        flowLayoutPanel1.Controls.Add(lbValue);
                        ShowData.Add(item, lbValue);
                    }
                } 
            }
        }

        public void SetData(string key, string value)
        {
            if (ShowData.ContainsKey(key))
            {
                ShowData[key].Text = value;
            } 
        }

        public DateTime CurrentDate =  DateTime.Now.AddDays(1);
        public double CurrentPrice = 0;

        public void SetData(DataRow lRow)
        {
            string Date = lRow["Date"].ToString();
            CurrentDate= DateTime.Parse(Date);
            string Open = lRow["Open"].ToString();
            string High = lRow["High"].ToString();
            string Low = lRow["Low"].ToString();
            string Close = lRow["Close"].ToString();
            CurrentPrice = double.Parse(Close);
            string Volume = lRow["Volume"].ToString();
            ShowData["Date"].Text = Date;
            ShowData["O"].Text = Open;
            ShowData["H"].Text = High;
            ShowData["L"].Text = Low;
            ShowData["C"].Text = Close;
            ShowData["V"].Text = Volume;
        }

        private Color customColor;
        [Category("Custom Properties")]
        [Description("Custom color property for the user control.")]
        public Color CustomColor
        {
            get { return customColor; }
            set
            {
                customColor = value;
                foreach (Label item in ShowData.Values)
                {
                    item.ForeColor = value;
                }
            }
        }

        private float customFontSize = 12f;
        [Category("Custom Properties")]
        [Description("Custom font size property for the user control.")]
        public float CustomFontSize
        {
            get { return customFontSize; }
            set
            {
                customFontSize = value;
                foreach (Label item in ShowData.Values)
                {
                    item.Font = new Font(item.Font.FontFamily, customFontSize);
                }
            }
        }

        public string GetData(string key)
        {
            if (ShowData.ContainsKey(key))
            {
                return ShowData[key].Text;
            }
            return "";
        }

        private void flowLayoutPanel1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
