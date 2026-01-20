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
    public class ActionTypeExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.ActionType.dbRow> ActionTypes = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.ActionType.Search ActionTypeSearchData;
        public ActionTypeExcel(wbm_common.DataObjects.ActionType.Search ActionTypeSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.ActionTypeSearchData = ActionTypeSearchData;
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
                        ActionTypePage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("ActionTypeExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.ActionType.dbRow>> rc = await ActionTypeHelper.ExecuteSearch(ActionTypeSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            ActionTypes = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = ActionTypes.Select(x => x.CreatedUserID).Union(ActionTypes.Select(x => x.LastAmendedUserID)).Distinct().ToList();

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

        private void ActionTypePage(ExcelPackage ep)
        {
            ExcelWorksheet ActionTypeWS = ep.Workbook.Worksheets.Add("ActionType");

            #region header
            ActionTypeWS.View.FreezePanes(2, 1);
            ActionTypeWS.Cells[1, 1, 1, 12].Style.Font.Bold = true;
            ActionTypeWS.Cells[1, 1, 1, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            ActionTypeWS.Column(1).Width = 15;
            ActionTypeWS.Column(2).Width = 15;
            ActionTypeWS.Column(3).Width = 11;
            ActionTypeWS.Column(4).Width = 25;
            ActionTypeWS.Column(4).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ActionTypeWS.Column(5).Width = 20;
            ActionTypeWS.Column(6).Width = 25;
            ActionTypeWS.Column(7).Width = 25;
            ActionTypeWS.Column(8).Width = 25;
            ActionTypeWS.Column(8).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ActionTypeWS.Column(9).Width = 25;
            ActionTypeWS.Column(10).Width = 25;
            ActionTypeWS.Column(11).Width = 25;
            ActionTypeWS.Column(12).Width = 25;

            ActionTypeWS.Cells[1, 1].Value = "ID";
            ActionTypeWS.Cells[1, 2].Value = "Description";
            ActionTypeWS.Cells[1, 3].Value = "Sort Order";
            ActionTypeWS.Cells[1, 4].Value = "Created DateTime";
            ActionTypeWS.Cells[1, 5].Value = "Created Method";
            ActionTypeWS.Cells[1, 6].Value = "Created User";
            ActionTypeWS.Cells[1, 7].Value = "Created User Name";
            ActionTypeWS.Cells[1, 8].Value = "Last Amended DateTime";
            ActionTypeWS.Cells[1, 9].Value = "Last Amended Method";
            ActionTypeWS.Cells[1, 10].Value = "Last Amended User";
            ActionTypeWS.Cells[1, 11].Value = "Last Amended User Name";
            ActionTypeWS.Cells[1, 12].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            ActionTypes = ActionTypes.OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < ActionTypes.Count(); i++)
            {
                ActionTypeWS.Cells[rc, 1].Value = ActionTypes[i].PublicID;
                ActionTypeWS.Cells[rc, 2].Value = ActionTypes[i].Description;
                ActionTypeWS.Cells[rc, 3].Value = ActionTypes[i].SortOrder;
                ActionTypeWS.Cells[rc, 4].Value = ActionTypes[i].CreatedDateTime.ToOADate();
                ActionTypeWS.Cells[rc, 5].Value = ActionTypes[i].CreatedMethod;
                if (users.Any(x => x.ID == ActionTypes[i].CreatedUserID))
                {
                    ActionTypeWS.Cells[rc, 6].Value = users.FirstOrDefault(x => x.ID == ActionTypes[i].CreatedUserID).Firstname;
                    ActionTypeWS.Cells[rc, 7].Value = users.FirstOrDefault(x => x.ID == ActionTypes[i].CreatedUserID).PublicID;
                }
                else
                    ActionTypeWS.Cells[rc, 6].Value = "UserID: " + ActionTypes[i].CreatedUserID.ToString();
                ActionTypeWS.Cells[rc, 9].Value = ActionTypes[i].LastAmendedDateTime.ToOADate();
                ActionTypeWS.Cells[rc, 10].Value = ActionTypes[i].LastAmendedMethod;
                if (users.Any(x => x.ID == ActionTypes[i].LastAmendedUserID))
                {
                    ActionTypeWS.Cells[rc, 11].Value = users.FirstOrDefault(x => x.ID == ActionTypes[i].LastAmendedUserID).Firstname;
                    ActionTypeWS.Cells[rc, 12].Value = users.FirstOrDefault(x => x.ID == ActionTypes[i].LastAmendedUserID).PublicID;
                }
                else
                    ActionTypeWS.Cells[rc, 11].Value = "UserID: " + ActionTypes[i].LastAmendedUserID.ToString();
                ActionTypeWS.Cells[rc, 12].Value = ActionTypes[i].DeletedFlag;
                rc++;
            }
            #endregion
        }
    }
}
