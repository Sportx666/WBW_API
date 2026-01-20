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
    public class CompanyExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.SiteSystem.dbRow> sitesystems = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.Company.dbRow> Companies = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.Company.Search CompanySearchData;
        public CompanyExcel(wbm_common.DataObjects.Company.Search CompanySearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.CompanySearchData = CompanySearchData;
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
                        CompanyPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("CompanyExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.Company.dbRow>> rc = await CompanyHelper.ExecuteSearch(CompanySearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            Companies = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListSiteSystemsID = Companies.Select(x => x.SystemID).Distinct().ToList();
            List<int> ListUserID = Companies.Select(x => x.CreatedUserID).Union(Companies.Select(x => x.LastAmendedUserID)).Distinct().ToList();

            if (ListSiteSystemsID.Count > 0)
            {
                Result<List<wbm_common.DataObjects.SiteSystem.dbRow>> sitesystemsResults = _WBMDB.SiteSystem_ListByListIDs(ListSiteSystemsID).GetAwaiter().GetResult();
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

        private void CompanyPage(ExcelPackage ep)
        {
            ExcelWorksheet CompanyWS = ep.Workbook.Worksheets.Add("Company");

            #region header
            CompanyWS.View.FreezePanes(2, 1);
            CompanyWS.Cells[1, 1, 1, 17].Style.Font.Bold = true;
            CompanyWS.Cells[1, 1, 1, 17].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            CompanyWS.Column(1).Width = 15;
            CompanyWS.Column(2).Width = 20;
            CompanyWS.Column(3).Width = 15;
            CompanyWS.Column(4).Width = 25;
            CompanyWS.Column(5).Width = 10;
            CompanyWS.Column(6).Width = 10;
            CompanyWS.Column(7).Width = 12;
            CompanyWS.Column(8).Width = 20;
            CompanyWS.Column(9).Width = 25;
            CompanyWS.Column(9).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            CompanyWS.Column(10).Width = 20;
            CompanyWS.Column(11).Width = 20;
            CompanyWS.Column(12).Width = 20;
            CompanyWS.Column(13).Width = 25;
            CompanyWS.Column(13).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            CompanyWS.Column(14).Width = 25;
            CompanyWS.Column(15).Width = 20;
            CompanyWS.Column(16).Width = 20;
            CompanyWS.Column(17).Width = 15;

            CompanyWS.Cells[1, 1].Value = "Short Name";
            CompanyWS.Cells[1, 2].Value = "Long Name";
            CompanyWS.Cells[1, 3].Value = "Site System";
            CompanyWS.Cells[1, 4].Value = "Site System Description";
            CompanyWS.Cells[1, 5].Value = "Active";
            CompanyWS.Cells[1, 6].Value = "Sort Order";
            CompanyWS.Cells[1, 7].Value = "Abbreviation";
            CompanyWS.Cells[1, 8].Value = "Abel Cost Centre";
            CompanyWS.Cells[1, 9].Value = "Created DateTime";
            CompanyWS.Cells[1, 10].Value = "Created Method";
            CompanyWS.Cells[1, 11].Value = "Created User";
            CompanyWS.Cells[1, 12].Value = "Created User Name";
            CompanyWS.Cells[1, 13].Value = "Last Amended DateTime";
            CompanyWS.Cells[1, 14].Value = "Last Amended Method";
            CompanyWS.Cells[1, 15].Value = "Last Amended User";
            CompanyWS.Cells[1, 16].Value = "Last Amended Name";
            CompanyWS.Cells[1, 17].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            Companies = Companies.OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < Companies.Count(); i++)
            {
                CompanyWS.Cells[rc, 1].Value = Companies[i].ShortName;
                CompanyWS.Cells[rc, 2].Value = Companies[i].LongName;

                if (sitesystems.Any(x => x.ID == Companies[i].SystemID))
                {
                    CompanyWS.Cells[rc, 3].Value = sitesystems.FirstOrDefault(x => x.ID == Companies[i].SystemID).PublicID;
                    CompanyWS.Cells[rc, 4].Value = sitesystems.FirstOrDefault(x => x.ID == Companies[i].SystemID).Description;
                }
                else
                    CompanyWS.Cells[rc, 3].Value = "SystemID: " + Companies[i].SystemID.ToString();

                CompanyWS.Cells[rc, 5].Value = Companies[i].Active;
                CompanyWS.Cells[rc, 6].Value = Companies[i].SortOrder;
                CompanyWS.Cells[rc, 7].Value = Companies[i].Abbreviation;
                CompanyWS.Cells[rc, 8].Value = Companies[i].AbelCostCentre;
                CompanyWS.Cells[rc, 9].Value = Companies[i].CreatedDateTime.ToOADate();
                CompanyWS.Cells[rc, 10].Value = Companies[i].CreatedMethod;

                if (users.Any(x => x.ID == Companies[i].CreatedUserID))
                {
                    CompanyWS.Cells[rc, 11].Value = users.FirstOrDefault(x => x.ID == Companies[i].CreatedUserID).Firstname;
                    CompanyWS.Cells[rc, 12].Value = users.FirstOrDefault(x => x.ID == Companies[i].CreatedUserID).PublicID;
                }
                else
                    CompanyWS.Cells[rc, 11].Value = "UserID: " + Companies[i].CreatedUserID.ToString();

                CompanyWS.Cells[rc, 13].Value = Companies[i].LastAmendedDateTime.ToOADate();
                CompanyWS.Cells[rc, 14].Value = Companies[i].LastAmendedMethod;

                if (users.Any(x => x.ID == Companies[i].LastAmendedUserID))
                {
                    CompanyWS.Cells[rc, 15].Value = users.FirstOrDefault(x => x.ID == Companies[i].LastAmendedUserID).Firstname;
                    CompanyWS.Cells[rc, 16].Value = users.FirstOrDefault(x => x.ID == Companies[i].LastAmendedUserID).PublicID;
                }
                else
                    CompanyWS.Cells[rc, 15].Value = "UserID: " + Companies[i].LastAmendedUserID.ToString();

                CompanyWS.Cells[rc, 17].Value = Companies[i].DeletedFlag;
                rc++;
            }
            #endregion
        }
    }
}
