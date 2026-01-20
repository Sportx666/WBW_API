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
    public class LongRunProcessStatusDescriptionExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow> LongRunProcessStatusDescriptions = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.LongRunProcessStatusDescription.Search LongRunProcessStatusDescriptionSearchData;
        public LongRunProcessStatusDescriptionExcel(wbm_common.DataObjects.LongRunProcessStatusDescription.Search LongRunProcessStatusDescriptionSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.LongRunProcessStatusDescriptionSearchData = LongRunProcessStatusDescriptionSearchData;
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
                        LongRunProcessStatusDescriptionPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("LongRunProcessStatusDescriptionExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.LongRunProcessStatusDescription.dbRow>> rc = await LongRunProcessStatusDescriptionHelper.ExecuteSearch(LongRunProcessStatusDescriptionSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            LongRunProcessStatusDescriptions = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = LongRunProcessStatusDescriptions.Select(x => x.CreatedUserID).Union(LongRunProcessStatusDescriptions.Select(x => x.LastAmendedUserID)).Distinct().ToList();


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

        private void LongRunProcessStatusDescriptionPage(ExcelPackage ep)
        {
            ExcelWorksheet LongRunProcessStatusDescriptionWS = ep.Workbook.Worksheets.Add("LongRunProcessStatusDescription");

            #region header
            LongRunProcessStatusDescriptionWS.View.FreezePanes(2, 1);
            LongRunProcessStatusDescriptionWS.Cells[1, 1, 1, 11].Style.Font.Bold = true;
            LongRunProcessStatusDescriptionWS.Cells[1, 1, 1, 11].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            LongRunProcessStatusDescriptionWS.Column(1).Width = 15;
            LongRunProcessStatusDescriptionWS.Column(2).Width = 15;
            LongRunProcessStatusDescriptionWS.Column(3).Width = 25;
            LongRunProcessStatusDescriptionWS.Column(3).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            LongRunProcessStatusDescriptionWS.Column(4).Width = 25;
            LongRunProcessStatusDescriptionWS.Column(5).Width = 25;
            LongRunProcessStatusDescriptionWS.Column(6).Width = 25;
            LongRunProcessStatusDescriptionWS.Column(7).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            LongRunProcessStatusDescriptionWS.Column(7).Width = 25;
            LongRunProcessStatusDescriptionWS.Column(8).Width = 25;
            LongRunProcessStatusDescriptionWS.Column(9).Width = 25;
            LongRunProcessStatusDescriptionWS.Column(10).Width = 25;
            LongRunProcessStatusDescriptionWS.Column(11).Width = 15;

            LongRunProcessStatusDescriptionWS.Cells[1, 1].Value = "ID";
            LongRunProcessStatusDescriptionWS.Cells[1, 2].Value = "Description";
            LongRunProcessStatusDescriptionWS.Cells[1, 3].Value = "Created DateTime";
            LongRunProcessStatusDescriptionWS.Cells[1, 4].Value = "Created Method";
            LongRunProcessStatusDescriptionWS.Cells[1, 5].Value = "Created User";
            LongRunProcessStatusDescriptionWS.Cells[1, 6].Value = "Created User Name";
            LongRunProcessStatusDescriptionWS.Cells[1, 7].Value = "Last Amended DateTime";
            LongRunProcessStatusDescriptionWS.Cells[1, 8].Value = "Last Amended Method";
            LongRunProcessStatusDescriptionWS.Cells[1, 9].Value = "Last Amended User";
            LongRunProcessStatusDescriptionWS.Cells[1, 10].Value = "Last Amended Name";
            LongRunProcessStatusDescriptionWS.Cells[1, 11].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < LongRunProcessStatusDescriptions.Count(); i++)
            {
                LongRunProcessStatusDescriptionWS.Cells[rc, 1].Value = LongRunProcessStatusDescriptions[i].ID;
                LongRunProcessStatusDescriptionWS.Cells[rc, 2].Value = LongRunProcessStatusDescriptions[i].Description;
                LongRunProcessStatusDescriptionWS.Cells[rc, 3].Value = LongRunProcessStatusDescriptions[i].CreatedDateTime.ToOADate();
                LongRunProcessStatusDescriptionWS.Cells[rc, 4].Value = LongRunProcessStatusDescriptions[i].CreatedMethod;

                if (users.Any(x => x.ID == LongRunProcessStatusDescriptions[i].CreatedUserID))
                {
                    LongRunProcessStatusDescriptionWS.Cells[rc, 5].Value = users.FirstOrDefault(x => x.ID == LongRunProcessStatusDescriptions[i].CreatedUserID).PublicID;
                    LongRunProcessStatusDescriptionWS.Cells[rc, 6].Value = users.FirstOrDefault(x => x.ID == LongRunProcessStatusDescriptions[i].CreatedUserID).Firstname;
                }
                else
                    LongRunProcessStatusDescriptionWS.Cells[rc, 5].Value = "UserID: " + LongRunProcessStatusDescriptions[i].CreatedUserID.ToString();

                LongRunProcessStatusDescriptionWS.Cells[rc, 7].Value = LongRunProcessStatusDescriptions[i].LastAmendedDateTime.ToOADate();
                LongRunProcessStatusDescriptionWS.Cells[rc, 8].Value = LongRunProcessStatusDescriptions[i].LastAmendedMethod;

                if (users.Any(x => x.ID == LongRunProcessStatusDescriptions[i].LastAmendedUserID))
                {
                    LongRunProcessStatusDescriptionWS.Cells[rc, 9].Value = users.FirstOrDefault(x => x.ID == LongRunProcessStatusDescriptions[i].LastAmendedUserID).PublicID;
                    LongRunProcessStatusDescriptionWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == LongRunProcessStatusDescriptions[i].LastAmendedUserID).Firstname;
                }
                else
                    LongRunProcessStatusDescriptionWS.Cells[rc, 9].Value = "UserID: " + LongRunProcessStatusDescriptions[i].LastAmendedUserID.ToString();

                LongRunProcessStatusDescriptionWS.Cells[rc, 11].Value = LongRunProcessStatusDescriptions[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
