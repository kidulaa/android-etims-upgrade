using System;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace EBM2x.UI.Popup.Calculator
{    
    public partial class SimpleCalculatorPage : ContentPage
    {    
		int currentState = 1;
		string mathOperator;
		double firstNumber, secondNumber;

        public SimpleCalculatorPage()
        {
            SetValue(NavigationPage.HasNavigationBarProperty, false);

            InitializeComponent();
            fixedGrid.InitializeComponent();

            UIManager.Instance().PosModel.SetSalesTitleText("Calculator");
            UIManager.Instance().PosModel.InvalidateSurface();

            UIManager.Instance().InformationModel.SetWarningMessage("");
            UIManager.Instance().InformationModel.SetInformationMessage("Please enter");

            OnClear(this, null);
        }

		void OnSelectNumber(object sender, EventArgs e)
		{
            try
            {
                var duration = TimeSpan.FromSeconds(0.1);
                Vibration.Vibrate(duration);
            }
            catch (FeatureNotSupportedException ex)
            {
            }
            catch (Exception ex)
            {
            }
            Button button = (Button)sender;
			string pressed = button.Text;

			if (this.resultText.Text == "0" || currentState < 0) {
				this.resultText.Text = "";
				if (currentState < 0)
					currentState *= -1;
			}

			this.resultText.Text += pressed;

			double number;
			if (double.TryParse(this.resultText.Text, out number)) {
				this.resultText.Text = number.ToString("N0");
				if (currentState == 1) {
					firstNumber = number;
				} else {
					secondNumber = number;
				}
			}
		}

		void OnSelectOperator(object sender, EventArgs e)
		{
            try
            {
                var duration = TimeSpan.FromSeconds(0.1);
                Vibration.Vibrate(duration);
            }
            catch (FeatureNotSupportedException ex)
            {
            }
            catch (Exception ex)
            {
            }
            currentState = -2;
			Button button = (Button)sender;
			string pressed = button.Text;
			mathOperator = pressed;
		}

		void OnClear(object sender, EventArgs e)
		{
            try
            {
                var duration = TimeSpan.FromSeconds(0.1);
                Vibration.Vibrate(duration);
            }
            catch (FeatureNotSupportedException ex)
            {
            }
            catch (Exception ex)
            {
            }
            firstNumber = 0;
			secondNumber = 0;
			currentState = 1;
			this.resultText.Text = "0";
		}

        async void OnFunctionClose(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        void OnCalculate(object sender, EventArgs e)
		{
            try
            {
                var duration = TimeSpan.FromSeconds(0.1);
                Vibration.Vibrate(duration);
            }
            catch (FeatureNotSupportedException ex)
            {
            }
            catch (Exception ex)
            {
            }
            if (currentState == 2)
            {
                var result = SimpleCalculator.Calculate(firstNumber, secondNumber, mathOperator);

                this.resultText.Text = result.ToString();
                firstNumber = result;
                currentState = -1;
			}
		}
	}
}
