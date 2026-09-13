using EBM2x.Database.Master;
using EBM2x.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EBM2x.Database.TableIO
{
    public class SysConfigModelIO : ModelIO
    {
        public List<SysconfigRecord> LoadConfigByCode(string[] codes)
        {
            List<SysconfigRecord> arrayList = new List<SysconfigRecord>();
            
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return arrayList;
            }

            string strCodes = string.Join(", ", codes);

            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT CONFIG_CD , ");                        
            sql.Append("       CONFIG_VALUE , ");                     
            sql.Append("       UPD_TY_CD  ");                          
            sql.Append("  FROM SYSCONFIG ");
            sql.AppendFormat(" WHERE CONFIG_CD IN ({0}) ", strCodes);

            command.CommandText = sql.ToString();
            command.CommandType = CommandType.Text;
            IDataReader reader = command.ExecuteReader(CommandBehavior.Default);
            while (reader.Read())
            {
                SysconfigRecord record = new SysconfigRecord();

                record.ConfigCd = reader.GetString(0);          
                record.ConfigValue = reader.GetString(1);      
                record.UpdTyCd = reader.GetString(2);         

                arrayList.Add(record);
            }

            return arrayList;
        }

        public void LoadConfigForPrint(ref object[] Rowary)
        {
            var codes = new[] { "'01'", "'02'", "'03'", "'04'", "'A'", "'B'", "'C'", "'D'", "'E'", "'F'", "'TEL'", "'EMAIL'" };

            List<SysconfigRecord> ds = LoadConfigByCode(codes);
            foreach (SysconfigRecord dr in ds)
            {
                switch (dr.ConfigCd)
                {
                    case "01":
                        {
                            Rowary[0] = dr.ConfigValue;             // TIN CD
                            break;
                        }

                    case "02":
                        {
                            Rowary[1] = dr.ConfigValue;             
                            break;
                        }

                    case "03":
                        {
                            Rowary[2] = dr.ConfigValue;             // SDC Client WIS ID
                            break;
                        }

                    case "04":
                        {
                            Rowary[3] = dr.ConfigValue;             // (MRC)
                            break;
                        }

                    case "A":
                        {
                            Rowary[4] = dr.ConfigValue;             
                            break;
                        }

                    case "B":
                        {
                            Rowary[5] = dr.ConfigValue;             
                            break;
                        }

                    case "C":
                        {
                            Rowary[6] = dr.ConfigValue;             
                            break;
                        }

                    case "D":
                        {
                            Rowary[7] = dr.ConfigValue;           
                            break;
                        }

                    case "E":
                        {
                            Rowary[8] = dr.ConfigValue;              
                            break;
                        }

                    case "F":
                        {
                            Rowary[9] = dr.ConfigValue;             
                            break;
                        }
                }
            }
        }

        public int UpdateSysConfig(string C_CD, string C_VAL)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return 0;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE SYSCONFIG ");
            sql.Append("    SET CONFIG_VALUE = @CONFIG_VALUE");
            sql.Append("   WHERE CONFIG_CD = @CONFIG_CD");

            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@CONFIG_CD";
            param.Value = C_CD.Trim();
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@CONFIG_VALUE";
            param.Value = C_VAL.Trim();
            command.Parameters.Add(param);

            return ExecuteNonQuery(sql.ToString(), command);
        }

        public int InsertSysConfig(string C_CD, string C_VAL, string UPD_CD)
        {
            IDbCommand command = GetDbCommand();
            if (command == null)
            {
                return 0;
            }

            StringBuilder sql = new StringBuilder();
            sql.Append(" INSERT INTO SYSCONFIG (");
            sql.Append("             CONFIG_CD,");
            sql.Append("             CONFIG_VALUE,");
            sql.Append("             UPD_TY_CD");
            sql.Append("      ) VALUES ( ");
            sql.Append("             @CONFIG_CD,");
            sql.Append("             @CONFIG_VALUE,");
            sql.Append("             @UPD_TY_CD");
            sql.Append("      )");

            IDbDataParameter param;

            param = command.CreateParameter();
            param.ParameterName = "@CONFIG_CD";
            param.Value = C_CD.Trim();
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@CONFIG_VALUE";
            param.Value = C_VAL.Trim();
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@UPD_TY_CD";
            param.Value = UPD_CD.Trim();
            command.Parameters.Add(param);

            return ExecuteNonQuery(sql.ToString(), command);
        }
    }
}
