using Microsoft.AspNetCore.SignalR;
using NLog;
using NLog.Web;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using WBM_API.WBM_API_DB;
using wbm_common.DataObjects;
using wbm_common.NonDatabaseObjects;


namespace WBM_API.Helpers.Excel
{
    public class InvoiceReportGroupDefnExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Company.dbRowExtended> companies = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow> InvoiceReportGroupDefns = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.InvoiceReportGroupDefn.Search InvoiceReportGroupDefnSearchData;
        public InvoiceReportGroupDefnExcel(wbm_common.DataObjects.InvoiceReportGroupDefn.Search InvoiceReportGroupDefnSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.InvoiceReportGroupDefnSearchData = InvoiceReportGroupDefnSearchData;
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
                        InvoiceReportGroupDefnPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("InvoiceReportGroupDefnExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow>> rc = await InvoiceReportGroupDefnHelper.ExecuteSearch(InvoiceReportGroupDefnSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            InvoiceReportGroupDefns = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = InvoiceReportGroupDefns.Select(x => x.CreatedUserID).Union(InvoiceReportGroupDefns.Select(x => x.LastAmendedUserID)).Distinct().ToList();
            List<int> ListCompanyIDs = InvoiceReportGroupDefns.Select(x => x.CompanyID).Distinct().ToList();

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

        private void InvoiceReportGroupDefnPage(ExcelPackage ep)
        {
            ExcelWorksheet InvoiceReportGroupDefnWS = ep.Workbook.Worksheets.Add("InvoiceReportGroupDefn");

            #region header
            InvoiceReportGroupDefnWS.View.FreezePanes(2, 1);
            InvoiceReportGroupDefnWS.Cells[1, 1, 1, 14].Style.Font.Bold = true;
            InvoiceReportGroupDefnWS.Cells[1, 1, 1, 14].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            InvoiceReportGroupDefnWS.Column(1).Width = 15;
            InvoiceReportGroupDefnWS.Column(2).Width = 20;
            InvoiceReportGroupDefnWS.Column(3).Width = 10;
            InvoiceReportGroupDefnWS.Column(4).Width = 25;
            InvoiceReportGroupDefnWS.Column(5).Width = 10;
            InvoiceReportGroupDefnWS.Column(6).Width = 25;
            InvoiceReportGroupDefnWS.Column(6).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            InvoiceReportGroupDefnWS.Column(7).Width = 25;
            InvoiceReportGroupDefnWS.Column(8).Width = 25;
            InvoiceReportGroupDefnWS.Column(9).Width = 25;
            InvoiceReportGroupDefnWS.Column(10).Width = 25;
            InvoiceReportGroupDefnWS.Column(10).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            InvoiceReportGroupDefnWS.Column(11).Width = 25;
            InvoiceReportGroupDefnWS.Column(12).Width = 25;
            InvoiceReportGroupDefnWS.Column(13).Width = 25;
            InvoiceReportGroupDefnWS.Column(14).Width = 15;

            InvoiceReportGroupDefnWS.Cells[1, 1].Value = "Company";
            InvoiceReportGroupDefnWS.Cells[1, 2].Value = "Company Name";
            InvoiceReportGroupDefnWS.Cells[1, 3].Value = "ID";
            InvoiceReportGroupDefnWS.Cells[1, 4].Value = "Description";
            InvoiceReportGroupDefnWS.Cells[1, 5].Value = "Sort Order";
            InvoiceReportGroupDefnWS.Cells[1, 6].Value = "Created DateTime";
            InvoiceReportGroupDefnWS.Cells[1, 7].Value = "Created Method";
            InvoiceReportGroupDefnWS.Cells[1, 8].Value = "Created User";
            InvoiceReportGroupDefnWS.Cells[1, 9].Value = "Created User Name";
            InvoiceReportGroupDefnWS.Cells[1, 10].Value = "Last Amended DateTime";
            InvoiceReportGroupDefnWS.Cells[1, 11].Value = "Last Amended Method";
            InvoiceReportGroupDefnWS.Cells[1, 12].Value = "Last Amended User";
            InvoiceReportGroupDefnWS.Cells[1, 13].Value = "Last Amended Name";
            InvoiceReportGroupDefnWS.Cells[1, 14].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            InvoiceReportGroupDefns = InvoiceReportGroupDefns.OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < InvoiceReportGroupDefns.Count(); i++)
            {
                if (companies.Any(x => x.ID == InvoiceReportGroupDefns[i].CompanyID))
                {
                    InvoiceReportGroupDefnWS.Cells[rc, 1].Value = companies.FirstOrDefault(x => x.ID == InvoiceReportGroupDefns[i].CompanyID).ShortName;
                    InvoiceReportGroupDefnWS.Cells[rc, 2].Value = companies.FirstOrDefault(x => x.ID == InvoiceReportGroupDefns[i].CompanyID).LongName;
                }
                else
                    InvoiceReportGroupDefnWS.Cells[rc, 1].Value = "CompanyID: " + InvoiceReportGroupDefns[i].CompanyID.ToString();

                InvoiceReportGroupDefnWS.Cells[rc, 3].Value = InvoiceReportGroupDefns[i].PublicID;
                InvoiceReportGroupDefnWS.Cells[rc, 4].Value = InvoiceReportGroupDefns[i].Description;
                InvoiceReportGroupDefnWS.Cells[rc, 5].Value = InvoiceReportGroupDefns[i].SortOrder;

                InvoiceReportGroupDefnWS.Cells[rc, 6].Value = InvoiceReportGroupDefns[i].CreatedDateTime.ToOADate();
                InvoiceReportGroupDefnWS.Cells[rc, 7].Value = InvoiceReportGroupDefns[i].CreatedMethod;

                if (users.Any(x => x.ID == InvoiceReportGroupDefns[i].CreatedUserID))
                {
                    InvoiceReportGroupDefnWS.Cells[rc, 8].Value = users.FirstOrDefault(x => x.ID == InvoiceReportGroupDefns[i].CreatedUserID).PublicID;
                    InvoiceReportGroupDefnWS.Cells[rc, 9].Value = users.FirstOrDefault(x => x.ID == InvoiceReportGroupDefns[i].CreatedUserID).Firstname;
                }
                else
                    InvoiceReportGroupDefnWS.Cells[rc, 8].Value = "UserID: " + InvoiceReportGroupDefns[i].CreatedUserID.ToString();

                InvoiceReportGroupDefnWS.Cells[rc, 10].Value = InvoiceReportGroupDefns[i].LastAmendedDateTime.ToOADate();
                InvoiceReportGroupDefnWS.Cells[rc, 11].Value = InvoiceReportGroupDefns[i].LastAmendedMethod;

                if (users.Any(x => x.ID == InvoiceReportGroupDefns[i].LastAmendedUserID))
                {
                    InvoiceReportGroupDefnWS.Cells[rc, 12].Value = users.FirstOrDefault(x => x.ID == InvoiceReportGroupDefns[i].LastAmendedUserID).PublicID;
                    InvoiceReportGroupDefnWS.Cells[rc, 13].Value = users.FirstOrDefault(x => x.ID == InvoiceReportGroupDefns[i].LastAmendedUserID).Firstname;
                }
                else
                    InvoiceReportGroupDefnWS.Cells[rc, 12].Value = "UserID: " + InvoiceReportGroupDefns[i].LastAmendedUserID.ToString();

                InvoiceReportGroupDefnWS.Cells[rc, 14].Value = InvoiceReportGroupDefns[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
