﻿using amdocs.ginger.GingerCoreNET;
using Amdocs.Ginger.Common;
using Amdocs.Ginger.CoreNET.Execution;
using Ginger.Run;
using GingerCore;
using GingerCore.Environments;
using GingerCoreNET.SourceControl;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Amdocs.Ginger.CoreNET.RunLib
{
    class ConfigFileProcessor
    {
        static readonly string ENCRYPTION_KEY = "D3^hdfr7%ws4Kb56=Qt";//????? !!!!!!!!!!!!!!!!!!!
        
        public bool ProcessCommandLineArgs(string[] lines)
        {
            string scURL = null;
            string scUser = null;
            string scPswd = null;

            foreach (string arg in lines)
            {
                int i = arg.IndexOf('=');
                string param = arg.Substring(0, i).Trim();
                string value = arg.Substring(i + 1).Trim();

                switch (param)
                {
                    case "SourceControlType":
                        Reporter.ToLog(eLogLevel.DEBUG, "Selected SourceControlType: '" + value + "'");
                        if (value.Equals("GIT"))
                            WorkSpace.Instance.UserProfile.SourceControlType = SourceControlBase.eSourceControlType.GIT;
                        else if (value.Equals("SVN"))
                            WorkSpace.Instance.UserProfile.SourceControlType = SourceControlBase.eSourceControlType.SVN;
                        else
                            WorkSpace.Instance.UserProfile.SourceControlType = SourceControlBase.eSourceControlType.None;
                        break;

                    case "SourceControlUrl":
                        Reporter.ToLog(eLogLevel.DEBUG, "Selected SourceControlUrl: '" + value + "'");
                        if (WorkSpace.Instance.UserProfile.SourceControlType == SourceControlBase.eSourceControlType.SVN)
                        {
                            if (!value.ToUpper().Contains("/SVN") && !value.ToUpper().Contains("/SVN/"))
                                value = value + "svn/";
                            if (!value.ToUpper().EndsWith("/"))
                                value = value + "/";
                        }
                        WorkSpace.Instance.UserProfile.SourceControlURL = value;
                        scURL = value;
                        break;

                    case "SourceControlUser":
                        Reporter.ToLog(eLogLevel.DEBUG, "Selected SourceControlUser: '" + value + "'");
                        if (WorkSpace.Instance.UserProfile.SourceControlType == SourceControlBase.eSourceControlType.GIT && value == "")
                            value = "Test";
                        WorkSpace.Instance.UserProfile.SourceControlUser = value;
                        scUser = value;
                        break;

                    case "SourceControlPassword":
                        Reporter.ToLog(eLogLevel.DEBUG, "Selected SourceControlPassword: '" + value + "'");
                        WorkSpace.Instance.UserProfile.SourceControlPass = value;
                        scPswd = value;
                        break;

                    case "PasswordEncrypted":
                        Reporter.ToLog(eLogLevel.DEBUG, "PasswordEncrypted: '" + value + "'");
                        string pswd = WorkSpace.Instance.UserProfile.SourceControlPass;
                        if (value == "Y")
                            pswd = EncryptionHandler.DecryptwithKey(WorkSpace.Instance.UserProfile.SourceControlPass, ENCRYPTION_KEY);
                        if (WorkSpace.Instance.UserProfile.SourceControlType == SourceControlBase.eSourceControlType.GIT && pswd == "")
                            pswd = "Test";
                        WorkSpace.Instance.UserProfile.SourceControlPass = pswd;
                        break;

                    case "SourceControlProxyServer":
                        Reporter.ToLog(eLogLevel.DEBUG, "Selected SourceControlProxyServer: '" + value + "'");
                        if (value == "")
                            WorkSpace.Instance.UserProfile.SolutionSourceControlConfigureProxy = false;
                        else
                            WorkSpace.Instance.UserProfile.SolutionSourceControlConfigureProxy = true;
                        if (value != "" && !value.ToUpper().StartsWith("HTTP://"))
                            value = "http://" + value;
                        WorkSpace.Instance.UserProfile.SolutionSourceControlProxyAddress = value;
                        break;

                    case "SourceControlProxyPort":
                        if (value == "")
                            WorkSpace.Instance.UserProfile.SolutionSourceControlConfigureProxy = false;
                        else
                            WorkSpace.Instance.UserProfile.SolutionSourceControlConfigureProxy = true;
                        Reporter.ToLog(eLogLevel.INFO, "Selected SourceControlProxyPort: '" + value + "'");
                        WorkSpace.Instance.UserProfile.SolutionSourceControlProxyPort = value;
                        break;

                    case "Solution":
                        if (scURL != null && scUser != "" && scPswd != null)
                        {
                            Reporter.ToLog(eLogLevel.DEBUG, "Downloading Solution from source control");
                            if (value.IndexOf(".git") != -1)
                            {
                                // App.DownloadSolution(value.Substring(0, value.IndexOf(".git") + 4));
                                RepositoryItemHelper.RepositoryItemFactory.DownloadSolution(value.Substring(0, value.IndexOf(".git") + 4));
                            }
                            else
                            {
                                // App.DownloadSolution(value);
                                RepositoryItemHelper.RepositoryItemFactory.DownloadSolution(value);
                            }
                        }
                        Reporter.ToLog(eLogLevel.DEBUG, "Loading the Solution: '" + value + "'");
                        try
                        {
                            if (WorkSpace.Instance.OpenSolution(value) == false)
                            {
                                Reporter.ToLog(eLogLevel.ERROR, "Failed to load the Solution");
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            Reporter.ToLog(eLogLevel.ERROR, "Failed to load the Solution");
                            Reporter.ToLog(eLogLevel.ERROR, $"Method - {MethodBase.GetCurrentMethod().Name}, Error - {ex.Message}", ex);
                            return false;
                        }
                        break;

                    case "Env":
                        Reporter.ToLog(eLogLevel.DEBUG, "Selected Environment: '" + value + "'");
                        ProjEnvironment env = WorkSpace.Instance.SolutionRepository.GetAllRepositoryItems<ProjEnvironment>().Where(x => x.Name.ToLower().Trim() == value.ToLower().Trim()).FirstOrDefault();
                        if (env != null)
                        {
                            WorkSpace.Instance.RunsetExecutor.RunsetExecutionEnvironment = env;
                        }
                        else
                        {
                            Reporter.ToLog(eLogLevel.ERROR, "Failed to find matching Environment in the Solution");
                            return false;
                        }
                        break;

                    case "RunSet":
                        Reporter.ToLog(eLogLevel.DEBUG, string.Format("Selected {0}: '{1}'", GingerDicser.GetTermResValue(eTermResKey.RunSet), value));
                        ObservableList<RunSetConfig> RunSets = WorkSpace.Instance.SolutionRepository.GetAllRepositoryItems<RunSetConfig>();
                        RunSetConfig runSetConfig = RunSets.Where(x => x.Name.ToLower().Trim() == value.ToLower().Trim()).FirstOrDefault();
                        if (runSetConfig != null)
                        {
                            WorkSpace.Instance.RunsetExecutor.RunSetConfig = runSetConfig;
                        }
                        else
                        {
                            Reporter.ToLog(eLogLevel.ERROR, string.Format("Failed to find matching {0} in the Solution", GingerDicser.GetTermResValue(eTermResKey.RunSet)));
                            return false;
                        }
                        break;

                    default:
                        Reporter.ToLog(eLogLevel.ERROR, "Un Known argument: '" + param + "'");
                        return false;
                }

            }
            return true;
        }

        public void ExecuteRunSetConfigFile()
        {
            string phase = "Running in Automatic Execution Mode";
            Reporter.ToLog(eLogLevel.INFO, phase);

            AutoLogProxy.LogAppOpened();


            var result = RunRunSetFromCommandLine();

            Reporter.ToLog(eLogLevel.INFO, "Closing Ginger automatically...");


            //setting the exit code based on execution status
            if (result == 0)
            {
                Reporter.ToLog(eLogLevel.DEBUG, ">> Run Set executed and passed, exit code: 0");
                Environment.ExitCode = 0;//success                    
            }
            else
            {
                Reporter.ToLog(eLogLevel.DEBUG, ">> No indication found for successful execution, exit code: 1");
                Environment.ExitCode = 1;//failure
            }

            AutoLogProxy.LogAppClosed();
            Environment.Exit(Environment.ExitCode);
        }



        public int RunRunSetFromCommandLine()
        {
            //0- success
            //1- failure

            try
            {
                Reporter.ToLog(eLogLevel.DEBUG, "Processing Command Line Arguments");
                if (ProcessCommandLineArgs() == false)
                {
                    Reporter.ToLog(eLogLevel.DEBUG, "Processing Command Line Arguments failed");
                    return 1;
                }

                AutoLogProxy.UserOperationStart("AutoRunWindow", WorkSpace.Instance.RunsetExecutor.RunSetConfig.Name, WorkSpace.Instance.RunsetExecutor.RunsetExecutionEnvironment.Name);
                Reporter.ToLog(eLogLevel.DEBUG, string.Format("########################## Starting {0} Automatic Execution Process ##########################", GingerDicser.GetTermResValue(eTermResKey.RunSet)));

                Reporter.ToLog(eLogLevel.DEBUG, string.Format("Loading {0} execution UI elements", GingerDicser.GetTermResValue(eTermResKey.RunSet)));
                try
                {
                   




                }
                catch (Exception ex)
                {
                    Reporter.ToLog(eLogLevel.ERROR, string.Format("Failed loading {0} execution UI elements, aborting execution.", GingerDicser.GetTermResValue(eTermResKey.RunSet)), ex);
                    return 1;
                }

                //Running Runset Analyzer to look for issues
                Reporter.ToLog(eLogLevel.DEBUG, string.Format("Running {0} Analyzer", GingerDicser.GetTermResValue(eTermResKey.RunSet)));
                try
                {
                    //run analyzer
                    int analyzeRes = WorkSpace.Instance.RunsetExecutor.RunRunsetAnalyzerBeforeRun22222(true);
                    if (analyzeRes == 1)
                    {
                        Reporter.ToLog(eLogLevel.ERROR, string.Format("{0} Analyzer found critical issues with the {0} configurations, aborting execution.", GingerDicser.GetTermResValue(eTermResKey.RunSet)));
                        return 1;//cancel run because issues found
                    }
                }
                catch (Exception ex)
                {
                    Reporter.ToLog(eLogLevel.ERROR, string.Format("Failed Running {0} Analyzer, still continue execution", GingerDicser.GetTermResValue(eTermResKey.RunSet)), ex);
                    //return 1;
                }

                //Execute
                try
                {
                    
                    RepositoryItemHelper.RepositoryItemFactory.ShowAutoRunWindow();                    

                    WorkSpace.Instance.RunsetExecutor.InitRunners();
                    Task t = Task.Factory.StartNew(() =>
                     {
                            

                          WorkSpace.Instance.RunsetExecutor.RunRunset();
                         
                     });
                    t.Wait();  
                    
                }
                catch (Exception ex)
                {
                    Reporter.ToLog(eLogLevel.ERROR, string.Format("Error occured during the {0} execution.", GingerDicser.GetTermResValue(eTermResKey.RunSet)), ex);
                    return 1;
                }

                if (WorkSpace.Instance.RunsetExecutor.RunSetExecutionStatus == eRunStatus.Passed)//TODO: improve
                    return 0;
                else
                    return 1;
            }
            catch (Exception ex)
            {
                Reporter.ToLog(eLogLevel.ERROR, "Un expected error occured during the execution", ex);
                return 1;
            }
            finally
            {
                AutoLogProxy.UserOperationEnd();
            }
        }

        // config file
        private bool ProcessCommandLineArgs()
        {
            // New option with one arg to config file
            // resole spaces and quotes mess in commnd arg + more user friednly to edit
            // We expect only AutoRun --> File location
            try
            {
                string[] Args = Environment.GetCommandLineArgs();

                // We expect Autorun as arg[1]
                string[] arg1 = Args[1].Split('=');

                if (arg1[0] != "ConfigFile")
                {
                    Reporter.ToLog(eLogLevel.ERROR, "'ConfigFile' argument was not found.");
                    return false;
                }

                string AutoRunFileName = arg1[1];

                Reporter.ToLog(eLogLevel.DEBUG, "Reading all arguments from the Config file placed at: '" + AutoRunFileName + "'");
                string[] lines = System.IO.File.ReadAllLines(AutoRunFileName);
                //TODO:  Move to CLIProcc !!!!
                ProcessCommandLineArgs(lines);

                if (WorkSpace.Instance.RunsetExecutor.RunSetConfig != null && WorkSpace.Instance.RunsetExecutor.RunsetExecutionEnvironment != null)
                {
                    return true;
                }
                else
                {
                    Reporter.ToLog(eLogLevel.ERROR, "Missing key arguments which required for execution");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Reporter.ToLog(eLogLevel.ERROR, "Exception occurred during command line arguments processing", ex);
                return false;
            }
        }


    }
}
