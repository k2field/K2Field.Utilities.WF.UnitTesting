using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using K2Field.Helpers.Core;
using K2Field.Helpers.Core.Code;

namespace K2Field.Utilities.Testing.WF.Core
{

    public static class SmartObjectHelper
    {
        public enum ReturnCodes
        {
            NoProcessInstanceFound = -1
        }

        public static string GetActivityStatus(string K2Server ,int processInstanceID, string activityName)
        {
            using (K2Helper helper = new K2Helper(K2Server))
            {
                Dictionary<string, object> Data = new Dictionary<string, object>();
                Data.Add("ProcessInstanceID", processInstanceID.ToString());
                Data.Add("ActivityName", activityName);
                DataTable dt = helper.SmartObjectClient().SmartObjectGetList(Data, "Activity_Instance", "List");
                
                DateTime latestTime = DateTime.MinValue;
                string lastestAndGreatestStatus = string.Empty;
                foreach (DataRow dr in dt.Rows)
                {
                    DateTime currentTime = dr.Field<DateTime>("StartDate");
                    if (currentTime > latestTime)
                    {
                        lastestAndGreatestStatus = dr.Field<string>("Status");
                        latestTime = currentTime;
                    }
                }
                return lastestAndGreatestStatus; //empty string if not found
            }
        }

        public static string GetProcessDataFieldValue(string K2Server, int processInstanceID, string dataFieldName)
        {
            //TODO: this currently opens a connection to wf server twice
            using (K2Helper helper = new K2Helper(K2Server))
            {
                Dictionary<string, object> Data = new Dictionary<string, object>();
                Data.Add("ProcessInstanceID", processInstanceID);
                Data.Add("DataName", dataFieldName);
                //////////Data.Add("ProcessName", "Underwriting\\Supplementary\\Setup");



                DataTable dt = helper.SmartObjectClient().SmartObjectGetList(Data, "Process_Data", "List", true);

                foreach (DataRow dr in dt.Rows)
                {
                    return dr.Field<string>("DataValue");
                }
                throw new Exception(string.Format("Cannot find datafield {0} for PI: {1}", dataFieldName, processInstanceID));
            }
        }


        private static void SplitPathIntoFolderAndName(string fullPath, out string folderName, out string processName)
        {
            string[] words = fullPath.Split('\\');
            processName = words[words.Length - 1];
            StringBuilder sb = new StringBuilder();
            string priorWord = string.Empty;
            //folderName = string.Empty;
            foreach (string word in words)
            {
                if (priorWord.Length > 0)
                {
                    sb.AppendFormat("{0}\\", priorWord);
                }
                priorWord = word;
            }
            folderName = sb.ToString().TrimEnd('\\');

        }

        /// <summary>
        /// ASSUMPTION: IPCs have the same (otherwise unique) folio as their parents 
        /// - not always the case, but no other way of detecting IPC
        /// This uses the parent Process instance ID to get the folio
        /// We then get a list of processes with that folio and exclude the parent.
        /// </summary>
        /// <param name="ParentProcessInstanceID">pid of the parent</param>
        /// <returns>-1 if ipc child is not found, otherwise the processinstanceid</returns>
        public static int GetIPCProcessInstanceByFolio(string K2Server, int ParentProcessInstanceID, string ProcessFullName)
        {
            using (K2Helper helper = new K2Helper(K2Server))
            {

                string processFolder;
                string processName;
                SplitPathIntoFolderAndName(ProcessFullName, out processFolder, out processName);

                Dictionary<string, object> Data = new Dictionary<string, object>();
                Data.Add("ProcessInstanceID", ParentProcessInstanceID.ToString());
                DataTable dt = helper.SmartObjectClient().SmartObjectGetList(Data, "Process_Instance", "List");
                if (dt.Rows.Count == 0)
                {
                    return (int)ReturnCodes.NoProcessInstanceFound;
                }
                else
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string folio = dr.Field<string>("Folio");
                        Data = new Dictionary<string, object>();
                        Data.Add("Folio", folio); //where for art thou folio?
                        Data.Add("ProcessName", processName);
                        Data.Add("Folder", processFolder);

                        dt = helper.SmartObjectClient().SmartObjectGetList(Data, "Process_Instance", "List");
                        if (dt.Rows.Count == 0)
                        {
                            return (int)ReturnCodes.NoProcessInstanceFound;
                        }
                        else
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                if (row.Field<int>("ProcessInstanceID") != ParentProcessInstanceID)
                                {
                                    return row.Field<int>("ProcessInstanceID");
                                }
                            }
                            return (int)ReturnCodes.NoProcessInstanceFound;
                        }
                    }

                    return (int)ReturnCodes.NoProcessInstanceFound;
                }
            }
        }

    }
}
