using EBM2x.Database.MasterEbm2x;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EBM2x.Dependency
{
    public interface IBlueToothService
    {
        bool IsAndoid();
        string GetSmsMessage(Models.journal.JournalModel journal);
        bool SendSMS(string phoneNumber, string message);

        string GetDeviceName();
        IList<string> GetDeviceList();
        void PrintJournal(string port, int baudRate, Models.journal.JournalModel journal, bool isReprint);
        void PrintJournalA4(Models.journal.JournalModel journal, bool isReprint);
        void PrintInvoiceA4(TransactionSalesModel salesModel,
            Models.journal.JournalModel journal1,
            Models.journal.JournalModel journal2,
            Models.journal.JournalModel journal3,
            Models.journal.JournalModel journal4,
            bool isReprint,
            Models.journal.JournalModel journal5);
       
    }
}
