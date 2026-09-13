using EBM2x.RraSdc;
using EBM2x.RraSdc.process;
using EBM2x.UI.Draw;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Tablet
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetDateTimeMenuPage : ExtContentPage
    {

        bool pageIsActive;
        public SetDateTimeMenuPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);
            InitializeComponent();
            UIManager.Instance().InputModel.Clear();

            pageIsActive = true;
            AnimationLoop();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing(); 
            pageIsActive = true;

            UIManager.Instance().PosModel.SetSalesTitleText("Send|Transaction");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Please select a function.");

            MessagingCenter.Subscribe<Object, string>(this, "Function", (sender, arg) => {
                Device.BeginInvokeOnMainThread(() => {
                    OnFunctionCalled(this, new ExtEventArgs(arg, UIManager.Instance().InputModel.EnteredText));
                });
            });
        }
        protected override void OnDisappearing()
        {
            pageIsActive = false;
            MessagingCenter.Unsubscribe<Object, string>(this, "Function");

            base.OnDisappearing();
        }
        async void AnimationLoop()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(300.0));
                if (pageIsActive)
                {
                    RraSdcUploadProcess rraSdcUploadProcess = new RraSdcUploadProcess();
                    rraSdcUploadProcess.UploadProcess();
                }
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // false: BackButton 작동, true: BackButton 작동 중지
        }

        async void OnFunctionCalled(object sender, EventArgs e)
        {
            Console.WriteLine("FID:" + ((ExtEventArgs)e).FunctionID + ", TEXT:" + ((ExtEventArgs)e).EnteredText);
            
            int value = 0;
            if(!string.IsNullOrEmpty(UIManager.Instance().InputModel.EnteredText))
            {
                value = int.Parse(UIManager.Instance().InputModel.EnteredText);
            }            

            switch (((ExtEventArgs)e).FunctionID)
            {
                case "Back":
                    UIManager.Instance().InputModel.Clear();
                    Navigation.InsertPageBefore(new SalesMenuPage(), this);
                    await Navigation.PopAsync();
                    break;
                case "SetYear":
                    if(value < 2020 || value > 2100)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "The value is out of range.", "Ok");
                    }
                    else
                    {
                        SetDateTimeYear(value, value.ToString("0000"));
                    }
                    UIManager.Instance().InputModel.Clear();
                    break;
                case "SetMonth":
                    if (value < 1 || value > 12)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "The value is out of range.", "Ok");
                    }
                    else
                    {
                        SetDateTimeMonth(value, value.ToString("00"));
                    }
                    UIManager.Instance().InputModel.Clear();
                    break;
                case "SetDay":
                    if (value < 1 || value > 31)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "The value is out of range.", "Ok");
                    }
                    else
                    {
                        SetDateTimeDay(value, value.ToString("00"));
                    }
                    UIManager.Instance().InputModel.Clear();
                    break;
                case "SetHour":
                    if (value < 0 || value > 23)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "The value is out of range.", "Ok");
                    }
                    else
                    {
                        SetDateTimeHour(value, value.ToString("00"));
                    }
                    UIManager.Instance().InputModel.Clear();
                    break;
                case "SetMinute":
                    if (value < 0 || value > 59)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "The value is out of range.", "Ok");
                    }
                    else
                    {
                        SetDateTimeMinute(value, value.ToString("00"));
                    }
                    UIManager.Instance().InputModel.Clear();
                    break;
                case "SetSecond":
                    if (value < 0 || value > 59)
                    {
                        EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Error", "The value is out of range.", "Ok");
                    }
                    else
                    {
                        SetDateTimeSecond(value, value.ToString("00"));
                    }
                    UIManager.Instance().InputModel.Clear();
                    break;
                case "SetDateTime":
                    UIManager.Instance().InputModel.Clear();
                    SelectServerTime();
                    break;
                default:
                    break;
            }
        }
        public void SetDateTimeYear(int value, string year)
        {
            string yyyyMMddHHmmssmmm = year + DateTime.Now.ToString("MMddhhmmssmmm");
            ITimeSave timeSave = DependencyService.Get<ITimeSave>();
            if (timeSave != null) timeSave.SetSystemDateTime(yyyyMMddHHmmssmmm);
        }
        public void SetDateTimeMonth(int value, string month)
        {
            string yyyyMMddHHmmssmmm = DateTime.Now.ToString("yyyy") + month + DateTime.Now.ToString("ddhhmmssmmm");
            ITimeSave timeSave = DependencyService.Get<ITimeSave>();
            if (timeSave != null) timeSave.SetSystemDateTime(yyyyMMddHHmmssmmm);
        }
        public void SetDateTimeDay(int value, string day)
        {
            string yyyyMMddHHmmssmmm = DateTime.Now.ToString("yyyyMM") + day + DateTime.Now.ToString("hhmmssmmm");
            ITimeSave timeSave = DependencyService.Get<ITimeSave>();
            if (timeSave != null) timeSave.SetSystemDateTime(yyyyMMddHHmmssmmm);
        }
        public void SetDateTimeHour(int value, string hour)
        {
            string yyyyMMddHHmmssmmm = DateTime.Now.ToString("yyyyMMdd") + hour + DateTime.Now.ToString("mmssmmm");
            ITimeSave timeSave = DependencyService.Get<ITimeSave>();
            if (timeSave != null) timeSave.SetSystemDateTime(yyyyMMddHHmmssmmm);
        }
        public void SetDateTimeMinute(int value, string minute)
        {
            string yyyyMMddHHmmssmmm = DateTime.Now.ToString("yyyyMMddhh") + minute + DateTime.Now.ToString("ssmmm");
            ITimeSave timeSave = DependencyService.Get<ITimeSave>();
            if (timeSave != null) timeSave.SetSystemDateTime(yyyyMMddHHmmssmmm);
        }
        public void SetDateTimeSecond(int value, string second)
        {
            string yyyyMMddHHmmssmmm = DateTime.Now.ToString("yyyyMMddhhmm") + second + DateTime.Now.ToString("mmm");
            ITimeSave timeSave = DependencyService.Get<ITimeSave>();
            if (timeSave != null) timeSave.SetSystemDateTime(yyyyMMddHHmmssmmm);
        }
        public async void SelectServerTime()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = RraSdcService.EXTERNAL_URL + "/" + "selectServerTime";
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpWebRequest.Method = "POST"; //POST
                    httpWebRequest.ContentType = "application/x-www-form-urlencoded";

                    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    Stream responseStream = httpWebResponse.GetResponseStream();
                    StreamReader readerPost = new StreamReader(responseStream, Encoding.UTF8);

                    string jsonResponse = readerPost.ReadToEnd().Trim();
                    if (!string.IsNullOrEmpty(jsonResponse) && jsonResponse.Length == 14)
                    {
                        string NowDate = DateTime.Now.ToString("yyyyMMdd");
                        string ServerDate = jsonResponse.Substring(0, 8);
                        if (NowDate != ServerDate)
                        {
                            // 시간 변경
                            EBM2x.UI.UiUtils.MsgBox.DisplayAlert(this, "Confirm", "Change the date time.. [" + jsonResponse + "]", "Ok");
                            ITimeSave timeSave = DependencyService.Get<ITimeSave>();
                            if (timeSave != null) await timeSave.SetSystemDateTime(jsonResponse);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}

