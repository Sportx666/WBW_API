using Microsoft.AspNetCore.SignalR;
using NLog;
using NLog.Web;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using wbm_common.NonDatabaseObjects;
using WBM_API.WBM_API_DB;
using wbm_common.DataObjects;


namespace WBM_API.Helpers.Excel
{
    public class StockOnHandExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Company.dbRowExtended> companies = null;
        public List<wbm_common.DataObjects.Site.dbRowExtended> sites = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.Owner.dbRow> owners = null;
        public List<wbm_common.DataObjects.StockOnHand.dbRowExtended> StockOnHands = null;
        public List<wbm_common.DataObjects.StorageRateCodeFrom.dbRow> storageratecodefroms = null;
        public List<wbm_common.DataObjects.Stock.dbRowExtended> stocks = null;
        public List<wbm_common.DataObjects.Location.dbRowExtended> locations = null;
        public List<wbm_common.DataObjects.RateCode.dbRow> ratecodes = null;
        public List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended> zones = null;
        public List<wbm_common.DataObjects.PalletType.dbRowExtended> pallettypes = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.StockOnHand.Search StockOnHandSearchData;
        public StockOnHandExcel(wbm_common.DataObjects.StockOnHand.Search StockOnHandSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.StockOnHandSearchData = StockOnHandSearchData;
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
                        StockOnHandPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("StockOnHandExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.StockOnHand.dbRowExtended>> rc = await StockOnHandHelper.ExecuteSearch(StockOnHandSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            StockOnHands = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListOwnerID = StockOnHands.Select(x => x.OwnerID).Distinct().ToList();
            List<int> ListStockIDs = StockOnHands.Select(x => x.StockID).Distinct().ToList();
            List<int> ListLocationIDs = StockOnHands.Select(x => x.LocationID).Distinct().ToList();
            List<int> ListStorageRateCodeFromIDs = StockOnHands.Select(x => x.StorageRateCodeFromID).Distinct().ToList();
            List<int> ListRateCodes = StockOnHands.Select(x => x.RateCodeID).Distinct().ToList();
            List<int> ListCompanyIDs = StockOnHands.Select(x => x.CompanyID).Distinct().ToList();
            List<int> ListSiteID = StockOnHands.Select(x => x.SiteID).Distinct().ToList();
            List<int> ListUserID = StockOnHands.Select(x => x.CreatedUserID).Union(StockOnHands.Select(x => x.LastAmendedUserID)).Distinct().ToList();
            List<int> ListZoneProcessingIDs = StockOnHands.Select(x => x.ZoneProcessingID).Distinct().ToList();
            List<int> ListPalletTypeIDs = StockOnHands.Select(x => x.PalletTypeID).Distinct().ToList();

            if (ListLocationIDs.Count > 0)
            {
                Result<List<wbm_common.DataObjects.Location.dbRowExtended>> locationsResults = _WBMDB.Location_ListByListIDs(ListLocationIDs).GetAwaiter().GetResult();
                if (locationsResults.IsFailure || locationsResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                locations = locationsResults.Value;
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

            if (ListStockIDs.Count > 0)
            {
                Result<List<wbm_common.DataObjects.Stock.dbRowExtended>> stocksResults = _WBMDB.Stock_ListByListIDs(ListStockIDs).GetAwaiter().GetResult();
                if (stocksResults.IsFailure || stocksResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                stocks = stocksResults.Value;
            }

            if (ListStorageRateCodeFromIDs.Count > 0)
            {
                Result<List<wbm_common.DataObjects.StorageRateCodeFrom.dbRow>> storageratecodefromsResults = _WBMDB.StorageRateCodeFrom_ListByListIDs(ListStorageRateCodeFromIDs).GetAwaiter().GetResult();
                if (storageratecodefromsResults.IsFailure || storageratecodefromsResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                storageratecodefroms = storageratecodefromsResults.Value;
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
                Result<List<wbm_common.DataObjects.Company.dbRowExtended>> companiesResults = _WBMDB.Company_ListByListIDs(ListCompanyIDs).GetAwaiter().GetResult();
                if (companiesResults.IsFailure || companiesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                companies = companiesResults.Value;
            }

            if (ListSiteID.Count > 0)
            {
                Result<List<wbm_common.DataObjects.Site.dbRowExtended>> sitesResults = _WBMDB.Site_ListByListIDs(ListSiteID).GetAwaiter().GetResult();
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

            if (ListZoneProcessingIDs.Count > 0)
            {
                Result<List<wbm_common.DataObjects.ZoneProcessing.dbRowExtended>> zonesResults = _WBMDB.ZoneProcessing_ListByListIDs(ListZoneProcessingIDs).GetAwaiter().GetResult();
                if (zonesResults.IsFailure || zonesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                zones = zonesResults.Value;
            }

            if (ListPalletTypeIDs.Count > 0)
            {
                Result<List<wbm_common.DataObjects.PalletType.dbRowExtended>> pallettypesResults = _WBMDB.PalletType_ListByListIDs(ListPalletTypeIDs).GetAwaiter().GetResult();
                if (pallettypesResults.IsFailure || pallettypesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                pallettypes = pallettypesResults.Value;
            }

            return true;
        }

        private void StockOnHandPage(ExcelPackage ep)
        {
            ExcelWorksheet StockOnHandWS = ep.Workbook.Worksheets.Add("StockOnHand");

            #region header
            StockOnHandWS.View.FreezePanes(2, 1);
            StockOnHandWS.Cells[1, 1, 1, 34].Style.Font.Bold = true;
            StockOnHandWS.Cells[1, 1, 1, 34].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            StockOnHandWS.Column(1).Width = 25;
            StockOnHandWS.Column(2).Width = 25;
            StockOnHandWS.Column(3).Width = 25;
            StockOnHandWS.Column(4).Width = 25;
            StockOnHandWS.Column(5).Width = 25;
            StockOnHandWS.Column(6).Width = 25;
            StockOnHandWS.Column(7).Width = 25;
            StockOnHandWS.Column(8).Width = 25;
            StockOnHandWS.Column(9).Width = 25;
            StockOnHandWS.Column(10).Width = 10;
            StockOnHandWS.Column(11).Width = 25;
            StockOnHandWS.Column(12).Width = 25;
            StockOnHandWS.Column(13).Width = 30;
            StockOnHandWS.Column(14).Width = 25;
            StockOnHandWS.Column(14).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            StockOnHandWS.Column(15).Width = 25;
            StockOnHandWS.Column(16).Width = 25;
            StockOnHandWS.Column(17).Width = 25;
            StockOnHandWS.Column(18).Width = 25;
            StockOnHandWS.Column(18).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            StockOnHandWS.Column(19).Width = 25;
            StockOnHandWS.Column(20).Width = 25;
            StockOnHandWS.Column(21).Width = 25;
            StockOnHandWS.Column(22).Width = 15;
            StockOnHandWS.Column(23).Width = 25;
            StockOnHandWS.Column(24).Width = 10;
            StockOnHandWS.Column(25).Width = 25;
            StockOnHandWS.Column(26).Width = 20;
            StockOnHandWS.Column(27).Width = 15;
            StockOnHandWS.Column(27).Style.Numberformat.Format = "dd/mm/yy";
            StockOnHandWS.Column(28).Width = 15;
            StockOnHandWS.Column(29).Width = 15;
            StockOnHandWS.Column(30).Width = 15;
            StockOnHandWS.Column(31).Width = 15;
            StockOnHandWS.Column(32).Width = 15;
            StockOnHandWS.Column(33).Width = 10;
            StockOnHandWS.Column(34).Width = 25;

            StockOnHandWS.Cells[1, 1].Value = "Company";
            StockOnHandWS.Cells[1, 2].Value = "Company Name";
            StockOnHandWS.Cells[1, 3].Value = "Owner ID";
            StockOnHandWS.Cells[1, 4].Value = "Owner Name";
            StockOnHandWS.Cells[1, 5].Value = "Site";
            StockOnHandWS.Cells[1, 6].Value = "Site Description";
            StockOnHandWS.Cells[1, 7].Value = "Location ID";
            StockOnHandWS.Cells[1, 8].Value = "Stock Product Code";
            StockOnHandWS.Cells[1, 9].Value = "Quantity";
            StockOnHandWS.Cells[1, 10].Value = "Rate Code";
            StockOnHandWS.Cells[1, 11].Value = "Rate Code Description";
            StockOnHandWS.Cells[1, 12].Value = "Stock Rate From";
            StockOnHandWS.Cells[1, 13].Value = "Stock Rate From Description";
            StockOnHandWS.Cells[1, 14].Value = "Created DateTime";
            StockOnHandWS.Cells[1, 15].Value = "Created Method";
            StockOnHandWS.Cells[1, 16].Value = "Created User";
            StockOnHandWS.Cells[1, 17].Value = "Created User Name";
            StockOnHandWS.Cells[1, 18].Value = "Last Amended DateTime";
            StockOnHandWS.Cells[1, 19].Value = "Last Amended Method";
            StockOnHandWS.Cells[1, 20].Value = "Last Amended User";
            StockOnHandWS.Cells[1, 21].Value = "Last Amended Name";
            StockOnHandWS.Cells[1, 22].Value = "Deleted Flag";
            StockOnHandWS.Cells[1, 23].Value = "Pallet Number";
            StockOnHandWS.Cells[1, 24].Value = "Zone";
            StockOnHandWS.Cells[1, 25].Value = "Zone Description";
            StockOnHandWS.Cells[1, 26].Value = "Status";
            StockOnHandWS.Cells[1, 27].Value = "Use-by Date";
            StockOnHandWS.Cells[1, 28].Value = "Lot Number";
            StockOnHandWS.Cells[1, 29].Value = "Pallet Weight";
            StockOnHandWS.Cells[1, 30].Value = "Pallet Width";
            StockOnHandWS.Cells[1, 31].Value = "Pallet Depth";
            StockOnHandWS.Cells[1, 32].Value = "Pallet Height";
            StockOnHandWS.Cells[1, 33].Value = "Pallet Type";
            StockOnHandWS.Cells[1, 34].Value = "Pallet Type Description";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < StockOnHands.Count(); i++)
            {
                if (companies.Any(x => x.ID == StockOnHands[i].CompanyID))
                {
                    StockOnHandWS.Cells[rc, 1].Value = companies.FirstOrDefault(x => x.ID == StockOnHands[i].CompanyID).ShortName;
                    StockOnHandWS.Cells[rc, 2].Value = companies.FirstOrDefault(x => x.ID == StockOnHands[i].CompanyID).LongName;
                }
                else
                    StockOnHandWS.Cells[rc, 1].Value = "CompanyID: " + StockOnHands[i].CompanyID.ToString();

                if (owners.Any(x => x.ID == StockOnHands[i].OwnerID))
                {
                    StockOnHandWS.Cells[rc, 3].Value = owners.FirstOrDefault(x => x.ID == StockOnHands[i].OwnerID).SourceOwnerID;
                    StockOnHandWS.Cells[rc, 4].Value = owners.FirstOrDefault(x => x.ID == StockOnHands[i].OwnerID).Name;
                }
                else
                    StockOnHandWS.Cells[rc, 3].Value = "OwnerID: " + StockOnHands[i].OwnerID.ToString();

                if (sites.Any(x => x.ID == StockOnHands[i].SiteID))
                {
                    StockOnHandWS.Cells[rc, 5].Value = sites.FirstOrDefault(x => x.ID == StockOnHands[i].SiteID).PublicID;
                    StockOnHandWS.Cells[rc, 6].Value = sites.FirstOrDefault(x => x.ID == StockOnHands[i].SiteID).Description;
                }
                else
                    StockOnHandWS.Cells[rc, 5].Value = "SiteID: " + StockOnHands[i].SiteID.ToString();

                if (locations.Any(x => x.ID == StockOnHands[i].LocationID))
                    StockOnHandWS.Cells[rc, 7].Value = locations.FirstOrDefault(x => x.ID == StockOnHands[i].LocationID).PublicLocationID;
                else
                    StockOnHandWS.Cells[rc, 7].Value = "LocationID: " + StockOnHands[i].LocationID.ToString();

                if (stocks.Any(x => x.ID == StockOnHands[i].StockID))
                    StockOnHandWS.Cells[rc, 8].Value = stocks.FirstOrDefault(x => x.ID == StockOnHands[i].StockID).ProductCode;
                else
                    StockOnHandWS.Cells[rc, 8].Value = "StockID: " + StockOnHands[i].StockID.ToString();

                StockOnHandWS.Cells[rc, 9].Value = StockOnHands[i].Quantity;

                if (ratecodes.Any(x => x.ID == StockOnHands[i].RateCodeID))
                {
                    StockOnHandWS.Cells[rc, 10].Value = ratecodes.FirstOrDefault(x => x.ID == StockOnHands[i].RateCodeID).RateCode;
                    StockOnHandWS.Cells[rc, 11].Value = ratecodes.FirstOrDefault(x => x.ID == StockOnHands[i].RateCodeID).Description;
                }
                else
                    StockOnHandWS.Cells[rc, 10].Value = "RateCodeID: " + StockOnHands[i].RateCodeID.ToString();

                if (storageratecodefroms.Any(x => x.ID == StockOnHands[i].StorageRateCodeFromID))
                {
                    StockOnHandWS.Cells[rc, 12].Value = storageratecodefroms.FirstOrDefault(x => x.ID == StockOnHands[i].StorageRateCodeFromID).PublicID;
                    StockOnHandWS.Cells[rc, 13].Value = storageratecodefroms.FirstOrDefault(x => x.ID == StockOnHands[i].StorageRateCodeFromID).Description;
                }
                else
                    StockOnHandWS.Cells[rc, 12].Value = "StorageRateCodeFromID: " + StockOnHands[i].StorageRateCodeFromID.ToString();

                StockOnHandWS.Cells[rc, 14].Value = StockOnHands[i].CreatedDateTime.ToOADate();
                StockOnHandWS.Cells[rc, 15].Value = StockOnHands[i].CreatedMethod;

                if (users.Any(x => x.ID == StockOnHands[i].CreatedUserID))
                {
                    StockOnHandWS.Cells[rc, 16].Value = users.FirstOrDefault(x => x.ID == StockOnHands[i].CreatedUserID).Firstname;
                    StockOnHandWS.Cells[rc, 17].Value = users.FirstOrDefault(x => x.ID == StockOnHands[i].CreatedUserID).PublicID;
                }
                else
                    StockOnHandWS.Cells[rc, 16].Value = "UserID: " + StockOnHands[i].CreatedUserID.ToString();

                StockOnHandWS.Cells[rc, 18].Value = StockOnHands[i].LastAmendedDateTime.ToOADate();
                StockOnHandWS.Cells[rc, 19].Value = StockOnHands[i].LastAmendedMethod;

                if (users.Any(x => x.ID == StockOnHands[i].LastAmendedUserID))
                {
                    StockOnHandWS.Cells[rc, 20].Value = users.FirstOrDefault(x => x.ID == StockOnHands[i].LastAmendedUserID).Firstname;
                    StockOnHandWS.Cells[rc, 21].Value = users.FirstOrDefault(x => x.ID == StockOnHands[i].LastAmendedUserID).PublicID;
                }
                else
                    StockOnHandWS.Cells[rc, 20].Value = "UserID: " + StockOnHands[i].LastAmendedUserID.ToString();

                StockOnHandWS.Cells[rc, 22].Value = StockOnHands[i].DeletedFlag;
                StockOnHandWS.Cells[rc, 23].Value = StockOnHands[i].PalletNumber;

                if (zones.Any(x => x.ID == StockOnHands[i].ZoneProcessingID))
                {
                    StockOnHandWS.Cells[rc, 24].Value = zones.FirstOrDefault(x => x.ID == StockOnHands[i].ZoneProcessingID).ZoneNumber;
                    StockOnHandWS.Cells[rc, 25].Value = zones.FirstOrDefault(x => x.ID == StockOnHands[i].ZoneProcessingID).Description;
                }
                else
                    StockOnHandWS.Cells[rc, 24].Value = "ZoneProcessingID: " + StockOnHands[i].ZoneProcessingID.ToString();

                StockOnHandWS.Cells[rc, 26].Value = StockOnHands[i].Status;
                StockOnHandWS.Cells[rc, 27].Value = StockOnHands[i].UseByDate?.ToOADate();
                StockOnHandWS.Cells[rc, 28].Value = StockOnHands[i].LotNumber;
                StockOnHandWS.Cells[rc, 29].Value = StockOnHands[i].PalletWeight;
                StockOnHandWS.Cells[rc, 30].Value = StockOnHands[i].PalletWidth;
                StockOnHandWS.Cells[rc, 31].Value = StockOnHands[i].PalletDepth;
                StockOnHandWS.Cells[rc, 32].Value = StockOnHands[i].PalletHeight;

                if (pallettypes.Any(x => x.ID == StockOnHands[i].PalletTypeID))
                {
                    StockOnHandWS.Cells[rc, 33].Value = pallettypes.FirstOrDefault(x => x.ID == StockOnHands[i].PalletTypeID).PublicID;
                    StockOnHandWS.Cells[rc, 34].Value = pallettypes.FirstOrDefault(x => x.ID == StockOnHands[i].PalletTypeID).Description;
                }
                else
                    StockOnHandWS.Cells[rc, 33].Value = "PalletTypeID: " + StockOnHands[i].PalletTypeID.ToString();

                rc++;
            }
            #endregion
        }
    }
}
