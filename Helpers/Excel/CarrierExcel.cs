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
    public class CarrierExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Site.dbRow> sites = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.Carrier.dbRow> carriers = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.Carrier.Search carrierSearchData;
        public CarrierExcel(wbm_common.DataObjects.Carrier.Search carrierSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.carrierSearchData = carrierSearchData;
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
                        CarrierPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("CarrierExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.Carrier.dbRow>> rc = await CarrierHelper.ExecuteSearch(carrierSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            carriers = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListSiteID = carriers.Select(x => x.SiteID).Distinct().ToList();
            List<int> ListUserID = carriers.Select(x => x.CreatedUserID).Union(carriers.Select(x => x.LastAmendedUserID)).Distinct().ToList();

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

        private void CarrierPage(ExcelPackage ep)
        {
            ExcelWorksheet CarrierWS = ep.Workbook.Worksheets.Add("Carrier");

            #region header
            CarrierWS.View.FreezePanes(2, 1);
            CarrierWS.Cells[1, 1, 1, 11].Style.Font.Bold = true;
            CarrierWS.Cells[1, 1, 1, 11].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            CarrierWS.Column(1).Width = 15;
            CarrierWS.Column(2).Width = 10;
            CarrierWS.Column(3).Width = 15;
            CarrierWS.Column(4).Width = 10;
            CarrierWS.Column(5).Width = 20;
            CarrierWS.Column(6).Width = 25;
            CarrierWS.Column(6).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            CarrierWS.Column(7).Width = 25;
            CarrierWS.Column(8).Width = 15;
            CarrierWS.Column(9).Width = 25;
            CarrierWS.Column(9).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            CarrierWS.Column(10).Width = 25;
            CarrierWS.Column(11).Width = 25;

            CarrierWS.Cells[1, 1].Value = "ID";
            CarrierWS.Cells[1, 2].Value = "Site";
            CarrierWS.Cells[1, 3].Value = "Description";
            CarrierWS.Cells[1, 4].Value = "Sort Order";
            CarrierWS.Cells[1, 5].Value = "Paperless KeyID";
            CarrierWS.Cells[1, 6].Value = "Created DateTime";
            CarrierWS.Cells[1, 7].Value = "Created Method";
            CarrierWS.Cells[1, 8].Value = "Created User";
            CarrierWS.Cells[1, 9].Value = "Last Amended DateTime";
            CarrierWS.Cells[1, 10].Value = "Last Amended Method";
            CarrierWS.Cells[1, 11].Value = "Last Amended User";
            #endregion

            #region processing
            int rc = 2;
            carriers = carriers.OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < carriers.Count(); i++)
            {
                CarrierWS.Cells[rc, 1].Value = carriers[i].PublicID;

                if (sites.Any(x => x.ID == carriers[i].SiteID))
                    CarrierWS.Cells[rc, 2].Value = sites.FirstOrDefault(x => x.ID == carriers[i].SiteID).PublicID;
                else
                    CarrierWS.Cells[rc, 2].Value = "SiteID: " + carriers[i].SiteID.ToString();

                CarrierWS.Cells[rc, 3].Value = carriers[i].Description;
                CarrierWS.Cells[rc, 4].Value = carriers[i].SortOrder;
                CarrierWS.Cells[rc, 5].Value = carriers[i].Paperless_KeyID;
                CarrierWS.Cells[rc, 6].Value = carriers[i].CreatedDateTime.ToOADate();
                CarrierWS.Cells[rc, 7].Value = carriers[i].CreatedMethod;

                if (users.Any(x => x.ID == carriers[i].CreatedUserID))
                {
                    CarrierWS.Cells[rc, 8].Value = users.FirstOrDefault(x => x.ID == carriers[i].CreatedUserID).PublicID;
                }
                else
                    CarrierWS.Cells[rc, 2].Value = "UserID: " + carriers[i].CreatedUserID.ToString();

                CarrierWS.Cells[rc, 9].Value = carriers[i].LastAmendedDateTime.ToOADate();
                CarrierWS.Cells[rc, 10].Value = carriers[i].LastAmendedMethod;

                if (users.Any(x => x.ID == carriers[i].LastAmendedUserID))
                {
                    CarrierWS.Cells[rc, 11].Value = users.FirstOrDefault(x => x.ID == carriers[i].LastAmendedUserID).PublicID;
                }
                else
                    CarrierWS.Cells[rc, 2].Value = "UserID: " + carriers[i].LastAmendedUserID.ToString();

                rc++;
            }
            #endregion
        }
    }
}
