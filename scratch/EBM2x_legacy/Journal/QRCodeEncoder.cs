using System;

namespace EBM2x.Journal
{
    internal class QRCodeEncoder
    {
        public QRCodeEncoder()
        {
        }

        public int QRCodeScale { get; internal set; }

        internal object Encode(string text)
        {
            throw new NotImplementedException();
        }
    }
}