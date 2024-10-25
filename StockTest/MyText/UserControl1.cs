using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyText
{
    public partial class UserControl1: UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            int padding = (MyText.Height - label1.Height - textBox1.Height) / 2;
            MyText.Padding = new Padding(0, padding, 0, padding);
        }
        public string Title
        {
            get { return label1.Text; }
            set { label1.Text = value; }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            OnTextChanged(EventArgs.Empty);
        }
        public override string Text
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }

        private void textBox1_Resize(object sender, EventArgs e)
        {
            
        }
    }
}
