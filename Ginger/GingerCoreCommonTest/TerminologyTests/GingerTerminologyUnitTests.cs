﻿using Amdocs.Ginger.Common;
using Amdocs.Ginger.Repository;
using GingerCore;
using GingerTestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GingerCoreCommonTest.TerminologyTests
{
    [TestClass]
    public class GingerTerminologyUnitTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext TestContext)
        {
            //
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            //
        }

        [TestInitialize]
        public void TestInitialize()
        {
            // before every test
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            //after every test
        }

        [TestMethod]
        public void GingerDefaultTerm_TestActivities()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Default;
            eTermResKey termResourceKey = eTermResKey.Activities;
            
            //Act
            string termValue = GingerTerminology.GetTerminologyValue(termResourceKey);

            //Assert
            Assert.AreEqual("Activities", termValue);
        }

        [TestMethod]
        public void GingerGherkinTerm_TestActivities()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Gherkin;
            eTermResKey termResourceKey = eTermResKey.Activities;

            //Act
            string termValue = GingerTerminology.GetTerminologyValue(termResourceKey);

            //Assert
            Assert.AreEqual("Steps", termValue);
        }

        [TestMethod]
        public void GingerTestingTerm_TestActivities()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Testing;
            eTermResKey termResourceKey = eTermResKey.Activities;

            //Act
            string termValue = GingerTerminology.GetTerminologyValue(termResourceKey);

            //Assert
            Assert.AreEqual("Steps", termValue);
        }


        [TestMethod]
        public void GingerDefaultTerm_TestRunset()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Default;
            eTermResKey termResourceKey = eTermResKey.RunSet;

            //Act
            string termValue = GingerTerminology.GetTerminologyValue(termResourceKey);

            //Assert
            Assert.AreEqual("Run Set", termValue);
        }

        [TestMethod]
        public void GingerGherkinTerm_TestRunset()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Gherkin;
            eTermResKey termResourceKey = eTermResKey.RunSet;

            //Act
            string termValue = GingerTerminology.GetTerminologyValue(termResourceKey);

            //Assert
            Assert.AreEqual("Run Set", termValue);
        }

        [TestMethod]
        public void GingerTestingTerm_TestRunset()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Testing;
            eTermResKey termResourceKey = eTermResKey.RunSet;

            //Act
            string termValue = GingerTerminology.GetTerminologyValue(termResourceKey);

            //Assert
            Assert.AreEqual("Calendar", termValue);
        }


        [TestMethod]
        public void GingerDefaultTerm_TestBusinessFlows()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Default;
            eTermResKey termResourceKey = eTermResKey.BusinessFlows;

            //Act
            string termValue = GingerTerminology.GetTerminologyValue(termResourceKey);

            //Assert
            Assert.AreEqual("Business Flow Features", termValue);
        }

        [TestMethod]
        public void GingerGherkinTerm_TestBusinessFlows()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Gherkin;
            eTermResKey termResourceKey = eTermResKey.BusinessFlows;

            //Act
            string termValue = GingerTerminology.GetTerminologyValue(termResourceKey);

            //Assert
            Assert.AreEqual("Business Flow Features", termValue);
        }

        [TestMethod]
        public void GingerTestingTerm_TestBusinessFlows()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Testing;
            eTermResKey termResourceKey = eTermResKey.BusinessFlows;

            //Act
            string termValue = GingerTerminology.GetTerminologyValue(termResourceKey);

            //Assert
            Assert.AreEqual("Test Sets", termValue);
        }

        [TestMethod]
        public void GingerDefaultTerm_TestVariable()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Default;
            eTermResKey termResourceKey = eTermResKey.Variable;

            //Act
            string termValue = GingerTerminology.GetTerminologyValue(termResourceKey);

            //Assert
            Assert.AreEqual("Variable", termValue);
        }

        [TestMethod]
        public void GingerGherkinTerm_TestVariable()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Gherkin;
            eTermResKey termResourceKey = eTermResKey.Variable;

            //Act
            string termValue = GingerTerminology.GetTerminologyValue(termResourceKey);

            //Assert
            Assert.AreEqual("Parameter", termValue);
        }

        [TestMethod]
        public void GingerTestingTerm_TestVariable()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Testing;
            eTermResKey termResourceKey = eTermResKey.Variable;

            //Act
            string termValue = GingerTerminology.GetTerminologyValue(termResourceKey);

            //Assert
            Assert.AreEqual("Parameter", termValue);
        }

        //ActivitiesGroup

        [TestMethod]
        public void GingerDefaultTerm_TestActivitiesGroup()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Default;
            eTermResKey termResourceKey = eTermResKey.ActivitiesGroup;

            //Act
            string termValue = GingerTerminology.GetTerminologyValue(termResourceKey);

            //Assert
            Assert.AreEqual("Activities Group", termValue);
        }

        [TestMethod]
        public void GingerGherkinTerm_TestActivitiesGroup()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Gherkin;
            eTermResKey termResourceKey = eTermResKey.ActivitiesGroup;

            //Act
            string termValue = GingerTerminology.GetTerminologyValue(termResourceKey);

            //Assert
            Assert.AreEqual("Scenario", termValue);
        }

        [TestMethod]
        public void GingerTestingTerm_TestActivitiesGroup()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Testing;
            eTermResKey termResourceKey = eTermResKey.ActivitiesGroup;

            //Act
            string termValue = GingerTerminology.GetTerminologyValue(termResourceKey);

            //Assert
            Assert.AreEqual("Test Case", termValue);
        }


        [TestMethod]
        public void GingerGherkinTerm_TestBusinessFlowPrefix()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Gherkin;

            //Act
            string termResValue = GingerDicser.GetTermResValue(eTermResKey.BusinessFlow, "TestPre");

            //Assert
            Assert.AreEqual("TestPre Business Flow Feature", termResValue);
        }


        [TestMethod]
        public void GingerGherkinTerm_TestBusinessFlowSuffix()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Gherkin;

            //Act
            string termResValue = GingerDicser.GetTermResValue(eTermResKey.BusinessFlow, "", "TestSuffix");

            //Assert
            Assert.AreEqual("Business Flow Feature TestSuffix", termResValue);
        }

        [TestMethod]
        public void GingerGherkinTerm_TestBusinessFlowCase()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Gherkin;

            //Act
            string termResValue = GingerDicser.GetTermResValue(eTermResKey.BusinessFlow, "", "", true);

            //Assert
            Assert.AreEqual("Business Flow Feature".ToUpper(), termResValue);
        }

        [TestMethod]
        public void GingerGherkinTerm_TestBusinessFlowPrefixSuffix()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Gherkin;

            //Act
            string termResValue = GingerDicser.GetTermResValue(eTermResKey.BusinessFlow, "TestPre", "TestSuffix", false);

            //Assert
            Assert.AreEqual("TestPre Business Flow Feature TestSuffix", termResValue);
        }

        [TestMethod]
        public void GingerGherkinTerm_TestBusinessFlowPrefixSuffixCase()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Gherkin;

            //Act
            string termResValue = GingerDicser.GetTermResValue(eTermResKey.BusinessFlow, "TestPre", "TestSuffix", true);

            //Assert
            Assert.AreEqual("TestPre Business Flow Feature TestSuffix".ToUpper(), termResValue);
        }

        [TestMethod]
        public void GingerTestingTerm_TestBusinessFlowPrefix()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Testing;

            //Act
            string termResValue = GingerDicser.GetTermResValue(eTermResKey.BusinessFlow, "TestPre");

            //Assert
            Assert.AreEqual("TestPre Test Set", termResValue);
        }


        [TestMethod]
        public void GingerTestingTerm_TestBusinessFlowSuffix()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Testing;

            //Act
            string termResValue = GingerDicser.GetTermResValue(eTermResKey.BusinessFlow, "", "TestSuffix");

            //Assert
            Assert.AreEqual("Test Set TestSuffix", termResValue);
        }

        [TestMethod]
        public void GingerTestingTerm_TestBusinessFlowCase()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Testing;

            //Act
            string termResValue = GingerDicser.GetTermResValue(eTermResKey.BusinessFlow, "", "", true);

            //Assert
            Assert.AreEqual("Test Set".ToUpper(), termResValue);
        }

        [TestMethod]
        public void GingerTestingTerm_TestBusinessFlowPrefixSuffix()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Testing;

            //Act
            string termResValue = GingerDicser.GetTermResValue(eTermResKey.BusinessFlow, "TestPre", "TestSuffix", false);

            //Assert
            Assert.AreEqual("TestPre Test Set TestSuffix", termResValue);
        }

        [TestMethod]
        public void GingerTestingTerm_TestBusinessFlowPrefixSuffixCase()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Testing;

            //Act
            string termResValue = GingerDicser.GetTermResValue(eTermResKey.BusinessFlow, "TestPre", "TestSuffix", true);

            //Assert
            Assert.AreEqual("TestPre Test Set TestSuffix".ToUpper(), termResValue);
        }


        [TestMethod]
        public void GingerGherkinTerm_TestActivityPrefix()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Testing;

            //Act
            string termResValue = GingerDicser.GetTermResValue(eTermResKey.Activity, "TestPre");

            //Assert
            Assert.AreEqual("TestPre Step", termResValue);
        }


        [TestMethod]
        public void GingerGherkinTerm_TestActivitySuffix()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Gherkin;

            //Act
            string termResValue = GingerDicser.GetTermResValue(eTermResKey.Activity, "", "TestSuffix");

            //Assert
            Assert.AreEqual("Step TestSuffix", termResValue);
        }

        [TestMethod]
        public void GingerGherkinTerm_TestActivityCase()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Gherkin;

            //Act
            string termResValue = GingerDicser.GetTermResValue(eTermResKey.Activity, "", "", true);

            //Assert
            Assert.AreEqual("Step".ToUpper(), termResValue);
        }

        [TestMethod]
        public void GingerGherkinTerm_TestActivityPrefixSuffix()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Gherkin;

            //Act
            string termResValue = GingerDicser.GetTermResValue(eTermResKey.Activity, "TestPre", "TestSuffix", false);

            //Assert
            Assert.AreEqual("TestPre Step TestSuffix", termResValue);
        }

        [TestMethod]
        public void GingerGherkinTerm_TestActivityPrefixSuffixCase()
        {
            //Arrange
            GingerTerminology.SET_TERMINOLOGY_TYPE = eTerminologyDicsType.Gherkin;

            //Act
            string termResValue = GingerDicser.GetTermResValue(eTermResKey.Activity, "TestPre", "TestSuffix", true);

            //Assert
            Assert.AreEqual("TestPre Step TestSuffix".ToUpper(), termResValue);
        }



    }

} //namespace
