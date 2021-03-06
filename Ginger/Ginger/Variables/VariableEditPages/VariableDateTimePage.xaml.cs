﻿#region License
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
using GingerCore.Variables;
using System;
using System.Windows.Controls;

namespace Ginger.Variables
{
    /// <summary>
    /// Interaction logic for VariableDateTimePage.xaml
    /// </summary>
    public partial class VariableDateTimePage : Page
    {
        private VariableDateTime variableDateTime;
        public VariableDateTimePage(VariableDateTime varDateTime)
        {
            variableDateTime = varDateTime;
            InitializeComponent();
            
            BindControlValue();

        }

        private void BindControlValue()
        {
            if (!string.IsNullOrEmpty(variableDateTime.InitialDateTime))
            {
                dtpInitialDate.Value = Convert.ToDateTime(variableDateTime.InitialDateTime);
            }

            dtpInitialDate.CustomFormat = variableDateTime.DateTimeFormat;
            dtpInitialDate.MinDate = Convert.ToDateTime(variableDateTime.MinDateTime);
            dtpInitialDate.MaxDate = Convert.ToDateTime(variableDateTime.MaxDateTime);
            
            dpMinDate.Value = Convert.ToDateTime(variableDateTime.MinDateTime);
            dpMinDate.CustomFormat = variableDateTime.DateTimeFormat;

            dpMaxDate.Value = Convert.ToDateTime(variableDateTime.MaxDateTime);
            dpMaxDate.CustomFormat = variableDateTime.DateTimeFormat;

            txtDateFormat.Text = variableDateTime.DateTimeFormat;

            GingerCore.GeneralLib.BindingHandler.ObjFieldBinding(txtDateFormat, TextBox.TextProperty,variableDateTime,nameof(VariableDateTime.DateTimeFormat));
            txtDateFormat.AddValidationRule(new DateTimeFormatValidationRule(variableDateTime));
        }


        private void dpMinDate_TextChanged(object sender, EventArgs e)
        {
            if (dpMinDate.Value <= dtpInitialDate.MaxDate)
            {
                dtpInitialDate.MinDate = dpMinDate.Value;
                variableDateTime.MinDateTime = dpMinDate.Value.ToString();
            }
            else
            {
                Reporter.ToLog(eLogLevel.ERROR,$"Minimum date :[{dpMinDate.Value}], sholud be <= Maximum Date:[{dtpInitialDate.MaxDate}]");
                dpMinDate.Focus();
                return;
            }
        }

        private void dpMaxDate_TextChanged(object sender, EventArgs e)
        {
            if(dpMaxDate.Value >= dtpInitialDate.MinDate)
            {
                dtpInitialDate.MaxDate = dpMaxDate.Value;
                variableDateTime.MaxDateTime = dpMaxDate.Value.ToString();
            }
            else
            {
                Reporter.ToLog(eLogLevel.ERROR, $"Maximum date :[{dpMaxDate.Value}], sholud be >= Minimum Date:[{dtpInitialDate.MinDate}]");
                dpMaxDate.Focus();
                return;
            }
                
        }

        private void dtpInitialDate_TextChanged(object sender, EventArgs e)
        {
            if (dtpInitialDate.Value < Convert.ToDateTime(variableDateTime.MinDateTime) || dtpInitialDate.Value > Convert.ToDateTime(variableDateTime.MaxDateTime))
            {
                Reporter.ToLog(eLogLevel.ERROR, $"Input Value is not in range:- Maximum date :[{dpMaxDate.Value}], Minimum Date:[{dtpInitialDate.MinDate}]");
                dtpInitialDate.Focus();
                return;
            }
            else
            {
                variableDateTime.InitialDateTime = dtpInitialDate.Value.ToString();
            }
        }

        private void txtDateFormat_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            dtpInitialDate.CustomFormat = txtDateFormat.Text;
            dpMinDate.CustomFormat = txtDateFormat.Text;
            dpMaxDate.CustomFormat = txtDateFormat.Text;
        }
    }
}
