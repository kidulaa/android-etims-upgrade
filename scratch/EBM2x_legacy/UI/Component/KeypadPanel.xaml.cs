using EBM2x.UI.Draw;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class KeypadPanel : ContentView
    {
        public event EventHandler FunctionCalled;
        public static readonly BindableProperty FunctionCalledProperty = BindableProperty.Create(
                                                         propertyName: "FunctionCalled",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(KeypadPanel),
                                                         defaultValue: null,
                                                         defaultBindingMode: BindingMode.OneWay);

        public KeypadPanel()
        {
            InitializeComponent();
        }

        void OnNumberButtonClicked(object sender, EventArgs e)
        {
            UIManager.Instance().InputModel.Append(((ExtEventArgs)e).FunctionID);
            UIManager.Instance().InformationModel.SetWarningMessage(""); // JINIT_메시지 Clear 추가
        }

        void OnBackspaceClicked(object sender, EventArgs e)
        {
            if(UIManager.Instance().InputModel.EnteredText.Length > 0)
            {
                UIManager.Instance().InputModel.DeleteLastOne();
            }
            else
            {
                if (FunctionCalled != null)
                    FunctionCalled?.Invoke(this, new ExtEventArgs(((ExtEventArgs)e).FunctionID, UIManager.Instance().InputModel.EnteredText));
            }
            UIManager.Instance().InformationModel.SetWarningMessage("");
        }

        void OnClearClicked(object sender, EventArgs e)
        {
            if(UIManager.Instance().InputModel.EnteredText.Length > 0)
            {
                UIManager.Instance().InputModel.Clear();
            }
            else
            {
                if (FunctionCalled != null)
                    FunctionCalled?.Invoke(this, new ExtEventArgs(((ExtEventArgs)e).FunctionID, UIManager.Instance().InputModel.EnteredText));
            }
            UIManager.Instance().InformationModel.SetWarningMessage("");
        }

        void OnEnterClicked(object sender, EventArgs e)
        {
            UIManager.Instance().InformationModel.SetWarningMessage(""); // JINIT_메시지 Clear 추가

            if (FunctionCalled != null)
                FunctionCalled?.Invoke(this, new ExtEventArgs(((ExtEventArgs)e).FunctionID, UIManager.Instance().InputModel.EnteredText));
        }
    }
}
