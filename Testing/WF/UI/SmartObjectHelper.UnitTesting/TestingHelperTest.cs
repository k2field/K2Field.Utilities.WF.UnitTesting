using SourceCode.Field.Workflow.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SourceCode.Field.Core.Helper;
using System.Collections.Generic;

namespace SmartObjectHelper.UnitTesting
{
    
    
    /// <summary>
    ///This is a test class for TestingHelperTest and is intended
    ///to contain all TestingHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TestingHelperTest
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
        ///A test for prepareDataFields
        ///</summary>
        [TestMethod()]
        [DeploymentItem("SourceCode.Field.Workflow.Testing.dll")]
        public void prepareDataFieldsTest()
        {
            TestingHelper_Accessor target = new TestingHelper_Accessor(); // TODO: Initialize to an appropriate value
            Dictionary<string, CoreDataField> dataFields = null; 
            target.prepareDataFields(dataFields);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
