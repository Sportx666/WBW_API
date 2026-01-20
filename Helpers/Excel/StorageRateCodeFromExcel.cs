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
    public class StorageRateCodeFromExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.StorageRateCodeFrom.dbRow> StorageRateCodeFroms = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.StorageRateCodeFrom.Search StorageRateCodeFromSearchData;
        public StorageRateCodeFromExcel(wbm_common.DataObjects.StorageRateCodeFrom.Search StorageRateCodeFromSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.StorageRateCodeFromSearchData = StorageRateCodeFromSearchData;
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
                        StorageRateCodeFromPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("StorageRateCodeFromExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.StorageRateCodeFrom.dbRow>> rc = await StorageRateCodeFromHelper.ExecuteSearch(StorageRateCodeFromSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            StorageRateCodeFroms = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = StorageRateCodeFroms.Select(x => x.CreatedUserID).Union(StorageRateCodeFroms.Select(x => x.LastAmendedUserID)).Distinct().ToList();


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

        private void StorageRateCodeFromPage(ExcelPackage ep)
        {
            ExcelWorksheet StorageRateCodeFromWS = ep.Workbook.Worksheets.Add("StorageRateCodeFrom");

            #region header
            StorageRateCodeFromWS.View.FreezePanes(2, 1);
            StorageRateCodeFromWS.Cells[1, 1, 1, 12].Style.Font.Bold = true;
            StorageRateCodeFromWS.Cells[1, 1, 1, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            StorageRateCodeFromWS.Column(1).Width = 15;
            StorageRateCodeFromWS.Column(2).Width = 15;
            StorageRateCodeFromWS.Column(3).Width = 10;
            StorageRateCodeFromWS.Column(4).Width = 25;
            StorageRateCodeFromWS.Column(4).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            StorageRateCodeFromWS.Column(5).Width = 25;
            StorageRateCodeFromWS.Column(6).Width = 25;
            StorageRateCodeFromWS.Column(7).Width = 25;
            StorageRateCodeFromWS.Column(8).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            StorageRateCodeFromWS.Column(8).Width = 25;
            StorageRateCodeFromWS.Column(9).Width = 25;
            StorageRateCodeFromWS.Column(10).Width = 25;
            StorageRateCodeFromWS.Column(11).Width = 25;
            StorageRateCodeFromWS.Column(12).Width = 15;

            StorageRateCodeFromWS.Cells[1, 1].Value = "ID";
            StorageRateCodeFromWS.Cells[1, 2].Value = "Description";
            StorageRateCodeFromWS.Cells[1, 3].Value = "Sort Order";
            StorageRateCodeFromWS.Cells[1, 4].Value = "Created DateTime";
            StorageRateCodeFromWS.Cells[1, 5].Value = "Created Method";
            StorageRateCodeFromWS.Cells[1, 6].Value = "Created User";
            StorageRateCodeFromWS.Cells[1, 7].Value = "Created User Name";
            StorageRateCodeFromWS.Cells[1, 8].Value = "Last Amended DateTime";
            StorageRateCodeFromWS.Cells[1, 9].Value = "Last Amended Method";
            StorageRateCodeFromWS.Cells[1, 10].Value = "Last Amended User";
            StorageRateCodeFromWS.Cells[1, 11].Value = "Last Amended Name";
            StorageRateCodeFromWS.Cells[1, 12].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            StorageRateCodeFroms = StorageRateCodeFroms.OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < StorageRateCodeFroms.Count(); i++)
            {
                StorageRateCodeFromWS.Cells[rc, 1].Value = StorageRateCodeFroms[i].PublicID;
                StorageRateCodeFromWS.Cells[rc, 2].Value = StorageRateCodeFroms[i].Description;
                StorageRateCodeFromWS.Cells[rc, 3].Value = StorageRateCodeFroms[i].SortOrder;
                StorageRateCodeFromWS.Cells[rc, 4].Value = StorageRateCodeFroms[i].CreatedDateTime.ToOADate();
                StorageRateCodeFromWS.Cells[rc, 5].Value = StorageRateCodeFroms[i].CreatedMethod;

                if (users.Any(x => x.ID == StorageRateCodeFroms[i].CreatedUserID))
                {
                    StorageRateCodeFromWS.Cells[rc, 6].Value = users.FirstOrDefault(x => x.ID == StorageRateCodeFroms[i].CreatedUserID).PublicID;
                    StorageRateCodeFromWS.Cells[rc, 7].Value = users.FirstOrDefault(x => x.ID == StorageRateCodeFroms[i].CreatedUserID).Firstname;
                }
                else
                    StorageRateCodeFromWS.Cells[rc, 6].Value = "UserID: " + StorageRateCodeFroms[i].CreatedUserID.ToString();

                StorageRateCodeFromWS.Cells[rc, 8].Value = StorageRateCodeFroms[i].LastAmendedDateTime.ToOADate();
                StorageRateCodeFromWS.Cells[rc, 9].Value = StorageRateCodeFroms[i].LastAmendedMethod;

                if (users.Any(x => x.ID == StorageRateCodeFroms[i].LastAmendedUserID))
                {
                    StorageRateCodeFromWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == StorageRateCodeFroms[i].LastAmendedUserID).PublicID;
                    StorageRateCodeFromWS.Cells[rc, 11].Value = users.FirstOrDefault(x => x.ID == StorageRateCodeFroms[i].LastAmendedUserID).Firstname;
                }
                else
                    StorageRateCodeFromWS.Cells[rc, 10].Value = "UserID: " + StorageRateCodeFroms[i].LastAmendedUserID.ToString();

                StorageRateCodeFromWS.Cells[rc, 12].Value = StorageRateCodeFroms[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
