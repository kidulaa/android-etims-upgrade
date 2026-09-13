using EBM2x.UI.i18n;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace EBM2x.Models
{
    public class InformationModel
    {
        private string warningMessage = "";
        private string informationMessage = "";
        public string AlertTitle { get; set; }
        public string AlertMessage { get; set; }

        public InformationModel()
        {
            warningMessage = "";
            informationMessage = "";
            AlertTitle = string.Empty;
            AlertMessage = string.Empty;
        }

        public void SetAlertMessage(string title, string message)
        {
            AlertTitle = UILocation.Instance().GetLocationText(title);
            AlertMessage = UILocation.Instance().GetLocationText(message);
        }

        public void SetWarningMessage(string message)
        {
            warningMessage = UILocation.Instance().GetLocationText(message);
            MessagingCenter.Send<Object, string>(this, "Warning Message", warningMessage);
        }

        public void SetInformationMessage(string message)
        {
            informationMessage = UILocation.Instance().GetLocationText(message);
            MessagingCenter.Send<Object, string>(this, "Information Message", informationMessage);
        }
    }
}
