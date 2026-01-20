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
    public class UnitTypeExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.UnitType.dbRow> UnitTypes = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.UnitType.Search UnitTypeSearchData;
        public UnitTypeExcel(wbm_common.DataObjects.UnitType.Search UnitTypeSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.UnitTypeSearchData = UnitTypeSearchData;
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
                        UnitTypePage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("UnitTypeExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.UnitType.dbRow>> rc = await UnitTypeHelper.ExecuteSearch(UnitTypeSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            UnitTypes = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = UnitTypes.Select(x => x.CreatedUserID).Union(UnitTypes.Select(x => x.LastAmendedUserID)).Distinct().ToList();


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

        private void UnitTypePage(ExcelPackage ep)
        {
            ExcelWorksheet UnitTypeWS = ep.Workbook.Worksheets.Add("UnitType");

            #region header
            UnitTypeWS.View.FreezePanes(2, 1);
            UnitTypeWS.Cells[1, 1, 1, 12].Style.Font.Bold = true;
            UnitTypeWS.Cells[1, 1, 1, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            UnitTypeWS.Column(1).Width = 15;
            UnitTypeWS.Column(2).Width = 15;
            UnitTypeWS.Column(3).Width = 10;
            UnitTypeWS.Column(4).Width = 25;
            UnitTypeWS.Column(4).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            UnitTypeWS.Column(5).Width = 25;
            UnitTypeWS.Column(6).Width = 25;
            UnitTypeWS.Column(7).Width = 25;
            UnitTypeWS.Column(8).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            UnitTypeWS.Column(8).Width = 25;
            UnitTypeWS.Column(9).Width = 25;
            UnitTypeWS.Column(10).Width = 25;
            UnitTypeWS.Column(11).Width = 25;
            UnitTypeWS.Column(12).Width = 15;

            UnitTypeWS.Cells[1, 1].Value = "ID";
            UnitTypeWS.Cells[1, 2].Value = "Description";
            UnitTypeWS.Cells[1, 3].Value = "Sort Order";
            UnitTypeWS.Cells[1, 4].Value = "Created DateTime";
            UnitTypeWS.Cells[1, 5].Value = "Created Method";
            UnitTypeWS.Cells[1, 6].Value = "Created User";
            UnitTypeWS.Cells[1, 7].Value = "Created User Name";
            UnitTypeWS.Cells[1, 8].Value = "Last Amended DateTime";
            UnitTypeWS.Cells[1, 9].Value = "Last Amended Method";
            UnitTypeWS.Cells[1, 10].Value = "Last Amended User";
            UnitTypeWS.Cells[1, 11].Value = "Last Amended Name";
            UnitTypeWS.Cells[1, 12].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            UnitTypes = UnitTypes.OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < UnitTypes.Count(); i++)
            {
                UnitTypeWS.Cells[rc, 1].Value = UnitTypes[i].PublicID;
                UnitTypeWS.Cells[rc, 2].Value = UnitTypes[i].Description;
                UnitTypeWS.Cells[rc, 3].Value = UnitTypes[i].SortOrder;
                UnitTypeWS.Cells[rc, 4].Value = UnitTypes[i].CreatedDateTime.ToOADate();
                UnitTypeWS.Cells[rc, 5].Value = UnitTypes[i].CreatedMethod;

                if (users.Any(x => x.ID == UnitTypes[i].CreatedUserID))
                {
                    UnitTypeWS.Cells[rc, 6].Value = users.FirstOrDefault(x => x.ID == UnitTypes[i].CreatedUserID).PublicID;
                    UnitTypeWS.Cells[rc, 7].Value = users.FirstOrDefault(x => x.ID == UnitTypes[i].CreatedUserID).Firstname;
                }
                else
                    UnitTypeWS.Cells[rc, 6].Value = "UserID: " + UnitTypes[i].CreatedUserID.ToString();

                UnitTypeWS.Cells[rc, 8].Value = UnitTypes[i].LastAmendedDateTime.ToOADate();
                UnitTypeWS.Cells[rc, 9].Value = UnitTypes[i].LastAmendedMethod;

                if (users.Any(x => x.ID == UnitTypes[i].LastAmendedUserID))
                {
                    UnitTypeWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == UnitTypes[i].LastAmendedUserID).PublicID;
                    UnitTypeWS.Cells[rc, 11].Value = users.FirstOrDefault(x => x.ID == UnitTypes[i].LastAmendedUserID).Firstname;
                }
                else
                    UnitTypeWS.Cells[rc, 10].Value = "UserID: " + UnitTypes[i].LastAmendedUserID.ToString();

                UnitTypeWS.Cells[rc, 12].Value = UnitTypes[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
