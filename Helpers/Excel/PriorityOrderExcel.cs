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
    public class PriorityOrderExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Company.dbRow> companies = null;
        public List<wbm_common.DataObjects.Site.dbRow> sites = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.Owner.dbRow> owners = null;
        public List<wbm_common.DataObjects.PriorityOrder.dbRow> PriorityOrders = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.PriorityOrder.Search PriorityOrderSearchData;
        public PriorityOrderExcel(wbm_common.DataObjects.PriorityOrder.Search PriorityOrderSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.PriorityOrderSearchData = PriorityOrderSearchData;
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
                        PriorityOrderPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("PriorityOrderExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.PriorityOrder.dbRow>> rc = await PriorityOrderHelper.ExecuteSearch(PriorityOrderSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            PriorityOrders = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListOwnerID = PriorityOrders.Select(x => x.OwnerID).Distinct().ToList();
            List<int> ListCompanyIDs = PriorityOrders.Select(x => x.CompanyID).Distinct().ToList();
            List<int> ListSiteID = PriorityOrders.Select(x => x.SiteID).Distinct().ToList();
            List<int> ListUserID = PriorityOrders.Select(x => x.CreatedUserID).Union(PriorityOrders.Select(x => x.LastAmendedUserID)).Distinct().ToList();

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

        private void PriorityOrderPage(ExcelPackage ep)
        {
            ExcelWorksheet PriorityOrderWS = ep.Workbook.Worksheets.Add("PriorityOrder");

            #region header
            PriorityOrderWS.View.FreezePanes(2, 1);
            PriorityOrderWS.Cells[1, 1, 1, 16].Style.Font.Bold = true;
            PriorityOrderWS.Cells[1, 1, 1, 16].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            PriorityOrderWS.Column(1).Width = 15;
            PriorityOrderWS.Column(2).Width = 25;
            PriorityOrderWS.Column(3).Width = 15;
            PriorityOrderWS.Column(4).Width = 15;
            PriorityOrderWS.Column(5).Width = 10;
            PriorityOrderWS.Column(6).Width = 25;
            PriorityOrderWS.Column(7).Width = 15;
            PriorityOrderWS.Column(8).Width = 25;
            PriorityOrderWS.Column(8).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            PriorityOrderWS.Column(9).Width = 25;
            PriorityOrderWS.Column(10).Width = 25;
            PriorityOrderWS.Column(11).Width = 25;
            PriorityOrderWS.Column(12).Width = 25;
            PriorityOrderWS.Column(12).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            PriorityOrderWS.Column(13).Width = 30;
            PriorityOrderWS.Column(14).Width = 25;
            PriorityOrderWS.Column(15).Width = 25;
            PriorityOrderWS.Column(16).Width = 15;

            PriorityOrderWS.Cells[1, 1].Value = "Company";
            PriorityOrderWS.Cells[1, 2].Value = "Company Name";
            PriorityOrderWS.Cells[1, 3].Value = "Owner ID";
            PriorityOrderWS.Cells[1, 4].Value = "Owner Name";
            PriorityOrderWS.Cells[1, 5].Value = "Site";
            PriorityOrderWS.Cells[1, 6].Value = "Site Description";
            PriorityOrderWS.Cells[1, 7].Value = "Order Priority";
            PriorityOrderWS.Cells[1, 8].Value = "Created DateTime";
            PriorityOrderWS.Cells[1, 9].Value = "Created Method";
            PriorityOrderWS.Cells[1, 10].Value = "Created User";
            PriorityOrderWS.Cells[1, 11].Value = "Created User Name";
            PriorityOrderWS.Cells[1, 12].Value = "Last Amended DateTime";
            PriorityOrderWS.Cells[1, 13].Value = "Last Amended Method";
            PriorityOrderWS.Cells[1, 14].Value = "Last Amended User";
            PriorityOrderWS.Cells[1, 15].Value = "Last Amended Name";
            PriorityOrderWS.Cells[1, 16].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < PriorityOrders.Count(); i++)
            {
                if (companies.Any(x => x.ID == PriorityOrders[i].CompanyID))
                {
                    PriorityOrderWS.Cells[rc, 1].Value = companies.FirstOrDefault(x => x.ID == PriorityOrders[i].CompanyID).ShortName;
                    PriorityOrderWS.Cells[rc, 2].Value = companies.FirstOrDefault(x => x.ID == PriorityOrders[i].CompanyID).LongName;
                }
                else
                    PriorityOrderWS.Cells[rc, 1].Value = "CompanyID: " + PriorityOrders[i].CompanyID.ToString();

                if (owners.Any(x => x.ID == PriorityOrders[i].OwnerID))
                {
                    PriorityOrderWS.Cells[rc, 3].Value = owners.FirstOrDefault(x => x.ID == PriorityOrders[i].OwnerID).SourceOwnerID;
                    PriorityOrderWS.Cells[rc, 4].Value = owners.FirstOrDefault(x => x.ID == PriorityOrders[i].OwnerID).Name;
                }
                else
                    PriorityOrderWS.Cells[rc, 3].Value = "OwnerID: " + PriorityOrders[i].OwnerID.ToString();

                if (sites.Any(x => x.ID == PriorityOrders[i].SiteID))
                {
                    PriorityOrderWS.Cells[rc, 5].Value = sites.FirstOrDefault(x => x.ID == PriorityOrders[i].SiteID).PublicID;
                    PriorityOrderWS.Cells[rc, 6].Value = sites.FirstOrDefault(x => x.ID == PriorityOrders[i].SiteID).Description;
                }
                else
                    PriorityOrderWS.Cells[rc, 5].Value = "SiteID: " + PriorityOrders[i].SiteID.ToString();

                PriorityOrderWS.Cells[rc, 7].Value = PriorityOrders[i].OrderPriority;

                PriorityOrderWS.Cells[rc, 9].Value = PriorityOrders[i].CreatedDateTime.ToOADate();
                PriorityOrderWS.Cells[rc, 9].Value = PriorityOrders[i].CreatedMethod;

                if (users.Any(x => x.ID == PriorityOrders[i].CreatedUserID))
                {
                    PriorityOrderWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == PriorityOrders[i].CreatedUserID).Firstname;
                    PriorityOrderWS.Cells[rc, 11].Value = users.FirstOrDefault(x => x.ID == PriorityOrders[i].CreatedUserID).PublicID;
                }
                else
                    PriorityOrderWS.Cells[rc, 10].Value = "UserID: " + PriorityOrders[i].CreatedUserID.ToString();

                PriorityOrderWS.Cells[rc, 12].Value = PriorityOrders[i].LastAmendedDateTime.ToOADate();
                PriorityOrderWS.Cells[rc, 13].Value = PriorityOrders[i].LastAmendedMethod;

                if (users.Any(x => x.ID == PriorityOrders[i].LastAmendedUserID))
                {
                    PriorityOrderWS.Cells[rc, 14].Value = users.FirstOrDefault(x => x.ID == PriorityOrders[i].LastAmendedUserID).Firstname;
                    PriorityOrderWS.Cells[rc, 15].Value = users.FirstOrDefault(x => x.ID == PriorityOrders[i].LastAmendedUserID).PublicID;
                }
                else
                    PriorityOrderWS.Cells[rc, 14].Value = "UserID: " + PriorityOrders[i].LastAmendedUserID.ToString();

                PriorityOrderWS.Cells[rc, 16].Value = PriorityOrders[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
