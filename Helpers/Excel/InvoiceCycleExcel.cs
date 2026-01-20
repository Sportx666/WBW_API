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
    public class InvoiceCycleExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.InvoiceCycle.dbRow> InvoiceCycles = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.InvoiceCycle.Search InvoiceCycleSearchData;
        public InvoiceCycleExcel(wbm_common.DataObjects.InvoiceCycle.Search InvoiceCycleSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.InvoiceCycleSearchData = InvoiceCycleSearchData;
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
                        InvoiceCyclePage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("InvoiceCycleExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.InvoiceCycle.dbRow>> rc = await InvoiceCycleHelper.ExecuteSearch(InvoiceCycleSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            InvoiceCycles = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = InvoiceCycles.Select(x => x.CreatedUserID).Union(InvoiceCycles.Select(x => x.LastAmendedUserID)).Distinct().ToList();

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

        private void InvoiceCyclePage(ExcelPackage ep)
        {
            ExcelWorksheet InvoiceCycleWS = ep.Workbook.Worksheets.Add("InvoiceCycle");

            #region header
            InvoiceCycleWS.View.FreezePanes(2, 1);
            InvoiceCycleWS.Cells[1, 1, 1, 14].Style.Font.Bold = true;
            InvoiceCycleWS.Cells[1, 1, 1, 14].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            InvoiceCycleWS.Column(1).Width = 12;
            InvoiceCycleWS.Column(2).Width = 25;
            InvoiceCycleWS.Column(3).Width = 15;
            InvoiceCycleWS.Column(4).Width = 25;
            InvoiceCycleWS.Column(5).Width = 25;
            InvoiceCycleWS.Column(6).Width = 25;
            InvoiceCycleWS.Column(6).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            InvoiceCycleWS.Column(7).Width = 20;
            InvoiceCycleWS.Column(8).Width = 15;
            InvoiceCycleWS.Column(9).Width = 25;
            InvoiceCycleWS.Column(10).Width = 25;
            InvoiceCycleWS.Column(10).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            InvoiceCycleWS.Column(11).Width = 25;
            InvoiceCycleWS.Column(12).Width = 25;
            InvoiceCycleWS.Column(13).Width = 25;
            InvoiceCycleWS.Column(14).Width = 15;

            InvoiceCycleWS.Cells[1, 1].Value = "UserID";
            InvoiceCycleWS.Cells[1, 2].Value = "Description";
            InvoiceCycleWS.Cells[1, 3].Value = "Increment Days";
            InvoiceCycleWS.Cells[1, 4].Value = "Allow User Set Next Date";
            InvoiceCycleWS.Cells[1, 5].Value = "Calculate Next Date Rule ID";
            InvoiceCycleWS.Cells[1, 6].Value = "Created DateTime";
            InvoiceCycleWS.Cells[1, 7].Value = "Created Method";
            InvoiceCycleWS.Cells[1, 8].Value = "Created User";
            InvoiceCycleWS.Cells[1, 9].Value = "Created User Name";
            InvoiceCycleWS.Cells[1, 10].Value = "Last Amended DateTime";
            InvoiceCycleWS.Cells[1, 11].Value = "Last Amended Method";
            InvoiceCycleWS.Cells[1, 12].Value = "Last Amended User";
            InvoiceCycleWS.Cells[1, 13].Value = "Last Amended Name";
            InvoiceCycleWS.Cells[1, 14].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < InvoiceCycles.Count(); i++)
            {
                //if (users.Any(x => x.ID == InvoiceCycles[i].UserID))
                //    InvoiceCycleWS.Cells[rc, 1].Value = users.FirstOrDefault(x => x.ID == InvoiceCycles[i].UserID).ShortName;
                //else
                //    InvoiceCycleWS.Cells[rc, 1].Value = "UserID: " + InvoiceCycles[i].UserID.ToString();

                InvoiceCycleWS.Cells[rc, 2].Value = InvoiceCycles[i].Description;
                InvoiceCycleWS.Cells[rc, 3].Value = InvoiceCycles[i].IncrementDays;
                InvoiceCycleWS.Cells[rc, 4].Value = InvoiceCycles[i].AllowUserSetNextDate;
                InvoiceCycleWS.Cells[rc, 5].Value = InvoiceCycles[i].CalculateNextDateRuleID;

                InvoiceCycleWS.Cells[rc, 6].Value = InvoiceCycles[i].CreatedDateTime.ToOADate();
                InvoiceCycleWS.Cells[rc, 7].Value = InvoiceCycles[i].CreatedMethod;

                if (users.Any(x => x.ID == InvoiceCycles[i].CreatedUserID))
                {
                    InvoiceCycleWS.Cells[rc, 8].Value = users.FirstOrDefault(x => x.ID == InvoiceCycles[i].CreatedUserID).Firstname;
                    InvoiceCycleWS.Cells[rc, 9].Value = users.FirstOrDefault(x => x.ID == InvoiceCycles[i].CreatedUserID).PublicID;
                }
                else
                    InvoiceCycleWS.Cells[rc, 8].Value = "UserID: " + InvoiceCycles[i].CreatedUserID.ToString();

                InvoiceCycleWS.Cells[rc, 10].Value = InvoiceCycles[i].LastAmendedDateTime.ToOADate();
                InvoiceCycleWS.Cells[rc, 11].Value = InvoiceCycles[i].LastAmendedMethod;

                if (users.Any(x => x.ID == InvoiceCycles[i].LastAmendedUserID))
                {
                    InvoiceCycleWS.Cells[rc, 12].Value = users.FirstOrDefault(x => x.ID == InvoiceCycles[i].LastAmendedUserID).Firstname;
                    InvoiceCycleWS.Cells[rc, 13].Value = users.FirstOrDefault(x => x.ID == InvoiceCycles[i].LastAmendedUserID).PublicID;
                }
                else
                    InvoiceCycleWS.Cells[rc, 12].Value = "UserID: " + InvoiceCycles[i].LastAmendedUserID.ToString();

                InvoiceCycleWS.Cells[rc, 14].Value = InvoiceCycles[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
