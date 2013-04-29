using System;
using System.Xml;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Assemblies;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using K2Field.Helpers.Core.Code;
using System.IO;
using System.Threading;
using SourceCode.Hosting.Client.BaseAPI;
using SourceCode.Workflow.Client;
using K2Field.Utilities.Testing.WF.Core;

namespace K2Field.Utilities.Testing.WF.UI
{
    public partial class WorkflowTestUI : Form
    {
        private K2Field.Utilities.Testing.WF.Core.TestingHelper K2TestHelper;
        private string K2Server = string.Empty;
        private string ViewFlowURL = string.Empty;
        private string TestFile = string.Empty;
        private bool CreateViewFlowTabs = false;
        private string xmlFilesParentDirectory = string.Empty;

        public WorkflowTestUI()
        {
            InitializeComponent();
            LoadConfigFile();
           
        }

        private void configureTester()
        {
            
            K2TestHelper = new TestingHelper();
            K2TestHelper.SendTestResult += new SendTestResultArgs(K2TestHelper_SendTestResult);
        }


        private void SaveConfigFile()
        {

           
            Properties.Settings.Default.K2Server = K2Server;
            Properties.Settings.Default.ViewFlowURL = ViewFlowURL;
            Properties.Settings.Default.TestFile = this.txtFilename.Text;
            Properties.Settings.Default.Save();
        }

        private void LoadConfigFile()
        {
            
            this.txtK2Server.Text = Properties.Settings.Default.K2Server;
            this.txtWorkspaceURL.Text = Properties.Settings.Default.ViewFlowURL;
            this.txtFilename.Text = Properties.Settings.Default.TestFile;

        
        }

        private void btnStartTests_Click(object sender, EventArgs e)
        {
            this.txtResults.Text = string.Empty;

            K2Server = this.txtK2Server.Text;
            ViewFlowURL = this.txtWorkspaceURL.Text;
            CreateViewFlowTabs = this.chkCreateViewFlowTabs.Checked;
            //Save Current setings.
            SaveConfigFile();

            //remove all tabs (viewflows) except results tab
            for (int i = 1; i < tabControl1.TabPages.Count; i++)
            {
                tabControl1.TabPages.RemoveAt(i);
                i--;
            }

            span = new TimeSpan();
            startdate = DateTime.Now;
            this.statusStripTimer.Enabled = true;
            this.xmlFilesParentDirectory = GetDirectory(this.txtFilename.Text);

            this.lblStatus.ForeColor = Color.Black;

            configureTester();
            System.Threading.Thread nt = new System.Threading.Thread(new System.Threading.ThreadStart(StartTest));
            nt.Start();
            if (K2TestHelper != null)
            {
                K2TestHelper.Dispose();
            }
        }

        private string GetDirectory(string fullFileName)
        {
            var direc = new DirectoryInfo(fullFileName);
            return direc.Parent.FullName;
        }

        private void StartTest()
        {

            K2TestHelper.XmlFilesParentDirectory = this.xmlFilesParentDirectory;
            K2TestHelper.TestsTypesToRun = new StringBuilder();
            foreach (var checkBox in this.Controls.OfType<CheckBox>().Where(c => c.Tag != null))
            {
                //for every checkbox with a non blank tag
                if (checkBox.Checked)
                {
                    K2TestHelper.TestsTypesToRun.Append(checkBox.Tag);
                }
            }
            K2TestHelper.RecordStructure = this.chkSaveTestStructure.Checked;
            K2TestHelper.K2Server = K2Server;
            K2TestHelper.LoadXMLTestFile(this.txtFilename.Text);
            K2TestHelper.ProceedWithTest = true;
            
            K2TestHelper.StartTest(K2Server);
        }

