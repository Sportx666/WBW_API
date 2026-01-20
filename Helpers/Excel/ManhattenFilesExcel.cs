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
    public class ManhattenFilesExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Company.dbRow> companies = null;
        public List<wbm_common.DataObjects.Site.dbRow> sites = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.Owner.dbRow> owners = null;
        public List<wbm_common.DataObjects.ManhattenFileType.dbRow> manhattenfiletypes = null;
        public List<wbm_common.DataObjects.ManhattenFiles.dbRow> ManhattenFiless = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.ManhattenFiles.Search ManhattenFilesSearchData;
        public ManhattenFilesExcel(wbm_common.DataObjects.ManhattenFiles.Search ManhattenFilesSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.ManhattenFilesSearchData = ManhattenFilesSearchData;
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
                        ManhattenFilesPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("ManhattenFilesExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.ManhattenFiles.dbRow>> rc = await ManhattenFilesHelper.ExecuteSearch(ManhattenFilesSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            ManhattenFiless = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListOwnerID = ManhattenFiless.Select(x => x.OwnerID).Distinct().ToList();
            List<int> ListManhattenFileTypes = ManhattenFiless.Select(x => x.FileTypeID).Distinct().ToList();
            List<int> ListCompanyIDs = ManhattenFiless.Select(x => x.CompanyID).Distinct().ToList();
            List<int> ListSiteID = ManhattenFiless.Select(x => x.SiteID).Distinct().ToList();
            List<int> ListUserID = ManhattenFiless.Select(x => x.CreatedUserID).Union(ManhattenFiless.Select(x => x.LastAmendedUserID)).Distinct().ToList();

            if (ListOwnerID.Count > 0)
            {
                Result<List<wbm_common.DataObjects.Owner.dbRow>> ownersResults = _WBMDB.Owner_ListByListIDs(ListOwnerID).GetAwaiter().GetResult();
                if (ownersResults.IsFailure || ownersResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                owners = ownersResults.Value;
            }

            if (ListManhattenFileTypes.Count > 0)
            {
                Result<List<wbm_common.DataObjects.ManhattenFileType.dbRow>> manhattenfiletypesResults = _WBMDB.ManhattenFileType_ListByListIDs(ListManhattenFileTypes).GetAwaiter().GetResult();
                if (manhattenfiletypesResults.IsFailure || manhattenfiletypesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                manhattenfiletypes = manhattenfiletypesResults.Value;
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

            if (ListSiteID.Count > 0)
            {
                Result<List<wbm_common.DataObjects.Site.dbRow>> sitesResults = _WBMDB.Site_ListByListIDs(ListSiteID).GetAwaiter().GetResult();
                if (sitesResults.IsFailure || sitesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                sites = sitesResults.Value;
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

        private void ManhattenFilesPage(ExcelPackage ep)
        {
            ExcelWorksheet ManhattenFilesWS = ep.Workbook.Worksheets.Add("ManhattenFiles");

            #region header
            ManhattenFilesWS.View.FreezePanes(2, 1);
            ManhattenFilesWS.Cells[1, 1, 1, 19].Style.Font.Bold = true;
            ManhattenFilesWS.Cells[1, 1, 1, 19].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            ManhattenFilesWS.Column(1).Width = 25;
            ManhattenFilesWS.Column(2).Width = 25;
            ManhattenFilesWS.Column(3).Width = 25;
            ManhattenFilesWS.Column(4).Width = 25;
            ManhattenFilesWS.Column(5).Width = 25;
            ManhattenFilesWS.Column(6).Width = 25;
            ManhattenFilesWS.Column(7).Width = 25;
            ManhattenFilesWS.Column(8).Width = 25;
            ManhattenFilesWS.Column(9).Width = 25;
            ManhattenFilesWS.Column(10).Width = 25;
            ManhattenFilesWS.Column(11).Width = 25;
            ManhattenFilesWS.Column(11).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ManhattenFilesWS.Column(12).Width = 25;
            ManhattenFilesWS.Column(13).Width = 30;
            ManhattenFilesWS.Column(14).Width = 15;
            ManhattenFilesWS.Column(15).Width = 15;
            ManhattenFilesWS.Column(15).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ManhattenFilesWS.Column(16).Width = 15;
            ManhattenFilesWS.Column(17).Width = 25;
            ManhattenFilesWS.Column(18).Width = 25;
            ManhattenFilesWS.Column(19).Width = 25;

            ManhattenFilesWS.Cells[1, 1].Value = "Company";
            ManhattenFilesWS.Cells[1, 2].Value = "Company Name";
            ManhattenFilesWS.Cells[1, 3].Value = "Owner ID";
            ManhattenFilesWS.Cells[1, 4].Value = "Owner Name";
            ManhattenFilesWS.Cells[1, 5].Value = "Site";
            ManhattenFilesWS.Cells[1, 6].Value = "Site Description";
            ManhattenFilesWS.Cells[1, 7].Value = "Filename";
            ManhattenFilesWS.Cells[1, 8].Value = "File Type Description";
            ManhattenFilesWS.Cells[1, 9].Value = "Date Loaded";
            ManhattenFilesWS.Cells[1, 10].Value = "Number Of Lines";
            ManhattenFilesWS.Cells[1, 11].Value = "Created DateTime";
            ManhattenFilesWS.Cells[1, 12].Value = "Created Method";
            ManhattenFilesWS.Cells[1, 13].Value = "Created User";
            ManhattenFilesWS.Cells[1, 14].Value = "Created User Name";
            ManhattenFilesWS.Cells[1, 15].Value = "Last Amended DateTime";
            ManhattenFilesWS.Cells[1, 16].Value = "Last Amended Method";
            ManhattenFilesWS.Cells[1, 17].Value = "Last Amended User";
            ManhattenFilesWS.Cells[1, 18].Value = "Last Amended Name";
            ManhattenFilesWS.Cells[1, 19].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < ManhattenFiless.Count(); i++)
            {
                if (companies.Any(x => x.ID == ManhattenFiless[i].CompanyID))
                {
                    ManhattenFilesWS.Cells[rc, 1].Value = companies.FirstOrDefault(x => x.ID == ManhattenFiless[i].CompanyID).ShortName;
                    ManhattenFilesWS.Cells[rc, 2].Value = companies.FirstOrDefault(x => x.ID == ManhattenFiless[i].CompanyID).LongName;
                }
                else
                    ManhattenFilesWS.Cells[rc, 1].Value = "CompanyID: " + ManhattenFiless[i].CompanyID.ToString();

                if (owners.Any(x => x.ID == ManhattenFiless[i].OwnerID))
                {
                    ManhattenFilesWS.Cells[rc, 3].Value = owners.FirstOrDefault(x => x.ID == ManhattenFiless[i].OwnerID).SourceOwnerID;
                    ManhattenFilesWS.Cells[rc, 4].Value = owners.FirstOrDefault(x => x.ID == ManhattenFiless[i].OwnerID).Name;
                }
                else
                    ManhattenFilesWS.Cells[rc, 3].Value = "OwnerID: " + ManhattenFiless[i].OwnerID.ToString();

                if (sites.Any(x => x.ID == ManhattenFiless[i].SiteID))
                {
                    ManhattenFilesWS.Cells[rc, 5].Value = sites.FirstOrDefault(x => x.ID == ManhattenFiless[i].SiteID).PublicID;
                    ManhattenFilesWS.Cells[rc, 6].Value = sites.FirstOrDefault(x => x.ID == ManhattenFiless[i].SiteID).Description;
                }
                else
                    ManhattenFilesWS.Cells[rc, 5].Value = "SiteID: " + ManhattenFiless[i].SiteID.ToString();

                if (manhattenfiletypes.Any(x => x.ID == ManhattenFiless[i].FileTypeID))
                {
                    ManhattenFilesWS.Cells[rc, 7].Value = manhattenfiletypes.FirstOrDefault(x => x.ID == ManhattenFiless[i].FileTypeID).PublicID;
                    ManhattenFilesWS.Cells[rc, 8].Value = manhattenfiletypes.FirstOrDefault(x => x.ID == ManhattenFiless[i].FileTypeID).Description;
                }
                else
                    ManhattenFilesWS.Cells[rc, 7].Value = "FileTypeID: " + ManhattenFiless[i].FileTypeID.ToString();

                ManhattenFilesWS.Cells[rc, 9].Value = ManhattenFiless[i].DateLoaded;
                ManhattenFilesWS.Cells[rc, 10].Value = ManhattenFiless[i].NumberOfLines;

                ManhattenFilesWS.Cells[rc, 11].Value = ManhattenFiless[i].CreatedDateTime.ToOADate();
                ManhattenFilesWS.Cells[rc, 12].Value = ManhattenFiless[i].CreatedMethod;
                if (users.Any(x => x.ID == ManhattenFiless[i].CreatedUserID))
                {
                    ManhattenFilesWS.Cells[rc, 13].Value = users.FirstOrDefault(x => x.ID == ManhattenFiless[i].CreatedUserID).Firstname;
                    ManhattenFilesWS.Cells[rc, 14].Value = users.FirstOrDefault(x => x.ID == ManhattenFiless[i].CreatedUserID).PublicID;
                }
                else
                    ManhattenFilesWS.Cells[rc, 13].Value = "UserID: " + ManhattenFiless[i].CreatedUserID.ToString();

                ManhattenFilesWS.Cells[rc, 15].Value = ManhattenFiless[i].LastAmendedDateTime.ToOADate();
                ManhattenFilesWS.Cells[rc, 16].Value = ManhattenFiless[i].LastAmendedMethod;

                if (users.Any(x => x.ID == ManhattenFiless[i].LastAmendedUserID))
                {
                    ManhattenFilesWS.Cells[rc, 17].Value = users.FirstOrDefault(x => x.ID == ManhattenFiless[i].LastAmendedUserID).Firstname;
                    ManhattenFilesWS.Cells[rc, 18].Value = users.FirstOrDefault(x => x.ID == ManhattenFiless[i].LastAmendedUserID).PublicID;
                }
                else
                    ManhattenFilesWS.Cells[rc, 17].Value = "UserID: " + ManhattenFiless[i].LastAmendedUserID.ToString();

                ManhattenFilesWS.Cells[rc, 19].Value = ManhattenFiless[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
