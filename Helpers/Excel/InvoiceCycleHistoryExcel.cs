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
    public class InvoiceCycleHistoryExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Company.dbRow> companies = null;
        public List<wbm_common.DataObjects.InvoiceCycle.dbRow> invoicecycles = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow> InvoiceCycleHistorys = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.InvoiceCycleHistory.Search InvoiceCycleHistorySearchData;
        public InvoiceCycleHistoryExcel(wbm_common.DataObjects.InvoiceCycleHistory.Search InvoiceCycleHistorySearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.InvoiceCycleHistorySearchData = InvoiceCycleHistorySearchData;
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
                        InvoiceCycleHistoryPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("InvoiceCycleHistoryExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.InvoiceCycleHistory.dbRow>> rc = await InvoiceCycleHistoryHelper.ExecuteSearch(InvoiceCycleHistorySearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            InvoiceCycleHistorys = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = InvoiceCycleHistorys.Select(x => x.CreatedUserID).Union(InvoiceCycleHistorys.Select(x => x.LastAmendedUserID)).Distinct().ToList();
            List<int> ListCompanyID = InvoiceCycleHistorys.Select(x => x.CompanyID).Distinct().ToList();
            List<int> ListInvoiceCycleID = InvoiceCycleHistorys.Select(x => x.InvoiceCycleID).Distinct().ToList();

            if (ListInvoiceCycleID.Count > 0)
            {
                Result<List<wbm_common.DataObjects.InvoiceCycle.dbRow>> invoicecyclesResults = _WBMDB.InvoiceCycle_ListByListIDs(ListInvoiceCycleID).GetAwaiter().GetResult();
                if (invoicecyclesResults.IsFailure || invoicecyclesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                invoicecycles = invoicecyclesResults.Value;
            }

            if (ListCompanyID.Count > 0)
            {
                Result<List<wbm_common.DataObjects.Company.dbRow>> companiesResults = _WBMDB.Company_ListByListIDs(ListCompanyID).GetAwaiter().GetResult();
                if (companiesResults.IsFailure || companiesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                companies = companiesResults.Value;
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

        private void InvoiceCycleHistoryPage(ExcelPackage ep)
        {
            ExcelWorksheet InvoiceCycleHistoryWS = ep.Workbook.Worksheets.Add("InvoiceCycleHistory");

            #region header
            InvoiceCycleHistoryWS.View.FreezePanes(2, 1);
            InvoiceCycleHistoryWS.Cells[1, 1, 1, 15].Style.Font.Bold = true;
            InvoiceCycleHistoryWS.Cells[1, 1, 1, 15].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            InvoiceCycleHistoryWS.Column(1).Width = 15;
            InvoiceCycleHistoryWS.Column(2).Width = 25;
            InvoiceCycleHistoryWS.Column(3).Width = 25;
            InvoiceCycleHistoryWS.Column(4).Width = 25;
            InvoiceCycleHistoryWS.Column(5).Width = 25;
            InvoiceCycleHistoryWS.Column(5).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            InvoiceCycleHistoryWS.Column(6).Width = 25;
            InvoiceCycleHistoryWS.Column(7).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            InvoiceCycleHistoryWS.Column(7).Width = 20;
            InvoiceCycleHistoryWS.Column(8).Width = 15;
            InvoiceCycleHistoryWS.Column(9).Width = 25;
            InvoiceCycleHistoryWS.Column(10).Width = 25;
            InvoiceCycleHistoryWS.Column(11).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            InvoiceCycleHistoryWS.Column(11).Width = 25;
            InvoiceCycleHistoryWS.Column(12).Width = 25;
            InvoiceCycleHistoryWS.Column(13).Width = 25;
            InvoiceCycleHistoryWS.Column(14).Width = 25;
            InvoiceCycleHistoryWS.Column(15).Width = 15;

            InvoiceCycleHistoryWS.Cells[1, 1].Value = "Company ID";
            InvoiceCycleHistoryWS.Cells[1, 2].Value = "Company Name";
            InvoiceCycleHistoryWS.Cells[1, 3].Value = "Invoice Increment Days";
            InvoiceCycleHistoryWS.Cells[1, 4].Value = "Invoice Description";
            InvoiceCycleHistoryWS.Cells[1, 5].Value = "Current Period Ending";
            InvoiceCycleHistoryWS.Cells[1, 6].Value = "Description";
            InvoiceCycleHistoryWS.Cells[1, 7].Value = "Created DateTime";
            InvoiceCycleHistoryWS.Cells[1, 8].Value = "Created Method";
            InvoiceCycleHistoryWS.Cells[1, 9].Value = "Created User";
            InvoiceCycleHistoryWS.Cells[1, 10].Value = "Created User Name";
            InvoiceCycleHistoryWS.Cells[1, 11].Value = "Last Amended DateTime";
            InvoiceCycleHistoryWS.Cells[1, 12].Value = "Last Amended Method";
            InvoiceCycleHistoryWS.Cells[1, 13].Value = "Last Amended User";
            InvoiceCycleHistoryWS.Cells[1, 14].Value = "Last Amended Name";
            InvoiceCycleHistoryWS.Cells[1, 15].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < InvoiceCycleHistorys.Count(); i++)
            {
                if (companies.Any(x => x.ID == InvoiceCycleHistorys[i].CompanyID))
                {
                    InvoiceCycleHistoryWS.Cells[rc, 1].Value = companies.FirstOrDefault(x => x.ID == InvoiceCycleHistorys[i].CompanyID).Abbreviation;
                    InvoiceCycleHistoryWS.Cells[rc, 2].Value = companies.FirstOrDefault(x => x.ID == InvoiceCycleHistorys[i].CompanyID).LongName;
                }
                else
                    InvoiceCycleHistoryWS.Cells[rc, 1].Value = "CompanyID: " + InvoiceCycleHistorys[i].CompanyID.ToString();

                if (invoicecycles.Any(x => x.ID == InvoiceCycleHistorys[i].InvoiceCycleID))
                {
                    InvoiceCycleHistoryWS.Cells[rc, 3].Value = invoicecycles.FirstOrDefault(x => x.ID == InvoiceCycleHistorys[i].InvoiceCycleID).IncrementDays;
                    InvoiceCycleHistoryWS.Cells[rc, 4].Value = invoicecycles.FirstOrDefault(x => x.ID == InvoiceCycleHistorys[i].InvoiceCycleID).Description;
                }
                else
                    InvoiceCycleHistoryWS.Cells[rc, 3].Value = "InvoiceCycleID: " + InvoiceCycleHistorys[i].InvoiceCycleID.ToString();

                InvoiceCycleHistoryWS.Cells[rc, 5].Value = InvoiceCycleHistorys[i].PeriodEndingDate;
                InvoiceCycleHistoryWS.Cells[rc, 6].Value = InvoiceCycleHistorys[i].Description;

                InvoiceCycleHistoryWS.Cells[rc, 7].Value = InvoiceCycleHistorys[i].CreatedDateTime.ToOADate();
                InvoiceCycleHistoryWS.Cells[rc, 8].Value = InvoiceCycleHistorys[i].CreatedMethod;

                if (users.Any(x => x.ID == InvoiceCycleHistorys[i].CreatedUserID))
                {
                    InvoiceCycleHistoryWS.Cells[rc, 9].Value = users.FirstOrDefault(x => x.ID == InvoiceCycleHistorys[i].CreatedUserID).Firstname;
                    InvoiceCycleHistoryWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == InvoiceCycleHistorys[i].CreatedUserID).PublicID;
                }
                else
                    InvoiceCycleHistoryWS.Cells[rc, 9].Value = "UserID: " + InvoiceCycleHistorys[i].CreatedUserID.ToString();

                InvoiceCycleHistoryWS.Cells[rc, 11].Value = InvoiceCycleHistorys[i].LastAmendedDateTime.ToOADate();
                InvoiceCycleHistoryWS.Cells[rc, 12].Value = InvoiceCycleHistorys[i].LastAmendedMethod;

                if (users.Any(x => x.ID == InvoiceCycleHistorys[i].LastAmendedUserID))
                {
                    InvoiceCycleHistoryWS.Cells[rc, 13].Value = users.FirstOrDefault(x => x.ID == InvoiceCycleHistorys[i].LastAmendedUserID).Firstname;
                    InvoiceCycleHistoryWS.Cells[rc, 14].Value = users.FirstOrDefault(x => x.ID == InvoiceCycleHistorys[i].LastAmendedUserID).PublicID;
                }
                else
                    InvoiceCycleHistoryWS.Cells[rc, 13].Value = "UserID: " + InvoiceCycleHistorys[i].LastAmendedUserID.ToString();

                InvoiceCycleHistoryWS.Cells[rc, 15].Value = InvoiceCycleHistorys[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
