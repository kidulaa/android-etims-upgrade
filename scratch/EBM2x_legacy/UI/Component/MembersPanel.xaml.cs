using EBM2x.Models.tran;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MembersPanel : ContentView
    {
        public MembersPanel()
        {
            InitializeComponent();
        }

        public void InvalidateSurface(TranNode tranNode)
        {
            if(string.IsNullOrEmpty(tranNode.CustomerNode.CustomerCode))
            {
                tinText.InvalidateSurface("PIN :");
            }
            else
            {
                string text = string.Format("PIN :{0} {1}", tranNode.CustomerNode.Tin, tranNode.CustomerNode.CustomerName);
                tinText.InvalidateSurface(text);
            }
        }

        void OnButtonClicked(object sender, EventArgs e)
        {
        }
    }
}
