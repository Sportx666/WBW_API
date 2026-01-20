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
    public class TransactionSourceExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.TransactionSource.dbRow> TransactionSources = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.TransactionSource.Search TransactionSourceSearchData;
        public TransactionSourceExcel(wbm_common.DataObjects.TransactionSource.Search TransactionSourceSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.TransactionSourceSearchData = TransactionSourceSearchData;
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
                        TransactionSourcePage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("TransactionSourceExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.TransactionSource.dbRow>> rc = await TransactionSourceHelper.ExecuteSearch(TransactionSourceSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            TransactionSources = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = TransactionSources.Select(x => x.CreatedUserID).Union(TransactionSources.Select(x => x.LastAmendedUserID)).Distinct().ToList();


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

        private void TransactionSourcePage(ExcelPackage ep)
        {
            ExcelWorksheet TransactionSourceWS = ep.Workbook.Worksheets.Add("TransactionSource");

            #region header
            TransactionSourceWS.View.FreezePanes(2, 1);
            TransactionSourceWS.Cells[1, 1, 1, 12].Style.Font.Bold = true;
            TransactionSourceWS.Cells[1, 1, 1, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            TransactionSourceWS.Column(1).Width = 15;
            TransactionSourceWS.Column(2).Width = 15;
            TransactionSourceWS.Column(3).Width = 10;
            TransactionSourceWS.Column(4).Width = 25;
            TransactionSourceWS.Column(4).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            TransactionSourceWS.Column(5).Width = 25;
            TransactionSourceWS.Column(6).Width = 25;
            TransactionSourceWS.Column(7).Width = 25;
            TransactionSourceWS.Column(8).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            TransactionSourceWS.Column(8).Width = 25;
            TransactionSourceWS.Column(9).Width = 25;
            TransactionSourceWS.Column(10).Width = 25;
            TransactionSourceWS.Column(11).Width = 25;
            TransactionSourceWS.Column(12).Width = 15;

            TransactionSourceWS.Cells[1, 1].Value = "ID";
            TransactionSourceWS.Cells[1, 2].Value = "Description";
            TransactionSourceWS.Cells[1, 3].Value = "Sort Order";
            TransactionSourceWS.Cells[1, 4].Value = "Created DateTime";
            TransactionSourceWS.Cells[1, 5].Value = "Created Method";
            TransactionSourceWS.Cells[1, 6].Value = "Created User";
            TransactionSourceWS.Cells[1, 7].Value = "Created User Name";
            TransactionSourceWS.Cells[1, 8].Value = "Last Amended DateTime";
            TransactionSourceWS.Cells[1, 9].Value = "Last Amended Method";
            TransactionSourceWS.Cells[1, 10].Value = "Last Amended User";
            TransactionSourceWS.Cells[1, 11].Value = "Last Amended Name";
            TransactionSourceWS.Cells[1, 12].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            TransactionSources = TransactionSources.OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < TransactionSources.Count(); i++)
            {
                TransactionSourceWS.Cells[rc, 1].Value = TransactionSources[i].PublicID;
                TransactionSourceWS.Cells[rc, 2].Value = TransactionSources[i].Description;
                TransactionSourceWS.Cells[rc, 3].Value = TransactionSources[i].SortOrder;
                TransactionSourceWS.Cells[rc, 4].Value = TransactionSources[i].CreatedDateTime.ToOADate();
                TransactionSourceWS.Cells[rc, 5].Value = TransactionSources[i].CreatedMethod;

                if (users.Any(x => x.ID == TransactionSources[i].CreatedUserID))
                {
                    TransactionSourceWS.Cells[rc, 6].Value = users.FirstOrDefault(x => x.ID == TransactionSources[i].CreatedUserID).PublicID;
                    TransactionSourceWS.Cells[rc, 7].Value = users.FirstOrDefault(x => x.ID == TransactionSources[i].CreatedUserID).Firstname;
                }
                else
                    TransactionSourceWS.Cells[rc, 6].Value = "UserID: " + TransactionSources[i].CreatedUserID.ToString();

                TransactionSourceWS.Cells[rc, 8].Value = TransactionSources[i].LastAmendedDateTime.ToOADate();
                TransactionSourceWS.Cells[rc, 9].Value = TransactionSources[i].LastAmendedMethod;

                if (users.Any(x => x.ID == TransactionSources[i].LastAmendedUserID))
                {
                    TransactionSourceWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == TransactionSources[i].LastAmendedUserID).PublicID;
                    TransactionSourceWS.Cells[rc, 11].Value = users.FirstOrDefault(x => x.ID == TransactionSources[i].LastAmendedUserID).Firstname;
                }
                else
                    TransactionSourceWS.Cells[rc, 10].Value = "UserID: " + TransactionSources[i].LastAmendedUserID.ToString();

                TransactionSourceWS.Cells[rc, 12].Value = TransactionSources[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
