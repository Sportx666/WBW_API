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
    public class CustomerAddressExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Company.dbRow> companies = null;
        public List<wbm_common.DataObjects.Site.dbRow> sites = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.Owner.dbRow> owners = null;
        public List<wbm_common.DataObjects.CustomerAddress.dbRow> CustomerAddresss = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.CustomerAddress.Search CustomerAddressSearchData;
        public CustomerAddressExcel(wbm_common.DataObjects.CustomerAddress.Search CustomerAddressSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.CustomerAddressSearchData = CustomerAddressSearchData;
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
                        CustomerAddressPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("CustomerAddressExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.CustomerAddress.dbRow>> rc = await CustomerAddressHelper.ExecuteSearch(CustomerAddressSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            CustomerAddresss = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListOwnerID = CustomerAddresss.Select(x => x.OwnerID).Distinct().ToList();
            List<int> ListCompanyIDs = CustomerAddresss.Select(x => x.CompanyID).Distinct().ToList();
            List<int> ListSiteID = CustomerAddresss.Select(x => x.SiteID).Distinct().ToList();
            List<int> ListUserID = CustomerAddresss.Select(x => x.CreatedUserID).Union(CustomerAddresss.Select(x => x.LastAmendedUserID)).Distinct().ToList();

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

        private void CustomerAddressPage(ExcelPackage ep)
        {
            ExcelWorksheet CustomerAddressWS = ep.Workbook.Worksheets.Add("CustomerAddress");

            #region header
            CustomerAddressWS.View.FreezePanes(2, 1);
            CustomerAddressWS.Cells[1, 1, 1, 25].Style.Font.Bold = true;
            CustomerAddressWS.Cells[1, 1, 1, 25].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            CustomerAddressWS.Column(1).Width = 12;
            CustomerAddressWS.Column(2).Width = 25;
            CustomerAddressWS.Column(3).Width = 12;
            CustomerAddressWS.Column(4).Width = 15;
            CustomerAddressWS.Column(5).Width = 10;
            CustomerAddressWS.Column(6).Width = 25;
            CustomerAddressWS.Column(7).Width = 10;
            CustomerAddressWS.Column(8).Width = 15;
            CustomerAddressWS.Column(9).Width = 25;
            CustomerAddressWS.Column(10).Width = 10;
            CustomerAddressWS.Column(11).Width = 25;
            CustomerAddressWS.Column(12).Width = 25;
            CustomerAddressWS.Column(13).Width = 30;
            CustomerAddressWS.Column(14).Width = 15;
            CustomerAddressWS.Column(15).Width = 15;
            CustomerAddressWS.Column(16).Width = 15;
            CustomerAddressWS.Column(17).Width = 25;
            CustomerAddressWS.Column(17).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            CustomerAddressWS.Column(18).Width = 25;
            CustomerAddressWS.Column(19).Width = 25;
            CustomerAddressWS.Column(20).Width = 25;
            CustomerAddressWS.Column(21).Width = 25;
            CustomerAddressWS.Column(21).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            CustomerAddressWS.Column(22).Width = 25;
            CustomerAddressWS.Column(23).Width = 25;
            CustomerAddressWS.Column(24).Width = 25;
            CustomerAddressWS.Column(25).Width = 25;

            CustomerAddressWS.Cells[1, 1].Value = "Company";
            CustomerAddressWS.Cells[1, 2].Value = "Company Name";
            CustomerAddressWS.Cells[1, 3].Value = "Owner ID";
            CustomerAddressWS.Cells[1, 4].Value = "Owner Name";
            CustomerAddressWS.Cells[1, 5].Value = "Site";
            CustomerAddressWS.Cells[1, 6].Value = "Site Description";
            CustomerAddressWS.Cells[1, 7].Value = "Name";
            CustomerAddressWS.Cells[1, 8].Value = "Address Line 1";
            CustomerAddressWS.Cells[1, 9].Value = "Address Line 2";
            CustomerAddressWS.Cells[1, 10].Value = "Address Line 3";
            CustomerAddressWS.Cells[1, 11].Value = "Address Line 4";
            CustomerAddressWS.Cells[1, 12].Value = "Suburb";
            CustomerAddressWS.Cells[1, 13].Value = "State";
            CustomerAddressWS.Cells[1, 14].Value = "Postcode";
            CustomerAddressWS.Cells[1, 15].Value = "Pallet Hire Delay Days";
            CustomerAddressWS.Cells[1, 16].Value = "Invoiced Enclosed";
            CustomerAddressWS.Cells[1, 17].Value = "Created DateTime";
            CustomerAddressWS.Cells[1, 18].Value = "Created Method";
            CustomerAddressWS.Cells[1, 19].Value = "Created User";
            CustomerAddressWS.Cells[1, 20].Value = "Created User Name";
            CustomerAddressWS.Cells[1, 21].Value = "Last Amended DateTime";
            CustomerAddressWS.Cells[1, 22].Value = "Last Amended Method";
            CustomerAddressWS.Cells[1, 23].Value = "Last Amended User";
            CustomerAddressWS.Cells[1, 24].Value = "Last Amended Name";
            CustomerAddressWS.Cells[1, 25].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < CustomerAddresss.Count(); i++)
            {
                if (companies.Any(x => x.ID == CustomerAddresss[i].CompanyID))
                {
                    CustomerAddressWS.Cells[rc, 1].Value = companies.FirstOrDefault(x => x.ID == CustomerAddresss[i].CompanyID).ShortName;
                    CustomerAddressWS.Cells[rc, 2].Value = companies.FirstOrDefault(x => x.ID == CustomerAddresss[i].CompanyID).LongName;
                }
                else
                    CustomerAddressWS.Cells[rc, 1].Value = "CompanyID: " + CustomerAddresss[i].CompanyID.ToString();

                if (owners.Any(x => x.ID == CustomerAddresss[i].OwnerID))
                {
                    CustomerAddressWS.Cells[rc, 3].Value = owners.FirstOrDefault(x => x.ID == CustomerAddresss[i].OwnerID).SourceOwnerID;
                    CustomerAddressWS.Cells[rc, 4].Value = owners.FirstOrDefault(x => x.ID == CustomerAddresss[i].OwnerID).Name;
                }
                else
                    CustomerAddressWS.Cells[rc, 3].Value = "OwnerID: " + CustomerAddresss[i].OwnerID.ToString();

                if (sites.Any(x => x.ID == CustomerAddresss[i].SiteID))
                {
                    CustomerAddressWS.Cells[rc, 5].Value = sites.FirstOrDefault(x => x.ID == CustomerAddresss[i].SiteID).PublicID;
                    CustomerAddressWS.Cells[rc, 6].Value = sites.FirstOrDefault(x => x.ID == CustomerAddresss[i].SiteID).Description;
                }
                else
                    CustomerAddressWS.Cells[rc, 5].Value = "SiteID: " + CustomerAddresss[i].SiteID.ToString();

                CustomerAddressWS.Cells[rc, 7].Value = CustomerAddresss[i].Name;
                CustomerAddressWS.Cells[rc, 8].Value = CustomerAddresss[i].AddressLine1;
                CustomerAddressWS.Cells[rc, 9].Value = CustomerAddresss[i].AddressLine2;
                CustomerAddressWS.Cells[rc, 10].Value = CustomerAddresss[i].AddressLine3;
                CustomerAddressWS.Cells[rc, 11].Value = CustomerAddresss[i].AddressLine4;
                CustomerAddressWS.Cells[rc, 12].Value = CustomerAddresss[i].Suburb;
                CustomerAddressWS.Cells[rc, 13].Value = CustomerAddresss[i].State;
                CustomerAddressWS.Cells[rc, 14].Value = CustomerAddresss[i].PostCode;
                CustomerAddressWS.Cells[rc, 15].Value = CustomerAddresss[i].PalletHireDelayDays;
                CustomerAddressWS.Cells[rc, 16].Value = CustomerAddresss[i].InvoiceEnclosed;
                CustomerAddressWS.Cells[rc, 17].Value = CustomerAddresss[i].CreatedDateTime.ToOADate();
                CustomerAddressWS.Cells[rc, 18].Value = CustomerAddresss[i].CreatedMethod;

                if (users.Any(x => x.ID == CustomerAddresss[i].CreatedUserID))
                {
                    CustomerAddressWS.Cells[rc, 19].Value = users.FirstOrDefault(x => x.ID == CustomerAddresss[i].CreatedUserID).Firstname;
                    CustomerAddressWS.Cells[rc, 20].Value = users.FirstOrDefault(x => x.ID == CustomerAddresss[i].CreatedUserID).PublicID;
                }
                else
                    CustomerAddressWS.Cells[rc, 19].Value = "UserID: " + CustomerAddresss[i].CreatedUserID.ToString();

                CustomerAddressWS.Cells[rc, 21].Value = CustomerAddresss[i].LastAmendedDateTime.ToOADate();
                CustomerAddressWS.Cells[rc, 22].Value = CustomerAddresss[i].LastAmendedMethod;

                if (users.Any(x => x.ID == CustomerAddresss[i].LastAmendedUserID))
                {
                    CustomerAddressWS.Cells[rc, 23].Value = users.FirstOrDefault(x => x.ID == CustomerAddresss[i].LastAmendedUserID).Firstname;
                    CustomerAddressWS.Cells[rc, 24].Value = users.FirstOrDefault(x => x.ID == CustomerAddresss[i].LastAmendedUserID).PublicID;
                }
                else
                    CustomerAddressWS.Cells[rc, 23].Value = "UserID: " + CustomerAddresss[i].LastAmendedUserID.ToString();

                CustomerAddressWS.Cells[rc, 25].Value = CustomerAddresss[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
