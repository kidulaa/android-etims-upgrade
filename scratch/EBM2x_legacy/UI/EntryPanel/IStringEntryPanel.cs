using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.UI.EntryPanel
{
    public interface IStringEntryPanel
    {
        void TitleInvalidateSurface();
        void TitleInvalidateSurface(string text);
        void TitleInvalidateSurface(string text, string textAlign, string textColor, float fillRate);
        string GetEntryValue();
        void SetEntryValue(string stringValue);
        void SetFocus();
        bool IsValid();
    }
}
