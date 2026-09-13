using EBM2x.Models;
using System;
using System.IO;

namespace EBM2x.Datafile.trlog
{
    public class BackupTransactionWriter : DatafileService
    {
        public static bool writeI(PosModel posModel, string filedate)
        {
            string strTranFolder = "";
            string strBackFolder = "";
            string strSALEDate = posModel.RegiTotal.RegiHeader.OpenDate;

            strTranFolder = DatafileService.GetPathName("tran/receipt");
            strBackFolder = DatafileService.GetPathName("backup/" + strSALEDate + "/receipt");
            System.IO.Directory.CreateDirectory(strBackFolder);

            try
            {
                DirectoryInfo source = new DirectoryInfo(strTranFolder);
                foreach (FileInfo file in source.GetFiles())
                {
                    if (System.IO.File.Exists(file.FullName))
                    {
                        string tarFileName = Path.Combine(strBackFolder, file.Name);

                        if (System.IO.File.Exists(tarFileName)) System.IO.File.Delete(tarFileName);

                        System.IO.File.Copy(file.FullName, tarFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                //Utils.LogWriter.ErrorLog(ex.ToString());
            }
            return true;
        }
        public static bool write(PosModel posModel, string filedate)
        {
            string[] filenames = null;
            string sourceFilename = "";
            string strTranFolder = "";
            string strBackFolder = "";
            string strSALEDate = posModel.RegiTotal.RegiHeader.OpenDate;

            strTranFolder = DatafileService.GetPathName("tran/receipt");
            strBackFolder = DatafileService.GetPathName("backup/" + strSALEDate + "/receipt");
            System.IO.Directory.CreateDirectory(strBackFolder);

            try
            {
                filenames = System.IO.Directory.GetFiles(strTranFolder, "*");
                //----------------------------------
                // Tran/Receipt Backup
                //----------------------------------
                for (int i = 0; i < filenames.Length; i++)
                {
                    if (filenames[i].LastIndexOf("\\", filenames[i].Length - 1) > 0)
                    {
                        sourceFilename = filenames[i].Substring(filenames[i].LastIndexOf("\\", filenames[i].Length - 1));
                        if (System.IO.File.Exists(filenames[i].ToString()))
                        {
                            string srcFileName = filenames[i].ToString();
                            string tarFileName = strBackFolder + sourceFilename;

                            if (System.IO.File.Exists(tarFileName)) System.IO.File.Delete(tarFileName);

                            System.IO.File.Copy(srcFileName, tarFileName);
                        }
                    }
                    else
                    {
                        sourceFilename = filenames[i].Substring(filenames[i].LastIndexOf("/", filenames[i].Length - 1));
                        if (System.IO.File.Exists(filenames[i].ToString()))
                        {
                            string srcFileName = filenames[i].ToString();
                            string tarFileName = strBackFolder + sourceFilename;

                            if (System.IO.File.Exists(tarFileName)) System.IO.File.Delete(tarFileName);

                            System.IO.File.Copy(srcFileName, tarFileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Utils.LogWriter.ErrorLog(ex.ToString());
            }
            return true;
        }
    }
}
