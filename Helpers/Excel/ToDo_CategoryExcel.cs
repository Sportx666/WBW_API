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
    public class ToDo_CategoryExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.ToDo_Category.dbRow> ToDo_Categorys = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.ToDo_Category.Search ToDo_CategorySearchData;
        public ToDo_CategoryExcel(wbm_common.DataObjects.ToDo_Category.Search ToDo_CategorySearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.ToDo_CategorySearchData = ToDo_CategorySearchData;
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
                        ToDo_CategoryPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("ToDo_CategoryExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.ToDo_Category.dbRow>> rc = await ToDo_CategoryHelper.ExecuteSearch(ToDo_CategorySearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            ToDo_Categorys = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = ToDo_Categorys.Select(x => x.CreatedUserID).Union(ToDo_Categorys.Select(x => x.LastAmendedUserID)).Distinct().ToList();


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

        private void ToDo_CategoryPage(ExcelPackage ep)
        {
            ExcelWorksheet ToDo_CategoryWS = ep.Workbook.Worksheets.Add("ToDo_Category");

            #region header
            ToDo_CategoryWS.View.FreezePanes(2, 1);
            ToDo_CategoryWS.Cells[1, 1, 1, 12].Style.Font.Bold = true;
            ToDo_CategoryWS.Cells[1, 1, 1, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            ToDo_CategoryWS.Column(1).Width = 15;
            ToDo_CategoryWS.Column(2).Width = 15;
            ToDo_CategoryWS.Column(3).Width = 10;
            ToDo_CategoryWS.Column(4).Width = 25;
            ToDo_CategoryWS.Column(4).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ToDo_CategoryWS.Column(5).Width = 25;
            ToDo_CategoryWS.Column(6).Width = 25;
            ToDo_CategoryWS.Column(7).Width = 25;
            ToDo_CategoryWS.Column(8).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ToDo_CategoryWS.Column(8).Width = 25;
            ToDo_CategoryWS.Column(9).Width = 25;
            ToDo_CategoryWS.Column(10).Width = 25;
            ToDo_CategoryWS.Column(11).Width = 25;
            ToDo_CategoryWS.Column(12).Width = 15;

            ToDo_CategoryWS.Cells[1, 1].Value = "ID";
            ToDo_CategoryWS.Cells[1, 2].Value = "Description";
            ToDo_CategoryWS.Cells[1, 3].Value = "Sort Order";
            ToDo_CategoryWS.Cells[1, 4].Value = "Created DateTime";
            ToDo_CategoryWS.Cells[1, 5].Value = "Created Method";
            ToDo_CategoryWS.Cells[1, 6].Value = "Created User";
            ToDo_CategoryWS.Cells[1, 7].Value = "Created User Name";
            ToDo_CategoryWS.Cells[1, 8].Value = "Last Amended DateTime";
            ToDo_CategoryWS.Cells[1, 9].Value = "Last Amended Method";
            ToDo_CategoryWS.Cells[1, 10].Value = "Last Amended User";
            ToDo_CategoryWS.Cells[1, 11].Value = "Last Amended Name";
            ToDo_CategoryWS.Cells[1, 12].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            ToDo_Categorys = ToDo_Categorys.OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < ToDo_Categorys.Count(); i++)
            {
                ToDo_CategoryWS.Cells[rc, 1].Value = ToDo_Categorys[i].PublicID;
                ToDo_CategoryWS.Cells[rc, 2].Value = ToDo_Categorys[i].Description;
                ToDo_CategoryWS.Cells[rc, 3].Value = ToDo_Categorys[i].SortOrder;
                ToDo_CategoryWS.Cells[rc, 4].Value = ToDo_Categorys[i].CreatedDateTime.ToOADate();
                ToDo_CategoryWS.Cells[rc, 5].Value = ToDo_Categorys[i].CreatedMethod;

                if (users.Any(x => x.ID == ToDo_Categorys[i].CreatedUserID))
                {
                    ToDo_CategoryWS.Cells[rc, 6].Value = users.FirstOrDefault(x => x.ID == ToDo_Categorys[i].CreatedUserID).PublicID;
                    ToDo_CategoryWS.Cells[rc, 7].Value = users.FirstOrDefault(x => x.ID == ToDo_Categorys[i].CreatedUserID).Firstname;
                }
                else
                    ToDo_CategoryWS.Cells[rc, 6].Value = "UserID: " + ToDo_Categorys[i].CreatedUserID.ToString();

                ToDo_CategoryWS.Cells[rc, 8].Value = ToDo_Categorys[i].LastAmendedDateTime.ToOADate();
                ToDo_CategoryWS.Cells[rc, 9].Value = ToDo_Categorys[i].LastAmendedMethod;

                if (users.Any(x => x.ID == ToDo_Categorys[i].LastAmendedUserID))
                {
                    ToDo_CategoryWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == ToDo_Categorys[i].LastAmendedUserID).PublicID;
                    ToDo_CategoryWS.Cells[rc, 11].Value = users.FirstOrDefault(x => x.ID == ToDo_Categorys[i].LastAmendedUserID).Firstname;
                }
                else
                    ToDo_CategoryWS.Cells[rc, 10].Value = "UserID: " + ToDo_Categorys[i].LastAmendedUserID.ToString();

                ToDo_CategoryWS.Cells[rc, 12].Value = ToDo_Categorys[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
