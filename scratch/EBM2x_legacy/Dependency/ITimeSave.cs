using System.IO;
using System.Threading.Tasks;

public interface ITimeSave
{
    //Method to save document as a file and view the saved document
	Task SetSystemDateTime(string yyyyMMddHHmmssmmm);
}

