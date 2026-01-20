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
    public class ZoneProcessingExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Company.dbRow> companies = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.ZoneProcessing.dbRow> ZoneProcessings = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.ZoneProcessing.Search ZoneProcessingSearchData;
        public ZoneProcessingExcel(wbm_common.DataObjects.ZoneProcessing.Search ZoneProcessingSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.ZoneProcessingSearchData = ZoneProcessingSearchData;
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
                        ZoneProcessingPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("ZoneProcessingExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.ZoneProcessing.dbRow>> rc = await ZoneProcessingHelper.ExecuteSearch(ZoneProcessingSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            ZoneProcessings = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = ZoneProcessings.Select(x => x.CreatedUserID).Union(ZoneProcessings.Select(x => x.LastAmendedUserID)).Distinct().ToList();
            List<int> ListCompanyID = ZoneProcessings.Select(x => x.CompanyID).Distinct().ToList();

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

        private void ZoneProcessingPage(ExcelPackage ep)
        {
            ExcelWorksheet ZoneProcessingWS = ep.Workbook.Worksheets.Add("ZoneProcessing");

            #region header
            ZoneProcessingWS.View.FreezePanes(2, 1);
            ZoneProcessingWS.Cells[1, 1, 1, 15].Style.Font.Bold = true;
            ZoneProcessingWS.Cells[1, 1, 1, 15].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            ZoneProcessingWS.Column(1).Width = 15;
            ZoneProcessingWS.Column(2).Width = 25;
            ZoneProcessingWS.Column(3).Width = 25;
            ZoneProcessingWS.Column(4).Width = 15;
            ZoneProcessingWS.Column(5).Width = 15;
            ZoneProcessingWS.Column(6).Width = 25;
            ZoneProcessingWS.Column(7).Width = 25;
            ZoneProcessingWS.Column(7).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ZoneProcessingWS.Column(8).Width = 25;
            ZoneProcessingWS.Column(9).Width = 25;
            ZoneProcessingWS.Column(10).Width = 25;
            ZoneProcessingWS.Column(11).Width = 25;
            ZoneProcessingWS.Column(11).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ZoneProcessingWS.Column(12).Width = 25;
            ZoneProcessingWS.Column(13).Width = 25;
            ZoneProcessingWS.Column(14).Width = 25;
            ZoneProcessingWS.Column(15).Width = 15;

            ZoneProcessingWS.Cells[1, 1].Value = "Company";
            ZoneProcessingWS.Cells[1, 2].Value = "Company Name";
            ZoneProcessingWS.Cells[1, 3].Value = "Zone Number";
            ZoneProcessingWS.Cells[1, 4].Value = "Is Pick Face";
            ZoneProcessingWS.Cells[1, 5].Value = "Is reserve";
            ZoneProcessingWS.Cells[1, 6].Value = "Is Carton Consolidation";
            ZoneProcessingWS.Cells[1, 7].Value = "Created DateTime";
            ZoneProcessingWS.Cells[1, 8].Value = "Created Method";
            ZoneProcessingWS.Cells[1, 9].Value = "Created User";
            ZoneProcessingWS.Cells[1, 10].Value = "Created User Name";
            ZoneProcessingWS.Cells[1, 11].Value = "Last Amended DateTime";
            ZoneProcessingWS.Cells[1, 12].Value = "Last Amended Method";
            ZoneProcessingWS.Cells[1, 13].Value = "Last Amended User";
            ZoneProcessingWS.Cells[1, 14].Value = "Last Amended Name";
            ZoneProcessingWS.Cells[1, 15].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < ZoneProcessings.Count(); i++)
            {
                if (companies.Any(x => x.ID == ZoneProcessings[i].CompanyID))
                {
                    ZoneProcessingWS.Cells[rc, 1].Value = companies.FirstOrDefault(x => x.ID == ZoneProcessings[i].CompanyID).Abbreviation;
                    ZoneProcessingWS.Cells[rc, 2].Value = companies.FirstOrDefault(x => x.ID == ZoneProcessings[i].CompanyID).LongName;
                }
                else
                    ZoneProcessingWS.Cells[rc, 1].Value = "CompanyID: " + ZoneProcessings[i].CompanyID.ToString();

                ZoneProcessingWS.Cells[rc, 3].Value = ZoneProcessings[i].ZoneNumber;
                ZoneProcessingWS.Cells[rc, 4].Value = ZoneProcessings[i].IsPickFace;
                ZoneProcessingWS.Cells[rc, 5].Value = ZoneProcessings[i].IsReserve;
                ZoneProcessingWS.Cells[rc, 6].Value = ZoneProcessings[i].IsCartonConsolidation;

                ZoneProcessingWS.Cells[rc, 7].Value = ZoneProcessings[i].CreatedDateTime.ToOADate();
                ZoneProcessingWS.Cells[rc, 8].Value = ZoneProcessings[i].CreatedMethod;

                if (users.Any(x => x.ID == ZoneProcessings[i].CreatedUserID))
                {
                    ZoneProcessingWS.Cells[rc, 9].Value = users.FirstOrDefault(x => x.ID == ZoneProcessings[i].CreatedUserID).PublicID;
                    ZoneProcessingWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == ZoneProcessings[i].CreatedUserID).Firstname;
                }
                else
                    ZoneProcessingWS.Cells[rc, 9].Value = "UserID: " + ZoneProcessings[i].CreatedUserID.ToString();

                ZoneProcessingWS.Cells[rc, 11].Value = ZoneProcessings[i].LastAmendedDateTime.ToOADate();
                ZoneProcessingWS.Cells[rc, 12].Value = ZoneProcessings[i].LastAmendedMethod;

                if (users.Any(x => x.ID == ZoneProcessings[i].LastAmendedUserID))
                {
                    ZoneProcessingWS.Cells[rc, 13].Value = users.FirstOrDefault(x => x.ID == ZoneProcessings[i].LastAmendedUserID).PublicID;
                    ZoneProcessingWS.Cells[rc, 14].Value = users.FirstOrDefault(x => x.ID == ZoneProcessings[i].LastAmendedUserID).Firstname;
                }
                else
                    ZoneProcessingWS.Cells[rc, 13].Value = "UserID: " + ZoneProcessings[i].LastAmendedUserID.ToString();

                ZoneProcessingWS.Cells[rc, 15].Value = ZoneProcessings[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
