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
    public class RateCollectionExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Company.dbRow> companies = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.RateFunction.dbRow> ratefunctions = null;
        public List<wbm_common.DataObjects.RateCode.dbRow> ratecodes = null;
        public List<wbm_common.DataObjects.RateCollection.dbRow> RateCollections = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.RateCollection.Search RateCollectionSearchData;
        public RateCollectionExcel(wbm_common.DataObjects.RateCollection.Search RateCollectionSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.RateCollectionSearchData = RateCollectionSearchData;
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
                        RateCollectionPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("RateCollectionExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.RateCollection.dbRow>> rc = await RateCollectionHelper.ExecuteSearch(RateCollectionSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            RateCollections = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListRateFunctions = RateCollections.Select(x => x.RateFunctionID).Distinct().ToList();
            List<int> ListRateCodes = RateCollections.Select(x => x.RateCodeID).Distinct().ToList();
            List<int> ListCompanyIDs = RateCollections.Select(x => x.CompanyID).Distinct().ToList();
            List<int> ListUserID = RateCollections.Select(x => x.CreatedUserID).Union(RateCollections.Select(x => x.LastAmendedUserID)).Distinct().ToList();

            if (ListRateFunctions.Count > 0)
            {
                Result<List<wbm_common.DataObjects.RateFunction.dbRow>> ratefunctionsResults = _WBMDB.RateFunction_ListByListIDs(ListRateFunctions).GetAwaiter().GetResult();
                if (ratefunctionsResults.IsFailure || ratefunctionsResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                ratefunctions = ratefunctionsResults.Value;
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

        private void RateCollectionPage(ExcelPackage ep)
        {
            ExcelWorksheet RateCollectionWS = ep.Workbook.Worksheets.Add("RateCollection");

            #region header
            RateCollectionWS.View.FreezePanes(2, 1);
            RateCollectionWS.Cells[1, 1, 1, 21].Style.Font.Bold = true;
            RateCollectionWS.Cells[1, 1, 1, 21].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            RateCollectionWS.Column(1).Width = 20;
            RateCollectionWS.Column(2).Width = 15;
            RateCollectionWS.Column(3).Width = 20;
            RateCollectionWS.Column(4).Width = 15;
            RateCollectionWS.Column(5).Width = 15;
            RateCollectionWS.Column(6).Width = 15;
            RateCollectionWS.Column(7).Width = 20;
            RateCollectionWS.Column(8).Width = 15;
            RateCollectionWS.Column(9).Width = 25;
            RateCollectionWS.Column(10).Width = 15;
            RateCollectionWS.Column(10).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            RateCollectionWS.Column(11).Width = 25;
            RateCollectionWS.Column(12).Width = 25;
            RateCollectionWS.Column(13).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            RateCollectionWS.Column(13).Width = 30;
            RateCollectionWS.Column(14).Width = 20;
            RateCollectionWS.Column(15).Width = 15;
            RateCollectionWS.Column(16).Width = 20;
            RateCollectionWS.Column(17).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            RateCollectionWS.Column(17).Width = 25;
            RateCollectionWS.Column(18).Width = 25;
            RateCollectionWS.Column(19).Width = 25;
            RateCollectionWS.Column(20).Width = 25;
            RateCollectionWS.Column(21).Width = 15;

            RateCollectionWS.Cells[1, 1].Value = "Rate Collection ID";
            RateCollectionWS.Cells[1, 2].Value = "Company";
            RateCollectionWS.Cells[1, 3].Value = "Company Name";
            RateCollectionWS.Cells[1, 4].Value = "Rate Function";
            RateCollectionWS.Cells[1, 5].Value = "Rate Function Description";
            RateCollectionWS.Cells[1, 6].Value = "Rate Code";
            RateCollectionWS.Cells[1, 7].Value = "Rate Code Description";
            RateCollectionWS.Cells[1, 8].Value = "Description";
            RateCollectionWS.Cells[1, 9].Value = "Secondary Sort";
            RateCollectionWS.Cells[1, 10].Value = "Date Of Change";
            RateCollectionWS.Cells[1, 11].Value = "New Rate Code ID";
            RateCollectionWS.Cells[1, 12].Value = "New Rate Code Description";
            RateCollectionWS.Cells[1, 13].Value = "Created DateTime";
            RateCollectionWS.Cells[1, 14].Value = "Created Method";
            RateCollectionWS.Cells[1, 15].Value = "Created User";
            RateCollectionWS.Cells[1, 16].Value = "Created User Name";
            RateCollectionWS.Cells[1, 17].Value = "Last Amended DateTime";
            RateCollectionWS.Cells[1, 18].Value = "Last Amended Method";
            RateCollectionWS.Cells[1, 19].Value = "Last Amended User";
            RateCollectionWS.Cells[1, 20].Value = "Last Amended Name";
            RateCollectionWS.Cells[1, 21].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < RateCollections.Count(); i++)
            {
                RateCollectionWS.Cells[rc, 1].Value = RateCollections[i].ID;

                if (companies.Any(x => x.ID == RateCollections[i].CompanyID))
                {
                    RateCollectionWS.Cells[rc, 2].Value = companies.FirstOrDefault(x => x.ID == RateCollections[i].CompanyID).ShortName;
                    RateCollectionWS.Cells[rc, 3].Value = companies.FirstOrDefault(x => x.ID == RateCollections[i].CompanyID).LongName;
                }
                else
                    RateCollectionWS.Cells[rc, 2].Value = "CompanyID: " + RateCollections[i].CompanyID.ToString();

                if (ratefunctions.Any(x => x.ID == RateCollections[i].RateFunctionID))
                {
                    RateCollectionWS.Cells[rc, 4].Value = ratefunctions.FirstOrDefault(x => x.ID == RateCollections[i].RateFunctionID).PublicID;
                    RateCollectionWS.Cells[rc, 5].Value = ratefunctions.FirstOrDefault(x => x.ID == RateCollections[i].RateFunctionID).Description;
                }
                else
                    RateCollectionWS.Cells[rc, 4].Value = "RateFunctionID: " + RateCollections[i].RateFunctionID.ToString();

                if (ratecodes.Any(x => x.ID == RateCollections[i].RateCodeID))
                {
                    RateCollectionWS.Cells[rc, 6].Value = ratecodes.FirstOrDefault(x => x.ID == RateCollections[i].RateCodeID).RateCode;
                    RateCollectionWS.Cells[rc, 7].Value = ratecodes.FirstOrDefault(x => x.ID == RateCollections[i].RateCodeID).Description;
                }
                else
                    RateCollectionWS.Cells[rc, 6].Value = "RateCodeID: " + RateCollections[i].RateCodeID.ToString();

                RateCollectionWS.Cells[rc, 8].Value = RateCollections[i].Description;
                RateCollectionWS.Cells[rc, 9].Value = RateCollections[i].SecondarySort;
                RateCollectionWS.Cells[rc, 10].Value = RateCollections[i].DateOfChange;

                if (ratecodes.Any(x => x.ID == RateCollections[i].NewRateCodeID))
                {
                    RateCollectionWS.Cells[rc, 11].Value = ratecodes.FirstOrDefault(x => x.ID == RateCollections[i].NewRateCodeID).RateCode;
                    RateCollectionWS.Cells[rc, 12].Value = ratecodes.FirstOrDefault(x => x.ID == RateCollections[i].NewRateCodeID).Description;
                }
                else
                    RateCollectionWS.Cells[rc, 11].Value = "NewRateCodeID: " + RateCollections[i].NewRateCodeID.ToString();

                RateCollectionWS.Cells[rc, 13].Value = RateCollections[i].CreatedDateTime.ToOADate();
                RateCollectionWS.Cells[rc, 14].Value = RateCollections[i].CreatedMethod;

                if (users.Any(x => x.ID == RateCollections[i].CreatedUserID))
                {
                    RateCollectionWS.Cells[rc, 15].Value = users.FirstOrDefault(x => x.ID == RateCollections[i].CreatedUserID).Firstname;
                    RateCollectionWS.Cells[rc, 16].Value = users.FirstOrDefault(x => x.ID == RateCollections[i].CreatedUserID).PublicID;
                }
                else
                    RateCollectionWS.Cells[rc, 15].Value = "UserID: " + RateCollections[i].CreatedUserID.ToString();

                RateCollectionWS.Cells[rc, 17].Value = RateCollections[i].LastAmendedDateTime.ToOADate();
                RateCollectionWS.Cells[rc, 18].Value = RateCollections[i].LastAmendedMethod;

                if (users.Any(x => x.ID == RateCollections[i].LastAmendedUserID))
                {
                    RateCollectionWS.Cells[rc, 19].Value = users.FirstOrDefault(x => x.ID == RateCollections[i].LastAmendedUserID).Firstname;
                    RateCollectionWS.Cells[rc, 20].Value = users.FirstOrDefault(x => x.ID == RateCollections[i].LastAmendedUserID).PublicID;
                }
                else
                    RateCollectionWS.Cells[rc, 19].Value = "UserID: " + RateCollections[i].LastAmendedUserID.ToString();

                RateCollectionWS.Cells[rc, 21].Value = RateCollections[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
