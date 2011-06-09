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
            this.labelCheezRootFolder = new System.Windows.Forms.Label();
            this.textBoxCheezRootFolder = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.numericUpDownFetchCount = new System.Windows.Forms.NumericUpDown();
            this.labelFetchCount = new System.Windows.Forms.Label();
            this.checkBoxDeleteOnExit = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFetchCount)).BeginInit();
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
            this.textBoxCheezRootFolder.Location = new System.Drawing.Point(134, 6);
            this.textBoxCheezRootFolder.Name = "textBoxCheezRootFolder";
            this.textBoxCheezRootFolder.Size = new System.Drawing.Size(229, 20);
            this.textBoxCheezRootFolder.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(369, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(28, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // numericUpDownFetchCount
            // 
            this.numericUpDownFetchCount.Location = new System.Drawing.Point(134, 32);
            this.numericUpDownFetchCount.Name = "numericUpDownFetchCount";
            this.numericUpDownFetchCount.Size = new System.Drawing.Size(59, 20);
            this.numericUpDownFetchCount.TabIndex = 3;
            // 
            // labelFetchCount
            // 
            this.labelFetchCount.AutoSize = true;
            this.labelFetchCount.Location = new System.Drawing.Point(22, 34);
            this.labelFetchCount.Name = "labelFetchCount";
            this.labelFetchCount.Size = new System.Drawing.Size(106, 13);
            this.labelFetchCount.TabIndex = 4;
            this.labelFetchCount.Text = "Fetch n items ahead:";
            // 
            // checkBoxDeleteOnExit
            // 
            this.checkBoxDeleteOnExit.AutoSize = true;
            this.checkBoxDeleteOnExit.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxDeleteOnExit.Location = new System.Drawing.Point(4, 58);
            this.checkBoxDeleteOnExit.Name = "checkBoxDeleteOnExit";
            this.checkBoxDeleteOnExit.Size = new System.Drawing.Size(189, 17);
            this.checkBoxDeleteOnExit.TabIndex = 5;
            this.checkBoxDeleteOnExit.Text = "Delete downloaded Images on exit";
            this.checkBoxDeleteOnExit.UseVisualStyleBackColor = true;
            // 
            // ConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 318);
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

    }
}