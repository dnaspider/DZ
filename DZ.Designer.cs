namespace dz
{
    partial class DZ
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
            this.components = new System.ComponentModel.Container();
            this.Timer1 = new System.Windows.Forms.Timer(this.components);
            this.SplitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ListBox1 = new System.Windows.Forms.ListBox();
            this.TextBox1 = new System.Windows.Forms.RichTextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).BeginInit();
            this.SplitContainer1.Panel1.SuspendLayout();
            this.SplitContainer1.Panel2.SuspendLayout();
            this.SplitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Timer1
            // 
            this.Timer1.Enabled = true;
            this.Timer1.Interval = 150;
            this.Timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // SplitContainer1
            // 
            this.SplitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SplitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SplitContainer1.Location = new System.Drawing.Point(12, 13);
            this.SplitContainer1.Name = "SplitContainer1";
            this.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitContainer1.Panel1
            // 
            this.SplitContainer1.Panel1.Controls.Add(this.ListBox1);
            // 
            // SplitContainer1.Panel2
            // 
            this.SplitContainer1.Panel2.Controls.Add(this.TextBox1);
            this.SplitContainer1.Panel2.Controls.Add(this.textBox2);
            this.SplitContainer1.Size = new System.Drawing.Size(260, 204);
            this.SplitContainer1.SplitterDistance = 77;
            this.SplitContainer1.SplitterWidth = 18;
            this.SplitContainer1.TabIndex = 0;
            this.SplitContainer1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SplitContainer1_MouseDown);
            // 
            // ListBox1
            // 
            this.ListBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ListBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListBox1.FormattingEnabled = true;
            this.ListBox1.Location = new System.Drawing.Point(0, 0);
            this.ListBox1.Name = "ListBox1";
            this.ListBox1.Size = new System.Drawing.Size(258, 75);
            this.ListBox1.TabIndex = 0;
            this.ListBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ListBox1_KeyPress);
            this.ListBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ListBox1_KeyUp);
            // 
            // TextBox1
            // 
            this.TextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBox1.EnableAutoDragDrop = true;
            this.TextBox1.Location = new System.Drawing.Point(0, 0);
            this.TextBox1.Name = "TextBox1";
            this.TextBox1.Size = new System.Drawing.Size(258, 107);
            this.TextBox1.TabIndex = 3;
            this.TextBox1.Text = "";
            this.TextBox1.DoubleClick += new System.EventHandler(this.TextBox1_DoubleClick);
            this.TextBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyDown);
            this.TextBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox1_KeyPress);
            this.TextBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox1_KeyUp);
            this.TextBox1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.TextBox1_PreviewKeyDown);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(68, 157);
            this.textBox2.Name = "textBox2";
            this.textBox2.PasswordChar = '*';
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 2;
            this.textBox2.UseSystemPasswordChar = true;
            this.textBox2.Visible = false;
            this.textBox2.TextChanged += new System.EventHandler(this.TextBox2_TextChanged);
            // 
            // DZ
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(284, 229);
            this.Controls.Add(this.SplitContainer1);
            this.Name = "DZ";
            this.Text = "DZ";
            this.TransparencyKey = System.Drawing.Color.GhostWhite;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DZ_FormClosing);
            this.Load += new System.EventHandler(this.DZ_Load);
            this.DoubleClick += new System.EventHandler(this.DZ_DoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DZ_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DZ_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DZ_MouseUp);
            this.SplitContainer1.Panel1.ResumeLayout(false);
            this.SplitContainer1.Panel2.ResumeLayout(false);
            this.SplitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).EndInit();
            this.SplitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer Timer1;
        private System.Windows.Forms.SplitContainer SplitContainer1;
        private System.Windows.Forms.ListBox ListBox1;
        private System.Windows.Forms.RichTextBox TextBox1;
        private System.Windows.Forms.TextBox textBox2;
    }
}

