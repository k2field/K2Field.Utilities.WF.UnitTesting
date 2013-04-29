using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SourceCode.Hosting.Client.BaseAPI;
using SourceCode.Workflow.Client;
using System.Configuration;
using K2Field.Helpers.Core;
using System.IO;
using System.Xml;
using System.Threading;
using SourceCode.Hosting.Client;
using SourceCode.Workflow;
using SourceCode.Workflow.Management;
using SourceCode.Workflow.Management.Criteria;

namespace K2Field.Utilities.Testing.WF.Core
{
    #region other classes and enums etc

    public class TestResultArgs : EventArgs
    {
        public Core.Process CurrentProcess { get; set; }
        public Core.Activity CurrentActivity { get; set; }
        public Core.TestResultStage ResultStage { get; set; }
        public string ExtraDetails { get; set; }

        public TestResultArgs(Process process, Activity activity, TestResultStage resultStage)
        {
            CurrentProcess = process;
            CurrentActivity = activity;
            ResultStage = resultStage;
            ExtraDetails = string.Empty;
        }

        public TestResultArgs(Process process, Activity activity, TestResultStage resultStage, string extraDetails)
        {
            CurrentProcess = process;
            CurrentActivity = activity;
            ResultStage = resultStage;
            ExtraDetails = extraDetails;
        }
    }

    public delegate void SendTestResultArgs(object sender, TestResultArgs e);

    public enum TestResultStage
    {
        TestEngineStarted,
        XMLLoaded,
        NewProcessTestStarting,
        ProcessStarting,
        ProcessStarted,
        ActivityPreMethodExecuted,
        ActivityPostMethodExecuted,
        ActivityActioned,
        ActivityNotFoundRetrying,
        ActivityNotFoundGivingUp,
        NewProcessTestFinished,
        TestEngineEnded,
        ActivityExecutionError,
        FatalError,
        DebugMessage
    }

    enum MethodType
    {
        PreMethod,
        PostMethod
    }

    #endregion

    public class TestingHelper: IDisposable
    {
        #region events

        public event SendTestResultArgs SendTestResult;

        #endregion

        #region public and private variables

        public string XmlFilesParentDirectory { get; set; }
        public List<Process> GlobalProcesses = new List<Process>();
        private K2Field.Helpers.Core.K2Helper k2helper;
        private Dictionary<string, string> dataFieldDictionary = new Dictionary<string, string>();
        private List<string> uniqueProcessIdList = new List<string>();
        private bool disposed = false;
        private bool XMLLoaded = false;
        public string K2Server = string.Empty;

        //TODO: these should not be class variables
        private ErrorLog lastError = null;
        private Process currentProcess = null;
        private Activity currentActivity = null;
        public bool ProceedWithTest { get; set; }
        public StringBuilder TestsTypesToRun { get; set; }

        public bool RecordStructure { get; set; }

        #endregion

        public TestingHelper()
        {
            this.TestsTypesToRun = new StringBuilder();
            this.XmlFilesParentDirectory = string.Empty;
        }


        #region LoadXML methods





        public bool LoadXMLTestFile(string xmlfile)
        {
            XMLLoaded = false;
            List<Process> localProcesses = new List<Process>();
            GlobalProcesses.Clear();
            SendResult(new TestResultArgs(null, null, TestResultStage.XMLLoaded, "Loading XML test script"));
            localProcesses = constructProcessesFromXMLFile(xmlfile, true, true, string.Empty);
            if (localProcesses != null)
            {
                XMLLoaded = true;
            }

            return XMLLoaded;
        }

        private void recordFileStructure(string childFilename, string parentFilename)
        {
            if (this.RecordStructure)
            {
                childFilename = childFilename.Replace(this.XmlFilesParentDirectory + "\\", string.Empty);
                parentFilename = parentFilename.Replace(this.XmlFilesParentDirectory + "\\", string.Empty);
                Console.WriteLine(childFilename);
                Console.WriteLine(parentFilename);
                var inputFields = new Dictionary<string, object>();
                inputFields.Add("Filename", childFilename);

                using (K2Helper helper = new K2Helper(K2Server))
                {
                    helper.SmartObjectClient().SMOClient.CallSmartObjectExecuteMethod("Filename", "Save", inputFields);
                    if (parentFilename.Length != 0)
                    {
                        inputFields = new Dictionary<string, object>();

                        inputFields.Add("ParentFilename", parentFilename);
                        inputFields.Add("ChildFilename", childFilename);

                        helper.SmartObjectClient().SMOClient.CallSmartObjectExecuteMethod("FilenameRelationship", "Save", inputFields);
                    }
                }
            }
        }


        private List<Process> constructProcessesFromXMLFile(string xmlfile, bool isParent, bool IsXmlRootDoc, string parentFilename)
        {
            List<Process> retVal = null;
            try
            {
                //load activities from the file instead
                XmlDocument ProcessesXmldoc = null;
               
                string processesXMLFullFileName = string.Empty;
                if (isParent)
                {
                    processesXMLFullFileName = xmlfile;
                }
                else
                {
                    processesXMLFullFileName = string.Format("{0}\\{1}", this.XmlFilesParentDirectory, xmlfile);
                }
                try
                {
                    ProcessesXmldoc = LoadXMLDoc(processesXMLFullFileName);
                    recordFileStructure(processesXMLFullFileName, parentFilename);
                    retVal = ConstructProcessesFromXMLDoc(ProcessesXmldoc, IsXmlRootDoc, processesXMLFullFileName);
                    return retVal;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Cannot load file '{0}' specified in fileName attribute of processesNode exception:{1}{2}", processesXMLFullFileName, ex.Message, ex.StackTrace), ex);
                }

            }
            catch (Exception ex)
            {
                if (ex.IsFatal())
                {
                    throw;
                }
                SendResult(new TestResultArgs(null, null, TestResultStage.FatalError, string.Format("Error loading as XML document : {0}{1}", ex.ToString(), ex.StackTrace)));
                return null;
            }
        }

