using EBM2x.Dependency;
using EBM2x.Utils;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.IO;
using Xamarin.Forms;

namespace EBM2x.Database
{
    public class EBM2xMsSQLiteClientProvider : IEBM2xDatabaseProvider
    {
        protected SqliteConnection sqlConnection = null;
        protected SqliteTransaction sqlTransaction = null;

        public string GetDatabaseName(string name)
        {
            string directory = "sqlite";
            
            //string localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            ISave iSave = DependencyService.Get<ISave>();
            if (iSave == null)
            {
            }
            string localApplicationData = iSave.GetEBM2xDataFolderPath();
            

            string pathName = Path.Combine(localApplicationData, "EBM2x/" + directory);
            if (!Directory.Exists(pathName))
            {
                Directory.CreateDirectory(pathName);
            }

            string fileName = Path.Combine(pathName, name);
            return fileName;
        }
        public string GetDatabasePath()
        {
            string directory = "sqlite";

            //string localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            ISave iSave = DependencyService.Get<ISave>();
            if (iSave == null)
            {
            }
            string localApplicationData = iSave.GetEBM2xDataFolderPath();

            string pathName = Path.Combine(localApplicationData, "EBM2x/" + directory);
            if (!Directory.Exists(pathName))
            {
                Directory.CreateDirectory(pathName);
            }

            return pathName;
        }

        public void OpenConnection(string server, string database, string uid, string pwd)
        {
            if (Connected())
            {
                if (sqlConnection != null) return;
                CloseConnection();
            }

            string databasenameStr = GetDatabaseName("RRA_EBM2x.db");
            string strConn = "Data Source=" + databasenameStr;

            try
            {
                sqlConnection = new Microsoft.Data.Sqlite.SqliteConnection(strConn);
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
                sqlConnection = null;
            }
        }

        public void CloseConnection()
        {
            try
            {
                if (sqlConnection != null) sqlConnection.Close();
            }
            catch (Exception ex)
            {
                LogWriter.ErrorLog(ex.ToString());
            }
            sqlConnection = null;
        }

        public void Reconnect()
        {
            CloseConnection();
            OpenConnection("", "", "", "");
        }

        public bool Connected()
        {
            if (sqlConnection != null && (int)sqlConnection.State == (int)ConnectionState.Open) return true;
            return false;
        }

