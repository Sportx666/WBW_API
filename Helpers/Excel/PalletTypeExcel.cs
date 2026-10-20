using Microsoft.AspNetCore.SignalR;
using NLog;
using NLog.Web;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using wbm_common.NonDatabaseObjects;
using WBM_API.WBM_API_DB;
using wbm_common.DataObjects;


namespace WBM_API.Helpers.Excel
{
    public class PalletTypeExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.SiteSystem.dbRow> sitesystems = null;
        public List<wbm_common.DataObjects.Site.dbRowExtended> sites = null;
        public List<wbm_common.DataObjects.PalletType.dbRowExtended> PalletTypes = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.PalletType.Search PalletTypeSearchData;
        public PalletTypeExcel(wbm_common.DataObjects.PalletType.Search PalletTypeSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.PalletTypeSearchData = PalletTypeSearchData;
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
                        PalletTypePage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("PalletTypeExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.PalletType.dbRowExtended>> rc = await PalletTypeHelper.ExecuteSearch(PalletTypeSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            PalletTypes = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = PalletTypes.Select(x => x.CreatedUserID).Union(PalletTypes.Select(x => x.LastAmendedUserID)).Distinct().ToList();
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

            List<int> ListSiteID = PalletTypes.Select(x => x.SiteID).ToList();
            if (ListSiteID.Count > 0)
            {
                Result<List<wbm_common.DataObjects.Site.dbRowExtended>> siteResults = _WBMDB.Site_ListByListIDs(ListSiteID).GetAwaiter().GetResult();
                if (siteResults.IsFailure || siteResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                sites = siteResults.Value;
            }

            List<int> ListSiteSystemID = PalletTypes.Select(x => x.SiteSystemID).ToList();
            if (ListSiteSystemID.Count > 0)
            {
                Result<List<wbm_common.DataObjects.SiteSystem.dbRow>> sitesytemsResults = _WBMDB.SiteSystem_ListByListIDs(ListSiteSystemID).GetAwaiter().GetResult();
                if (sitesytemsResults.IsFailure || sitesytemsResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                sitesystems = sitesytemsResults.Value;
            }

            return true;
        }

        private void PalletTypePage(ExcelPackage ep)
        {
            ExcelWorksheet PalletTypeWS = ep.Workbook.Worksheets.Add("PalletType");

            #region header
            PalletTypeWS.View.FreezePanes(2, 1);
            PalletTypeWS.Cells[1, 1, 1, 15].Style.Font.Bold = true;
            PalletTypeWS.Cells[1, 1, 1, 15].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            PalletTypeWS.Column(1).Width = 10;
            PalletTypeWS.Column(2).Width = 10;
            PalletTypeWS.Column(3).Width = 15;
            PalletTypeWS.Column(4).Width = 25;
            PalletTypeWS.Column(5).Width = 20;
            PalletTypeWS.Column(6).Width = 10;
            PalletTypeWS.Column(7).Width = 25;
            PalletTypeWS.Column(7).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            PalletTypeWS.Column(8).Width = 25;
            PalletTypeWS.Column(9).Width = 25;
            PalletTypeWS.Column(10).Width = 25;
            PalletTypeWS.Column(11).Width = 25;
            PalletTypeWS.Column(11).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            PalletTypeWS.Column(12).Width = 25;
            PalletTypeWS.Column(13).Width = 25;
            PalletTypeWS.Column(14).Width = 25;
            PalletTypeWS.Column(15).Width = 15;

            PalletTypeWS.Cells[1, 1].Value = "ID";
            PalletTypeWS.Cells[1, 2].Value = "Site";
            PalletTypeWS.Cells[1, 3].Value = "Site System";
            PalletTypeWS.Cells[1, 4].Value = "Description";
            PalletTypeWS.Cells[1, 5].Value = "Apply Pallet Hire";
            PalletTypeWS.Cells[1, 6].Value = "Sort Order";
            PalletTypeWS.Cells[1, 7].Value = "Created DateTime";
            PalletTypeWS.Cells[1, 8].Value = "Created Method";
            PalletTypeWS.Cells[1, 9].Value = "Created User";
            PalletTypeWS.Cells[1, 10].Value = "Created User Name";
            PalletTypeWS.Cells[1, 11].Value = "Last Amended DateTime";
            PalletTypeWS.Cells[1, 12].Value = "Last Amended Method";
            PalletTypeWS.Cells[1, 13].Value = "Last Amended User";
            PalletTypeWS.Cells[1, 14].Value = "Last Amended Name";
            PalletTypeWS.Cells[1, 15].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            PalletTypes = PalletTypes.OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < PalletTypes.Count(); i++)
            {
                PalletTypeWS.Cells[rc, 1].Value = PalletTypes[i].PublicID;

                if (sites.Any(x => x.ID == PalletTypes[i].SiteID))
                    PalletTypeWS.Cells[rc, 2].Value = sites.FirstOrDefault(x => x.ID == PalletTypes[i].SiteID).PublicID;
                else
                    PalletTypeWS.Cells[rc, 2].Value = "SiteID: " + PalletTypes[i].SiteID.ToString();

                if (sitesystems.Any(x => x.ID == PalletTypes[i].SiteSystemID))
                {
                    PalletTypeWS.Cells[rc, 3].Value = sitesystems.FirstOrDefault(x => x.ID == PalletTypes[i].SiteSystemID).PublicID;
                }
                else
                    PalletTypeWS.Cells[rc, 3].Value = "SiteSystemID: " + PalletTypes[i].SiteSystemID.ToString();

                PalletTypeWS.Cells[rc, 4].Value = PalletTypes[i].Description;
                PalletTypeWS.Cells[rc, 5].Value = PalletTypes[i].ApplyPalletHire;
                PalletTypeWS.Cells[rc, 6].Value = PalletTypes[i].SortOrder;
                PalletTypeWS.Cells[rc, 7].Value = PalletTypes[i].CreatedDateTime.ToOADate();
                PalletTypeWS.Cells[rc, 8].Value = PalletTypes[i].CreatedMethod;

                if (users.Any(x => x.ID == PalletTypes[i].CreatedUserID))
                {
                    PalletTypeWS.Cells[rc, 9].Value = users.FirstOrDefault(x => x.ID == PalletTypes[i].CreatedUserID).PublicID;
                    PalletTypeWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == PalletTypes[i].CreatedUserID).Firstname;
                }
                else
                    PalletTypeWS.Cells[rc, 9].Value = "UserID: " + PalletTypes[i].CreatedUserID.ToString();

                PalletTypeWS.Cells[rc, 11].Value = PalletTypes[i].LastAmendedDateTime.ToOADate();
                PalletTypeWS.Cells[rc, 12].Value = PalletTypes[i].LastAmendedMethod;

                if (users.Any(x => x.ID == PalletTypes[i].LastAmendedUserID))
                {
                    PalletTypeWS.Cells[rc, 13].Value = users.FirstOrDefault(x => x.ID == PalletTypes[i].LastAmendedUserID).PublicID;
                    PalletTypeWS.Cells[rc, 14].Value = users.FirstOrDefault(x => x.ID == PalletTypes[i].LastAmendedUserID).Firstname;
                }
                else
                    PalletTypeWS.Cells[rc, 13].Value = "UserID: " + PalletTypes[i].LastAmendedUserID.ToString();

                PalletTypeWS.Cells[rc, 15].Value = PalletTypes[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
