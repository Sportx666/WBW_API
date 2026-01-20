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
    public class ContainerHeavyLiftExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Company.dbRow> companies = null;
        public List<wbm_common.DataObjects.Site.dbRow> sites = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.Owner.dbRow> owners = null;
        public List<wbm_common.DataObjects.RateCode.dbRow> ratecodes = null;
        public List<wbm_common.DataObjects.ContainerHeavyLift.dbRow> ContainerHeavyLifts = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.ContainerHeavyLift.Search ContainerHeavyLiftSearchData;
        public ContainerHeavyLiftExcel(wbm_common.DataObjects.ContainerHeavyLift.Search ContainerHeavyLiftSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.ContainerHeavyLiftSearchData = ContainerHeavyLiftSearchData;
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
                        ContainerHeavyLiftPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("ContainerHeavyLiftExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.ContainerHeavyLift.dbRow>> rc = await ContainerHeavyLiftHelper.ExecuteSearch(ContainerHeavyLiftSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            ContainerHeavyLifts = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListOwnerID = ContainerHeavyLifts.Select(x => x.OwnerID).Distinct().ToList();
            List<int> ListRateCodes = ContainerHeavyLifts.Select(x => x.RateCodeID).Distinct().ToList();
            List<int> ListCompanyIDs = ContainerHeavyLifts.Select(x => x.CompanyID).Distinct().ToList();
            List<int> ListSiteID = ContainerHeavyLifts.Select(x => x.SiteID).Distinct().ToList();
            List<int> ListUserID = ContainerHeavyLifts.Select(x => x.CreatedUserID).Union(ContainerHeavyLifts.Select(x => x.LastAmendedUserID)).Distinct().ToList();

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

        private void ContainerHeavyLiftPage(ExcelPackage ep)
        {
            ExcelWorksheet ContainerHeavyLiftWS = ep.Workbook.Worksheets.Add("ContainerHeavyLift");

            #region header
            ContainerHeavyLiftWS.View.FreezePanes(2, 1);
            ContainerHeavyLiftWS.Cells[1, 1, 1, 20].Style.Font.Bold = true;
            ContainerHeavyLiftWS.Cells[1, 1, 1, 20].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            ContainerHeavyLiftWS.Column(1).Width = 12;
            ContainerHeavyLiftWS.Column(2).Width = 25;
            ContainerHeavyLiftWS.Column(3).Width = 12;
            ContainerHeavyLiftWS.Column(4).Width = 15;
            ContainerHeavyLiftWS.Column(5).Width = 10;
            ContainerHeavyLiftWS.Column(6).Width = 25;
            ContainerHeavyLiftWS.Column(7).Width = 10;
            ContainerHeavyLiftWS.Column(8).Width = 15;
            ContainerHeavyLiftWS.Column(9).Width = 25;
            ContainerHeavyLiftWS.Column(10).Width = 10;
            ContainerHeavyLiftWS.Column(11).Width = 25;
            ContainerHeavyLiftWS.Column(12).Width = 25;
            ContainerHeavyLiftWS.Column(12).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ContainerHeavyLiftWS.Column(13).Width = 30;
            ContainerHeavyLiftWS.Column(14).Width = 15;
            ContainerHeavyLiftWS.Column(15).Width = 25;
            ContainerHeavyLiftWS.Column(16).Width = 25;
            ContainerHeavyLiftWS.Column(16).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ContainerHeavyLiftWS.Column(17).Width = 25;
            ContainerHeavyLiftWS.Column(18).Width = 25;
            ContainerHeavyLiftWS.Column(19).Width = 25;
            ContainerHeavyLiftWS.Column(20).Width = 25;

            ContainerHeavyLiftWS.Cells[1, 1].Value = "Company";
            ContainerHeavyLiftWS.Cells[1, 2].Value = "Company Name";
            ContainerHeavyLiftWS.Cells[1, 3].Value = "Owner ID";
            ContainerHeavyLiftWS.Cells[1, 4].Value = "Owner Name";
            ContainerHeavyLiftWS.Cells[1, 5].Value = "Site";
            ContainerHeavyLiftWS.Cells[1, 6].Value = "Site Description";
            ContainerHeavyLiftWS.Cells[1, 7].Value = "Min Weight";
            ContainerHeavyLiftWS.Cells[1, 8].Value = "Min Length";
            ContainerHeavyLiftWS.Cells[1, 9].Value = "Percentage";
            ContainerHeavyLiftWS.Cells[1, 10].Value = "Rate Code";
            ContainerHeavyLiftWS.Cells[1, 11].Value = "Rate Code Description";
            ContainerHeavyLiftWS.Cells[1, 12].Value = "Created DateTime";
            ContainerHeavyLiftWS.Cells[1, 13].Value = "Created Method";
            ContainerHeavyLiftWS.Cells[1, 14].Value = "Created User";
            ContainerHeavyLiftWS.Cells[1, 15].Value = "Created User Name";
            ContainerHeavyLiftWS.Cells[1, 16].Value = "Last Amended DateTime";
            ContainerHeavyLiftWS.Cells[1, 17].Value = "Last Amended Method";
            ContainerHeavyLiftWS.Cells[1, 18].Value = "Last Amended User";
            ContainerHeavyLiftWS.Cells[1, 19].Value = "Last Amended Name";
            ContainerHeavyLiftWS.Cells[1, 20].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < ContainerHeavyLifts.Count(); i++)
            {
                if (companies.Any(x => x.ID == ContainerHeavyLifts[i].CompanyID))
                {
                    ContainerHeavyLiftWS.Cells[rc, 1].Value = companies.FirstOrDefault(x => x.ID == ContainerHeavyLifts[i].CompanyID).ShortName;
                    ContainerHeavyLiftWS.Cells[rc, 2].Value = companies.FirstOrDefault(x => x.ID == ContainerHeavyLifts[i].CompanyID).LongName;
                }
                else
                    ContainerHeavyLiftWS.Cells[rc, 1].Value = "CompanyID: " + ContainerHeavyLifts[i].CompanyID.ToString();

                if (owners.Any(x => x.ID == ContainerHeavyLifts[i].OwnerID))
                {
                    ContainerHeavyLiftWS.Cells[rc, 3].Value = owners.FirstOrDefault(x => x.ID == ContainerHeavyLifts[i].OwnerID).SourceOwnerID;
                    ContainerHeavyLiftWS.Cells[rc, 4].Value = owners.FirstOrDefault(x => x.ID == ContainerHeavyLifts[i].OwnerID).Name;
                }
                else
                    ContainerHeavyLiftWS.Cells[rc, 3].Value = "OwnerID: " + ContainerHeavyLifts[i].OwnerID.ToString();

                if (sites.Any(x => x.ID == ContainerHeavyLifts[i].SiteID))
                {
                    ContainerHeavyLiftWS.Cells[rc, 5].Value = sites.FirstOrDefault(x => x.ID == ContainerHeavyLifts[i].SiteID).PublicID;
                    ContainerHeavyLiftWS.Cells[rc, 6].Value = sites.FirstOrDefault(x => x.ID == ContainerHeavyLifts[i].SiteID).Description;
                }
                else
                    ContainerHeavyLiftWS.Cells[rc, 5].Value = "SiteID: " + ContainerHeavyLifts[i].SiteID.ToString();

                ContainerHeavyLiftWS.Cells[rc, 7].Value = ContainerHeavyLifts[i].MinWeight;
                ContainerHeavyLiftWS.Cells[rc, 8].Value = ContainerHeavyLifts[i].MinLength;
                ContainerHeavyLiftWS.Cells[rc, 9].Value = ContainerHeavyLifts[i].Percentage;

                if (ratecodes.Any(x => x.ID == ContainerHeavyLifts[i].RateCodeID))
                {
                    ContainerHeavyLiftWS.Cells[rc, 10].Value = ratecodes.FirstOrDefault(x => x.ID == ContainerHeavyLifts[i].RateCodeID).RateCode;
                    ContainerHeavyLiftWS.Cells[rc, 11].Value = ratecodes.FirstOrDefault(x => x.ID == ContainerHeavyLifts[i].RateCodeID).Description;
                }
                else
                    ContainerHeavyLiftWS.Cells[rc, 10].Value = "RateCodeID: " + ContainerHeavyLifts[i].RateCodeID.ToString();


                ContainerHeavyLiftWS.Cells[rc, 12].Value = ContainerHeavyLifts[i].CreatedDateTime.ToOADate();
                ContainerHeavyLiftWS.Cells[rc, 13].Value = ContainerHeavyLifts[i].CreatedMethod;

                if (users.Any(x => x.ID == ContainerHeavyLifts[i].CreatedUserID))
                {
                    ContainerHeavyLiftWS.Cells[rc, 14].Value = users.FirstOrDefault(x => x.ID == ContainerHeavyLifts[i].CreatedUserID).Firstname;
                    ContainerHeavyLiftWS.Cells[rc, 15].Value = users.FirstOrDefault(x => x.ID == ContainerHeavyLifts[i].CreatedUserID).PublicID;
                }
                else
                    ContainerHeavyLiftWS.Cells[rc, 14].Value = "UserID: " + ContainerHeavyLifts[i].CreatedUserID.ToString();

                ContainerHeavyLiftWS.Cells[rc, 16].Value = ContainerHeavyLifts[i].LastAmendedDateTime.ToOADate();
                ContainerHeavyLiftWS.Cells[rc, 17].Value = ContainerHeavyLifts[i].LastAmendedMethod;

                if (users.Any(x => x.ID == ContainerHeavyLifts[i].LastAmendedUserID))
                {
                    ContainerHeavyLiftWS.Cells[rc, 18].Value = users.FirstOrDefault(x => x.ID == ContainerHeavyLifts[i].LastAmendedUserID).Firstname;
                    ContainerHeavyLiftWS.Cells[rc, 19].Value = users.FirstOrDefault(x => x.ID == ContainerHeavyLifts[i].LastAmendedUserID).PublicID;
                }
                else
                    ContainerHeavyLiftWS.Cells[rc, 18].Value = "UserID: " + ContainerHeavyLifts[i].LastAmendedUserID.ToString();

                ContainerHeavyLiftWS.Cells[rc, 20].Value = ContainerHeavyLifts[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
