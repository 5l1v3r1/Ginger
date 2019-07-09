﻿using Amdocs.Ginger.Common;
using Amdocs.Ginger.Common.UIElement;
using Ginger.BusinessFlowPages_New;
using Ginger.WindowExplorer;
using GingerCore.Platforms;
using GingerCoreNET;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Ginger.BusinessFlowsLibNew.AddActionMenu
{
    /// <summary>
    /// Interaction logic for WindowsExplorerNavPage.xaml
    /// </summary>
    public partial class WindowsExplorerNavPage : Page
    {
        Context mContext;
        IWindowExplorer mWindowExplorerDriver;
        List<AgentMappingPage> mWinExplorerPageList = null;
        WindowExplorerPage CurrentLoadedPage = null;

        public WindowsExplorerNavPage(Context context)
        {
            InitializeComponent();
            mContext = context;            
            context.PropertyChanged += Context_PropertyChanged;
            SetFrameEnableDisable();
            LoadWindowExplorerPage(mContext);            
        }
        
        /// <summary>
        /// Context Property changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Context_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Context.AgentStatus))
            {
                SetFrameEnableDisable();
                CurrentLoadedPage.SetWindowExplorerForNewPanel(mWindowExplorerDriver);
            }
            else if (e.PropertyName == nameof(Context.Agent))
            {
                SetFrameEnableDisable();
                LoadWindowExplorerPage(mContext);
                CurrentLoadedPage.SetWindowExplorerForNewPanel(mWindowExplorerDriver);
            }
        }

        /// <summary>
        /// This method will check if agent is running then it will enable the frame
        /// </summary>
        private void SetFrameEnableDisable()
        {
            bool isAgentRunning = AgentHelper.CheckIfAgentIsRunning(mContext.BusinessFlow.CurrentActivity, mContext.Runner, mContext, out mWindowExplorerDriver);
            if (isAgentRunning)
            {
                xSelectedItemFrame.IsEnabled = true;
            }
            else
            {
                if (mWinExplorerPageList != null)
                {
                    AgentMappingPage objHelper = mWinExplorerPageList.Where(x => x.ObjectAgent.DriverType == mContext.Agent.DriverType &&
                                                                                            x.ObjectAgent.ItemName == mContext.Agent.ItemName).FirstOrDefault();
                    if (objHelper != null && objHelper.ObjectWindowPage != null)
                    {
                        objHelper.ObjectWindowPage = new WindowExplorerPage(AgentHelper.GetAppAgent(mContext.Activity, mContext.Runner, mContext), mContext);
                        CurrentLoadedPage = (WindowExplorerPage)objHelper.ObjectWindowPage;
                        xSelectedItemFrame.Content = CurrentLoadedPage;
                    } 
                }
                xSelectedItemFrame.IsEnabled = false;
            }
        }

        /// <summary>
        /// This method is used to do the search
        /// </summary>
        /// <returns></returns>
        public bool DoSearchControls()
        {            
            return CurrentLoadedPage.DoSearchControls();
        }

        /// <summary>
        /// This method is used to get the new WindowExplorerPage based on Context and Agent
        /// </summary>
        /// <returns></returns>
        private void LoadWindowExplorerPage(Context context)
        {
            bool isLoaded = false;
            if (mWinExplorerPageList != null && mWinExplorerPageList.Count > 0)
            {
                AgentMappingPage objHelper = mWinExplorerPageList.Where(x => x.ObjectAgent.DriverType == context.Agent.DriverType && 
                                                                                x.ObjectAgent.ItemName == context.Agent.ItemName).FirstOrDefault();
                if (objHelper != null && objHelper.ObjectWindowPage != null)
                {
                    CurrentLoadedPage = (WindowExplorerPage)objHelper.ObjectWindowPage;
                    isLoaded = true;
                }
            }

            if (!isLoaded)
            {
                ApplicationAgent appAgent = AgentHelper.GetAppAgent(context.BusinessFlow.CurrentActivity, context.Runner, context);
                if (appAgent != null)
                {
                    CurrentLoadedPage = new WindowExplorerPage(appAgent, context);
                    CurrentLoadedPage.SetWindowExplorerForNewPanel(mWindowExplorerDriver);
                    if (mWinExplorerPageList == null)
                    {
                        mWinExplorerPageList = new List<AgentMappingPage>();
                    }
                    mWinExplorerPageList.Add(new AgentMappingPage(context.Agent, CurrentLoadedPage)); 
                }
            }

            xSelectedItemFrame.Content = CurrentLoadedPage;
        }
    }
}
