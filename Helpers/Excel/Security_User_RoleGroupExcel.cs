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
    public class Security_User_RoleGroupExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Company.dbRow> companies = null;
        public List<wbm_common.DataObjects.Security_RoleGroup.dbRow> rolegroups = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.Security_User_RoleGroup.dbRow> Security_User_RoleGroups = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.Security_User_RoleGroup.Search Security_User_RoleGroupSearchData;
        public Security_User_RoleGroupExcel(wbm_common.DataObjects.Security_User_RoleGroup.Search Security_User_RoleGroupSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.Security_User_RoleGroupSearchData = Security_User_RoleGroupSearchData;
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
                        Security_User_RoleGroupPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("Security_User_RoleGroupExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.Security_User_RoleGroup.dbRow>> rc = await Security_User_RoleGroupHelper.ExecuteSearch(Security_User_RoleGroupSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            Security_User_RoleGroups = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = Security_User_RoleGroups.Select(x => x.CreatedUserID).Union(Security_User_RoleGroups.Select(x => x.LastAmendedUserID)).Union(Security_User_RoleGroups.Select(x => x.UserID)).Distinct().ToList();
            List<int> ListCompanyID = Security_User_RoleGroups.Select(x => x.CompanyID).Distinct().ToList();
            List<int> ListRoleGroupID = Security_User_RoleGroups.Select(x => x.RoleGroupID).Distinct().ToList();

            if (ListRoleGroupID.Count > 0)
            {
                Result<List<wbm_common.DataObjects.Security_RoleGroup.dbRow>> rolegroupsResults = _WBMDB.Security_RoleGroup_ListByListIDs(ListRoleGroupID).GetAwaiter().GetResult();
                if (rolegroupsResults.IsFailure || rolegroupsResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                rolegroups = rolegroupsResults.Value;
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

        private void Security_User_RoleGroupPage(ExcelPackage ep)
        {
            ExcelWorksheet Security_User_RoleGroupWS = ep.Workbook.Worksheets.Add("Security_User_RoleGroup");

            #region header
            Security_User_RoleGroupWS.View.FreezePanes(2, 1);
            Security_User_RoleGroupWS.Cells[1, 1, 1, 15].Style.Font.Bold = true;
            Security_User_RoleGroupWS.Cells[1, 1, 1, 15].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            Security_User_RoleGroupWS.Column(1).Width = 25;
            Security_User_RoleGroupWS.Column(2).Width = 25;
            Security_User_RoleGroupWS.Column(3).Width = 25;
            Security_User_RoleGroupWS.Column(4).Width = 25;
            Security_User_RoleGroupWS.Column(5).Width = 25;
            Security_User_RoleGroupWS.Column(6).Width = 25;
            Security_User_RoleGroupWS.Column(6).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            Security_User_RoleGroupWS.Column(7).Width = 25;
            Security_User_RoleGroupWS.Column(8).Width = 25;
            Security_User_RoleGroupWS.Column(9).Width = 25;
            Security_User_RoleGroupWS.Column(10).Width = 25;
            Security_User_RoleGroupWS.Column(10).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            Security_User_RoleGroupWS.Column(11).Width = 25;
            Security_User_RoleGroupWS.Column(12).Width = 25;
            Security_User_RoleGroupWS.Column(13).Width = 25;
            Security_User_RoleGroupWS.Column(14).Width = 15;

            Security_User_RoleGroupWS.Cells[1, 1].Value = "Username";
            Security_User_RoleGroupWS.Cells[1, 2].Value = "Company";
            Security_User_RoleGroupWS.Cells[1, 3].Value = "Company Name";
            Security_User_RoleGroupWS.Cells[1, 4].Value = "Role Group ID";
            Security_User_RoleGroupWS.Cells[1, 5].Value = "Role Group Description";
            Security_User_RoleGroupWS.Cells[1, 6].Value = "Created DateTime";
            Security_User_RoleGroupWS.Cells[1, 7].Value = "Created Method";
            Security_User_RoleGroupWS.Cells[1, 8].Value = "Created User";
            Security_User_RoleGroupWS.Cells[1, 9].Value = "Created User Name";
            Security_User_RoleGroupWS.Cells[1, 10].Value = "Last Amended DateTime";
            Security_User_RoleGroupWS.Cells[1, 11].Value = "Last Amended Method";
            Security_User_RoleGroupWS.Cells[1, 12].Value = "Last Amended User";
            Security_User_RoleGroupWS.Cells[1, 13].Value = "Last Amended Name";
            Security_User_RoleGroupWS.Cells[1, 14].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < Security_User_RoleGroups.Count(); i++)
            {
                if (users.Any(x => x.ID == Security_User_RoleGroups[i].UserID))
                    Security_User_RoleGroupWS.Cells[rc, 1].Value = users.FirstOrDefault(x => x.ID == Security_User_RoleGroups[i].UserID).PublicID;
                else
                    Security_User_RoleGroupWS.Cells[rc, 1].Value = "UserID: " + Security_User_RoleGroups[i].UserID.ToString();

                if (companies.Any(x => x.ID == Security_User_RoleGroups[i].CompanyID))
                {
                    Security_User_RoleGroupWS.Cells[rc, 2].Value = companies.FirstOrDefault(x => x.ID == Security_User_RoleGroups[i].CompanyID).Abbreviation;
                    Security_User_RoleGroupWS.Cells[rc, 3].Value = companies.FirstOrDefault(x => x.ID == Security_User_RoleGroups[i].CompanyID).LongName;
                }
                else
                    Security_User_RoleGroupWS.Cells[rc, 2].Value = "CompanyID: " + Security_User_RoleGroups[i].CompanyID.ToString();

                if (rolegroups.Any(x => x.ID == Security_User_RoleGroups[i].RoleGroupID))
                {
                    Security_User_RoleGroupWS.Cells[rc, 4].Value = rolegroups.FirstOrDefault(x => x.ID == Security_User_RoleGroups[i].RoleGroupID).PublicID;
                    Security_User_RoleGroupWS.Cells[rc, 5].Value = rolegroups.FirstOrDefault(x => x.ID == Security_User_RoleGroups[i].RoleGroupID).Description;
                }
                else
                    Security_User_RoleGroupWS.Cells[rc, 4].Value = "RoleGroupID: " + Security_User_RoleGroups[i].RoleGroupID.ToString();

                Security_User_RoleGroupWS.Cells[rc, 6].Value = Security_User_RoleGroups[i].CreatedDateTime.ToOADate();
                Security_User_RoleGroupWS.Cells[rc, 7].Value = Security_User_RoleGroups[i].CreatedMethod;

                if (users.Any(x => x.ID == Security_User_RoleGroups[i].CreatedUserID))
                {
                    Security_User_RoleGroupWS.Cells[rc, 8].Value = users.FirstOrDefault(x => x.ID == Security_User_RoleGroups[i].CreatedUserID).PublicID;
                    Security_User_RoleGroupWS.Cells[rc, 9].Value = users.FirstOrDefault(x => x.ID == Security_User_RoleGroups[i].CreatedUserID).Firstname;
                }
                else
                    Security_User_RoleGroupWS.Cells[rc, 8].Value = "UserID: " + Security_User_RoleGroups[i].CreatedUserID.ToString();

                Security_User_RoleGroupWS.Cells[rc, 10].Value = Security_User_RoleGroups[i].LastAmendedDateTime.ToOADate();
                Security_User_RoleGroupWS.Cells[rc, 11].Value = Security_User_RoleGroups[i].LastAmendedMethod;

                if (users.Any(x => x.ID == Security_User_RoleGroups[i].LastAmendedUserID))
                {
                    Security_User_RoleGroupWS.Cells[rc, 12].Value = users.FirstOrDefault(x => x.ID == Security_User_RoleGroups[i].LastAmendedUserID).PublicID;
                    Security_User_RoleGroupWS.Cells[rc, 13].Value = users.FirstOrDefault(x => x.ID == Security_User_RoleGroups[i].LastAmendedUserID).Firstname;
                }
                else
                    Security_User_RoleGroupWS.Cells[rc, 12].Value = "UserID: " + Security_User_RoleGroups[i].LastAmendedUserID.ToString();

                Security_User_RoleGroupWS.Cells[rc, 14].Value = Security_User_RoleGroups[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
