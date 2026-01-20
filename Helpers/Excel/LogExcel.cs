using Microsoft.AspNetCore.SignalR;
using NLog;
using NLog.Web;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using WBM_API.Models;
using WBM_API.WBM_API_DB;
using wbm_common.DataObjects;


namespace WBM_API.Helpers.Excel
{
    public class LogExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Log.dbRow> Logs = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.Log.Search LogSearchData;
        public LogExcel(wbm_common.DataObjects.Log.Search LogSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.LogSearchData = LogSearchData;
            _WBMDB = wBMDB;
        }
        public async Task<bool> GenerateExcel(Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> inCallBackStatusFunction = null, IHubCallerClients inClients = null, wbm_common.DataObjects.LongRunProcessHeader.dbRow longRunProcessHeader = null)
        {
            CallBackStatusFunction = inCallBackStatusFunction;
            Clients = inClients;
            lrph = longRunProcessHeader;
            if (await GetData())
            {
                try
                {
                    using (ExcelPackage ep = new ExcelPackage(ms))
                    {
                        LogPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("LogExcelReport: " + excpt.Message);
                    return false;
                }
            }
            else
                HasErrors = true;

            return !HasErrors;
        }

        private async Task<bool> callCallBackFunctionFunction(string StatusDescription, int CurC, int maxc)
        {
            if (CallBackStatusFunction != null)
                await CallBackStatusFunction(Clients, _WBMDB, StatusDescription, CurC, maxc, lrph);
            return true;
        }

        private async Task<bool> GetData()
        {

            Result<List<wbm_common.DataObjects.Log.dbRow>> rc = await LogHelper.ExecuteSearch(LogSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            Logs = rc.Value;

            return true;
        }

        private void LogPage(ExcelPackage ep)
        {
            ExcelWorksheet LogWS = ep.Workbook.Worksheets.Add("Log");

            #region header
            LogWS.View.FreezePanes(2, 1);
            LogWS.Cells[1, 1, 1, 12].Style.Font.Bold = true;
            LogWS.Cells[1, 1, 1, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            LogWS.Column(1).Width = 15;
            LogWS.Column(2).Width = 25;
            LogWS.Column(2).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            LogWS.Column(3).Width = 25;
            LogWS.Column(4).Width = 25;
            LogWS.Column(5).Width = 25;
            LogWS.Column(6).Width = 25;
            LogWS.Column(7).Width = 25;
            LogWS.Column(8).Width = 25;
            LogWS.Column(9).Width = 25;
            LogWS.Column(10).Width = 25;
            LogWS.Column(11).Width = 25;
            LogWS.Column(12).Width = 25;
            LogWS.Column(13).Width = 25;

            LogWS.Cells[1, 1].Value = "ID";
            LogWS.Cells[1, 2].Value = "CallTime";
            LogWS.Cells[1, 3].Value = "APIEndPoint";
            LogWS.Cells[1, 4].Value = "Authenticated";
            LogWS.Cells[1, 5].Value = "UserID";
            LogWS.Cells[1, 6].Value = "UserName";
            LogWS.Cells[1, 7].Value = "CallSuccess";
            LogWS.Cells[1, 8].Value = "CallingData";
            LogWS.Cells[1, 9].Value = "ErrorText";
            LogWS.Cells[1, 10].Value = "ReturnData";
            LogWS.Cells[1, 11].Value = "CallLength";
            LogWS.Cells[1, 12].Value = "APICallReference";
            LogWS.Cells[1, 13].Value = "APITransactionReference";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < Logs.Count(); i++)
            {
                LogWS.Cells[rc, 1].Value = Logs[i].ID;
                LogWS.Cells[rc, 2].Value = Logs[i].CallTime.ToOADate();
                LogWS.Cells[rc, 3].Value = Logs[i].APIEndPoint;
                LogWS.Cells[rc, 4].Value = Logs[i].Authenticated;
                LogWS.Cells[rc, 5].Value = Logs[i].UserID;
                LogWS.Cells[rc, 6].Value = Logs[i].UserName;
                LogWS.Cells[rc, 7].Value = Logs[i].CallSuccess;
                LogWS.Cells[rc, 8].Value = Logs[i].CallingData;
                LogWS.Cells[rc, 9].Value = Logs[i].ErrorText;
                LogWS.Cells[rc, 10].Value = Logs[i].ReturnData;
                LogWS.Cells[rc, 11].Value = Logs[i].CallLength;
                LogWS.Cells[rc, 12].Value = Logs[i].APICallReference;
                LogWS.Cells[rc, 13].Value = Logs[i].APITransactionReference;

                rc++;
            }
            #endregion
        }
    }
}