        private void ConstructProcessesFromXMLNodeList(XmlNodeList xmlnodelist, List<Process> processes, string xmlRootDir, string parentFilename)
        {
            //List<Process> newProcesses = new List<Process>();
            foreach (XmlNode processNode in xmlnodelist)
            {
                string processTypeString = XmlHelper.GetAttributeValue(processNode, "type", "Assert").ToLower();
                ProcessType processType = (ProcessType)Enum.Parse(typeof(ProcessType), processTypeString);
                string processName = XmlHelper.GetAttributeValue(processNode, "processName");
                string processUniqueID = XmlHelper.GetAttributeValue(processNode, "uniqueID", "NoUniqueID");
                string fileName = XmlHelper.GetAttributeValue(processNode, "fileName");

                bool runTest = bool.Parse(XmlHelper.GetAttributeValue(processNode, "TestEnabled", "True"));
                if (!runTest) continue;   // Skip the remainder of this iteration.;

                if (!Regex.Match(this.TestsTypesToRun.ToString(), processTypeString, RegexOptions.IgnoreCase).Success)
                {
                    //These are not the process types you are looking for. #jedimindtrick
                    continue;
                }

                if (string.IsNullOrEmpty(fileName))
                {//This is an actual process to test and not a pointer to another file
                    if (processUniqueID == "NoUniqueID" || !uniqueProcessIdList.Contains(processUniqueID))
                    {
                        uniqueProcessIdList.Add(processUniqueID);
                        Process newProcess = new Process();
                        if (string.IsNullOrEmpty(processName))
                        {//old style Process Name is a node of the child nodes
                            newProcess = ConstructProcessFromXmlNodeListOldStyle(processNode, xmlRootDir, parentFilename);
                        }
                        else
                        {
                            newProcess = ConstructProcessFromXmlNodeListNewStyle(processNode, xmlRootDir, parentFilename);
                        }
                        newProcess.ProcessType = processType;
                        processes.Add(newProcess);
                    }
                    else
                    {
                        SendResult(new TestResultArgs(null, null, TestResultStage.DebugMessage, string.Format("Constructed Process already exists in test file, so skipping it {0} - {1}", processName, processUniqueID)));
                    }
                }
            }
        }

        /// <summary>
        /// Master Method Constructs Processes and Activities heirarchy from Documents. Only called from one place, 
        /// but can be the master test file OR sub file(s) the master points to.
        /// </summary>
        /// <param name="xmldoc">the xml doc containing the test file xml</param>
        /// <param name="IsXmlRootDoc">Is this the master file or a sub file. If it is a sub file, where is the master file in relation to it?</param>
        /// <returns></returns>
        private List<Process> ConstructProcessesFromXMLDoc(XmlDocument xmldoc, bool IsXmlRootDoc, string docFilename)
        {
            List<Process> Processes = new List<Process>();
            XmlNode parentNode = xmldoc.SelectSingleNode("Processes");
            string xmlRootDir;
            if (IsXmlRootDoc)
            {
                xmlRootDir = XmlHelper.GetAttributeValue(parentNode, "rootDir", "");
            }
            else
            {
                xmlRootDir = string.Empty;
            }
            XmlNodeList xmlnodelistProceesses = xmldoc.GetElementsByTagName(@"Processes");
            foreach (XmlNode processesNode in xmlnodelistProceesses)
            {
                string processesFileName = XmlHelper.GetAttributeValue(processesNode, "fileName", string.Empty);

                bool runTest = bool.Parse(XmlHelper.GetAttributeValue(processesNode, "TestEnabled", "True"));
                if (runTest)
                {
                    if (processesFileName.Length > 0)
                    {
                        String processesFullFileName = string.Format("{0}{1}", xmlRootDir, processesFileName);

                        List<Process> newProcs = constructProcessesFromXMLFile(processesFullFileName, false, false, docFilename);
                        if (newProcs == null)
                        {
                            return null;
                        }
                        Processes.AddRange(newProcs);
                    }
                }
            }

            XmlElement elm = xmldoc.DocumentElement;
            XmlNodeList xmlnodelist = elm.ChildNodes;  //xmldoc.SelectNodes(@"Processes/Process");
            ConstructProcessesFromXMLNodeList(xmlnodelist, GlobalProcesses, xmlRootDir, docFilename);
            return Processes;
        }

        private Process ConstructProcessFromXmlNodeListOldStyle(XmlNode processNode, string xmlRootDir, string parentFilename)
        {
            Core.Process newprocess = new Core.Process();
            //TODO another sub method to construct the process

            return ConstructProcessFromXmlNodeListOldStyle(processNode, newprocess, xmlRootDir, parentFilename);
        }

        private Process ConstructProcessFromXmlNodeListOldStyle(XmlNode processNode, Process newprocess, string xmlRootDir, string parentFilename)
        {
            newprocess.Description = XmlHelper.GetAttributeValue(processNode, "description", XmlHelper.NameCaseSensitive.No);
            //Now lets get all Activities listed in the XML
            newprocess.Activities.AddRange(ConstructActivitiesFromXMLNodeList(processNode.ChildNodes, xmlRootDir, newprocess, parentFilename));
            return newprocess;
        }

        private Process ConstructProcessFromXmlNodeListNewStyle(XmlNode processNode, string xmlRootDir, string parentFilename)
        {
            Core.Process newprocess = new Core.Process();

            Activity startActivity = ConstructStartActivityFromXMLNode(processNode);
            newprocess.Activities.Add(startActivity);
            return ConstructProcessFromXmlNodeListOldStyle(processNode, newprocess, xmlRootDir,  parentFilename);
        }

        private List<Activity> ConstructActivitiesFromXMLNodeList(XmlNodeList processChildrenNodeList, string xmlRootDir, Process newProcess, string parentFilename)
        {
            //Old format contained Activities node for every activity
            //new format - not every nodes may be Activity node
            //for each activity in the process
            List<Activity> activities = new List<Activity>();
            foreach (XmlNode activityNode in processChildrenNodeList)
            {
                if (activityNode.Name == "Activities")
                {
                    string activitiesFileName = XmlHelper.GetAttributeValue(activityNode, "fileName");
                    //if Node contains a filename attribute and it is populated 
                    if (string.IsNullOrEmpty(activitiesFileName))
                    {
                        List<Activity> newactivities = ConstructActivityFromXMLNode(activityNode, xmlRootDir, parentFilename);
                        activities.AddRange(newactivities);
                    }
                    else
                    {
                        List<Activity> subFileActivities = ConstructActivitiesFromXMLFile(activitiesFileName, xmlRootDir, parentFilename);
                        activities.AddRange(subFileActivities);
                    }
                }
                else if (activityNode.Name == "Activity")
                {

                    //We have found that this activity is of type "StartProcess"
                    XmlNodeList xmlnodelistProceesses = activityNode.SelectNodes(@"Process");
                    if (xmlnodelistProceesses != null && xmlnodelistProceesses.Count > 0)
                    {
                        Activity activityWithSubProcess = new Activity();
                        
                        activityWithSubProcess.ErrorExpected = bool.Parse(XmlHelper.GetAttributeValue(activityNode, "exceptionExpected", "False"));
                        activityWithSubProcess.Name = "SubProcess";
                        activityWithSubProcess.SubProcesses = new List<Process>();
                        ConstructProcessesFromXMLNodeList(xmlnodelistProceesses, activityWithSubProcess.SubProcesses, xmlRootDir, parentFilename);
                        foreach (Process proc in activityWithSubProcess.SubProcesses)
                        {
                            proc.RootProcess = newProcess;
                        }
                        activities.Add(activityWithSubProcess);
                    }
                    else
                    {
                        List<Activity> newactivities = ConstructActivityFromXMLNode(activityNode, xmlRootDir, parentFilename);
                        activities.AddRange(newactivities);
                    }
                }
                else
                {
                    //could be DataFields, PreMethodCall etc.
                }
            }
            return activities;
        }

