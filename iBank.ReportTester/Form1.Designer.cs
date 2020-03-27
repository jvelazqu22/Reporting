namespace iBank.ReportTester
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
            this.btnSubmitReport = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtReportId = new System.Windows.Forms.TextBox();
            this.lblProcessing = new System.Windows.Forms.Label();
            this.btnRunBroadcastServer = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtServerNumber = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBatchNumber = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblProcessingBcst = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnRunReportQueueManager = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSubmitReport
            // 
            this.btnSubmitReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubmitReport.Location = new System.Drawing.Point(465, 95);
            this.btnSubmitReport.Margin = new System.Windows.Forms.Padding(2);
            this.btnSubmitReport.Name = "btnSubmitReport";
            this.btnSubmitReport.Size = new System.Drawing.Size(157, 38);
            this.btnSubmitReport.TabIndex = 0;
            this.btnSubmitReport.Text = "Submit";
            this.btnSubmitReport.UseVisualStyleBackColor = true;
            this.btnSubmitReport.Click += new System.EventHandler(this.btnSubmitReport_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 25);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Report ID";
            // 
            // txtReportId
            // 
            this.txtReportId.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReportId.Location = new System.Drawing.Point(4, 54);
            this.txtReportId.Margin = new System.Windows.Forms.Padding(2);
            this.txtReportId.Name = "txtReportId";
            this.txtReportId.Size = new System.Drawing.Size(618, 29);
            this.txtReportId.TabIndex = 2;
            // 
            // lblProcessing
            // 
            this.lblProcessing.AutoSize = true;
            this.lblProcessing.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProcessing.Location = new System.Drawing.Point(529, 147);
            this.lblProcessing.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblProcessing.Name = "lblProcessing";
            this.lblProcessing.Size = new System.Drawing.Size(99, 20);
            this.lblProcessing.TabIndex = 3;
            this.lblProcessing.Text = "Processing...";
            this.lblProcessing.Visible = false;
            // 
            // btnRunBroadcastServer
            // 
            this.btnRunBroadcastServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRunBroadcastServer.Location = new System.Drawing.Point(465, 27);
            this.btnRunBroadcastServer.Margin = new System.Windows.Forms.Padding(2);
            this.btnRunBroadcastServer.Name = "btnRunBroadcastServer";
            this.btnRunBroadcastServer.Size = new System.Drawing.Size(157, 42);
            this.btnRunBroadcastServer.TabIndex = 4;
            this.btnRunBroadcastServer.Text = "Submit";
            this.btnRunBroadcastServer.UseVisualStyleBackColor = true;
            this.btnRunBroadcastServer.Click += new System.EventHandler(this.btnRunBroadcastServer_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtServerNumber);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtBatchNumber);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.lblProcessingBcst);
            this.groupBox1.Controls.Add(this.btnRunBroadcastServer);
            this.groupBox1.Location = new System.Drawing.Point(15, 245);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(626, 112);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Broadcast Server";
            // 
            // txtServerNumber
            // 
            this.txtServerNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtServerNumber.Location = new System.Drawing.Point(90, 72);
            this.txtServerNumber.Margin = new System.Windows.Forms.Padding(2);
            this.txtServerNumber.Name = "txtServerNumber";
            this.txtServerNumber.Size = new System.Drawing.Size(44, 29);
            this.txtServerNumber.TabIndex = 10;
            this.txtServerNumber.Text = "200";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(4, 80);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 20);
            this.label3.TabIndex = 9;
            this.label3.Text = "Server #";
            // 
            // txtBatchNumber
            // 
            this.txtBatchNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBatchNumber.Location = new System.Drawing.Point(90, 27);
            this.txtBatchNumber.Margin = new System.Windows.Forms.Padding(2);
            this.txtBatchNumber.Name = "txtBatchNumber";
            this.txtBatchNumber.Size = new System.Drawing.Size(251, 29);
            this.txtBatchNumber.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(4, 27);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "Batch #";
            // 
            // lblProcessingBcst
            // 
            this.lblProcessingBcst.AutoSize = true;
            this.lblProcessingBcst.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProcessingBcst.Location = new System.Drawing.Point(529, 80);
            this.lblProcessingBcst.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblProcessingBcst.Name = "lblProcessingBcst";
            this.lblProcessingBcst.Size = new System.Drawing.Size(99, 20);
            this.lblProcessingBcst.TabIndex = 6;
            this.lblProcessingBcst.Text = "Processing...";
            this.lblProcessingBcst.Visible = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtReportId);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.lblProcessing);
            this.groupBox2.Controls.Add(this.btnSubmitReport);
            this.groupBox2.Location = new System.Drawing.Point(15, 24);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(626, 181);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Online Report";
            // 
            // btnRunReportQueueManager
            // 
            this.btnRunReportQueueManager.Location = new System.Drawing.Point(223, 382);
            this.btnRunReportQueueManager.Name = "btnRunReportQueueManager";
            this.btnRunReportQueueManager.Size = new System.Drawing.Size(75, 23);
            this.btnRunReportQueueManager.TabIndex = 8;
            this.btnRunReportQueueManager.Text = "Run";
            this.btnRunReportQueueManager.UseVisualStyleBackColor = true;
            this.btnRunReportQueueManager.Click += new System.EventHandler(this.btnRunReportQueueManager_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(11, 385);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(197, 20);
            this.label4.TabIndex = 9;
            this.label4.Text = "Report Queue Manager";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 425);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnRunReportQueueManager);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Report Tester";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSubmitReport;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtReportId;
        private System.Windows.Forms.Label lblProcessing;
        private System.Windows.Forms.Button btnRunBroadcastServer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblProcessingBcst;
        private System.Windows.Forms.TextBox txtBatchNumber;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtServerNumber;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnRunReportQueueManager;
        private System.Windows.Forms.Label label4;
    }
}

