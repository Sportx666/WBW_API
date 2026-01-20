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
    public class Security_Role_ItemExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_Role.dbRow> roles = null;
        public List<wbm_common.DataObjects.Security_Item.dbRow> items = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.Security_Role_Item.dbRow> Security_Role_Items = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.Security_Role_Item.Search Security_Role_ItemSearchData;
        public Security_Role_ItemExcel(wbm_common.DataObjects.Security_Role_Item.Search Security_Role_ItemSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.Security_Role_ItemSearchData = Security_Role_ItemSearchData;
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
                        Security_Role_ItemPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("Security_Role_ItemExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.Security_Role_Item.dbRow>> rc = await Security_Role_ItemHelper.ExecuteSearch(Security_Role_ItemSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            Security_Role_Items = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = Security_Role_Items.Select(x => x.CreatedUserID).Union(Security_Role_Items.Select(x => x.LastAmendedUserID)).Union(Security_Role_Items.Select(x => x.LastAmendedUserID)).Distinct().ToList();
            List<int> ListRoleID = Security_Role_Items.Select(x => x.RoleID).Distinct().ToList();
            List<int> ListItemID = Security_Role_Items.Select(x => x.ItemID).Distinct().ToList();

            if (ListItemID.Count > 0)
            {
                Result<List<wbm_common.DataObjects.Security_Item.dbRow>> itemsResults = _WBMDB.Security_Item_ListByListIDs(ListItemID).GetAwaiter().GetResult();
                if (itemsResults.IsFailure || itemsResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                items = itemsResults.Value;
            }

            if (ListRoleID.Count > 0)
            {
                Result<List<wbm_common.DataObjects.Security_Role.dbRow>> rolesResults = _WBMDB.Security_Role_ListByListIDs(ListRoleID).GetAwaiter().GetResult();
                if (rolesResults.IsFailure || rolesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                roles = rolesResults.Value;
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

        private void Security_Role_ItemPage(ExcelPackage ep)
        {
            ExcelWorksheet Security_Role_ItemWS = ep.Workbook.Worksheets.Add("Security_Role_Item");

            #region header
            Security_Role_ItemWS.View.FreezePanes(2, 1);
            Security_Role_ItemWS.Cells[1, 1, 1, 13].Style.Font.Bold = true;
            Security_Role_ItemWS.Cells[1, 1, 1, 13].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            Security_Role_ItemWS.Column(1).Width = 15;
            Security_Role_ItemWS.Column(2).Width = 20;
            Security_Role_ItemWS.Column(3).Width = 15;
            Security_Role_ItemWS.Column(4).Width = 25;
            Security_Role_ItemWS.Column(5).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            Security_Role_ItemWS.Column(5).Width = 25;
            Security_Role_ItemWS.Column(6).Width = 25;
            Security_Role_ItemWS.Column(7).Width = 25;
            Security_Role_ItemWS.Column(9).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            Security_Role_ItemWS.Column(8).Width = 25;
            Security_Role_ItemWS.Column(9).Width = 25;
            Security_Role_ItemWS.Column(10).Width = 25;
            Security_Role_ItemWS.Column(11).Width = 25;
            Security_Role_ItemWS.Column(12).Width = 25;
            Security_Role_ItemWS.Column(13).Width = 15;

            Security_Role_ItemWS.Cells[1, 1].Value = "Role ID";
            Security_Role_ItemWS.Cells[1, 2].Value = "Role Description";
            Security_Role_ItemWS.Cells[1, 3].Value = "Tag ID";
            Security_Role_ItemWS.Cells[1, 4].Value = "Access Level";
            Security_Role_ItemWS.Cells[1, 5].Value = "Created DateTime";
            Security_Role_ItemWS.Cells[1, 6].Value = "Created Method";
            Security_Role_ItemWS.Cells[1, 7].Value = "Created User";
            Security_Role_ItemWS.Cells[1, 8].Value = "Created User Name";
            Security_Role_ItemWS.Cells[1, 9].Value = "Last Amended DateTime";
            Security_Role_ItemWS.Cells[1, 10].Value = "Last Amended Method";
            Security_Role_ItemWS.Cells[1, 11].Value = "Last Amended User";
            Security_Role_ItemWS.Cells[1, 12].Value = "Last Amended Name";
            Security_Role_ItemWS.Cells[1, 13].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < Security_Role_Items.Count(); i++)
            {
                if (roles.Any(x => x.ID == Security_Role_Items[i].RoleID))
                {
                    Security_Role_ItemWS.Cells[rc, 1].Value = roles.FirstOrDefault(x => x.ID == Security_Role_Items[i].RoleID).PublicID;
                    Security_Role_ItemWS.Cells[rc, 2].Value = roles.FirstOrDefault(x => x.ID == Security_Role_Items[i].RoleID).Description;
                }
                else
                    Security_Role_ItemWS.Cells[rc, 1].Value = "RoleID: " + Security_Role_Items[i].RoleID.ToString();

                if (items.Any(x => x.ID == Security_Role_Items[i].ItemID))
                    Security_Role_ItemWS.Cells[rc, 3].Value = items.FirstOrDefault(x => x.ID == Security_Role_Items[i].ItemID).TagID;
                else
                    Security_Role_ItemWS.Cells[rc, 3].Value = "ItemID: " + Security_Role_Items[i].ItemID.ToString();

                Security_Role_ItemWS.Cells[rc, 4].Value = Security_Role_Items[i].AccessLevel;
                Security_Role_ItemWS.Cells[rc, 5].Value = Security_Role_Items[i].CreatedDateTime.ToOADate();
                Security_Role_ItemWS.Cells[rc, 6].Value = Security_Role_Items[i].CreatedMethod;

                if (users.Any(x => x.ID == Security_Role_Items[i].CreatedUserID))
                {
                    Security_Role_ItemWS.Cells[rc, 7].Value = users.FirstOrDefault(x => x.ID == Security_Role_Items[i].CreatedUserID).PublicID;
                    Security_Role_ItemWS.Cells[rc, 8].Value = users.FirstOrDefault(x => x.ID == Security_Role_Items[i].CreatedUserID).Firstname;
                }
                else
                    Security_Role_ItemWS.Cells[rc, 7].Value = "UserID: " + Security_Role_Items[i].CreatedUserID.ToString();

                Security_Role_ItemWS.Cells[rc, 9].Value = Security_Role_Items[i].LastAmendedDateTime.ToOADate();
                Security_Role_ItemWS.Cells[rc, 10].Value = Security_Role_Items[i].LastAmendedMethod;

                if (users.Any(x => x.ID == Security_Role_Items[i].LastAmendedUserID))
                {
                    Security_Role_ItemWS.Cells[rc, 11].Value = users.FirstOrDefault(x => x.ID == Security_Role_Items[i].LastAmendedUserID).PublicID;
                    Security_Role_ItemWS.Cells[rc, 12].Value = users.FirstOrDefault(x => x.ID == Security_Role_Items[i].LastAmendedUserID).Firstname;
                }
                else
                    Security_Role_ItemWS.Cells[rc, 11].Value = "UserID: " + Security_Role_Items[i].LastAmendedUserID.ToString();

                Security_Role_ItemWS.Cells[rc, 13].Value = Security_Role_Items[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
