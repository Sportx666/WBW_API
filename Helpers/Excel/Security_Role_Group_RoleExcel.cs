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
    public class Security_RoleGroup_RoleExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_Role.dbRow> roles = null;
        public List<wbm_common.DataObjects.Security_RoleGroup.dbRow> rolegroups = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.Security_RoleGroup_Role.dbRow> Security_RoleGroup_Roles = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.Security_RoleGroup_Role.Search Security_RoleGroup_RoleSearchData;
        public Security_RoleGroup_RoleExcel(wbm_common.DataObjects.Security_RoleGroup_Role.Search Security_RoleGroup_RoleSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.Security_RoleGroup_RoleSearchData = Security_RoleGroup_RoleSearchData;
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
                        Security_RoleGroup_RolePage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("Security_RoleGroup_RoleExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.Security_RoleGroup_Role.dbRow>> rc = await Security_RoleGroup_RoleHelper.ExecuteSearch(Security_RoleGroup_RoleSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            Security_RoleGroup_Roles = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = Security_RoleGroup_Roles.Select(x => x.CreatedUserID).Union(Security_RoleGroup_Roles.Select(x => x.LastAmendedUserID)).Distinct().ToList();
            List<int> ListGroupRolesID = Security_RoleGroup_Roles.Select(x => x.RoleGroupID).Distinct().ToList();
            List<int> ListRolesID = Security_RoleGroup_Roles.Select(x => x.RoleID).Distinct().ToList();

            if (ListGroupRolesID.Count > 0)
            {
                Result<List<wbm_common.DataObjects.Security_RoleGroup.dbRow>> rolegroupsResults = _WBMDB.Security_RoleGroup_ListByListIDs(ListGroupRolesID).GetAwaiter().GetResult();
                if (rolegroupsResults.IsFailure || rolegroupsResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                rolegroups = rolegroupsResults.Value;
            }

            if (ListRolesID.Count > 0)
            {
                Result<List<wbm_common.DataObjects.Security_Role.dbRow>> rolesResults = _WBMDB.Security_Role_ListByListIDs(ListRolesID).GetAwaiter().GetResult();
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

        private void Security_RoleGroup_RolePage(ExcelPackage ep)
        {
            ExcelWorksheet Security_RoleGroup_RoleWS = ep.Workbook.Worksheets.Add("Security_RoleGroup_Role");

            #region header
            Security_RoleGroup_RoleWS.View.FreezePanes(2, 1);
            Security_RoleGroup_RoleWS.Cells[1, 1, 1, 13].Style.Font.Bold = true;
            Security_RoleGroup_RoleWS.Cells[1, 1, 1, 13].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            Security_RoleGroup_RoleWS.Column(1).Width = 25;
            Security_RoleGroup_RoleWS.Column(2).Width = 25;
            Security_RoleGroup_RoleWS.Column(3).Width = 25;
            Security_RoleGroup_RoleWS.Column(4).Width = 25;
            Security_RoleGroup_RoleWS.Column(5).Width = 25;
            Security_RoleGroup_RoleWS.Column(5).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            Security_RoleGroup_RoleWS.Column(6).Width = 25;
            Security_RoleGroup_RoleWS.Column(7).Width = 25;
            Security_RoleGroup_RoleWS.Column(8).Width = 25;
            Security_RoleGroup_RoleWS.Column(9).Width = 25;
            Security_RoleGroup_RoleWS.Column(9).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            Security_RoleGroup_RoleWS.Column(10).Width = 25;
            Security_RoleGroup_RoleWS.Column(11).Width = 25;
            Security_RoleGroup_RoleWS.Column(12).Width = 25;
            Security_RoleGroup_RoleWS.Column(13).Width = 15;

            Security_RoleGroup_RoleWS.Cells[1, 1].Value = "Role Group ID";
            Security_RoleGroup_RoleWS.Cells[1, 2].Value = "Role Group Description";
            Security_RoleGroup_RoleWS.Cells[1, 3].Value = "Role ID";
            Security_RoleGroup_RoleWS.Cells[1, 4].Value = "Role Description";
            Security_RoleGroup_RoleWS.Cells[1, 5].Value = "Created DateTime";
            Security_RoleGroup_RoleWS.Cells[1, 6].Value = "Created Method";
            Security_RoleGroup_RoleWS.Cells[1, 7].Value = "Created User";
            Security_RoleGroup_RoleWS.Cells[1, 8].Value = "Created User Name";
            Security_RoleGroup_RoleWS.Cells[1, 9].Value = "Last Amended DateTime";
            Security_RoleGroup_RoleWS.Cells[1, 10].Value = "Last Amended Method";
            Security_RoleGroup_RoleWS.Cells[1, 11].Value = "Last Amended User";
            Security_RoleGroup_RoleWS.Cells[1, 12].Value = "Last Amended Name";
            Security_RoleGroup_RoleWS.Cells[1, 13].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < Security_RoleGroup_Roles.Count(); i++)
            {
                if (rolegroups.Any(x => x.ID == Security_RoleGroup_Roles[i].RoleGroupID))
                {
                    Security_RoleGroup_RoleWS.Cells[rc, 1].Value = rolegroups.FirstOrDefault(x => x.ID == Security_RoleGroup_Roles[i].RoleGroupID).PublicID;
                    Security_RoleGroup_RoleWS.Cells[rc, 2].Value = rolegroups.FirstOrDefault(x => x.ID == Security_RoleGroup_Roles[i].RoleGroupID).Description;
                }
                else
                    Security_RoleGroup_RoleWS.Cells[rc, 1].Value = "RoleGroupID: " + Security_RoleGroup_Roles[i].RoleGroupID.ToString();

                if (roles.Any(x => x.ID == Security_RoleGroup_Roles[i].RoleID))
                {
                    Security_RoleGroup_RoleWS.Cells[rc, 3].Value = roles.FirstOrDefault(x => x.ID == Security_RoleGroup_Roles[i].RoleID).PublicID;
                    Security_RoleGroup_RoleWS.Cells[rc, 4].Value = roles.FirstOrDefault(x => x.ID == Security_RoleGroup_Roles[i].RoleID).Description;
                }
                else
                    Security_RoleGroup_RoleWS.Cells[rc, 3].Value = "RoleID: " + Security_RoleGroup_Roles[i].RoleID.ToString();

                Security_RoleGroup_RoleWS.Cells[rc, 5].Value = Security_RoleGroup_Roles[i].CreatedDateTime.ToOADate();
                Security_RoleGroup_RoleWS.Cells[rc, 6].Value = Security_RoleGroup_Roles[i].CreatedMethod;

                if (users.Any(x => x.ID == Security_RoleGroup_Roles[i].CreatedUserID))
                {
                    Security_RoleGroup_RoleWS.Cells[rc, 7].Value = users.FirstOrDefault(x => x.ID == Security_RoleGroup_Roles[i].CreatedUserID).PublicID;
                    Security_RoleGroup_RoleWS.Cells[rc, 8].Value = users.FirstOrDefault(x => x.ID == Security_RoleGroup_Roles[i].CreatedUserID).Firstname;
                }
                else
                    Security_RoleGroup_RoleWS.Cells[rc, 7].Value = "UserID: " + Security_RoleGroup_Roles[i].CreatedUserID.ToString();

                Security_RoleGroup_RoleWS.Cells[rc, 9].Value = Security_RoleGroup_Roles[i].LastAmendedDateTime.ToOADate();
                Security_RoleGroup_RoleWS.Cells[rc, 10].Value = Security_RoleGroup_Roles[i].LastAmendedMethod;

                if (users.Any(x => x.ID == Security_RoleGroup_Roles[i].LastAmendedUserID))
                {
                    Security_RoleGroup_RoleWS.Cells[rc, 11].Value = users.FirstOrDefault(x => x.ID == Security_RoleGroup_Roles[i].LastAmendedUserID).PublicID;
                    Security_RoleGroup_RoleWS.Cells[rc, 12].Value = users.FirstOrDefault(x => x.ID == Security_RoleGroup_Roles[i].LastAmendedUserID).Firstname;
                }
                else
                    Security_RoleGroup_RoleWS.Cells[rc, 11].Value = "UserID: " + Security_RoleGroup_Roles[i].LastAmendedUserID.ToString();

                Security_RoleGroup_RoleWS.Cells[rc, 13].Value = Security_RoleGroup_Roles[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
