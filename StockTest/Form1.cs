using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockTest
{
    public partial class Auto : Form
    {
        public Auto()
        {
            InitializeComponent();

            AddMenu("Main", "Sub", "frmData");
            AddMenu("Main", "Sub2", "Simulator");
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private ToolStripMenuItem GetMenu(MenuStrip menuMain,string name)
        {
            foreach (ToolStripMenuItem item in menuMain.Items)
            {
                if (item.Text == name)
                    return item;
            }
            return null;
        }
        private void AddMenu(string name,string subname,string FormName)
        {
            
            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            ToolStripMenuItem oldmenu = GetMenu(menuMain, name);
            if (oldmenu!=null)
                menuItem = oldmenu;
            else
            {
                menuItem.Text = name;
                menuItem.MouseMove += MenuItem_MouseMove;
                menuItem.MouseLeave += MenuItem_MouseLeave;
            }
            ToolStripMenuItem subItem = new ToolStripMenuItem();
            subItem.Text = subname;
            subItem.MouseMove += MenuItem_MouseMove;
            subItem.MouseLeave += MenuItem_MouseLeave;
            subItem.Click += delegate (object sender, EventArgs e) { OpenForm_Click(sender, e, FormName); };
            menuItem.DropDownItems.Add(subItem); 

            menuMain.Items.Add(menuItem);
        }
        private void OpenForm_Click(object sender, EventArgs e, string formName)
        {
            var frm = Application.OpenForms[formName];
            if (frm == null)
            {
                var form = Activator.CreateInstance(Type.GetType("StockTest." + formName)) as Form;
                form.MdiParent = this;
                form.WindowState = FormWindowState.Maximized; 
                form.Show();
            }
            else
            {
                frm.Focus();
            }
        }
        private void MenuItem_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void MenuItem_MouseMove(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }
        private void Auto_Load(object sender, EventArgs e)
        {
            var form = Activator.CreateInstance(Type.GetType("StockTest.Simulator" )) as Form;
            form.MdiParent = this;
            form.WindowState = FormWindowState.Maximized;
            form.Show();
        }
    }
}
