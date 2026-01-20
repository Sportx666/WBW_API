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
    public class ProcessingRuleDefnExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Company.dbRow> companies = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.ProcessingRuleDefn.dbRow> ProcessingRuleDefns = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.ProcessingRuleDefn.Search ProcessingRuleDefnSearchData;
        public ProcessingRuleDefnExcel(wbm_common.DataObjects.ProcessingRuleDefn.Search ProcessingRuleDefnSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.ProcessingRuleDefnSearchData = ProcessingRuleDefnSearchData;
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
                        ProcessingRuleDefnPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("ProcessingRuleDefnExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.ProcessingRuleDefn.dbRow>> rc = await ProcessingRuleDefnHelper.ExecuteSearch(ProcessingRuleDefnSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            ProcessingRuleDefns = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = ProcessingRuleDefns.Select(x => x.CreatedUserID).Union(ProcessingRuleDefns.Select(x => x.LastAmendedUserID)).Distinct().ToList();
            List<int> ListCompanyIDs = ProcessingRuleDefns.Select(x => x.CompanyID).Distinct().ToList();

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

        private void ProcessingRuleDefnPage(ExcelPackage ep)
        {
            ExcelWorksheet ProcessingRuleDefnWS = ep.Workbook.Worksheets.Add("ProcessingRuleDefn");

            #region header
            ProcessingRuleDefnWS.View.FreezePanes(2, 1);
            ProcessingRuleDefnWS.Cells[1, 1, 1, 14].Style.Font.Bold = true;
            ProcessingRuleDefnWS.Cells[1, 1, 1, 14].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            ProcessingRuleDefnWS.Column(1).Width = 15;
            ProcessingRuleDefnWS.Column(2).Width = 20;
            ProcessingRuleDefnWS.Column(3).Width = 10;
            ProcessingRuleDefnWS.Column(4).Width = 25;
            ProcessingRuleDefnWS.Column(5).Width = 10;
            ProcessingRuleDefnWS.Column(6).Width = 25;
            ProcessingRuleDefnWS.Column(6).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ProcessingRuleDefnWS.Column(7).Width = 25;
            ProcessingRuleDefnWS.Column(8).Width = 25;
            ProcessingRuleDefnWS.Column(9).Width = 25;
            ProcessingRuleDefnWS.Column(10).Width = 25;
            ProcessingRuleDefnWS.Column(10).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ProcessingRuleDefnWS.Column(11).Width = 25;
            ProcessingRuleDefnWS.Column(12).Width = 25;
            ProcessingRuleDefnWS.Column(13).Width = 25;
            ProcessingRuleDefnWS.Column(14).Width = 15;

            ProcessingRuleDefnWS.Cells[1, 1].Value = "Company";
            ProcessingRuleDefnWS.Cells[1, 2].Value = "Company Name";
            ProcessingRuleDefnWS.Cells[1, 3].Value = "ID";
            ProcessingRuleDefnWS.Cells[1, 4].Value = "Description";
            ProcessingRuleDefnWS.Cells[1, 5].Value = "Sort Order";
            ProcessingRuleDefnWS.Cells[1, 6].Value = "Created DateTime";
            ProcessingRuleDefnWS.Cells[1, 7].Value = "Created Method";
            ProcessingRuleDefnWS.Cells[1, 8].Value = "Created User";
            ProcessingRuleDefnWS.Cells[1, 9].Value = "Created User Name";
            ProcessingRuleDefnWS.Cells[1, 10].Value = "Last Amended DateTime";
            ProcessingRuleDefnWS.Cells[1, 11].Value = "Last Amended Method";
            ProcessingRuleDefnWS.Cells[1, 12].Value = "Last Amended User";
            ProcessingRuleDefnWS.Cells[1, 13].Value = "Last Amended Name";
            ProcessingRuleDefnWS.Cells[1, 14].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            ProcessingRuleDefns = ProcessingRuleDefns.OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < ProcessingRuleDefns.Count(); i++)
            {
                if (companies.Any(x => x.ID == ProcessingRuleDefns[i].CompanyID))
                {
                    ProcessingRuleDefnWS.Cells[rc, 1].Value = companies.FirstOrDefault(x => x.ID == ProcessingRuleDefns[i].CompanyID).ShortName;
                    ProcessingRuleDefnWS.Cells[rc, 2].Value = companies.FirstOrDefault(x => x.ID == ProcessingRuleDefns[i].CompanyID).LongName;
                }
                else
                    ProcessingRuleDefnWS.Cells[rc, 1].Value = "CompanyID: " + ProcessingRuleDefns[i].CompanyID.ToString();

                ProcessingRuleDefnWS.Cells[rc, 3].Value = ProcessingRuleDefns[i].PublicID;
                ProcessingRuleDefnWS.Cells[rc, 4].Value = ProcessingRuleDefns[i].Description;
                ProcessingRuleDefnWS.Cells[rc, 5].Value = ProcessingRuleDefns[i].SortOrder;

                ProcessingRuleDefnWS.Cells[rc, 6].Value = ProcessingRuleDefns[i].CreatedDateTime.ToOADate();
                ProcessingRuleDefnWS.Cells[rc, 7].Value = ProcessingRuleDefns[i].CreatedMethod;

                if (users.Any(x => x.ID == ProcessingRuleDefns[i].CreatedUserID))
                {
                    ProcessingRuleDefnWS.Cells[rc, 8].Value = users.FirstOrDefault(x => x.ID == ProcessingRuleDefns[i].CreatedUserID).PublicID;
                    ProcessingRuleDefnWS.Cells[rc, 9].Value = users.FirstOrDefault(x => x.ID == ProcessingRuleDefns[i].CreatedUserID).Firstname;
                }
                else
                    ProcessingRuleDefnWS.Cells[rc, 8].Value = "UserID: " + ProcessingRuleDefns[i].CreatedUserID.ToString();

                ProcessingRuleDefnWS.Cells[rc, 10].Value = ProcessingRuleDefns[i].LastAmendedDateTime.ToOADate();
                ProcessingRuleDefnWS.Cells[rc, 11].Value = ProcessingRuleDefns[i].LastAmendedMethod;

                if (users.Any(x => x.ID == ProcessingRuleDefns[i].LastAmendedUserID))
                {
                    ProcessingRuleDefnWS.Cells[rc, 12].Value = users.FirstOrDefault(x => x.ID == ProcessingRuleDefns[i].LastAmendedUserID).PublicID;
                    ProcessingRuleDefnWS.Cells[rc, 13].Value = users.FirstOrDefault(x => x.ID == ProcessingRuleDefns[i].LastAmendedUserID).Firstname;
                }
                else
                    ProcessingRuleDefnWS.Cells[rc, 12].Value = "UserID: " + ProcessingRuleDefns[i].LastAmendedUserID.ToString();

                ProcessingRuleDefnWS.Cells[rc, 14].Value = ProcessingRuleDefns[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
