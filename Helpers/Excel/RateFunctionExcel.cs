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
    public class RateFunctionExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.RateFunction.dbRow> RateFunctions = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.RateFunction.Search RateFunctionSearchData;
        public RateFunctionExcel(wbm_common.DataObjects.RateFunction.Search RateFunctionSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.RateFunctionSearchData = RateFunctionSearchData;
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
                        RateFunctionPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("RateFunctionExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.RateFunction.dbRow>> rc = await RateFunctionHelper.ExecuteSearch(RateFunctionSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            RateFunctions = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = RateFunctions.Select(x => x.CreatedUserID).Union(RateFunctions.Select(x => x.LastAmendedUserID)).Distinct().ToList();


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

        private void RateFunctionPage(ExcelPackage ep)
        {
            ExcelWorksheet RateFunctionWS = ep.Workbook.Worksheets.Add("RateFunction");

            #region header
            RateFunctionWS.View.FreezePanes(2, 1);
            RateFunctionWS.Cells[1, 1, 1, 13].Style.Font.Bold = true;
            RateFunctionWS.Cells[1, 1, 1, 13].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            RateFunctionWS.Column(1).Width = 15;
            RateFunctionWS.Column(2).Width = 15;
            RateFunctionWS.Column(3).Width = 10;
            RateFunctionWS.Column(4).Width = 25;
            RateFunctionWS.Column(5).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            RateFunctionWS.Column(5).Width = 25;
            RateFunctionWS.Column(6).Width = 25;
            RateFunctionWS.Column(7).Width = 25;
            RateFunctionWS.Column(9).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            RateFunctionWS.Column(8).Width = 25;
            RateFunctionWS.Column(9).Width = 25;
            RateFunctionWS.Column(10).Width = 25;
            RateFunctionWS.Column(11).Width = 25;
            RateFunctionWS.Column(12).Width = 25;
            RateFunctionWS.Column(13).Width = 15;

            RateFunctionWS.Cells[1, 1].Value = "ID";
            RateFunctionWS.Cells[1, 2].Value = "Description";
            RateFunctionWS.Cells[1, 3].Value = "Sort Order";
            RateFunctionWS.Cells[1, 4].Value = "Can Have Multiple";
            RateFunctionWS.Cells[1, 5].Value = "Created DateTime";
            RateFunctionWS.Cells[1, 6].Value = "Created Method";
            RateFunctionWS.Cells[1, 7].Value = "Created User";
            RateFunctionWS.Cells[1, 8].Value = "Created User Name";
            RateFunctionWS.Cells[1, 9].Value = "Last Amended DateTime";
            RateFunctionWS.Cells[1, 10].Value = "Last Amended Method";
            RateFunctionWS.Cells[1, 11].Value = "Last Amended User";
            RateFunctionWS.Cells[1, 12].Value = "Last Amended Name";
            RateFunctionWS.Cells[1, 13].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            RateFunctions = RateFunctions.OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < RateFunctions.Count(); i++)
            {
                RateFunctionWS.Cells[rc, 1].Value = RateFunctions[i].PublicID;
                RateFunctionWS.Cells[rc, 2].Value = RateFunctions[i].Description;
                RateFunctionWS.Cells[rc, 3].Value = RateFunctions[i].SortOrder;
                RateFunctionWS.Cells[rc, 4].Value = RateFunctions[i].CanHaveMultiple;
                RateFunctionWS.Cells[rc, 5].Value = RateFunctions[i].CreatedDateTime.ToOADate();
                RateFunctionWS.Cells[rc, 6].Value = RateFunctions[i].CreatedMethod;

                if (users.Any(x => x.ID == RateFunctions[i].CreatedUserID))
                {
                    RateFunctionWS.Cells[rc, 7].Value = users.FirstOrDefault(x => x.ID == RateFunctions[i].CreatedUserID).PublicID;
                    RateFunctionWS.Cells[rc, 8].Value = users.FirstOrDefault(x => x.ID == RateFunctions[i].CreatedUserID).Firstname;
                }
                else
                    RateFunctionWS.Cells[rc, 7].Value = "UserID: " + RateFunctions[i].CreatedUserID.ToString();

                RateFunctionWS.Cells[rc, 9].Value = RateFunctions[i].LastAmendedDateTime.ToOADate();
                RateFunctionWS.Cells[rc, 10].Value = RateFunctions[i].LastAmendedMethod;

                if (users.Any(x => x.ID == RateFunctions[i].LastAmendedUserID))
                {
                    RateFunctionWS.Cells[rc, 11].Value = users.FirstOrDefault(x => x.ID == RateFunctions[i].LastAmendedUserID).PublicID;
                    RateFunctionWS.Cells[rc, 12].Value = users.FirstOrDefault(x => x.ID == RateFunctions[i].LastAmendedUserID).Firstname;
                }
                else
                    RateFunctionWS.Cells[rc, 11].Value = "UserID: " + RateFunctions[i].LastAmendedUserID.ToString();

                RateFunctionWS.Cells[rc, 13].Value = RateFunctions[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
