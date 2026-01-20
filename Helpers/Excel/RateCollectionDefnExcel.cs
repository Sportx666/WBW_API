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
    public class RateCollectionDefnExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Company.dbRow> companies = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.RateCollectionDefn.dbRow> RateCollectionDefns = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.RateCollectionDefn.Search RateCollectionDefnSearchData;
        public RateCollectionDefnExcel(wbm_common.DataObjects.RateCollectionDefn.Search RateCollectionDefnSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.RateCollectionDefnSearchData = RateCollectionDefnSearchData;
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
                        RateCollectionDefnPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("RateCollectionDefnExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.RateCollectionDefn.dbRow>> rc = await RateCollectionDefnHelper.ExecuteSearch(RateCollectionDefnSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            RateCollectionDefns = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = RateCollectionDefns.Select(x => x.CreatedUserID).Union(RateCollectionDefns.Select(x => x.LastAmendedUserID)).Distinct().ToList();
            List<int> ListCompanyIDs = RateCollectionDefns.Select(x => x.CompanyID).Distinct().ToList();

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

        private void RateCollectionDefnPage(ExcelPackage ep)
        {
            ExcelWorksheet RateCollectionDefnWS = ep.Workbook.Worksheets.Add("RateCollectionDefn");

            #region header
            RateCollectionDefnWS.View.FreezePanes(2, 1);
            RateCollectionDefnWS.Cells[1, 1, 1, 14].Style.Font.Bold = true;
            RateCollectionDefnWS.Cells[1, 1, 1, 14].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            RateCollectionDefnWS.Column(1).Width = 20;
            RateCollectionDefnWS.Column(2).Width = 20;
            RateCollectionDefnWS.Column(3).Width = 15;
            RateCollectionDefnWS.Column(4).Width = 20;
            RateCollectionDefnWS.Column(5).Width = 20;
            RateCollectionDefnWS.Column(6).Width = 20;
            RateCollectionDefnWS.Column(6).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            RateCollectionDefnWS.Column(7).Width = 20;
            RateCollectionDefnWS.Column(8).Width = 20;
            RateCollectionDefnWS.Column(9).Width = 25;
            RateCollectionDefnWS.Column(10).Width = 20;
            RateCollectionDefnWS.Column(10).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            RateCollectionDefnWS.Column(11).Width = 25;
            RateCollectionDefnWS.Column(12).Width = 25;
            RateCollectionDefnWS.Column(13).Width = 25;
            RateCollectionDefnWS.Column(14).Width = 20;

            RateCollectionDefnWS.Cells[1, 1].Value = "Company";
            RateCollectionDefnWS.Cells[1, 2].Value = "Company Name";
            RateCollectionDefnWS.Cells[1, 3].Value = "ID";
            RateCollectionDefnWS.Cells[1, 4].Value = "Description";
            RateCollectionDefnWS.Cells[1, 5].Value = "Sort Order";
            RateCollectionDefnWS.Cells[1, 6].Value = "Created DateTime";
            RateCollectionDefnWS.Cells[1, 7].Value = "Created Method";
            RateCollectionDefnWS.Cells[1, 8].Value = "Created User";
            RateCollectionDefnWS.Cells[1, 9].Value = "Created User Name";
            RateCollectionDefnWS.Cells[1, 10].Value = "Last Amended DateTime";
            RateCollectionDefnWS.Cells[1, 11].Value = "Last Amended Method";
            RateCollectionDefnWS.Cells[1, 12].Value = "Last Amended User";
            RateCollectionDefnWS.Cells[1, 13].Value = "Last Amended Name";
            RateCollectionDefnWS.Cells[1, 14].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < RateCollectionDefns.Count(); i++)
            {
                if (companies.Any(x => x.ID == RateCollectionDefns[i].CompanyID))
                {
                    RateCollectionDefnWS.Cells[rc, 1].Value = companies.FirstOrDefault(x => x.ID == RateCollectionDefns[i].CompanyID).ShortName;
                    RateCollectionDefnWS.Cells[rc, 2].Value = companies.FirstOrDefault(x => x.ID == RateCollectionDefns[i].CompanyID).LongName;
                }
                else
                    RateCollectionDefnWS.Cells[rc, 1].Value = "CompanyID: " + RateCollectionDefns[i].CompanyID.ToString();

                RateCollectionDefnWS.Cells[rc, 3].Value = RateCollectionDefns[i].PublicID;
                RateCollectionDefnWS.Cells[rc, 4].Value = RateCollectionDefns[i].Description;
                RateCollectionDefnWS.Cells[rc, 5].Value = RateCollectionDefns[i].SortOrder;

                RateCollectionDefnWS.Cells[rc, 6].Value = RateCollectionDefns[i].CreatedDateTime.ToOADate();
                RateCollectionDefnWS.Cells[rc, 7].Value = RateCollectionDefns[i].CreatedMethod;

                if (users.Any(x => x.ID == RateCollectionDefns[i].CreatedUserID))
                {
                    RateCollectionDefnWS.Cells[rc, 8].Value = users.FirstOrDefault(x => x.ID == RateCollectionDefns[i].CreatedUserID).Firstname;
                    RateCollectionDefnWS.Cells[rc, 9].Value = users.FirstOrDefault(x => x.ID == RateCollectionDefns[i].CreatedUserID).PublicID;
                }
                else
                    RateCollectionDefnWS.Cells[rc, 8].Value = "UserID: " + RateCollectionDefns[i].CreatedUserID.ToString();

                RateCollectionDefnWS.Cells[rc, 10].Value = RateCollectionDefns[i].LastAmendedDateTime.ToOADate();
                RateCollectionDefnWS.Cells[rc, 11].Value = RateCollectionDefns[i].LastAmendedMethod;

                if (users.Any(x => x.ID == RateCollectionDefns[i].LastAmendedUserID))
                {
                    RateCollectionDefnWS.Cells[rc, 12].Value = users.FirstOrDefault(x => x.ID == RateCollectionDefns[i].LastAmendedUserID).Firstname;
                    RateCollectionDefnWS.Cells[rc, 13].Value = users.FirstOrDefault(x => x.ID == RateCollectionDefns[i].LastAmendedUserID).PublicID;
                }
                else
                    RateCollectionDefnWS.Cells[rc, 12].Value = "UserID: " + RateCollectionDefns[i].LastAmendedUserID.ToString();

                RateCollectionDefnWS.Cells[rc, 14].Value = RateCollectionDefns[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
