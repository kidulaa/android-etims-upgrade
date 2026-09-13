using System.IO;
using System.Threading.Tasks;

public interface ISQLiteDatabaseUtil
{
    //Method to save document as a file and view the saved document
    Stream GetSQLiteDatabaseStream();
    Stream GetSQLiteDatabaseZipStream();
}

