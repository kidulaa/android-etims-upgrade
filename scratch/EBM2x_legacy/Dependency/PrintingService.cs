using EBM2x.Database.MasterEbm2x;
using EBM2x.UI;
using System.Collections.Generic;
using Xamarin.Forms;

namespace EBM2x.Dependency
{
    public class PrintingService
    {
        private readonly IBlueToothService _blueToothService;
        IList<string> Services;

        public PrintingService()
        {
            _blueToothService = DependencyService.Get<IBlueToothService>();
            Services = _blueToothService.GetDeviceList();
        }

        public bool IsAndoid()
        {
            return _blueToothService.IsAndoid();
        }

        public string GetSmsMessage(Models.journal.JournalModel journal)
        {
            return _blueToothService.GetSmsMessage(journal);
        }

        public bool SendSMS(string phoneNumber, string message)
        {
            return _blueToothService.SendSMS(phoneNumber, message);
        }

        public async void writeJurnal(Models.journal.JournalModel journal, bool IsReprint)
        {
            string port = UIManager.Instance().PosModel.Environment.EnvPosSetup.PrinterPort;
            int baudRate = UIManager.Instance().PosModel.Environment.EnvPosSetup.PrinterBaudRate;

            // added by clinton

            if (baudRate >= 9600)
            {
               // baudRate = 4800;

            }

            if (port.Equals(null))
            {
                port = "Printer";

            }

            //

            string service = _blueToothService.GetDeviceName();
            if (!service.Equals("windows"))
            {
                if (!string.IsNullOrEmpty(port)) service = port;
                for (int i = 0; i < Services.Count; i++)
                {
                    if (service.Equals(Services[i]))
                    {
                      _blueToothService.PrintJournal(port, baudRate, journal, IsReprint);
                        return;
                    }
                }
                return;
            }
            else
            {
               _blueToothService.PrintJournal(port, baudRate, journal, IsReprint);
                //await System.Threading.Tasks.Task.Delay(300);
                System.Threading.Thread.Sleep(4000);
            }
        }
        public void writeJurnalA4(Models.journal.JournalModel journal5, bool IsReprint)
        {
            string service = _blueToothService.GetDeviceName();
            //if (service.Equals("windows"))
            //{
                _blueToothService.PrintJournalA4(journal5, IsReprint);
            //}
        }
        public void writeInvoiceA4(TransactionSalesModel salesModel,
            Models.journal.JournalModel journal1,
            Models.journal.JournalModel journal2,
            Models.journal.JournalModel journal3,
            Models.journal.JournalModel journal4, bool IsReprint,
            Models.journal.JournalModel journal5)
        {

            string service = _blueToothService.GetDeviceName();
            //if (service.Equals("windows"))
            //{
                _blueToothService.PrintInvoiceA4(salesModel, journal1, journal2, journal3, journal4, IsReprint, journal5);
            //}
        }
    }
}
