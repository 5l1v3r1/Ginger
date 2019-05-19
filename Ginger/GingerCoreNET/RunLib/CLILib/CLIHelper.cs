﻿using amdocs.ginger.GingerCoreNET;
using Amdocs.Ginger.Common;
using Ginger;
using Ginger.Run;
using GingerCore;
using GingerCore.Environments;
using GingerCoreNET.SourceControl;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Amdocs.Ginger.CoreNET.RunLib.CLILib
{
    public class CLIHelper
    {
        static readonly string ENCRYPTION_KEY = "D3^hdfr7%ws4Kb56=Qt";//????? !!!!!!!!!!!!!!!!!!!

        public string Solution;
        public string Env;
        public string Runset;
        public string SourceControlURL;
        public string SourcecontrolUser;
        public string sourceControlPass;
        public eAppReporterLoggingLevel AppLoggingLevel;


        static bool mShowAutoRunWindow ; // default is false except in ConfigFile which is true to keep backword compatibility
        
        public static bool ShowAutoRunWindow
        {
            get
            {
                return mShowAutoRunWindow;
            }
            set
            {
                mShowAutoRunWindow = value;
                //Reporter.ToLog(eLogLevel.DEBUG, string.Format("ShowAutoRunWindow {0}", value));
            }
        }

        static bool mDownloadSolutionFromSourceControl;
        public static bool DownloadSolutionFromSourceControlBool
        {
            get
            {
                return mDownloadSolutionFromSourceControl;
            }
            set
            {
                mDownloadSolutionFromSourceControl = value;
            }

        }

        static bool mRunAnalyzer;
        public static bool RunAnalyzer {
            get
            {
                return mRunAnalyzer;
            }
            set
            {
                mRunAnalyzer = value;
            }
        }

        RunsetExecutor mRunsetExecutor;
        //UserProfile WorkSpace.Instance.UserProfile;
        RunSetConfig runSetConfig;

        public bool ProcessArgs(RunsetExecutor runsetExecutor)
        {
            try
            {
                mRunsetExecutor = runsetExecutor;
                // SetDebugLevel();//disabeling because it is overwriting the UserProfile setting for logging level
                DownloadSolutionFromSourceControl();
                if (OpenSolution())
                {
                    SelectEnv();
                    SelectRunset();
                    SetRunAnalyzer();
                    HandleAutoRunWindow();
                    return true;
                }

                return false;
            }
            catch(Exception ex)
            {
                Reporter.ToLog(eLogLevel.ERROR, "Unexpected error occurred while processing the Run Configurations", ex);
                return false;
            }
        }

        private void SetDebugLevel()
        {            
            Reporter.AppLoggingLevel = AppLoggingLevel;
        }

        private void HandleAutoRunWindow()
        {
            if(ShowAutoRunWindow)
            {
                Reporter.ToLog(eLogLevel.DEBUG, "Showing RunSet AutoRunWindow");
                RepositoryItemHelper.RepositoryItemFactory.ShowAutoRunWindow();
            }
            else
            {
                Reporter.ToLog(eLogLevel.DEBUG, "Not Showing RunSet AutoRunWindow");
            }
        }

        private void SetRunAnalyzer()
        {
            // TODO: once analyzer moved to GingerCoreNET we can run it here 
            try
            {
                runSetConfig.RunWithAnalyzer = RunAnalyzer;

                //// Return true if there are analyzer issues
                //private bool RunAnalyzer()
                //{
                //    //Running Runset Analyzer to look for issues
                //    Reporter.ToLog(eLogLevel.DEBUG, string.Format("Running {0} Analyzer", GingerDicser.GetTermResValue(eTermResKey.RunSet)));
                //    try
                //    {
                //        //run analyzer
                //        int analyzeRes = runsetExecutor.RunRunsetAnalyzerBeforeRunSync(true);
                //        if (analyzeRes == 1)
                //        {
                //            Reporter.ToLog(eLogLevel.ERROR, string.Format("{0} Analyzer found critical issues with the {0} configurations, aborting execution.", GingerDicser.GetTermResValue(eTermResKey.RunSet)));
                //            return true;//cancel run because issues found
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        Reporter.ToLog(eLogLevel.ERROR, string.Format("Failed Running {0} Analyzer, still continue execution", GingerDicser.GetTermResValue(eTermResKey.RunSet)), ex);
                //        return true;
                //    }
                //    return false;
                //}
            }
            catch(Exception ex)
            {
                Reporter.ToUser(eUserMsgKey.CannotRunShortcut, ex.Message);
            }
        }

        private void SelectRunset()
        {            
            Reporter.ToLog(eLogLevel.DEBUG, string.Format("Selected {0}: '{1}'", GingerDicser.GetTermResValue(eTermResKey.RunSet), Runset));
            ObservableList<RunSetConfig> RunSets = WorkSpace.Instance.SolutionRepository.GetAllRepositoryItems<RunSetConfig>();
            runSetConfig = RunSets.Where(x => x.Name.ToLower().Trim() == Runset.ToLower().Trim()).FirstOrDefault();
            if (runSetConfig != null)
            {
                mRunsetExecutor.RunSetConfig = runSetConfig;
            }
            else
            {
                Reporter.ToLog(eLogLevel.ERROR, string.Format("Failed to find matching {0} in the Solution", GingerDicser.GetTermResValue(eTermResKey.RunSet)));
                Reporter.ToUser(eUserMsgKey.CannotRunShortcut);
                // TODO: throw
                // return false;
            }
        }

        private void SelectEnv()
        {           
            Reporter.ToLog(eLogLevel.DEBUG, "Selected Environment: '" + Env + "'");
            ProjEnvironment env = WorkSpace.Instance.SolutionRepository.GetAllRepositoryItems<ProjEnvironment>().Where(x => x.Name.ToLower().Trim() == Env.ToLower().Trim()).FirstOrDefault();
            if (env != null)
            {
                mRunsetExecutor.RunsetExecutionEnvironment = env;
            }
            else
            {
                Reporter.ToLog(eLogLevel.ERROR, "Failed to find matching Environment in the Solution");
                // TODO: throw
                // return false;
            }
        }

        private void DownloadSolutionFromSourceControl()
        {
            if (SourceControlURL != null && SourcecontrolUser != "" && sourceControlPass != null)
            {
                Reporter.ToLog(eLogLevel.DEBUG, "Downloading Solution from source control");
                if (SourceControlURL.IndexOf(".git") != -1)
                {
                    // App.DownloadSolution(value.Substring(0, value.IndexOf(".git") + 4));
                    RepositoryItemHelper.RepositoryItemFactory.DownloadSolution(SourceControlURL.Substring(0, SourceControlURL.IndexOf(".git") + 4));
                }
                else
                {
                    // App.DownloadSolution(value);
                    //RepositoryItemHelper.RepositoryItemFactory.DownloadSolution(SourceControlURL);

                    RepositoryItemHelper.RepositoryItemFactory.DownloadSolution(Solution);
                }
            }
        }

        internal void SetSourceControlPassword(string value)
        {
            Reporter.ToLog(eLogLevel.DEBUG, "Selected SourceControlPassword: '" + value + "'");
            WorkSpace.Instance.UserProfile.SourceControlPass = value;
            sourceControlPass = value;
        }

        internal void PasswordEncrypted(string value)
        {
            Reporter.ToLog(eLogLevel.DEBUG, "PasswordEncrypted: '" + value + "'");
            string pswd = WorkSpace.Instance.UserProfile.SourceControlPass;
            if (value == "Y")
            {
                pswd = EncryptionHandler.DecryptwithKey(WorkSpace.Instance.UserProfile.SourceControlPass, ENCRYPTION_KEY);
            }

            if (WorkSpace.Instance.UserProfile.SourceControlType == SourceControlBase.eSourceControlType.GIT && pswd == "")
            {
                pswd = "Test";
            }

            WorkSpace.Instance.UserProfile.SourceControlPass = pswd;
        }

        internal void SourceControlProxyPort(string value)
        {
            if (value == "")
            {
                WorkSpace.Instance.UserProfile.SolutionSourceControlConfigureProxy = false;
            }
            else
            {
                WorkSpace.Instance.UserProfile.SolutionSourceControlConfigureProxy = true;
            }

            Reporter.ToLog(eLogLevel.INFO, "Selected SourceControlProxyPort: '" + value + "'");
            WorkSpace.Instance.UserProfile.SolutionSourceControlProxyPort = value;
        }

        internal void SourceControlProxyServer(string value)
        {
            Reporter.ToLog(eLogLevel.DEBUG, "Selected SourceControlProxyServer: '" + value + "'");
            if (value == "")
            {
                WorkSpace.Instance.UserProfile.SolutionSourceControlConfigureProxy = false;
            }
            else
            {
                WorkSpace.Instance.UserProfile.SolutionSourceControlConfigureProxy = true;
            }

            if (value != "" && !value.ToUpper().StartsWith("HTTP://"))
            {
                value = "http://" + value;
            }

            WorkSpace.Instance.UserProfile.SolutionSourceControlProxyAddress = value;
        }

        internal void SetSourceControlUser(string value)
        {
            Reporter.ToLog(eLogLevel.DEBUG, "Selected SourceControlUser: '" + value + "'");
            if (WorkSpace.Instance.UserProfile.SourceControlType == SourceControlBase.eSourceControlType.GIT && value == "")
            {
                value = "Test";
            }

            WorkSpace.Instance.UserProfile.SourceControlUser = value;
            SourcecontrolUser = value;
        }

        internal void SetSourceControlURL(string value)
        {
            Reporter.ToLog(eLogLevel.DEBUG, "Selected SourceControlUrl: '" + value + "'");
            if (WorkSpace.Instance.UserProfile.SourceControlType == SourceControlBase.eSourceControlType.SVN)
            {
                if (!value.ToUpper().Contains("/SVN") && !value.ToUpper().Contains("/SVN/"))
                {
                    value = value + "svn/";
                }
                if (!value.ToUpper().EndsWith("/"))
                {
                    value = value + "/";
                }
            }
            WorkSpace.Instance.UserProfile.SourceControlURL = value;
            SourceControlURL = value;
        }

        internal void SetSourceControlType(string value)
        {
            Reporter.ToLog(eLogLevel.DEBUG, "Selected SourceControlType: '" + value + "'");
            if (value.Equals("GIT"))
            {
                WorkSpace.Instance.UserProfile.SourceControlType = SourceControlBase.eSourceControlType.GIT;
            }
            else if (value.Equals("SVN"))
            {
               WorkSpace.Instance.UserProfile.SourceControlType = SourceControlBase.eSourceControlType.SVN;
            }
            else
            {
                WorkSpace.Instance.UserProfile.SourceControlType = SourceControlBase.eSourceControlType.None;
            }
        }

        private bool OpenSolution()
        {
            //Reporter.ToLog(eLogLevel.DEBUG, "Loading the Solution: '" + Solution + "'");
            try
            {
                //if (WorkSpace.Instance.OpenSolution(Solution) == false)
                //{
                //    Reporter.ToLog(eLogLevel.ERROR, "Failed to load the Solution");
                //    // TODO: throw
                //    return;
                //}
                return WorkSpace.Instance.OpenSolution(Solution);
            }
            catch (Exception ex)
            {
                //Reporter.ToLog(eLogLevel.ERROR, "Failed to load the Solution");
                //Reporter.ToLog(eLogLevel.ERROR, $"Method - {MethodBase.GetCurrentMethod().Name}, Error - {ex.Message}", ex);
                // TODO: throw
                return false;
            }
        }

        public void CloseSolution()
        {
            try
            {
                WorkSpace.Instance.CloseSolution();
            }
            catch (Exception ex)
            { 
                Reporter.ToLog(eLogLevel.ERROR, "Unexpected Error occurred while closing the Solution", ex);
            }
        }
    }
}
