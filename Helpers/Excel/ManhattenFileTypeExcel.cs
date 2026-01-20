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
    public class ManhattenFileTypeExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.ManhattenFileType.dbRow> ManhattenFileTypes = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.ManhattenFileType.Search ManhattenFileTypeSearchData;
        public ManhattenFileTypeExcel(wbm_common.DataObjects.ManhattenFileType.Search ManhattenFileTypeSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.ManhattenFileTypeSearchData = ManhattenFileTypeSearchData;
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
                        ManhattenFileTypePage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("ManhattenFileTypeExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.ManhattenFileType.dbRow>> rc = await ManhattenFileTypeHelper.ExecuteSearch(ManhattenFileTypeSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            ManhattenFileTypes = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = ManhattenFileTypes.Select(x => x.CreatedUserID).Union(ManhattenFileTypes.Select(x => x.LastAmendedUserID)).Distinct().ToList();


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

        private void ManhattenFileTypePage(ExcelPackage ep)
        {
            ExcelWorksheet ManhattenFileTypeWS = ep.Workbook.Worksheets.Add("ManhattenFileType");

            #region header
            ManhattenFileTypeWS.View.FreezePanes(2, 1);
            ManhattenFileTypeWS.Cells[1, 1, 1, 12].Style.Font.Bold = true;
            ManhattenFileTypeWS.Cells[1, 1, 1, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            ManhattenFileTypeWS.Column(1).Width = 15;
            ManhattenFileTypeWS.Column(2).Width = 15;
            ManhattenFileTypeWS.Column(3).Width = 10;
            ManhattenFileTypeWS.Column(4).Width = 25;
            ManhattenFileTypeWS.Column(4).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ManhattenFileTypeWS.Column(5).Width = 25;
            ManhattenFileTypeWS.Column(6).Width = 25;
            ManhattenFileTypeWS.Column(7).Width = 25;
            ManhattenFileTypeWS.Column(8).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ManhattenFileTypeWS.Column(8).Width = 25;
            ManhattenFileTypeWS.Column(9).Width = 25;
            ManhattenFileTypeWS.Column(10).Width = 25;
            ManhattenFileTypeWS.Column(11).Width = 25;
            ManhattenFileTypeWS.Column(12).Width = 25;

            ManhattenFileTypeWS.Cells[1, 1].Value = "ID";
            ManhattenFileTypeWS.Cells[1, 2].Value = "Description";
            ManhattenFileTypeWS.Cells[1, 3].Value = "Sort Order";
            ManhattenFileTypeWS.Cells[1, 4].Value = "Created DateTime";
            ManhattenFileTypeWS.Cells[1, 5].Value = "Created Method";
            ManhattenFileTypeWS.Cells[1, 6].Value = "Created User";
            ManhattenFileTypeWS.Cells[1, 7].Value = "Created User Name";
            ManhattenFileTypeWS.Cells[1, 8].Value = "Last Amended DateTime";
            ManhattenFileTypeWS.Cells[1, 9].Value = "Last Amended Method";
            ManhattenFileTypeWS.Cells[1, 10].Value = "Last Amended User";
            ManhattenFileTypeWS.Cells[1, 11].Value = "Last Amended Name";
            ManhattenFileTypeWS.Cells[1, 12].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            ManhattenFileTypes = ManhattenFileTypes.OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < ManhattenFileTypes.Count(); i++)
            {
                ManhattenFileTypeWS.Cells[rc, 1].Value = ManhattenFileTypes[i].PublicID;
                ManhattenFileTypeWS.Cells[rc, 2].Value = ManhattenFileTypes[i].Description;
                ManhattenFileTypeWS.Cells[rc, 3].Value = ManhattenFileTypes[i].SortOrder;
                ManhattenFileTypeWS.Cells[rc, 4].Value = ManhattenFileTypes[i].CreatedDateTime.ToOADate();
                ManhattenFileTypeWS.Cells[rc, 5].Value = ManhattenFileTypes[i].CreatedMethod;

                if (users.Any(x => x.ID == ManhattenFileTypes[i].CreatedUserID))
                {
                    ManhattenFileTypeWS.Cells[rc, 6].Value = users.FirstOrDefault(x => x.ID == ManhattenFileTypes[i].CreatedUserID).PublicID;
                    ManhattenFileTypeWS.Cells[rc, 7].Value = users.FirstOrDefault(x => x.ID == ManhattenFileTypes[i].CreatedUserID).Firstname;
                }
                else
                    ManhattenFileTypeWS.Cells[rc, 6].Value = "UserID: " + ManhattenFileTypes[i].CreatedUserID.ToString();

                ManhattenFileTypeWS.Cells[rc, 8].Value = ManhattenFileTypes[i].LastAmendedDateTime.ToOADate();
                ManhattenFileTypeWS.Cells[rc, 9].Value = ManhattenFileTypes[i].LastAmendedMethod;

                if (users.Any(x => x.ID == ManhattenFileTypes[i].LastAmendedUserID))
                {
                    ManhattenFileTypeWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == ManhattenFileTypes[i].LastAmendedUserID).PublicID;
                    ManhattenFileTypeWS.Cells[rc, 11].Value = users.FirstOrDefault(x => x.ID == ManhattenFileTypes[i].LastAmendedUserID).Firstname;
                }
                else
                    ManhattenFileTypeWS.Cells[rc, 10].Value = "UserID: " + ManhattenFileTypes[i].LastAmendedUserID.ToString();

                ManhattenFileTypeWS.Cells[rc, 12].Value = ManhattenFileTypes[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
