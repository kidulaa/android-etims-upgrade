namespace System.Drawing
{
    internal class Printing
    {
        internal class PrintDocument
        {
            public PrintDocument()
            {
            }

            public Action<object, object> PrintPage { get; internal set; }

            internal void Print()
            {
                throw new NotImplementedException();
            }
        }
    }
}