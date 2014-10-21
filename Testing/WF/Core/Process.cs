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


        public override string ToString() 
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format(new NullFormat(), "Description:{0}\n Folio:{1}\n ProcessError:{2}\n ProcessHasUnexpectedErrors:{3}\n ProcessInstanceID:{4}\n ProcessName:{5}\n ProcessStatus:{6}\n ProcessType:{7}\n RootProcess:{8}\n TestEndDate:{9}\n TestStartDate:{10}\n", this.Description, this.Folio, this.ProcessError, this.ProcessHasUnexpectedErrors, this.ProcessInstanceID, this.ProcessName, this.ProcessStatus, this.ProcessType, this.RootProcess, this.TestEndDate, this.TestStartDate));
            sb.Append("\nList of Activities:\new");
            
            foreach (Activity act in Activities)
            {
                sb.Append(act.Name);
                sb.Append("\n");
            }
            return sb.ToString();
        }

    }

    //TODO: Own class file
    public class NullFormat : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type service)
        {
            if (service == typeof(ICustomFormatter))
            {
                return this;
            }
            else
            {
                return null;
            }
        }

        public string Format(string format, object arg, IFormatProvider provider)
        {
            if (arg == null)
            {
                return "NULL";
            }
            IFormattable formattable = arg as IFormattable;
            if (formattable != null)
            {
                return formattable.ToString(format, provider);
            }
            return arg.ToString();
        }
    }

    public enum ProcessType
    {
        cleanup,
        setup,
        assert
    }

}
