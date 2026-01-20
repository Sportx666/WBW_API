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
    public class ContainerLiftExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Company.dbRow> companies = null;
        public List<wbm_common.DataObjects.Site.dbRow> sites = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.Owner.dbRow> owners = null;
        public List<wbm_common.DataObjects.ContainerLift.dbRow> ContainerLifts = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.ContainerLift.Search ContainerLiftSearchData;
        public ContainerLiftExcel(wbm_common.DataObjects.ContainerLift.Search ContainerLiftSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.ContainerLiftSearchData = ContainerLiftSearchData;
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
                        ContainerLiftPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("ContainerLiftExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.ContainerLift.dbRow>> rc = await ContainerLiftHelper.ExecuteSearch(ContainerLiftSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            ContainerLifts = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListOwnerID = ContainerLifts.Select(x => x.OwnerID).Distinct().ToList();
            List<int> ListCompanyIDs = ContainerLifts.Select(x => x.CompanyID).Distinct().ToList();
            List<int> ListSiteID = ContainerLifts.Select(x => x.SiteID).Distinct().ToList();
            List<int> ListUserID = ContainerLifts.Select(x => x.CreatedUserID).Union(ContainerLifts.Select(x => x.LastAmendedUserID)).Distinct().ToList();

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

        private void ContainerLiftPage(ExcelPackage ep)
        {
            ExcelWorksheet ContainerLiftWS = ep.Workbook.Worksheets.Add("ContainerLift");

            #region header
            ContainerLiftWS.View.FreezePanes(2, 1);
            ContainerLiftWS.Cells[1, 1, 1, 16].Style.Font.Bold = true;
            ContainerLiftWS.Cells[1, 1, 1, 16].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            ContainerLiftWS.Column(1).Width = 12;
            ContainerLiftWS.Column(2).Width = 25;
            ContainerLiftWS.Column(3).Width = 12;
            ContainerLiftWS.Column(4).Width = 15;
            ContainerLiftWS.Column(5).Width = 10;
            ContainerLiftWS.Column(6).Width = 25;
            ContainerLiftWS.Column(7).Width = 25;
            ContainerLiftWS.Column(8).Width = 25;
            ContainerLiftWS.Column(8).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ContainerLiftWS.Column(9).Width = 25;
            ContainerLiftWS.Column(10).Width = 20;
            ContainerLiftWS.Column(11).Width = 25;
            ContainerLiftWS.Column(12).Width = 25;
            ContainerLiftWS.Column(12).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ContainerLiftWS.Column(13).Width = 30;
            ContainerLiftWS.Column(14).Width = 25;
            ContainerLiftWS.Column(15).Width = 25;
            ContainerLiftWS.Column(16).Width = 15;

            ContainerLiftWS.Cells[1, 1].Value = "Company";
            ContainerLiftWS.Cells[1, 2].Value = "Company Name";
            ContainerLiftWS.Cells[1, 3].Value = "Owner ID";
            ContainerLiftWS.Cells[1, 4].Value = "Owner Name";
            ContainerLiftWS.Cells[1, 5].Value = "Site";
            ContainerLiftWS.Cells[1, 6].Value = "Site Description";
            ContainerLiftWS.Cells[1, 7].Value = "Default Number Of Lifts";
            ContainerLiftWS.Cells[1, 8].Value = "Created DateTime";
            ContainerLiftWS.Cells[1, 9].Value = "Created Method";
            ContainerLiftWS.Cells[1, 10].Value = "Created User";
            ContainerLiftWS.Cells[1, 11].Value = "Created User Name";
            ContainerLiftWS.Cells[1, 12].Value = "Last Amended DateTime";
            ContainerLiftWS.Cells[1, 13].Value = "Last Amended Method";
            ContainerLiftWS.Cells[1, 14].Value = "Last Amended User";
            ContainerLiftWS.Cells[1, 15].Value = "Last Amended Name";
            ContainerLiftWS.Cells[1, 16].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < ContainerLifts.Count(); i++)
            {
                if (companies.Any(x => x.ID == ContainerLifts[i].CompanyID))
                {
                    ContainerLiftWS.Cells[rc, 1].Value = companies.FirstOrDefault(x => x.ID == ContainerLifts[i].CompanyID).ShortName;
                    ContainerLiftWS.Cells[rc, 2].Value = companies.FirstOrDefault(x => x.ID == ContainerLifts[i].CompanyID).LongName;
                }
                else
                    ContainerLiftWS.Cells[rc, 1].Value = "CompanyID: " + ContainerLifts[i].CompanyID.ToString();

                if (owners.Any(x => x.ID == ContainerLifts[i].OwnerID))
                {
                    ContainerLiftWS.Cells[rc, 3].Value = owners.FirstOrDefault(x => x.ID == ContainerLifts[i].OwnerID).SourceOwnerID;
                    ContainerLiftWS.Cells[rc, 4].Value = owners.FirstOrDefault(x => x.ID == ContainerLifts[i].OwnerID).Name;
                }
                else
                    ContainerLiftWS.Cells[rc, 3].Value = "OwnerID: " + ContainerLifts[i].OwnerID.ToString();

                if (sites.Any(x => x.ID == ContainerLifts[i].SiteID))
                {
                    ContainerLiftWS.Cells[rc, 5].Value = sites.FirstOrDefault(x => x.ID == ContainerLifts[i].SiteID).PublicID;
                    ContainerLiftWS.Cells[rc, 6].Value = sites.FirstOrDefault(x => x.ID == ContainerLifts[i].SiteID).Description;
                }
                else
                    ContainerLiftWS.Cells[rc, 5].Value = "SiteID: " + ContainerLifts[i].SiteID.ToString();

                ContainerLiftWS.Cells[rc, 7].Value = ContainerLifts[i].DefaultNumberOfLifts;

                ContainerLiftWS.Cells[rc, 8].Value = ContainerLifts[i].CreatedDateTime.ToOADate();
                ContainerLiftWS.Cells[rc, 9].Value = ContainerLifts[i].CreatedMethod;

                if (users.Any(x => x.ID == ContainerLifts[i].CreatedUserID))
                {
                    ContainerLiftWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == ContainerLifts[i].CreatedUserID).Firstname;
                    ContainerLiftWS.Cells[rc, 11].Value = users.FirstOrDefault(x => x.ID == ContainerLifts[i].CreatedUserID).PublicID;
                }
                else
                    ContainerLiftWS.Cells[rc, 10].Value = "UserID: " + ContainerLifts[i].CreatedUserID.ToString();

                ContainerLiftWS.Cells[rc, 12].Value = ContainerLifts[i].LastAmendedDateTime.ToOADate();
                ContainerLiftWS.Cells[rc, 13].Value = ContainerLifts[i].LastAmendedMethod;

                if (users.Any(x => x.ID == ContainerLifts[i].LastAmendedUserID))
                {
                    ContainerLiftWS.Cells[rc, 14].Value = users.FirstOrDefault(x => x.ID == ContainerLifts[i].LastAmendedUserID).Firstname;
                    ContainerLiftWS.Cells[rc, 15].Value = users.FirstOrDefault(x => x.ID == ContainerLifts[i].LastAmendedUserID).PublicID;
                }
                else
                    ContainerLiftWS.Cells[rc, 14].Value = "UserID: " + ContainerLifts[i].LastAmendedUserID.ToString();

                ContainerLiftWS.Cells[rc, 16].Value = ContainerLifts[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
