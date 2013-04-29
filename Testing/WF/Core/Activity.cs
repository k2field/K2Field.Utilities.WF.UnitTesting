using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K2Field.Helpers.Core.Code;

namespace K2Field.Utilities.Testing.WF.Core
{
    public class Activity
    {
        public string Name { get; set; }
        /// <summary>
        /// The process name can be specified for each activity allow ICP calls to take place.
        /// </summary>
        public string ProcessName { get; set; }
        public int ProcessInstanceID { get; set; }
        public string ActivityExecutionError { get; set; }
        public bool ErrorExpected { get; set; }
        public string TestStatus { get; set; }
        public string Action { get; set; }
        public MethodCalls PreMethodCall { get; set; }
        public MethodCalls PostMethodCall { get; set; }
        public int RetryInSeconds { get; set; }
        public int MaxRetryCount { get; set; }
        public int CountOfRetries { get; set; }
        public Dictionary<string, K2Field.Helpers.Core.CoreDataField> DataFields { get; set; }
        public List<Process> SubProcesses { get; set; }
        public bool Retry { get; set; }
        
        public Activity()
        {
            DataFields = new Dictionary<string, K2Field.Helpers.Core.CoreDataField>();
            PreMethodCall = new MethodCalls();
            PostMethodCall = new MethodCalls();
            Retry = true;
        }
    }

}
