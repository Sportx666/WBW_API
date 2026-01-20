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
    public class ToDoStatus_TypeExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.ToDoStatus_Type.dbRow> ToDoStatus_Types = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.ToDoStatus_Type.Search ToDoStatus_TypeSearchData;
        public ToDoStatus_TypeExcel(wbm_common.DataObjects.ToDoStatus_Type.Search ToDoStatus_TypeSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.ToDoStatus_TypeSearchData = ToDoStatus_TypeSearchData;
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
                        ToDoStatus_TypePage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("ToDoStatus_TypeExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.ToDoStatus_Type.dbRow>> rc = await ToDoStatus_TypeHelper.ExecuteSearch(ToDoStatus_TypeSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            ToDoStatus_Types = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = ToDoStatus_Types.Select(x => x.CreatedUserID).Union(ToDoStatus_Types.Select(x => x.LastAmendedUserID)).Distinct().ToList();


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

        private void ToDoStatus_TypePage(ExcelPackage ep)
        {
            ExcelWorksheet ToDoStatus_TypeWS = ep.Workbook.Worksheets.Add("ToDoStatus_Type");

            #region header
            ToDoStatus_TypeWS.View.FreezePanes(2, 1);
            ToDoStatus_TypeWS.Cells[1, 1, 1, 12].Style.Font.Bold = true;
            ToDoStatus_TypeWS.Cells[1, 1, 1, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            ToDoStatus_TypeWS.Column(1).Width = 15;
            ToDoStatus_TypeWS.Column(2).Width = 15;
            ToDoStatus_TypeWS.Column(3).Width = 10;
            ToDoStatus_TypeWS.Column(4).Width = 25;
            ToDoStatus_TypeWS.Column(4).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ToDoStatus_TypeWS.Column(5).Width = 25;
            ToDoStatus_TypeWS.Column(6).Width = 25;
            ToDoStatus_TypeWS.Column(7).Width = 25;
            ToDoStatus_TypeWS.Column(8).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ToDoStatus_TypeWS.Column(8).Width = 25;
            ToDoStatus_TypeWS.Column(9).Width = 25;
            ToDoStatus_TypeWS.Column(10).Width = 25;
            ToDoStatus_TypeWS.Column(11).Width = 25;
            ToDoStatus_TypeWS.Column(12).Width = 15;

            ToDoStatus_TypeWS.Cells[1, 1].Value = "ID";
            ToDoStatus_TypeWS.Cells[1, 2].Value = "Description";
            ToDoStatus_TypeWS.Cells[1, 3].Value = "Sort Order";
            ToDoStatus_TypeWS.Cells[1, 4].Value = "Created DateTime";
            ToDoStatus_TypeWS.Cells[1, 5].Value = "Created Method";
            ToDoStatus_TypeWS.Cells[1, 6].Value = "Created User";
            ToDoStatus_TypeWS.Cells[1, 7].Value = "Created User Name";
            ToDoStatus_TypeWS.Cells[1, 8].Value = "Last Amended DateTime";
            ToDoStatus_TypeWS.Cells[1, 9].Value = "Last Amended Method";
            ToDoStatus_TypeWS.Cells[1, 10].Value = "Last Amended User";
            ToDoStatus_TypeWS.Cells[1, 11].Value = "Last Amended Name";
            ToDoStatus_TypeWS.Cells[1, 12].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            ToDoStatus_Types = ToDoStatus_Types.OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < ToDoStatus_Types.Count(); i++)
            {
                ToDoStatus_TypeWS.Cells[rc, 1].Value = ToDoStatus_Types[i].PublicID;
                ToDoStatus_TypeWS.Cells[rc, 2].Value = ToDoStatus_Types[i].Description;
                ToDoStatus_TypeWS.Cells[rc, 3].Value = ToDoStatus_Types[i].SortOrder;
                ToDoStatus_TypeWS.Cells[rc, 4].Value = ToDoStatus_Types[i].CreatedDateTime.ToOADate();
                ToDoStatus_TypeWS.Cells[rc, 5].Value = ToDoStatus_Types[i].CreatedMethod;

                if (users.Any(x => x.ID == ToDoStatus_Types[i].CreatedUserID))
                {
                    ToDoStatus_TypeWS.Cells[rc, 6].Value = users.FirstOrDefault(x => x.ID == ToDoStatus_Types[i].CreatedUserID).PublicID;
                    ToDoStatus_TypeWS.Cells[rc, 7].Value = users.FirstOrDefault(x => x.ID == ToDoStatus_Types[i].CreatedUserID).Firstname;
                }
                else
                    ToDoStatus_TypeWS.Cells[rc, 6].Value = "UserID: " + ToDoStatus_Types[i].CreatedUserID.ToString();

                ToDoStatus_TypeWS.Cells[rc, 8].Value = ToDoStatus_Types[i].LastAmendedDateTime.ToOADate();
                ToDoStatus_TypeWS.Cells[rc, 9].Value = ToDoStatus_Types[i].LastAmendedMethod;

                if (users.Any(x => x.ID == ToDoStatus_Types[i].LastAmendedUserID))
                {
                    ToDoStatus_TypeWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == ToDoStatus_Types[i].LastAmendedUserID).PublicID;
                    ToDoStatus_TypeWS.Cells[rc, 11].Value = users.FirstOrDefault(x => x.ID == ToDoStatus_Types[i].LastAmendedUserID).Firstname;
                }
                else
                    ToDoStatus_TypeWS.Cells[rc, 10].Value = "UserID: " + ToDoStatus_Types[i].LastAmendedUserID.ToString();

                ToDoStatus_TypeWS.Cells[rc, 12].Value = ToDoStatus_Types[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
