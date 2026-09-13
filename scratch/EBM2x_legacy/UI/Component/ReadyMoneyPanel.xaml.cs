using EBM2x.Models.ReadyMoney;
using EBM2x.Models.State;
using EBM2x.Models.tran;
using EBM2x.UI.Draw;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReadyMoneyPanel : ContentView
    {
        StateNodeList stateNodeList = null;

        public event EventHandler ButtonClicked;
        public static readonly BindableProperty ButtonClickedProperty = BindableProperty.Create(
                                                         propertyName: "ButtonClicked",
                                                         returnType: typeof(EventHandler),
                                                         declaringType: typeof(ReadyMoneyPanel),
                                                         defaultValue: null,
                                                         defaultBindingMode: BindingMode.OneWay);

        public ReadyMoneyPanel()
        {
            InitializeComponent();

            stateNodeList = new StateNodeList();

            stateNodeList.Add(new StateNode
            {
                Information = "Please enter a quantity of 5,000 KES",
                ContentView = itemNode01
            });
            stateNodeList.Add(new StateNode
            {
                Information = "Please enter a quantity of 2,000 KES",
                ContentView = itemNode02
            });
            stateNodeList.Add(new StateNode
            {
                Information = "Please enter a quantity of 1,000 KES",
                ContentView = itemNode03
            });
            stateNodeList.Add(new StateNode
            {
                Information = "Please enter a quantity of 500 KES",
                ContentView = itemNode04
            });
            stateNodeList.Add(new StateNode
            {
                Information = "Please enter a quantity of 100 KES",
                ContentView = itemNode05
            });
            stateNodeList.Add(new StateNode
            {
                Information = "Please enter a quantity of 50 KES",
                ContentView = itemNode06
            });
            stateNodeList.Add(new StateNode
            {
                Information = "Please enter a quantity of 20 KES",
                ContentView = itemNode07
            });
            stateNodeList.Add(new StateNode
            {
                Information = "Please enter a quantity of 10 KES",
                ContentView = itemNode08
            });
            stateNodeList.Add(new StateNode
            {
                Information = "Please enter a quantity of 5 KES",
                ContentView = itemNode09
            });

            stateNodeList.SetCurrent(1);
        }

        public void InvalidateSurface(ReadyMoneyList arg)
        {
            bool currentFlag = false;
            for (int i = 0; i < arg.Count(); i++)
            {
                currentFlag = false;
                if (arg.CurrentLineNumber == (i + 1)) currentFlag = true;
                ((ReadyMoneyItemView)(stateNodeList.Get(i).ContentView)).InvalidateSurface(arg.Get(i), currentFlag);
            }
            totalAmountNumber.InvalidateSurface(arg.GetTotal(), true);

            stateNodeList.CurrentLineNumber = arg.CurrentLineNumber;
            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage(stateNodeList.Get(stateNodeList.CurrentLineNumber - 1).Information);
        }

        void OnBoxButtonClicked(object sender, EventArgs e)
        {
            if (ButtonClicked != null) ButtonClicked?.Invoke(this, e);
        }
    }
}
