namespace Lex_BurnzZ
{
    partial class Form1
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
            this.boxCode = new System.Windows.Forms.RichTextBox();
            this.boxConsole = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonAnalyze = new System.Windows.Forms.Button();
            this.LexTable = new System.Windows.Forms.DataGridView();
            this.lex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tokenTags = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.openCode = new System.Windows.Forms.OpenFileDialog();
            this.SymbolTable = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.LexTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SymbolTable)).BeginInit();
            this.SuspendLayout();
            // 
            // boxCode
            // 
            this.boxCode.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.boxCode.Location = new System.Drawing.Point(12, 46);
            this.boxCode.Name = "boxCode";
            this.boxCode.Size = new System.Drawing.Size(313, 572);
            this.boxCode.TabIndex = 0;
            this.boxCode.Text = "";
            this.boxCode.TextChanged += new System.EventHandler(this.boxCode_TextChanged_1);
            // 
            // boxConsole
            // 
            this.boxConsole.BackColor = System.Drawing.SystemColors.WindowText;
            this.boxConsole.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.boxConsole.ForeColor = System.Drawing.SystemColors.Window;
            this.boxConsole.Location = new System.Drawing.Point(331, 473);
            this.boxConsole.Name = "boxConsole";
            this.boxConsole.ReadOnly = true;
            this.boxConsole.Size = new System.Drawing.Size(497, 145);
            this.boxConsole.TabIndex = 1;
            this.boxConsole.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Code:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(331, 457);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Console Output:";
            // 
            // buttonAnalyze
            // 
            this.buttonAnalyze.Location = new System.Drawing.Point(775, 444);
            this.buttonAnalyze.Name = "buttonAnalyze";
            this.buttonAnalyze.Size = new System.Drawing.Size(53, 23);
            this.buttonAnalyze.TabIndex = 5;
            this.buttonAnalyze.Text = "Analyze";
            this.buttonAnalyze.UseVisualStyleBackColor = true;
            this.buttonAnalyze.Click += new System.EventHandler(this.buttonAnalyze_Click);
            // 
            // LexTable
            // 
            this.LexTable.BackgroundColor = System.Drawing.SystemColors.Control;
            this.LexTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.LexTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.lex,
            this.tokenTags});
            this.LexTable.Location = new System.Drawing.Point(331, 46);
            this.LexTable.Name = "LexTable";
            this.LexTable.ReadOnly = true;
            this.LexTable.RowHeadersVisible = false;
            this.LexTable.Size = new System.Drawing.Size(497, 392);
            this.LexTable.TabIndex = 6;
            this.LexTable.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // lex
            // 
            this.lex.HeaderText = "lexemes";
            this.lex.Name = "lex";
            this.lex.ReadOnly = true;
            this.lex.Width = 175;
            // 
            // tokenTags
            // 
            this.tokenTags.HeaderText = "TokenTags";
            this.tokenTags.Name = "tokenTags";
            this.tokenTags.ReadOnly = true;
            this.tokenTags.Width = 315;
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(704, 444);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(56, 23);
            this.buttonBrowse.TabIndex = 4;
            this.buttonBrowse.Text = "browse...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // openCode
            // 
            this.openCode.FileName = "*.lol";
            // 
            // SymbolTable
            // 
            this.SymbolTable.BackgroundColor = System.Drawing.SystemColors.Control;
            this.SymbolTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SymbolTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.Column3});
            this.SymbolTable.Location = new System.Drawing.Point(834, 46);
            this.SymbolTable.Name = "SymbolTable";
            this.SymbolTable.ReadOnly = true;
            this.SymbolTable.RowHeadersVisible = false;
            this.SymbolTable.Size = new System.Drawing.Size(265, 572);
            this.SymbolTable.TabIndex = 7;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Name";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 70;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Type";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 90;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Value";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1038, 630);
            this.Controls.Add(this.SymbolTable);
            this.Controls.Add(this.LexTable);
            this.Controls.Add(this.buttonAnalyze);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.boxConsole);
            this.Controls.Add(this.boxCode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Form1";
            this.Text = "LOLcode Lexical interpreter";
            ((System.ComponentModel.ISupportInitialize)(this.LexTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SymbolTable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox boxCode;
        private System.Windows.Forms.RichTextBox boxConsole;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonAnalyze;
        private System.Windows.Forms.DataGridView LexTable;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.OpenFileDialog openCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn lex;
        private System.Windows.Forms.DataGridViewTextBoxColumn tokenTags;
        private System.Windows.Forms.DataGridView SymbolTable;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
    }
}

