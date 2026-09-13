using EBM2x.Database;
using EBM2x.Process.import_export;
using EBM2x.UI.Draw;
using EBM2x.UI.i18n;
using EBM2x.UI.Setup;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Tablet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminMenuPage : ContentPage
    {
        public AdminMenuPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();

            fixedGrid00.InitializeComponent();
            fixedGrid01.InitializeComponent();
            extFunctionPanel.InitializeComponent();

            extFunctionPanel.IsVisible = true;
            extMenuButton.InvalidateSurface(extFunctionPanel.IsVisible);
            UIManager.Instance().InputModel.Clear(); // JINIT_201911, Input Clear 추가

            if (UIManager.Instance().IsWindows)
            {
                extExport.IsVisible = false;
            }
            else
            {
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<Object, string>(this, "Function", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    OnFunctionCalled(this, new ExtEventArgs(arg, UIManager.Instance().InputModel.EnteredText));
                });
            });

            UIManager.Instance().PosModel.SetSalesTitleText("Admin Menu");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Please select a function");
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<Object, string>(this, "Function");

            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        async void OnFunctionCalled(object sender, EventArgs e)
        {
            Console.WriteLine("FID:" + ((ExtEventArgs)e).FunctionID + ", TEXT:" + ((ExtEventArgs)e).EnteredText);

            switch (((ExtEventArgs)e).FunctionID)
            {
                case "PosSetup":
                    UIManager.Instance().InputModel.Clear();
                    await Navigation.PushAsync(new EBM2xSetupPage());
                    break;

                case "PosReSetup":
                    UIManager.Instance().InputModel.Clear();
                    await Navigation.PushAsync(new EBM2xSetupPage(true));
                    break;

                case "ExtMenu":
                    // Extend the menu.
                    extFunctionPanel.IsVisible = !extFunctionPanel.IsVisible;
                    extMenuButton.InvalidateSurface(extFunctionPanel.IsVisible);
                    break;

                case "Back":
                    UIManager.Instance().InputModel.Clear();
                    await Navigation.PopAsync();
                    break;

                case "CopyDatabase":
                    UIManager.Instance().InputModel.Clear();
                    try
                    {
                        EBM2xMsSQLiteClientProvider providerCopyDatabase = new EBM2xMsSQLiteClientProvider();
                        bool ret = providerCopyDatabase.CopySQLiteDatabase();
                        if (ret)
                        {
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "CopySQLiteDatabase", "OK");
                        }
                        else
                        {
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "CopySQLiteDatabase", "OK");
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", ex.ToString(), "OK");
                    }
                    break;

                case "ExportDatabase":
                    UIManager.Instance().InputModel.Clear();
                    try
                    {
                        ISave iSave = DependencyService.Get<ISave>();
                        if (iSave != null)
                        {
                            if (iSave.IsSdCard())
                            {
                                await iSave.ExportData();
                                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Exported to SD Card 'Android/data/com.companyname.ebm2x/EBM2xBackup'.", "OK");
                            }
                            else
                            {
                                EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Failed", "Check the SD Card.", "OK");
                            }
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", ex.ToString(), "OK");
                    }
                    break;
                case "ImportDatabase":
                    UIManager.Instance().InputModel.Clear();
                    try
                    {
                        FileData filedata = await CrossFilePicker.Current.PickFile();
                        if (filedata != null && !string.IsNullOrEmpty(filedata.FileName))
                        {
                            string filepath = filedata.FilePath;
                            EBM2xDBClientProvider bBM2xDBClientProvider = EBM2xDBClientProvider.getInstance();
                            if (bBM2xDBClientProvider != null)
                            {
                                string locationTitle2 = UILocation.Instance().GetLocationText("Combine?");
                                string locationMessage2 = UILocation.Instance().GetLocationText("Do you want to proceed?");
                                var ret = await DisplayAlert(locationTitle2, locationMessage2, "Yes", "No");
                                if (!ret) break;

                                if (UIManager.Instance().IsWindows && UIManager.Instance().IsMySQL)
                                {
                                    string DBServer = UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLServer;
                                    string Database = UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLDatabase;
                                    string DBUid = UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLUid;
                                    string DBPwd = UIManager.Instance().PosModel.Environment.EnvPosSetup.MySQLPwd;
                                    bBM2xDBClientProvider.OpenConnection(DBServer, Database, DBUid, DBPwd);
                                }
                                else
                                {
                                    bBM2xDBClientProvider.OpenConnection("", "", "", "");
                                }
                                //bBM2xDBClientProvider.OpenConnection();
                                using (var command = bBM2xDBClientProvider.GetDbCommand())
                                {
                                    int count = 0;
                                    string message = "";
                                    string buffer = "";
                                    string line;
                                    System.IO.StreamReader file = new System.IO.StreamReader(filepath);
                                    while ((line = file.ReadLine()) != null)
                                    {
                                        if (string.IsNullOrEmpty(line)) continue;

                                        if (line.IndexOf(";") >= 0)
                                        {
                                            buffer = buffer + " " + line;

                                            try
                                            {
                                                command.CommandText = buffer;
                                                command.ExecuteNonQuery();
                                                count++;

                                                if(count % 1000 == 0)
                                                {
                                                    await Task.Delay(TimeSpan.FromSeconds(0.5));
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                if(ex.Message.IndexOf("UNIQUE constraint failed") > -1)
                                                {
                                                    message = ex.Message;
                                                }
                                                else
                                                {
                                                    message = ex.Message;
                                                }
                                            }

                                            buffer = "";
                                        }
                                        else
                                        {
                                            buffer = buffer + " " + line;
                                        }
                                    }
                                    file.Close();
                                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeded", "Completed", "OK");
                                }
                            }
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", ex.ToString(), "OK");
                    }

                    break;
                case "MigrationDB":
                    MigrationDBProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    break;
                case "CreateDB":
                    CreateDBProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    break;
                case "CreateTable":
                    CreateTableProcess.excuteProcess(UIManager.Instance().PosModel, UIManager.Instance().InputModel, UIManager.Instance().InformationModel);
                    EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Succeeded", "Created Table", "OK");
                    break;
                default:
                    break;
            }
        }
    }
}
