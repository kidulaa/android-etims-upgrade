using EBM2x.Models;
using EBM2x.Models.regitotal;
using EBM2x.Process.eot;
using EBM2x.Datafile;
using System;
using System.IO;

namespace EBM2x.Process.close
{
    public class CloseProcess
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            RegiHeader regiHeader = posModel.RegiTotal.RegiHeader;

            regiHeader.CloseFlag = true;             
            regiHeader.CloseSequence += 1;        
            regiHeader.setCloseDate();
            regiHeader.setCloseTime();

       
            OtherEndOfTransaction.excuteProcess(posModel, inputModel, informationModel);

            
            PosModelInitialize.excuteProcess(posModel, inputModel, informationModel);

           
            posModel.TranModel.Clear();

            
            string filedate = posModel.RegiTotal.RegiHeader.OpenDate;
            DateTime m_DateTime = Utils.Common.toDateTime(filedate, 900 * -1);
            string m_BaseDateTime = m_DateTime.ToString("yyyyMMdd");

            string rootdirectory = DatafileService.GetPathName("backup");
            if (System.IO.Directory.Exists(rootdirectory))
            {
                string[] directoryName = System.IO.Directory.GetDirectories(rootdirectory, "*");
                for (int i = 0; i < directoryName.Length; i++)
                {
                    string sourceDirName = "";
                    if(directoryName[i].LastIndexOf("\\", directoryName[i].Length - 1) > 0)
                    {
                        sourceDirName = directoryName[i].Substring(directoryName[i].LastIndexOf("\\", directoryName[i].Length - 1));
                    }
                    else
                    {
                        sourceDirName = directoryName[i].Substring(directoryName[i].LastIndexOf("/", directoryName[i].Length - 1));
                    }
                    string sourceDirnametemp = "";

                    if (sourceDirName.Length > 8) sourceDirnametemp = sourceDirName.Substring(5, 4);
                    if (sourceDirnametemp.Equals(filedate.Substring(4, 4)))
                    {
                        System.IO.Directory.Delete(directoryName[i], true);
                    }

                    try
                    {
                        sourceDirName = sourceDirName.Substring(1, 8);
                    }
                    catch
                    {
                        continue;
                    }

                    if (Utils.Common.isDateType(sourceDirName))
                    {
                        if (m_BaseDateTime.CompareTo(sourceDirName) > 0)
                            System.IO.Directory.Delete(directoryName[i], true);
                    }
                }
            }

            informationModel.SetInformationMessage("Receipt file Backup..."); 
            Datafile.trlog.BackupTransactionWriter.write(posModel, filedate);

            informationModel.SetInformationMessage("Regitotal file Backup...");
            Datafile.regitotal.BackupRegiTotalWriter.write(posModel, filedate);

            return StateModel.OP_NEXT;
        }

        public static string excuteProcessTest(PosModel posModel)
        {
            
            string filedate = posModel.RegiTotal.RegiHeader.OpenDate;
            DateTime m_DateTime = Utils.Common.toDateTime(filedate, 900 * -1);
            string m_BaseDateTime = m_DateTime.ToString("yyyyMMdd");

            string rootdirectory = DatafileService.GetPathName("backup");
            if (System.IO.Directory.Exists(rootdirectory))
            {
                string[] directoryName = System.IO.Directory.GetDirectories(rootdirectory, "*");
                for (int i = 0; i < directoryName.Length; i++)
                {
                    string sourceDirName = "";
                    if (directoryName[i].LastIndexOf("\\", directoryName[i].Length - 1) > 0)
                    {
                        sourceDirName = directoryName[i].Substring(directoryName[i].LastIndexOf("\\", directoryName[i].Length - 1));
                    }
                    else
                    {
                        sourceDirName = directoryName[i].Substring(directoryName[i].LastIndexOf("/", directoryName[i].Length - 1));
                    }
                    string sourceDirnametemp = "";

                    if (sourceDirName.Length > 8) sourceDirnametemp = sourceDirName.Substring(5, 4);
                    if (sourceDirnametemp.Equals(filedate.Substring(4, 4)))
                    {
                        System.IO.Directory.Delete(directoryName[i], true);
                    }

                    try
                    {
                        sourceDirName = sourceDirName.Substring(1, 8);
                    }
                    catch
                    {
                        continue;
                    }

                    if (Utils.Common.isDateType(sourceDirName))
                    {
                        if (m_BaseDateTime.CompareTo(sourceDirName) > 0)
                            System.IO.Directory.Delete(directoryName[i], true);
                    }
                }
            }

            Datafile.trlog.BackupTransactionWriter.write(posModel, filedate);

            return StateModel.OP_NEXT;
        }
    }
}
