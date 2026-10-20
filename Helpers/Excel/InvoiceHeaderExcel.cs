using Microsoft.AspNetCore.SignalR;
using NLog;
using NLog.Web;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using wbm_common.NonDatabaseObjects;
using WBM_API.WBM_API_DB;


namespace WBM_API.Helpers.Excel
{
    public class InvoiceHeaderExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Company.dbRowExtended> companies = null;
        public List<wbm_common.DataObjects.Site.dbRowExtended> sites = null;
        public List<wbm_common.DataObjects.Owner.dbRow> owners = null;
        public List<wbm_common.DataObjects.InvoiceCycle.dbRow> invoicecycles = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended> InvoiceHeaders = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.InvoiceHeader.Search InvoiceHeaderSearchData;
        public InvoiceHeaderExcel(wbm_common.DataObjects.InvoiceHeader.Search InvoiceHeaderSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.InvoiceHeaderSearchData = InvoiceHeaderSearchData;
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
                        InvoiceHeaderPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("InvoiceHeaderExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended>> rc = await InvoiceHeaderHelper.ExecuteSearch(InvoiceHeaderSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            InvoiceHeaders = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListOwnerID = InvoiceHeaders.Select(x => x.OwnerID).Distinct().ToList();
            List<int> ListSiteID = InvoiceHeaders.Select(x => x.SiteID).Distinct().ToList();
            List<int> ListUserID = InvoiceHeaders.Select(x => x.CreatedUserID).Union(InvoiceHeaders.Select(x => x.LastAmendedUserID)).Distinct().ToList();
            List<int> ListCompanyID = InvoiceHeaders.Select(x => x.CompanyID).Distinct().ToList();
            List<int> ListInvoiceCycleID = InvoiceHeaders.Select(x => x.InvoiceCycleID).Distinct().ToList();

            if (ListOwnerID.Count > 0)
            {
                Result<List<wbm_common.DataObjects.Owner.dbRow>> ownersResults = _WBMDB.Owner_ListByListIDs(ListOwnerID).GetAwaiter().GetResult();
                if (ownersResults.IsFailure || ownersResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                owners = ownersResults.Value;
            }

            if (ListSiteID.Count > 0)
            {
                Result<List<wbm_common.DataObjects.Site.dbRowExtended>> sitesResults = _WBMDB.Site_ListByListIDs(ListSiteID).GetAwaiter().GetResult();
                if (sitesResults.IsFailure || sitesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                sites = sitesResults.Value;
            }

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
                Result<List<wbm_common.DataObjects.Company.dbRowExtended>> companiesResults = _WBMDB.Company_ListByListIDs(ListCompanyID).GetAwaiter().GetResult();
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

        private void InvoiceHeaderPage(ExcelPackage ep)
        {
            ExcelWorksheet InvoiceHeaderWS = ep.Workbook.Worksheets.Add("InvoiceHeader");

            #region header
            InvoiceHeaderWS.View.FreezePanes(2, 1);
            InvoiceHeaderWS.Cells[1, 1, 1, 25].Style.Font.Bold = true;
            InvoiceHeaderWS.Cells[1, 1, 1, 25].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            InvoiceHeaderWS.Column(1).Width = 15;
            InvoiceHeaderWS.Column(2).Width = 25;
            InvoiceHeaderWS.Column(3).Width = 25;
            InvoiceHeaderWS.Column(3).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            InvoiceHeaderWS.Column(4).Width = 25;
            InvoiceHeaderWS.Column(5).Width = 25;
            InvoiceHeaderWS.Column(6).Width = 25;
            InvoiceHeaderWS.Column(7).Width = 25;
            InvoiceHeaderWS.Column(8).Width = 25;
            InvoiceHeaderWS.Column(9).Width = 25;
            InvoiceHeaderWS.Column(10).Width = 25;
            InvoiceHeaderWS.Column(11).Width = 25;
            InvoiceHeaderWS.Column(12).Width = 25;
            InvoiceHeaderWS.Column(13).Width = 25;
            InvoiceHeaderWS.Column(14).Width = 25;
            InvoiceHeaderWS.Column(15).Width = 25;
            InvoiceHeaderWS.Column(16).Width = 25;
            InvoiceHeaderWS.Column(16).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            InvoiceHeaderWS.Column(17).Width = 25;
            InvoiceHeaderWS.Column(17).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            InvoiceHeaderWS.Column(18).Width = 25;
            InvoiceHeaderWS.Column(19).Width = 25;
            InvoiceHeaderWS.Column(20).Width = 25;
            InvoiceHeaderWS.Column(21).Width = 25;
            InvoiceHeaderWS.Column(21).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            InvoiceHeaderWS.Column(22).Width = 25;
            InvoiceHeaderWS.Column(23).Width = 25;
            InvoiceHeaderWS.Column(24).Width = 25;
            InvoiceHeaderWS.Column(25).Width = 15;

            InvoiceHeaderWS.Cells[1, 1].Value = "Invoice Number";
            InvoiceHeaderWS.Cells[1, 2].Value = "Invoice Prefix";
            InvoiceHeaderWS.Cells[1, 3].Value = "Invoice Date";
            InvoiceHeaderWS.Cells[1, 4].Value = "Increment Days";
            InvoiceHeaderWS.Cells[1, 5].Value = "Description";
            InvoiceHeaderWS.Cells[1, 6].Value = "Company Short Name";
            InvoiceHeaderWS.Cells[1, 7].Value = "Company Long Name";
            InvoiceHeaderWS.Cells[1, 8].Value = "Source Owner ID";
            InvoiceHeaderWS.Cells[1, 9].Value = "Owner Name";
            InvoiceHeaderWS.Cells[1, 10].Value = "Site Public ID";
            InvoiceHeaderWS.Cells[1, 11].Value = "Site Description";
            InvoiceHeaderWS.Cells[1, 12].Value = "Client ExGST Amount";
            InvoiceHeaderWS.Cells[1, 13].Value = "Client GST Amount";
            InvoiceHeaderWS.Cells[1, 14].Value = "Client Total Amount";
            InvoiceHeaderWS.Cells[1, 15].Value = "Invoice Format ID";
            InvoiceHeaderWS.Cells[1, 16].Value = "First Export DateTime";
            InvoiceHeaderWS.Cells[1, 17].Value = "Created DateTime";
            InvoiceHeaderWS.Cells[1, 18].Value = "Created Method";
            InvoiceHeaderWS.Cells[1, 19].Value = "Created User";
            InvoiceHeaderWS.Cells[1, 20].Value = "Created User Name";
            InvoiceHeaderWS.Cells[1, 21].Value = "Last Amended DateTime";
            InvoiceHeaderWS.Cells[1, 22].Value = "Last Amended Method";
            InvoiceHeaderWS.Cells[1, 23].Value = "Last Amended User";
            InvoiceHeaderWS.Cells[1, 24].Value = "Last Amended Name";
            InvoiceHeaderWS.Cells[1, 25].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < InvoiceHeaders.Count(); i++)
            {
                InvoiceHeaderWS.Cells[rc, 1].Value = InvoiceHeaders[i].InvoiceNumber;
                InvoiceHeaderWS.Cells[rc, 2].Value = InvoiceHeaders[i].InvoicePrefix;
                InvoiceHeaderWS.Cells[rc, 3].Value = InvoiceHeaders[i].InvoiceDate.ToOADate();


                if (invoicecycles.Any(x => x.ID == InvoiceHeaders[i].InvoiceCycleID))
                {
                    InvoiceHeaderWS.Cells[rc, 4].Value = invoicecycles.FirstOrDefault(x => x.ID == InvoiceHeaders[i].InvoiceCycleID).IncrementDays;
                    InvoiceHeaderWS.Cells[rc, 5].Value = invoicecycles.FirstOrDefault(x => x.ID == InvoiceHeaders[i].InvoiceCycleID).Description;
                }
                else
                    InvoiceHeaderWS.Cells[rc, 4].Value = "InvoiceCycleID: " + InvoiceHeaders[i].InvoiceCycleID.ToString();

                if (companies.Any(x => x.ID == InvoiceHeaders[i].CompanyID))
                {
                    InvoiceHeaderWS.Cells[rc, 6].Value = companies.FirstOrDefault(x => x.ID == InvoiceHeaders[i].CompanyID).Abbreviation;
                    InvoiceHeaderWS.Cells[rc, 7].Value = companies.FirstOrDefault(x => x.ID == InvoiceHeaders[i].CompanyID).LongName;
                }
                else
                    InvoiceHeaderWS.Cells[rc, 6].Value = "CompanyID: " + InvoiceHeaders[i].CompanyID.ToString();


                if (owners.Any(x => x.ID == InvoiceHeaders[i].OwnerID))
                {
                    InvoiceHeaderWS.Cells[rc, 8].Value = owners.FirstOrDefault(x => x.ID == InvoiceHeaders[i].OwnerID).SourceOwnerID;
                    InvoiceHeaderWS.Cells[rc, 9].Value = owners.FirstOrDefault(x => x.ID == InvoiceHeaders[i].OwnerID).Name;
                }
                else
                    InvoiceHeaderWS.Cells[rc, 8].Value = "OwnerID: " + InvoiceHeaders[i].OwnerID.ToString();

                if (sites.Any(x => x.ID == InvoiceHeaders[i].SiteID))
                {
                    InvoiceHeaderWS.Cells[rc, 10].Value = sites.FirstOrDefault(x => x.ID == InvoiceHeaders[i].SiteID).PublicID;
                    InvoiceHeaderWS.Cells[rc, 11].Value = sites.FirstOrDefault(x => x.ID == InvoiceHeaders[i].SiteID).Description;
                }
                else
                    InvoiceHeaderWS.Cells[rc, 10].Value = "SiteID: " + InvoiceHeaders[i].SiteID.ToString();

                InvoiceHeaderWS.Cells[rc, 12].Value = InvoiceHeaders[i].ClientExGSTAmount;
                InvoiceHeaderWS.Cells[rc, 13].Value = InvoiceHeaders[i].ClientGSTAmount;
                InvoiceHeaderWS.Cells[rc, 14].Value = InvoiceHeaders[i].ClientTotalAmount;
                InvoiceHeaderWS.Cells[rc, 15].Value = InvoiceHeaders[i].InvoiceFormatID;
                if (InvoiceHeaders[i].FirstExportDateTime != null)
                    InvoiceHeaderWS.Cells[rc, 16].Value = InvoiceHeaders[i].FirstExportDateTime.Value.ToOADate();

                InvoiceHeaderWS.Cells[rc, 17].Value = InvoiceHeaders[i].CreatedDateTime.ToOADate();
                InvoiceHeaderWS.Cells[rc, 18].Value = InvoiceHeaders[i].CreatedMethod;

                if (users.Any(x => x.ID == InvoiceHeaders[i].CreatedUserID))
                {
                    InvoiceHeaderWS.Cells[rc, 19].Value = users.FirstOrDefault(x => x.ID == InvoiceHeaders[i].CreatedUserID).Firstname;
                    InvoiceHeaderWS.Cells[rc, 20].Value = users.FirstOrDefault(x => x.ID == InvoiceHeaders[i].CreatedUserID).PublicID;
                }
                else
                    InvoiceHeaderWS.Cells[rc, 19].Value = "UserID: " + InvoiceHeaders[i].CreatedUserID.ToString();

                InvoiceHeaderWS.Cells[rc, 21].Value = InvoiceHeaders[i].LastAmendedDateTime.ToOADate();
                InvoiceHeaderWS.Cells[rc, 22].Value = InvoiceHeaders[i].LastAmendedMethod;

                if (users.Any(x => x.ID == InvoiceHeaders[i].LastAmendedUserID))
                {
                    InvoiceHeaderWS.Cells[rc, 23].Value = users.FirstOrDefault(x => x.ID == InvoiceHeaders[i].LastAmendedUserID).Firstname;
                    InvoiceHeaderWS.Cells[rc, 24].Value = users.FirstOrDefault(x => x.ID == InvoiceHeaders[i].LastAmendedUserID).PublicID;
                }
                else
                    InvoiceHeaderWS.Cells[rc, 23].Value = "UserID: " + InvoiceHeaders[i].LastAmendedUserID.ToString();

                InvoiceHeaderWS.Cells[rc, 25].Value = InvoiceHeaders[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
