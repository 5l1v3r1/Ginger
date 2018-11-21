﻿#region License
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

using System.Collections.Generic;
using Amdocs.Ginger.Common.Enums;
using Amdocs.Ginger.Repository;

namespace GingerCore.Variables
{
    public class VariableString : VariableBase 
    {
        public VariableString()
        {
        }

        public override string VariableUIType
        {
            get { return GingerDicser.GetTermResValue(eTermResKey.Variable) + " String"; }
        }

        public override string VariableEditPage { get { return "VariableStringPage"; } }
        private string mInitialStringValue;
        
        // Only for set initial value, later on using only Value from VarBase
        // So when doing reset or start flow will reset to Initial String Value
        // Later on if during the flow the value change it will be on the VarBase.Value
        [IsSerializedForLocalRepository]
        public string InitialStringValue 
        { 
            set { mInitialStringValue = value; Value = value; OnPropertyChanged("Formula"); } 
            get { 
                //TODO: cleanup later, quick and dirty Temp solution for converting scripts prior to v1.5 which had the value and no StringInitialValue
                if (mInitialStringValue == null)
                {
                    mInitialStringValue = Value;
                }
                return mInitialStringValue; 
            } 
        }
        
        public override string GetFormula()
        {
            return "Initial String Value=" + mInitialStringValue;
        }

        public override void ResetValue()
        {            
            Value = mInitialStringValue; 
        }

        public override void GenerateAutoValue()
        { 
            //NA
        }

        public override eImageType Image { get { return eImageType.Variable; } }
        public override string VariableType() { return "String"; }

        public override List<VariableBase.eSetValueOptions> GetSupportedOperations()
        {
            List<VariableBase.eSetValueOptions> supportedOperations = new List<VariableBase.eSetValueOptions>();
            supportedOperations.Add(VariableBase.eSetValueOptions.SetValue);
            supportedOperations.Add(VariableBase.eSetValueOptions.ResetValue);
            supportedOperations.Add(VariableBase.eSetValueOptions.ClearSpecialChar);
            return supportedOperations;
        }

        public override bool SupportSetValue { get { return true; } }
    }
}