        void K2TestHelper_SendTestResult(object sender, K2Field.Utilities.Testing.WF.Core.TestResultArgs e)
        {
            //now we have the results from the test, lets display them
            bool isDisplayed = false;
            if (e.ResultStage == TestResultStage.NewProcessTestStarting && !isDisplayed)
            {
                AddResult("\r");
                AddResult(e.ResultStage.ToString() + "  " + e.ExtraDetails);
                isDisplayed = true;
            }

            if (e.ResultStage == TestResultStage.ProcessStarted && !isDisplayed)
            {
                if (this.chkCreateViewFlowTabs.Checked) 
                {
                    AddResult("\t" + e.ResultStage.ToString() + "  " + e.ExtraDetails);
                    Thread.Sleep(2000);
                    AddNewTab(e.CurrentProcess.Description, e.CurrentProcess.ProcessInstanceID);
                }
                isDisplayed = true;
            }


            if (e.ResultStage == TestResultStage.ActivityActioned && !isDisplayed)
            {
                AddResult("\t" + e.ResultStage.ToString() + "  " + e.ExtraDetails);
                isDisplayed = true;
            }

            if (e.ResultStage.ToString().Contains("Activity") && !isDisplayed)
            {
                AddResult("\t" + e.ResultStage.ToString() + "  " + e.ExtraDetails);
                isDisplayed = true;
            }

            if (e.ResultStage == TestResultStage.TestEngineEnded && !isDisplayed)
            {
                this.statusStripTimer.Enabled = false;
                isDisplayed = true;
                ReportResults(K2TestHelper.GlobalProcesses);
            }

            if (e.ResultStage == TestResultStage.DebugMessage && chkDebug.Checked)
            {
                isDisplayed = false;
            }
            else if (e.ResultStage == TestResultStage.DebugMessage)
            {
                isDisplayed = true;
            }

            if (!isDisplayed)
            {
                AddResult(e.ResultStage.ToString() + "  " + e.ExtraDetails);
            }
        }

        private void ReportResults(List<Core.Process> Processes)
        {
            AddResult("\n");
            AddResult("-------------------------------Results-----------------------------");
            bool testFailed = false;
            foreach (var p in Processes)
            {
                if (p.ProcessType == ProcessType.assert)
                {
                    if (p.ProcessHasUnexpectedErrors)
                    {
                        testFailed = true;
                    }
                    string result = (!p.ProcessHasUnexpectedErrors) ? "Passed" : "Failed - " + p.ActivityExecutionError;
                    AddResult("------------------------------------------------------------------------------");
                    AddResult(string.Format("{0} ", " Description : " + p.Description));
                    AddResult(string.Format("{0} ", " Status : " + result));
                    AddResult(string.Format("{0} ", " Process Status : " + p.ProcessStatus));
                    if (p.ProcessHasUnexpectedErrors) AddResult(string.Format("{0} ", " Error : " + p.ProcessError));
                }
            }
            if (testFailed)
            {
                AddResult("Error: At least one test failed");
            }
            else
            {
                AddResult("All tests succeeded");
            }
            WriteXMLResults(Processes);
        }

