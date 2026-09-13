using EBM2x.Models;
using EBM2x.Models.tran;
using EBM2x.UI;
using EBM2x.Utils;
using System;

namespace EBM2x.Journal.tran
{
    public class TranNodeJournal
    {
        string line = "";
        public void create(PosModel posModel)
        {
            if(UIManager.Instance().Is58mmPrinter)
            {
                line = "--------------------------------"; //32Byte
            }
            else
            {
                line = "-----------------------------------"; //35Byte
            }

            //createToBackupTender(posModel, string.Empty);
            createToCust(posModel);

        }
        public void createWithoutPayment(PosModel posModel)
        {
            if (UIManager.Instance().Is58mmPrinter)
            {
                line = "--------------------------------"; //32Byte
            }
            else
            {
                line = "-----------------------------------"; //35Byte
            }

            //createToBackupTender(posModel, string.Empty);
            createToCustWithoutPayment(posModel);

        }

        public void createToCust(PosModel posModel)
        {
            try
            {
                // Tran Node
                TranNode tranNode = posModel.TranModel.TranNode;

                //  Header 
                Journal.JournalHeader header = new Journal.JournalHeader();
                header.create(posModel);

                if (!string.IsNullOrEmpty(tranNode.CustomerNode.Tin))
                {
                    // CLIENT TIN 
                    posModel.Journal.Add("", "CLIENT PIN : " + tranNode.CustomerNode.Tin);
                    posModel.Journal.Add("", "CLIENT NAME: " + tranNode.CustomerNode.CustomerName);
                }
                //if (!string.IsNullOrEmpty(tranNode.InsurerNode.InsurerName))
                //{
                //    // INSURER 
                //    posModel.Journal.Add("", "INSURER    : " + tranNode.InsurerNode.InsurerName);
                //}

                // JCNA Hotel 
                if (UIManager.Instance().PosModel.Environment.EnvPosSetup.TempletType.Equals("Hotel"))
                {
                    if (tranNode.HotelRoomNode != null)
                    {
                        if (!string.IsNullOrEmpty(tranNode.HotelRoomNode.GuestName))
                        {
                            posModel.Journal.Add(tranNode.HotelRoomNode.HotelRoomCode + " " + "Guest name : " + tranNode.HotelRoomNode.GuestName);
                        }
                        if (tranNode.HotelRoomNode.ArrivalDate != null && tranNode.HotelRoomNode.DepartureDate != null)
                        {
                            posModel.Journal.Add("Arrival Date : " + tranNode.HotelRoomNode.ArrivalDate.ToString("dd/MM/yyyy"));
                            posModel.Journal.Add("Departure Date : " + tranNode.HotelRoomNode.DepartureDate.ToString("dd/MM/yyyy"));
                        }
                    }
                }

                if (!string.IsNullOrEmpty(tranNode.CustomerNode.Tin) || !string.IsNullOrEmpty(tranNode.InsurerNode.InsurerName))
                {
                    posModel.Journal.Add(line);
                }

                //Body
                // posModel.Journal.Add("message", string.Empty);
                if (tranNode.TranFlag.Equals("R"))
                {
                    posModel.Journal.Add("bold", "".PadLeft(15) + "CREDIT NOTE");
                    // JCNA 20191203 inv id
                    //posModel.Journal.Add("", "NORMAL RECEIPT#: " + posModel.TranModel.TranNode.RefundReasonNode.OrgBarcodeNo);
                    posModel.Journal.Add("", "NORMAL RECEIPT#: " + posModel.TranModel.TranNode.RefundReasonNode.OrgInvoiceNo);
                    posModel.Journal.Add("", line);
                    posModel.Journal.Add("", "CREDIT NOTE IS APPROVED ONLY FOR");
                    posModel.Journal.Add("", "ORIGINAL SALES RECEIPT");
                    posModel.Journal.Add(line);
                    //posModel.Journal.AddLine();
                }

                ItemNodeJournal itemNodeJournal = new ItemNodeJournal();
                itemNodeJournal.getItemListString(posModel, tranNode.ItemList);
                posModel.Journal.Add(line);

                // Pharmacy
                if (posModel.Environment.EnvPosSetup.TempletType == Models.PosDefine.TEMPLET_PHARMACY)
                {
                    Models.tran.ItemNode checkItemNode = posModel.TranModel.TranNode.ItemList.CheckInsurance();
                    if (checkItemNode != null)
                    {
                        if (UIManager.Instance().Is58mmPrinter)
                        {
                            posModel.Journal.Add(Journal.JournalUtil.lpad(14, checkItemNode.InsurerName) + checkItemNode.InsurerRate + "%" + Journal.JournalUtil.lpad(15, (tranNode.InsuranceDiscountAmount * tranNode.Sign)));
                            //posModel.Journal.AddFormat("{0,-17}{1,2}%{2,15}", checkItemNode.InsurerName, checkItemNode.InsurerRate, Journal.JournalUtil.lpad(15, tranNode.InsuranceDiscountAmount * tranNode.Sign));
                            posModel.Journal.Add(line);
                        }
                        else
                        {
                            if (tranNode.TenderList.List.Count == 0)
                            {
                                posModel.Journal.Add(Journal.JournalUtil.lpad(17, checkItemNode.InsurerName + " " + checkItemNode.InsurerRate.ToString("##0.00") + "% : ") + Journal.JournalUtil.lpad(15, (tranNode.InsuranceDiscountAmount * tranNode.Sign).ToString("#,##.00")));
                                posModel.Journal.Add(Journal.JournalUtil.lpad(17, "CASH" + " " + (100 - checkItemNode.InsurerRate).ToString("##0.00") + "% : ") + Journal.JournalUtil.lpad(15, (tranNode.Subtotal * tranNode.Sign).ToString("#,#0.00")));
                                posModel.Journal.Add(line);
                            }
                            else
                            {
                                string paymentName = tranNode.TenderList.List[0].TenderName;
                                posModel.Journal.Add(Journal.JournalUtil.lpad(17, checkItemNode.InsurerName + " " + checkItemNode.InsurerRate.ToString("##0.00") + "% : ") + Journal.JournalUtil.lpad(15, (tranNode.InsuranceDiscountAmount * tranNode.Sign).ToString("#,##.00")));
                                posModel.Journal.Add(Journal.JournalUtil.lpad(17, paymentName + " " + (100 - checkItemNode.InsurerRate).ToString("##0.00") + "% : ") + Journal.JournalUtil.lpad(15, (tranNode.Subtotal * tranNode.Sign).ToString("#,##.00")));
                                //posModel.Journal.AddFormat("{0,-17}{1,2}%{2,15}", checkItemNode.InsurerName, checkItemNode.InsurerRate, Journal.JournalUtil.lpad(15, tranNode.InsuranceDiscountAmount * tranNode.Sign));
                                posModel.Journal.Add(line);
                            }
                        }
                    }
                }
                
                TenderNodeJournal tenderNodeJournal = new TenderNodeJournal();
                tenderNodeJournal.getTenderListString(posModel, tranNode.TenderList);
                //posModel.Journal.AddLine();

              
                GetBalanceDueString(posModel, tranNode);
                posModel.Journal.AddLine();

                if (UIManager.Instance().Is58mmPrinter)
                {
                    // JCNA 20200130
                    posModel.Journal.Add("reprint", "");
                    //posModel.Journal.Add("ITEM NUMBER : ");
                    //posModel.Journal.AddFormat("ITEM NUMBER :  {0,20}", posModel.TranModel.TranNode.ItemList.Count());
                    posModel.Journal.Add(Journal.JournalUtil.rpad(17, "ITEM NUMBER: ") + Journal.JournalUtil.lpad(13, (posModel.TranModel.TranNode.ItemList.Count())));
                    posModel.Journal.Add(line);
                }
                else
                {
                    // JCNA 20200130
                    posModel.Journal.Add("reprint", "");
                    
                    //posModel.Journal.Add("ITEM NUMBER : ");
                    //posModel.Journal.AddFormat("ITEM NUMBER :  {0,20}", posModel.TranModel.TranNode.ItemList.Count());
                    posModel.Journal.Add(Journal.JournalUtil.rpad(20, "ITEM NUMBER : ") + Journal.JournalUtil.lpad(15, (posModel.TranModel.TranNode.ItemList.Count())));
                    posModel.Journal.Add(line);
                }

                // Journal Footer
                Journal.JournalFooter footer = new Journal.JournalFooter();
                footer.create(posModel);
            }
            catch (Exception e)
            {
                LogWriter.ErrorLog(e.ToString());
            }
        }
        public void createToCustWithoutPayment(PosModel posModel)
        {
            try
            {
                //Tran Node
                TranNode tranNode = posModel.TranModel.TranNode;

                //Header 
                Journal.JournalHeader header = new Journal.JournalHeader();
                header.create(posModel);

                if (!string.IsNullOrEmpty(tranNode.CustomerNode.Tin))
                {
                    // CLIENT TIN
                    posModel.Journal.Add("", "CLIENT PIN : " + tranNode.CustomerNode.Tin);
                    posModel.Journal.Add("", "CLIENT NAME: " + tranNode.CustomerNode.CustomerName);
                }

                // JCNA Hotel 
                if (UIManager.Instance().PosModel.Environment.EnvPosSetup.TempletType.Equals("Hotel"))
                {
                    if (tranNode.HotelRoomNode != null)
                    {
                        if (!string.IsNullOrEmpty(tranNode.HotelRoomNode.GuestName))
                        {
                            posModel.Journal.Add(tranNode.HotelRoomNode.HotelRoomCode + " " + "Guest name : " + tranNode.HotelRoomNode.GuestName);
                        }
                        if (tranNode.HotelRoomNode.ArrivalDate != null && tranNode.HotelRoomNode.DepartureDate != null)
                        {
                            posModel.Journal.Add("Arrival Date : " + tranNode.HotelRoomNode.ArrivalDate.ToString("dd/MM/yyyy"));
                            posModel.Journal.Add("Departure Date : " + tranNode.HotelRoomNode.DepartureDate.ToString("dd/MM/yyyy"));
                        }
                    }
                }

                if (!string.IsNullOrEmpty(tranNode.CustomerNode.Tin) || !string.IsNullOrEmpty(tranNode.InsurerNode.InsurerName))
                {
                    posModel.Journal.Add(line);
                }
                posModel.Journal.Add("  THIS IS NOT AN OFFICIAL RECEIPT");
                posModel.Journal.AddLine();

                ItemNodeJournal itemNodeJournal = new ItemNodeJournal();
                itemNodeJournal.getItemListString(posModel, tranNode.ItemList);
                posModel.Journal.Add(line);

                // Pharmacy
                if (posModel.Environment.EnvPosSetup.TempletType == Models.PosDefine.TEMPLET_PHARMACY)
                {
                   
                    Models.tran.ItemNode checkItemNode = posModel.TranModel.TranNode.ItemList.CheckInsurance();
                    if (checkItemNode != null)
                    {
                        if (UIManager.Instance().Is58mmPrinter)
                        {
                           
                            posModel.Journal.Add(Journal.JournalUtil.lpad(14, checkItemNode.InsurerName) + checkItemNode.InsurerRate + "%" + Journal.JournalUtil.lpad(15, (tranNode.InsuranceDiscountAmount * tranNode.Sign)));
                            //posModel.Journal.AddFormat("{0,-17}{1,2}%{2,15}", checkItemNode.InsurerName, checkItemNode.InsurerRate, Journal.JournalUtil.lpad(15, tranNode.InsuranceDiscountAmount * tranNode.Sign));
                            posModel.Journal.Add(line);
                        }
                        else
                        {
                            
                            string paymentName = tranNode.TenderList.List[0].TenderName;
                            posModel.Journal.Add(Journal.JournalUtil.lpad(17, checkItemNode.InsurerName + " " + checkItemNode.InsurerRate.ToString("##0.00") + "% : ") + Journal.JournalUtil.lpad(15, (tranNode.InsuranceDiscountAmount * tranNode.Sign).ToString("#,##.00")));
                            posModel.Journal.Add(Journal.JournalUtil.lpad(17, paymentName + " " + (100 - checkItemNode.InsurerRate).ToString("##0.00") + "% : ") + Journal.JournalUtil.lpad(15, (tranNode.Subtotal * tranNode.Sign).ToString("#,##.00")));
                            //posModel.Journal.AddFormat("{0,-17}{1,2}%{2,15}", checkItemNode.InsurerName, checkItemNode.InsurerRate, Journal.JournalUtil.lpad(15, tranNode.InsuranceDiscountAmount * tranNode.Sign));
                            posModel.Journal.Add(line);
                        }
                    }
                }

                if (UIManager.Instance().Is58mmPrinter)
                {
                    posModel.Journal.Add("--------------------------------");
                    posModel.Journal.Add(Journal.JournalUtil.rpad(17, "ITEM NUMBER: ") + Journal.JournalUtil.lpad(13, (posModel.TranModel.TranNode.ItemList.Count())));
                    posModel.Journal.Add("--------------------------------");
                }
                else
                {
                    posModel.Journal.Add("-----------------------------------");
                    posModel.Journal.Add(Journal.JournalUtil.rpad(20, "ITEM NUMBER : ") + Journal.JournalUtil.lpad(15, (posModel.TranModel.TranNode.ItemList.Count())));
                    posModel.Journal.Add("-----------------------------------");
                }

                // Journal Footer
                Journal.JournalFooter footer = new Journal.JournalFooter();
                footer.createWithoutPayment(posModel);
            }
            catch (Exception e)
            {
                LogWriter.ErrorLog(e.ToString());
            }
        }

