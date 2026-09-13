using System;

namespace EBM2x.UI.Draw
{
    public class ExtEventArgs : EventArgs
    {
        public string FunctionID { get; set; }
        public string EnteredText { get; set; }
        public object EnteredObject { get; set; }

        public ExtEventArgs(string id, string text, object enteredObject)
        {
            FunctionID = id;
            if (string.IsNullOrEmpty(text)) EnteredText = "";
            else EnteredText = text;

            EnteredObject = enteredObject;
        }

        public ExtEventArgs(string id, object enteredObject)
        {
            FunctionID = id;
            EnteredText = string.Empty;

            EnteredObject = enteredObject;
        }
    }
}
