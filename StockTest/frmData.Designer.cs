namespace StockTest
{
    partial class frmData
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.StockNo = new MyText.UserControl1();
            this.startTime = new System.Windows.Forms.DateTimePicker();
            this.EndTime = new System.Windows.Forms.DateTimePicker();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(7, 74);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(1162, 452);
            this.dataGridView1.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(24, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "GetData";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // StockNo
            // 
            this.StockNo.Location = new System.Drawing.Point(105, 12);
            this.StockNo.Name = "StockNo";
            this.StockNo.Size = new System.Drawing.Size(229, 46);
            this.StockNo.TabIndex = 5;
            this.StockNo.Title = "StockNo";
            // 
            // startTime
            // 
            this.startTime.Location = new System.Drawing.Point(323, 13);
            this.startTime.Name = "startTime";
            this.startTime.Size = new System.Drawing.Size(149, 25);
            this.startTime.TabIndex = 6;
            // 
            // EndTime
            // 
            this.EndTime.Location = new System.Drawing.Point(478, 13);
            this.EndTime.Name = "EndTime";
            this.EndTime.Size = new System.Drawing.Size(149, 25);
            this.EndTime.TabIndex = 7;
            // 
            // frmData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1182, 539);
            this.Controls.Add(this.EndTime);
            this.Controls.Add(this.startTime);
            this.Controls.Add(this.StockNo);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "frmData";
            this.Text = "frmData";
            this.Load += new System.EventHandler(this.frmData_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button1;
        private MyText.UserControl1 StockNo;
        private System.Windows.Forms.DateTimePicker startTime;
        private System.Windows.Forms.DateTimePicker EndTime;
    }
}