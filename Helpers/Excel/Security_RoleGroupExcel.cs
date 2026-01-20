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
    public class Security_RoleGroupExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.Security_RoleGroup.dbRow> Security_RoleGroups = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.Security_RoleGroup.Search Security_RoleGroupSearchData;
        public Security_RoleGroupExcel(wbm_common.DataObjects.Security_RoleGroup.Search Security_RoleGroupSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.Security_RoleGroupSearchData = Security_RoleGroupSearchData;
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
                        Security_RoleGroupPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("Security_RoleGroupExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.Security_RoleGroup.dbRow>> rc = await Security_RoleGroupHelper.ExecuteSearch(Security_RoleGroupSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            Security_RoleGroups = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = Security_RoleGroups.Select(x => x.CreatedUserID).Union(Security_RoleGroups.Select(x => x.LastAmendedUserID)).Distinct().ToList();

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

        private void Security_RoleGroupPage(ExcelPackage ep)
        {
            ExcelWorksheet Security_RoleGroupWS = ep.Workbook.Worksheets.Add("Security_RoleGroup");

            #region header
            Security_RoleGroupWS.View.FreezePanes(2, 1);
            Security_RoleGroupWS.Cells[1, 1, 1, 11].Style.Font.Bold = true;
            Security_RoleGroupWS.Cells[1, 1, 1, 11].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            Security_RoleGroupWS.Column(1).Width = 10;
            Security_RoleGroupWS.Column(2).Width = 20;
            Security_RoleGroupWS.Column(3).Width = 25;
            Security_RoleGroupWS.Column(4).Width = 25;
            Security_RoleGroupWS.Column(3).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            Security_RoleGroupWS.Column(5).Width = 25;
            Security_RoleGroupWS.Column(6).Width = 25;
            Security_RoleGroupWS.Column(7).Width = 25;
            Security_RoleGroupWS.Column(7).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            Security_RoleGroupWS.Column(8).Width = 25;
            Security_RoleGroupWS.Column(9).Width = 25;
            Security_RoleGroupWS.Column(10).Width = 25;
            Security_RoleGroupWS.Column(11).Width = 15;

            Security_RoleGroupWS.Cells[1, 1].Value = "ID";
            Security_RoleGroupWS.Cells[1, 2].Value = "Description";
            Security_RoleGroupWS.Cells[1, 3].Value = "Created DateTime";
            Security_RoleGroupWS.Cells[1, 4].Value = "Created Method";
            Security_RoleGroupWS.Cells[1, 5].Value = "Created User";
            Security_RoleGroupWS.Cells[1, 6].Value = "Created User Name";
            Security_RoleGroupWS.Cells[1, 7].Value = "Last Amended DateTime";
            Security_RoleGroupWS.Cells[1, 8].Value = "Last Amended Method";
            Security_RoleGroupWS.Cells[1, 9].Value = "Last Amended User";
            Security_RoleGroupWS.Cells[1, 10].Value = "Last Amended Name";
            Security_RoleGroupWS.Cells[1, 11].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < Security_RoleGroups.Count(); i++)
            {
                Security_RoleGroupWS.Cells[rc, 1].Value = Security_RoleGroups[i].ID;
                Security_RoleGroupWS.Cells[rc, 2].Value = Security_RoleGroups[i].Description;
                Security_RoleGroupWS.Cells[rc, 3].Value = Security_RoleGroups[i].CreatedDateTime.ToOADate();
                Security_RoleGroupWS.Cells[rc, 4].Value = Security_RoleGroups[i].CreatedMethod;

                if (users.Any(x => x.ID == Security_RoleGroups[i].CreatedUserID))
                {
                    Security_RoleGroupWS.Cells[rc, 5].Value = users.FirstOrDefault(x => x.ID == Security_RoleGroups[i].CreatedUserID).PublicID;
                    Security_RoleGroupWS.Cells[rc, 6].Value = users.FirstOrDefault(x => x.ID == Security_RoleGroups[i].CreatedUserID).Firstname;
                }
                else
                    Security_RoleGroupWS.Cells[rc, 5].Value = "UserID: " + Security_RoleGroups[i].CreatedUserID.ToString();

                Security_RoleGroupWS.Cells[rc, 7].Value = Security_RoleGroups[i].LastAmendedDateTime.ToOADate();
                Security_RoleGroupWS.Cells[rc, 8].Value = Security_RoleGroups[i].LastAmendedMethod;

                if (users.Any(x => x.ID == Security_RoleGroups[i].LastAmendedUserID))
                {
                    Security_RoleGroupWS.Cells[rc, 9].Value = users.FirstOrDefault(x => x.ID == Security_RoleGroups[i].LastAmendedUserID).PublicID;
                    Security_RoleGroupWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == Security_RoleGroups[i].LastAmendedUserID).Firstname;
                }
                else
                    Security_RoleGroupWS.Cells[rc, 9].Value = "UserID: " + Security_RoleGroups[i].LastAmendedUserID.ToString();

                Security_RoleGroupWS.Cells[rc, 11].Value = Security_RoleGroups[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
