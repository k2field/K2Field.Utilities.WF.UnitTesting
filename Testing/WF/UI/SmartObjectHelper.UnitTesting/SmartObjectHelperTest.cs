using SourceCode.Field.Workflow.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SmartObjectHelper.UnitTesting
{
    
    
    /// <summary>
    ///This is a test class for SmartObjectHelperTest and is intended
    ///to contain all SmartObjectHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SmartObjectHelperTest
    {


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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for SplitPathIntoFolderAndName
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SourceCode.Field.Workflow.Testing.dll")]
        public void SplitPathIntoFolderAndNameTest()
        {
            string fullPath = @"Underwriting\Quote\New Quote";
            string folderName = string.Empty; // TODO: Initialize to an appropriate value
            string folderNameExpected = @"Underwriting\Quote"; // TODO: Initialize to an appropriate value
            string processName = string.Empty; // TODO: Initialize to an appropriate value
            string processNameExpected = "New Quote"; // TODO: Initialize to an appropriate value
            SmartObjectHelper_Accessor.SplitPathIntoFolderAndName(fullPath, out folderName, out processName);
            Assert.AreEqual(folderNameExpected, folderName);
            Assert.AreEqual(processNameExpected, processName);
        }
    }
}
