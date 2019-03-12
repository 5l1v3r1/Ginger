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

using GingerWPFUnitTest.POMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GingerTest.POMs.Common
{
    public class POMButtons : GingerPOMBase
    {
        DependencyObject mDependencyObject;

        public POMButtons(DependencyObject dependencyObject)
        {
            mDependencyObject = dependencyObject;
        }

        public ButtonPOM this[string text]
        {
            get
            {                
                Button b = (Button)FindElementByText<Button>(mDependencyObject, text);
                if (b != null)
                {
                    ButtonPOM buttonPOM = new ButtonPOM(b);
                    return buttonPOM;
                }
                else
                {
                    throw new Exception("Cannot find button with text: " + text);
                }
            }
        }
     
    }
}

