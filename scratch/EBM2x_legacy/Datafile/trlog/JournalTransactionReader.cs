using EBM2x.Models;
using EBM2x.Models.journal;

namespace EBM2x.Datafile.trlog
{
    public class JournalTransactionReader : DatafileService
    {
        public static JournalModel read(PosModel posModel, string tranname)
        {
            JournalModel instance = new JournalModel();
            //read(posModel, tranname, instance);
            return instance;
        }
    }
}