        public IDbCommand GetDbCommand()
        {
            if (!Connected() || sqlConnection == null ) return null;
            
            try
            {
                return sqlConnection.CreateCommand();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void TransactionStart()
        {
            if (!Connected()) return;

            // only begin transaction when there is no transaction
            try
            {
                if (sqlTransaction == null)
                {
                    sqlTransaction = sqlConnection.BeginTransaction();
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void TransactionCommit()
        {
            if (!Connected()) return;
            if (sqlTransaction == null) return;
            try
            {
                sqlTransaction.Commit();
                sqlTransaction.Dispose();
                sqlTransaction = null;
            }
            catch (Exception ex)
            {
            }
        }

        public void TransactionRollback()
        {
            if (!Connected()) return;
            if (sqlTransaction == null) return;

            try
            {
                sqlTransaction.Rollback();
                sqlTransaction.Dispose();
                sqlTransaction = null;
            }
            catch (Exception ex)
            {
            }
        }

        public void CreateDatabase()
        {
            string databasenameStr = GetDatabaseName("RRA_EBM2x.db");
            // SQLite Datafile을 생성 (Zero byte file).
            System.IO.FileStream fs = System.IO.File.Create(databasenameStr);
            fs.Close();
        }

        public bool CopySQLiteDatabase()
        {
            string rrasdcnameStr = GetDatabaseName("RRA_EBM2x.db.RraSdc");
            string databasenameStr = GetDatabaseName("RRA_EBM2x.db");
            // JCNA 20201127
            string databasenameStrZip = GetDatabaseName("RRA_EBM2x.zip");
            string extractPath = GetDatabasePath();
            try
            {
                if (System.IO.File.Exists(databasenameStr))
                {
                    try
                    {
                        string movefile = databasenameStr + DateTime.Now.ToString(".yyyyMMddHHmmss");
                        System.IO.File.Move(databasenameStr, movefile);
                    }
                    catch (Exception e1)
                    {
                        string e1Msg = e1.Message;
                    }
                }
                if (System.IO.File.Exists(databasenameStrZip))
                {
                    try
                    {
                        string movefile = databasenameStrZip + DateTime.Now.ToString(".yyyyMMddHHmmss");
                        System.IO.File.Move(databasenameStrZip, movefile);
                    }
                    catch (Exception e1)
                    {
                        string e1Msg = e1.Message;
                    }
                }

                using (FileStream destination = File.Create(databasenameStrZip))
                {
                   
                    // JCNA 20200130
                    Stream source = null;
                    ISQLiteDatabaseUtil sQLiteDatabaseUtil = DependencyService.Get<ISQLiteDatabaseUtil>();
                    if (sQLiteDatabaseUtil != null) source = sQLiteDatabaseUtil.GetSQLiteDatabaseZipStream();
                    // Copy source to destination.
                    if (source != null) source.CopyTo(destination);
                    else 
                    return false;
                    

                }
                // Android 7 처리를 위해 테스트 
                //using (FileStream destination = File.Create(databasenameStr))
                //{
                //    // JCNA 20200130
                //    Stream source = null;
                //    ISQLiteDatabaseUtil sQLiteDatabaseUtil = DependencyService.Get<ISQLiteDatabaseUtil>();
                //    if (sQLiteDatabaseUtil != null) source = sQLiteDatabaseUtil.GetSQLiteDatabaseStream();
                //    // Copy source to destination.
                //    if (source != null) source.CopyTo(destination);
                //    else return false;
                //}

                //// Android 7 처리를 위해 테스트 
                //// Create a string array with the lines of text
                //string[] lines = { "First line", "Second line", "Third line" };
                //// Write the string array to a new file named "WriteLines.txt".
                //using (StreamWriter outputFile = new StreamWriter(Path.Combine(extractPath, "WriteLines.txt")))
                //{
                //    foreach (string line in lines) outputFile.WriteLine(line);
                //}
                //string text = File.ReadAllText(Path.Combine(extractPath, "WriteLines.txt"));

                // Android 7 처리를 위해 테스트 
                //// UNZIP
                //ZipFile.ExtractToDirectory(databasenameStrZip, extractPath);
                // 안드로이드 7에서는 ZipFile을 사용할 수 없음.

                using (ZipInputStream s = new ZipInputStream(File.OpenRead(databasenameStrZip)))
                {
                    ZipEntry theEntry;
                    if ((theEntry = s.GetNextEntry()) != null)
                    {
                        string fileName = Path.GetFileName(theEntry.Name);

                        if (theEntry.IsFile)
                        {
                            using (FileStream streamWriter = File.Create(Path.Combine(extractPath, fileName)))
                            {
                                int size = 102400; // 100KB
                                byte[] data = new byte[102400];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                using (FileStream destination = File.Create(rrasdcnameStr))
                {
                    // JCNA 20200130
                    Stream source = null;
                    ISQLiteDatabaseUtil sQLiteDatabaseUtil = DependencyService.Get<ISQLiteDatabaseUtil>();
                    if (sQLiteDatabaseUtil != null) source = sQLiteDatabaseUtil.GetSQLiteDatabaseZipStream();
                    // Copy source to destination.
                    if (source != null) source.CopyTo(destination);
                }
            }
            catch (Exception e)
            {
                //string eMsg = e.Message;
                return false;
            }

            if (System.IO.File.Exists(databasenameStr))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CopySQLiteDatabase(bool RraSdc) 
        {
            string databasenameStr = GetDatabaseName("RRA_EBM2x.db");
            // JCNA 20201127
            string databasenameStrZip = GetDatabaseName("RRA_EBM2x.zip");
            string extractPath = GetDatabasePath();
            try
            {
                if (System.IO.File.Exists(databasenameStr))
                {
                    string movefile = databasenameStr + DateTime.Now.ToString(".yyyyMMddHHmmss");
                    System.IO.File.Move(databasenameStr, movefile);
                }
                if (System.IO.File.Exists(databasenameStrZip))
                {
                    string movefile = databasenameStrZip + DateTime.Now.ToString(".yyyyMMddHHmmss");
                    System.IO.File.Move(databasenameStrZip, movefile);
                }

                using (FileStream destination = File.Create(databasenameStrZip))
                {
                    // JCNA 20200130
                    Stream source = null;
                    ISQLiteDatabaseUtil sQLiteDatabaseUtil = DependencyService.Get<ISQLiteDatabaseUtil>();
                    if (sQLiteDatabaseUtil != null) source = sQLiteDatabaseUtil.GetSQLiteDatabaseZipStream();
                    // Copy source to destination.
                    if (source != null) source.CopyTo(destination);
                    else return false;
                }

                //// Android 7 처리
                //// UNZIP
                //ZipFile.ExtractToDirectory(databasenameStrZip, extractPath);
                // 안드로이드 7에서는 ZipFile을 사용할 수 없음.

                using (ZipInputStream s = new ZipInputStream(File.OpenRead(databasenameStrZip)))
                {
                    ZipEntry theEntry;
                    if ((theEntry = s.GetNextEntry()) != null)
                    {
                        string fileName = Path.GetFileName(theEntry.Name);

                        if (theEntry.IsFile)
                        {
                            using (FileStream streamWriter = File.Create(Path.Combine(extractPath, fileName)))
                            {
                                int size = 102400; // 100KB
                                byte[] data = new byte[102400];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                string aaa = e.Message;
            }

            if (System.IO.File.Exists(databasenameStr))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ExportDatabase(string sourcefilename)
        {
            string databasenameStr = GetDatabaseName("RRA_EBM2x.db");
            string exportfile = sourcefilename + DateTime.Now.ToString("yyyyMMddHHmmss");
            try
            {
                if (System.IO.File.Exists(databasenameStr))
                {
                    System.IO.File.Copy(databasenameStr, exportfile);
                }
            }
            catch (Exception e)
            {
                string aaa = e.Message;
            }

            if (System.IO.File.Exists(exportfile))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
