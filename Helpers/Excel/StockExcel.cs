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
    public class StockExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Company.dbRow> companies = null;
        public List<wbm_common.DataObjects.Site.dbRow> sites = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.Owner.dbRow> owners = null;
        public List<wbm_common.DataObjects.StockRateCategory.dbRow> stockRateCategories = null;
        public List<wbm_common.DataObjects.PalletStorageType.dbRow> palletStorageTypes = null;
        public List<wbm_common.DataObjects.Stock.dbRow> Stocks = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.Stock.Search StockSearchData;
        public StockExcel(wbm_common.DataObjects.Stock.Search StockSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.StockSearchData = StockSearchData;
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
                        StockPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("StockExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.Stock.dbRow>> rc = await StockHelper.ExecuteSearch(StockSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            Stocks = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListOwnerID = Stocks.Select(x => x.OwnerID).Distinct().ToList();
            List<int> ListStockRateCategories = Stocks.Select(x => x.StockRateCategoryID).Distinct().ToList();
            List<int> ListPalletStorageTypes = Stocks.Select(x => x.PalletStorageTypeID).Distinct().ToList();
            List<int> ListCompanyIDs = Stocks.Select(x => x.CompanyID).Distinct().ToList();
            List<int> ListSiteID = Stocks.Select(x => x.SiteID).Distinct().ToList();
            List<int> ListUserID = Stocks.Select(x => x.CreatedUserID).Union(Stocks.Select(x => x.LastAmendedUserID)).Distinct().ToList();

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

            if (ListPalletStorageTypes.Count > 0)
            {
                Result<List<wbm_common.DataObjects.PalletStorageType.dbRow>> palletStorageTypesResults = _WBMDB.PalletStorageType_ListByListIDs(ListPalletStorageTypes).GetAwaiter().GetResult();
                if (palletStorageTypesResults.IsFailure || palletStorageTypesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                palletStorageTypes = palletStorageTypesResults.Value;
            }

            if (ListStockRateCategories.Count > 0)
            {
                Result<List<wbm_common.DataObjects.StockRateCategory.dbRow>> stockratecategoriesResults = _WBMDB.StockRateCategory_ListByListIDs(ListStockRateCategories).GetAwaiter().GetResult();
                if (stockratecategoriesResults.IsFailure || stockratecategoriesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                stockRateCategories = stockratecategoriesResults.Value;
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

        private void StockPage(ExcelPackage ep)
        {
            ExcelWorksheet StockWS = ep.Workbook.Worksheets.Add("Stock");

            #region header
            StockWS.View.FreezePanes(2, 1);
            StockWS.Cells[1, 1, 1, 28].Style.Font.Bold = true;
            StockWS.Cells[1, 1, 1, 28].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            StockWS.Column(1).Width = 15;
            StockWS.Column(2).Width = 25;
            StockWS.Column(3).Width = 25;
            StockWS.Column(4).Width = 25;
            StockWS.Column(5).Width = 25;
            StockWS.Column(6).Width = 25;
            StockWS.Column(7).Width = 25;
            StockWS.Column(8).Width = 25;
            StockWS.Column(9).Width = 25;
            StockWS.Column(10).Width = 25;
            StockWS.Column(11).Width = 25;
            StockWS.Column(12).Width = 25;
            StockWS.Column(13).Width = 30;
            StockWS.Column(14).Width = 25;
            StockWS.Column(15).Width = 25;
            StockWS.Column(16).Width = 25;
            StockWS.Column(17).Width = 25;
            StockWS.Column(18).Width = 25;
            StockWS.Column(19).Width = 25;
            StockWS.Column(20).Width = 25;
            StockWS.Column(20).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            StockWS.Column(21).Width = 25;
            StockWS.Column(22).Width = 25;
            StockWS.Column(23).Width = 25;
            StockWS.Column(24).Width = 25;
            StockWS.Column(24).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            StockWS.Column(25).Width = 25;
            StockWS.Column(26).Width = 25;

            StockWS.Cells[1, 1].Value = "Company";
            StockWS.Cells[1, 2].Value = "Company Name";
            StockWS.Cells[1, 3].Value = "Owner ID";
            StockWS.Cells[1, 4].Value = "Owner Name";
            StockWS.Cells[1, 5].Value = "Site";
            StockWS.Cells[1, 6].Value = "Site Description";
            StockWS.Cells[1, 7].Value = "Product Code";
            StockWS.Cells[1, 8].Value = "Description";
            StockWS.Cells[1, 9].Value = "Category ID";
            StockWS.Cells[1, 10].Value = "Category Description";
            StockWS.Cells[1, 11].Value = "Length";
            StockWS.Cells[1, 12].Value = "Width";
            StockWS.Cells[1, 13].Value = "Height";
            StockWS.Cells[1, 14].Value = "Weight";
            StockWS.Cells[1, 15].Value = "Pallet Storage Type";
            StockWS.Cells[1, 16].Value = "Pallet Storage Type Description";
            StockWS.Cells[1, 17].Value = "Stock Rate Category";
            StockWS.Cells[1, 18].Value = "Stock Rate Category Description";
            StockWS.Cells[1, 19].Value = "Items Per Pallet";
            StockWS.Cells[1, 20].Value = "Created DateTime";
            StockWS.Cells[1, 21].Value = "Created Method";
            StockWS.Cells[1, 22].Value = "Created User";
            StockWS.Cells[1, 23].Value = "Created User Name";
            StockWS.Cells[1, 24].Value = "Last Amended DateTime";
            StockWS.Cells[1, 25].Value = "Last Amended Method";
            StockWS.Cells[1, 26].Value = "Last Amended User";
            StockWS.Cells[1, 27].Value = "Last Amended Name";
            StockWS.Cells[1, 28].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < Stocks.Count(); i++)
            {
                if (companies.Any(x => x.ID == Stocks[i].CompanyID))
                {
                    StockWS.Cells[rc, 1].Value = companies.FirstOrDefault(x => x.ID == Stocks[i].CompanyID).ShortName;
                    StockWS.Cells[rc, 2].Value = companies.FirstOrDefault(x => x.ID == Stocks[i].CompanyID).LongName;
                }
                else
                    StockWS.Cells[rc, 1].Value = "CompanyID: " + Stocks[i].CompanyID.ToString();

                if (owners.Any(x => x.ID == Stocks[i].OwnerID))
                {
                    StockWS.Cells[rc, 3].Value = owners.FirstOrDefault(x => x.ID == Stocks[i].OwnerID).SourceOwnerID;
                    StockWS.Cells[rc, 4].Value = owners.FirstOrDefault(x => x.ID == Stocks[i].OwnerID).Name;
                }
                else
                    StockWS.Cells[rc, 3].Value = "OwnerID: " + Stocks[i].OwnerID.ToString();

                if (sites.Any(x => x.ID == Stocks[i].SiteID))
                {
                    StockWS.Cells[rc, 5].Value = sites.FirstOrDefault(x => x.ID == Stocks[i].SiteID).PublicID;
                    StockWS.Cells[rc, 6].Value = sites.FirstOrDefault(x => x.ID == Stocks[i].SiteID).Description;
                }
                else
                    StockWS.Cells[rc, 5].Value = "SiteID: " + Stocks[i].SiteID.ToString();

                StockWS.Cells[rc, 7].Value = Stocks[i].ProductCode;
                StockWS.Cells[rc, 8].Value = Stocks[i].Description;

                if (stockRateCategories.Any(x => x.ID == Stocks[i].StockRateCategoryID))
                {
                    StockWS.Cells[rc, 9].Value = stockRateCategories.FirstOrDefault(x => x.ID == Stocks[i].StockRateCategoryID).PublicID;
                    StockWS.Cells[rc, 10].Value = stockRateCategories.FirstOrDefault(x => x.ID == Stocks[i].StockRateCategoryID).Description;
                }
                else
                    StockWS.Cells[rc, 9].Value = "StockRateCategoryID: " + Stocks[i].StockRateCategoryID.ToString();

                StockWS.Cells[rc, 11].Value = Stocks[i].Length;
                StockWS.Cells[rc, 12].Value = Stocks[i].Width;
                StockWS.Cells[rc, 13].Value = Stocks[i].Height;
                StockWS.Cells[rc, 14].Value = Stocks[i].Weight;

                if (palletStorageTypes.Any(x => x.ID == Stocks[i].PalletStorageTypeID))
                {
                    StockWS.Cells[rc, 15].Value = palletStorageTypes.FirstOrDefault(x => x.ID == Stocks[i].PalletStorageTypeID).PublicID;
                    StockWS.Cells[rc, 16].Value = palletStorageTypes.FirstOrDefault(x => x.ID == Stocks[i].PalletStorageTypeID).Description;
                }
                else
                    StockWS.Cells[rc, 15].Value = "PalletStorageTypeID: " + Stocks[i].PalletStorageTypeID.ToString();

                if (stockRateCategories.Any(x => x.ID == Stocks[i].StockRateCategoryID))
                {
                    StockWS.Cells[rc, 17].Value = stockRateCategories.FirstOrDefault(x => x.ID == Stocks[i].StockRateCategoryID).PublicID;
                    StockWS.Cells[rc, 18].Value = stockRateCategories.FirstOrDefault(x => x.ID == Stocks[i].StockRateCategoryID).Description;
                }
                else
                    StockWS.Cells[rc, 17].Value = "StockRateCategoryID: " + Stocks[i].StockRateCategoryID.ToString();

                StockWS.Cells[rc, 19].Value = Stocks[i].ItemsPerPallet;

                StockWS.Cells[rc, 20].Value = Stocks[i].CreatedDateTime.ToOADate();
                StockWS.Cells[rc, 21].Value = Stocks[i].CreatedMethod;

                if (users.Any(x => x.ID == Stocks[i].CreatedUserID))
                {
                    StockWS.Cells[rc, 22].Value = users.FirstOrDefault(x => x.ID == Stocks[i].CreatedUserID).Firstname;
                    StockWS.Cells[rc, 23].Value = users.FirstOrDefault(x => x.ID == Stocks[i].CreatedUserID).PublicID;
                }
                else
                    StockWS.Cells[rc, 22].Value = "UserID: " + Stocks[i].CreatedUserID.ToString();

                StockWS.Cells[rc, 24].Value = Stocks[i].LastAmendedDateTime.ToOADate();
                StockWS.Cells[rc, 25].Value = Stocks[i].LastAmendedMethod;

                if (users.Any(x => x.ID == Stocks[i].LastAmendedUserID))
                {
                    StockWS.Cells[rc, 26].Value = users.FirstOrDefault(x => x.ID == Stocks[i].LastAmendedUserID).Firstname;
                    StockWS.Cells[rc, 27].Value = users.FirstOrDefault(x => x.ID == Stocks[i].LastAmendedUserID).PublicID;
                }
                else
                    StockWS.Cells[rc, 26].Value = "UserID: " + Stocks[i].LastAmendedUserID.ToString();

                StockWS.Cells[rc, 28].Value = Stocks[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
