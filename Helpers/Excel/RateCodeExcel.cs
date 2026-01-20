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
    public class RateCodeExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.RateCode.dbRow> RateCodes = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.RateCode.Search RateCodeSearchData;
        public RateCodeExcel(wbm_common.DataObjects.RateCode.Search RateCodeSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.RateCodeSearchData = RateCodeSearchData;
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
                        RateCodePage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("RateCodeExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.RateCode.dbRow>> rc = await RateCodeHelper.ExecuteSearch(RateCodeSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            RateCodes = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = RateCodes.Select(x => x.CreatedUserID).Union(RateCodes.Select(x => x.LastAmendedUserID)).Distinct().ToList();


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

        private void RateCodePage(ExcelPackage ep)
        {
            ExcelWorksheet RateCodeWS = ep.Workbook.Worksheets.Add("RateCode");

            #region header
            RateCodeWS.View.FreezePanes(2, 1);
            RateCodeWS.Cells[1, 1, 1, 19].Style.Font.Bold = true;
            RateCodeWS.Cells[1, 1, 1, 19].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            RateCodeWS.Column(1).Width = 15;
            RateCodeWS.Column(2).Width = 15;
            RateCodeWS.Column(3).Width = 10;
            RateCodeWS.Column(4).Width = 25;
            RateCodeWS.Column(5).Width = 25;
            RateCodeWS.Column(6).Width = 25;
            RateCodeWS.Column(7).Width = 25;
            RateCodeWS.Column(7).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            RateCodeWS.Column(8).Width = 25;
            RateCodeWS.Column(9).Width = 25;
            RateCodeWS.Column(10).Width = 25;
            RateCodeWS.Column(11).Width = 25;
            RateCodeWS.Column(11).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            RateCodeWS.Column(12).Width = 25;
            RateCodeWS.Column(13).Width = 25;
            RateCodeWS.Column(14).Width = 25;
            RateCodeWS.Column(15).Width = 25;
            RateCodeWS.Column(15).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            RateCodeWS.Column(16).Width = 25;
            RateCodeWS.Column(17).Width = 25;
            RateCodeWS.Column(18).Width = 25;
            RateCodeWS.Column(19).Width = 15;

            RateCodeWS.Cells[1, 1].Value = "Rate Code";
            RateCodeWS.Cells[1, 2].Value = "Description";
            RateCodeWS.Cells[1, 3].Value = "Rate Type";
            RateCodeWS.Cells[1, 4].Value = "Minimum Charge";
            RateCodeWS.Cells[1, 5].Value = "Basic";
            RateCodeWS.Cells[1, 6].Value = "Included Units";
            RateCodeWS.Cells[1, 7].Value = "Date Of Change";
            RateCodeWS.Cells[1, 8].Value = "New Minimum Charge";
            RateCodeWS.Cells[1, 9].Value = "New Basic";
            RateCodeWS.Cells[1, 10].Value = "New Included Units";
            RateCodeWS.Cells[1, 11].Value = "Created DateTime";
            RateCodeWS.Cells[1, 12].Value = "Created Method";
            RateCodeWS.Cells[1, 13].Value = "Created User";
            RateCodeWS.Cells[1, 14].Value = "Created User Name";
            RateCodeWS.Cells[1, 15].Value = "Last Amended DateTime";
            RateCodeWS.Cells[1, 16].Value = "Last Amended Method";
            RateCodeWS.Cells[1, 17].Value = "Last Amended User";
            RateCodeWS.Cells[1, 18].Value = "Last Amended Name";
            RateCodeWS.Cells[1, 19].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < RateCodes.Count(); i++)
            {
                RateCodeWS.Cells[rc, 1].Value = RateCodes[i].RateCode;
                RateCodeWS.Cells[rc, 2].Value = RateCodes[i].Description;
                RateCodeWS.Cells[rc, 3].Value = RateCodes[i].RateType;
                RateCodeWS.Cells[rc, 4].Value = RateCodes[i].MinimumCharge;
                RateCodeWS.Cells[rc, 5].Value = RateCodes[i].Basic;
                RateCodeWS.Cells[rc, 6].Value = RateCodes[i].IncludedUnits;
                RateCodeWS.Cells[rc, 7].Value = RateCodes[i].DateOfChange;
                RateCodeWS.Cells[rc, 8].Value = RateCodes[i].NewMinimumCharge;
                RateCodeWS.Cells[rc, 9].Value = RateCodes[i].NewBasic;
                RateCodeWS.Cells[rc, 10].Value = RateCodes[i].NewIncludedUnits;

                RateCodeWS.Cells[rc, 11].Value = RateCodes[i].CreatedDateTime.ToOADate();
                RateCodeWS.Cells[rc, 12].Value = RateCodes[i].CreatedMethod;

                if (users.Any(x => x.ID == RateCodes[i].CreatedUserID))
                {
                    RateCodeWS.Cells[rc, 13].Value = users.FirstOrDefault(x => x.ID == RateCodes[i].CreatedUserID).PublicID;
                    RateCodeWS.Cells[rc, 14].Value = users.FirstOrDefault(x => x.ID == RateCodes[i].CreatedUserID).Firstname;
                }
                else
                    RateCodeWS.Cells[rc, 13].Value = "UserID: " + RateCodes[i].CreatedUserID.ToString();

                RateCodeWS.Cells[rc, 15].Value = RateCodes[i].LastAmendedDateTime.ToOADate();
                RateCodeWS.Cells[rc, 16].Value = RateCodes[i].LastAmendedMethod;

                if (users.Any(x => x.ID == RateCodes[i].LastAmendedUserID))
                {
                    RateCodeWS.Cells[rc, 17].Value = users.FirstOrDefault(x => x.ID == RateCodes[i].LastAmendedUserID).PublicID;
                    RateCodeWS.Cells[rc, 18].Value = users.FirstOrDefault(x => x.ID == RateCodes[i].LastAmendedUserID).Firstname;
                }
                else
                    RateCodeWS.Cells[rc, 17].Value = "UserID: " + RateCodes[i].LastAmendedUserID.ToString();

                RateCodeWS.Cells[rc, 19].Value = RateCodes[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
