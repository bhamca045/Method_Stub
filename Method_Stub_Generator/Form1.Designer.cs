namespace Method_Stub_Generator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.txtConnString = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtStoreProc = new System.Windows.Forms.TextBox();
            this.txtDBName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cgOption1 = new System.Windows.Forms.RadioButton();
            this.cgOption2 = new System.Windows.Forms.RadioButton();
            this.btnGenerateCode = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.codeOutput = new System.Windows.Forms.RichTextBox();
            this.btnCopyCode = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.txtControllerName = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "DB Conn. String:";
            // 
            // txtConnString
            // 
            this.txtConnString.Location = new System.Drawing.Point(114, 13);
            this.txtConnString.Name = "txtConnString";
            this.txtConnString.Size = new System.Drawing.Size(543, 20);
            this.txtConnString.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Store_Proc Name:";
            // 
            // txtStoreProc
            // 
            this.txtStoreProc.Location = new System.Drawing.Point(114, 41);
            this.txtStoreProc.Name = "txtStoreProc";
            this.txtStoreProc.Size = new System.Drawing.Size(364, 20);
            this.txtStoreProc.TabIndex = 3;
            // 
            // txtDBName
            // 
            this.txtDBName.Location = new System.Drawing.Point(114, 70);
            this.txtDBName.Name = "txtDBName";
            this.txtDBName.Size = new System.Drawing.Size(175, 20);
            this.txtDBName.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Data Base Name:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(691, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = resources.GetString("label4.Text");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(13, 109);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(128, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Code Generation For:";
            // 
            // cgOption1
            // 
            this.cgOption1.AutoSize = true;
            this.cgOption1.Location = new System.Drawing.Point(12, 127);
            this.cgOption1.Name = "cgOption1";
            this.cgOption1.Size = new System.Drawing.Size(182, 17);
            this.cgOption1.TabIndex = 11;
            this.cgOption1.Text = "DAL Library, Controller with UTCs";
            this.cgOption1.UseVisualStyleBackColor = true;
            // 
            // cgOption2
            // 
            this.cgOption2.AutoSize = true;
            this.cgOption2.Checked = true;
            this.cgOption2.Location = new System.Drawing.Point(304, 127);
            this.cgOption2.Name = "cgOption2";
            this.cgOption2.Size = new System.Drawing.Size(174, 17);
            this.cgOption2.TabIndex = 12;
            this.cgOption2.TabStop = true;
            this.cgOption2.Text = "BL Library, Controller with UTCs";
            this.cgOption2.UseVisualStyleBackColor = true;
            // 
            // btnGenerateCode
            // 
            this.btnGenerateCode.Location = new System.Drawing.Point(562, 121);
            this.btnGenerateCode.Name = "btnGenerateCode";
            this.btnGenerateCode.Size = new System.Drawing.Size(95, 23);
            this.btnGenerateCode.TabIndex = 15;
            this.btnGenerateCode.Text = "Generate Code.";
            this.btnGenerateCode.UseVisualStyleBackColor = true;
            this.btnGenerateCode.Click += new System.EventHandler(this.btnGenerateCode_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 146);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(691, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = resources.GetString("label6.Text");
            // 
            // codeOutput
            // 
            this.codeOutput.Location = new System.Drawing.Point(16, 165);
            this.codeOutput.Name = "codeOutput";
            this.codeOutput.Size = new System.Drawing.Size(688, 294);
            this.codeOutput.TabIndex = 17;
            this.codeOutput.Text = "";
            this.codeOutput.WordWrap = false;
            // 
            // btnCopyCode
            // 
            this.btnCopyCode.Enabled = false;
            this.btnCopyCode.Location = new System.Drawing.Point(479, 465);
            this.btnCopyCode.Name = "btnCopyCode";
            this.btnCopyCode.Size = new System.Drawing.Size(75, 23);
            this.btnCopyCode.TabIndex = 18;
            this.btnCopyCode.Text = "Copy Code";
            this.btnCopyCode.UseVisualStyleBackColor = true;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(579, 465);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 19;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(310, 74);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(66, 13);
            this.label7.TabIndex = 20;
            this.label7.Text = "Class Name:";
            // 
            // txtControllerName
            // 
            this.txtControllerName.Location = new System.Drawing.Point(379, 72);
            this.txtControllerName.Name = "txtControllerName";
            this.txtControllerName.Size = new System.Drawing.Size(180, 20);
            this.txtControllerName.TabIndex = 21;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(559, 75);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(131, 13);
            this.label8.TabIndex = 22;
            this.label8.Text = "(Exclude \'Controller\' Word)";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(719, 500);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtControllerName);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnCopyCode);
            this.Controls.Add(this.codeOutput);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnGenerateCode);
            this.Controls.Add(this.cgOption2);
            this.Controls.Add(this.cgOption1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtDBName);
            this.Controls.Add(this.txtStoreProc);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtConnString);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Method Stub Generator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtConnString;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtStoreProc;
        private System.Windows.Forms.TextBox txtDBName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton cgOption1;
        private System.Windows.Forms.RadioButton cgOption2;
        private System.Windows.Forms.Button btnGenerateCode;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RichTextBox codeOutput;
        private System.Windows.Forms.Button btnCopyCode;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtControllerName;
        private System.Windows.Forms.Label label8;
    }
}