        private Activity ConstructStartActivityFromXMLNode(XmlNode processNode)
        {
            Activity newactivity = new Activity();
            newactivity.Name = "Start";

            string processName = XmlHelper.GetAttributeValue(processNode, "processName");

            newactivity.ErrorExpected = bool.Parse(XmlHelper.GetAttributeValue(processNode, "exceptionExpected", "False"));

            newactivity.ProcessName = processName;


            newactivity.MaxRetryCount = 0;
            newactivity.RetryInSeconds = 0;
            //Get the pre and post methods for this activity.
            newactivity.PreMethodCall = GetMethodCalls(processNode, "PreMethodCall");
            newactivity.PostMethodCall = GetMethodCalls(processNode, "PostMethodCall");
            //Get/Set data fields.
            XmlElement dataFieldsElement = XmlHelper.GetElement(processNode, "DataFields");
            foreach (XmlNode datafieldNode in dataFieldsElement.ChildNodes)
            {

                newactivity.DataFields.Add(XmlHelper.GetAttributeValue(datafieldNode,"Name", XmlHelper.NameCaseSensitive.No),ConstructDataField(datafieldNode));
            }
            return newactivity;
        }

        private CoreDataField ConstructDataField(XmlNode datafieldNode)
        {
            CoreDataField dataField = new CoreDataField();
            string strdatafieldType = XmlHelper.GetAttributeValue(datafieldNode, "type");
            if (string.IsNullOrEmpty(strdatafieldType))
            {
                strdatafieldType = Enum.GetName(typeof(CoreDataFieldType), CoreDataFieldType.Process);
            }
            CoreDataFieldType datafieldType = (CoreDataFieldType)Enum.Parse(typeof(CoreDataFieldType), strdatafieldType);
            dataField.Name = XmlHelper.GetAttributeValue(datafieldNode, "name", XmlHelper.NameCaseSensitive.No);
            dataField.Check = XmlHelper.GetAttributeValue(datafieldNode, "action", "set");
            dataField.Type = datafieldType;
            dataField.Value = datafieldNode.InnerText;
            return dataField;
        }

        private List<Activity> ConstructActivityFromXMLNode(XmlNode activityNode, string xmlRootDir, string parentFilename)
        {
            if (activityNode == null)
            {
                throw new NullReferenceException("activityNode is null");
            }
            string fileName = XmlHelper.GetAttributeValue(activityNode, "fileName", string.Empty);
            
            if (!string.IsNullOrEmpty(fileName))
            {
                return ConstructActivitiesFromXMLFile(fileName, xmlRootDir, parentFilename);
            }


            Activity newactivity = new Activity();
            newactivity.ErrorExpected = bool.Parse(XmlHelper.GetAttributeValue(activityNode, "exceptionExpected", "False"));
            string processName = XmlHelper.GetChildNodeInnerText(activityNode, "ProcessName", XmlHelper.ReturnCanBeEmpty.No);
            newactivity.ProcessName = processName;
            if (activityNode.Name == "Activities")
            {
                newactivity.Name = XmlHelper.GetChildNodeInnerText(activityNode, "Activity", XmlHelper.ReturnCanBeEmpty.No);
            }
            else if (activityNode.Name == "Activity")
            {
                newactivity.Name = XmlHelper.GetAttributeValue(activityNode, "name");
            }
            else
            {
                throw new InvalidOperationException(string.Format("Invalid Node Passed. 'Activities' or 'Activity' expected but got '{0}", activityNode.Name));
            }
            newactivity.Action = XmlHelper.GetChildNodeInnerText(activityNode, "Action", XmlHelper.ReturnCanBeEmpty.Yes);
            newactivity.MaxRetryCount = int.Parse(XmlHelper.GetChildNodeInnerText(activityNode,"RetryCount", XmlHelper.ReturnCanBeEmpty.No));
            newactivity.RetryInSeconds = int.Parse(XmlHelper.GetChildNodeInnerText(activityNode,"RetryInSeconds", XmlHelper.ReturnCanBeEmpty.No));
            //Get the pre and post methods for this activity.
            newactivity.PreMethodCall = GetMethodCalls(activityNode, "PreMethodCall");
            newactivity.PostMethodCall = GetMethodCalls(activityNode, "PostMethodCall");
            //Get/Set data fields.
            if (activityNode["DataFields"] != null)
            {
                foreach (XmlNode datafield in activityNode["DataFields"].ChildNodes)
                {
                    newactivity.DataFields.Add(XmlHelper.GetAttributeValue(datafield,"name",XmlHelper.NameCaseSensitive.No) , ConstructDataField(datafield));
                }
            }
            else
            {
                throw new NullReferenceException(string.Format("Constructed Activity has no datafields {0} - {1}", processName, newactivity.Name));
//                SendResult(new TestResultArgs(null, null, TestResultStage.DebugMessage, string.Format("Constructed Activity {0} - {1}", processName, newactivity.Name)));
            }
            SendResult(new TestResultArgs(null, null, TestResultStage.DebugMessage, string.Format("Constructed Activity {0} - {1}", processName, newactivity.Name)));

            //Return a list of only 1 activity
            List<Activity> retActivity = new List<Activity>();
            retActivity.Add(newactivity);
            return retActivity;
        }

        private List<Activity> ConstructActivitiesFromXMLFile(string activitiesXMLFileName, string xmlRootDir, string parentFilename)
        {
            SendResult(new TestResultArgs(null, null, TestResultStage.DebugMessage, string.Format("Attempting to construct Activities from file {0}", activitiesXMLFileName)));

             //New format contains 1 Activities node with multiple child activity nodes
            List<Activity> activities = new List<Activity>();
            //load activities from the file instead
            XmlDocument ActivitiesXmldoc = null;

            ////////string prevLevelIndicator = "..\\";
            ////////string previousDirectoryLevels = string.Empty;
            ////////if (this.XmlFileChildDirectoryLevel > 0)
            ////////{
            ////////    previousDirectoryLevels = new StringBuilder().Insert(0, prevLevelIndicator, this.XmlFileChildDirectoryLevel).ToString();
            ////////activitiesXMLFileName = activitiesXMLFileName.Replace(previousDirectoryLevels, string.Empty);
            ////////}
            
            if (activitiesXMLFileName.StartsWith("~\\"))
            {
                activitiesXMLFileName = activitiesXMLFileName.Replace("~\\", "");
            }
            SendResult(new TestResultArgs(null, null, TestResultStage.DebugMessage, string.Format("process filename: {0}", activitiesXMLFileName)));

            string activitiesXMLFullFileName = string.Format("{0}{1}\\{2}", this.XmlFilesParentDirectory, xmlRootDir, activitiesXMLFileName);

            recordFileStructure(activitiesXMLFullFileName, parentFilename);
            try
            {
                ActivitiesXmldoc = LoadXMLDoc(activitiesXMLFullFileName);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Cannot load file '{0}' specified in fileName attribute of ActivityNode exception:{1}{2}", activitiesXMLFileName, ex.Message, ex.StackTrace), ex);
            }

            XmlNodeList activitiesXmlNodeList = ActivitiesXmldoc.GetElementsByTagName("Activities");

            foreach (XmlNode activityNode in activitiesXmlNodeList[0].ChildNodes)
            {
                List<Activity> newactivities = ConstructActivityFromXMLNode(activityNode, xmlRootDir, activitiesXMLFullFileName);
                //////}
                //////else
                //////{Activity newactivity = ConstructActivitiesFromXMLNodeOldFormat(activities);
                //////        newprocess.Act
                //////}
                activities.AddRange(newactivities);
            }

            return activities;
        }

