using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Modules
{
    public static class Strings
    {
        public static string print(params string[] parray)
        {
            if (parray.Length == 0)
                throw new Exception("Missing parameter");
            else if (parray.Length == 1)
                return parray[0];
            else
            {
                var array1 = new string[parray.Length - 1 + 1];
                Array.Copy(parray, 1, array1, 0, parray.Length - 1);
                return string.Format(parray[0], array1);
            }
        }

        public const string MSG_NOTEXIST_TEXT = "The value of [{0}] cannot be empty.";
        public const string MSG_TRYAGAIN = "Please try again.";

        public const string MSG_NOT_NUMERIC_TEXT = "The value of [{0}] is not a number.";
        public const string MSG_USERID_RULE = "User ID must have 8-20 characters long and contains only alphanumeric and four _-@. special characters";
        public const string MSG_PASSWORD_RULE = "Password must have 8-20 characters long and contains ASCII characters only";
        public const string MSG_EMAIL_RULE = "Email must have format like someone@place.dom";

        public const string MSG_SYSTEM_CHECKMSG01 = "WIS setting check.";
        public const string MSG_SYSTEM_CHECKMSG02 = "Please wait....";


        public const string MSG_SYSTEM_VERCHECK = "current system version has been updated to higher version." + "\n" + "please update system executable file and then try again..";

        public const string MSG_SYSTEM_CHECK = "Completed system environment setting";

        public const string MSG_OPEN_HELLO = "Have a good day.";
        public const string MSG_LOGIN_HELLO = " Hello:" + "\n" + "Welcome to Client WIS.";


        public const string MSG_EXIST_RUNSYSTEM = "Program is already working now.";

        public const string MSG_EXIST_INPUTVALUE01 = "Tin Number is required.";
        public const string MSG_EXIST_INPUTVALUE02 = "User Id required.";
        public const string MSG_EXIST_INPUTVALUE03 = "Password required.";

        public const string MSG_EXIST_VALUE01 = "Registered Id already" + "\n" + "Please Try again";
        public const string MSG_EXIST_TINCD = "Already existed TINCD...";

        public const string MSG_SELECT_SELECTGU = "Please select type";
        public const string MSG_SELECT_SISEGU = "Please select type";
        public const string MSG_SELECT_REINVEST = "Please select item";

        public const string MSG_SELECT_HEAVY = "too many data registered. total search is not surpported." + "\n" + "please input related datas and try again.";

        public const string MSG_FAIL_BASECREAT = "Failed to create basic information." + "\n" + "please contack administrator.";
        public const string MSG_FAIL_TABLECREAT = "Failed for makeing table";
        public const string MSG_FAIL_CODECREAT = "Failed for making base code";

        public const string MSG_FAIL_SYSVERMODIFY = "Failed for changing system version";

        public const string MSG_FAIL_MODIFY = "After disconnecting internet, try to use existing executable file." + "\n" + "Please contact adminstrator";
        public const string MSG_FAIL_TABLEMODIFY = "Failed for updating table";
        public const string MSG_FAIL_CODEMODIFY = "Failed for updating base code";
        public const string MSG_FAIL_NETWORK = "No internet connection." + "\n" + "Try again after confirming your Internet connection.";

        public const string MSG_FAIL_BLANK = "Required category is empty. " + "\n" + "Please try again.";
        public const string MSG_FAIL_NUMBER = "Only numerical input " + "\n" + "Please try again.";
        public const string MSG_FAIL_NUMFORMAT = "please register some positive number" + "\n" + "Please try again.";
        public const string MSG_FAIL_STRDATE = "search starting date should be former than today. " + "\n" + "Please try again.";
        public const string MSG_FAIL_ENDDATE = "search ended date shoud be later than starting date. " + "\n" + "Please try again.";
        public const string MSG_FAIL_PWDCONFIRM = "Wrong password." + "\n" + "Please try again.";
        public const string MSG_FAIL_PWDCOMPARE = "Confirmation password different to New Password" + "\n" + "Please try again";
        public const string MSG_FAIL_DATCLOSE01 = "Closing date should be selected former than today. " + "\n" + "Please try again.";
        public const string MSG_FAIL_DATCLOSE02 = "Closing date should be selected later than former closing date. " + "\n" + "Please try again.";
        public const string MSG_FAIL_DATCLOSE03 = "Closing year and month should be selected former than today. " + "\n" + "Please try again.";
        public const string MSG_FAIL_DATCLOSE04 = "Closing year and month should be selected later than former closing date. " + "\n" + "Please try again.";
        public const string MSG_FAIL_DATCLOSE05 = "deleting of closing information is possible from latest closing date. " + "\n" + "Please try again.";
        public const string MSG_FAIL_DATMONTH = "Closing month should be later than latest closing month. " + "\n" + "Please try again.";

        public const string MSG_FAIL_DATE = "please check the date";

        public const string MSG_FAIL_DTALENCNOUNT = "Password lenght is incorrect." + "\n" + "Please try again ";

        public const string MSG_CONFIRM_DELETE = "Are you sure to delete Data?";


        public const string MSG_EXIST_SEQ = "existed number already ?";
        public const string MSG_EXIST_DATA = "existed data already ?" + "\n" + "Please try again.";

        public const string MSG_EXIST_USERID = "existed Id already ?";
        public const string MSG_EXIST_USERACCD = "";
        public const string MSG_QUESTION_USERID = "";
        public const string MSG_EXIST_BANKCD = "";
        public const string MSG_EXIST_SYSCD = "";
        public const string MSG_EXIST_CONTSEQ = "";
        public const string MSG_EXIST_COLLECT = "";


        public const string MSG_FAIL_SAVE = "Failed saving";
        public const string MSG_FAIL_CONNECT_FMS = "Failed for connecting system" + "\n" + "please refer to system management";
        public const string MSG_FAIL_COLLECT = "Failed....";
        public const string MSG_FAIL_ClOSESIM = "";
        public const string MSG_FAIL_PREDDATA = "Failed to generate data";
        public const string MSG_FAIL_SELECT = "Failed for searching";
        public const string MSG_FAIL_PRINT = "please check print setting";
        public const string MSG_FAIL_EXCEL = "Failed for making Excel file";
        public const string MSG_FAIL_DELETE = "Failed to delete.";
        // Public Const MSG_FAIL_DELETE = "no data for deleting" & vbCrLf & "Please try again."

        public const string MSG_OK_SAVE = "Saved successfully.";
        public const string MSG_OK_DELETE = "deleting completed";

        public const string MSG_OK_CLEAR = "please input new data.";
        public const string MSG_OK_DOWNLOAD = "downloaded.";
        public const string MSG_OK_SELECT_CLEAR = "Input new search condition.";

        public const string MSG_OK_DOWNLOAD_1 = "Basic System Code Downloaded.";
        public const string MSG_OK_DOWNLOAD_2 = "Item Classification Code Downloaded.";
        public const string MSG_OK_DOWNLOAD_3 = "Item Information Downloaded.";
        public const string MSG_OK_DOWNLOAD_4 = "Imported Information Downloaded.";
        public const string MSG_OK_DOWNLOAD_5 = "Purchase information Downloaded.";
        public const string MSG_OK_DOWNLOAD_6 = "Branch office Information Downloaded.";

        public const string MSG_OK_TBLCREATE = "Table created";
        public const string MSG_OK_TBLMODIFY = "Table changed";
        public const string MSG_OK_CODCREATE = "base code created";
        public const string MSG_OK_DATCREATE = "data updated";

        public const string MSG_OK_CLOSE = "Closing completed.";

        public const string MSG_OK_COLLECT = "";
        public const string MSG_OK_LOADFORM = "View loading completed";
        public const string MSG_OK_SELECT = "search completed";
        public const string MSG_OK_ClOSESIM = "";
        public const string MSG_OK_EXCEL = "Excel File created.";
        public const string MSG_OK_PRINT = "print completed";


        public const string MSG_NOTEXIST_POWER = "this menu can be approched only master ID";
        public const string MSG_NOTEXIST_DATA = "Record not found.";

        public const string MSG_NOTEXIST_COLLECT = "No data to collect.";
        public const string MSG_NOTEXIST_RATE = "Information doesn't exist.";

        public const string MSG_NOTICE_DATACHANGE = "If the current high quantity is not 0, only the sales unit price, taxation type, and remarks information can be modified.";

        public const string MSG_NOTICE_SEND_START = "Start data transfer." + "\n" + "Please wait until coming up the message that transfer is complete";
        public const string MSG_NOTICE_SEND_END = "The data transfer is complete.";
        public const string MSG_NOTICE_RECEIVE_START = "Begin to receive data." + "\n" + "Please wait until the reception complete message comes up.";
        public const string MSG_NOTICE_RECEIVE_END = "Data reception is complete.";

        public const string MSG_FAIL_SEND = "Failed to send data.";
        public const string MSG_FAIL_RECEIVE = "Failed to receive data.";

        public const string MSG_FAIL_DETAIL = "Please complete the configuration details and save with 'detail row add' & 'detail row delete' button.";
        public const string MSG_FAIL_DETAIL2 = "Please complete the configuration details and save with 'detail row modified' button.";
        public const string MSG_FAIL_KEY = "Please check invoice ID.";
        public const string MSG_FAIL_SAME1 = "Selected document type and transaction type (sales / sales-refundable) must be identical.";
        public const string MSG_FAIL_SAME2 = "When selecting the type of transaction (sales-refundable), receipt type must be selected the 'REFUND'.";
        public const string MSG_FAIL_SAME3 = "'Discount total amount' to add details to each item is automatically calculated by the discount rate.";
        public const string MSG_FAIL_SAME4 = "When selecting Type of transaction (sales), receipt type must be selected the 'SALES'.";
        public const string MSG_FAIL_SAME5 = "This import items can not be modified before deleteing details of item with 'detail row delete' button.";
        public const string MSG_FAIL_SAME6 = "Please check out the unpaid balance of current customer.";
        public const string MSG_FAIL_SAME7 = "No stock item of this carry-in stock. Please try save again after inputting stock item.";
        public const string MSG_FAIL_SAME8 = "This carry-out item can not be modified. Please delete details first with 'detail row delete' button when changing item.";
        public const string MSG_FAIL_SAME9 = "Input the item details about the carry-out of stock invoice and then save.";
        public const string MSG_FAIL_SAME10 = "Please check out the receivable balance of current customer.";

       
        public const string MSG_FAIL_SAME11 = "Your invoice is being processed automatically. View the document after processing.";

        public const string MSG_ING_SELECT = "Data searching...";
        public const string MSG_ING_SAVE = "Data saving...";
        public const string MSG_ING_UPDATE = "Data updating...";
        public const string MSG_ING_DELETE = "Data deleting...";
        public const string MSG_ING_INSERT = "Data inputting...";
        public const string MSG_ING_LOADFORM = "View loading...";
        public const string MSG_ING_ClOSESIM = "Test canceling...";
        public const string MSG_ING_PRINT = "Printing...";
        public const string MSG_ING_EXCEL = "Excel File generating...";
        public const string MSG_ING_COLLECT = "Data collecting...";


        public const string MSG_END_SYSTEM = "do you exit this system?";

        public const string MSG_QUESTION_ITEMCDCREAT = "Do you want to automatically generate the code item?";


        public const string MSG_INSERT_NEEDFUND = "";

        public const string TITLE_SYSTEMNAME = "Sales management system";
        public const string TITLE_COLLECT = "Data Collect";
        // Public Const TITLE_COLLECT = "Data Collect"

        public const int MAX_FORM_HEIGHT = 9230;
        public const int MAX_FORM_WIDTH = 12325;


        public const string MSG_DEC_M010010000 = "System Base Code Manage";
        public const string MSG_DEC_M010020000 = "Transmit WIS DATA to SCM";

        public const string MSG_DEC_M010010020 = "This view is searching item classification information.";
        public const string MSG_DEC_M010010030 = "This view is registering item information.";
        public const string MSG_DEC_M010010040 = "This view is searching item information.";
        public const string MSG_DEC_M010010050 = "This view is searching user information";
        public const string MSG_DEC_M010010070 = "This view is managing system environment information.";

        public const string MSG_DEC_M010020010 = "This view is managing client information.";
        public const string MSG_DEC_M010020020 = "This view is searching client information.";

        public const string MSG_DEC_M020010000 = "It's the view for registering of Sale Information.";
        public const string MSG_DEC_M020020000 = "It's the view for managing carry-out of stock.";
        public const string MSG_DEC_M020030000 = "This view is searching sales information for each invoice.";
        public const string MSG_DEC_M020040000 = "";
        public const string MSG_DEC_M020050000 = "";
        public const string MSG_DEC_M020060000 = "This view is searching sales cancel status.";
        public const string MSG_DEC_M020070000 = "";

        public const string MSG_DEC_M023010000 = "This view is downloading imported information.";
        public const string MSG_DEC_M023020000 = "The screen to view the imported information.";
        public const string MSG_DEC_M023030000 = "Manage the status of inport list.";

        public const string MSG_DEC_M030010000 = "This view is registering purchase information.";
        public const string MSG_DEC_M030013000 = "This view is registering import information.";
        public const string MSG_DEC_M030015000 = "This view is registering sales information.";

        public const string MSG_DEC_M030020000 = "This screen displays the purchase status (by document).";
        public const string MSG_DEC_M030030000 = "";
        public const string MSG_DEC_M030040000 = "";
        public const string MSG_DEC_M030041000 = "This view is searching purchase cancel status.";
        public const string MSG_DEC_M030050000 = "This is a screen that receives the sales information.";
        public const string MSG_DEC_M030050010 = "Please download sales information.";
        public const string MSG_DEC_M030060000 = "The screen to view the sales information.";


        public const string MSG_DEC_M060010000 = "Please search the stock information.";
        public const string MSG_DEC_M060020000 = "";
        public const string MSG_DEC_M060030000 = "";
        public const string MSG_DEC_M060040000 = "It's the view to display the stock status.";



        public const string MSG_DEC_M070010000 = "It's the view to register the deposit information.";
        public const string MSG_DEC_M070020000 = "It's the view to search the deposit information.";
        public const string MSG_DEC_M070030000 = "It's the view to register the withdrawal information.";
        public const string MSG_DEC_M070040000 = "It's the view to search the withdrawal information.";

        public const string MSG_DEC_M080010000 = "Process the stock daily closing.";
        public const string MSG_DEC_M080020000 = "Search the stock status for daily closing.";
        public const string MSG_DEC_M080030000 = "Process the stock monthly closing.";
        public const string MSG_DEC_M080040000 = "Search the stock status for monthly closing.";

        public const string MSG_DEC_M090010010 = "Receiving item information.";
        public const string MSG_DEC_M090010020 = "Receiving item classification information.";

        public const string MSG_DEC_M090010070 = "Receiving branch stock status.";
        public const string MSG_DEC_M090010080 = "Receiving closing information.";

        public const string MSG_DEC_M090020010 = "The screen to send the item information.";
        public const string MSG_DEC_M090020020 = "The screen to send the sales information.";
        public const string MSG_DEC_M090020030 = "The screen to send the branch stock information.";
        public const string MSG_DEC_M090020040 = "The screen to send the closing information.";
        public const string MSG_DEC_M090020050 = "The screen to send the receipts Information.";

        public const string MSG_NOTEXIST_ACCOUNT = "There are not existed about id or password." + "\n" + "Please check and try again.";
        public const string MSG_NOTEXIST_TINCODE = "PIN CODE information not correct." + "\n" + "Please check and try again.";
        public const string MSG_NOTEXIST_CONFIGSET = "CONFIG setting is required." + "\n" + "Please check and try again.";
    }
}
