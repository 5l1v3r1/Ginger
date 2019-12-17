#region License
/*
Copyright © 2014-2019 European Support Limited

Licensed under the Apache License, Version 2.0 (the "License")
you may not use this file except in compliance with the License.
You may obtain a copy of the License at 

http://www.apache.org/licenses/LICENSE-2.0 

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS, 
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
See the License for the specific language governing permissions and 
limitations under the License. 
*/
#endregion

using amdocs.ginger.GingerCoreNET;
using Amdocs.Ginger.Common;
using Amdocs.Ginger.CoreNET.Execution;
using Amdocs.Ginger.CoreNET.Repository;
using Amdocs.Ginger.CoreNET.Run.RunSetActions;
using Amdocs.Ginger.CoreNET.RunLib;
using Amdocs.Ginger.CoreNET.RunLib.CLILib;
using Amdocs.Ginger.Repository;
using Ginger.Run;
using Ginger.Run.RunSetActions;
using Ginger.SolutionGeneral;
using GingerCore;
using GingerCore.Actions;
using GingerCore.Environments;
using GingerCore.Platforms;
using GingerCore.Variables;
using GingerCoreNET.SolutionRepositoryLib.RepositoryObjectsLib.PlatformsLib;
using GingerTestHelper;
using IWshRuntimeLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace UnitTests.NonUITests.GingerRunnerTests
{
    [TestClass]
    [Level1]
    public class GingerRunnerTest
    {
        static BusinessFlow mBF;
        static GingerRunner mGR;
        static SolutionRepository SR;
        static Solution solution;
        static ProjEnvironment environment;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            mBF = new BusinessFlow();
            mBF.Name = "BF Test Fire Fox";
            mBF.Active = true;

            Activity activity = new Activity();
            mBF.AddActivity(activity);

            ActDummy action1 = new ActDummy();
            ActDummy action2 = new ActDummy();

            mBF.Activities[0].Acts.Add(action1);
            mBF.Activities[0].Acts.Add(action2);

            Platform p = new Platform();
            p.PlatformType = ePlatformType.Web;
            mBF.TargetApplications.Add(new TargetApplication() { AppName = "SCM" });

            VariableString v1 = new VariableString() { Name = "v1", InitialStringValue = "1" };
            mBF.AddVariable(v1);

            mGR = new GingerRunner();
            mGR.Name = "Test Runner";
            mGR.CurrentSolution = new Ginger.SolutionGeneral.Solution();

            environment = new ProjEnvironment();
            environment.Name = "Default";
            mBF.Environment = environment.Name;


            Agent a = new Agent();
            //a.DriverType = Agent.eDriverType.SeleniumFireFox;//have known firefox issues with selenium 3
            a.DriverType = Agent.eDriverType.SeleniumChrome;

            mGR.SolutionAgents = new ObservableList<Agent>();
            mGR.SolutionAgents.Add(a);
            // p2.Agent = a;

            mGR.ApplicationAgents.Add(new ApplicationAgent() { AppName = "SCM", Agent = a });
            mGR.SolutionApplications = new ObservableList<ApplicationPlatform>();
            mGR.SolutionApplications.Add(new ApplicationPlatform() { AppName = "SCM", Platform = ePlatformType.Web, Description = "New application" });
            mGR.BusinessFlows.Add(mBF);
            mGR.SpecificEnvironmentName = environment.Name;
            mGR.UseSpecificEnvironment = false;

            string path = Path.Combine(TestResources.GetTestResourcesFolder(@"Solutions" +Path.DirectorySeparatorChar + "BasicSimple"));
            string solutionFile = System.IO.Path.Combine(path, @"Ginger.Solution.xml");
            solution = Solution.LoadSolution(solutionFile);
            SR = GingerSolutionRepository.CreateGingerSolutionRepository();
            SR.Open(path);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
           
        }


        //[Ignore]
        //[TestMethod]  [Timeout(60000)]
        //public void SCM_Login()
        //{

        //    ////Arrange

        //    //ResetBusinessFlow();

        //    //// mGR.SetSpeed(1);

        //    //Activity a1 = new Activity();
        //    //a1.Active = true;
        //    //a1.TargetApplication = "SCM";
        //    //mBF.Activities.Add(a1);

        //    //ActGotoURL act1 = new ActGotoURL() { LocateBy = eLocateBy.NA, Value = "https://ginger-automation.github.io/test.html", Active = true };
        //    //a1.Acts.Add(act1);

        //    //ActTextBox act2 = new ActTextBox() { LocateBy = eLocateBy.ByID, LocateValue = "UserName", Value = "Yaron", TextBoxAction = ActTextBox.eTextBoxAction.SetValue, Active = true };
        //    //a1.Acts.Add(act2);

        //    //ActTextBox act3 = new ActTextBox() { LocateBy = eLocateBy.ByID, LocateValue = "Password", Value = "123456", TextBoxAction = ActTextBox.eTextBoxAction.SetValue, Active = true };
        //    //a1.Acts.Add(act3);

        //    //ActSubmit act4 = new ActSubmit() { LocateBy = eLocateBy.ByValue, LocateValue = "Log in", Active = true };
        //    //a1.Acts.Add(act4);

        //    //VariableString v1 = (VariableString)mBF.GetVariable("v1");
        //    //v1.Value = "123";

        //    ////Act            
        //    //mGR.RunRunner();
        //    //// mGR.CurrentBusinessFlow = mBF;
        //    //// mGR.RunActivity(a1);

        //    ////Assert
        //    //Assert.AreEqual(mBF.RunStatus, eRunStatus.Passed);
        //    //Assert.AreEqual(a1.Status, eRunStatus.Passed);
        //    //Assert.AreEqual(act1.Status, eRunStatus.Passed);
        //    //Assert.AreEqual(act2.Status, eRunStatus.Passed);
        //    //Assert.AreEqual(act3.Status, eRunStatus.Passed);
        //    //Assert.AreEqual(act4.Status, eRunStatus.Passed);

        //    //Assert.AreEqual(v1.Value, "123");
        //}


        //// Test the time to enter data into text box
        //[Ignore]
        //[TestMethod]  [Timeout(60000)]
        //public void SpeedTest()
        //{
        //    ////Arrange
        //    //ResetBusinessFlow();

        //    //Activity a0 = new Activity();
        //    //a0.Active = true;

        //    //ActGotoURL act1 = new ActGotoURL() { LocateBy = eLocateBy.NA, Value = "https://ginger-automation.github.io/test.html", Active = true };
        //    //a0.Acts.Add(act1);

        //    //mBF.Activities.Add(a0);

        //    //Activity a1 = new Activity();
        //    //a1.Active = true;
        //    //mBF.Activities.Add(a1);

        //    //for (int i = 1; i < 10; i++)
        //    //{
        //    //    ActTextBox act2 = new ActTextBox() { LocateBy = eLocateBy.ByID, LocateValue = "UserName", Value = "Yaron", TextBoxAction = ActTextBox.eTextBoxAction.SetValue, Active = true };
        //    //    a1.Acts.Add(act2);

        //    //    ActTextBox act3 = new ActTextBox() { LocateBy = eLocateBy.ByID, LocateValue = "Password", Value = "123456", TextBoxAction = ActTextBox.eTextBoxAction.SetValue, Active = true };
        //    //    a1.Acts.Add(act3);
        //    //}

        //    //mGR.RunRunner();

        //    ////Assert
        //    //Assert.AreEqual(mBF.RunStatus, eRunStatus.Passed, "mBF.RunStatus");
        //    //Assert.AreEqual(mBF.Activities.Count(), (from x in mBF.Activities where x.Status == eRunStatus.Passed select x).Count(), "All activities should Passed");
        //    //Assert.IsTrue(a1.Elapsed < 10000, "a1.Elapsed Time less than 10000 ms");
        //}


        //private void ResetBusinessFlow()
        //{
        //    mBF.Activities.Clear();
        //    mBF.RunStatus = eRunStatus.Pending;
        //}

        //[Ignore]
        //[TestMethod]  [Timeout(60000)]
        //public void TestVariableResetIssue()
        //{
        //    ////This was a tricky bug not repro every time.
        //    //// the issue was when seeting Biz flow for Agent a reset vars happened.


        //    ////Arrange
        //    //ResetBusinessFlow();

        //    //Activity a1 = new Activity();
        //    //a1.Active = true;
        //    //mBF.Activities.Add(a1);

        //    //ActGotoURL act1 = new ActGotoURL() { LocateBy = eLocateBy.NA, Value = "https://ginger-automation.github.io/test.html", Active = true };
        //    //a1.Acts.Add(act1);

        //    //// Not happening with dummy
        //    ////ActDummy act1 = new ActDummy();
        //    ////a1.Acts.Add(act1);

        //    //VariableString v1 = (VariableString)mBF.GetVariable("v1");
        //    //v1.Value = "123";

        //    ////Act            
        //    //mGR.RunRunner();

        //    ////Assert
        //    //Assert.AreEqual(mBF.RunStatus, eRunStatus.Passed);
        //    //Assert.AreEqual(a1.Status, eRunStatus.Passed);

        //    //Assert.AreEqual(v1.Value, "123");  // <<< the importnat part as with this defect it turned to "1" - initial val
        //}


        [TestMethod]
        [Timeout(60000)]
        public void TestRunsetConfigBFVariables()
        {
            //Arrange
            ObservableList<BusinessFlow> bfList = SR.GetAllRepositoryItems<BusinessFlow>();
            BusinessFlow BF1 = bfList[0];

            ObservableList<Activity> activityList = BF1.Activities;

            Activity activity = activityList[0];

            ActDummy act1 = new ActDummy() { Value = "", Active = true };
            activity.Acts.Add(act1);

            VariableString v1 = new VariableString() { Name = "v1", InitialStringValue = "aaa" };
            BF1.AddVariable(v1);

            BF1.Active = true;

            mGR.BusinessFlows.Add(BF1);

            //Adding Same Business Flow Again to GingerRunner
            BusinessFlow bfToAdd = (BusinessFlow)BF1.CreateCopy(false);
            bfToAdd.ContainingFolder = BF1.ContainingFolder;
            bfToAdd.Active = true;
            bfToAdd.Reset();
            bfToAdd.InstanceGuid = Guid.NewGuid();
            mGR.BusinessFlows.Add(bfToAdd);

            WorkSpace.Instance.SolutionRepository = SR;

            //Act
            //Changing initial value of 2nd BF from BusinessFlow Config 
            mGR.BusinessFlows[2].Variables[0].Value = "bbb";
            mGR.BusinessFlows[2].Variables[0].DiffrentFromOrigin = true;

            mGR.RunRunner();

            //Assert
            Assert.AreEqual(BF1.RunStatus, eRunStatus.Passed);
            Assert.AreEqual(activity.Status, eRunStatus.Passed);

            Assert.AreEqual(bfToAdd.RunStatus, eRunStatus.Passed);

            Assert.AreEqual(mGR.BusinessFlows[1].Variables[0].Value, "aaa");
            Assert.AreEqual(mGR.BusinessFlows[2].Variables[0].Value, "bbb");

        }


        [TestMethod]
        public void TestDyanamicRunsetOprations()
        {
            RunSetConfig runSetConfigurations = CreteRunsetWithOperations();

            RunsetExecutor GMR = new RunsetExecutor();
            GMR.RunsetExecutionEnvironment = environment;
            GMR.RunSetConfig = runSetConfigurations;

            CLIHelper cLIHelper = new CLIHelper();
            cLIHelper.RunAnalyzer = true;
            cLIHelper.ShowAutoRunWindow = false;
            cLIHelper.DownloadUpgradeSolutionFromSourceControl = false;

            RunSetAutoRunConfiguration autoRunConfiguration = new RunSetAutoRunConfiguration(solution, GMR, cLIHelper);
            CLIDynamicXML mCLIDynamicXML = new CLIDynamicXML();
            autoRunConfiguration.SelectedCLI = mCLIDynamicXML;
            string file = autoRunConfiguration.SelectedCLI.CreateContent(solution, GMR, cLIHelper);

            XElement nodes = XElement.Parse(file);

            List<XElement> AddRunsetOPerationsNodes = nodes.Elements("AddRunsetOperation").ToList();

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(file);
            XmlNodeList runsetoperations = xDoc.GetElementsByTagName("AddRunsetOperation");

            //Assert
            Assert.AreEqual(runsetoperations.Count , 3);
            Assert.AreEqual(runsetoperations[0].FirstChild.Name, "MailFrom");
            Assert.AreEqual(runsetoperations[0].LastChild.Name, "IncludeAttachmentReport") ;
            Assert.AreEqual(runsetoperations[1].FirstChild.Name, "selectedHTMLReportTemplateID");
            Assert.AreEqual(runsetoperations[1].LastChild.Name, "isHTMLReportPermanentFolderNameUsed");
            Assert.AreEqual(runsetoperations[2].HasChildNodes,false);

        }

        [TestMethod]
        public void TestDynamicRunsetExecution()
        {

            RunSetConfig runSetConfig1 = new RunSetConfig();
            mGR.IsUpdateBusinessFlowRunList = true;
           
            runSetConfig1.GingerRunners.Add(mGR);
           // mGR.BusinessFlowsRunList = new BusinessFlowRun()
            runSetConfig1.UpdateRunnersBusinessFlowRunsList();
            runSetConfig1.mRunModeParallel = false;

            RunSetActionHTMLReport produceHTML2 = CreateProduceHTMlOperation();
            runSetConfig1.RunSetActions.Add(produceHTML2);

            RunsetExecutor GMR1 = new RunsetExecutor();
            GMR1.RunsetExecutionEnvironment = environment;
            GMR1.RunSetConfig = runSetConfig1;
            WorkSpace.Instance.RunsetExecutor = GMR1;
            CLIHelper cLIHelper1 = new CLIHelper();
            cLIHelper1.RunAnalyzer = false;
            cLIHelper1.ShowAutoRunWindow = true;
            cLIHelper1.DownloadUpgradeSolutionFromSourceControl = false;

            RunSetAutoRunConfiguration autoRunConfiguration1 = new RunSetAutoRunConfiguration(solution, GMR1, cLIHelper1);
            CLIDynamicXML mCLIDynamicXML1  = new CLIDynamicXML();
            autoRunConfiguration1.SelectedCLI = mCLIDynamicXML1;
            String xmlFile =autoRunConfiguration1.SelectedCLI.CreateContent(solution, GMR1, cLIHelper1);

            RunSetAutoRunShortcut runSetAutoRunShortcut = new RunSetAutoRunShortcut(autoRunConfiguration1);
            runSetAutoRunShortcut.CreateShortcut = true;
            runSetAutoRunShortcut.ShortcutFileName = "TestDynamicRunset";
            runSetAutoRunShortcut.ExecutorType = RunSetAutoRunShortcut.eExecutorType.GingerExe;
            runSetAutoRunShortcut.ShortcutFolderPath= Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            runSetAutoRunShortcut.ExecuterFolderPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
           
            autoRunConfiguration1.CreateContentFile();
            if (runSetAutoRunShortcut.CreateShortcut)
            {
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(runSetAutoRunShortcut.ShortcutFileFullPath);
                shortcut.Description = runSetAutoRunShortcut.ShortcutFileName;
                shortcut.WorkingDirectory = runSetAutoRunShortcut.ExecuterFolderPath;
                shortcut.TargetPath = runSetAutoRunShortcut.ExecuterFullPath;
                shortcut.Arguments = autoRunConfiguration1.ConfigArgs;

                string iconPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "GingerIconNew.ico");
                if (System.IO.File.Exists(iconPath))
                {
                    shortcut.IconLocation = iconPath;
                }
                shortcut.Save();

                CLIProcessor cLIProcessor = new CLIProcessor();
                string argument = autoRunConfiguration1.ConfigArgs;
                argument = argument.Replace('"', ' ');
                string[] args = argument.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                string s = args[2] + " "+args[3];
                args[2] = s;
                args = args.Take(args.Count() - 1 ).ToArray();
                cLIProcessor.ExecuteArgs(args);
            }
        }

        public RunSetActionHTMLReport CreateProduceHTMlOperation()
        {
            RunSetActionHTMLReport produceHTML1 = new RunSetActionHTMLReport();
            produceHTML1.Condition = RunSetActionBase.eRunSetActionCondition.AlwaysRun;
            produceHTML1.RunAt = RunSetActionBase.eRunAt.ExecutionEnd;
            produceHTML1.isHTMLReportFolderNameUsed = true;
            produceHTML1.HTMLReportFolderName = Path.Combine(TestResources.GetTestResourcesFolder(@"Solutions" + Path.DirectorySeparatorChar + "BasicSimple" + Path.DirectorySeparatorChar + "HTMLReports"));
            produceHTML1.isHTMLReportPermanentFolderNameUsed = false;
            produceHTML1.Active = true;
            return produceHTML1;
        }

        public RunSetConfig CreteRunsetWithOperations()
        {
            RunSetConfig runSetConfig = new RunSetConfig();
            runSetConfig.GingerRunners.Add(mGR);
            runSetConfig.mRunModeParallel = false;

            //added HTMl send mail action
            RunSetActionHTMLReportSendEmail sendMail = new RunSetActionHTMLReportSendEmail();
            sendMail.Condition = RunSetActionBase.eRunSetActionCondition.AlwaysRun;
            sendMail.RunAt = RunSetActionBase.eRunAt.ExecutionEnd;
            sendMail.MailFrom = "Test@gmail.com";
            sendMail.MailTo = "Test@gamil.com";
            sendMail.Email.EmailMethod = GingerCore.GeneralLib.Email.eEmailMethod.OUTLOOK;
            sendMail.Active = true;

            //added Produce Html action

            RunSetActionHTMLReport produceHTML = CreateProduceHTMlOperation();

            //added JSON action
            RunSetActionJSONSummary jsonReportOperation = new RunSetActionJSONSummary();
            jsonReportOperation.Name = "Json Report";
            jsonReportOperation.RunAt = RunSetActionBase.eRunAt.ExecutionEnd;
            jsonReportOperation.Condition = RunSetActionBase.eRunSetActionCondition.AlwaysRun;
            jsonReportOperation.Active = true;

            runSetConfig.RunSetActions.Add(sendMail);
            runSetConfig.RunSetActions.Add(produceHTML);
            runSetConfig.RunSetActions.Add(jsonReportOperation);

            return runSetConfig;
        }
    }
}
