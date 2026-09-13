using System.IO;
using System.Threading.Tasks;

public interface ISave
{
    bool IsEBM2xDataFolderPath();
    bool ExistsEBM2xDataFolderPath();
    string GetEBM2xDataFolderPath();

    //Method to save document as a file and view the saved document
    Task SaveAndView(string filename, string contentType, MemoryStream stream);

    bool IsSdCard();
    Task ExportData();
}

