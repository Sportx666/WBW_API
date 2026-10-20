using Microsoft.AspNetCore.SignalR;
using NLog;
using NLog.Web;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using wbm_common.NonDatabaseObjects;
using WBM_API.WBM_API_DB;


namespace WBM_API.Helpers.Excel
{
    public class InvoiceReportGroupExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Company.dbRowExtended> companies = null;
        public List<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow> invoiceReportGroupDefn = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended> InvoiceReportGroups = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.InvoiceReportGroup.Search InvoiceReportGroupSearchData;
        public InvoiceReportGroupExcel(wbm_common.DataObjects.InvoiceReportGroup.Search InvoiceReportGroupSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.InvoiceReportGroupSearchData = InvoiceReportGroupSearchData;
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
                        InvoiceReportGroupPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("InvoiceReportGroupExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>> rc = await InvoiceReportGroupHelper.ExecuteSearch(InvoiceReportGroupSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            InvoiceReportGroups = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = InvoiceReportGroups.Select(x => x.CreatedUserID).Union(InvoiceReportGroups.Select(x => x.LastAmendedUserID)).Distinct().ToList();
            List<int> ListInvoiceReportGroupDefnIDs = InvoiceReportGroups.Select(x => x.InvoiceReportGroupDefnID).Distinct().ToList();
            List<int> ListCompanyIDs = InvoiceReportGroups.Select(x => x.CompanyID).Distinct().ToList();

            if (ListCompanyIDs.Count > 0)
            {
                Result<List<wbm_common.DataObjects.Company.dbRowExtended>> companiesResults = _WBMDB.Company_ListByListIDs(ListCompanyIDs).GetAwaiter().GetResult();
                if (companiesResults.IsFailure || companiesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                companies = companiesResults.Value;
            }

            if (ListInvoiceReportGroupDefnIDs.Count > 0)
            {
                Result<List<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow>> invoiceReportGroupDefnResults = _WBMDB.InvoiceReportGroupDefn_ListByListIDs(ListInvoiceReportGroupDefnIDs).GetAwaiter().GetResult();
                if (invoiceReportGroupDefnResults.IsFailure || invoiceReportGroupDefnResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                invoiceReportGroupDefn = invoiceReportGroupDefnResults.Value;
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

        private void InvoiceReportGroupPage(ExcelPackage ep)
        {
            ExcelWorksheet InvoiceReportGroupWS = ep.Workbook.Worksheets.Add("InvoiceReportGroup");

            #region header
            InvoiceReportGroupWS.View.FreezePanes(2, 1);
            InvoiceReportGroupWS.Cells[1, 1, 1, 15].Style.Font.Bold = true;
            InvoiceReportGroupWS.Cells[1, 1, 1, 15].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            InvoiceReportGroupWS.Column(1).Width = 15;
            InvoiceReportGroupWS.Column(2).Width = 20;
            InvoiceReportGroupWS.Column(3).Width = 15;
            InvoiceReportGroupWS.Column(4).Width = 25;
            InvoiceReportGroupWS.Column(5).Width = 15;
            InvoiceReportGroupWS.Column(6).Width = 25;
            InvoiceReportGroupWS.Column(7).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            InvoiceReportGroupWS.Column(7).Width = 25;
            InvoiceReportGroupWS.Column(8).Width = 25;
            InvoiceReportGroupWS.Column(9).Width = 25;
            InvoiceReportGroupWS.Column(10).Width = 25;
            InvoiceReportGroupWS.Column(11).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            InvoiceReportGroupWS.Column(11).Width = 25;
            InvoiceReportGroupWS.Column(12).Width = 25;
            InvoiceReportGroupWS.Column(13).Width = 25;
            InvoiceReportGroupWS.Column(14).Width = 25;
            InvoiceReportGroupWS.Column(15).Width = 15;

            InvoiceReportGroupWS.Cells[1, 1].Value = "Company";
            InvoiceReportGroupWS.Cells[1, 2].Value = "Company Name";
            InvoiceReportGroupWS.Cells[1, 3].Value = "ID";
            InvoiceReportGroupWS.Cells[1, 4].Value = "Invoice Report Group Defn ID";
            InvoiceReportGroupWS.Cells[1, 5].Value = "Group Name";
            InvoiceReportGroupWS.Cells[1, 6].Value = "Sort Order";
            InvoiceReportGroupWS.Cells[1, 7].Value = "Created DateTime";
            InvoiceReportGroupWS.Cells[1, 8].Value = "Created Method";
            InvoiceReportGroupWS.Cells[1, 9].Value = "Created User";
            InvoiceReportGroupWS.Cells[1, 10].Value = "Created User Name";
            InvoiceReportGroupWS.Cells[1, 11].Value = "Last Amended DateTime";
            InvoiceReportGroupWS.Cells[1, 12].Value = "Last Amended Method";
            InvoiceReportGroupWS.Cells[1, 13].Value = "Last Amended User";
            InvoiceReportGroupWS.Cells[1, 14].Value = "Last Amended Name";
            InvoiceReportGroupWS.Cells[1, 15].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            InvoiceReportGroups = InvoiceReportGroups.OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < InvoiceReportGroups.Count(); i++)
            {
                if (companies.Any(x => x.ID == InvoiceReportGroups[i].CompanyID))
                {
                    InvoiceReportGroupWS.Cells[rc, 1].Value = companies.FirstOrDefault(x => x.ID == InvoiceReportGroups[i].CompanyID).ShortName;
                    InvoiceReportGroupWS.Cells[rc, 2].Value = companies.FirstOrDefault(x => x.ID == InvoiceReportGroups[i].CompanyID).LongName;
                }
                else
                    InvoiceReportGroupWS.Cells[rc, 1].Value = "CompanyID: " + InvoiceReportGroups[i].CompanyID.ToString();

                InvoiceReportGroupWS.Cells[rc, 3].Value = InvoiceReportGroups[i].PublicID;

                if (invoiceReportGroupDefn.Any(x => x.ID == InvoiceReportGroups[i].InvoiceReportGroupDefnID))
                {
                    InvoiceReportGroupWS.Cells[rc, 4].Value = invoiceReportGroupDefn.FirstOrDefault(x => x.ID == InvoiceReportGroups[i].InvoiceReportGroupDefnID).PublicID;
                }
                else
                    InvoiceReportGroupWS.Cells[rc, 4].Value = "InvoiceReportGroupDefn: " + InvoiceReportGroups[i].InvoiceReportGroupDefnID.ToString();

                InvoiceReportGroupWS.Cells[rc, 5].Value = InvoiceReportGroups[i].GroupName;
                InvoiceReportGroupWS.Cells[rc, 6].Value = InvoiceReportGroups[i].SortOrder;

                InvoiceReportGroupWS.Cells[rc, 7].Value = InvoiceReportGroups[i].CreatedDateTime.ToOADate();
                InvoiceReportGroupWS.Cells[rc, 8].Value = InvoiceReportGroups[i].CreatedMethod;

                if (users.Any(x => x.ID == InvoiceReportGroups[i].CreatedUserID))
                {
                    InvoiceReportGroupWS.Cells[rc, 9].Value = users.FirstOrDefault(x => x.ID == InvoiceReportGroups[i].CreatedUserID).PublicID;
                    InvoiceReportGroupWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == InvoiceReportGroups[i].CreatedUserID).Firstname;
                }
                else
                    InvoiceReportGroupWS.Cells[rc, 9].Value = "UserID: " + InvoiceReportGroups[i].CreatedUserID.ToString();

                InvoiceReportGroupWS.Cells[rc, 11].Value = InvoiceReportGroups[i].LastAmendedDateTime.ToOADate();
                InvoiceReportGroupWS.Cells[rc, 12].Value = InvoiceReportGroups[i].LastAmendedMethod;

                if (users.Any(x => x.ID == InvoiceReportGroups[i].LastAmendedUserID))
                {
                    InvoiceReportGroupWS.Cells[rc, 13].Value = users.FirstOrDefault(x => x.ID == InvoiceReportGroups[i].LastAmendedUserID).PublicID;
                    InvoiceReportGroupWS.Cells[rc, 14].Value = users.FirstOrDefault(x => x.ID == InvoiceReportGroups[i].LastAmendedUserID).Firstname;
                }
                else
                    InvoiceReportGroupWS.Cells[rc, 13].Value = "UserID: " + InvoiceReportGroups[i].LastAmendedUserID.ToString();

                InvoiceReportGroupWS.Cells[rc, 15].Value = InvoiceReportGroups[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
