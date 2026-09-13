using EBM2x.Models;
using EBM2x.Models.journal;

namespace EBM2x.Datafile.trlog
{
    public class JournalHoldReader : DatafileService
    {
        public static JournalModel read(PosModel posModel, string holdname)
        {
            JournalModel instance = new JournalModel();
            //read(posModel, holdname, instance);
            return instance;
        }
    }
}
