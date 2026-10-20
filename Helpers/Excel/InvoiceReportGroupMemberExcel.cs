using Microsoft.AspNetCore.SignalR;
using NLog;
using NLog.Web;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using wbm_common.NonDatabaseObjects;
using WBM_API.WBM_API_DB;


namespace WBM_API.Helpers.Excel
{
    public class InvoiceReportGroupMemberExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Company.dbRowExtended> companies = null;
        public List<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow> InvoiceReportGroupDefn = null;
        public List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended> InvoiceReportGroup = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended> InvoiceReportGroupMembers = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.InvoiceReportGroupMember.Search InvoiceReportGroupMemberSearchData;
        public InvoiceReportGroupMemberExcel(wbm_common.DataObjects.InvoiceReportGroupMember.Search InvoiceReportGroupMemberSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.InvoiceReportGroupMemberSearchData = InvoiceReportGroupMemberSearchData;
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
                        InvoiceReportGroupMemberPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("InvoiceReportGroupMemberExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.InvoiceReportGroupMember.dbRowExtended>> rc = await InvoiceReportGroupMemberHelper.ExecuteSearch(InvoiceReportGroupMemberSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            InvoiceReportGroupMembers = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = InvoiceReportGroupMembers.Select(x => x.CreatedUserID).Union(InvoiceReportGroupMembers.Select(x => x.LastAmendedUserID)).Distinct().ToList();
            List<int> ListInvoiceReportGroupDefnIDs = InvoiceReportGroupMembers.Select(x => x.InvoiceReportGroupDefnID).Distinct().ToList();
            List<int> ListInvoiceReportGroupIDs = InvoiceReportGroupMembers.Select(x => x.InvoiceReportGroupID).Distinct().ToList();
            List<int> ListCompanyIDs = InvoiceReportGroupMembers.Select(x => x.CompanyID).Distinct().ToList();

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
                Result<List<wbm_common.DataObjects.InvoiceReportGroupDefn.dbRow>> InvoiceReportGroupDefnResults = _WBMDB.InvoiceReportGroupDefn_ListByListIDs(ListInvoiceReportGroupDefnIDs).GetAwaiter().GetResult();
                if (InvoiceReportGroupDefnResults.IsFailure || InvoiceReportGroupDefnResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                InvoiceReportGroupDefn = InvoiceReportGroupDefnResults.Value;
            }

            if (ListInvoiceReportGroupIDs.Count > 0)
            {
                Result<List<wbm_common.DataObjects.InvoiceReportGroup.dbRowExtended>> InvoiceReportGroupResults = _WBMDB.InvoiceReportGroup_ListByListIDs(ListInvoiceReportGroupIDs).GetAwaiter().GetResult();
                if (InvoiceReportGroupResults.IsFailure || InvoiceReportGroupResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                InvoiceReportGroup = InvoiceReportGroupResults.Value;
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

        private void InvoiceReportGroupMemberPage(ExcelPackage ep)
        {
            ExcelWorksheet InvoiceReportGroupMemberWS = ep.Workbook.Worksheets.Add("InvoiceReportGroupMember");

            #region header
            InvoiceReportGroupMemberWS.View.FreezePanes(2, 1);
            InvoiceReportGroupMemberWS.Cells[1, 1, 1, 14].Style.Font.Bold = true;
            InvoiceReportGroupMemberWS.Cells[1, 1, 1, 14].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            InvoiceReportGroupMemberWS.Column(1).Width = 15;
            InvoiceReportGroupMemberWS.Column(2).Width = 20;
            InvoiceReportGroupMemberWS.Column(3).Width = 15;
            InvoiceReportGroupMemberWS.Column(4).Width = 25;
            InvoiceReportGroupMemberWS.Column(5).Width = 15;
            InvoiceReportGroupMemberWS.Column(6).Width = 25;
            InvoiceReportGroupMemberWS.Column(6).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            InvoiceReportGroupMemberWS.Column(7).Width = 25;
            InvoiceReportGroupMemberWS.Column(8).Width = 25;
            InvoiceReportGroupMemberWS.Column(9).Width = 25;
            InvoiceReportGroupMemberWS.Column(10).Width = 25;
            InvoiceReportGroupMemberWS.Column(10).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            InvoiceReportGroupMemberWS.Column(11).Width = 25;
            InvoiceReportGroupMemberWS.Column(12).Width = 25;
            InvoiceReportGroupMemberWS.Column(13).Width = 25;
            InvoiceReportGroupMemberWS.Column(14).Width = 25;

            InvoiceReportGroupMemberWS.Cells[1, 1].Value = "Company";
            InvoiceReportGroupMemberWS.Cells[1, 2].Value = "Company Name";
            InvoiceReportGroupMemberWS.Cells[1, 3].Value = "Invoice Report Group Defn ID";
            InvoiceReportGroupMemberWS.Cells[1, 4].Value = "Invoice Report Group ID";
            InvoiceReportGroupMemberWS.Cells[1, 5].Value = "Rate Collection Charge Code";
            InvoiceReportGroupMemberWS.Cells[1, 6].Value = "Created DateTime";
            InvoiceReportGroupMemberWS.Cells[1, 7].Value = "Created Method";
            InvoiceReportGroupMemberWS.Cells[1, 8].Value = "Created User";
            InvoiceReportGroupMemberWS.Cells[1, 9].Value = "Created User Name";
            InvoiceReportGroupMemberWS.Cells[1, 10].Value = "Last Amended DateTime";
            InvoiceReportGroupMemberWS.Cells[1, 11].Value = "Last Amended Method";
            InvoiceReportGroupMemberWS.Cells[1, 12].Value = "Last Amended User";
            InvoiceReportGroupMemberWS.Cells[1, 13].Value = "Last Amended Name";
            InvoiceReportGroupMemberWS.Cells[1, 14].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            InvoiceReportGroupMembers = InvoiceReportGroupMembers.ToList();
            for (int i = 0; i < InvoiceReportGroupMembers.Count(); i++)
            {
                if (companies.Any(x => x.ID == InvoiceReportGroupMembers[i].CompanyID))
                {
                    InvoiceReportGroupMemberWS.Cells[rc, 1].Value = companies.FirstOrDefault(x => x.ID == InvoiceReportGroupMembers[i].CompanyID).ShortName;
                    InvoiceReportGroupMemberWS.Cells[rc, 2].Value = companies.FirstOrDefault(x => x.ID == InvoiceReportGroupMembers[i].CompanyID).LongName;
                }
                else
                    InvoiceReportGroupMemberWS.Cells[rc, 1].Value = "CompanyID: " + InvoiceReportGroupMembers[i].CompanyID.ToString();

                if (InvoiceReportGroupDefn.Any(x => x.ID == InvoiceReportGroupMembers[i].InvoiceReportGroupDefnID))
                {
                    InvoiceReportGroupMemberWS.Cells[rc, 3].Value = InvoiceReportGroupDefn.FirstOrDefault(x => x.ID == InvoiceReportGroupMembers[i].InvoiceReportGroupDefnID).PublicID;
                }
                else
                    InvoiceReportGroupMemberWS.Cells[rc, 3].Value = "InvoiceReportGroupDefnID: " + InvoiceReportGroupMembers[i].InvoiceReportGroupDefnID.ToString();

                if (InvoiceReportGroup.Any(x => x.ID == InvoiceReportGroupMembers[i].InvoiceReportGroupID))
                {
                    InvoiceReportGroupMemberWS.Cells[rc, 4].Value = InvoiceReportGroup.FirstOrDefault(x => x.ID == InvoiceReportGroupMembers[i].InvoiceReportGroupID).PublicID;
                }
                else
                    InvoiceReportGroupMemberWS.Cells[rc, 4].Value = "InvoiceReportGroupID: " + InvoiceReportGroupMembers[i].InvoiceReportGroupID.ToString();

                InvoiceReportGroupMemberWS.Cells[rc, 5].Value = InvoiceReportGroupMembers[i].RateCollectionChargeCode;

                InvoiceReportGroupMemberWS.Cells[rc, 6].Value = InvoiceReportGroupMembers[i].CreatedDateTime.ToOADate();
                InvoiceReportGroupMemberWS.Cells[rc, 7].Value = InvoiceReportGroupMembers[i].CreatedMethod;

                if (users.Any(x => x.ID == InvoiceReportGroupMembers[i].CreatedUserID))
                {
                    InvoiceReportGroupMemberWS.Cells[rc, 8].Value = users.FirstOrDefault(x => x.ID == InvoiceReportGroupMembers[i].CreatedUserID).PublicID;
                    InvoiceReportGroupMemberWS.Cells[rc, 9].Value = users.FirstOrDefault(x => x.ID == InvoiceReportGroupMembers[i].CreatedUserID).Firstname;
                }
                else
                    InvoiceReportGroupMemberWS.Cells[rc, 8].Value = "UserID: " + InvoiceReportGroupMembers[i].CreatedUserID.ToString();

                InvoiceReportGroupMemberWS.Cells[rc, 10].Value = InvoiceReportGroupMembers[i].LastAmendedDateTime.ToOADate();
                InvoiceReportGroupMemberWS.Cells[rc, 11].Value = InvoiceReportGroupMembers[i].LastAmendedMethod;

                if (users.Any(x => x.ID == InvoiceReportGroupMembers[i].LastAmendedUserID))
                {
                    InvoiceReportGroupMemberWS.Cells[rc, 12].Value = users.FirstOrDefault(x => x.ID == InvoiceReportGroupMembers[i].LastAmendedUserID).PublicID;
                    InvoiceReportGroupMemberWS.Cells[rc, 13].Value = users.FirstOrDefault(x => x.ID == InvoiceReportGroupMembers[i].LastAmendedUserID).Firstname;
                }
                else
                    InvoiceReportGroupMemberWS.Cells[rc, 12].Value = "UserID: " + InvoiceReportGroupMembers[i].LastAmendedUserID.ToString();

                InvoiceReportGroupMemberWS.Cells[rc, 14].Value = InvoiceReportGroupMembers[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
