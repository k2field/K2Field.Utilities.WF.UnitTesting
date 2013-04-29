using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace K2Field.Utilities.Testing.WF.Core
{
    public static class ConfigHelper
    {

        public static string GetConnectionString(string connName)
        {
            string strReturn = string.Empty;
            if (!(string.IsNullOrEmpty(connName)))
            {
                strReturn = ConfigurationManager.ConnectionStrings[connName].ConnectionString;
            }
            else
            {
                strReturn = ConfigurationManager.ConnectionStrings["ManagementServerCS"].ConnectionString;
            }
            return strReturn;
        }

    }
}
