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
    public class Security_ItemCategoryExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.Security_ItemCategory.dbRow> Security_ItemCategorys = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.Security_ItemCategory.Search Security_ItemCategorySearchData;
        public Security_ItemCategoryExcel(wbm_common.DataObjects.Security_ItemCategory.Search Security_ItemCategorySearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.Security_ItemCategorySearchData = Security_ItemCategorySearchData;
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
                        Security_ItemCategoryPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("Security_ItemCategoryExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.Security_ItemCategory.dbRow>> rc = await Security_ItemCategoryHelper.ExecuteSearch(Security_ItemCategorySearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            Security_ItemCategorys = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = Security_ItemCategorys.Select(x => x.CreatedUserID).Union(Security_ItemCategorys.Select(x => x.LastAmendedUserID)).Distinct().ToList();


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

        private void Security_ItemCategoryPage(ExcelPackage ep)
        {
            ExcelWorksheet Security_ItemCategoryWS = ep.Workbook.Worksheets.Add("Security_ItemCategory");

            #region header
            Security_ItemCategoryWS.View.FreezePanes(2, 1);
            Security_ItemCategoryWS.Cells[1, 1, 1, 11].Style.Font.Bold = true;
            Security_ItemCategoryWS.Cells[1, 1, 1, 11].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            Security_ItemCategoryWS.Column(1).Width = 15;
            Security_ItemCategoryWS.Column(2).Width = 15;
            Security_ItemCategoryWS.Column(3).Width = 15;
            Security_ItemCategoryWS.Column(4).Width = 25;
            Security_ItemCategoryWS.Column(3).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            Security_ItemCategoryWS.Column(5).Width = 25;
            Security_ItemCategoryWS.Column(6).Width = 25;
            Security_ItemCategoryWS.Column(7).Width = 25;
            Security_ItemCategoryWS.Column(7).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            Security_ItemCategoryWS.Column(8).Width = 25;
            Security_ItemCategoryWS.Column(9).Width = 25;
            Security_ItemCategoryWS.Column(10).Width = 25;
            Security_ItemCategoryWS.Column(11).Width = 15;

            Security_ItemCategoryWS.Cells[1, 1].Value = "ID";
            Security_ItemCategoryWS.Cells[1, 2].Value = "Description";
            Security_ItemCategoryWS.Cells[1, 3].Value = "Created DateTime";
            Security_ItemCategoryWS.Cells[1, 4].Value = "Created Method";
            Security_ItemCategoryWS.Cells[1, 5].Value = "Created User";
            Security_ItemCategoryWS.Cells[1, 6].Value = "Created User Name";
            Security_ItemCategoryWS.Cells[1, 7].Value = "Last Amended DateTime";
            Security_ItemCategoryWS.Cells[1, 8].Value = "Last Amended Method";
            Security_ItemCategoryWS.Cells[1, 9].Value = "Last Amended User";
            Security_ItemCategoryWS.Cells[1, 10].Value = "Last Amended Name";
            Security_ItemCategoryWS.Cells[1, 11].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < Security_ItemCategorys.Count(); i++)
            {
                Security_ItemCategoryWS.Cells[rc, 1].Value = Security_ItemCategorys[i].PublicID;
                Security_ItemCategoryWS.Cells[rc, 2].Value = Security_ItemCategorys[i].Description;
                Security_ItemCategoryWS.Cells[rc, 3].Value = Security_ItemCategorys[i].CreatedDateTime.ToOADate();
                Security_ItemCategoryWS.Cells[rc, 4].Value = Security_ItemCategorys[i].CreatedMethod;

                if (users.Any(x => x.ID == Security_ItemCategorys[i].CreatedUserID))
                {
                    Security_ItemCategoryWS.Cells[rc, 5].Value = users.FirstOrDefault(x => x.ID == Security_ItemCategorys[i].CreatedUserID).PublicID;
                    Security_ItemCategoryWS.Cells[rc, 6].Value = users.FirstOrDefault(x => x.ID == Security_ItemCategorys[i].CreatedUserID).Firstname;
                }
                else
                    Security_ItemCategoryWS.Cells[rc, 5].Value = "UserID: " + Security_ItemCategorys[i].CreatedUserID.ToString();

                Security_ItemCategoryWS.Cells[rc, 7].Value = Security_ItemCategorys[i].LastAmendedDateTime.ToOADate();
                Security_ItemCategoryWS.Cells[rc, 8].Value = Security_ItemCategorys[i].LastAmendedMethod;

                if (users.Any(x => x.ID == Security_ItemCategorys[i].LastAmendedUserID))
                {
                    Security_ItemCategoryWS.Cells[rc, 9].Value = users.FirstOrDefault(x => x.ID == Security_ItemCategorys[i].LastAmendedUserID).PublicID;
                    Security_ItemCategoryWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == Security_ItemCategorys[i].LastAmendedUserID).Firstname;
                }
                else
                    Security_ItemCategoryWS.Cells[rc, 9].Value = "UserID: " + Security_ItemCategorys[i].LastAmendedUserID.ToString();

                Security_ItemCategoryWS.Cells[rc, 11].Value = Security_ItemCategorys[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
