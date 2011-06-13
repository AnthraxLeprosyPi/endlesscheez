namespace EndlessCheez.Plugin {
    partial class ConfigurationForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationForm));
            this.labelCheezRootFolder = new System.Windows.Forms.Label();
            this.textBoxCheezRootFolder = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.numericUpDownFetchCount = new System.Windows.Forms.NumericUpDown();
            this.labelFetchCount = new System.Windows.Forms.Label();
            this.checkBoxDeleteOnExit = new System.Windows.Forms.CheckBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFetchCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelCheezRootFolder
            // 
            this.labelCheezRootFolder.AutoSize = true;
            this.labelCheezRootFolder.Location = new System.Drawing.Point(12, 9);
            this.labelCheezRootFolder.Name = "labelCheezRootFolder";
            this.labelCheezRootFolder.Size = new System.Drawing.Size(116, 13);
            this.labelCheezRootFolder.TabIndex = 0;
            this.labelCheezRootFolder.Text = "Local Download folder:";
            // 
            // textBoxCheezRootFolder
            // 
            this.textBoxCheezRootFolder.Location = new System.Drawing.Point(145, 6);
            this.textBoxCheezRootFolder.Name = "textBoxCheezRootFolder";
            this.textBoxCheezRootFolder.Size = new System.Drawing.Size(448, 20);
            this.textBoxCheezRootFolder.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(599, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(35, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // numericUpDownFetchCount
            // 
            this.numericUpDownFetchCount.Location = new System.Drawing.Point(145, 32);
            this.numericUpDownFetchCount.Name = "numericUpDownFetchCount";
            this.numericUpDownFetchCount.Size = new System.Drawing.Size(59, 20);
            this.numericUpDownFetchCount.TabIndex = 3;
            // 
            // labelFetchCount
            // 
            this.labelFetchCount.AutoSize = true;
            this.labelFetchCount.Location = new System.Drawing.Point(12, 34);
            this.labelFetchCount.Name = "labelFetchCount";
            this.labelFetchCount.Size = new System.Drawing.Size(113, 13);
            this.labelFetchCount.TabIndex = 4;
            this.labelFetchCount.Text = "Fetch # items at once:";
            // 
            // checkBoxDeleteOnExit
            // 
            this.checkBoxDeleteOnExit.AutoSize = true;
            this.checkBoxDeleteOnExit.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxDeleteOnExit.Location = new System.Drawing.Point(15, 58);
            this.checkBoxDeleteOnExit.Name = "checkBoxDeleteOnExit";
            this.checkBoxDeleteOnExit.Size = new System.Drawing.Size(189, 17);
            this.checkBoxDeleteOnExit.TabIndex = 5;
            this.checkBoxDeleteOnExit.Text = "Delete downloaded Images on exit";
            this.checkBoxDeleteOnExit.UseVisualStyleBackColor = true;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(445, 170);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(189, 13);
            this.linkLabel1.TabIndex = 6;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "http://developer.cheezburger.com/api";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(304, 170);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "API - Terms && Conditions:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Image = global::EndlessCheez.Properties.Resources.EndlessCheez_enabled;
            this.pictureBox1.Location = new System.Drawing.Point(586, 34);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(634, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "©";
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(116, 170);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(173, 13);
            this.linkLabel2.TabIndex = 11;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "anthrax.leprosy.pi@googlemail.com";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 170);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Contact the author:";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.Tomato;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(15, 88);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(619, 79);
            this.textBox1.TabIndex = 8;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 192);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.checkBoxDeleteOnExit);
            this.Controls.Add(this.labelFetchCount);
            this.Controls.Add(this.numericUpDownFetchCount);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBoxCheezRootFolder);
            this.Controls.Add(this.labelCheezRootFolder);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ConfigurationForm";
            this.Text = "EndlessCheez - Configuration";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFetchCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelCheezRootFolder;
        private System.Windows.Forms.TextBox textBoxCheezRootFolder;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.NumericUpDown numericUpDownFetchCount;
        private System.Windows.Forms.Label labelFetchCount;
        private System.Windows.Forms.CheckBox checkBoxDeleteOnExit;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;

    }
}