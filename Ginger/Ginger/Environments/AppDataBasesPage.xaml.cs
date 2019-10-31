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
using Amdocs.Ginger.Common.Enums;
using Ginger.DatabaseLib;
using Ginger.UserControls;
using GingerCore;
using GingerCore.Actions;
using GingerCore.DataSource;
using GingerCore.Environments;
using GingerWPF.WizardLib;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ginger.Environments
{
    /// <summary>
    /// Interaction logic for AppDataBasesWindow.xaml
    /// </summary>
    public partial class AppDataBasesPage : Page
    {
        public EnvApplication AppOwner { get; set; }

        Context mContext;

        public AppDataBasesPage(EnvApplication applicationOwner, Context context)
        {
            InitializeComponent();
            AppOwner = applicationOwner;
            mContext = context;
            //Set grid look and data
            SetGridView();
            SetGridData();
            //Added for Encryption
            if (grdAppDbs.grdMain != null)
            {
                grdAppDbs.grdMain.CellEditEnding += grdMain_CellEditEnding;
                grdAppDbs.grdMain.PreparingCellForEdit += grdMain_PreparingCellForEdit;
            }            
        }

        private void grdMain_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            if (e.Column.Header.ToString() == nameof(Database.Name))
            {
                Database selectedDB = (Database)grdAppDbs.CurrentItem;
                selectedDB.NameBeforeEdit = selectedDB.Name;
            }
        }

        private async void grdMain_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.Header.ToString() == "User Password")
            {
                Database selectedEnvDB = (Database)grdAppDbs.CurrentItem;
                String intialValue = selectedEnvDB.Pass;
                if (!string.IsNullOrEmpty(intialValue))
                {
                    bool res = false;
                    if (!EncryptionHandler.IsStringEncrypted(intialValue))
                    {
                        selectedEnvDB.Pass = EncryptionHandler.EncryptString(intialValue, ref res);
                        if (res == false)
                        {
                            selectedEnvDB.Pass = null;
                        }
                    }                   
                }
            }

            if (e.Column.Header.ToString() == nameof(Database.Name))
            {
                Database selectedDB = (Database)grdAppDbs.CurrentItem;
                if (selectedDB.Name != selectedDB.NameBeforeEdit)
                {
                    await UpdateDatabaseNameChange(selectedDB);
                }
            }
        }

        public async Task UpdateDatabaseNameChange(Database db)
        {
            if (db == null)
            {
                return;
            }
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                await Task.Run(() =>
                {
                    Reporter.ToStatus(eStatusMsgKey.RenameItem, null, db.NameBeforeEdit, db.Name);
                    ObservableList<BusinessFlow> allBF = WorkSpace.Instance.SolutionRepository.GetAllRepositoryItems<BusinessFlow>();
                    Parallel.ForEach(allBF, new ParallelOptions { MaxDegreeOfParallelism = 5 }, businessFlow =>
                    {
                        Parallel.ForEach(businessFlow.Activities, new ParallelOptions { MaxDegreeOfParallelism = 5 }, activity =>
                        {
                            Parallel.ForEach(activity.Acts, new ParallelOptions { MaxDegreeOfParallelism = 5 }, act =>
                            {
                                if (act.GetType() == typeof(ActDBValidation))
                                {
                                    ActDBValidation actDB = (ActDBValidation)act;
                                    if (actDB.DBName == db.NameBeforeEdit)
                                    {
                                        businessFlow.DirtyStatus = eDirtyStatus.Modified;
                                        actDB.DBName = db.Name;
                                    }
                                }

                            });
                        });
                    });
                });

                db.NameBeforeEdit = db.Name;
            }
            catch (Exception ex)
            {
                Reporter.ToLog(eLogLevel.DEBUG, "Error occurred while renaming DBName", ex);
            }
            finally
            {
                Reporter.HideStatusMessage();
                Mouse.OverrideCursor = null;
            }
        }

        #region Events
        private void TestDBConnection(object sender, RoutedEventArgs e)
        {
            try
            {
                Database db = (Database)grdAppDbs.grdMain.SelectedItem;                                
                if (db == null)
                {
                    Reporter.ToUser(eUserMsgKey.AskToSelectItem);
                    return;
                }

                db.DSList = WorkSpace.Instance.SolutionRepository.GetAllRepositoryItems<DataSourceBase>();
                db.ProjEnvironment = mContext.Environment;
                db.BusinessFlow =  null;
                if (string.IsNullOrEmpty(db.ConnectionString) && !string.IsNullOrEmpty(db.TNS) && db.TNS.ToLower().Contains("data source=") && db.TNS.ToLower().Contains("password=") && db.TNS.ToLower().Contains("user id="))
                {
                    System.Data.SqlClient.SqlConnectionStringBuilder scSB = new System.Data.SqlClient.SqlConnectionStringBuilder();
                    scSB.ConnectionString = db.TNS;
                    db.TNS = scSB.DataSource;
                    db.User = scSB.UserID;
                    db.Pass = scSB.Password;
                    db.ConnectionString = scSB.ConnectionString;
                }
                if (!string.IsNullOrEmpty(db.TNS) && string.IsNullOrEmpty(db.ConnectionString))
                {
                    db.ConnectionString = db.TNS;
                }
                // db.CloseConnection();
                if (db.TestConnection() == true)
                {
                    Reporter.ToUser(eUserMsgKey.DbConnSucceed);
                }
                else
                {
                    Reporter.ToUser(eUserMsgKey.DbConnFailed);
                }
                db.CloseConnection();
            }
            catch (Exception ex)
            {

                // TODO: remove  !!!!!!!!!!!!!!!!!!!!!!
                if (ex.Message.Contains("Oracle.ManagedDataAccess.dll is missing"))
                {
                    if (Reporter.ToUser(eUserMsgKey.OracleDllIsMissing, AppDomain.CurrentDomain.BaseDirectory) == Amdocs.Ginger.Common.eUserMsgSelection.Yes)
                    {
                        System.Diagnostics.Process.Start("https://docs.oracle.com/database/121/ODPNT/installODPmd.htm#ODPNT8149");
                        System.Threading.Thread.Sleep(2000);
                        System.Diagnostics.Process.Start("http://www.oracle.com/technetwork/topics/dotnet/downloads/odacdeploy-4242173.html"); 
                        
                    }
                    return;
                }
                
               Reporter.ToUser(eUserMsgKey.ErrorConnectingToDataBase, ex.Message);
            }
        }
        #endregion Events

        #region Functions
        private void SetGridView()
        {
            //Set the Tool Bar look
            grdAppDbs.ShowEdit = Visibility.Collapsed;
            grdAppDbs.ShowUpDown = Visibility.Collapsed;
            grdAppDbs.ShowUndo = Visibility.Visible;
            grdAppDbs.ShowHeader = Visibility.Collapsed;
            grdAppDbs.AddToolbarTool(eImageType.DataSource, "Test Connection", new RoutedEventHandler(TestDBConnection));

            grdAppDbs.btnAdd.AddHandler(Button.ClickEvent, new RoutedEventHandler(AddNewDB));

            //Set the Data Grid columns
            GridViewDef view = new GridViewDef(GridViewDef.DefaultViewName);
            view.GridColsView = new ObservableList<GridColView>();

            view.GridColsView.Add(new GridColView() { Field = nameof(Database.Name), WidthWeight = 20 });
            view.GridColsView.Add(new GridColView() { Field = nameof(Database.Description), WidthWeight = 30 });
            view.GridColsView.Add(new GridColView() { Field = nameof(Database.DBVer), Header = "Version", WidthWeight = 10 });

            view.GridColsView.Add(new GridColView() { Field = nameof(Database.ServiceID), WidthWeight = 10, ReadOnly = true, Header = "DB Service" });
            // OLD to be removed
            view.GridColsView.Add(new GridColView() { Field = nameof(Database.DBType), WidthWeight = 10, StyleType = GridColView.eGridColStyleType.ComboBox, CellValuesList = Database.DbTypes, Header = "DB Type" });
            view.GridColsView.Add(new GridColView() { Field = nameof(Database.TNS), Header="TNS / File Path / Host ", WidthWeight = 30 });
            view.GridColsView.Add(new GridColView() { Field = "VE1", Header="...", WidthWeight = 5, MaxWidth = 30, StyleType = GridColView.eGridColStyleType.Template, CellTemplate = (DataTemplate)this.appDataBasesWindowGrid.Resources["TNSValueExpressionButton"] });
            view.GridColsView.Add(new GridColView() { Field = nameof(Database.User), Header="User Name", WidthWeight = 10 });
            view.GridColsView.Add(new GridColView() { Field = "VE2", Header = "...", WidthWeight = 5, MaxWidth = 30, StyleType = GridColView.eGridColStyleType.Template, CellTemplate = (DataTemplate)this.appDataBasesWindowGrid.Resources["UserValueExpressionButton"] });
            view.GridColsView.Add(new GridColView() { Field = nameof(Database.Pass),Header="User Password", WidthWeight = 10 });
            view.GridColsView.Add(new GridColView() { Field = "VE3", Header = "...", WidthWeight = 5, MaxWidth = 30, StyleType = GridColView.eGridColStyleType.Template, CellTemplate = (DataTemplate)this.appDataBasesWindowGrid.Resources["PswdValueExpressionButton"] });
            view.GridColsView.Add(new GridColView() { Field = nameof(Database.ConnectionString), WidthWeight = 20, Header = "Connection String (Optional)" });
            view.GridColsView.Add(new GridColView() { Field = "VE4", Header = "...", WidthWeight = 5, MaxWidth = 30, StyleType = GridColView.eGridColStyleType.Template, CellTemplate = (DataTemplate)this.appDataBasesWindowGrid.Resources["ConnStrValueExpressionButton"] });
            view.GridColsView.Add(new GridColView() { Field = nameof(Database.KeepConnectionOpen), Header = "Keep Connection Open" , StyleType= GridColView.eGridColStyleType.CheckBox, MaxWidth = 150, WidthWeight=10 });
            grdAppDbs.SetAllColumnsDefaultView(view);
            grdAppDbs.InitViewItems();
        }
        private void AddNewDB(object sender, RoutedEventArgs e)
        {
            //Database db = new Database();
            //db.Name = "New";
            //grdAppDbs.DataSourceList.Add(db);

            // Prep for getting list of DBs from installed plugins

            //TODO: Open wizard !!!

            WizardWindow.ShowWizard(new AddDatabaseWizard(AppOwner));            

        }

        private void SetGridData()
        {
            grdAppDbs.DataSourceList = AppOwner.Dbs;
        }
        #endregion Functions

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            grdAppDbs.grdMain.CommitEdit();
            grdAppDbs.grdMain.CancelEdit();
        }

        private void GridTNSVEButton_Click(object sender, RoutedEventArgs e)
        {
            Database selectedEnvDB = (Database)grdAppDbs.CurrentItem;
            ValueExpressionEditorPage VEEW = new ValueExpressionEditorPage(selectedEnvDB, nameof(Database.TNS), null);
            VEEW.ShowAsWindow();
        }

        private void GridUserVEButton_Click(object sender, RoutedEventArgs e)
        {
            Database selectedEnvDB = (Database)grdAppDbs.CurrentItem;
            ValueExpressionEditorPage VEEW = new ValueExpressionEditorPage(selectedEnvDB, nameof(Database.User), null);
            VEEW.ShowAsWindow();
        }

        private void GridPswdVEButton_Click(object sender, RoutedEventArgs e)
        {
            Database selectedEnvDB = (Database)grdAppDbs.CurrentItem;
            ValueExpressionEditorPage VEEW = new ValueExpressionEditorPage(selectedEnvDB, nameof(Database.Pass), null);
            VEEW.ShowAsWindow();
        }

        private void GridConnStrVEButton_Click(object sender, RoutedEventArgs e)
        {
            Database selectedEnvDB = (Database)grdAppDbs.CurrentItem;
            ValueExpressionEditorPage VEEW = new ValueExpressionEditorPage(selectedEnvDB, nameof(Database.ConnectionString), null);
            VEEW.ShowAsWindow();
        }

        private void grdAppDbs_SelectedItemChanged(object selectedItem)
        {
            Database database = (Database)selectedItem;
            xDBNameTextBox.BindControl(database, nameof(Database.Name));            
            xParamsGrid.ItemsSource = database.DBParmas;
            xConnectionStringTextBox.ClearControlsBindings();
            xConnectionStringTextBox.BindControl(database, nameof(Database.ConnectionString));
            // TODO: get parameters and show in frame use same mechanism like plugins
        }
    }
}
