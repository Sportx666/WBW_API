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
    public class InvoiceCycleCurrentExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Company.dbRow> companies = null;
        public List<wbm_common.DataObjects.InvoiceCycle.dbRow> invoicecycles = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow> InvoiceCycleCurrents = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.InvoiceCycleCurrent.Search InvoiceCycleCurrentSearchData;
        public InvoiceCycleCurrentExcel(wbm_common.DataObjects.InvoiceCycleCurrent.Search InvoiceCycleCurrentSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.InvoiceCycleCurrentSearchData = InvoiceCycleCurrentSearchData;
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
                        InvoiceCycleCurrentPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("InvoiceCycleCurrentExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.InvoiceCycleCurrent.dbRow>> rc = await InvoiceCycleCurrentHelper.ExecuteSearch(InvoiceCycleCurrentSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            InvoiceCycleCurrents = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = InvoiceCycleCurrents.Select(x => x.CreatedUserID).Union(InvoiceCycleCurrents.Select(x => x.LastAmendedUserID)).Distinct().ToList();
            List<int> ListCompanyID = InvoiceCycleCurrents.Select(x => x.CompanyID).Distinct().ToList();
            List<int> ListInvoiceCycleID = InvoiceCycleCurrents.Select(x => x.InvoiceCycleID).Distinct().ToList();

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

        private void InvoiceCycleCurrentPage(ExcelPackage ep)
        {
            ExcelWorksheet InvoiceCycleCurrentWS = ep.Workbook.Worksheets.Add("InvoiceCycleCurrent");

            #region header
            InvoiceCycleCurrentWS.View.FreezePanes(2, 1);
            InvoiceCycleCurrentWS.Cells[1, 1, 1, 14].Style.Font.Bold = true;
            InvoiceCycleCurrentWS.Cells[1, 1, 1, 14].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            InvoiceCycleCurrentWS.Column(1).Width = 15;
            InvoiceCycleCurrentWS.Column(2).Width = 25;
            InvoiceCycleCurrentWS.Column(3).Width = 25;
            InvoiceCycleCurrentWS.Column(4).Width = 25;
            InvoiceCycleCurrentWS.Column(5).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            InvoiceCycleCurrentWS.Column(5).Width = 25;
            InvoiceCycleCurrentWS.Column(6).Width = 25;
            InvoiceCycleCurrentWS.Column(6).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            InvoiceCycleCurrentWS.Column(7).Width = 20;
            InvoiceCycleCurrentWS.Column(8).Width = 15;
            InvoiceCycleCurrentWS.Column(9).Width = 25;
            InvoiceCycleCurrentWS.Column(10).Width = 25;
            InvoiceCycleCurrentWS.Column(10).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            InvoiceCycleCurrentWS.Column(11).Width = 25;
            InvoiceCycleCurrentWS.Column(12).Width = 25;
            InvoiceCycleCurrentWS.Column(13).Width = 25;
            InvoiceCycleCurrentWS.Column(14).Width = 15;

            InvoiceCycleCurrentWS.Cells[1, 1].Value = "Company ID";
            InvoiceCycleCurrentWS.Cells[1, 2].Value = "Company Name";
            InvoiceCycleCurrentWS.Cells[1, 3].Value = "Invoice Increment Days";
            InvoiceCycleCurrentWS.Cells[1, 4].Value = "Invoice Description";
            InvoiceCycleCurrentWS.Cells[1, 5].Value = "Current Period Ending";
            InvoiceCycleCurrentWS.Cells[1, 6].Value = "Created DateTime";
            InvoiceCycleCurrentWS.Cells[1, 7].Value = "Created Method";
            InvoiceCycleCurrentWS.Cells[1, 8].Value = "Created User";
            InvoiceCycleCurrentWS.Cells[1, 9].Value = "Created User Name";
            InvoiceCycleCurrentWS.Cells[1, 10].Value = "Last Amended DateTime";
            InvoiceCycleCurrentWS.Cells[1, 11].Value = "Last Amended Method";
            InvoiceCycleCurrentWS.Cells[1, 12].Value = "Last Amended User";
            InvoiceCycleCurrentWS.Cells[1, 13].Value = "Last Amended Name";
            InvoiceCycleCurrentWS.Cells[1, 14].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < InvoiceCycleCurrents.Count(); i++)
            {
                if (companies.Any(x => x.ID == InvoiceCycleCurrents[i].CompanyID))
                {
                    InvoiceCycleCurrentWS.Cells[rc, 1].Value = companies.FirstOrDefault(x => x.ID == InvoiceCycleCurrents[i].CompanyID).Abbreviation;
                    InvoiceCycleCurrentWS.Cells[rc, 2].Value = companies.FirstOrDefault(x => x.ID == InvoiceCycleCurrents[i].CompanyID).LongName;
                }
                else
                    InvoiceCycleCurrentWS.Cells[rc, 1].Value = "CompanyID: " + InvoiceCycleCurrents[i].CompanyID.ToString();

                if (invoicecycles.Any(x => x.ID == InvoiceCycleCurrents[i].InvoiceCycleID))
                {
                    InvoiceCycleCurrentWS.Cells[rc, 3].Value = invoicecycles.FirstOrDefault(x => x.ID == InvoiceCycleCurrents[i].InvoiceCycleID).IncrementDays;
                    InvoiceCycleCurrentWS.Cells[rc, 4].Value = invoicecycles.FirstOrDefault(x => x.ID == InvoiceCycleCurrents[i].InvoiceCycleID).Description;
                }
                else
                    InvoiceCycleCurrentWS.Cells[rc, 3].Value = "InvoiceCycleID: " + InvoiceCycleCurrents[i].InvoiceCycleID.ToString();

                InvoiceCycleCurrentWS.Cells[rc, 5].Value = InvoiceCycleCurrents[i].CurrentPeriodEnding;

                InvoiceCycleCurrentWS.Cells[rc, 6].Value = InvoiceCycleCurrents[i].CreatedDateTime.ToOADate();
                InvoiceCycleCurrentWS.Cells[rc, 7].Value = InvoiceCycleCurrents[i].CreatedMethod;

                if (users.Any(x => x.ID == InvoiceCycleCurrents[i].CreatedUserID))
                {
                    InvoiceCycleCurrentWS.Cells[rc, 8].Value = users.FirstOrDefault(x => x.ID == InvoiceCycleCurrents[i].CreatedUserID).Firstname;
                    InvoiceCycleCurrentWS.Cells[rc, 9].Value = users.FirstOrDefault(x => x.ID == InvoiceCycleCurrents[i].CreatedUserID).PublicID;
                }
                else
                    InvoiceCycleCurrentWS.Cells[rc, 8].Value = "UserID: " + InvoiceCycleCurrents[i].CreatedUserID.ToString();

                InvoiceCycleCurrentWS.Cells[rc, 10].Value = InvoiceCycleCurrents[i].LastAmendedDateTime.ToOADate();
                InvoiceCycleCurrentWS.Cells[rc, 11].Value = InvoiceCycleCurrents[i].LastAmendedMethod;

                if (users.Any(x => x.ID == InvoiceCycleCurrents[i].LastAmendedUserID))
                {
                    InvoiceCycleCurrentWS.Cells[rc, 12].Value = users.FirstOrDefault(x => x.ID == InvoiceCycleCurrents[i].LastAmendedUserID).Firstname;
                    InvoiceCycleCurrentWS.Cells[rc, 13].Value = users.FirstOrDefault(x => x.ID == InvoiceCycleCurrents[i].LastAmendedUserID).PublicID;
                }
                else
                    InvoiceCycleCurrentWS.Cells[rc, 12].Value = "UserID: " + InvoiceCycleCurrents[i].LastAmendedUserID.ToString();

                InvoiceCycleCurrentWS.Cells[rc, 14].Value = InvoiceCycleCurrents[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
