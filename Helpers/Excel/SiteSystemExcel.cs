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
    public class SiteSystemExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.SiteSystem.dbRow> SiteSystems = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.SiteSystem.Search SiteSystemSearchData;
        public SiteSystemExcel(wbm_common.DataObjects.SiteSystem.Search SiteSystemSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.SiteSystemSearchData = SiteSystemSearchData;
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
                        SiteSystemPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("SiteSystemExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.SiteSystem.dbRow>> rc = await SiteSystemHelper.ExecuteSearch(SiteSystemSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            SiteSystems = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = SiteSystems.Select(x => x.CreatedUserID).Union(SiteSystems.Select(x => x.LastAmendedUserID)).Distinct().ToList();

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

        private void SiteSystemPage(ExcelPackage ep)
        {
            ExcelWorksheet SiteSystemWS = ep.Workbook.Worksheets.Add("SiteSystem");

            #region header
            SiteSystemWS.View.FreezePanes(2, 1);
            SiteSystemWS.Cells[1, 1, 1, 12].Style.Font.Bold = true;
            SiteSystemWS.Cells[1, 1, 1, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            SiteSystemWS.Column(1).Width = 15;
            SiteSystemWS.Column(2).Width = 15;
            SiteSystemWS.Column(3).Width = 25;
            SiteSystemWS.Column(4).Width = 25;
            SiteSystemWS.Column(4).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            SiteSystemWS.Column(5).Width = 25;
            SiteSystemWS.Column(6).Width = 25;
            SiteSystemWS.Column(7).Width = 25;
            SiteSystemWS.Column(8).Width = 25;
            SiteSystemWS.Column(8).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            SiteSystemWS.Column(9).Width = 25;
            SiteSystemWS.Column(10).Width = 25;
            SiteSystemWS.Column(11).Width = 25;
            SiteSystemWS.Column(12).Width = 15;

            SiteSystemWS.Cells[1, 1].Value = "ID";
            SiteSystemWS.Cells[1, 2].Value = "Description";
            SiteSystemWS.Cells[1, 3].Value = "Sort Order";
            SiteSystemWS.Cells[1, 4].Value = "Created DateTime";
            SiteSystemWS.Cells[1, 5].Value = "Created Method";
            SiteSystemWS.Cells[1, 6].Value = "Created User";
            SiteSystemWS.Cells[1, 7].Value = "Created User Name";
            SiteSystemWS.Cells[1, 8].Value = "Last Amended DateTime";
            SiteSystemWS.Cells[1, 9].Value = "Last Amended Method";
            SiteSystemWS.Cells[1, 10].Value = "Last Amended User";
            SiteSystemWS.Cells[1, 11].Value = "Last Amended Name";
            SiteSystemWS.Cells[1, 12].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            SiteSystems = SiteSystems.OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < SiteSystems.Count(); i++)
            {
                SiteSystemWS.Cells[rc, 1].Value = SiteSystems[i].PublicID;
                SiteSystemWS.Cells[rc, 2].Value = SiteSystems[i].Description;
                SiteSystemWS.Cells[rc, 3].Value = SiteSystems[i].SortOrder;
                SiteSystemWS.Cells[rc, 4].Value = SiteSystems[i].CreatedDateTime.ToOADate();
                SiteSystemWS.Cells[rc, 5].Value = SiteSystems[i].CreatedMethod;

                if (users.Any(x => x.ID == SiteSystems[i].CreatedUserID))
                {
                    SiteSystemWS.Cells[rc, 6].Value = users.FirstOrDefault(x => x.ID == SiteSystems[i].CreatedUserID).PublicID;
                    SiteSystemWS.Cells[rc, 7].Value = users.FirstOrDefault(x => x.ID == SiteSystems[i].CreatedUserID).Firstname;
                }
                else
                    SiteSystemWS.Cells[rc, 6].Value = "UserID: " + SiteSystems[i].CreatedUserID.ToString();

                SiteSystemWS.Cells[rc, 8].Value = SiteSystems[i].LastAmendedDateTime.ToOADate();
                SiteSystemWS.Cells[rc, 9].Value = SiteSystems[i].LastAmendedMethod;

                if (users.Any(x => x.ID == SiteSystems[i].LastAmendedUserID))
                {
                    SiteSystemWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == SiteSystems[i].LastAmendedUserID).PublicID;
                    SiteSystemWS.Cells[rc, 11].Value = users.FirstOrDefault(x => x.ID == SiteSystems[i].LastAmendedUserID).Firstname;
                }
                else
                    SiteSystemWS.Cells[rc, 10].Value = "UserID: " + SiteSystems[i].LastAmendedUserID.ToString();

                SiteSystemWS.Cells[rc, 12].Value = SiteSystems[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