        public void GetBalanceDueString(PosModel posModel, TranNode tranNode)
        {
            string nodeData;

            if (UIManager.Instance().Is58mmPrinter)
            {
                /*
                // 0---------1---------2---------3---------4---------5
                //  ----------------------------------------
                //      -9,999,999,990
                //      -9,999,999,990
                //     -9,999,999,990
                // 0---------1---------2---------3---------4---------5
                posModel.Journal.AddLine();
                //nodeData = "" + Journal.JournalUtil.lpad(29, tranNode.Subtotal * tranNode.Sign);
                nodeData = "Total   Amt" + Journal.JournalUtil.lpad(19, tranNode.Subtotal * tranNode.Sign);
                posModel.Journal.Add("hide", nodeData);
                */
                if (tranNode.Change > 0)
                {
                    /*
                    //nodeData = ""
                    nodeData = "Receive Amt"
                        + Journal.JournalUtil.lpad(19, tranNode.Receive * tranNode.Sign);
                    posModel.Journal.Add("hide", nodeData);
                    //lyw201409003 add --]
                    */

                    //nodeData = ""
                    posModel.Journal.AddLine();
                    nodeData = "PAID      " + Journal.JournalUtil.lpad(22, tranNode.Receive * tranNode.Sign);
                    posModel.Journal.Add(nodeData);
                    nodeData = "CHANGE    " + Journal.JournalUtil.lpad(22, tranNode.Change * tranNode.Sign);
                    posModel.Journal.Add(nodeData);
                }
            }
            else
            {
                /*
                // 0---------1---------2---------3---------4---------5
                //  ----------------------------------------
                //               -9,999,999,990
                //                 -9,999,999,990
                //                -9,999,999,990
                // 0---------1---------2---------3---------4---------5
                posModel.Journal.AddLine();
                //nodeData = "      " + Journal.JournalUtil.lpad(29, tranNode.Subtotal * tranNode.Sign);
                nodeData = "Total   Amt" + Journal.JournalUtil.lpad(29, tranNode.Subtotal * tranNode.Sign);
                posModel.Journal.Add("hide", nodeData);
                */
                if (tranNode.Change > 0)
                {
                    /*
                    //nodeData = ""
                    nodeData = "Receive Amt"
                        + Journal.JournalUtil.lpad(29, tranNode.Receive * tranNode.Sign);
                    posModel.Journal.Add("hide", nodeData);
                    //lyw201409003 add --]
                    */

                    //nodeData = ""
                    posModel.Journal.AddLine();
                    nodeData = "PAID      " + Journal.JournalUtil.lpad(25, tranNode.Receive * tranNode.Sign);
                    posModel.Journal.Add(nodeData);
                    nodeData = "CHANGE    " + Journal.JournalUtil.lpad(25, tranNode.Change * tranNode.Sign);
                    posModel.Journal.Add(nodeData);
                }
            }
        }
    }
}
