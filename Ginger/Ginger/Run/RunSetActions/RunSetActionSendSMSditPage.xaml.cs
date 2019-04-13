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

using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;
using GingerCore.GeneralLib;

namespace Ginger.Run.RunSetActions
{
    /// <summary>
    /// Interaction logic for RunSetActionEmailReportEditPage.xaml
    /// </summary>
    public partial class RunSetActionSendSMSEditPage : Page
    {
        public RunSetActionSendSMSEditPage(RunSetActionSendSMS runSetActionSendSMS)
        {
            InitializeComponent();

            if (runSetActionSendSMS.SMSEmail == null)
            {
                runSetActionSendSMS.SMSEmail = new Email();                
            }

            RunsetActionDeliveryMethodConfigPageFrame.Content = new RunSetActionDeliveryMethodConfigPage(runSetActionSendSMS);

            MailFromTextBox.Init(null, runSetActionSendSMS.SMSEmail, nameof(Email.MailFrom));
            MailToTextBox.Init(null, runSetActionSendSMS.SMSEmail, nameof(Email.MailTo));
            SubjectTextBox.Init(null, runSetActionSendSMS.SMSEmail, nameof(Email.Subject));
            BodyTextBox.Init(null, runSetActionSendSMS.SMSEmail, nameof(Email.Body));
            BodyTextBox.AdjustHight(100);

        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}