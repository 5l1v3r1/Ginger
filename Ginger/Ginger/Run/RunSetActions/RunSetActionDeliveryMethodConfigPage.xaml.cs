﻿using GingerCore.GeneralLib;
using System.Windows;
using System.Windows.Controls;
using GingerCore;
namespace Ginger.Run.RunSetActions
{
    /// <summary>
    /// Interaction logic for RunSetActionDeliveryMethodConfigPage.xaml
    /// </summary>
    public partial class RunSetActionDeliveryMethodConfigPage : Page
    {
        public RunSetActionDeliveryMethodConfigPage(RunSetActionSendSMS runSetActionSendSMS)
        {
            InitializeComponent();
            
            xSMTPMailHostTextBox.Init(null, runSetActionSendSMS.SMSEmail, nameof(Email.SMTPMailHost));
            BindingHandler.ObjFieldBinding(xSMTPPortTextBox, TextBox.TextProperty, runSetActionSendSMS.SMSEmail, nameof(Email.SMTPPort));
            xSMTPUserTextBox.Init(null, runSetActionSendSMS.SMSEmail, nameof(Email.SMTPUser));
            BindingHandler.ObjFieldBinding(xSMTPPassTextBox, TextBox.TextProperty, runSetActionSendSMS.SMSEmail, nameof(Email.SMTPPass));
            GingerCore.General.FillComboFromEnumObj(xEmailMethodComboBox, runSetActionSendSMS.SMSEmail.EmailMethod);
            BindingHandler.ObjFieldBinding(xEmailMethodComboBox, ComboBox.SelectedValueProperty, runSetActionSendSMS.SMSEmail, nameof(Email.EmailMethod));
        }
        public RunSetActionDeliveryMethodConfigPage(RunSetActionHTMLReportSendEmail runSetActionHTMlReportSendEmail)
        {
            InitializeComponent();

            xSMTPMailHostTextBox.Init(null, runSetActionHTMlReportSendEmail.Email, nameof(Email.SMTPMailHost));
            BindingHandler.ObjFieldBinding(xSMTPPortTextBox, TextBox.TextProperty, runSetActionHTMlReportSendEmail.Email, nameof(Email.SMTPPort));
            xSMTPUserTextBox.Init(null, runSetActionHTMlReportSendEmail.Email, nameof(Email.SMTPUser));
            BindingHandler.ObjFieldBinding(xSMTPPassTextBox, TextBox.TextProperty, runSetActionHTMlReportSendEmail.Email, nameof(Email.SMTPPass));
            GingerCore.General.FillComboFromEnumObj(xEmailMethodComboBox, runSetActionHTMlReportSendEmail.Email.EmailMethod);
            BindingHandler.ObjFieldBinding(xEmailMethodComboBox, ComboBox.SelectedValueProperty, runSetActionHTMlReportSendEmail.Email, nameof(Email.EmailMethod));
        }
        public RunSetActionDeliveryMethodConfigPage(RunSetActionSendFreeEmail runSetActionSendFreeEmail)
        {
            InitializeComponent();

            xSMTPMailHostTextBox.Init(null, runSetActionSendFreeEmail.Email, nameof(Email.SMTPMailHost));
            BindingHandler.ObjFieldBinding(xSMTPPortTextBox, TextBox.TextProperty, runSetActionSendFreeEmail.Email, nameof(Email.SMTPPort));
            xSMTPUserTextBox.Init(null, runSetActionSendFreeEmail.Email, nameof(Email.SMTPUser));
            BindingHandler.ObjFieldBinding(xSMTPPassTextBox, TextBox.TextProperty, runSetActionSendFreeEmail.Email, nameof(Email.SMTPPass));
            GingerCore.General.FillComboFromEnumObj(xEmailMethodComboBox, runSetActionSendFreeEmail.Email.EmailMethod);
            BindingHandler.ObjFieldBinding(xEmailMethodComboBox, ComboBox.SelectedValueProperty, runSetActionSendFreeEmail.Email, nameof(Email.EmailMethod));
        }
        private void xEmailMethodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (xEmailMethodComboBox.SelectedItem.ToString() == "OUTLOOK")
            {
                xSMTPConfig.Visibility = Visibility.Collapsed;
            }
            else
            {
                xSMTPConfig.Visibility = Visibility.Visible;
            }
        }
        private void xcbConfigureCredential_Checked(object sender, RoutedEventArgs e)
        {
            xSMTPUserTextBox.Visibility = Visibility.Visible;
            xSMTPPassTextBox.Visibility = Visibility.Visible;
            xLabelPass.Visibility = Visibility.Visible;
            xLabelUser.Visibility = Visibility.Visible;
        }
        private void xcbConfigureCredential_Unchecked(object sender, RoutedEventArgs e)
        {
            xSMTPUserTextBox.Visibility = Visibility.Collapsed;
            xSMTPPassTextBox.Visibility = Visibility.Collapsed;
            xLabelPass.Visibility = Visibility.Collapsed;
            xLabelUser.Visibility = Visibility.Collapsed;
        }
        private void xSMTPPassTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            bool res = false;
            if (!EncryptionHandler.IsStringEncrypted(xSMTPPassTextBox.Text))
            {
                xSMTPPassTextBox.Text = EncryptionHandler.EncryptString(xSMTPPassTextBox.Text, ref res);
                if (res == false)
                {
                    xSMTPPassTextBox.Text = string.Empty;
                }
            }
        }
    }
}
