using EBM2x.Models.DiningTable;
using EBM2x.UI.Draw;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EBM2x.UI.Component
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DiningTablePanel : ContentView
    {
        public string BoxColor
        {
            get { return base.GetValue(BoxColorProperty).ToString(); }
            set { base.SetValue(BoxColorProperty, value); }
        }
        public static readonly BindableProperty BoxColorProperty = BindableProperty.Create(
                                                         propertyName: "BoxColor",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(DiningTablePanel),
                                                         defaultValue: "daaaf0",
                                                         defaultBindingMode: BindingMode.TwoWay);
        public DiningTablePanel()
        {
            InitializeComponent();

            groupBox.IsVisible = false;
            groupNum.IsVisible = false;

        }

        public void InvalidateSurface(DiningTableNode itemNode)
        {
            if (itemNode != null && itemNode.IsVisible)
            {
                noNumber.InvalidateSurface(itemNode.DiningTableCode);

                if (itemNode.IsGroup)
                {
                    groupBox.IsVisible = true;
                    groupNum.IsVisible = true;
                    groupBox.InvalidateSurface(itemNode.GroupCode);
                    groupNum.InvalidateSurface(itemNode.GroupCode);
                }
                else
                {
                    groupNum.InvalidateSurface("");
                    groupBox.IsVisible = false;
                    groupNum.IsVisible = false;
                }

                if (itemNode.IsOrdered)
                {
                    backgroungBox.InvalidateSurface("9ef7c9");

                    firstOrderText.InvalidateSurface(" " + itemNode.GetFirstOrder() + " (since then)");
                    durationText.InvalidateSurface(" " + itemNode.GetDurationTime() + " (duration of stay)");

                    totalTitle.InvalidateSurface("Total:");
                    totalNumber.InvalidateSurface(itemNode.Amount, true);
                }
            }
        }

        void OnButtonClicked(object sender, EventArgs e)
        {
        }
    }
}
