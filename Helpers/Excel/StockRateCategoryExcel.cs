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
    public class StockRateCategoryExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Company.dbRow> companies = null;
        public List<wbm_common.DataObjects.Site.dbRow> sites = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.StockRateCategory.dbRow> StockRateCategorys = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.StockRateCategory.Search StockRateCategorySearchData;
        public StockRateCategoryExcel(wbm_common.DataObjects.StockRateCategory.Search StockRateCategorySearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.StockRateCategorySearchData = StockRateCategorySearchData;
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
                        StockRateCategoryPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("StockRateCategoryExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.StockRateCategory.dbRow>> rc = await StockRateCategoryHelper.ExecuteSearch(StockRateCategorySearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            StockRateCategorys = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = StockRateCategorys.Select(x => x.CreatedUserID).Union(StockRateCategorys.Select(x => x.LastAmendedUserID)).Distinct().ToList();
            List<int> ListCompanyID = StockRateCategorys.Select(x => x.CompanyID).Distinct().ToList();
            List<int> ListSiteID = StockRateCategorys.Select(x => x.SiteID).Distinct().ToList();

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

        private void StockRateCategoryPage(ExcelPackage ep)
        {
            ExcelWorksheet StockRateCategoryWS = ep.Workbook.Worksheets.Add("StockRateCategory");

            #region header
            StockRateCategoryWS.View.FreezePanes(2, 1);
            StockRateCategoryWS.Cells[1, 1, 1, 16].Style.Font.Bold = true;
            StockRateCategoryWS.Cells[1, 1, 1, 16].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            StockRateCategoryWS.Column(1).Width = 25;
            StockRateCategoryWS.Column(2).Width = 25;
            StockRateCategoryWS.Column(3).Width = 25;
            StockRateCategoryWS.Column(4).Width = 25;
            StockRateCategoryWS.Column(5).Width = 25;
            StockRateCategoryWS.Column(6).Width = 25;
            StockRateCategoryWS.Column(7).Width = 25;
            StockRateCategoryWS.Column(8).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            StockRateCategoryWS.Column(8).Width = 25;
            StockRateCategoryWS.Column(9).Width = 25;
            StockRateCategoryWS.Column(10).Width = 25;
            StockRateCategoryWS.Column(11).Width = 25;
            StockRateCategoryWS.Column(12).Width = 25;
            StockRateCategoryWS.Column(12).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            StockRateCategoryWS.Column(13).Width = 25;
            StockRateCategoryWS.Column(14).Width = 25;
            StockRateCategoryWS.Column(15).Width = 25;
            StockRateCategoryWS.Column(16).Width = 15;

            StockRateCategoryWS.Cells[1, 1].Value = "Company";
            StockRateCategoryWS.Cells[1, 2].Value = "Company Name";
            StockRateCategoryWS.Cells[1, 3].Value = "ID";
            StockRateCategoryWS.Cells[1, 4].Value = "Site";
            StockRateCategoryWS.Cells[1, 5].Value = "Site Description";
            StockRateCategoryWS.Cells[1, 6].Value = "Description";
            StockRateCategoryWS.Cells[1, 7].Value = "Sort Order";
            StockRateCategoryWS.Cells[1, 8].Value = "Created DateTime";
            StockRateCategoryWS.Cells[1, 9].Value = "Created Method";
            StockRateCategoryWS.Cells[1, 10].Value = "Created User";
            StockRateCategoryWS.Cells[1, 11].Value = "Created User Name";
            StockRateCategoryWS.Cells[1, 12].Value = "Last Amended DateTime";
            StockRateCategoryWS.Cells[1, 13].Value = "Last Amended Method";
            StockRateCategoryWS.Cells[1, 14].Value = "Last Amended User";
            StockRateCategoryWS.Cells[1, 15].Value = "Last Amended Name";
            StockRateCategoryWS.Cells[1, 16].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            StockRateCategorys = StockRateCategorys.OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < StockRateCategorys.Count(); i++)
            {
                if (companies.Any(x => x.ID == StockRateCategorys[i].CompanyID))
                {
                    StockRateCategoryWS.Cells[rc, 1].Value = companies.FirstOrDefault(x => x.ID == StockRateCategorys[i].CompanyID).ShortName;
                    StockRateCategoryWS.Cells[rc, 2].Value = companies.FirstOrDefault(x => x.ID == StockRateCategorys[i].CompanyID).LongName;
                }
                else
                    StockRateCategoryWS.Cells[rc, 1].Value = "CompanyID: " + StockRateCategorys[i].CompanyID.ToString();

                StockRateCategoryWS.Cells[rc, 3].Value = StockRateCategorys[i].PublicID;

                if (sites.Any(x => x.ID == StockRateCategorys[i].SiteID))
                {
                    StockRateCategoryWS.Cells[rc, 4].Value = sites.FirstOrDefault(x => x.ID == StockRateCategorys[i].SiteID).PublicID;
                    StockRateCategoryWS.Cells[rc, 5].Value = sites.FirstOrDefault(x => x.ID == StockRateCategorys[i].SiteID).Description;
                }
                else
                    StockRateCategoryWS.Cells[rc, 4].Value = "SiteID: " + StockRateCategorys[i].SiteID.ToString();

                StockRateCategoryWS.Cells[rc, 6].Value = StockRateCategorys[i].Description;
                StockRateCategoryWS.Cells[rc, 7].Value = StockRateCategorys[i].SortOrder;
                StockRateCategoryWS.Cells[rc, 8].Value = StockRateCategorys[i].CreatedDateTime.ToOADate();
                StockRateCategoryWS.Cells[rc, 9].Value = StockRateCategorys[i].CreatedMethod;

                if (users.Any(x => x.ID == StockRateCategorys[i].CreatedUserID))
                {
                    StockRateCategoryWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == StockRateCategorys[i].CreatedUserID).PublicID;
                    StockRateCategoryWS.Cells[rc, 11].Value = users.FirstOrDefault(x => x.ID == StockRateCategorys[i].CreatedUserID).Firstname;
                }
                else
                    StockRateCategoryWS.Cells[rc, 10].Value = "UserID: " + StockRateCategorys[i].CreatedUserID.ToString();

                StockRateCategoryWS.Cells[rc, 12].Value = StockRateCategorys[i].LastAmendedDateTime.ToOADate();
                StockRateCategoryWS.Cells[rc, 13].Value = StockRateCategorys[i].LastAmendedMethod;

                if (users.Any(x => x.ID == StockRateCategorys[i].LastAmendedUserID))
                {
                    StockRateCategoryWS.Cells[rc, 14].Value = users.FirstOrDefault(x => x.ID == StockRateCategorys[i].LastAmendedUserID).PublicID;
                    StockRateCategoryWS.Cells[rc, 15].Value = users.FirstOrDefault(x => x.ID == StockRateCategorys[i].LastAmendedUserID).Firstname;
                }
                else
                    StockRateCategoryWS.Cells[rc, 14].Value = "UserID: " + StockRateCategorys[i].LastAmendedUserID.ToString();

                StockRateCategoryWS.Cells[rc, 16].Value = StockRateCategorys[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
