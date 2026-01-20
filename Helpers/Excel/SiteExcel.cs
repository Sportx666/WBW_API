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
    public class SiteExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.SiteSystem.dbRow> sitesystems = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.Site.dbRow> Sites = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.Site.Search SiteSearchData;
        public SiteExcel(wbm_common.DataObjects.Site.Search SiteSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.SiteSearchData = SiteSearchData;
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
                        SitePage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("SiteExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.Site.dbRow>> rc = await SiteHelper.ExecuteSearch(SiteSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            Sites = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = Sites.Select(x => x.CreatedUserID).Union(Sites.Select(x => x.LastAmendedUserID)).Distinct().ToList();
            List<int> ListSiteSystems = Sites.Select(x => x.SiteSystemID).Distinct().ToList();

            if (ListSiteSystems.Count > 0)
            {
                Result<List<wbm_common.DataObjects.SiteSystem.dbRow>> sitesystemsResults = _WBMDB.SiteSystem_ListByListIDs(ListSiteSystems).GetAwaiter().GetResult();
                if (sitesystemsResults.IsFailure || sitesystemsResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                sitesystems = sitesystemsResults.Value;
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

        private void SitePage(ExcelPackage ep)
        {
            ExcelWorksheet SiteWS = ep.Workbook.Worksheets.Add("Site");

            #region header
            SiteWS.View.FreezePanes(2, 1);
            SiteWS.Cells[1, 1, 1, 15].Style.Font.Bold = true;
            SiteWS.Cells[1, 1, 1, 15].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            SiteWS.Column(1).Width = 15;
            SiteWS.Column(2).Width = 15;
            SiteWS.Column(3).Width = 25;
            SiteWS.Column(4).Width = 25;
            SiteWS.Column(5).Width = 25;
            SiteWS.Column(6).Width = 25;
            SiteWS.Column(7).Width = 25;
            SiteWS.Column(7).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            SiteWS.Column(8).Width = 25;
            SiteWS.Column(9).Width = 25;
            SiteWS.Column(10).Width = 25;
            SiteWS.Column(11).Width = 25;
            SiteWS.Column(11).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            SiteWS.Column(12).Width = 25;
            SiteWS.Column(13).Width = 25;
            SiteWS.Column(14).Width = 25;
            SiteWS.Column(15).Width = 15;

            SiteWS.Cells[1, 1].Value = "ID";
            SiteWS.Cells[1, 2].Value = "Site System ID";
            SiteWS.Cells[1, 3].Value = "Site System Description";
            SiteWS.Cells[1, 4].Value = "Active";
            SiteWS.Cells[1, 5].Value = "Description";
            SiteWS.Cells[1, 6].Value = "Sort Order";
            SiteWS.Cells[1, 7].Value = "Created DateTime";
            SiteWS.Cells[1, 8].Value = "Created Method";
            SiteWS.Cells[1, 9].Value = "Created User";
            SiteWS.Cells[1, 10].Value = "Created User Name";
            SiteWS.Cells[1, 11].Value = "Last Amended DateTime";
            SiteWS.Cells[1, 12].Value = "Last Amended Method";
            SiteWS.Cells[1, 13].Value = "Last Amended User";
            SiteWS.Cells[1, 14].Value = "Last Amended Name";
            SiteWS.Cells[1, 15].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            Sites = Sites.OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < Sites.Count(); i++)
            {
                SiteWS.Cells[rc, 1].Value = Sites[i].PublicID;

                if (sitesystems.Any(x => x.ID == Sites[i].SiteSystemID))
                {
                    SiteWS.Cells[rc, 2].Value = sitesystems.FirstOrDefault(x => x.ID == Sites[i].SiteSystemID).PublicID;
                    SiteWS.Cells[rc, 3].Value = sitesystems.FirstOrDefault(x => x.ID == Sites[i].SiteSystemID).Description;
                }
                else
                    SiteWS.Cells[rc, 2].Value = "SiteSystemID: " + Sites[i].SiteSystemID.ToString();

                SiteWS.Cells[rc, 4].Value = Sites[i].Active;
                SiteWS.Cells[rc, 5].Value = Sites[i].Description;
                SiteWS.Cells[rc, 6].Value = Sites[i].SortOrder;
                SiteWS.Cells[rc, 7].Value = Sites[i].CreatedDateTime.ToOADate();
                SiteWS.Cells[rc, 8].Value = Sites[i].CreatedMethod;

                if (users.Any(x => x.ID == Sites[i].CreatedUserID))
                {
                    SiteWS.Cells[rc, 9].Value = users.FirstOrDefault(x => x.ID == Sites[i].CreatedUserID).PublicID;
                    SiteWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == Sites[i].CreatedUserID).Firstname;
                }
                else
                    SiteWS.Cells[rc, 9].Value = "UserID: " + Sites[i].CreatedUserID.ToString();

                SiteWS.Cells[rc, 11].Value = Sites[i].LastAmendedDateTime.ToOADate();
                SiteWS.Cells[rc, 12].Value = Sites[i].LastAmendedMethod;

                if (users.Any(x => x.ID == Sites[i].LastAmendedUserID))
                {
                    SiteWS.Cells[rc, 13].Value = users.FirstOrDefault(x => x.ID == Sites[i].LastAmendedUserID).PublicID;
                    SiteWS.Cells[rc, 14].Value = users.FirstOrDefault(x => x.ID == Sites[i].LastAmendedUserID).Firstname;
                }
                else
                    SiteWS.Cells[rc, 13].Value = "UserID: " + Sites[i].LastAmendedUserID.ToString();

                SiteWS.Cells[rc, 15].Value = Sites[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
