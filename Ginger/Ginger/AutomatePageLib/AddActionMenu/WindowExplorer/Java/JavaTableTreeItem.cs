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

using Amdocs.Ginger.Common;
using GingerCore.Actions;
using GingerWPF.UserControlsLib.UCTreeView;
using System.Windows.Controls;
using Ginger.Actions._Common.ActUIElementLib;
using Amdocs.Ginger.Repository;

namespace Ginger.WindowExplorer.Java
{
    public class JavaTableTreeItem : JavaElementTreeItem, ITreeViewItem, IWindowExplorerTreeItem
    {
      ObservableList<Act> mAvailableActions = new ObservableList<Act>();
      UIElementTableConfigPage mUIElementTableConfigPage = null;
      StackPanel ITreeViewItem.Header()
      {
          string ImageFileName = "@Grid_16x16.png";
          return TreeViewUtils.CreateItemHeader(Name, ImageFileName);
      }
        ObservableList<Act> IWindowExplorerTreeItem.GetElementActions()
        {
            return mAvailableActions;
        }

        Page ITreeViewItem.EditPage(Amdocs.Ginger.Common.Context mContext)
        {
            if (mUIElementTableConfigPage == null)
            {
                mUIElementTableConfigPage = new UIElementTableConfigPage(base.JavaElementInfo, mAvailableActions, mContext);
            }
            return mUIElementTableConfigPage;
        }
        ObservableList<ActInputValue> IWindowExplorerTreeItem.GetItemSpecificActionInputValues()
        {
            return mUIElementTableConfigPage.GetTableRelatedInputValues();
        }
    }
}
