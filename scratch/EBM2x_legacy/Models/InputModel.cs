using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace EBM2x.Models
{
    public class InputModel
    {
        public string EnteredText { get; set; }
        public bool IsPassword { get; set; }

        public InputModel()
        {
            EnteredText = "";
            IsPassword = false;
        }

        public void Append(string text)
        {
            if(text.Equals(".")) // 2021.07.24
            {
                if (EnteredText.IndexOf(".") >= 0) return;
            }
            if (EnteredText.IndexOf(".") >= 0) // 2021.07.24
            {
                if(EnteredText.Length + text.Length > EnteredText.IndexOf(".") + 3) return;
            }
            EnteredText += text;
            if (IsPassword)
            {
                MessagingCenter.Send<Object, string>(this, "It was entered", GetEchoText(EnteredText));
            }
            else
            {
                MessagingCenter.Send<Object, string>(this, "It was entered", EnteredText);
            }
        }

        public void Clear()
        {
            EnteredText = "";
            MessagingCenter.Send<Object, string>(this, "It was entered", EnteredText);
        }

        public void DeleteLastOne()
        {
            if(EnteredText.Length > 1)
            {
                EnteredText = EnteredText.Substring(0, EnteredText.Length - 1);
                if (IsPassword)
                {
                    MessagingCenter.Send<Object, string>(this, "It was entered", GetEchoText(EnteredText));
                }
                else
                {
                    MessagingCenter.Send<Object, string>(this, "It was entered", EnteredText);
                }
            }
            else if (EnteredText.Length == 1)
            {
                EnteredText = "";
                MessagingCenter.Send<Object, string>(this, "It was entered", EnteredText);
            }
        }

        public string GetEchoText(string text)
        {
            string chars = "**************************************************";
            string buffer = chars.Substring(0, (text.Length - 1));
            buffer += text.Substring(text.Length - 1);

            return buffer;
        }
    }
}
