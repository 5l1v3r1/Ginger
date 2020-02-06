﻿using amdocs.ginger.GingerCoreNET;
using Amdocs.Ginger.CoreNET.Execution;
using Amdocs.Ginger.CoreNET.RosLynLib;
using Amdocs.Ginger.CoreNET.RunLib;
using Amdocs.Ginger.CoreNET.RunLib.CLILib;
using Amdocs.Ginger.Repository;
using CommandLine;
using Ginger.Run;
using GingerCore.Environments;
using GingerTestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Amdocs.Ginger.Common;
using Ginger.Run.RunSetActions;
using Amdocs.Ginger.CoreNET.RunLib.DynamicExecutionLib;
using Ginger.ExecuterService.Contracts.V1.ExecutionConfiguration;

namespace WorkspaceHold
{
    [Level3]
    [TestClass]
    public class CLITest
    {
        // TODO: run one by one as it used same run exc
        static string mTempFolder;
        static string mSolutionFolder;

        [ClassInitialize]
        public static void ClassInitialize(TestContext TestContext)
        {
            mTempFolder = TestResources.GetTempFolder(nameof(CLITest));
            mSolutionFolder = Path.Combine(TestResources.GetTestResourcesFolder(@"Solutions"), "CLI");

            // Hook console message
            Reporter.logToConsoleEvent += ConsoleMessageEvent;
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {

        }


        [TestInitialize]
        public void TestInitialize()
        {
            WorkSpace.LockWS();
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            WorkSpace.RelWS();            
        }


        [TestMethod]
        public void OLDCLIConfigTest()
        {
            
            // Arrange
            PrepareForCLICreationAndExecution();
            // Create config file
            CLIHelper cLIHelper = new CLIHelper();
            cLIHelper.RunAnalyzer = true;
            cLIHelper.ShowAutoRunWindow = false;
            cLIHelper.DownloadUpgradeSolutionFromSourceControl = false;

            RunSetAutoRunConfiguration runSetAutoRunConfiguration = new RunSetAutoRunConfiguration(WorkSpace.Instance.Solution, WorkSpace.Instance.RunsetExecutor, cLIHelper);
            runSetAutoRunConfiguration.ConfigFileFolderPath = mTempFolder;
            runSetAutoRunConfiguration.SelectedCLI = new CLIConfigFile();
            runSetAutoRunConfiguration.CreateContentFile();

            // Act            
            CLIProcessor CLI = new CLIProcessor();
            CLI.ExecuteArgs(new string[] { "ConfigFile=" + runSetAutoRunConfiguration.ConfigFileFullPath });

            // Assert            
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].RunStatus, Amdocs.Ginger.CoreNET.Execution.eRunStatus.Passed, "BF RunStatus=Passed");
            
        }

