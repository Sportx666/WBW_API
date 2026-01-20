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
    public class Security_User_ItemExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Company.dbRow> companies = null;
        public List<wbm_common.DataObjects.Security_Item.dbRow> items = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.Security_User_Item.dbRow> Security_User_Items = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.Security_User_Item.Search Security_User_ItemSearchData;
        public Security_User_ItemExcel(wbm_common.DataObjects.Security_User_Item.Search Security_User_ItemSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.Security_User_ItemSearchData = Security_User_ItemSearchData;
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
                        Security_User_ItemPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("Security_User_ItemExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.Security_User_Item.dbRow>> rc = await Security_User_ItemHelper.ExecuteSearch(Security_User_ItemSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            Security_User_Items = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = Security_User_Items.Select(x => x.CreatedUserID).Union(Security_User_Items.Select(x => x.LastAmendedUserID)).Union(Security_User_Items.Select(x => x.UserID)).Distinct().ToList();
            List<int> ListCompanyID = Security_User_Items.Select(x => x.CompanyID).Distinct().ToList();
            List<int> ListItemID = Security_User_Items.Select(x => x.ItemID).Distinct().ToList();

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

            if (ListCompanyID.Count > 0)
            {
                Result<List<wbm_common.DataObjects.Company.dbRow>> companiesResults = _WBMDB.Company_ListByListIDs(ListCompanyID).GetAwaiter().GetResult();
                if (companiesResults.IsFailure || companiesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                companies = companiesResults.Value;
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

        private void Security_User_ItemPage(ExcelPackage ep)
        {
            ExcelWorksheet Security_User_ItemWS = ep.Workbook.Worksheets.Add("Security_User_Item");

            #region header
            Security_User_ItemWS.View.FreezePanes(2, 1);
            Security_User_ItemWS.Cells[1, 1, 1, 14].Style.Font.Bold = true;
            Security_User_ItemWS.Cells[1, 1, 1, 14].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            Security_User_ItemWS.Column(1).Width = 25;
            Security_User_ItemWS.Column(2).Width = 25;
            Security_User_ItemWS.Column(3).Width = 25;
            Security_User_ItemWS.Column(4).Width = 25;
            Security_User_ItemWS.Column(5).Width = 25;
            Security_User_ItemWS.Column(6).Width = 25;
            Security_User_ItemWS.Column(6).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            Security_User_ItemWS.Column(7).Width = 25;
            Security_User_ItemWS.Column(8).Width = 25;
            Security_User_ItemWS.Column(9).Width = 25;
            Security_User_ItemWS.Column(10).Width = 25;
            Security_User_ItemWS.Column(10).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            Security_User_ItemWS.Column(11).Width = 25;
            Security_User_ItemWS.Column(12).Width = 25;
            Security_User_ItemWS.Column(13).Width = 25;
            Security_User_ItemWS.Column(14).Width = 15;

            Security_User_ItemWS.Cells[1, 1].Value = "Username";
            Security_User_ItemWS.Cells[1, 2].Value = "Company";
            Security_User_ItemWS.Cells[1, 3].Value = "Company Name";
            Security_User_ItemWS.Cells[1, 4].Value = "Tag ID";
            Security_User_ItemWS.Cells[1, 5].Value = "Access Level";
            Security_User_ItemWS.Cells[1, 6].Value = "Created DateTime";
            Security_User_ItemWS.Cells[1, 7].Value = "Created Method";
            Security_User_ItemWS.Cells[1, 8].Value = "Created User";
            Security_User_ItemWS.Cells[1, 9].Value = "Created User Name";
            Security_User_ItemWS.Cells[1, 10].Value = "Last Amended DateTime";
            Security_User_ItemWS.Cells[1, 11].Value = "Last Amended Method";
            Security_User_ItemWS.Cells[1, 12].Value = "Last Amended User";
            Security_User_ItemWS.Cells[1, 13].Value = "Last Amended Name";
            Security_User_ItemWS.Cells[1, 14].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < Security_User_Items.Count(); i++)
            {
                if (users.Any(x => x.ID == Security_User_Items[i].UserID))
                    Security_User_ItemWS.Cells[rc, 1].Value = users.FirstOrDefault(x => x.ID == Security_User_Items[i].UserID).PublicID;
                else
                    Security_User_ItemWS.Cells[rc, 1].Value = "UserID: " + Security_User_Items[i].UserID.ToString();

                if (companies.Any(x => x.ID == Security_User_Items[i].CompanyID))
                {
                    Security_User_ItemWS.Cells[rc, 2].Value = companies.FirstOrDefault(x => x.ID == Security_User_Items[i].CompanyID).Abbreviation;
                    Security_User_ItemWS.Cells[rc, 3].Value = companies.FirstOrDefault(x => x.ID == Security_User_Items[i].CompanyID).LongName;
                }
                else
                    Security_User_ItemWS.Cells[rc, 2].Value = "CompanyID: " + Security_User_Items[i].CompanyID.ToString();

                if (items.Any(x => x.ID == Security_User_Items[i].ItemID))
                    Security_User_ItemWS.Cells[rc, 4].Value = items.FirstOrDefault(x => x.ID == Security_User_Items[i].ItemID).TagID;
                else
                    Security_User_ItemWS.Cells[rc, 4].Value = "ItemID: " + Security_User_Items[i].ItemID.ToString();

                Security_User_ItemWS.Cells[rc, 5].Value = Security_User_Items[i].AccessLevel;
                Security_User_ItemWS.Cells[rc, 6].Value = Security_User_Items[i].CreatedDateTime.ToOADate();
                Security_User_ItemWS.Cells[rc, 7].Value = Security_User_Items[i].CreatedMethod;

                if (users.Any(x => x.ID == Security_User_Items[i].CreatedUserID))
                {
                    Security_User_ItemWS.Cells[rc, 8].Value = users.FirstOrDefault(x => x.ID == Security_User_Items[i].CreatedUserID).PublicID;
                    Security_User_ItemWS.Cells[rc, 9].Value = users.FirstOrDefault(x => x.ID == Security_User_Items[i].CreatedUserID).Firstname;
                }
                else
                    Security_User_ItemWS.Cells[rc, 8].Value = "UserID: " + Security_User_Items[i].CreatedUserID.ToString();

                Security_User_ItemWS.Cells[rc, 10].Value = Security_User_Items[i].LastAmendedDateTime.ToOADate();
                Security_User_ItemWS.Cells[rc, 11].Value = Security_User_Items[i].LastAmendedMethod;

                if (users.Any(x => x.ID == Security_User_Items[i].LastAmendedUserID))
                {
                    Security_User_ItemWS.Cells[rc, 12].Value = users.FirstOrDefault(x => x.ID == Security_User_Items[i].LastAmendedUserID).PublicID;
                    Security_User_ItemWS.Cells[rc, 13].Value = users.FirstOrDefault(x => x.ID == Security_User_Items[i].LastAmendedUserID).Firstname;
                }
                else
                    Security_User_ItemWS.Cells[rc, 12].Value = "UserID: " + Security_User_Items[i].LastAmendedUserID.ToString();

                Security_User_ItemWS.Cells[rc, 14].Value = Security_User_Items[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
