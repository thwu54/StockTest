using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using My;
namespace StockTest
{
    public partial class frmData : Form
    {
        public frmData()
        {
            InitializeComponent();
            StockNo.Text = "2330";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataTable _lTable = My.MyGraphic.GetDataSource(StockNo.Text, "180");
            dataGridView1.DataSource = _lTable;
        }

        private void frmData_Load(object sender, EventArgs e)
        {

        }
    }
}