        [TestMethod]
        public void OLDCLIConfigRegressionTest()
        {
            
            //Arrange                
            //Create config file         
            PrepareForCLICreationAndExecution();
            string txt = string.Format("Solution={0}", mSolutionFolder) + Environment.NewLine;
            txt += string.Format("Env={0}", "Default") + Environment.NewLine;
            txt += string.Format("RunSet={0}", "Default Run Set") + Environment.NewLine;
            txt += string.Format("RunAnalyzer={0}", "True") + Environment.NewLine;
            txt += string.Format("ShowAutoRunWindow={0}", "False") + Environment.NewLine;
            string configFile = TestResources.GetTempFile("runset1.ginger.config");
            System.IO.File.WriteAllText(configFile, txt);

            // Act            
            CLIProcessor CLI = new CLIProcessor();
            CLI.ExecuteArgs(new string[] { "ConfigFile=" + configFile });

            // Assert            
            Assert.AreEqual(eRunStatus.Passed, WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].RunStatus, "BF RunStatus=Passed");
            
        }


        [TestMethod]
        public void CLIVersion()
        {

            //Arrange                            
            PrepareForCLICreationAndExecution();

            // Act            
            CLIProcessor CLI = new CLIProcessor();
            CLI.ExecuteArgs(new string[] { "-t" });

            // Assert            
            // ????

        }

        [TestMethod]
        public void CLIBadParams()
        {
            lock (mConsoleMessages)
            {
                //Arrange                            
                mConsoleMessages.Clear();
                CLIProcessor CLI = new CLIProcessor();

                // Act            
                CLI.ExecuteArgs(new string[] { "--blabla" });

                // Assert                            
                Assert.AreEqual(eLogLevel.ERROR, mConsoleMessages[0].LogLevel, "message loglevel is ERROR");
                Assert.AreEqual("Please fix the arguments and try again", mConsoleMessages[0].MessageToConsole, "console message");
            }

        }

        [TestMethod]
        public void CLIHelp()
        {
            lock (mConsoleMessages)
            {
                //Arrange                            
                mConsoleMessages.Clear();
                CLIProcessor CLI = new CLIProcessor();

                // Act            
                CLI.ExecuteArgs(new string[] { "help" });

                // Assert            
                Assert.AreEqual(1, mConsoleMessages.Count, "message count");
                Assert.IsTrue(mConsoleMessages[0].MessageToConsole.Contains("Ginger support"), "help message");
            }
        }

        [TestMethod]
        public void CLIGridWithoutParams()
        {
            lock (mConsoleMessages)
            {
                //Arrange                            
                mConsoleMessages.Clear();
                CLIProcessor CLI = new CLIProcessor();

                // Act            
                CLI.ExecuteArgs(new string[] { "grid" });

                // Assert            
                Assert.AreEqual(1, mConsoleMessages.Count, "There is 1 line of help");
                Assert.AreEqual(eLogLevel.INFO, mConsoleMessages[0].LogLevel, "message loglevel is ERROR");
                Assert.AreEqual("Starting Ginger Grid at port: 15001", mConsoleMessages[0].MessageToConsole, "console message");
            }

        }

        class ConsoleMessage
        {
            public eLogLevel LogLevel;
            public string MessageToConsole;
        }

        static List<ConsoleMessage> mConsoleMessages = new List<ConsoleMessage>();
        public static void ConsoleMessageEvent(eLogLevel logLevel, string messageToConsole)
        {
            mConsoleMessages.Add(new ConsoleMessage(){LogLevel = logLevel, MessageToConsole = messageToConsole});
        }        

        [TestMethod]
        public void CLIDynamicXMLTest()
        {
            // Arrange
            PrepareForCLICreationAndExecution();
            // Create config file
            CLIHelper cLIHelper = new CLIHelper();
            cLIHelper.RunAnalyzer = true;
            cLIHelper.ShowAutoRunWindow = false;
            cLIHelper.DownloadUpgradeSolutionFromSourceControl = false;

            RunSetAutoRunConfiguration runSetAutoRunConfiguration = new RunSetAutoRunConfiguration(WorkSpace.Instance.Solution, WorkSpace.Instance.RunsetExecutor, cLIHelper);
            runSetAutoRunConfiguration.ConfigFileFolderPath = mTempFolder;
            runSetAutoRunConfiguration.SelectedCLI = new CLIDynamicFile(CLIDynamicFile.eFileType.XML);
            runSetAutoRunConfiguration.CreateContentFile();

            // Act            
            CLIProcessor CLI = new CLIProcessor();
            CLI.ExecuteArgs(new string[] { "dynamic", "-f" ,runSetAutoRunConfiguration.ConfigFileFullPath });

            // Assert            
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].RunStatus, eRunStatus.Passed, "BF RunStatus=Passed");
        }


        /// <summary>
        /// Testing JSON config creation and execution
        /// </summary>
        [TestMethod]
        public void CLIDynamicJSON_CreateAndExecute_Test()
        {
            // Arrange
            PrepareForCLICreationAndExecution(runsetName:"Calc_Test");
            // Create config file
            CLIHelper cLIHelper = new CLIHelper();
            cLIHelper.RunAnalyzer = true;
            cLIHelper.ShowAutoRunWindow = false;
            cLIHelper.DownloadUpgradeSolutionFromSourceControl = false;
            
            RunSetAutoRunConfiguration runSetAutoRunConfiguration = new RunSetAutoRunConfiguration(WorkSpace.Instance.Solution, WorkSpace.Instance.RunsetExecutor, cLIHelper);
            runSetAutoRunConfiguration.ConfigFileFolderPath = mTempFolder;
            runSetAutoRunConfiguration.SelectedCLI = new CLIDynamicFile(CLIDynamicFile.eFileType.JSON);
            runSetAutoRunConfiguration.CreateContentFile();

            // Act            
            CLIProcessor CLI = new CLIProcessor();
            CLI.ExecuteArgs(new string[] { "dynamic", "-f", runSetAutoRunConfiguration.ConfigFileFullPath });

            // Assert        
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.RunSetConfig.Name, "Calc_Test", "Validating correct Run set was executed" );
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].Name, "Calculator_Test", "Validating correct Business Flow was executed");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].RunStatus, eRunStatus.Passed, "Validating BF execution Passed");
        }

        /// <summary>
        /// Testing JSON existing Runset with customized Values execution
        /// </summary>
        [TestMethod]
        public void CLIDynamicJSON_ExistingCustomized_IDsAndNames_Test()
        {
            // Arrange
            string jsonConfigFilePath = CreateTempJSONConfigFile(Path.Combine(TestResources.GetTestResourcesFolder("CLI"), "CLI-CustomExistingRunset.Ginger.AutoRunConfigs.json"), mSolutionFolder);

            // Act            
            CLIProcessor CLI = new CLIProcessor();
            CLI.ExecuteArgs(new string[] { "dynamic", "-f", jsonConfigFilePath });      

            // Assert        
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.RunSetConfig.Name, "Calc_Test", "Validating correct Run set was executed");

            //Envs Validation
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.RunsetExecutionEnvironment.Name, "Env1", "Validating correct customized Run set Environment");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].ProjEnvironment.Name, "Env2", "Validating correct customized Runner Environment");

            //Agent Validation
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].ApplicationAgents[0].AgentName, "IE", "Validating correct customized Runner Agent");

            //BF 1 Validation
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].Name, "Calculator_Test", "Validating correct Business Flow was executed");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].GetBFandActivitiesVariabeles(false).Where(x => x.Name == "DoMultiply?").FirstOrDefault().Value, "No", "Validating Customized BF level Selection List Variable");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].GetBFandActivitiesVariabeles(false).Where(x => x.Name == "SecondNum_ForSum").FirstOrDefault().Value, "44", "Validating Customized Activity level String Variable");           
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].RunStatus, eRunStatus.Passed, "Validating BF execution Passed");

            //BF 2 Validation (same instance of BF)
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[2].Name, "Calculator_Test", "Validating correct Business Flow was executed");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[2].GetBFandActivitiesVariabeles(false).Where(x => x.Name == "DoDivide?").FirstOrDefault().Value, "Yes", "Validating Customized BF level Selection List Variable");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[2].GetBFandActivitiesVariabeles(false).Where(x => x.Name == "SecondNum_ForDivide").FirstOrDefault().Value, "1", "Validating Customized Activity level String Variable");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[2].RunStatus, eRunStatus.Passed, "Validating BF execution Passed");

            Assert.AreEqual(((RunSetActionHTMLReportSendEmail)(WorkSpace.Instance.RunsetExecutor.RunSetConfig.RunSetActions[0])).MailTo, "bbb@amdocs.com", "Validating customized report mail Address");
            Assert.AreEqual(((RunSetActionHTMLReportSendEmail)(WorkSpace.Instance.RunsetExecutor.RunSetConfig.RunSetActions[0])).Subject, "Test44", "Validating customized report mail Subject");
        }

        /// <summary>
        /// Testing JSON existing Runset with customized Values execution only using items Names
        /// </summary>
        [TestMethod]
        public void CLIDynamicJSON_ExistingCustomized_OnlyNames_Test()
        {
            // Arrange
            string jsonConfigFilePath = CreateTempJSONConfigFile(Path.Combine(TestResources.GetTestResourcesFolder("CLI"), "CLI-CustomExistingRunset_OnlyNames.Ginger.AutoRunConfigs.json"), mSolutionFolder);

            // Act            
            CLIProcessor CLI = new CLIProcessor();
            CLI.ExecuteArgs(new string[] { "dynamic", "-f", jsonConfigFilePath });

            // Assert        
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.RunSetConfig.Name, "Calc_Test", "Validating correct Run set was executed");

            //Envs Validation
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.RunsetExecutionEnvironment.Name, "Env1", "Validating correct customized Run set Environment");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].ProjEnvironment.Name, "Env2", "Validating correct customized Runner Environment");

            //Agent Validation
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].ApplicationAgents[0].AgentName, "IE", "Validating correct customized Runner Agent");

            //BF 1 Validation
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].Name, "Calculator_Test", "Validating correct Business Flow was executed");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].GetBFandActivitiesVariabeles(false).Where(x => x.Name == "DoMultiply?").FirstOrDefault().Value, "No", "Validating Customized BF level Selection List Variable");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].GetBFandActivitiesVariabeles(false).Where(x => x.Name == "SecondNum_ForSum").FirstOrDefault().Value, "44", "Validating Customized Activity level String Variable");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].RunStatus, eRunStatus.Passed, "Validating BF execution Passed");

            //BF 2 Validation (same instance of BF)
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[2].Name, "Calculator_Test", "Validating correct Business Flow was executed");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[2].GetBFandActivitiesVariabeles(false).Where(x => x.Name == "DoDivide?").FirstOrDefault().Value, "Yes", "Validating Customized BF level Selection List Variable");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[2].GetBFandActivitiesVariabeles(false).Where(x => x.Name == "SecondNum_ForDivide").FirstOrDefault().Value, "1", "Validating Customized Activity level String Variable");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[2].RunStatus, eRunStatus.Passed, "Validating BF execution Passed");

            Assert.AreEqual(((RunSetActionHTMLReportSendEmail)(WorkSpace.Instance.RunsetExecutor.RunSetConfig.RunSetActions[0])).MailTo, "bbb@amdocs.com", "Validating customized report mail Address");
            Assert.AreEqual(((RunSetActionHTMLReportSendEmail)(WorkSpace.Instance.RunsetExecutor.RunSetConfig.RunSetActions[0])).Subject, "Test44", "Validating customized report mail Subject");
        }

        /// <summary>
        /// Testing JSON existing Runset with customized Values execution while only items IDs is provided
        /// </summary>
        [Ignore]
        [TestMethod]
        public void CLIDynamicJSON_ExistingCustomized_OnlyIDs_Test()
        {
            // Arrange
            string jsonConfigFilePath = CreateTempJSONConfigFile(Path.Combine(TestResources.GetTestResourcesFolder("CLI"), "CLI-CustomExistingRunset_OnlyIDs.Ginger.AutoRunConfigs.json"), mSolutionFolder);

            // Act            
            CLIProcessor CLI = new CLIProcessor();
            CLI.ExecuteArgs(new string[] { "dynamic", "-f", jsonConfigFilePath });

            // Assert        
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.RunSetConfig.Name, "Calc_Test", "Validating correct Run set was executed");

            //Envs Validation
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.RunsetExecutionEnvironment.Name, "Env1", "Validating correct customized Run set Environment");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].ProjEnvironment.Name, "Env2", "Validating correct customized Runner Environment");

            //Agent Validation
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].ApplicationAgents[0].AgentName, "IE", "Validating correct customized Runner Agent");

            //BF 1 Validation
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].Name, "Calculator_Test", "Validating correct Business Flow was executed");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].GetBFandActivitiesVariabeles(false).Where(x => x.Name == "DoMultiply?").FirstOrDefault().Value, "No", "Validating Customized BF level Selection List Variable");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].GetBFandActivitiesVariabeles(false).Where(x => x.Name == "SecondNum_ForSum").FirstOrDefault().Value, "44", "Validating Customized Activity level String Variable");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].RunStatus, eRunStatus.Passed, "Validating BF execution Passed");

            //BF 2 Validation (same instance of BF)
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[2].Name, "Calculator_Test", "Validating correct Business Flow was executed");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[2].GetBFandActivitiesVariabeles(false).Where(x => x.Name == "DoDivide?").FirstOrDefault().Value, "Yes", "Validating Customized BF level Selection List Variable");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[2].GetBFandActivitiesVariabeles(false).Where(x => x.Name == "SecondNum_ForDivide").FirstOrDefault().Value, "1", "Validating Customized Activity level String Variable");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[2].RunStatus, eRunStatus.Passed, "Validating BF execution Passed");

            Assert.AreEqual(((RunSetActionHTMLReportSendEmail)(WorkSpace.Instance.RunsetExecutor.RunSetConfig.RunSetActions[0])).MailTo, "bbb@amdocs.com", "Validating customized report mail Address");
            Assert.AreEqual(((RunSetActionHTMLReportSendEmail)(WorkSpace.Instance.RunsetExecutor.RunSetConfig.RunSetActions[0])).Subject, "Test44", "Validating customized report mail Subject");
        }

        /// <summary>
        /// Testing JSON non existing Runset 
        /// </summary>   
        [Ignore]
        [TestMethod]
        public void CLIDynamicJSON_NotExist_Test()
        {
            // Arrange
            string jsonConfigFilePath = CreateTempJSONConfigFile(Path.Combine(TestResources.GetTestResourcesFolder("CLI"), "CLI-NotExisting.Ginger.AutoRunConfigs.json"), mSolutionFolder);

            // Act            
            CLIProcessor CLI = new CLIProcessor();
            CLI.ExecuteArgs(new string[] { "dynamic", "-f", jsonConfigFilePath });

            // Assert        
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.RunSetConfig.Name, "Calc_Test_Dynamic", "Validating correct Run set was executed");

            //Runner 
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].Name, "Runner Dynamic", "Validating correct Runner Name");

            //Envs Validation
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.RunsetExecutionEnvironment.Name, "Env1", "Validating correct Run set Environment");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].ProjEnvironment.Name, "Env2", "Validating correct Runner Environment");

            //Agent Validation
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].ApplicationAgents[0].AgentName, "IE", "Validating correct Runner Agent");

            //BF 1 Validation
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].Name, "Calculator_Test", "Validating correct Business Flow was executed");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].GetBFandActivitiesVariabeles(false).Where(x => x.Name == "DoMultiply?").FirstOrDefault().Value, "No", "Validating Customized BF level Selection List Variable");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].GetBFandActivitiesVariabeles(false).Where(x => x.Name == "SecondNum_ForSum").FirstOrDefault().Value, "44", "Validating Customized Activity level String Variable");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].RunStatus, eRunStatus.Passed, "Validating BF execution Passed");

            //BF 2 Validation (same instance of BF)
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[2].Name, "Calculator_Test", "Validating correct Business Flow was executed");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[2].GetBFandActivitiesVariabeles(false).Where(x => x.Name == "DoDivide?").FirstOrDefault().Value, "No", "Validating Customized BF level Selection List Variable");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[2].GetBFandActivitiesVariabeles(false).Where(x => x.Name == "SecondNum_ForDivide").FirstOrDefault().Value, "1", "Validating Customized Activity level String Variable");
            Assert.AreEqual(WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[2].RunStatus, eRunStatus.Stopped, "Validating BF execution Stopped");

            Assert.AreEqual(((RunSetActionHTMLReportSendEmail)(WorkSpace.Instance.RunsetExecutor.RunSetConfig.RunSetActions[0])).MailTo, "menik@amdocs.com", "Validating report mail Address");
            Assert.AreEqual(((RunSetActionHTMLReportSendEmail)(WorkSpace.Instance.RunsetExecutor.RunSetConfig.RunSetActions[0])).Subject, "AAA", "Validating report mail Subject");
        }

        [TestMethod]
        public void OLDCLIDynamicRegressionTest()
        {
         
                //Arrange
                //Create config file       
                string fileName = Path.Combine(TestResources.GetTestResourcesFolder("CLI"), "CLI-Default Run Set.Ginger.AutoRunConfigs.xml");
                string dynamicXML = System.IO.File.ReadAllText(fileName);
                dynamicXML = dynamicXML.Replace("SOLUTION_PATH", mSolutionFolder);
                string configFile = TestResources.GetTempFile("CLI-Default Run Set.Ginger.AutoRunConfigs.xml");
                System.IO.File.WriteAllText(configFile, dynamicXML);

                // Act            
                CLIProcessor CLI = new CLIProcessor();
                CLI.ExecuteArgs(new string[] { "Dynamic=" + configFile });

                // Assert            
                Assert.AreEqual(eRunStatus.Passed, WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].RunStatus, "BF RunStatus=Passed");
        }

        [TestMethod]
        public void OLDCLIConfigFileTest()
        {
            // Arrange
            PrepareForCLICreationAndExecution();
            // Create config file
            CLIHelper cLIHelper = new CLIHelper();
            cLIHelper.RunAnalyzer = true;
            cLIHelper.ShowAutoRunWindow = false;
            cLIHelper.DownloadUpgradeSolutionFromSourceControl = false;

            RunSetAutoRunConfiguration runSetAutoRunConfiguration = new RunSetAutoRunConfiguration(WorkSpace.Instance.Solution, WorkSpace.Instance.RunsetExecutor, cLIHelper);
            runSetAutoRunConfiguration.ConfigFileFolderPath = mTempFolder;
            runSetAutoRunConfiguration.SelectedCLI = new CLIConfigFile();
            runSetAutoRunConfiguration.CreateContentFile();

            // Act            
            CLIProcessor CLI = new CLIProcessor();
            CLI.ExecuteArgs(new string[] { "ConfigFile=" + runSetAutoRunConfiguration.ConfigFileFullPath });

            // Assert            
            Assert.AreEqual(eRunStatus.Passed, WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].RunStatus, "BF RunStatus=Passed");
        }

        [TestMethod]
        public void CLIScriptRegressionTest()
        {
            //Arrange
            // PrepareForCLIExecution();
            // Create config file
            string scriptFile = TestResources.GetTempFile("runset1.ginger.script");
            string jsonFileName = TestResources.GetTempFile("runset.json");
            string txt = "int i=1;" + Environment.NewLine;
            txt += "i++;" + Environment.NewLine;
            txt += nameof(GingerScriptGlobals.OpenSolution) + "(@\"" + mSolutionFolder + "\");" + Environment.NewLine;
            txt += nameof(GingerScriptGlobals.OpenRunSet) + "(\"Default Run Set\", \"Default\");" + Environment.NewLine;    // Runset, env
            txt += nameof(GingerScriptGlobals.CreateExecutionSummaryJSON) + "(@\"" + jsonFileName + "\");" + Environment.NewLine;    // summary json
            txt += "i" + Environment.NewLine;  // script rc
            File.WriteAllText(scriptFile, txt);

            // Act
            CLIProcessor CLI = new CLIProcessor();
            CLI.ExecuteArgs(new string[] { "script", "-f" , scriptFile });

            // Assert
            Assert.AreEqual(eRunStatus.Passed, WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].RunStatus, "BF RunStatus=Passed");
        }

        [TestMethod]
        public void CLIArgsTest()
        {
            // Arrange
            PrepareForCLICreationAndExecution();
            // Create config file
            CLIHelper cLIHelper = new CLIHelper();
            cLIHelper.RunAnalyzer = true;
            cLIHelper.ShowAutoRunWindow = false;
            cLIHelper.DownloadUpgradeSolutionFromSourceControl = false;

            RunSetAutoRunConfiguration runSetAutoRunConfiguration = new RunSetAutoRunConfiguration(WorkSpace.Instance.Solution, WorkSpace.Instance.RunsetExecutor, cLIHelper);
            runSetAutoRunConfiguration.ConfigFileFolderPath = mTempFolder;
            runSetAutoRunConfiguration.SelectedCLI = new CLIArgs();

            // Act            
            CLIProcessor CLI = new CLIProcessor();
            string[] args = CommandLineToStringArray(runSetAutoRunConfiguration.CLIContent).ToArray();
            CLI.ExecuteArgs(args);

            // Assert            
            Assert.AreEqual(eRunStatus.Passed, WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].RunStatus, "BF RunStatus=Passed");
        }

        [TestMethod]
        public void CLIArgsWithDoNotAnalyzeTest()
        {
            //Arrange
            PrepareForCLICreationAndExecution();
            // Create args
            string[] args = { "run", "--solution", mSolutionFolder, "--env", "Default", "--runset", "Default Run Set", "--do-not-analyze"};
            
            // Act            
            CLIProcessor CLI = new CLIProcessor();
            CLI.ExecuteArgs(args);

            // Assert            
            Assert.AreEqual(eRunStatus.Passed, WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].RunStatus, "BF RunStatus=Passed");

            // TODO: test analyze was run !!!!
        }


        private void PrepareForCLICreationAndExecution(string runsetName= "Default Run Set", string envName = "Default")
        {            
            WorkSpace.Instance.OpenSolution(mSolutionFolder);
            SolutionRepository SR = WorkSpace.Instance.SolutionRepository;
            RunsetExecutor runsetExecutor = new RunsetExecutor();
            runsetExecutor.RunsetExecutionEnvironment = (from x in SR.GetAllRepositoryItems<ProjEnvironment>() where x.Name == envName select x).SingleOrDefault();
            runsetExecutor.RunSetConfig = (from x in SR.GetAllRepositoryItems<RunSetConfig>() where x.Name == runsetName select x).SingleOrDefault();
            WorkSpace.Instance.RunsetExecutor = runsetExecutor;
            WorkSpace.Instance.RunsetExecutor.InitRunners();
        }

        [TestMethod]
        public void NewCLIArgsRegressionTest()
        {            
            //Arrange
            PrepareForCLICreationAndExecution();

            // Create args
            List<string> args = new List<string>();

            args.Add("run");

            args.Add("--solution");
            args.Add(mSolutionFolder);

            args.Add("--env");
            args.Add("Default");

            args.Add("--runset");
            args.Add("Default Run Set");

            args.Add("--do-not-analyze");

            args.Add("--showui");            

            // Act            
            CLIProcessor CLI = new CLIProcessor();
            CLI.ExecuteArgs(args.ToArray());

            // Assert            
            Assert.AreEqual(eRunStatus.Passed, WorkSpace.Instance.RunsetExecutor.Runners[0].BusinessFlows[0].RunStatus, "BF RunStatus=Passed");
            
        }

        [TestMethod]
        public void NewCreateCLIArgs()
        {
            //Arrange
            RunOptions options = new RunOptions() { Environment = "env1", DoNotAnalyze = true, Runset = "rs1" };


            // Act            
            var arguments = CommandLine.Parser.Default.FormatCommandLine<RunOptions>(options);

            // Assert            
            Assert.IsTrue(arguments.StartsWith("run"), "arguments Starts With run");
            Assert.IsTrue(arguments.Contains("--env env1"), "arguments Contains --env env1");
            Assert.IsTrue(arguments.Contains("--runset rs1"), "arguments Contains --runset rs1");
            Assert.IsTrue(arguments.Contains("--do-not-analyze"), "arguments Contains --do-not-analyze");                   
        }

        [TestMethod]
        public void ParseStringToArgs()
        {
            //Arrange
            string s = @"run -s c:\123 -e env1";

            // Act            
            var arguments = CommandLineToStringArray(s);

            // Assert            
            Assert.IsTrue(arguments.First() == "run");
            Assert.IsTrue(arguments.Contains("-s"), "arguments Contains -s");
            Assert.IsTrue(arguments.Contains(@"c:\123"), @"arguments Contains c:\123");
            Assert.IsTrue(arguments.Contains("-e"), "arguments Contains -e");
            Assert.IsTrue(arguments.Contains("env1"), "arguments Contains env1");
        }

        // Parse a command line with multiple switches to string list - used for test only!
        public static IEnumerable<string> CommandLineToStringArray(string commandLine)
        {
            if (string.IsNullOrWhiteSpace(commandLine))
                yield break;

            var sb = new StringBuilder();
            bool inQuote = false;
            foreach (char c in commandLine)
            {
                if (c == '"' && !inQuote)
                {
                    inQuote = true;
                    continue;
                }

                if (c != '"' && !(char.IsWhiteSpace(c) && !inQuote))
                {
                    sb.Append(c);
                    continue;
                }

                if (sb.Length > 0)
                {
                    var result = sb.ToString();
                    sb.Clear();
                    inQuote = false;
                    yield return result;
                }
            }

            if (sb.Length > 0)
                yield return sb.ToString();
        }


        private string CreateTempJSONConfigFile(string resourceJSONFilePath, string solutionPath)
        {
            GingerExecConfig config = DynamicExecutionManager.DeserializeDynamicExecutionFromJSON(System.IO.File.ReadAllText(resourceJSONFilePath));
            config.SolutionLocalPath = solutionPath;            
            string tempJSONConfigFilePath = TestResources.GetTempFile(System.IO.Path.GetFileName(resourceJSONFilePath));
            System.IO.File.WriteAllText(tempJSONConfigFilePath, DynamicExecutionManager.SerializeDynamicExecutionToJSON(config));

            return tempJSONConfigFilePath;
        }
    }
}
