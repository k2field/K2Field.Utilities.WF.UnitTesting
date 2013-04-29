using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K2Field.Utilities.Testing.WF.Core
{
    public class MethodCalls
    {
        /// <summary>
        /// Must be the full path to the .dll file
        /// </summary>
        public string Assembly { get; set; }
        public string Class { get; set; }
        public string Method { get; set; }
        public List<string> Parameters { get; set; }
        public bool NeedToInvoke { get; set; }

        public MethodCalls()
        {
            Parameters = new List<string>();
        }
    }
}
