#region License
/*
Copyright © 2014-2020 European Support Limited

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

using System.Windows.Controls;

namespace Ginger.Actions
{
    public partial class ActLinkEditPage : Page
    {
        private GingerCore.Actions.ActLink f;

        public ActLinkEditPage(GingerCore.Actions.ActLink Act)
        {
            InitializeComponent();

            this.f = Act;

            GingerCore.General.FillComboFromEnumObj(ActionNameComboBox, Act.LinkAction);
            //TODO: fix hard coded ButtonAction use Fields
            GingerCore.GeneralLib.BindingHandler.ObjFieldBinding(ActionNameComboBox, ComboBox.TextProperty, Act, "LinkAction");
        }
    }
}
