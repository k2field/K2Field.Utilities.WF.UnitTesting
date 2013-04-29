using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SourceCode.Field.Core.Helper;
using K2Field.Utilities.WorkflowTesting.Core;

namespace K2Field.Utilities.Examples.SampleData
{
    public class Travel
    {
        public void CreateTravelObject(Process p, string num)
        {
            using (SourceCode.Field.Core.Helper.K2Helper helper = new K2Helper("dlx"))
            {
                Dictionary<string, object> Data = new Dictionary<string, object>();
                Data.Add("PersonName", "Adam Castle");
                Data.Add("Amount", num);
                string TravelRequestsID = helper.SmartObjectClient().SmartObjectCreate(Data, "TravelRequestsID", "TravelRequests");

                //Lets create a new data field, so when our process starts it will start with the new travel request ID
                CoreDataField df = new CoreDataField();
                df.Name = "TravelRequestsID";
                df.Value = TravelRequestsID;

                p.Activities[0].DataFields.Add(df.Name,df);
            }
        }
    }


    public static class StaticTravel
    {
        //we can also use static methods
        public static string GetActivityStatus(int processInstanceID, string activityName)
        {
            using (SourceCode.Field.Core.Helper.K2Helper helper = new K2Helper("dlx"))
            {
                Dictionary<string, object> Data = new Dictionary<string, object>();
                Data.Add("Process Instance ID", processInstanceID.ToString());
                Data.Add("Activity Name", activityName);
                DataTable dt = helper.SmartObjectClient().SmartObjectGetList(Data, "Activity_Instance", "List");
                return dt.Rows[0].Field<string>("Status");
            }
        }
    }

}