        private MethodCalls GetMethodCalls(XmlNode activitiesNode, string nodeTypeName)
        {
            MethodCalls preMethodCall = new MethodCalls();

            if (activitiesNode[nodeTypeName] != null)
            {
                if (activitiesNode[nodeTypeName].Attributes.Count == 3)
                {
                    preMethodCall.Assembly = activitiesNode[nodeTypeName].Attributes["Assembly"].InnerText;
                    preMethodCall.Class = activitiesNode[nodeTypeName].Attributes["Class"].InnerText;
                    preMethodCall.Method = activitiesNode[nodeTypeName].Attributes["Method"].InnerText;
                    //Get Parameters
                    foreach (XmlNode parameter in activitiesNode[nodeTypeName].ChildNodes)
                    {
                        preMethodCall.Parameters.Add(parameter.InnerText);
                    }
                    preMethodCall.NeedToInvoke = true;
                }
            }
            return preMethodCall;
        }

        /// <summary>
        /// Loads the xml file. Throws any FileStream errors
        /// </summary>
        /// <param name="xmlfile">A valid XML filename</param>
        /// <returns>An XMLdoc popultaed by the XML</returns>
        private XmlDocument LoadXMLDoc(string xmlfile)
        {
            FileStream fs = null;
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                fs = new FileStream(xmlfile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                xmldoc.Load(fs);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Dispose();
                }
            }
            return xmldoc;
        }

        #endregion

        #region InvokeMethods

        protected void SendResult(TestResultArgs e)
        {
            if (SendTestResult != null) SendTestResult(this, e);
        }

        private void InvokeMethod(Process process, Activity activity, string assembly, string className, string method, List<string> parameters)
        {
            object[] para = new object[parameters.Count];

            for (int i = 0; i < parameters.Count; i++)
            {
                switch (parameters[i])
                {

                    case "[Process]":
                        para[i] = process;
                        break;

                    case "[Activity]":
                        para[i] = activity;
                        break;

                    default:
                        para[i] = parameters[i];
                        break;
                }
            }
            DynamicCodeExecution.InvokeMethod(assembly, className, method, para);
        }

        #endregion

        #region Main Testing Methods

        public void StartTest(string k2server)
        {
            K2Server = k2server;
            SendResult(new TestResultArgs(null, null, TestResultStage.TestEngineStarted, "Please wait for test to start..."));
            try
            {
                k2helper = new K2Helper(K2Server);
                if (XMLLoaded)
                {
                    StartProcessTesting();
                }
                else
                {
                    //Don't throw, error reported to screen
                    //throw new Exception("XML Test file not loaded");
                }
            }//leeeeeeeeeeeee
            catch (Exception ex)
            {
                if (ex.IsFatal())
                {
                    throw;
                }
                SendResult(new TestResultArgs(null, null, TestResultStage.FatalError, ex.Message + ex.StackTrace));
                return;
            }
            finally
            {
                if (k2helper != null)
                {
                    k2helper.Dispose();
                }
            }

        }

        private void StartProcessTesting()
        {
            int debugConter = 0;

            lastError = GetLastError();
            foreach (Process p in GlobalProcesses)
            {
                debugConter++;
                ProcessProcess(p);

                if (!this.ProceedWithTest)
                {
                    //user may have hit the stop button.
                    break;
                }
            }//foreach process
            SendResult(new TestResultArgs(null, null, TestResultStage.TestEngineEnded));
        }

        private void ProcessProcess(Process p)
        {
            currentProcess = p;
            try
            {
                string message = string.Format("Starting test {0,30}", p.Description);
                SendResult(new TestResultArgs(p, null, TestResultStage.NewProcessTestStarting, message));
                p.TestStartDate = DateTime.Now;

                ProcessActivities(p);

                p.TestEndDate = DateTime.Now;
                SendResult(new TestResultArgs(p, null, TestResultStage.NewProcessTestFinished, "Test " + p.Description + " ended"));
                var servItems = k2helper.WorkflowServer().GetProcessInstances(p.Folio);
                if (servItems.Count == 1)
                {
                    var err = k2helper.WorkflowServer().GetError(servItems[0].ProcID, servItems[0].ID);
                    if (!string.IsNullOrEmpty(err))
                    {
                        if (err.Count() > 0) p.ProcessError = err.ToString();
                        p.ProcessStatus = "In Error";
                    }
                    else
                    {
                        p.ProcessStatus = "Running";
                    }
                }
                else
                {
                    p.ProcessStatus = "Complete";
                }
            }
            catch (Exception ex)
            {
                if (ex.IsFatal())
                {
                    throw;
                }
                SendResult(new TestResultArgs(currentProcess, currentActivity, TestResultStage.FatalError, string.Format("Process : {0}{1}{2}", currentActivity.ProcessName, ex.Message, ex.StackTrace)));
                p.ProcessStatus = "In Error";
            }
        }

