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
    public class ActionRateCodeExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Company.dbRow> companies = null;
        public List<wbm_common.DataObjects.Site.dbRow> sites = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.Owner.dbRow> owners = null;
        public List<wbm_common.DataObjects.ActionType.dbRow> actiontypes = null;
        public List<wbm_common.DataObjects.UnitType.dbRow> unittypes = null;
        public List<wbm_common.DataObjects.StockRateCategory.dbRow> stockratecategories = null;
        public List<wbm_common.DataObjects.RateCode.dbRow> ratecodes = null;
        public List<wbm_common.DataObjects.ActionRateCode.dbRow> ActionRateCodes = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.ActionRateCode.Search ActionRateCodeSearchData;
        public ActionRateCodeExcel(wbm_common.DataObjects.ActionRateCode.Search ActionRateCodeSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.ActionRateCodeSearchData = ActionRateCodeSearchData;
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
                        ActionRateCodePage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("ActionRateCodeExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.ActionRateCode.dbRow>> rc = await ActionRateCodeHelper.ExecuteSearch(ActionRateCodeSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            ActionRateCodes = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListOwnerID = ActionRateCodes.Select(x => x.OwnerID).Distinct().ToList();
            List<int> ListActionTypes = ActionRateCodes.Select(x => x.ActionTypeID).Distinct().ToList();
            List<int> ListUnitTypes = ActionRateCodes.Select(x => x.UnitTypeID).Distinct().ToList();
            List<int> ListStockRateCategories = ActionRateCodes.Select(x => x.StockRateCategoryID).Distinct().ToList();
            List<int> ListRateCodes = ActionRateCodes.Select(x => x.RateCodeID).Distinct().ToList();
            List<int> ListCompanyIDs = ActionRateCodes.Select(x => x.CompanyID).Distinct().ToList();
            List<int> ListSiteID = ActionRateCodes.Select(x => x.SiteID).Distinct().ToList();
            List<int> ListUserID = ActionRateCodes.Select(x => x.CreatedUserID).Union(ActionRateCodes.Select(x => x.LastAmendedUserID)).Distinct().ToList();

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

            if (ListActionTypes.Count > 0)
            {
                Result<List<wbm_common.DataObjects.ActionType.dbRow>> actiontypesResults = _WBMDB.ActionType_ListByListIDs(ListActionTypes).GetAwaiter().GetResult();
                if (actiontypesResults.IsFailure || actiontypesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                actiontypes = actiontypesResults.Value;
            }

            if (ListUnitTypes.Count > 0)
            {
                Result<List<wbm_common.DataObjects.UnitType.dbRow>> unittypesResults = _WBMDB.UnitType_ListByListIDs(ListUnitTypes).GetAwaiter().GetResult();
                if (unittypesResults.IsFailure || unittypesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                unittypes = unittypesResults.Value;
            }

            if (ListStockRateCategories.Count > 0)
            {
                Result<List<wbm_common.DataObjects.StockRateCategory.dbRow>> stockratecategoriesResults = _WBMDB.StockRateCategory_ListByListIDs(ListStockRateCategories).GetAwaiter().GetResult();
                if (stockratecategoriesResults.IsFailure || stockratecategoriesResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                stockratecategories = stockratecategoriesResults.Value;
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

        private void ActionRateCodePage(ExcelPackage ep)
        {
            ExcelWorksheet ActionRateCodeWS = ep.Workbook.Worksheets.Add("ActionRateCode");

            #region header
            ActionRateCodeWS.View.FreezePanes(2, 1);
            ActionRateCodeWS.Cells[1, 1, 1, 26].Style.Font.Bold = true;
            ActionRateCodeWS.Cells[1, 1, 1, 26].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            ActionRateCodeWS.Column(1).Width = 12;
            ActionRateCodeWS.Column(2).Width = 25;
            ActionRateCodeWS.Column(3).Width = 12;
            ActionRateCodeWS.Column(4).Width = 15;
            ActionRateCodeWS.Column(5).Width = 10;
            ActionRateCodeWS.Column(6).Width = 25;
            ActionRateCodeWS.Column(7).Width = 10;
            ActionRateCodeWS.Column(8).Width = 15;
            ActionRateCodeWS.Column(9).Width = 25;
            ActionRateCodeWS.Column(10).Width = 10;
            ActionRateCodeWS.Column(11).Width = 25;
            ActionRateCodeWS.Column(12).Width = 25;
            ActionRateCodeWS.Column(13).Width = 30;
            ActionRateCodeWS.Column(14).Width = 15;
            ActionRateCodeWS.Column(15).Width = 15;
            ActionRateCodeWS.Column(16).Width = 15;
            ActionRateCodeWS.Column(17).Width = 25;
            ActionRateCodeWS.Column(18).Width = 25;
            ActionRateCodeWS.Column(18).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ActionRateCodeWS.Column(19).Width = 25;
            ActionRateCodeWS.Column(20).Width = 25;
            ActionRateCodeWS.Column(21).Width = 25;
            ActionRateCodeWS.Column(22).Width = 25;
            ActionRateCodeWS.Column(22).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ActionRateCodeWS.Column(23).Width = 25;
            ActionRateCodeWS.Column(24).Width = 25;
            ActionRateCodeWS.Column(25).Width = 25;
            ActionRateCodeWS.Column(26).Width = 25;

            ActionRateCodeWS.Cells[1, 1].Value = "Company";
            ActionRateCodeWS.Cells[1, 2].Value = "Company Name";
            ActionRateCodeWS.Cells[1, 3].Value = "Owner ID";
            ActionRateCodeWS.Cells[1, 4].Value = "Owner Name";
            ActionRateCodeWS.Cells[1, 5].Value = "Site";
            ActionRateCodeWS.Cells[1, 6].Value = "Site Description";
            ActionRateCodeWS.Cells[1, 7].Value = "Is Priority";
            ActionRateCodeWS.Cells[1, 8].Value = "Action Type";
            ActionRateCodeWS.Cells[1, 9].Value = "Action Type Description";
            ActionRateCodeWS.Cells[1, 10].Value = "Unit Type";
            ActionRateCodeWS.Cells[1, 11].Value = "Unit Type Description";
            ActionRateCodeWS.Cells[1, 12].Value = "Stock Rate Category";
            ActionRateCodeWS.Cells[1, 13].Value = "Stock Rate Category Description";
            ActionRateCodeWS.Cells[1, 14].Value = "Weight Break";
            ActionRateCodeWS.Cells[1, 15].Value = "Length Break";
            ActionRateCodeWS.Cells[1, 16].Value = "Rate Code";
            ActionRateCodeWS.Cells[1, 17].Value = "Rate Code Description";
            ActionRateCodeWS.Cells[1, 18].Value = "Created DateTime";
            ActionRateCodeWS.Cells[1, 19].Value = "Created Method";
            ActionRateCodeWS.Cells[1, 20].Value = "Created User";
            ActionRateCodeWS.Cells[1, 21].Value = "Created User Name";
            ActionRateCodeWS.Cells[1, 22].Value = "Last Amended DateTime";
            ActionRateCodeWS.Cells[1, 23].Value = "Last Amended Method";
            ActionRateCodeWS.Cells[1, 24].Value = "Last Amended User";
            ActionRateCodeWS.Cells[1, 25].Value = "Last Amended Name";
            ActionRateCodeWS.Cells[1, 26].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < ActionRateCodes.Count(); i++)
            {
                if (companies.Any(x => x.ID == ActionRateCodes[i].CompanyID))
                {
                    ActionRateCodeWS.Cells[rc, 1].Value = companies.FirstOrDefault(x => x.ID == ActionRateCodes[i].CompanyID).ShortName;
                    ActionRateCodeWS.Cells[rc, 2].Value = companies.FirstOrDefault(x => x.ID == ActionRateCodes[i].CompanyID).LongName;
                }
                else
                    ActionRateCodeWS.Cells[rc, 1].Value = "CompanyID: " + ActionRateCodes[i].CompanyID.ToString();

                if (owners.Any(x => x.ID == ActionRateCodes[i].OwnerID))
                {
                    ActionRateCodeWS.Cells[rc, 3].Value = owners.FirstOrDefault(x => x.ID == ActionRateCodes[i].OwnerID).SourceOwnerID;
                    ActionRateCodeWS.Cells[rc, 4].Value = owners.FirstOrDefault(x => x.ID == ActionRateCodes[i].OwnerID).Name;
                }
                else
                    ActionRateCodeWS.Cells[rc, 3].Value = "OwnerID: " + ActionRateCodes[i].OwnerID.ToString();

                if (sites.Any(x => x.ID == ActionRateCodes[i].SiteID))
                {
                    ActionRateCodeWS.Cells[rc, 5].Value = sites.FirstOrDefault(x => x.ID == ActionRateCodes[i].SiteID).PublicID;
                    ActionRateCodeWS.Cells[rc, 6].Value = sites.FirstOrDefault(x => x.ID == ActionRateCodes[i].SiteID).Description;
                }
                else
                    ActionRateCodeWS.Cells[rc, 5].Value = "SiteID: " + ActionRateCodes[i].SiteID.ToString();

                ActionRateCodeWS.Cells[rc, 7].Value = ActionRateCodes[i].IsPriority;

                if (actiontypes.Any(x => x.ID == ActionRateCodes[i].ActionTypeID))
                {
                    ActionRateCodeWS.Cells[rc, 8].Value = actiontypes.FirstOrDefault(x => x.ID == ActionRateCodes[i].ActionTypeID).PublicID;
                    ActionRateCodeWS.Cells[rc, 9].Value = actiontypes.FirstOrDefault(x => x.ID == ActionRateCodes[i].ActionTypeID).Description;
                }
                else
                    ActionRateCodeWS.Cells[rc, 8].Value = "ActionTypeID: " + ActionRateCodes[i].ActionTypeID.ToString();

                if (unittypes.Any(x => x.ID == ActionRateCodes[i].UnitTypeID))
                {
                    ActionRateCodeWS.Cells[rc, 10].Value = unittypes.FirstOrDefault(x => x.ID == ActionRateCodes[i].UnitTypeID).PublicID;
                    ActionRateCodeWS.Cells[rc, 11].Value = unittypes.FirstOrDefault(x => x.ID == ActionRateCodes[i].UnitTypeID).Description;
                }
                else
                    ActionRateCodeWS.Cells[rc, 10].Value = "UnitTypeID: " + ActionRateCodes[i].UnitTypeID.ToString();

                if (stockratecategories.Any(x => x.ID == ActionRateCodes[i].StockRateCategoryID))
                {
                    ActionRateCodeWS.Cells[rc, 12].Value = stockratecategories.FirstOrDefault(x => x.ID == ActionRateCodes[i].StockRateCategoryID).PublicID;
                    ActionRateCodeWS.Cells[rc, 13].Value = stockratecategories.FirstOrDefault(x => x.ID == ActionRateCodes[i].StockRateCategoryID).Description;
                }
                else
                    ActionRateCodeWS.Cells[rc, 12].Value = "StockRateCategoryID: " + ActionRateCodes[i].StockRateCategoryID.ToString();

                ActionRateCodeWS.Cells[rc, 14].Value = ActionRateCodes[i].WeightBreak;
                ActionRateCodeWS.Cells[rc, 15].Value = ActionRateCodes[i].LengthBreak;

                if (ratecodes.Any(x => x.ID == ActionRateCodes[i].RateCodeID))
                {
                    ActionRateCodeWS.Cells[rc, 16].Value = ratecodes.FirstOrDefault(x => x.ID == ActionRateCodes[i].RateCodeID).RateCode;
                    ActionRateCodeWS.Cells[rc, 17].Value = ratecodes.FirstOrDefault(x => x.ID == ActionRateCodes[i].RateCodeID).Description;
                }
                else
                    ActionRateCodeWS.Cells[rc, 16].Value = "RateCodeID: " + ActionRateCodes[i].RateCodeID.ToString();


                ActionRateCodeWS.Cells[rc, 18].Value = ActionRateCodes[i].CreatedDateTime.ToOADate();
                ActionRateCodeWS.Cells[rc, 19].Value = ActionRateCodes[i].CreatedMethod;
                if (users.Any(x => x.ID == ActionRateCodes[i].CreatedUserID))
                {
                    ActionRateCodeWS.Cells[rc, 20].Value = users.FirstOrDefault(x => x.ID == ActionRateCodes[i].CreatedUserID).Firstname;
                    ActionRateCodeWS.Cells[rc, 21].Value = users.FirstOrDefault(x => x.ID == ActionRateCodes[i].CreatedUserID).PublicID;
                }
                else
                    ActionRateCodeWS.Cells[rc, 20].Value = "UserID: " + ActionRateCodes[i].CreatedUserID.ToString();
                ActionRateCodeWS.Cells[rc, 22].Value = ActionRateCodes[i].LastAmendedDateTime.ToOADate();
                ActionRateCodeWS.Cells[rc, 23].Value = ActionRateCodes[i].LastAmendedMethod;
                if (users.Any(x => x.ID == ActionRateCodes[i].LastAmendedUserID))
                {
                    ActionRateCodeWS.Cells[rc, 24].Value = users.FirstOrDefault(x => x.ID == ActionRateCodes[i].LastAmendedUserID).Firstname;
                    ActionRateCodeWS.Cells[rc, 25].Value = users.FirstOrDefault(x => x.ID == ActionRateCodes[i].LastAmendedUserID).PublicID;
                }
                else
                    ActionRateCodeWS.Cells[rc, 24].Value = "UserID: " + ActionRateCodes[i].LastAmendedUserID.ToString();

                ActionRateCodeWS.Cells[rc, 26].Value = ActionRateCodes[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
