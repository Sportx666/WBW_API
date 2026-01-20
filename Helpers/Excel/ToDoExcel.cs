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
    public class ToDoExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.ChangeHeader_TableName.dbRow> changeheadertablenames = null;
        public List<wbm_common.DataObjects.ToDoStatus_Type.dbRow> todostatuses = null;
        public List<wbm_common.DataObjects.Owner.dbRow> owners = null;
        public List<wbm_common.DataObjects.Site.dbRow> sites = null;
        public List<wbm_common.DataObjects.Company.dbRow> companies = null;
        public List<wbm_common.DataObjects.ToDo_Category.dbRow> todocategories = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.ToDo.dbRow> ToDos = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.ToDo.Search ToDoSearchData;
        public ToDoExcel(wbm_common.DataObjects.ToDo.Search ToDoSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.ToDoSearchData = ToDoSearchData;
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
                        ToDoPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("ToDoExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.ToDo.dbRow>> rc = await ToDoHelper.ExecuteSearch(ToDoSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            ToDos = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = ToDos.Select(x => x.CreatedUserID).Union(ToDos.Select(x => x.LastAmendedUserID)).Distinct().ToList();
            List<int> ListToDoCategoryIDs = ToDos.Select(x => x.CategoryID).Distinct().ToList();
            List<int> ListCompanyIDs = ToDos.Select(x => x.CompanyID).Distinct().ToList();
            List<int> ListSiteIDs = ToDos.Select(x => x.SiteID).Distinct().ToList();
            List<int> ListOwnerIDs = ToDos.Select(x => x.OwnerID).Distinct().ToList();
            List<int> ListToDoStatusIDs = ToDos.Select(x => x.StatusID).Distinct().ToList();
            List<int> ListchangeheadertableIDs = ToDos.Select(x => x.ChangeHeaderTableID).Distinct().ToList();

            if (ListToDoCategoryIDs.Count > 0)
            {
                Result<List<wbm_common.DataObjects.ToDo_Category.dbRow>> todocategoriesResults = _WBMDB.ToDo_Category_ListByListIDs(ListToDoCategoryIDs).GetAwaiter().GetResult();
                if (todocategoriesResults.IsFailure || todocategoriesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                todocategories = todocategoriesResults.Value;
            }

            if (ListCompanyIDs.Count > 0)
            {
                Result<List<wbm_common.DataObjects.Company.dbRow>> companiesResults = _WBMDB.Company_ListByListIDs(ListCompanyIDs).GetAwaiter().GetResult();
                if (companiesResults.IsFailure || companiesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                companies = companiesResults.Value;
            }

            if (ListSiteIDs.Count > 0)
            {
                Result<List<wbm_common.DataObjects.Site.dbRow>> sitesResults = _WBMDB.Site_ListByListIDs(ListSiteIDs).GetAwaiter().GetResult();
                if (sitesResults.IsFailure || sitesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                sites = sitesResults.Value;
            }

            if (ListOwnerIDs.Count > 0)
            {
                Result<List<wbm_common.DataObjects.Owner.dbRow>> ownersResults = _WBMDB.Owner_ListByListIDs(ListOwnerIDs).GetAwaiter().GetResult();
                if (ownersResults.IsFailure || ownersResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                owners = ownersResults.Value;
            }

            if (ListToDoStatusIDs.Count > 0)
            {
                Result<List<wbm_common.DataObjects.ToDoStatus_Type.dbRow>> todostatusesResults = _WBMDB.ToDoStatus_Type_ListByListIDs(ListToDoStatusIDs).GetAwaiter().GetResult();
                if (todostatusesResults.IsFailure || todostatusesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                todostatuses = todostatusesResults.Value;
            }

            if (ListchangeheadertableIDs.Count > 0)
            {
                Result<List<wbm_common.DataObjects.ChangeHeader_TableName.dbRow>> changeheadertablenamesResults = _WBMDB.ChangeHeader_TableName_ListByListIDs(ListchangeheadertableIDs).GetAwaiter().GetResult();
                if (changeheadertablenamesResults.IsFailure || changeheadertablenamesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                changeheadertablenames = changeheadertablenamesResults.Value;
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

        private void ToDoPage(ExcelPackage ep)
        {
            ExcelWorksheet ToDoWS = ep.Workbook.Worksheets.Add("ToDo");

            #region header
            ToDoWS.View.FreezePanes(2, 1);
            ToDoWS.Cells[1, 1, 1, 24].Style.Font.Bold = true;
            ToDoWS.Cells[1, 1, 1, 24].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            ToDoWS.Column(1).Width = 15;
            ToDoWS.Column(2).Width = 20;
            ToDoWS.Column(3).Width = 15;
            ToDoWS.Column(4).Width = 25;
            ToDoWS.Column(5).Width = 10;
            ToDoWS.Column(6).Width = 25;
            ToDoWS.Column(7).Width = 15;
            ToDoWS.Column(8).Width = 15;
            ToDoWS.Column(9).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ToDoWS.Column(9).Width = 25;
            ToDoWS.Column(10).Width = 15;
            ToDoWS.Column(11).Width = 25;
            ToDoWS.Column(12).Width = 25;
            ToDoWS.Column(13).Width = 25;
            ToDoWS.Column(14).Width = 15;
            ToDoWS.Column(15).Width = 15;
            ToDoWS.Column(16).Width = 25;
            ToDoWS.Column(17).Width = 25;
            ToDoWS.Column(16).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ToDoWS.Column(18).Width = 25;
            ToDoWS.Column(19).Width = 25;
            ToDoWS.Column(20).Width = 25;
            ToDoWS.Column(21).Width = 25;
            ToDoWS.Column(20).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ToDoWS.Column(22).Width = 25;
            ToDoWS.Column(23).Width = 25;
            ToDoWS.Column(24).Width = 15;

            ToDoWS.Cells[1, 1].Value = "Category ID";
            ToDoWS.Cells[1, 2].Value = "Category Description";
            ToDoWS.Cells[1, 3].Value = "Company";
            ToDoWS.Cells[1, 4].Value = "Company Name";
            ToDoWS.Cells[1, 5].Value = "Site";
            ToDoWS.Cells[1, 6].Value = "Site Description";
            ToDoWS.Cells[1, 7].Value = "Owner ID";
            ToDoWS.Cells[1, 8].Value = "Owner Name";
            ToDoWS.Cells[1, 9].Value = "Event Datetime";
            ToDoWS.Cells[1, 10].Value = "Description";
            ToDoWS.Cells[1, 11].Value = "Change Header Table ID";
            ToDoWS.Cells[1, 12].Value = "Source Owner";
            ToDoWS.Cells[1, 13].Value = "Source Owner Name";
            ToDoWS.Cells[1, 14].Value = "Status ID";
            ToDoWS.Cells[1, 15].Value = "Note";
            ToDoWS.Cells[1, 16].Value = "Created DateTime";
            ToDoWS.Cells[1, 17].Value = "Created Method";
            ToDoWS.Cells[1, 18].Value = "Created User";
            ToDoWS.Cells[1, 19].Value = "Created User Name";
            ToDoWS.Cells[1, 20].Value = "Last Amended DateTime";
            ToDoWS.Cells[1, 21].Value = "Last Amended Method";
            ToDoWS.Cells[1, 22].Value = "Last Amended User";
            ToDoWS.Cells[1, 23].Value = "Last Amended Name";
            ToDoWS.Cells[1, 24].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < ToDos.Count(); i++)
            {
                if (todocategories.Any(x => x.ID == ToDos[i].CategoryID))
                {
                    ToDoWS.Cells[rc, 1].Value = todocategories.FirstOrDefault(x => x.ID == ToDos[i].CategoryID).PublicID;
                    ToDoWS.Cells[rc, 2].Value = todocategories.FirstOrDefault(x => x.ID == ToDos[i].CategoryID).Description;
                }
                else
                    ToDoWS.Cells[rc, 1].Value = "CategoryID: " + ToDos[i].CategoryID.ToString();

                if (companies.Any(x => x.ID == ToDos[i].CompanyID))
                {
                    ToDoWS.Cells[rc, 3].Value = companies.FirstOrDefault(x => x.ID == ToDos[i].CompanyID).Abbreviation;
                    ToDoWS.Cells[rc, 4].Value = companies.FirstOrDefault(x => x.ID == ToDos[i].CompanyID).LongName;
                }
                else
                    ToDoWS.Cells[rc, 3].Value = "CompanyID: " + ToDos[i].CompanyID.ToString();

                if (sites.Any(x => x.ID == ToDos[i].SiteID))
                {
                    ToDoWS.Cells[rc, 5].Value = sites.FirstOrDefault(x => x.ID == ToDos[i].SiteID).PublicID;
                    ToDoWS.Cells[rc, 6].Value = sites.FirstOrDefault(x => x.ID == ToDos[i].SiteID).Description;
                }
                else
                    ToDoWS.Cells[rc, 5].Value = "SiteID: " + ToDos[i].SiteID.ToString();

                if (owners.Any(x => x.ID == ToDos[i].OwnerID))
                {
                    ToDoWS.Cells[rc, 7].Value = owners.FirstOrDefault(x => x.ID == ToDos[i].OwnerID).ID;
                    ToDoWS.Cells[rc, 8].Value = owners.FirstOrDefault(x => x.ID == ToDos[i].OwnerID).Name;
                }
                else
                    ToDoWS.Cells[rc, 7].Value = "OwnerID: " + ToDos[i].OwnerID.ToString();

                ToDoWS.Cells[rc, 9].Value = ToDos[i].EventDateTime.ToOADate();
                ToDoWS.Cells[rc, 10].Value = ToDos[i].Description;

                if (changeheadertablenames.Any(x => x.ID == ToDos[i].ChangeHeaderTableID))
                    ToDoWS.Cells[rc, 11].Value = changeheadertablenames.FirstOrDefault(x => x.ID == ToDos[i].ChangeHeaderTableID).TableName;
                else
                    ToDoWS.Cells[rc, 11].Value = "ChangeHeaderTableID: " + ToDos[i].ChangeHeaderTableID.ToString();

                if (owners.Any(x => x.ID == ToDos[i].SourceID))
                {
                    ToDoWS.Cells[rc, 12].Value = owners.FirstOrDefault(x => x.ID == ToDos[i].SourceID).ID;
                    ToDoWS.Cells[rc, 13].Value = owners.FirstOrDefault(x => x.ID == ToDos[i].SourceID).Name;
                }
                else
                    ToDoWS.Cells[rc, 12].Value = "SourceID: " + ToDos[i].SourceID.ToString();

                if (todostatuses.Any(x => x.ID == ToDos[i].StatusID))
                    ToDoWS.Cells[rc, 14].Value = todostatuses.FirstOrDefault(x => x.ID == ToDos[i].StatusID).PublicID;
                else
                    ToDoWS.Cells[rc, 14].Value = "StatusID: " + ToDos[i].StatusID.ToString();

                ToDoWS.Cells[rc, 15].Value = ToDos[i].Note;

                ToDoWS.Cells[rc, 16].Value = ToDos[i].CreatedDateTime.ToOADate();
                ToDoWS.Cells[rc, 17].Value = ToDos[i].CreatedMethod;

                if (users.Any(x => x.ID == ToDos[i].CreatedUserID))
                {
                    ToDoWS.Cells[rc, 18].Value = users.FirstOrDefault(x => x.ID == ToDos[i].CreatedUserID).PublicID;
                    ToDoWS.Cells[rc, 19].Value = users.FirstOrDefault(x => x.ID == ToDos[i].CreatedUserID).Firstname;
                }
                else
                    ToDoWS.Cells[rc, 18].Value = "UserID: " + ToDos[i].CreatedUserID.ToString();

                ToDoWS.Cells[rc, 20].Value = ToDos[i].LastAmendedDateTime.ToOADate();
                ToDoWS.Cells[rc, 21].Value = ToDos[i].LastAmendedMethod;

                if (users.Any(x => x.ID == ToDos[i].LastAmendedUserID))
                {
                    ToDoWS.Cells[rc, 22].Value = users.FirstOrDefault(x => x.ID == ToDos[i].LastAmendedUserID).PublicID;
                    ToDoWS.Cells[rc, 23].Value = users.FirstOrDefault(x => x.ID == ToDos[i].LastAmendedUserID).Firstname;
                }
                else
                    ToDoWS.Cells[rc, 22].Value = "UserID: " + ToDos[i].LastAmendedUserID.ToString();

                ToDoWS.Cells[rc, 24].Value = ToDos[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
