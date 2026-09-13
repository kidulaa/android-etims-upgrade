using EBM2x.Datafile.env;
using EBM2x.Datafile.regitotal;
using EBM2x.Datafile.trlog;
using EBM2x.Models;
using EBM2x.Utils;
using System;

namespace EBM2x.Process.start
{
    public class Initialize
    {
        public static string excuteProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            if (!EnvPosSetupService.IsEnvPosSetup())
            {
                
                return StateModel.OP_POS_SETUP;
            }
            else
            {
                EnvironmentReader.read(posModel.Environment);

                if (!posModel.Environment.EnvPosNode.StoreCode.Equals(posModel.Environment.EnvPosSetup.StoreCode)
                    || posModel.Environment.EnvPosNode.PosNumber.Equals(posModel.Environment.EnvPosSetup.PosNumber))
                {
                    posModel.Environment.EnvPosNode.StoreCode = posModel.Environment.EnvPosSetup.StoreCode;
                    posModel.Environment.EnvPosNode.PosNumber = posModel.Environment.EnvPosSetup.PosNumber;

                    EnvPosNodeService.SaveEnvPosNode(posModel.Environment.EnvPosNode);
                }

                //// 신규 배포프로그램이 존재하는 경우 업데이트 처리.

                //=========================================
                // Master Database 처리
                //=========================================
                //if (!SQLiteProvider.IsExist())
                //{
                //    SQLiteProvider.getInstance().CreateDatabase();
                //    // 테블생성
                //    MasterManager.CreateTables();
                //}

                //=========================================
                // Tran Database 처리
                //=========================================
                //if (!SQLiteTranProvider.IsExist())
                //{
                //    SQLiteTranProvider.getInstance().CreateDatabase();
                //    // 테블생성
                //    //TranManager.CreateTables();
                //}

                //=========================================
                // AngelFood Database 처리
                //=========================================
                //NeoAngelFood.FoodMasterManager foodManager = new NeoAngelFood.FoodMasterManager();
                //foodManager.FoodDatabase();

                try
                {
                    //////////////////////////////
                    /// POS 환경 정보 LOAD
                    //////////////////////////////
                    if (posModel.Environment != null)
                    {
                        EnvironmentReader.read(posModel.Environment);
                    }

                    //////////////////////////////
                    /// REGI 정보 LOAD
                    //////////////////////////////

                    RegiTotalReader.read(posModel);
                    //posModel.RegiTotal.RegiHeader.increaseReceiptNo();

                    posModel.RegiTotal.RegiHeader.StoreCode = posModel.Environment.EnvPosNode.StoreCode;
                    posModel.RegiTotal.RegiHeader.StoreName = posModel.Environment.EnvPosNode.StoreName;
                    posModel.RegiTotal.RegiHeader.RegiNo = posModel.Environment.EnvPosNode.PosNumber;
                    posModel.RegiTotal.RegiHeader.RegiName = posModel.Environment.EnvPosNode.PosName;

                    // POS번호가 4자리보다 적으면 오류로 처리(환경파일이 잘못설정되어 있으면)
                    if (posModel.RegiTotal.RegiHeader.RegiNo.Length < 4)
                    {
                        RegiTotalWriter.DeleteFile();
                        return StateModel.OP_EXIT;
                    }

                    // 프로그램 로딩시점에 최종 사용자 정보 CLEAR
                    posModel.RegiTotal.RegiHeader.UserID = string.Empty;
                    posModel.RegiTotal.RegiHeader.UserName = string.Empty;
                    // 초기 TR전송 유무를 false로 설정[InitializeTranStart 에서 true로 설정]
                    posModel.RegiTotal.RegiHeader.TRSend = false;

                    //////////////////////////////
                    /// PosModel 초기화
                    //////////////////////////////
                    posModel.InitailizeTran();

                    return StateModel.OP_NEXT;
                }
                catch (Exception ex)
                {
                    LogWriter.ErrorLog(ex.ToString());
                    return StateModel.OP_ERROR;
                }
            }
        }

        public static string RegiClearProcess(PosModel posModel, InputModel inputModel, InformationModel informationModel)
        {
            RegiTotalWriter.DeleteFileAll();
            TransactionWriter.DeleteFileAll();
            RraSdcJsonWriter.DeleteFileAll();

            TransactionWriter.DeleteFiles("master");

            return StateModel.OP_NEXT;
        }
    }
}
