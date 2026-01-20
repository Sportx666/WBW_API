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
    public class Security_RoleExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.Security_Role.dbRow> Security_Roles = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.Security_Role.Search Security_RoleSearchData;
        public Security_RoleExcel(wbm_common.DataObjects.Security_Role.Search Security_RoleSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.Security_RoleSearchData = Security_RoleSearchData;
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
                        Security_RolePage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("Security_RoleExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.Security_Role.dbRow>> rc = await Security_RoleHelper.ExecuteSearch(Security_RoleSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            Security_Roles = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = Security_Roles.Select(x => x.CreatedUserID).Union(Security_Roles.Select(x => x.LastAmendedUserID)).Distinct().ToList();

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

        private void Security_RolePage(ExcelPackage ep)
        {
            ExcelWorksheet Security_RoleWS = ep.Workbook.Worksheets.Add("Security_Role");

            #region header
            Security_RoleWS.View.FreezePanes(2, 1);
            Security_RoleWS.Cells[1, 1, 1, 11].Style.Font.Bold = true;
            Security_RoleWS.Cells[1, 1, 1, 11].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            Security_RoleWS.Column(1).Width = 10;
            Security_RoleWS.Column(2).Width = 20;
            Security_RoleWS.Column(3).Width = 25;
            Security_RoleWS.Column(4).Width = 25;
            Security_RoleWS.Column(3).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            Security_RoleWS.Column(5).Width = 25;
            Security_RoleWS.Column(6).Width = 25;
            Security_RoleWS.Column(7).Width = 25;
            Security_RoleWS.Column(7).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            Security_RoleWS.Column(8).Width = 25;
            Security_RoleWS.Column(9).Width = 25;
            Security_RoleWS.Column(10).Width = 25;
            Security_RoleWS.Column(11).Width = 15;

            Security_RoleWS.Cells[1, 1].Value = "ID";
            Security_RoleWS.Cells[1, 2].Value = "Description";
            Security_RoleWS.Cells[1, 3].Value = "Created DateTime";
            Security_RoleWS.Cells[1, 4].Value = "Created Method";
            Security_RoleWS.Cells[1, 5].Value = "Created User";
            Security_RoleWS.Cells[1, 6].Value = "Created User Name";
            Security_RoleWS.Cells[1, 7].Value = "Last Amended DateTime";
            Security_RoleWS.Cells[1, 8].Value = "Last Amended Method";
            Security_RoleWS.Cells[1, 9].Value = "Last Amended User";
            Security_RoleWS.Cells[1, 10].Value = "Last Amended Name";
            Security_RoleWS.Cells[1, 11].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < Security_Roles.Count(); i++)
            {
                Security_RoleWS.Cells[rc, 1].Value = Security_Roles[i].ID;
                Security_RoleWS.Cells[rc, 2].Value = Security_Roles[i].Description;
                Security_RoleWS.Cells[rc, 3].Value = Security_Roles[i].CreatedDateTime.ToOADate();
                Security_RoleWS.Cells[rc, 4].Value = Security_Roles[i].CreatedMethod;

                if (users.Any(x => x.ID == Security_Roles[i].CreatedUserID))
                {
                    Security_RoleWS.Cells[rc, 5].Value = users.FirstOrDefault(x => x.ID == Security_Roles[i].CreatedUserID).PublicID;
                    Security_RoleWS.Cells[rc, 6].Value = users.FirstOrDefault(x => x.ID == Security_Roles[i].CreatedUserID).Firstname;
                }
                else
                    Security_RoleWS.Cells[rc, 5].Value = "UserID: " + Security_Roles[i].CreatedUserID.ToString();

                Security_RoleWS.Cells[rc, 7].Value = Security_Roles[i].LastAmendedDateTime.ToOADate();
                Security_RoleWS.Cells[rc, 8].Value = Security_Roles[i].LastAmendedMethod;

                if (users.Any(x => x.ID == Security_Roles[i].LastAmendedUserID))
                {
                    Security_RoleWS.Cells[rc, 9].Value = users.FirstOrDefault(x => x.ID == Security_Roles[i].LastAmendedUserID).PublicID;
                    Security_RoleWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == Security_Roles[i].LastAmendedUserID).Firstname;
                }
                else
                    Security_RoleWS.Cells[rc, 9].Value = "UserID: " + Security_Roles[i].LastAmendedUserID.ToString();

                Security_RoleWS.Cells[rc, 11].Value = Security_Roles[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
