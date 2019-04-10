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
using Amdocs.Ginger.CoreNET.RunLib;
using Amdocs.Ginger.Repository;
using GingerCore;
using GingerWPF.WizardLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static Ginger.ExtensionMethods;

namespace Ginger.Agents.AddAgentWizardLib
{
    /// <summary>
    /// Interaction logic for AddAgentDetailsPage.xaml
    /// </summary>
    public partial class AddAgentDetailsPage : Page, IWizardPage
    {
        AddAgentWizard mWizard;
        public readonly ObservableList<PluginPackage> Plugins;
        private List<DriverInfo> DriversforPlatform = new List<DriverInfo>();

        public AddAgentDetailsPage()
        {
            InitializeComponent();
            Plugins = WorkSpace.Instance.SolutionRepository.GetAllRepositoryItems<PluginPackage>();
        }        

        public void WizardEvent(WizardEventArgs WizardEventArgs)
        {
            mWizard = (AddAgentWizard)WizardEventArgs.Wizard;
            switch (WizardEventArgs.EventType)
            {            
                case EventType.Init:                                        
                    xAgentNameTextBox.BindControl(mWizard.Agent, nameof(Agent.Name));                  
                    xAgentNameTextBox.AddValidationRule(new AgentNameValidationRule());
                    xAgentNameTextBox.Focus();

                    xAgentDescriptionTextBox.BindControl(mWizard.Agent, nameof(Agent.Notes));
                    xAgentTagsViewer.Init(mWizard.Agent.Tags);

                    //Removing ASCF from platform combobox
                    List<GingerCore.General.ComboEnumItem> platformList = (GingerCore.General.GetEnumValuesForCombo(typeof(GingerCoreNET.SolutionRepositoryLib.RepositoryObjectsLib.PlatformsLib.ePlatformType))).Where(x => ((GingerCoreNET.SolutionRepositoryLib.RepositoryObjectsLib.PlatformsLib.ePlatformType)x.Value) != GingerCoreNET.SolutionRepositoryLib.RepositoryObjectsLib.PlatformsLib.ePlatformType.ASCF).ToList();
                  
                    xPlatformTypeComboBox.BindControl(platformList);
                    xPlatformTypeComboBox.SelectionChanged += xPlatformTypeComboBox_SelectionChanged;
                    xPlatformTypeComboBox.SelectedIndex = 0;

                    xDriverTypeComboBox.BindControl(mWizard.Agent, nameof(Agent.DriverType));
                    xDriverTypeComboBox.SelectionChanged += xDriverTypeComboBox_SelectionChanged;
                    xDriverTypeComboBox.AddValidationRule(eValidationRule.CannotBeEmpty);                    
                    xDriverTypeStackPanel.Visibility = Visibility.Collapsed;

       
                    break;                
            }

        }


        private void xPlatformTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            xDriverTypeComboBox.SelectedItem = null;
            xDriverTypeComboBox.Items.Clear();
       
            //List<object> driverTypeValues = mWizard.Agent.GetDriverTypesByPlatfrom(xPlatformTypeComboBox.SelectedValue.ToString());


            //GingerCore.General.FillComboFromEnumObj(xDriverTypeComboBox, mWizard.Agent.DriverType, driverTypeValues, false);


            DriversforPlatform= DriverInfo.GetDriversforPlatform(xPlatformTypeComboBox.SelectedValue.ToString());

            foreach (DriverInfo  Di in DriversforPlatform)
            {
                xDriverTypeComboBox.Items.Add(Di);

            }
    

            if (xDriverTypeComboBox.Items.Count > 0)
                xDriverTypeComboBox.SelectedItem = xDriverTypeComboBox.Items[0];

            if (xDriverTypeComboBox.Items.Count > 1)
            {
                xDriverTypeStackPanel.Visibility = Visibility.Visible;
            }
            else
            {
                xDriverTypeStackPanel.Visibility = Visibility.Collapsed;
            }

            ObservableList<PluginPackage> PIS = WorkSpace.Instance.SolutionRepository.GetAllRepositoryItems<PluginPackage>();
        }

        private void xDriverTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)

        {
            xDriverSubTypeComboBox.Items.Clear();
            xDriverSubTypeStackPanel.Visibility = Visibility.Visible;

            DriverInfo DI= xDriverTypeComboBox.SelectedItem as DriverInfo;

            foreach(var service in DI.services)
            {
                xDriverSubTypeComboBox.Items.Add(service);
            }
          

         //   mWizard.Agent.InitDriverConfigs();
        }

  
    
        void ShowConfig()
        {
           
             
                xDriverConfigStackPanel.Visibility = Visibility.Visible;
            
            
            
        }


    }
}
