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
    public class ProcessingRuleExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Company.dbRow> companies = null;
        public List<wbm_common.DataObjects.Site.dbRow> sites = null;
        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.ProcessingRuleProcess.dbRow> processingruleprocess = null;
        public List<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow> triggers = null;
        public List<wbm_common.DataObjects.ProcessingRuleDefn.dbRow> processingrulesdefns = null;
        public List<wbm_common.DataObjects.ProcessingRule.dbRow> ProcessingRules = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.ProcessingRule.Search ProcessingRuleSearchData;
        public ProcessingRuleExcel(wbm_common.DataObjects.ProcessingRule.Search ProcessingRuleSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.ProcessingRuleSearchData = ProcessingRuleSearchData;
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
                        ProcessingRulePage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("ProcessingRuleExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.ProcessingRule.dbRow>> rc = await ProcessingRuleHelper.ExecuteSearch(ProcessingRuleSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            ProcessingRules = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListTriggerID = ProcessingRules.Select(x => x.TriggerID).Distinct().ToList();
            List<int> ListProcessingRuleProcessIDs = ProcessingRules.Select(x => x.ProcessID).Distinct().ToList();
            List<int> ListProcessingDefnIDs = ProcessingRules.Select(x => x.ProcessingRuleDefnID).Distinct().ToList();
            List<int> ListCompanyIDs = ProcessingRules.Select(x => x.CompanyID).Distinct().ToList();
            List<int> ListSiteID = ProcessingRules.Select(x => x.SiteID).Distinct().ToList();
            List<int> ListUserID = ProcessingRules.Select(x => x.CreatedUserID).Union(ProcessingRules.Select(x => x.LastAmendedUserID)).Distinct().ToList();

            if (ListProcessingRuleProcessIDs.Count > 0)
            {
                Result<List<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>> processingruleprocessResults = _WBMDB.ProcessingRuleProcess_ListByListIDs(ListProcessingRuleProcessIDs).GetAwaiter().GetResult();
                if (processingruleprocessResults.IsFailure || processingruleprocessResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                processingruleprocess = processingruleprocessResults.Value;
            }

            if (ListTriggerID.Count > 0)
            {
                Result<List<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>> triggersResults = _WBMDB.ProcessingRuleTriggerCondition_ListByListIDs(ListTriggerID).GetAwaiter().GetResult();
                if (triggersResults.IsFailure || triggersResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                triggers = triggersResults.Value;
            }

            if (ListProcessingDefnIDs.Count > 0)
            {
                Result<List<wbm_common.DataObjects.ProcessingRuleDefn.dbRow>> processingrulesdefnsResults = _WBMDB.ProcessingRuleDefn_ListByListIDs(ListProcessingDefnIDs).GetAwaiter().GetResult();
                if (processingrulesdefnsResults.IsFailure || processingrulesdefnsResults.IsException)
                {
                    ErrorMessage = ErrorCodes.GeneralDB.ToString();
                    return false;
                }
                processingrulesdefns = processingrulesdefnsResults.Value;
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

        private void ProcessingRulePage(ExcelPackage ep)
        {
            ExcelWorksheet ProcessingRuleWS = ep.Workbook.Worksheets.Add("ProcessingRule");

            #region header
            ProcessingRuleWS.View.FreezePanes(2, 1);
            ProcessingRuleWS.Cells[1, 1, 1, 20].Style.Font.Bold = true;
            ProcessingRuleWS.Cells[1, 1, 1, 20].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            ProcessingRuleWS.Column(1).Width = 12;
            ProcessingRuleWS.Column(2).Width = 25;
            ProcessingRuleWS.Column(3).Width = 12;
            ProcessingRuleWS.Column(4).Width = 25;
            ProcessingRuleWS.Column(5).Width = 10;
            ProcessingRuleWS.Column(6).Width = 30;
            ProcessingRuleWS.Column(7).Width = 10;
            ProcessingRuleWS.Column(8).Width = 25;
            ProcessingRuleWS.Column(9).Width = 25;
            ProcessingRuleWS.Column(10).Width = 10;
            ProcessingRuleWS.Column(11).Width = 25;
            ProcessingRuleWS.Column(12).Width = 25;
            ProcessingRuleWS.Column(12).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ProcessingRuleWS.Column(13).Width = 25;
            ProcessingRuleWS.Column(14).Width = 25;
            ProcessingRuleWS.Column(15).Width = 25;
            ProcessingRuleWS.Column(16).Width = 25;
            ProcessingRuleWS.Column(16).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ProcessingRuleWS.Column(17).Width = 25;
            ProcessingRuleWS.Column(18).Width = 25;
            ProcessingRuleWS.Column(19).Width = 25;
            ProcessingRuleWS.Column(20).Width = 15;

            ProcessingRuleWS.Cells[1, 1].Value = "Company";
            ProcessingRuleWS.Cells[1, 2].Value = "Company Name";
            ProcessingRuleWS.Cells[1, 3].Value = "Site";
            ProcessingRuleWS.Cells[1, 4].Value = "Site Description";
            ProcessingRuleWS.Cells[1, 5].Value = "Processing Rule Defn";
            ProcessingRuleWS.Cells[1, 6].Value = "Processing Rule Defn Description";
            ProcessingRuleWS.Cells[1, 7].Value = "Trigger ID";
            ProcessingRuleWS.Cells[1, 8].Value = "Trigger Description";
            ProcessingRuleWS.Cells[1, 9].Value = "Sort Order";
            ProcessingRuleWS.Cells[1, 10].Value = "Process ID";
            ProcessingRuleWS.Cells[1, 11].Value = "Process Description";
            ProcessingRuleWS.Cells[1, 12].Value = "Created DateTime";
            ProcessingRuleWS.Cells[1, 13].Value = "Created Method";
            ProcessingRuleWS.Cells[1, 14].Value = "Created User";
            ProcessingRuleWS.Cells[1, 15].Value = "Created User Name";
            ProcessingRuleWS.Cells[1, 16].Value = "Last Amended DateTime";
            ProcessingRuleWS.Cells[1, 17].Value = "Last Amended Method";
            ProcessingRuleWS.Cells[1, 18].Value = "Last Amended User";
            ProcessingRuleWS.Cells[1, 19].Value = "Last Amended Name";
            ProcessingRuleWS.Cells[1, 20].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < ProcessingRules.Count(); i++)
            {
                if (companies.Any(x => x.ID == ProcessingRules[i].CompanyID))
                {
                    ProcessingRuleWS.Cells[rc, 1].Value = companies.FirstOrDefault(x => x.ID == ProcessingRules[i].CompanyID).ShortName;
                    ProcessingRuleWS.Cells[rc, 2].Value = companies.FirstOrDefault(x => x.ID == ProcessingRules[i].CompanyID).LongName;
                }
                else
                    ProcessingRuleWS.Cells[rc, 1].Value = "CompanyID: " + ProcessingRules[i].CompanyID.ToString();

                if (sites.Any(x => x.ID == ProcessingRules[i].SiteID))
                {
                    ProcessingRuleWS.Cells[rc, 3].Value = sites.FirstOrDefault(x => x.ID == ProcessingRules[i].SiteID).PublicID;
                    ProcessingRuleWS.Cells[rc, 4].Value = sites.FirstOrDefault(x => x.ID == ProcessingRules[i].SiteID).Description;
                }
                else
                    ProcessingRuleWS.Cells[rc, 3].Value = "SiteID: " + ProcessingRules[i].SiteID.ToString();

                if (processingrulesdefns.Any(x => x.ID == ProcessingRules[i].ProcessingRuleDefnID))
                {
                    ProcessingRuleWS.Cells[rc, 5].Value = processingrulesdefns.FirstOrDefault(x => x.ID == ProcessingRules[i].ProcessingRuleDefnID).PublicID;
                    ProcessingRuleWS.Cells[rc, 6].Value = processingrulesdefns.FirstOrDefault(x => x.ID == ProcessingRules[i].ProcessingRuleDefnID).Description;
                }
                else
                    ProcessingRuleWS.Cells[rc, 5].Value = "ProcessingRuleDefnID: " + ProcessingRules[i].ProcessingRuleDefnID.ToString();

                if (triggers.Any(x => x.ID == ProcessingRules[i].TriggerID))
                {
                    ProcessingRuleWS.Cells[rc, 7].Value = triggers.FirstOrDefault(x => x.ID == ProcessingRules[i].TriggerID).PublicID;
                    ProcessingRuleWS.Cells[rc, 8].Value = triggers.FirstOrDefault(x => x.ID == ProcessingRules[i].TriggerID).Description;
                }
                else
                    ProcessingRuleWS.Cells[rc, 7].Value = "TriggerID: " + ProcessingRules[i].TriggerID.ToString();

                ProcessingRuleWS.Cells[rc, 9].Value = ProcessingRules[i].SortOrder;

                if (processingruleprocess.Any(x => x.ID == ProcessingRules[i].ProcessID))
                {
                    ProcessingRuleWS.Cells[rc, 10].Value = processingruleprocess.FirstOrDefault(x => x.ID == ProcessingRules[i].ProcessID).PublicID;
                    ProcessingRuleWS.Cells[rc, 11].Value = processingruleprocess.FirstOrDefault(x => x.ID == ProcessingRules[i].ProcessID).Description;
                }
                else
                    ProcessingRuleWS.Cells[rc, 10].Value = "ProcessID: " + ProcessingRules[i].ProcessID.ToString();

                ProcessingRuleWS.Cells[rc, 12].Value = ProcessingRules[i].CreatedDateTime.ToOADate();
                ProcessingRuleWS.Cells[rc, 13].Value = ProcessingRules[i].CreatedMethod;

                if (users.Any(x => x.ID == ProcessingRules[i].CreatedUserID))
                {
                    ProcessingRuleWS.Cells[rc, 14].Value = users.FirstOrDefault(x => x.ID == ProcessingRules[i].CreatedUserID).Firstname;
                    ProcessingRuleWS.Cells[rc, 15].Value = users.FirstOrDefault(x => x.ID == ProcessingRules[i].CreatedUserID).PublicID;
                }
                else
                    ProcessingRuleWS.Cells[rc, 14].Value = "UserID: " + ProcessingRules[i].CreatedUserID.ToString();

                ProcessingRuleWS.Cells[rc, 16].Value = ProcessingRules[i].LastAmendedDateTime.ToOADate();
                ProcessingRuleWS.Cells[rc, 17].Value = ProcessingRules[i].LastAmendedMethod;

                if (users.Any(x => x.ID == ProcessingRules[i].LastAmendedUserID))
                {
                    ProcessingRuleWS.Cells[rc, 18].Value = users.FirstOrDefault(x => x.ID == ProcessingRules[i].LastAmendedUserID).Firstname;
                    ProcessingRuleWS.Cells[rc, 19].Value = users.FirstOrDefault(x => x.ID == ProcessingRules[i].LastAmendedUserID).PublicID;
                }
                else
                    ProcessingRuleWS.Cells[rc, 18].Value = "UserID: " + ProcessingRules[i].LastAmendedUserID.ToString();

                ProcessingRuleWS.Cells[rc, 20].Value = ProcessingRules[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
