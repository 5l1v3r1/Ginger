#region License
/*
Copyright © 2014-2018 European Support Limited

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
using Amdocs.Ginger.Repository;
using GingerWPF.PluginsLib.AddPluginWizardLib;
using GingerWPF.WizardLib;
using System.Windows;
using System.Windows.Controls;

namespace GingerWPF.PluginsLib
{
    /// <summary>
    /// Interaction logic for PluginsPage.xaml
    /// </summary>
    public partial class PluginPackagesPage : Page
    {
        public PluginPackagesPage()
        {
            InitializeComponent();
            
            MaingGrid.ItemsSource = WorkSpace.Instance.SolutionRepository.GetAllRepositoryItems<PluginPackage>();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            WizardWindow.ShowWizard(new AddPluginPackageWizard());            
        }
    }
}
