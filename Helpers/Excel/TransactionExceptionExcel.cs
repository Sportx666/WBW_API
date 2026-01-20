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
    public class TransactionExceptionExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.TransactionExceptionType.dbRow> transactionexceptiontype = null;
        public List<wbm_common.DataObjects.TransactionHeader.dbRow> headers = null;
        public List<wbm_common.DataObjects.TransactionDetail.dbRow> details = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.TransactionException.dbRow> TransactionExceptions = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.TransactionException.Search TransactionExceptionSearchData;
        public TransactionExceptionExcel(wbm_common.DataObjects.TransactionException.Search TransactionExceptionSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.TransactionExceptionSearchData = TransactionExceptionSearchData;
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
                        TransactionExceptionPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("TransactionExceptionExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.TransactionException.dbRow>> rc = await TransactionExceptionHelper.ExecuteSearch(TransactionExceptionSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            TransactionExceptions = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = TransactionExceptions.Select(x => x.CreatedUserID).Union(TransactionExceptions.Select(x => x.LastAmendedUserID)).Distinct().ToList();
            List<int> ListDetailIDs = TransactionExceptions.Select(x => x.TransactionDetailID).Distinct().ToList();
            List<int> ListHeaderIDs = TransactionExceptions.Select(x => x.TransactionHeaderID).Distinct().ToList();
            List<int> ListExceptionTypeIDs = TransactionExceptions.Select(x => x.TransactionExceptionTypeID).Distinct().ToList();

            if (ListExceptionTypeIDs.Count > 0)
            {
                Result<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>> transactionexceptiontypeResults = _WBMDB.TransactionExceptionType_ListByListIDs(ListExceptionTypeIDs).GetAwaiter().GetResult();
                if (transactionexceptiontypeResults.IsFailure || transactionexceptiontypeResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                transactionexceptiontype = transactionexceptiontypeResults.Value;
            }

            if (ListHeaderIDs.Count > 0)
            {
                Result<List<wbm_common.DataObjects.TransactionHeader.dbRow>> headersResults = _WBMDB.TransactionHeader_ListByListIDs(ListHeaderIDs).GetAwaiter().GetResult();
                if (headersResults.IsFailure || headersResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                headers = headersResults.Value;
            }

            if (ListDetailIDs.Count > 0)
            {
                Result<List<wbm_common.DataObjects.TransactionDetail.dbRow>> detailsResults = _WBMDB.TransactionDetail_ListByListIDs(ListDetailIDs).GetAwaiter().GetResult();
                if (detailsResults.IsFailure || detailsResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                details = detailsResults.Value;
            }

            if (ListUserID.Count > 0)
            {
                Result<List<wbm_common.DataObjects.Security_User.dbRow>> userResults = _WBMDB.SecurityUser_ListByListID(ListUserID).GetAwaiter().GetResult();
                if (userResults.IsFailure || userResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                users = userResults.Value;
            }

            return true;
        }

        private void TransactionExceptionPage(ExcelPackage ep)
        {
            ExcelWorksheet TransactionExceptionWS = ep.Workbook.Worksheets.Add("TransactionException");

            #region header
            TransactionExceptionWS.View.FreezePanes(2, 1);
            TransactionExceptionWS.Cells[1, 1, 1, 19].Style.Font.Bold = true;
            TransactionExceptionWS.Cells[1, 1, 1, 19].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            TransactionExceptionWS.Column(1).Width = 30;
            TransactionExceptionWS.Column(2).Width = 25;
            TransactionExceptionWS.Column(3).Width = 25;
            TransactionExceptionWS.Column(4).Width = 25;
            TransactionExceptionWS.Column(5).Width = 25;
            TransactionExceptionWS.Column(6).Width = 25;
            TransactionExceptionWS.Column(7).Width = 25;
            TransactionExceptionWS.Column(7).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            TransactionExceptionWS.Column(8).Width = 25;
            TransactionExceptionWS.Column(9).Width = 25;
            TransactionExceptionWS.Column(10).Width = 25;
            TransactionExceptionWS.Column(11).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            TransactionExceptionWS.Column(11).Width = 25;
            TransactionExceptionWS.Column(12).Width = 25;
            TransactionExceptionWS.Column(13).Width = 25;
            TransactionExceptionWS.Column(14).Width = 25;
            TransactionExceptionWS.Column(15).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            TransactionExceptionWS.Column(15).Width = 25;
            TransactionExceptionWS.Column(16).Width = 25;
            TransactionExceptionWS.Column(17).Width = 25;
            TransactionExceptionWS.Column(18).Width = 25;
            TransactionExceptionWS.Column(19).Width = 15;

            TransactionExceptionWS.Cells[1, 1].Value = "Transaction Header Description";
            TransactionExceptionWS.Cells[1, 2].Value = "Transaction Detail ID";
            TransactionExceptionWS.Cells[1, 3].Value = "Transaction Exception Type";
            TransactionExceptionWS.Cells[1, 4].Value = "Transcation Exception Type Description";
            TransactionExceptionWS.Cells[1, 5].Value = "Description";
            TransactionExceptionWS.Cells[1, 6].Value = "Cleared Flag";
            TransactionExceptionWS.Cells[1, 7].Value = "Cleared DateTime";
            TransactionExceptionWS.Cells[1, 8].Value = "Cleared Method";
            TransactionExceptionWS.Cells[1, 9].Value = "Cleared User";
            TransactionExceptionWS.Cells[1, 10].Value = "Cleared User Name";
            TransactionExceptionWS.Cells[1, 11].Value = "Created DateTime";
            TransactionExceptionWS.Cells[1, 12].Value = "Created Method";
            TransactionExceptionWS.Cells[1, 13].Value = "Created User";
            TransactionExceptionWS.Cells[1, 14].Value = "Created User Name";
            TransactionExceptionWS.Cells[1, 15].Value = "Last Amended DateTime";
            TransactionExceptionWS.Cells[1, 16].Value = "Last Amended Method";
            TransactionExceptionWS.Cells[1, 17].Value = "Last Amended User";
            TransactionExceptionWS.Cells[1, 18].Value = "Last Amended Name";
            TransactionExceptionWS.Cells[1, 19].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < TransactionExceptions.Count(); i++)
            {
                if (headers.Any(x => x.ID == TransactionExceptions[i].TransactionHeaderID))
                    TransactionExceptionWS.Cells[rc, 1].Value = headers.FirstOrDefault(x => x.ID == TransactionExceptions[i].TransactionHeaderID).Description;
                else
                    TransactionExceptionWS.Cells[rc, 1].Value = "TransactionHeaderID: " + TransactionExceptions[i].TransactionHeaderID.ToString();

                if (details.Any(x => x.ID == TransactionExceptions[i].TransactionDetailID))
                    TransactionExceptionWS.Cells[rc, 2].Value = details.FirstOrDefault(x => x.ID == TransactionExceptions[i].TransactionDetailID).FullDescription;
                else
                    TransactionExceptionWS.Cells[rc, 2].Value = "TransactionDetailID: " + TransactionExceptions[i].TransactionDetailID.ToString();

                if (transactionexceptiontype.Any(x => x.ID == TransactionExceptions[i].TransactionExceptionTypeID))
                {
                    TransactionExceptionWS.Cells[rc, 3].Value = transactionexceptiontype.FirstOrDefault(x => x.ID == TransactionExceptions[i].TransactionExceptionTypeID).PublicID;
                    TransactionExceptionWS.Cells[rc, 4].Value = transactionexceptiontype.FirstOrDefault(x => x.ID == TransactionExceptions[i].TransactionExceptionTypeID).Description;
                }
                else
                    TransactionExceptionWS.Cells[rc, 3].Value = "TransactionExceptionTypeID: " + TransactionExceptions[i].TransactionExceptionTypeID.ToString();

                TransactionExceptionWS.Cells[rc, 5].Value = TransactionExceptions[i].Description;
                TransactionExceptionWS.Cells[rc, 6].Value = TransactionExceptions[i].ClearedFlag;
                TransactionExceptionWS.Cells[rc, 7].Value = TransactionExceptions[i].ClearedDateTime.ToOADate();
                TransactionExceptionWS.Cells[rc, 8].Value = TransactionExceptions[i].ClearedMethod;

                if (users.Any(x => x.ID == TransactionExceptions[i].ClearedUserID))
                {
                    TransactionExceptionWS.Cells[rc, 9].Value = users.FirstOrDefault(x => x.ID == TransactionExceptions[i].ClearedUserID).PublicID;
                    TransactionExceptionWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == TransactionExceptions[i].ClearedUserID).Firstname;
                }
                else
                    TransactionExceptionWS.Cells[rc, 9].Value = "ClearedUserID: " + TransactionExceptions[i].ClearedUserID.ToString();

                TransactionExceptionWS.Cells[rc, 11].Value = TransactionExceptions[i].CreatedDateTime.ToOADate();
                TransactionExceptionWS.Cells[rc, 12].Value = TransactionExceptions[i].CreatedMethod;

                if (users.Any(x => x.ID == TransactionExceptions[i].CreatedUserID))
                {
                    TransactionExceptionWS.Cells[rc, 13].Value = users.FirstOrDefault(x => x.ID == TransactionExceptions[i].CreatedUserID).PublicID;
                    TransactionExceptionWS.Cells[rc, 14].Value = users.FirstOrDefault(x => x.ID == TransactionExceptions[i].CreatedUserID).Firstname;
                }
                else
                    TransactionExceptionWS.Cells[rc, 13].Value = "UserID: " + TransactionExceptions[i].CreatedUserID.ToString();

                TransactionExceptionWS.Cells[rc, 15].Value = TransactionExceptions[i].LastAmendedDateTime.ToOADate();
                TransactionExceptionWS.Cells[rc, 16].Value = TransactionExceptions[i].LastAmendedMethod;

                if (users.Any(x => x.ID == TransactionExceptions[i].LastAmendedUserID))
                {
                    TransactionExceptionWS.Cells[rc, 17].Value = users.FirstOrDefault(x => x.ID == TransactionExceptions[i].LastAmendedUserID).PublicID;
                    TransactionExceptionWS.Cells[rc, 18].Value = users.FirstOrDefault(x => x.ID == TransactionExceptions[i].LastAmendedUserID).Firstname;
                }
                else
                    TransactionExceptionWS.Cells[rc, 17].Value = "UserID: " + TransactionExceptions[i].LastAmendedUserID.ToString();

                TransactionExceptionWS.Cells[rc, 19].Value = TransactionExceptions[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
