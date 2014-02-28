namespace SPGeneratorForms
{
    partial class SQLDiffForm
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
            this.checkBoxTrusted1 = new System.Windows.Forms.CheckBox();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.textPassword1 = new System.Windows.Forms.TextBox();
            this.textUser1 = new System.Windows.Forms.TextBox();
            this.comboServers1 = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBases1 = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBases2 = new System.Windows.Forms.ComboBox();
            this.comboServers2 = new System.Windows.Forms.ComboBox();
            this.checkBoxTrusted2 = new System.Windows.Forms.CheckBox();
            this.textUser2 = new System.Windows.Forms.TextBox();
            this.textPassword2 = new System.Windows.Forms.TextBox();
            this.checkBoxTables = new System.Windows.Forms.CheckBox();
            this.checkBoxConstraints = new System.Windows.Forms.CheckBox();
            this.checkBoxFunctions = new System.Windows.Forms.CheckBox();
            this.checkBoxProcedures = new System.Windows.Forms.CheckBox();
            this.checkBoxIndexes = new System.Windows.Forms.CheckBox();
            this.buttonDiff = new System.Windows.Forms.Button();
            this.checkBoxFKeys = new System.Windows.Forms.CheckBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.listBoxNotExists = new System.Windows.Forms.ListBox();
            this.labelProgress = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.listBoxDiffers = new System.Windows.Forms.ListBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxTrusted1
            // 
            this.checkBoxTrusted1.Checked = true;
            this.checkBoxTrusted1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxTrusted1.Location = new System.Drawing.Point(130, 46);
            this.checkBoxTrusted1.Name = "checkBoxTrusted1";
            this.checkBoxTrusted1.Size = new System.Drawing.Size(62, 20);
            this.checkBoxTrusted1.TabIndex = 13;
            this.checkBoxTrusted1.Text = "Trusted";
            this.checkBoxTrusted1.UseVisualStyleBackColor = true;
            // 
            // buttonConnect
            // 
            this.buttonConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonConnect.Location = new System.Drawing.Point(422, 30);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(64, 21);
            this.buttonConnect.TabIndex = 12;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // textPassword1
            // 
            this.textPassword1.Enabled = false;
            this.textPassword1.Location = new System.Drawing.Point(63, 46);
            this.textPassword1.Name = "textPassword1";
            this.textPassword1.ReadOnly = true;
            this.textPassword1.Size = new System.Drawing.Size(61, 20);
            this.textPassword1.TabIndex = 11;
            this.textPassword1.Text = "Password";
            // 
            // textUser1
            // 
            this.textUser1.Enabled = false;
            this.textUser1.Location = new System.Drawing.Point(6, 46);
            this.textUser1.Name = "textUser1";
            this.textUser1.ReadOnly = true;
            this.textUser1.Size = new System.Drawing.Size(51, 20);
            this.textUser1.TabIndex = 10;
            this.textUser1.Text = "User";
            // 
            // comboServers1
            // 
            this.comboServers1.Location = new System.Drawing.Point(6, 19);
            this.comboServers1.Name = "comboServers1";
            this.comboServers1.Size = new System.Drawing.Size(186, 21);
            this.comboServers1.TabIndex = 9;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBases1);
            this.groupBox1.Controls.Add(this.comboServers1);
            this.groupBox1.Controls.Add(this.checkBoxTrusted1);
            this.groupBox1.Controls.Add(this.textUser1);
            this.groupBox1.Controls.Add(this.textPassword1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(199, 102);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Сервер 1";
            // 
            // comboBases1
            // 
            this.comboBases1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBases1.Location = new System.Drawing.Point(6, 72);
            this.comboBases1.Name = "comboBases1";
            this.comboBases1.Size = new System.Drawing.Size(186, 21);
            this.comboBases1.TabIndex = 14;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboBases2);
            this.groupBox2.Controls.Add(this.comboServers2);
            this.groupBox2.Controls.Add(this.checkBoxTrusted2);
            this.groupBox2.Controls.Add(this.textUser2);
            this.groupBox2.Controls.Add(this.textPassword2);
            this.groupBox2.Location = new System.Drawing.Point(217, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(199, 102);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Сервер 2";
            // 
            // comboBases2
            // 
            this.comboBases2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBases2.Location = new System.Drawing.Point(6, 72);
            this.comboBases2.Name = "comboBases2";
            this.comboBases2.Size = new System.Drawing.Size(186, 21);
            this.comboBases2.TabIndex = 14;
            // 
            // comboServers2
            // 
            this.comboServers2.Location = new System.Drawing.Point(6, 19);
            this.comboServers2.Name = "comboServers2";
            this.comboServers2.Size = new System.Drawing.Size(186, 21);
            this.comboServers2.TabIndex = 9;
            // 
            // checkBoxTrusted2
            // 
            this.checkBoxTrusted2.Checked = true;
            this.checkBoxTrusted2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxTrusted2.Location = new System.Drawing.Point(130, 46);
            this.checkBoxTrusted2.Name = "checkBoxTrusted2";
            this.checkBoxTrusted2.Size = new System.Drawing.Size(62, 20);
            this.checkBoxTrusted2.TabIndex = 13;
            this.checkBoxTrusted2.Text = "Trusted";
            this.checkBoxTrusted2.UseVisualStyleBackColor = true;
            // 
            // textUser2
            // 
            this.textUser2.Enabled = false;
            this.textUser2.Location = new System.Drawing.Point(6, 46);
            this.textUser2.Name = "textUser2";
            this.textUser2.ReadOnly = true;
            this.textUser2.Size = new System.Drawing.Size(51, 20);
            this.textUser2.TabIndex = 10;
            this.textUser2.Text = "User";
            // 
            // textPassword2
            // 
            this.textPassword2.Enabled = false;
            this.textPassword2.Location = new System.Drawing.Point(63, 46);
            this.textPassword2.Name = "textPassword2";
            this.textPassword2.ReadOnly = true;
            this.textPassword2.Size = new System.Drawing.Size(61, 20);
            this.textPassword2.TabIndex = 11;
            this.textPassword2.Text = "Password";
            // 
            // checkBoxTables
            // 
            this.checkBoxTables.Location = new System.Drawing.Point(496, 16);
            this.checkBoxTables.Name = "checkBoxTables";
            this.checkBoxTables.Size = new System.Drawing.Size(110, 24);
            this.checkBoxTables.TabIndex = 16;
            this.checkBoxTables.Text = "Tables";
            this.checkBoxTables.UseVisualStyleBackColor = true;
            // 
            // checkBoxConstraints
            // 
            this.checkBoxConstraints.Location = new System.Drawing.Point(513, 40);
            this.checkBoxConstraints.Name = "checkBoxConstraints";
            this.checkBoxConstraints.Size = new System.Drawing.Size(93, 24);
            this.checkBoxConstraints.TabIndex = 17;
            this.checkBoxConstraints.Text = "Constraints";
            this.checkBoxConstraints.UseVisualStyleBackColor = true;
            // 
            // checkBoxFunctions
            // 
            this.checkBoxFunctions.Location = new System.Drawing.Point(643, 16);
            this.checkBoxFunctions.Name = "checkBoxFunctions";
            this.checkBoxFunctions.Size = new System.Drawing.Size(93, 24);
            this.checkBoxFunctions.TabIndex = 18;
            this.checkBoxFunctions.Text = "Functions";
            this.checkBoxFunctions.UseVisualStyleBackColor = true;
            // 
            // checkBoxProcedures
            // 
            this.checkBoxProcedures.Location = new System.Drawing.Point(643, 40);
            this.checkBoxProcedures.Name = "checkBoxProcedures";
            this.checkBoxProcedures.Size = new System.Drawing.Size(93, 24);
            this.checkBoxProcedures.TabIndex = 19;
            this.checkBoxProcedures.Text = "Procedures";
            this.checkBoxProcedures.UseVisualStyleBackColor = true;
            // 
            // checkBoxIndexes
            // 
            this.checkBoxIndexes.Location = new System.Drawing.Point(513, 61);
            this.checkBoxIndexes.Name = "checkBoxIndexes";
            this.checkBoxIndexes.Size = new System.Drawing.Size(93, 24);
            this.checkBoxIndexes.TabIndex = 20;
            this.checkBoxIndexes.Text = "Indexes";
            this.checkBoxIndexes.UseVisualStyleBackColor = true;
            // 
            // buttonDiff
            // 
            this.buttonDiff.Location = new System.Drawing.Point(643, 70);
            this.buttonDiff.Name = "buttonDiff";
            this.buttonDiff.Size = new System.Drawing.Size(98, 23);
            this.buttonDiff.TabIndex = 21;
            this.buttonDiff.Text = "Diff";
            this.buttonDiff.UseVisualStyleBackColor = true;
            this.buttonDiff.Click += new System.EventHandler(this.buttonDiff_Click);
            // 
            // checkBoxFKeys
            // 
            this.checkBoxFKeys.Location = new System.Drawing.Point(513, 82);
            this.checkBoxFKeys.Name = "checkBoxFKeys";
            this.checkBoxFKeys.Size = new System.Drawing.Size(93, 24);
            this.checkBoxFKeys.TabIndex = 22;
            this.checkBoxFKeys.Text = "ForeignKeys";
            this.checkBoxFKeys.UseVisualStyleBackColor = true;
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(0, 608);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(867, 23);
            this.progressBar1.TabIndex = 23;
            // 
            // listBoxNotExists
            // 
            this.listBoxNotExists.FormattingEnabled = true;
            this.listBoxNotExists.Location = new System.Drawing.Point(12, 120);
            this.listBoxNotExists.Name = "listBoxNotExists";
            this.listBoxNotExists.Size = new System.Drawing.Size(173, 160);
            this.listBoxNotExists.TabIndex = 26;
            this.listBoxNotExists.SelectedValueChanged += new System.EventHandler(this.listBoxNotExists_SelectedValueChanged);
            // 
            // labelProgress
            // 
            this.labelProgress.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelProgress.Location = new System.Drawing.Point(0, 592);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(867, 16);
            this.labelProgress.TabIndex = 27;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.richTextBox1.Location = new System.Drawing.Point(217, 121);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(269, 459);
            this.richTextBox1.TabIndex = 28;
            this.richTextBox1.Text = "";
            // 
            // richTextBox2
            // 
            this.richTextBox2.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.richTextBox2.Location = new System.Drawing.Point(492, 121);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(269, 459);
            this.richTextBox2.TabIndex = 29;
            this.richTextBox2.Text = "";
            // 
            // listBoxDiffers
            // 
            this.listBoxDiffers.FormattingEnabled = true;
            this.listBoxDiffers.Location = new System.Drawing.Point(12, 286);
            this.listBoxDiffers.Name = "listBoxDiffers";
            this.listBoxDiffers.Size = new System.Drawing.Size(173, 160);
            this.listBoxDiffers.TabIndex = 30;
            this.listBoxDiffers.SelectedValueChanged += new System.EventHandler(this.listBoxDiffers_SelectedValueChanged);
            // 
            // SQLDiffForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(867, 631);
            this.Controls.Add(this.listBoxDiffers);
            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.labelProgress);
            this.Controls.Add(this.listBoxNotExists);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.checkBoxFKeys);
            this.Controls.Add(this.buttonDiff);
            this.Controls.Add(this.checkBoxIndexes);
            this.Controls.Add(this.checkBoxProcedures);
            this.Controls.Add(this.checkBoxFunctions);
            this.Controls.Add(this.checkBoxConstraints);
            this.Controls.Add(this.checkBoxTables);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonConnect);
            this.Name = "SQLDiffForm";
            this.Text = "SQLDiffForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxTrusted1;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.TextBox textPassword1;
        private System.Windows.Forms.TextBox textUser1;
        private System.Windows.Forms.ComboBox comboServers1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBases1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox comboBases2;
        private System.Windows.Forms.ComboBox comboServers2;
        private System.Windows.Forms.CheckBox checkBoxTrusted2;
        private System.Windows.Forms.TextBox textUser2;
        private System.Windows.Forms.TextBox textPassword2;
        private System.Windows.Forms.CheckBox checkBoxTables;
        private System.Windows.Forms.CheckBox checkBoxConstraints;
        private System.Windows.Forms.CheckBox checkBoxFunctions;
        private System.Windows.Forms.CheckBox checkBoxProcedures;
        private System.Windows.Forms.CheckBox checkBoxIndexes;
        private System.Windows.Forms.Button buttonDiff;
        private System.Windows.Forms.CheckBox checkBoxFKeys;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ListBox listBoxNotExists;
        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.ListBox listBoxDiffers;
    }
}