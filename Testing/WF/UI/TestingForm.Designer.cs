namespace K2Field.Utilities.Testing.WF.UI
{
    partial class WorkflowTestUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkflowTestUI));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.txtResults = new System.Windows.Forms.RichTextBox();
            this.btnStartTests = new System.Windows.Forms.Button();
            this.ofdXMLFile = new System.Windows.Forms.OpenFileDialog();
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtK2Server = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtWorkspaceURL = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.chkCreateViewFlowTabs = new System.Windows.Forms.CheckBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolstripStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusTimer = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStripTimer = new System.Windows.Forms.Timer(this.components);
            this.btnDisplayTestContext = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.chkDebug = new System.Windows.Forms.CheckBox();
            this.btnStopTests = new System.Windows.Forms.Button();
            this.chkRunCleanUp = new System.Windows.Forms.CheckBox();
            this.chkRunSetUp = new System.Windows.Forms.CheckBox();
            this.chkRunAsserts = new System.Windows.Forms.CheckBox();
            this.chkSaveTestStructure = new System.Windows.Forms.CheckBox();
            this.AutoCompleteTasks = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(0, 144);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1145, 405);
            this.tabControl1.TabIndex = 0;
            
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.txtResults);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1137, 379);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Test Results";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // txtResults
            // 
            this.txtResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResults.Location = new System.Drawing.Point(3, 3);
            this.txtResults.Name = "txtResults";
            this.txtResults.Size = new System.Drawing.Size(1131, 373);
            this.txtResults.TabIndex = 9;
            this.txtResults.Text = "";
            // 
            // btnStartTests
            // 
            this.btnStartTests.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartTests.Location = new System.Drawing.Point(1045, 5);
            this.btnStartTests.Name = "btnStartTests";
            this.btnStartTests.Size = new System.Drawing.Size(96, 23);
            this.btnStartTests.TabIndex = 5;
            this.btnStartTests.Text = "Start Tests";
            this.btnStartTests.UseVisualStyleBackColor = true;
            this.btnStartTests.Click += new System.EventHandler(this.btnStartTests_Click);
            // 
            // txtFilename
            // 
            this.txtFilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilename.Location = new System.Drawing.Point(99, 59);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.Size = new System.Drawing.Size(471, 20);
            this.txtFilename.TabIndex = 6;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(1061, 554);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(566, 59);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(55, 20);
            this.btnBrowse.TabIndex = 7;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtK2Server
            // 
            this.txtK2Server.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtK2Server.Location = new System.Drawing.Point(99, 35);
            this.txtK2Server.Name = "txtK2Server";
            this.txtK2Server.Size = new System.Drawing.Size(109, 20);
            this.txtK2Server.TabIndex = 2;
            this.txtK2Server.Text = "NOT SET";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "K2 Server";
            // 
            // txtWorkspaceURL
            // 
            this.txtWorkspaceURL.Location = new System.Drawing.Point(99, 12);
            this.txtWorkspaceURL.Name = "txtWorkspaceURL";
            this.txtWorkspaceURL.Size = new System.Drawing.Size(363, 20);
            this.txtWorkspaceURL.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(46, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Test File";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Viewflow URL";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(468, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(372, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "i.e. http://dlx:81/workspace/Tasklistcontrol/ViewFlowMain.aspx?ProcessID=";
            // 
            // chkCreateViewFlowTabs
            // 
            this.chkCreateViewFlowTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkCreateViewFlowTabs.AutoSize = true;
            this.chkCreateViewFlowTabs.Checked = true;
            this.chkCreateViewFlowTabs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCreateViewFlowTabs.Location = new System.Drawing.Point(840, 1);
            this.chkCreateViewFlowTabs.Name = "chkCreateViewFlowTabs";
            this.chkCreateViewFlowTabs.Size = new System.Drawing.Size(202, 17);
            this.chkCreateViewFlowTabs.TabIndex = 3;
            this.chkCreateViewFlowTabs.Text = "show viewflow for each workflow test";
            this.chkCreateViewFlowTabs.UseVisualStyleBackColor = true;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolstripStatus,
            this.toolStripStatusTimer});
            this.statusStrip.Location = new System.Drawing.Point(0, 581);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1147, 22);
            this.statusStrip.TabIndex = 12;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolstripStatus
            // 
            this.toolstripStatus.Name = "toolstripStatus";
            this.toolstripStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusTimer
            // 
            this.toolStripStatusTimer.Name = "toolStripStatusTimer";
            this.toolStripStatusTimer.Size = new System.Drawing.Size(0, 17);
            // 
            // statusStripTimer
            // 
            this.statusStripTimer.Interval = 1000;
            this.statusStripTimer.Tick += new System.EventHandler(this.statusStripTimer_Tick);
            // 
            // btnDisplayTestContext
            // 
            this.btnDisplayTestContext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDisplayTestContext.Location = new System.Drawing.Point(1002, 69);
            this.btnDisplayTestContext.Name = "btnDisplayTestContext";
            this.btnDisplayTestContext.Size = new System.Drawing.Size(139, 31);
            this.btnDisplayTestContext.TabIndex = 8;
            this.btnDisplayTestContext.Text = "Display file in test context";
            this.btnDisplayTestContext.UseVisualStyleBackColor = true;
            this.btnDisplayTestContext.Click += new System.EventHandler(this.btnDisplayTestContext_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(2, 115);
            this.lblStatus.MaximumSize = new System.Drawing.Size(0, 26);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(156, 26);
            this.lblStatus.TabIndex = 14;
            this.lblStatus.Text = "No Tests Run";
            // 
            // chkDebug
            // 
            this.chkDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkDebug.AutoSize = true;
            this.chkDebug.Location = new System.Drawing.Point(840, 18);
            this.chkDebug.Name = "chkDebug";
            this.chkDebug.Size = new System.Drawing.Size(137, 17);
            this.chkDebug.TabIndex = 4;
            this.chkDebug.Text = "Show debug Messages";
            this.chkDebug.UseVisualStyleBackColor = true;
            // 
            // btnStopTests
            // 
            this.btnStopTests.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStopTests.Location = new System.Drawing.Point(1045, 38);
            this.btnStopTests.Name = "btnStopTests";
            this.btnStopTests.Size = new System.Drawing.Size(96, 23);
            this.btnStopTests.TabIndex = 15;
            this.btnStopTests.Text = "Stop Tests";
            this.btnStopTests.UseVisualStyleBackColor = true;
            this.btnStopTests.Click += new System.EventHandler(this.btnStopTests_Click);
            // 
            // chkRunCleanUp
            // 
            this.chkRunCleanUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkRunCleanUp.AutoSize = true;
            this.chkRunCleanUp.Checked = true;
            this.chkRunCleanUp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRunCleanUp.Location = new System.Drawing.Point(840, 35);
            this.chkRunCleanUp.Name = "chkRunCleanUp";
            this.chkRunCleanUp.Size = new System.Drawing.Size(93, 17);
            this.chkRunCleanUp.TabIndex = 16;
            this.chkRunCleanUp.Tag = "Cleanup";
            this.chkRunCleanUp.Text = "Run Clean Up";
            this.chkRunCleanUp.UseVisualStyleBackColor = true;
            // 
            // chkRunSetUp
            // 
            this.chkRunSetUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkRunSetUp.AutoSize = true;
            this.chkRunSetUp.Checked = true;
            this.chkRunSetUp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRunSetUp.Location = new System.Drawing.Point(840, 52);
            this.chkRunSetUp.Name = "chkRunSetUp";
            this.chkRunSetUp.Size = new System.Drawing.Size(82, 17);
            this.chkRunSetUp.TabIndex = 17;
            this.chkRunSetUp.Tag = "Setup";
            this.chkRunSetUp.Text = "Run Set Up";
            this.chkRunSetUp.UseVisualStyleBackColor = true;
            // 
            // chkRunAsserts
            // 
            this.chkRunAsserts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkRunAsserts.AutoSize = true;
            this.chkRunAsserts.Checked = true;
            this.chkRunAsserts.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRunAsserts.Location = new System.Drawing.Point(840, 69);
            this.chkRunAsserts.Name = "chkRunAsserts";
            this.chkRunAsserts.Size = new System.Drawing.Size(83, 17);
            this.chkRunAsserts.TabIndex = 18;
            this.chkRunAsserts.Tag = "Assert";
            this.chkRunAsserts.Text = "Run Asserts";
            this.chkRunAsserts.UseVisualStyleBackColor = true;
            // 
            // chkSaveTestStructure
            // 
            this.chkSaveTestStructure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSaveTestStructure.AutoSize = true;
            this.chkSaveTestStructure.Location = new System.Drawing.Point(840, 92);
            this.chkSaveTestStructure.Name = "chkSaveTestStructure";
            this.chkSaveTestStructure.Size = new System.Drawing.Size(121, 17);
            this.chkSaveTestStructure.TabIndex = 19;
            this.chkSaveTestStructure.Tag = "";
            this.chkSaveTestStructure.Text = "Save Test Structure";
            this.chkSaveTestStructure.UseVisualStyleBackColor = true;
            // 
            // AutoCompleteTasks
            // 
            this.AutoCompleteTasks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AutoCompleteTasks.AutoSize = true;
            this.AutoCompleteTasks.Checked = true;
            this.AutoCompleteTasks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AutoCompleteTasks.Location = new System.Drawing.Point(840, 106);
            this.AutoCompleteTasks.Name = "AutoCompleteTasks";
            this.AutoCompleteTasks.Size = new System.Drawing.Size(153, 17);
            this.AutoCompleteTasks.TabIndex = 20;
            this.AutoCompleteTasks.Tag = "";
            this.AutoCompleteTasks.Text = "Automatically Action Tasks";
            this.AutoCompleteTasks.UseVisualStyleBackColor = true;
            this.AutoCompleteTasks.CheckedChanged += new System.EventHandler(this.AutoCompleteTasks_CheckedChanged);
            // 
            // WorkflowTestUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1147, 603);
            this.Controls.Add(this.AutoCompleteTasks);
            this.Controls.Add(this.chkSaveTestStructure);
            this.Controls.Add(this.chkRunAsserts);
            this.Controls.Add(this.chkRunSetUp);
            this.Controls.Add(this.chkRunCleanUp);
            this.Controls.Add(this.btnStopTests);
            this.Controls.Add(this.chkDebug);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnDisplayTestContext);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.chkCreateViewFlowTabs);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtWorkspaceURL);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtK2Server);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.txtFilename);
            this.Controls.Add(this.btnStartTests);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WorkflowTestUI";
            this.Text = "K2 Workfow Testing";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.RichTextBox txtResults;
        private System.Windows.Forms.Button btnStartTests;
        private System.Windows.Forms.OpenFileDialog ofdXMLFile;
        private System.Windows.Forms.TextBox txtFilename;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtK2Server;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtWorkspaceURL;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkCreateViewFlowTabs;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolstripStatus;
        private System.Windows.Forms.Timer statusStripTimer;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusTimer;
        private System.Windows.Forms.Button btnDisplayTestContext;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.CheckBox chkDebug;
        private System.Windows.Forms.Button btnStopTests;
        private System.Windows.Forms.CheckBox chkRunCleanUp;
        private System.Windows.Forms.CheckBox chkRunSetUp;
        private System.Windows.Forms.CheckBox chkRunAsserts;
        private System.Windows.Forms.CheckBox chkSaveTestStructure;
        private System.Windows.Forms.CheckBox AutoCompleteTasks;
    }
}

