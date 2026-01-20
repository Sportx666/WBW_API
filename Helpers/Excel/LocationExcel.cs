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
    public class LocationExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Company.dbRow> companies = null;
        public List<wbm_common.DataObjects.Site.dbRow> sites = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.PalletStorageType.dbRow> palletStorageTypes = null;
        public List<wbm_common.DataObjects.Location.dbRow> Locations = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.Location.Search LocationSearchData;
        public LocationExcel(wbm_common.DataObjects.Location.Search LocationSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.LocationSearchData = LocationSearchData;
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
                        LocationPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("LocationExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.Location.dbRow>> rc = await LocationHelper.ExecuteSearch(LocationSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            Locations = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListPalletStorageTypeID = Locations.Select(x => x.PalletStorageTypeID).Distinct().ToList();
            List<int> ListCompanyIDs = Locations.Select(x => x.CompanyID).Distinct().ToList();
            List<int> ListSiteID = Locations.Select(x => x.SiteID).Distinct().ToList();
            List<int> ListUserID = Locations.Select(x => x.CreatedUserID).Union(Locations.Select(x => x.LastAmendedUserID)).Distinct().ToList();

            if (ListPalletStorageTypeID.Count > 0)
            {
                Result<List<wbm_common.DataObjects.PalletStorageType.dbRow>> palletStorageTypesResults = _WBMDB.PalletStorageType_ListByListIDs(ListPalletStorageTypeID).GetAwaiter().GetResult();
                if (palletStorageTypesResults.IsFailure || palletStorageTypesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                palletStorageTypes = palletStorageTypesResults.Value;
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

        private void LocationPage(ExcelPackage ep)
        {
            ExcelWorksheet LocationWS = ep.Workbook.Worksheets.Add("Location");

            #region header
            LocationWS.View.FreezePanes(2, 1);
            LocationWS.Cells[1, 1, 1, 16].Style.Font.Bold = true;
            LocationWS.Cells[1, 1, 1, 16].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            LocationWS.Column(1).Width = 25;
            LocationWS.Column(2).Width = 25;
            LocationWS.Column(3).Width = 25;
            LocationWS.Column(4).Width = 25;
            LocationWS.Column(5).Width = 25;
            LocationWS.Column(6).Width = 25;
            LocationWS.Column(7).Width = 30;
            LocationWS.Column(8).Width = 25;
            LocationWS.Column(8).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            LocationWS.Column(9).Width = 25;
            LocationWS.Column(10).Width = 25;
            LocationWS.Column(11).Width = 25;
            LocationWS.Column(12).Width = 25;
            LocationWS.Column(12).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            LocationWS.Column(13).Width = 25;
            LocationWS.Column(14).Width = 25;
            LocationWS.Column(15).Width = 25;
            LocationWS.Column(16).Width = 25;

            LocationWS.Cells[1, 1].Value = "Company";
            LocationWS.Cells[1, 2].Value = "Company Name";
            LocationWS.Cells[1, 3].Value = "Site";
            LocationWS.Cells[1, 4].Value = "Site Description";
            LocationWS.Cells[1, 5].Value = "Location ID";
            LocationWS.Cells[1, 6].Value = "Pallet Storage Type";
            LocationWS.Cells[1, 7].Value = "Pallet Storage Type Description";
            LocationWS.Cells[1, 8].Value = "Created DateTime";
            LocationWS.Cells[1, 9].Value = "Created Method";
            LocationWS.Cells[1, 10].Value = "Created User";
            LocationWS.Cells[1, 11].Value = "Created User Name";
            LocationWS.Cells[1, 12].Value = "Last Amended DateTime";
            LocationWS.Cells[1, 13].Value = "Last Amended Method";
            LocationWS.Cells[1, 14].Value = "Last Amended User";
            LocationWS.Cells[1, 15].Value = "Last Amended Name";
            LocationWS.Cells[1, 16].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < Locations.Count(); i++)
            {
                if (companies.Any(x => x.ID == Locations[i].CompanyID))
                {
                    LocationWS.Cells[rc, 1].Value = companies.FirstOrDefault(x => x.ID == Locations[i].CompanyID).ShortName;
                    LocationWS.Cells[rc, 2].Value = companies.FirstOrDefault(x => x.ID == Locations[i].CompanyID).LongName;
                }
                else
                    LocationWS.Cells[rc, 1].Value = "CompanyID: " + Locations[i].CompanyID.ToString();

                if (sites.Any(x => x.ID == Locations[i].SiteID))
                {
                    LocationWS.Cells[rc, 3].Value = sites.FirstOrDefault(x => x.ID == Locations[i].SiteID).PublicID;
                    LocationWS.Cells[rc, 4].Value = sites.FirstOrDefault(x => x.ID == Locations[i].SiteID).Description;
                }
                else
                    LocationWS.Cells[rc, 3].Value = "SiteID: " + Locations[i].SiteID.ToString();

                LocationWS.Cells[rc, 5].Value = Locations[i].PublicLocationID;

                if (palletStorageTypes.Any(x => x.ID == Locations[i].PalletStorageTypeID))
                {
                    LocationWS.Cells[rc, 6].Value = palletStorageTypes.FirstOrDefault(x => x.ID == Locations[i].PalletStorageTypeID).PublicID;
                    LocationWS.Cells[rc, 7].Value = palletStorageTypes.FirstOrDefault(x => x.ID == Locations[i].PalletStorageTypeID).Description;
                }
                else
                    LocationWS.Cells[rc, 6].Value = "PalletStorageTypeID: " + Locations[i].PalletStorageTypeID.ToString();


                LocationWS.Cells[rc, 8].Value = Locations[i].CreatedDateTime.ToOADate();
                LocationWS.Cells[rc, 9].Value = Locations[i].CreatedMethod;

                if (users.Any(x => x.ID == Locations[i].CreatedUserID))
                {
                    LocationWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == Locations[i].CreatedUserID).Firstname;
                    LocationWS.Cells[rc, 11].Value = users.FirstOrDefault(x => x.ID == Locations[i].CreatedUserID).PublicID;
                }
                else
                    LocationWS.Cells[rc, 10].Value = "UserID: " + Locations[i].CreatedUserID.ToString();

                LocationWS.Cells[rc, 12].Value = Locations[i].LastAmendedDateTime.ToOADate();
                LocationWS.Cells[rc, 13].Value = Locations[i].LastAmendedMethod;

                if (users.Any(x => x.ID == Locations[i].LastAmendedUserID))
                {
                    LocationWS.Cells[rc, 14].Value = users.FirstOrDefault(x => x.ID == Locations[i].LastAmendedUserID).Firstname;
                    LocationWS.Cells[rc, 15].Value = users.FirstOrDefault(x => x.ID == Locations[i].LastAmendedUserID).PublicID;
                }
                else
                    LocationWS.Cells[rc, 14].Value = "UserID: " + Locations[i].LastAmendedUserID.ToString();

                LocationWS.Cells[rc, 16].Value = Locations[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
