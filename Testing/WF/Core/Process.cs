using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K2Field.Utilities.Testing.WF.Core
{
    public class Process
    {
        private bool processHasUnexpectedErrors = false;


        public string Folio { get; set; }
        public string Description { get; set; }
        public string ProcessName { get; set; }
        public int ProcessInstanceID { get; set; }
        public string ActivityExecutionError { get; set; }
        public string ProcessError { get; set; }
        public string ProcessStatus { get; set; }
        public bool ProcessHasUnexpectedErrors
        {
            get
            {
                return processHasUnexpectedErrors;
            }
            set
            {
                processHasUnexpectedErrors = value;
                if (RootProcess != null)
                {
                    RootProcess.processHasUnexpectedErrors = processHasUnexpectedErrors;
                }
            }
        }

        public DateTime TestStartDate { get; set; }
        public DateTime TestEndDate { get; set; }
        public List<Activity> Activities { get; set; }
        public ProcessType ProcessType { get; set; }
        public Process RootProcess { get; set; }


        public Process()
        {
            Activities = new List<Activity>();
        }
    }

    public enum ProcessType
    {
        cleanup,
        setup,
        assert
    }

}