        private void ProcessActivities(Process p)
        {
            foreach (Activity a in p.Activities)
            {
                currentActivity = a;
                //Break out of this test if there is an error.
                if (p.ProcessHasUnexpectedErrors) break;

                if (a.Name.Equals("start", StringComparison.OrdinalIgnoreCase))
                {
                    ProcessStartActivity(p, a);
                }//END OF START ACTIVITY
                else if (a.Name.Equals("SubProcess", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (Process subProcess in a.SubProcesses)
                    {
                        ProcessProcess(subProcess);
                    }
                }
                    
                else //Not start
                {
                    ProcessActivity(p, a);
                }//Not Start
                if (!this.ProceedWithTest)
                {
                    break;
                }
            }//foreach activity
        }

        private bool AreIDsEqual(ErrorLog err1, ErrorLog err2)
        {
            if (err1 == null)
            {
                if (err2 == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (err2 == null)
            {
                if (err1 != null)
                {
                    return false;
                }
            }
            //else
            return (err1.ID == err2.ID);
            
        }

        private void ProcessActivity(Process p, Activity a)
        {
            //Ok so we have an activity to action.
            //try and get the item from the worklist, but only a limited number of times.
            bool actioned = false;
            for (int i = 0; i < a.MaxRetryCount; i++)
            {
                bool breakFromLoop = false;
                
                retryLoopIteration(i, p, a, out breakFromLoop, out actioned);
                if (breakFromLoop)
                {
                    break;
                }
            }//retry loop
            if (a.Retry)
            {
                p.ActivityExecutionError = "Could not find activity event: '" + a.Name + "'";

                ////////SendResult(new TestResultArgs(p, a, TestResultStage.DebugMessage, string.Format("a.teststatus blank {0}",a.TestStatus)));
                SendResult(new TestResultArgs(p, a, TestResultStage.ActivityNotFoundGivingUp, "Error: Activity Name: '" + a.Name + "' could not be found after " + a.MaxRetryCount.ToString() + " retries."));
                p.ProcessStatus = "Error";
                a.TestStatus = "Could not find activity";
                p.ProcessHasUnexpectedErrors = true;
                a.Retry = false;
            }
        }

        private void CheckActivityStatus(Process p, Activity a, out bool breakFromLoop)
        {
            breakFromLoop = false;
            char[] squareBrackets = { '[', ']' };
            string statusToCheck = a.Action.Trim(squareBrackets);
            int processInstanceIDToUse = p.ProcessInstanceID;
            //This is a system event and we just want to check if it is there and finished.
            if (!p.ProcessName.Equals(a.ProcessName))
            {//TODO: doesnot cater for recursive IPCs
                processInstanceIDToUse = SmartObjectHelper.GetIPCProcessInstanceByFolio(K2Server, p.ProcessInstanceID, a.ProcessName);
                //////SendResult(new TestResultArgs(p, a, TestResultStage.DebugMessage, string.Format("IPC detected pid:{0} originalProcessName:{1} newProcessName{2}",processInstanceIDToUse,p.ProcessName, a.ProcessName)));
                //Check for IPC and if so get the child IPC pid
                if (processInstanceIDToUse == (int)SmartObjectHelper.ReturnCodes.NoProcessInstanceFound)
                {
                    string IPCMessage = string.Format("'{0}' ({1})- '{2}':IPC child not found", p.ProcessName, a.ProcessName, a.Name);
                    //////a.TestStatus = IPCMessage;
                    SendResult(new TestResultArgs(p, a, TestResultStage.ActivityNotFoundRetrying, IPCMessage));
                    //////break;
                }
            }

            if (statusToCheck == "NotTaken")
            {
                //check path taken or not taken as appropriate.
                if (string.IsNullOrEmpty(SmartObjectHelper.GetActivityStatus(K2Server, processInstanceIDToUse, a.Name).Trim()))
                {
                    a.Retry = false;
                    a.TestStatus = string.Format("'{0}' - '{1}':Activity Not Taken Correctly: '", a.ProcessName, a.Name);
                    SendResult(new TestResultArgs(p, a, TestResultStage.ActivityActioned, a.TestStatus + a.Name + "' "));
                    breakFromLoop = true; return;
                }
                else
                {
                    string err = string.Format("'{0}' - '{1}':Activity Taken Incorrectly: ", a.ProcessName, a.Name);
                    FailTest(err, TestResultStage.ActivityExecutionError, p, a);
                    breakFromLoop = true; return;
                }
            }
            else //Completed, Active etc.
            {
                string actualStatus = SmartObjectHelper.GetActivityStatus(K2Server, processInstanceIDToUse, a.Name).Trim();
                //////SendResult(new TestResultArgs(p, a, TestResultStage.DebugMessage, string.Format("statusToCheck:{0} actualStatus:{1} newProcessName{2}", statusToCheck, actualStatus, a.ProcessName)));
                if (actualStatus == statusToCheck)
                {
                    a.Retry = false;
                    a.TestStatus = string.Format("'{0}' - '{1}':Activity {2}: '", a.ProcessName, a.Name, actualStatus);
                    SendResult(new TestResultArgs(p, a, TestResultStage.ActivityActioned, a.TestStatus + a.Name + "' "));
                    bool breakOut;
                    //process the datafields to determine if we have to save a dfs value
                    processDataFields(p, a, out breakOut);
                    breakFromLoop = true; return;
                }
                else
                {
                    //
                    string err = string.Format("'{0}' - '{1}':[{2}] action expected but actual status was [{3}]", a.ProcessName, a.Name, statusToCheck, actualStatus);
                    FailTest(err, TestResultStage.ActivityNotFoundRetrying, p, a);
                }
            }
        }

        private void ActionWorklistItemsIfFound(WorklistItems items, Process p, Activity a, out bool breakFromLoop, out bool actioned, out bool IPCeventFound)
        {
            actioned = false;
            breakFromLoop = false;
            IPCeventFound = false;
            
             string currentUser = "K2:" + System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            for (int x = 0; x < items.Count; x++)
            {
                //Loop through and see if there is an item for this activity assigned to the current user.
                if (string.Equals(currentUser, items[x].Destination, StringComparison.OrdinalIgnoreCase)
                    && a.Name.Equals(items[x].ActivityName))
                {
                    //if we are here then maybe we have an item in an IPC event, 
                    //set the a.ProcInstID which will be called on the next round.
                    // important to note that IPC retry counts should be set higher to
                    //cope with this hack!
                    a.ProcessInstanceID = items[x].ProcInstID;
                    IPCeventFound = true;
                    breakFromLoop = true; return;
                }
                if (!this.ProceedWithTest)
                {
                    breakFromLoop = true; return;
                }
            }
            for (int x = 0; x < items.Count; x++)
            {
                //we have a matching activity, but it is not assigned to the current user.
                if (string.Equals(items[x].ActivityName, a.Name, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        //If we are on the last loop, lets impersonate the destination user and action the item.
                        string impersonatedUser = string.Empty;
                        Dictionary<string, object> filter = new Dictionary<string, object>();
                        switch (items[x].Actioner.ActionerType)
                        {
                            case ActionerType.User:
                                impersonatedUser = items[x].Actioner.Name;
                                break;
                            case ActionerType.Groups:
                                filter.Add("Group_name", items[x].Actioner.Name);
                                filter.Add("LabelName", "K2:");
                                //get the first user in the group.
                                var group = k2helper.SmartObjectClient().SmartObjectGetList(filter, "UMUSer", "Get_Group_Users");
                                impersonatedUser = group.Rows[0]["Name"].ToString();
                                break;
                            case ActionerType.Role:
                                filter.Add("Role_Name", items[x].Actioner.Name);
                                //get the first user in the group.
                                var role = k2helper.SmartObjectClient().SmartObjectGetList(filter, "UMUSer", "Get_Role_Users");
                                impersonatedUser = role.Rows[0]["Name"].ToString();
                                break;
                        }
                        //now impersonate them and action the worklist item.
                        var wlitem = k2helper.WorkflowClient().GetWorkListItem(items[x].ProcInstID + "_ " + items[x].ActInstDestID, impersonatedUser);
                        
                        actioned = ActionActivity(p, a, wlitem, impersonatedUser, true);
                        if (!actioned)
                        {
                            breakFromLoop = true; return;
                        }
                        //wlitem.Actions[a.Action].Execute();
                        //k2helper.WorkflowServer().RedirectWorklistItem(items[x].Destination, currentUser, items[x].ProcInstID, items[x].ActInstDestID, items[x].ID);
                    }
                    catch (Exception ex)
                    {
                        if (ex.IsFatal())
                        {
                            throw;
                        }
                        string err = "Error getting worklist item : " + ex.Message;
                        FailTest(err, TestResultStage.ActivityExecutionError, p, a);
                        breakFromLoop = true; return;
                    }
                    if (!this.ProceedWithTest)
                    {
                        breakFromLoop = true; return;
                    }
                    Thread.Sleep(1000);
                }
            }//end for
        }

        private void whatdoesthisdo(SourceCode.Workflow.Client.Worklist wl, Process p, Activity a, out bool breakFromLoop, out bool actioned)
        {
            breakFromLoop = false;
            actioned = false;
            //Incase there are multiple worklist items, lets find the activity we want to action.
            foreach (SourceCode.Workflow.Client.WorklistItem item in wl)
            {
                if (string.Equals(item.ActivityInstanceDestination.Name, a.Name, StringComparison.OrdinalIgnoreCase))
                {
                    string currentuser = "K2:" + System.Security.Principal.WindowsIdentity.GetCurrent().Name;

                    //We have found the activity that we should update 
                    //first set process instance data fields.
                    Dictionary<string, object> input = new Dictionary<string, object>();
                    bool breakFromInnerLoop = false;
                    processDataFields(item, a, out breakFromInnerLoop);
                    if (breakFromInnerLoop)
                    {
                        breakFromLoop = true; return;
                    }
                    //We need to see if the allocated person is the curent person , otehrwise reopen the worklst item.
                    if (string.Equals(item.AllocatedUser, currentuser, StringComparison.OrdinalIgnoreCase))
                    {
                        //Pre Method
                        RunMethod(p, a, MethodType.PreMethod);
                        actioned = ActionActivity(p, a, item, currentuser, false);
                        if (!actioned)
                        {
                            breakFromLoop = true; return;
                        }
                        RunMethod(p, a, MethodType.PostMethod);
                    }
                    else
                    {
                        //open this item as the allocated user.
                        SourceCode.Workflow.Client.WorklistItem newitem = null;
                        try
                        {
                            newitem = k2helper.WorkflowClient().GetWorkListItem(item.SerialNumber, item.AllocatedUser);
                        }
                        catch (Exception ex)
                        {
                            if (ex.IsFatal())
                            {
                                throw;
                            }
                            string err = "Error getting worklist " + ex.Message;
                            FailTest(err, TestResultStage.ActivityExecutionError, p, a);
                            breakFromLoop = true; return;
                        }

                        actioned = ActionActivity(p, a, newitem, currentuser, false);
                        if (!actioned)
                        {
                            breakFromLoop = true; return;
                        }
                    }//open as allocated user
                    breakFromLoop = true; return;
                }//activity name matches
                if (!this.ProceedWithTest)
                {
                    breakFromLoop = true; return;
                }
            }//foreach workitem
        }

        private void retryLoopIteration(int i, Process p, Activity a, out bool breakFromLoop, out bool actioned)
        {
            actioned = false;
            breakFromLoop = false;
            a.CountOfRetries = i;
            SendResult(new TestResultArgs(p, a, TestResultStage.DebugMessage, string.Format("a.Retry was {0}", a.Retry)));
            bool IPCeventFound = false;
            /////////////////////////////////////////////////////////////////////////////////////////////////////
            if (a.Action.StartsWith("[") && a.Action.EndsWith("]"))
            {
                CheckActivityStatus(p, a, out breakFromLoop);
                if (!this.ProceedWithTest)
                {
                    breakFromLoop = true;
                }
                if (breakFromLoop)
                {
                    return;
                }
            }//search for an activity in a certain state
            else //try to execute the action
            {
                
                //Get all items by the folio so it should be unique
                SourceCode.Workflow.Management.Criteria.WorklistCriteriaFilter fil = new SourceCode.Workflow.Management.Criteria.WorklistCriteriaFilter();
                fil.AddRegularFilter(SourceCode.Workflow.Management.WorklistFields.Folio, SourceCode.Workflow.Management.Criteria.Comparison.Equals, p.Folio);
                fil.AddRegularFilter(SourceCode.Workflow.Management.WorklistFields.ProcessFullName, SourceCode.Workflow.Management.Criteria.Comparison.Equals, string.IsNullOrEmpty(a.ProcessName) ? p.ProcessName : a.ProcessName);
                WorklistItems items = k2helper.WorkflowServer().GetWorklistItems(fil);

                ActionWorklistItemsIfFound(items, p, a, out breakFromLoop, out actioned, out IPCeventFound);
                if (!actioned)
                {
                    //Open a conection to the server
                    SourceCode.Workflow.Client.Worklist wl = k2helper.WorkflowClient().GetAllWorkListItem((a.ProcessInstanceID == 0) ? p.ProcessInstanceID : a.ProcessInstanceID);

                    whatdoesthisdo(wl,p,a, out breakFromLoop, out actioned);
                    if (breakFromLoop)
                    {
                        return;
                    }
                }//workitem exists
            }// else try to execute action

            if (a.Retry)
            {
                string msg = string.Format("Activity Name: '{0}' was not available, retrying in {1} seconds. Retries left : {2}", a.Name , a.RetryInSeconds , (a.MaxRetryCount - a.CountOfRetries));
                SendResult(new TestResultArgs(p, a, TestResultStage.ActivityNotFoundRetrying, msg));
            }
            ErrorLog newError = GetLastError();
            if (!AreIDsEqual(newError, lastError))
            { //Basically if there is a new error
                if (a.ErrorExpected)
                {//Are We expecting an error?
                    SendResult(new TestResultArgs(p, null, TestResultStage.ActivityActioned, string.Format("Error Expected and occured: {0} '{2}' ---> '{1}'"
                        , newError.ID, newError.Description, newError.ProcessName)));
                    a.Retry = false;
                    breakFromLoop = true;
                }
                else
                {
                    SendResult(new TestResultArgs(p, null, TestResultStage.FatalError, string.Format("Error: {0} '{2}' ---> '{1}'"
                        , newError.ID, newError.Description, newError.ProcessName)));
                    a.Retry = false;
                    a.TestStatus = "More errors";
                    p.ProcessHasUnexpectedErrors = true;
                }
            }
            else
            {//no new error
                if (a.ErrorExpected)
                {
                    SendResult(new TestResultArgs(p, null, TestResultStage.ActivityNotFoundRetrying, string.Format("'{0}' ({1})- '{2}': Error Expected but did not occur!.. yet!: {3}", p.ProcessName, a.ProcessName, a.Name, a.TestStatus)));
                    //Thread.Sleep(1000);
                    actioned = false;
                    a.Retry = true;
                    a.TestStatus = string.Empty;
                }
                else
                {
                    SendResult(new TestResultArgs(p, a, TestResultStage.DebugMessage, "No Error expected and no error!"));
                }
            }
            lastError = newError;
            if (!this.ProceedWithTest)
            {
                breakFromLoop = true; return;
            }
            if (a.Retry)
            {
                SendResult(new TestResultArgs(p, a, TestResultStage.DebugMessage, "told to retry, sleeping"));
                System.Threading.Thread.Sleep(a.RetryInSeconds * 1000);
            }
        }

        private void processDataField(CoreDataField dfToProcess, DataField wfdf, CoreDataField dfThatMocksWFDF, out bool needsUpdate, out bool breakFromLoop)
        {
            string checkText = dfToProcess.Check;
            needsUpdate = false;
            breakFromLoop = false;
            try
            {
                //Two modes, one setting and getting, one getting only, which does not need wfDataField
                if (checkText.Length == 0 || checkText.Equals("set"))
                {

                    wfdf.Value = dfToProcess.Value;
                    needsUpdate = true;
                }
                else if (checkText.StartsWith("check:", StringComparison.OrdinalIgnoreCase))
                {
                    string[] partsOfCheck = checkText.Split(new char[] { ':' });
                    string whatToCheck = partsOfCheck[1];
                    if (!AssertValue(dfThatMocksWFDF, whatToCheck, dfToProcess))
                    {
                        FailTest("Assert Failed", TestResultStage.ActivityExecutionError, currentProcess, currentActivity);
                        
                        breakFromLoop = true;
                    }
                }
                else if (checkText.StartsWith("store:", StringComparison.OrdinalIgnoreCase))
                {
                    string[] partsOfCheck = checkText.Split(new char[] { ':' });
                    string keyToStoreAgainst = partsOfCheck[1];
                    //will add if it doesn't exist and overwrite if it does exist
                    //I want this behaviour .... for now!!! Change to .Add() if you want to throw an error if it exists
                    dataFieldDictionary[keyToStoreAgainst] = dfThatMocksWFDF.Value.ToString();
                }
                else if (checkText.StartsWith("setToVariable", StringComparison.OrdinalIgnoreCase))
                {
                    if (dataFieldDictionary.ContainsKey(dfToProcess.Name))
                    {
                        wfdf.Value = dataFieldDictionary[dfToProcess.Name];
                    }
                    else
                    {
                        string err = string.Format("Problem setting datafield. configured to set from temp variable, but temp variable name does not exist varName:{0} dfName:{1}", dfToProcess.Name, wfdf.Name);

                        FailTest(err, TestResultStage.ActivityExecutionError, currentProcess, currentActivity);
                    }
                }
                else
                {
                    throw new NotImplementedException(string.Format("only catering for checking 'set', '', 'check:?', 'store:?' but got '{0}'", dfToProcess.Check));
                }
            }
            catch (NullReferenceException ex)
            {
                string methodProperties = string.Format("mode: '{0}' p:{1}, a:{2}", checkText, currentProcess, currentActivity);
                if (wfdf == null)
                {
                    throw new ArgumentException("cannot set workflow datafield in this activity mode " + methodProperties, ex);
                }
                else if (dfThatMocksWFDF == null)
                {
                    throw new ArgumentException("cannot lookup workflow values in this activity mode" + methodProperties, ex);
                }
                else
                {
                    throw;
                }
            }

        }

        private void processDataFields(Process p, Activity a, out bool breakFromLoop)
        {
            bool needsUpdate = false;
            breakFromLoop = false;

            foreach (var df in a.DataFields)
            {
                string dfValue = SmartObjectHelper.GetProcessDataFieldValue(K2Server, p.ProcessInstanceID, df.Key);
                var mockDF = new CoreDataField();
                mockDF.Name = df.Key;
                mockDF.Value = dfValue;
                processDataField(df.Value, null, mockDF, out needsUpdate, out breakFromLoop);
            }

        }

        private void processDataFields(SourceCode.Workflow.Client.WorklistItem item, Activity a, out bool breakFromLoop)
        {
            bool needsUpdateOut = false;
            bool needsUpdate = false;
            breakFromLoop = false;
            
            foreach (var df in a.DataFields)
            {
                DataField wfdf;
                if (df.Value.Type == CoreDataFieldType.Process)
                {
                    wfdf = item.ProcessInstance.DataFields[df.Key];
                }
                else
                {
                    wfdf = item.ActivityInstanceDestination.DataFields[df.Key];
                }
                CoreDataField dfFromWFserver = K2Field.Helpers.Core.Code.Converters.ConvertWFDataFieldToCoreDataField(wfdf);
                processDataField(df.Value, wfdf, dfFromWFserver, out needsUpdateOut, out breakFromLoop);
                if (needsUpdateOut)
                {
                    needsUpdate = true;
                }
                if (breakFromLoop)
                {
                    break;
                }
            }
           
            //Update the process data fields.
            if (needsUpdate)
            {
                item.ProcessInstance.Update();
            }                   
        }

        private void FailTest(string reasonText, TestResultStage tesResultStage, Process p, Activity a)
        {
            p.ActivityExecutionError = reasonText;
            a.ActivityExecutionError = p.ActivityExecutionError;
            if (tesResultStage != TestResultStage.ActivityNotFoundRetrying)
            {
                p.ProcessHasUnexpectedErrors = true;
                a.Retry = false;
                a.TestStatus = "Failed";
            }
            SendResult(new TestResultArgs(p, a, TestResultStage.ActivityExecutionError, reasonText));
        }

        private bool AssertValue(CoreDataField mockwfdf, string whatToCheck, CoreDataField df)
        {
            bool retVal = false;
            switch (whatToCheck)
            {
                case "ne":
                    retVal = !(mockwfdf.Value.ToString().Equals(df.Value.ToString()));
                    break;
                case "eq":
                    retVal = mockwfdf.Value.ToString().Equals(df.Value.ToString());
                    break;
                default:
                    throw new NotImplementedException(string.Format("whatToCheck was {0}, but I've only programmed 'ne' and 'eq' so far", whatToCheck));
                    break;
            }
            if (retVal)
            {
                SendResult(new TestResultArgs(currentProcess, currentActivity, TestResultStage.DebugMessage, string.Format("Assert passed: {0} {1} {2}: {3} {4} {5}'", currentProcess.ProcessName, currentActivity.Name, mockwfdf.Name, mockwfdf.Value, whatToCheck, df.Value)));
            }
            else
            {
                string err = string.Format("Assert failed: {0} {1} {2}: {3} {4} {5}'", currentProcess.ProcessName, currentActivity.Name, mockwfdf.Name, mockwfdf.Value, whatToCheck, df.Value);
                FailTest(err, TestResultStage.ActivityExecutionError, currentProcess, currentActivity);
            }
            return retVal;
        }

        private void prepareDataFields(Dictionary<string, CoreDataField> dataFields)
        {
            foreach (var df in dataFields)
            {
                if (df.Value.Check.Equals("setToVariable", StringComparison.OrdinalIgnoreCase))
                {
                    string keyToSet = df.Value.Value.ToString();
                    if (dataFieldDictionary.ContainsKey(keyToSet) && dataFields.ContainsKey(keyToSet))
                    {
                        dataFields[keyToSet].Value = dataFieldDictionary[keyToSet];
                    }
                    else
                    {
                        string reason = string.Format("Trying to set datafield '{0}' to an existing stored valiable called '{1}' but cannot find it", df.Key, keyToSet);
                        FailTest(reason, TestResultStage.FatalError, currentProcess, currentActivity);
                    }
                }
            }
        }

        private void ProcessStartActivity(Process p, Activity a)
        {
            RunMethod(p, a, MethodType.PreMethod);
            //Start Process
            SendResult(new TestResultArgs(p, a, TestResultStage.ProcessStarting, "Process : " + a.ProcessName));
            p.Folio = string.Format("{0}", "TEST : " + DateTime.Now.ToString());

            //If we need to set a datafield to an existing variable, we need to modify the a.DataFields collection
            prepareDataFields(a.DataFields);
            try
            {
                p.ProcessInstanceID = k2helper.StartK2Process(a.ProcessName, p.Folio, a.DataFields);
            }
            catch (Exception ex)
            {
                if (ex.IsFatal())
                {
                    throw;
                }
                SendResult(new TestResultArgs(p, a, TestResultStage.FatalError, string.Format("Process : {0}{1}{2}", a.ProcessName, ex.Message, ex.StackTrace)));
                /// throw
            }

            if (p.ProcessInstanceID < 0)
            {
                //TODO: refactor to use FailTest
                p.ActivityExecutionError = "Process did not start : ";
                a.ActivityExecutionError = p.ActivityExecutionError;

                SendResult(new TestResultArgs(p, a, TestResultStage.FatalError, "Process did not start : " + p.ActivityExecutionError));
                p.ProcessHasUnexpectedErrors = true;
            }
            else
            {
                //the p.Processname only gets assigned once the start activity has been initiated.
                p.ProcessName = a.ProcessName;
                a.Retry = false;
                a.TestStatus = "Created";
                //////SendResult(new TestResultArgs(p, a, TestResultStage.DebugMessage, "a.teststatus set to created"));
                SendResult(new TestResultArgs(p, a, TestResultStage.ProcessStarted));
            }

            //Post Method
            RunMethod(p, a, MethodType.PostMethod);
        }

        #endregion

        #region helper methods

        private SourceCode.Workflow.Management.ErrorLog GetLastError() //////int ErrorCountBeforeTests, out int LastErrorNumBeforeTests )
        {
            WorkflowManagementServer ManagementServer = null;
            try
            {
                ManagementServer = new WorkflowManagementServer();
                ManagementServer.Open(ConfigHelper.GetConnectionString("ManagementServerCS"));
                ErrorProfile profile = ManagementServer.GetErrorProfile("All");
                ErrorLogCriteriaFilter elcf = new ErrorLogCriteriaFilter();
                elcf.ORDER_BY(ErrorLogFields.ErrorLogID, CriteriaSortOrder.Ascending);
                ErrorLogs logs = ManagementServer.GetErrorLogs(profile.ID);
                if (logs.Count > 0)
                {
                    return logs[logs.Count - 1];
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                if (ManagementServer != null && ManagementServer.Connection != null)
                {
                    ManagementServer.Connection.Dispose();
                    ManagementServer = null;
                }
            }
        }

        private bool ActionActivity(Process p, Activity a, SourceCode.Workflow.Client.WorklistItem item, string actionuser, bool impersonateUser)
        {
            try
            {
                ////////item.ActivityInstanceDestination.DataFields["Data.Actioner"].Value = "bob";
                ////////item.ProcessInstance.Update();
                bool fieldToSaveFound = false;

                foreach (KeyValuePair<string, CoreDataField> kvp in a.DataFields)
                {
                    if (kvp.Value.Check == "set")
                    {
                        //try to set the datafield
                        if (kvp.Value.Type == CoreDataFieldType.Activity)
                        {
                            item.ActivityInstanceDestination.DataFields[kvp.Value.Name].Value = kvp.Value.Value;
                        }
                        else
                        {
                            item.ProcessInstance.DataFields[kvp.Value.Name].Value = kvp.Value.Value;
                        }
                        fieldToSaveFound = true;
                    }
                }
                if (fieldToSaveFound)
                {
                    item.ProcessInstance.Update();
                }


                if (impersonateUser)
                {
                    k2helper.WorkflowClient().connection.ImpersonateUser(actionuser);
                }
                item.Actions[a.Action].Execute();
                string actionMsg = string.Format("'{0}' - '{1}': Activity '{2}' as user {3}", a.ProcessName, a.Name, a.Action, actionuser);
                a.Retry = false;
                a.TestStatus = actionMsg;
                SendResult(new TestResultArgs(p, a, TestResultStage.ActivityActioned, actionMsg));
                if (impersonateUser)
                {
                    k2helper.WorkflowClient().connection.RevertUser();
                }
                return true;
            }
            catch (Exception ex)
            {
                if (ex.IsFatal())
                {
                    throw;
                }

                if (impersonateUser)
                {
                    k2helper.WorkflowClient().connection.RevertUser();
                }
                string err = string.Format("'{0}' - '{1}': Execution failed : {2}, {3}", a.ProcessName, a.Name, ex.Message, ex.StackTrace);
                //Some errors are temporary, so retry
                FailTest(err, TestResultStage.ActivityNotFoundRetrying, p, a);
                return false;
            }

        }

        private void RunMethod(Process p, Activity a, MethodType type)
        {
            //Do a quick check
            switch (type)
            {
                case MethodType.PreMethod:
                    if (!a.PreMethodCall.NeedToInvoke) return;
                    break;

                case MethodType.PostMethod:
                    if (!a.PostMethodCall.NeedToInvoke) return;
                    break;
            }

            string sAssembly = (type == MethodType.PreMethod) ? a.PreMethodCall.Assembly : a.PostMethodCall.Assembly;
            string sClass = (type == MethodType.PreMethod) ? a.PreMethodCall.Class : a.PostMethodCall.Class;
            string sMethod = (type == MethodType.PreMethod) ? a.PreMethodCall.Method : a.PostMethodCall.Method;

            List<string> sParameters = (type == MethodType.PreMethod) ? a.PreMethodCall.Parameters : a.PostMethodCall.Parameters;

            //Run method.
            try
            {
                InvokeMethod(p, a, sAssembly, sClass, sMethod, sParameters);
                SendResult(new TestResultArgs(p, a, (type == MethodType.PreMethod) ? TestResultStage.ActivityPreMethodExecuted : TestResultStage.ActivityPostMethodExecuted));
            }
            catch (Exception ex)
            {
                if (ex.IsFatal())
                {
                    throw;
                }
                //TODO: refactor to use FailTest
                p.ActivityExecutionError = a.ActivityExecutionError = ex.Message.ToString();
                SendResult(new TestResultArgs(p, a, TestResultStage.ActivityExecutionError, type.ToString() + " execution error : " + p.ActivityExecutionError));
                p.ProcessHasUnexpectedErrors = true;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }
                // Release unmanaged resources. If disposing is false, 
                // only the following code is executed.
                if (k2helper != null)
                {
                    k2helper.Dispose();
                }
                // Note that this is not thread safe.
                // Another thread could start disposing the object
                // after the managed resources are disposed,
                // but before the disposed flag is set to true.
                // If thread safety is necessary, it must be
                // implemented by the client.

            }
            disposed = true;
        }

        #endregion
    }

}