        private void WriteXMLResults(List<Process> Processes)
        {

            System.IO.DirectoryInfo dir = new DirectoryInfo("TestResults");
            if (!dir.Exists) dir.Create();

            XmlTextWriter writer = new XmlTextWriter(string.Format("TestResults\\{0}{1}{2}_{3}.{4}.TestResults.xml", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute), null);
            writer.WriteStartElement("Results");
            writer.WriteAttributeString("K2Server", K2Server);
            writer.WriteAttributeString("TestRunBy", System.Security.Principal.WindowsIdentity.GetCurrent().Name);

            foreach (var p in Processes)
            {
                string result = (!p.ProcessHasUnexpectedErrors) ? "Passed" : "Failed";

                writer.WriteStartElement("Result");
                writer.WriteElementString("StartDate", p.TestStartDate.ToString());
                writer.WriteElementString("EndDate", p.TestEndDate.ToString());
                writer.WriteElementString("ProcessName", p.ProcessName);
                writer.WriteElementString("Description", p.Description);
                writer.WriteElementString("TestResult", result);
                writer.WriteElementString("ProcessStatus", p.ProcessStatus);
                writer.WriteElementString("ActivityExecutionError", p.ActivityExecutionError);
                writer.WriteElementString("ProcessError", p.ProcessError);
                writer.WriteElementString("ProcInstID", p.ProcessInstanceID.ToString());

                //write out activity data
                foreach (var act in p.Activities)
                {
                    writer.WriteStartElement("ActivityResult");
                    writer.WriteElementString("Name", act.Name);
                    writer.WriteElementString("ProcessName", act.ProcessName);
                    writer.WriteElementString("Action", act.Action);
                    writer.WriteElementString("TestStatus", act.TestStatus);
                    writer.WriteElementString("MaxRetryCount", act.MaxRetryCount.ToString());
                    writer.WriteElementString("CountOfRetries", act.CountOfRetries.ToString());
                    writer.WriteElementString("ActivityExecutionError", act.ActivityExecutionError);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.Close();
        }

        public delegate void mydelegate();

        private string newstring;

        private void WriteResult()
        {
            System.Threading.Thread.Sleep(100);
            this.txtResults.Text += Environment.NewLine + DateTime.Now.ToString() + "\t" + newstring;
            if (newstring.ToLower().Contains("error:"))
            {
                this.lblStatus.ForeColor = Color.Red;
            }
            this.lblStatus.Text = newstring;
        }

        private void AddResult(string txt)
        {
            newstring = txt;
            mydelegate inst = new mydelegate(WriteResult);
            this.Invoke(inst);
        }

        private string TabHeaderText = string.Empty;
        private int TabProcInst = 0;

        private void DrawURLTab()
        {
            TabPage tab = new TabPage();
            tab.Text = TabHeaderText;
            System.Windows.Forms.WebBrowser browser = new WebBrowser();

            string url = ViewFlowURL + TabProcInst.ToString();

            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                browser.Url = new Uri(url);
                browser.Dock = DockStyle.Fill;
                tab.Controls.Add(browser);
                tabControl1.TabPages.Add(tab);
                AddResult("\t" + url);
            }
        }

        private void AddNewTab(string headertext, int procinst)
        {
            TabHeaderText = headertext;
            TabProcInst = procinst;

            mydelegate inst = new mydelegate(DrawURLTab);
            this.Invoke(inst);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {

            OpenFileDialog dia = new OpenFileDialog();
            dia.Filter = "xml files (*.xml)|*.xml";

            if (dia.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.txtFilename.Text = dia.FileName;
            }
        }


        TimeSpan span = new TimeSpan();
        DateTime startdate = DateTime.Now;

        private void statusStripTimer_Tick(object sender, EventArgs e)
        {
            span = DateTime.Now.Subtract(startdate);
            this.toolStripStatusTimer.Text = string.Format("Test time - {0:00}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds); 
        }

        private void btnDisplayTestContext_Click(object sender, EventArgs e)
        {
            this.txtResults.Text = string.Empty;
            this.lblStatus.ForeColor = Color.Red;
            K2TestHelper.LoadXMLTestFile(this.txtFilename.Text);

            //Noe write out process info.

            foreach (Process proc in K2TestHelper.GlobalProcesses)
            {
                this.txtResults.Text += "\n\n**New Test**";
                this.txtResults.Text += "\nTest Description : " + proc.Description;

                foreach(Activity act in proc.Activities)
                {
                    this.txtResults.Text += "\n\n**New Activity**";

                    this.txtResults.Text += "\nActivity ProcessName : " + act.ProcessName;
                    this.txtResults.Text += "\nActivity Name : " + act.Name;
                    this.txtResults.Text += "\nActivity Action : " + act.Action;
                    this.txtResults.Text += "\nActivity MaxRetryCount : " + act.MaxRetryCount;
                    this.txtResults.Text += "\nActivity RetryInSeconds : " + act.RetryInSeconds;

                    foreach (var df in act.DataFields)
                    {
                        this.txtResults.Text += string.Format("\nActivity DataField : Name '{0}', Value : '{1}'", df.Value, df.Key);
                    }

                    if(act.PreMethodCall.NeedToInvoke)
                    {
                        this.txtResults.Text += "\nActivity PreMethodCall Assembly : " + act.PreMethodCall.Assembly;
                        this.txtResults.Text += "\nActivity PreMethodCall Class : " + act.PreMethodCall.Class;
                        this.txtResults.Text += "\nActivity PreMethodCall Method : " + act.PreMethodCall.Method;

                        foreach (string s in act.PreMethodCall.Parameters)
                        {
                            this.txtResults.Text += "\nActivity PreMethodCall Method Parameters: " + s;
                        }
                    }

                    if (act.PostMethodCall.NeedToInvoke)
                    {
                        this.txtResults.Text += "\nActivity PostMethodCall Assembly : " + act.PostMethodCall.Assembly;
                        this.txtResults.Text += "\nActivity PostMethodCall Class : " + act.PostMethodCall.Class;
                        this.txtResults.Text += "\nActivity PostMethodCall Method : " + act.PostMethodCall.Method;

                        foreach (string s in act.PostMethodCall.Parameters)
                        {
                            this.txtResults.Text += "\nActivity PostMethodCall Method Parameters: " + s;
                        }
                    }

                }
            }
        }

        private void btnStopTests_Click(object sender, EventArgs e)
        {
            K2TestHelper.ProceedWithTest = false;
        }


    }
}
