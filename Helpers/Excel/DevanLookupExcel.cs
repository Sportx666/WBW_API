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
    public class DevanLookupExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.ContainerSize.dbRow> containerSizes = null;
        public List<wbm_common.DataObjects.Company.dbRow> companies = null;
        public List<wbm_common.DataObjects.Site.dbRow> sites = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.Owner.dbRow> owners = null;
        public List<wbm_common.DataObjects.RateCode.dbRow> ratecodes = null;
        public List<wbm_common.DataObjects.DevanLookup.dbRow> DevanLookups = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.DevanLookup.Search DevanLookupSearchData;
        public DevanLookupExcel(wbm_common.DataObjects.DevanLookup.Search DevanLookupSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.DevanLookupSearchData = DevanLookupSearchData;
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
                        DevanLookupPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("DevanLookupExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.DevanLookup.dbRow>> rc = await DevanLookupHelper.ExecuteSearch(DevanLookupSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            DevanLookups = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListContainerSizesID = DevanLookups.Select(x => x.ContainerSizeID).Distinct().ToList();
            List<int> ListOwnerID = DevanLookups.Select(x => x.OwnerID).Distinct().ToList();
            List<int> ListRateCodes = DevanLookups.Select(x => x.RateCodeID).Distinct().ToList();
            List<int> ListCompanyIDs = DevanLookups.Select(x => x.CompanyID).Distinct().ToList();
            List<int> ListSiteID = DevanLookups.Select(x => x.SiteID).Distinct().ToList();
            List<int> ListUserID = DevanLookups.Select(x => x.CreatedUserID).Union(DevanLookups.Select(x => x.LastAmendedUserID)).Distinct().ToList();

            if (ListContainerSizesID.Count > 0)
            {
                Result<List<wbm_common.DataObjects.ContainerSize.dbRow>> containerSizesResults = _WBMDB.ContainerSize_ListByListIDs(ListContainerSizesID).GetAwaiter().GetResult();
                if (containerSizesResults.IsFailure || containerSizesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                containerSizes = containerSizesResults.Value;
            }

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

            if (ListRateCodes.Count > 0)
            {
                Result<List<wbm_common.DataObjects.RateCode.dbRow>> ratecodesResults = _WBMDB.RateCode_ListByListIDs(ListRateCodes).GetAwaiter().GetResult();
                if (ratecodesResults.IsFailure || ratecodesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                ratecodes = ratecodesResults.Value;
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

        private void DevanLookupPage(ExcelPackage ep)
        {
            ExcelWorksheet DevanLookupWS = ep.Workbook.Worksheets.Add("DevanLookup");

            #region header
            DevanLookupWS.View.FreezePanes(2, 1);
            DevanLookupWS.Cells[1, 1, 1, 20].Style.Font.Bold = true;
            DevanLookupWS.Cells[1, 1, 1, 20].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            DevanLookupWS.Column(1).Width = 12;
            DevanLookupWS.Column(2).Width = 25;
            DevanLookupWS.Column(3).Width = 12;
            DevanLookupWS.Column(4).Width = 15;
            DevanLookupWS.Column(5).Width = 10;
            DevanLookupWS.Column(6).Width = 25;
            DevanLookupWS.Column(7).Width = 20;
            DevanLookupWS.Column(8).Width = 20;
            DevanLookupWS.Column(9).Width = 25;
            DevanLookupWS.Column(10).Width = 10;
            DevanLookupWS.Column(11).Width = 25;
            DevanLookupWS.Column(12).Width = 25;
            DevanLookupWS.Column(12).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            DevanLookupWS.Column(13).Width = 30;
            DevanLookupWS.Column(14).Width = 15;
            DevanLookupWS.Column(15).Width = 25;
            DevanLookupWS.Column(16).Width = 25;
            DevanLookupWS.Column(16).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            DevanLookupWS.Column(17).Width = 25;
            DevanLookupWS.Column(18).Width = 25;
            DevanLookupWS.Column(19).Width = 25;
            DevanLookupWS.Column(20).Width = 25;

            DevanLookupWS.Cells[1, 1].Value = "Company";
            DevanLookupWS.Cells[1, 2].Value = "Company Name";
            DevanLookupWS.Cells[1, 3].Value = "Owner ID";
            DevanLookupWS.Cells[1, 4].Value = "Owner Name";
            DevanLookupWS.Cells[1, 5].Value = "Site";
            DevanLookupWS.Cells[1, 6].Value = "Site Description";
            DevanLookupWS.Cells[1, 7].Value = "Container Size";
            DevanLookupWS.Cells[1, 8].Value = "Is Container Loose";
            DevanLookupWS.Cells[1, 9].Value = "Maximum SKU Count";
            DevanLookupWS.Cells[1, 10].Value = "Rate Code";
            DevanLookupWS.Cells[1, 11].Value = "Rate Code Description";
            DevanLookupWS.Cells[1, 12].Value = "Created DateTime";
            DevanLookupWS.Cells[1, 13].Value = "Created Method";
            DevanLookupWS.Cells[1, 14].Value = "Created User";
            DevanLookupWS.Cells[1, 15].Value = "Created User Name";
            DevanLookupWS.Cells[1, 16].Value = "Last Amended DateTime";
            DevanLookupWS.Cells[1, 17].Value = "Last Amended Method";
            DevanLookupWS.Cells[1, 18].Value = "Last Amended User";
            DevanLookupWS.Cells[1, 19].Value = "Last Amended Name";
            DevanLookupWS.Cells[1, 20].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < DevanLookups.Count(); i++)
            {
                if (companies.Any(x => x.ID == DevanLookups[i].CompanyID))
                {
                    DevanLookupWS.Cells[rc, 1].Value = companies.FirstOrDefault(x => x.ID == DevanLookups[i].CompanyID).ShortName;
                    DevanLookupWS.Cells[rc, 2].Value = companies.FirstOrDefault(x => x.ID == DevanLookups[i].CompanyID).LongName;
                }
                else
                    DevanLookupWS.Cells[rc, 1].Value = "CompanyID: " + DevanLookups[i].CompanyID.ToString();

                if (owners.Any(x => x.ID == DevanLookups[i].OwnerID))
                {
                    DevanLookupWS.Cells[rc, 3].Value = owners.FirstOrDefault(x => x.ID == DevanLookups[i].OwnerID).SourceOwnerID;
                    DevanLookupWS.Cells[rc, 4].Value = owners.FirstOrDefault(x => x.ID == DevanLookups[i].OwnerID).Name;
                }
                else
                    DevanLookupWS.Cells[rc, 3].Value = "OwnerID: " + DevanLookups[i].OwnerID.ToString();

                if (sites.Any(x => x.ID == DevanLookups[i].SiteID))
                {
                    DevanLookupWS.Cells[rc, 5].Value = sites.FirstOrDefault(x => x.ID == DevanLookups[i].SiteID).PublicID;
                    DevanLookupWS.Cells[rc, 6].Value = sites.FirstOrDefault(x => x.ID == DevanLookups[i].SiteID).Description;
                }
                else
                    DevanLookupWS.Cells[rc, 5].Value = "SiteID: " + DevanLookups[i].SiteID.ToString();

                if (containerSizes.Any(x => x.ID == DevanLookups[i].ContainerSizeID))
                    DevanLookupWS.Cells[rc, 7].Value = containerSizes.FirstOrDefault(x => x.ID == DevanLookups[i].ContainerSizeID).PublicID;
                else
                    DevanLookupWS.Cells[rc, 7].Value = "ContainerSizeID: " + DevanLookups[i].ContainerSizeID.ToString();

                DevanLookupWS.Cells[rc, 8].Value = DevanLookups[i].IsContainerLoose;
                DevanLookupWS.Cells[rc, 9].Value = DevanLookups[i].MaximumSKUCount;

                if (ratecodes.Any(x => x.ID == DevanLookups[i].RateCodeID))
                {
                    DevanLookupWS.Cells[rc, 10].Value = ratecodes.FirstOrDefault(x => x.ID == DevanLookups[i].RateCodeID).RateCode;
                    DevanLookupWS.Cells[rc, 11].Value = ratecodes.FirstOrDefault(x => x.ID == DevanLookups[i].RateCodeID).Description;
                }
                else
                    DevanLookupWS.Cells[rc, 10].Value = "RateCodeID: " + DevanLookups[i].RateCodeID.ToString();

                DevanLookupWS.Cells[rc, 12].Value = DevanLookups[i].CreatedDateTime.ToOADate();
                DevanLookupWS.Cells[rc, 13].Value = DevanLookups[i].CreatedMethod;

                if (users.Any(x => x.ID == DevanLookups[i].CreatedUserID))
                {
                    DevanLookupWS.Cells[rc, 14].Value = users.FirstOrDefault(x => x.ID == DevanLookups[i].CreatedUserID).Firstname;
                    DevanLookupWS.Cells[rc, 15].Value = users.FirstOrDefault(x => x.ID == DevanLookups[i].CreatedUserID).PublicID;
                }
                else
                    DevanLookupWS.Cells[rc, 14].Value = "UserID: " + DevanLookups[i].CreatedUserID.ToString();

                DevanLookupWS.Cells[rc, 16].Value = DevanLookups[i].LastAmendedDateTime.ToOADate();
                DevanLookupWS.Cells[rc, 17].Value = DevanLookups[i].LastAmendedMethod;

                if (users.Any(x => x.ID == DevanLookups[i].LastAmendedUserID))
                {
                    DevanLookupWS.Cells[rc, 18].Value = users.FirstOrDefault(x => x.ID == DevanLookups[i].LastAmendedUserID).Firstname;
                    DevanLookupWS.Cells[rc, 19].Value = users.FirstOrDefault(x => x.ID == DevanLookups[i].LastAmendedUserID).PublicID;
                }
                else
                    DevanLookupWS.Cells[rc, 18].Value = "UserID: " + DevanLookups[i].LastAmendedUserID.ToString();

                DevanLookupWS.Cells[rc, 20].Value = DevanLookups[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
