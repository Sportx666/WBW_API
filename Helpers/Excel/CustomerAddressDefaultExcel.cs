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
    public class CustomerAddressDefaultExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Company.dbRow> companies = null;
        public List<wbm_common.DataObjects.Site.dbRow> sites = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.Owner.dbRow> owners = null;
        public List<wbm_common.DataObjects.CustomerAddressDefault.dbRow> CustomerAddressDefaults = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.CustomerAddressDefault.Search CustomerAddressDefaultSearchData;
        public CustomerAddressDefaultExcel(wbm_common.DataObjects.CustomerAddressDefault.Search CustomerAddressDefaultSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.CustomerAddressDefaultSearchData = CustomerAddressDefaultSearchData;
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
                        CustomerAddressDefaultPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("CustomerAddressDefaultExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.CustomerAddressDefault.dbRow>> rc = await CustomerAddressDefaultHelper.ExecuteSearch(CustomerAddressDefaultSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            CustomerAddressDefaults = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListOwnerID = CustomerAddressDefaults.Select(x => x.OwnerID).Distinct().ToList();
            List<int> ListCompanyIDs = CustomerAddressDefaults.Select(x => x.CompanyID).Distinct().ToList();
            List<int> ListSiteID = CustomerAddressDefaults.Select(x => x.SiteID).Distinct().ToList();
            List<int> ListUserID = CustomerAddressDefaults.Select(x => x.CreatedUserID).Union(CustomerAddressDefaults.Select(x => x.LastAmendedUserID)).Distinct().ToList();

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

            if (ListCompanyIDs.Count > 0)
            {
                Result<List<wbm_common.DataObjects.Company.dbRow>> companiesResults = _WBMDB.Company_ListByListIDs(ListCompanyIDs).GetAwaiter().GetResult();
                if (companiesResults.IsFailure || companiesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                companies = companiesResults.Value;
            }

            if (ListSiteID.Count > 0)
            {
                Result<List<wbm_common.DataObjects.Site.dbRow>> sitesResults = _WBMDB.Site_ListByListIDs(ListSiteID).GetAwaiter().GetResult();
                if (sitesResults.IsFailure || sitesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                sites = sitesResults.Value;
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

        private void CustomerAddressDefaultPage(ExcelPackage ep)
        {
            ExcelWorksheet CustomerAddressDefaultWS = ep.Workbook.Worksheets.Add("CustomerAddressDefault");

            #region header
            CustomerAddressDefaultWS.View.FreezePanes(2, 1);
            CustomerAddressDefaultWS.Cells[1, 1, 1, 20].Style.Font.Bold = true;
            CustomerAddressDefaultWS.Cells[1, 1, 1, 20].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            CustomerAddressDefaultWS.Column(1).Width = 12;
            CustomerAddressDefaultWS.Column(2).Width = 25;
            CustomerAddressDefaultWS.Column(3).Width = 12;
            CustomerAddressDefaultWS.Column(4).Width = 15;
            CustomerAddressDefaultWS.Column(5).Width = 10;
            CustomerAddressDefaultWS.Column(6).Width = 25;
            CustomerAddressDefaultWS.Column(7).Width = 10;
            CustomerAddressDefaultWS.Column(8).Width = 15;
            CustomerAddressDefaultWS.Column(9).Width = 25;
            CustomerAddressDefaultWS.Column(10).Width = 10;
            CustomerAddressDefaultWS.Column(11).Width = 25;
            CustomerAddressDefaultWS.Column(12).Width = 25;
            CustomerAddressDefaultWS.Column(12).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            CustomerAddressDefaultWS.Column(13).Width = 30;
            CustomerAddressDefaultWS.Column(14).Width = 25;
            CustomerAddressDefaultWS.Column(15).Width = 25;
            CustomerAddressDefaultWS.Column(16).Width = 25;
            CustomerAddressDefaultWS.Column(16).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            CustomerAddressDefaultWS.Column(17).Width = 25;
            CustomerAddressDefaultWS.Column(18).Width = 25;
            CustomerAddressDefaultWS.Column(19).Width = 25;
            CustomerAddressDefaultWS.Column(20).Width = 25;

            CustomerAddressDefaultWS.Cells[1, 1].Value = "ID";
            CustomerAddressDefaultWS.Cells[1, 2].Value = "Description";
            CustomerAddressDefaultWS.Cells[1, 3].Value = "Company";
            CustomerAddressDefaultWS.Cells[1, 4].Value = "Company Name";
            CustomerAddressDefaultWS.Cells[1, 5].Value = "Site";
            CustomerAddressDefaultWS.Cells[1, 6].Value = "Site Description";
            CustomerAddressDefaultWS.Cells[1, 7].Value = "Owner ID";
            CustomerAddressDefaultWS.Cells[1, 8].Value = "Owner Name";
            CustomerAddressDefaultWS.Cells[1, 9].Value = "Pallet Hire Dekays Days";
            CustomerAddressDefaultWS.Cells[1, 10].Value = "Invoiced Enclosed";
            CustomerAddressDefaultWS.Cells[1, 11].Value = "Sort Order";
            CustomerAddressDefaultWS.Cells[1, 12].Value = "Created DateTime";
            CustomerAddressDefaultWS.Cells[1, 13].Value = "Created Method";
            CustomerAddressDefaultWS.Cells[1, 14].Value = "Created User";
            CustomerAddressDefaultWS.Cells[1, 15].Value = "Created User Name";
            CustomerAddressDefaultWS.Cells[1, 16].Value = "Last Amended DateTime";
            CustomerAddressDefaultWS.Cells[1, 17].Value = "Last Amended Method";
            CustomerAddressDefaultWS.Cells[1, 18].Value = "Last Amended User";
            CustomerAddressDefaultWS.Cells[1, 19].Value = "Last Amended Name";
            CustomerAddressDefaultWS.Cells[1, 20].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < CustomerAddressDefaults.Count(); i++)
            {
                CustomerAddressDefaultWS.Cells[rc, 1].Value = CustomerAddressDefaults[i].PublicID;
                CustomerAddressDefaultWS.Cells[rc, 2].Value = CustomerAddressDefaults[i].Description;

                if (companies.Any(x => x.ID == CustomerAddressDefaults[i].CompanyID))
                {
                    CustomerAddressDefaultWS.Cells[rc, 3].Value = companies.FirstOrDefault(x => x.ID == CustomerAddressDefaults[i].CompanyID).ShortName;
                    CustomerAddressDefaultWS.Cells[rc, 4].Value = companies.FirstOrDefault(x => x.ID == CustomerAddressDefaults[i].CompanyID).LongName;
                }
                else
                    CustomerAddressDefaultWS.Cells[rc, 3].Value = "CompanyID: " + CustomerAddressDefaults[i].CompanyID.ToString();

                if (sites.Any(x => x.ID == CustomerAddressDefaults[i].SiteID))
                {
                    CustomerAddressDefaultWS.Cells[rc, 5].Value = sites.FirstOrDefault(x => x.ID == CustomerAddressDefaults[i].SiteID).PublicID;
                    CustomerAddressDefaultWS.Cells[rc, 6].Value = sites.FirstOrDefault(x => x.ID == CustomerAddressDefaults[i].SiteID).Description;
                }
                else
                    CustomerAddressDefaultWS.Cells[rc, 5].Value = "SiteID: " + CustomerAddressDefaults[i].SiteID.ToString();

                if (owners.Any(x => x.ID == CustomerAddressDefaults[i].OwnerID))
                {
                    CustomerAddressDefaultWS.Cells[rc, 7].Value = owners.FirstOrDefault(x => x.ID == CustomerAddressDefaults[i].OwnerID).SourceOwnerID;
                    CustomerAddressDefaultWS.Cells[rc, 8].Value = owners.FirstOrDefault(x => x.ID == CustomerAddressDefaults[i].OwnerID).Name;
                }
                else
                    CustomerAddressDefaultWS.Cells[rc, 7].Value = "OwnerID: " + CustomerAddressDefaults[i].OwnerID.ToString();

                CustomerAddressDefaultWS.Cells[rc, 9].Value = CustomerAddressDefaults[i].PalletHireDelayDays;
                CustomerAddressDefaultWS.Cells[rc, 10].Value = CustomerAddressDefaults[i].InvoiceEnclosed;
                CustomerAddressDefaultWS.Cells[rc, 11].Value = CustomerAddressDefaults[i].SortOrder;
                CustomerAddressDefaultWS.Cells[rc, 12].Value = CustomerAddressDefaults[i].CreatedDateTime.ToOADate();
                CustomerAddressDefaultWS.Cells[rc, 13].Value = CustomerAddressDefaults[i].CreatedMethod;

                if (users.Any(x => x.ID == CustomerAddressDefaults[i].CreatedUserID))
                {
                    CustomerAddressDefaultWS.Cells[rc, 14].Value = users.FirstOrDefault(x => x.ID == CustomerAddressDefaults[i].CreatedUserID).Firstname;
                    CustomerAddressDefaultWS.Cells[rc, 15].Value = users.FirstOrDefault(x => x.ID == CustomerAddressDefaults[i].CreatedUserID).PublicID;
                }
                else
                    CustomerAddressDefaultWS.Cells[rc, 14].Value = "UserID: " + CustomerAddressDefaults[i].CreatedUserID.ToString();

                CustomerAddressDefaultWS.Cells[rc, 16].Value = CustomerAddressDefaults[i].LastAmendedDateTime.ToOADate();
                CustomerAddressDefaultWS.Cells[rc, 17].Value = CustomerAddressDefaults[i].LastAmendedMethod;

                if (users.Any(x => x.ID == CustomerAddressDefaults[i].LastAmendedUserID))
                {
                    CustomerAddressDefaultWS.Cells[rc, 18].Value = users.FirstOrDefault(x => x.ID == CustomerAddressDefaults[i].LastAmendedUserID).Firstname;
                    CustomerAddressDefaultWS.Cells[rc, 19].Value = users.FirstOrDefault(x => x.ID == CustomerAddressDefaults[i].LastAmendedUserID).PublicID;
                }
                else
                    CustomerAddressDefaultWS.Cells[rc, 18].Value = "UserID: " + CustomerAddressDefaults[i].LastAmendedUserID.ToString();

                CustomerAddressDefaultWS.Cells[rc, 20].Value = CustomerAddressDefaults[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
