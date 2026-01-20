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
    public class ConnoteFeeExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Carrier.dbRow> carriers = null;
        public List<wbm_common.DataObjects.Company.dbRow> companies = null;
        public List<wbm_common.DataObjects.Site.dbRow> sites = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.Owner.dbRow> owners = null;
        public List<wbm_common.DataObjects.RateCode.dbRow> ratecodes = null;
        public List<wbm_common.DataObjects.ConnoteFee.dbRow> ConnoteFees = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.ConnoteFee.Search ConnoteFeeSearchData;
        public ConnoteFeeExcel(wbm_common.DataObjects.ConnoteFee.Search ConnoteFeeSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.ConnoteFeeSearchData = ConnoteFeeSearchData;
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
                        ConnoteFeePage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("ConnoteFeeExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.ConnoteFee.dbRow>> rc = await ConnoteFeeHelper.ExecuteSearch(ConnoteFeeSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            ConnoteFees = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListCarrierID = ConnoteFees.Select(x => x.CarrierID).Distinct().ToList();
            List<int> ListOwnerID = ConnoteFees.Select(x => x.OwnerID).Distinct().ToList();
            List<int> ListRateCodes = ConnoteFees.Select(x => x.RateCodeID).Distinct().ToList();
            List<int> ListCompanyIDs = ConnoteFees.Select(x => x.CompanyID).Distinct().ToList();
            List<int> ListSiteID = ConnoteFees.Select(x => x.SiteID).Distinct().ToList();
            List<int> ListUserID = ConnoteFees.Select(x => x.CreatedUserID).Union(ConnoteFees.Select(x => x.LastAmendedUserID)).Distinct().ToList();

            if (ListCarrierID.Count > 0)
            {
                Result<List<wbm_common.DataObjects.Carrier.dbRow>> carriersResults = _WBMDB.Carrier_ListByListIDs(ListCarrierID).GetAwaiter().GetResult();
                if (carriersResults.IsFailure || carriersResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                carriers = carriersResults.Value;
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

        private void ConnoteFeePage(ExcelPackage ep)
        {
            ExcelWorksheet ConnoteFeeWS = ep.Workbook.Worksheets.Add("ConnoteFee");

            #region header
            ConnoteFeeWS.View.FreezePanes(2, 1);
            ConnoteFeeWS.Cells[1, 1, 1, 20].Style.Font.Bold = true;
            ConnoteFeeWS.Cells[1, 1, 1, 20].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            ConnoteFeeWS.Column(1).Width = 12;
            ConnoteFeeWS.Column(2).Width = 25;
            ConnoteFeeWS.Column(3).Width = 12;
            ConnoteFeeWS.Column(4).Width = 15;
            ConnoteFeeWS.Column(5).Width = 10;
            ConnoteFeeWS.Column(6).Width = 25;
            ConnoteFeeWS.Column(7).Width = 20;
            ConnoteFeeWS.Column(8).Width = 15;
            ConnoteFeeWS.Column(9).Width = 25;
            ConnoteFeeWS.Column(10).Width = 10;
            ConnoteFeeWS.Column(11).Width = 25;
            ConnoteFeeWS.Column(12).Width = 25;
            ConnoteFeeWS.Column(12).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ConnoteFeeWS.Column(13).Width = 30;
            ConnoteFeeWS.Column(14).Width = 15;
            ConnoteFeeWS.Column(15).Width = 25;
            ConnoteFeeWS.Column(16).Width = 25;
            ConnoteFeeWS.Column(16).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ConnoteFeeWS.Column(17).Width = 25;
            ConnoteFeeWS.Column(18).Width = 25;
            ConnoteFeeWS.Column(19).Width = 25;
            ConnoteFeeWS.Column(20).Width = 25;

            ConnoteFeeWS.Cells[1, 1].Value = "Company";
            ConnoteFeeWS.Cells[1, 2].Value = "Company Name";
            ConnoteFeeWS.Cells[1, 3].Value = "Owner ID";
            ConnoteFeeWS.Cells[1, 4].Value = "Owner Name";
            ConnoteFeeWS.Cells[1, 5].Value = "Site";
            ConnoteFeeWS.Cells[1, 6].Value = "Site Description";
            ConnoteFeeWS.Cells[1, 7].Value = "Customer Number";
            ConnoteFeeWS.Cells[1, 8].Value = "Carrier";
            ConnoteFeeWS.Cells[1, 9].Value = "Carrier Description";
            ConnoteFeeWS.Cells[1, 10].Value = "Rate Code";
            ConnoteFeeWS.Cells[1, 11].Value = "Rate Code Description";
            ConnoteFeeWS.Cells[1, 12].Value = "Created DateTime";
            ConnoteFeeWS.Cells[1, 13].Value = "Created Method";
            ConnoteFeeWS.Cells[1, 14].Value = "Created User";
            ConnoteFeeWS.Cells[1, 15].Value = "Created User Name";
            ConnoteFeeWS.Cells[1, 16].Value = "Last Amended DateTime";
            ConnoteFeeWS.Cells[1, 17].Value = "Last Amended Method";
            ConnoteFeeWS.Cells[1, 18].Value = "Last Amended User";
            ConnoteFeeWS.Cells[1, 19].Value = "Last Amended Name";
            ConnoteFeeWS.Cells[1, 20].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < ConnoteFees.Count(); i++)
            {
                if (companies.Any(x => x.ID == ConnoteFees[i].CompanyID))
                {
                    ConnoteFeeWS.Cells[rc, 1].Value = companies.FirstOrDefault(x => x.ID == ConnoteFees[i].CompanyID).ShortName;
                    ConnoteFeeWS.Cells[rc, 2].Value = companies.FirstOrDefault(x => x.ID == ConnoteFees[i].CompanyID).LongName;
                }
                else
                    ConnoteFeeWS.Cells[rc, 1].Value = "CompanyID: " + ConnoteFees[i].CompanyID.ToString();

                if (owners.Any(x => x.ID == ConnoteFees[i].OwnerID))
                {
                    ConnoteFeeWS.Cells[rc, 3].Value = owners.FirstOrDefault(x => x.ID == ConnoteFees[i].OwnerID).SourceOwnerID;
                    ConnoteFeeWS.Cells[rc, 4].Value = owners.FirstOrDefault(x => x.ID == ConnoteFees[i].OwnerID).Name;
                }
                else
                    ConnoteFeeWS.Cells[rc, 3].Value = "OwnerID: " + ConnoteFees[i].OwnerID.ToString();

                if (sites.Any(x => x.ID == ConnoteFees[i].SiteID))
                {
                    ConnoteFeeWS.Cells[rc, 5].Value = sites.FirstOrDefault(x => x.ID == ConnoteFees[i].SiteID).PublicID;
                    ConnoteFeeWS.Cells[rc, 6].Value = sites.FirstOrDefault(x => x.ID == ConnoteFees[i].SiteID).Description;
                }
                else
                    ConnoteFeeWS.Cells[rc, 5].Value = "SiteID: " + ConnoteFees[i].SiteID.ToString();

                ConnoteFeeWS.Cells[rc, 7].Value = ConnoteFees[i].CustomerNumber;

                if (carriers.Any(x => x.ID == ConnoteFees[i].CarrierID))
                {
                    ConnoteFeeWS.Cells[rc, 8].Value = carriers.FirstOrDefault(x => x.ID == ConnoteFees[i].CarrierID).PublicID;
                    ConnoteFeeWS.Cells[rc, 9].Value = carriers.FirstOrDefault(x => x.ID == ConnoteFees[i].CarrierID).Description;
                }
                else
                    ConnoteFeeWS.Cells[rc, 8].Value = "CarrierID: " + ConnoteFees[i].CarrierID.ToString();

                if (ratecodes.Any(x => x.ID == ConnoteFees[i].RateCodeID))
                {
                    ConnoteFeeWS.Cells[rc, 10].Value = ratecodes.FirstOrDefault(x => x.ID == ConnoteFees[i].RateCodeID).RateCode;
                    ConnoteFeeWS.Cells[rc, 11].Value = ratecodes.FirstOrDefault(x => x.ID == ConnoteFees[i].RateCodeID).Description;
                }
                else
                    ConnoteFeeWS.Cells[rc, 10].Value = "RateCodeID: " + ConnoteFees[i].RateCodeID.ToString();

                ConnoteFeeWS.Cells[rc, 12].Value = ConnoteFees[i].CreatedDateTime.ToOADate();
                ConnoteFeeWS.Cells[rc, 13].Value = ConnoteFees[i].CreatedMethod;

                if (users.Any(x => x.ID == ConnoteFees[i].CreatedUserID))
                {
                    ConnoteFeeWS.Cells[rc, 14].Value = users.FirstOrDefault(x => x.ID == ConnoteFees[i].CreatedUserID).Firstname;
                    ConnoteFeeWS.Cells[rc, 15].Value = users.FirstOrDefault(x => x.ID == ConnoteFees[i].CreatedUserID).PublicID;
                }
                else
                    ConnoteFeeWS.Cells[rc, 14].Value = "UserID: " + ConnoteFees[i].CreatedUserID.ToString();

                ConnoteFeeWS.Cells[rc, 16].Value = ConnoteFees[i].LastAmendedDateTime.ToOADate();
                ConnoteFeeWS.Cells[rc, 17].Value = ConnoteFees[i].LastAmendedMethod;

                if (users.Any(x => x.ID == ConnoteFees[i].LastAmendedUserID))
                {
                    ConnoteFeeWS.Cells[rc, 18].Value = users.FirstOrDefault(x => x.ID == ConnoteFees[i].LastAmendedUserID).Firstname;
                    ConnoteFeeWS.Cells[rc, 19].Value = users.FirstOrDefault(x => x.ID == ConnoteFees[i].LastAmendedUserID).PublicID;
                }
                else
                    ConnoteFeeWS.Cells[rc, 18].Value = "UserID: " + ConnoteFees[i].LastAmendedUserID.ToString();

                ConnoteFeeWS.Cells[rc, 20].Value = ConnoteFees[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
