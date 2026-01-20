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
    public class Security_ItemExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_ItemCategory.dbRow> categories = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.Security_Item.dbRow> Security_Items = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.Security_Item.Search Security_ItemSearchData;
        public Security_ItemExcel(wbm_common.DataObjects.Security_Item.Search Security_ItemSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.Security_ItemSearchData = Security_ItemSearchData;
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
                        Security_ItemPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("Security_ItemExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.Security_Item.dbRow>> rc = await Security_ItemHelper.ExecuteSearch(Security_ItemSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            Security_Items = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = Security_Items.Select(x => x.CreatedUserID).Union(Security_Items.Select(x => x.LastAmendedUserID)).Distinct().ToList();
            List<int> ListCategoryID = Security_Items.Select(x => x.ItemCategoryID).Distinct().ToList();

            if (ListCategoryID.Count > 0)
            {
                Result<List<wbm_common.DataObjects.Security_ItemCategory.dbRow>> categoriesResults = _WBMDB.Security_ItemCategory_ListByListIDs(ListCategoryID).GetAwaiter().GetResult();
                if (categoriesResults.IsFailure || categoriesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                categories = categoriesResults.Value;
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

        private void Security_ItemPage(ExcelPackage ep)
        {
            ExcelWorksheet Security_ItemWS = ep.Workbook.Worksheets.Add("Security_Item");

            #region header
            Security_ItemWS.View.FreezePanes(2, 1);
            Security_ItemWS.Cells[1, 1, 1, 11].Style.Font.Bold = true;
            Security_ItemWS.Cells[1, 1, 1, 11].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            Security_ItemWS.Column(1).Width = 25;
            Security_ItemWS.Column(2).Width = 25;
            Security_ItemWS.Column(3).Width = 25;
            Security_ItemWS.Column(4).Width = 25;
            Security_ItemWS.Column(3).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            Security_ItemWS.Column(5).Width = 25;
            Security_ItemWS.Column(6).Width = 25;
            Security_ItemWS.Column(7).Width = 25;
            Security_ItemWS.Column(7).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            Security_ItemWS.Column(8).Width = 25;
            Security_ItemWS.Column(9).Width = 25;
            Security_ItemWS.Column(10).Width = 25;
            Security_ItemWS.Column(11).Width = 15;

            Security_ItemWS.Cells[1, 1].Value = "Item Category ID";
            Security_ItemWS.Cells[1, 2].Value = "Tag ID";
            Security_ItemWS.Cells[1, 3].Value = "Created DateTime";
            Security_ItemWS.Cells[1, 4].Value = "Created Method";
            Security_ItemWS.Cells[1, 5].Value = "Created User";
            Security_ItemWS.Cells[1, 6].Value = "Created User Name";
            Security_ItemWS.Cells[1, 7].Value = "Last Amended DateTime";
            Security_ItemWS.Cells[1, 8].Value = "Last Amended Method";
            Security_ItemWS.Cells[1, 9].Value = "Last Amended User";
            Security_ItemWS.Cells[1, 10].Value = "Last Amended Name";
            Security_ItemWS.Cells[1, 11].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < Security_Items.Count(); i++)
            {
                if (categories.Any(x => x.ID == Security_Items[i].ItemCategoryID))
                    Security_ItemWS.Cells[rc, 1].Value = categories.FirstOrDefault(x => x.ID == Security_Items[i].ItemCategoryID).PublicID;
                else
                    Security_ItemWS.Cells[rc, 1].Value = "ItemCategoryID: " + Security_Items[i].ItemCategoryID.ToString();

                Security_ItemWS.Cells[rc, 2].Value = Security_Items[i].TagID;
                Security_ItemWS.Cells[rc, 3].Value = Security_Items[i].CreatedDateTime.ToOADate();
                Security_ItemWS.Cells[rc, 4].Value = Security_Items[i].CreatedMethod;

                if (users.Any(x => x.ID == Security_Items[i].CreatedUserID))
                {
                    Security_ItemWS.Cells[rc, 5].Value = users.FirstOrDefault(x => x.ID == Security_Items[i].CreatedUserID).PublicID;
                    Security_ItemWS.Cells[rc, 6].Value = users.FirstOrDefault(x => x.ID == Security_Items[i].CreatedUserID).Firstname;
                }
                else
                    Security_ItemWS.Cells[rc, 5].Value = "UserID: " + Security_Items[i].CreatedUserID.ToString();

                Security_ItemWS.Cells[rc, 7].Value = Security_Items[i].LastAmendedDateTime.ToOADate();
                Security_ItemWS.Cells[rc, 8].Value = Security_Items[i].LastAmendedMethod;

                if (users.Any(x => x.ID == Security_Items[i].LastAmendedUserID))
                {
                    Security_ItemWS.Cells[rc, 9].Value = users.FirstOrDefault(x => x.ID == Security_Items[i].LastAmendedUserID).PublicID;
                    Security_ItemWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == Security_Items[i].LastAmendedUserID).Firstname;
                }
                else
                    Security_ItemWS.Cells[rc, 9].Value = "UserID: " + Security_Items[i].LastAmendedUserID.ToString();

                Security_ItemWS.Cells[rc, 11].Value = Security_Items[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
