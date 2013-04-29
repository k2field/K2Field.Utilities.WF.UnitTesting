using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using K2Field.Utilities.WorkflowTesting.Core;
using K2Field.Helpers.Core.Code;

namespace K2Field.Utilities.WorkflowTesting.Core
{
    /// <summary>
    /// Summary description for Travel
    /// </summary>
    [TestClass]
    public class Travel
    {
        public Travel()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TravelTest()
        {

            TestingHelper K2TestHelper = new TestingHelper();
            K2TestHelper.LoadXMLTestFile(@"C:\test\SourceCode.Workflow.TestingUI\Tests\Travel\TestSettings.xml");

            K2TestHelper.Processes.AddRange(K2TestHelper.Processes);

            K2TestHelper.StartTest("dlx");

            foreach (var p in K2TestHelper.Processes)
            {
                string result = (!p.ProcessHasErrors) ? "Passed" : "Failed - " + p.ActivityExecutionError;
                Console.WriteLine("------------------------------------------------------------------------------");
                Console.WriteLine(string.Format("{0} ", " Description : " + p.Description));
                Console.WriteLine(string.Format("{0} ", " Status : " + result));
                Console.WriteLine(string.Format("{0} ", " Process Status : " + p.ProcessStatus));
                if (p.ProcessHasErrors) Console.WriteLine(string.Format("{0} ", " Error: " + p.ProcessError));
            }
        }

    }
}
