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
    public class ProcessingRuleTriggerConditionExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow> ProcessingRuleTriggerConditions = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.ProcessingRuleTriggerCondition.Search ProcessingRuleTriggerConditionSearchData;
        public ProcessingRuleTriggerConditionExcel(wbm_common.DataObjects.ProcessingRuleTriggerCondition.Search ProcessingRuleTriggerConditionSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.ProcessingRuleTriggerConditionSearchData = ProcessingRuleTriggerConditionSearchData;
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
                        ProcessingRuleTriggerConditionPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("ProcessingRuleTriggerConditionExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.ProcessingRuleTriggerCondition.dbRow>> rc = await ProcessingRuleTriggerConditionHelper.ExecuteSearch(ProcessingRuleTriggerConditionSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            ProcessingRuleTriggerConditions = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = ProcessingRuleTriggerConditions.Select(x => x.CreatedUserID).Union(ProcessingRuleTriggerConditions.Select(x => x.LastAmendedUserID)).Distinct().ToList();


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

        private void ProcessingRuleTriggerConditionPage(ExcelPackage ep)
        {
            ExcelWorksheet ProcessingRuleTriggerConditionWS = ep.Workbook.Worksheets.Add("ProcessingRuleTriggerCondition");

            #region header
            ProcessingRuleTriggerConditionWS.View.FreezePanes(2, 1);
            ProcessingRuleTriggerConditionWS.Cells[1, 1, 1, 12].Style.Font.Bold = true;
            ProcessingRuleTriggerConditionWS.Cells[1, 1, 1, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            ProcessingRuleTriggerConditionWS.Column(1).Width = 15;
            ProcessingRuleTriggerConditionWS.Column(2).Width = 15;
            ProcessingRuleTriggerConditionWS.Column(3).Width = 10;
            ProcessingRuleTriggerConditionWS.Column(4).Width = 25;
            ProcessingRuleTriggerConditionWS.Column(4).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ProcessingRuleTriggerConditionWS.Column(5).Width = 25;
            ProcessingRuleTriggerConditionWS.Column(6).Width = 25;
            ProcessingRuleTriggerConditionWS.Column(7).Width = 25;
            ProcessingRuleTriggerConditionWS.Column(8).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ProcessingRuleTriggerConditionWS.Column(8).Width = 25;
            ProcessingRuleTriggerConditionWS.Column(9).Width = 25;
            ProcessingRuleTriggerConditionWS.Column(10).Width = 25;
            ProcessingRuleTriggerConditionWS.Column(11).Width = 25;
            ProcessingRuleTriggerConditionWS.Column(12).Width = 15;

            ProcessingRuleTriggerConditionWS.Cells[1, 1].Value = "ID";
            ProcessingRuleTriggerConditionWS.Cells[1, 2].Value = "Description";
            ProcessingRuleTriggerConditionWS.Cells[1, 3].Value = "Sort Order";
            ProcessingRuleTriggerConditionWS.Cells[1, 4].Value = "Created DateTime";
            ProcessingRuleTriggerConditionWS.Cells[1, 5].Value = "Created Method";
            ProcessingRuleTriggerConditionWS.Cells[1, 6].Value = "Created User";
            ProcessingRuleTriggerConditionWS.Cells[1, 7].Value = "Created User Name";
            ProcessingRuleTriggerConditionWS.Cells[1, 8].Value = "Last Amended DateTime";
            ProcessingRuleTriggerConditionWS.Cells[1, 9].Value = "Last Amended Method";
            ProcessingRuleTriggerConditionWS.Cells[1, 10].Value = "Last Amended User";
            ProcessingRuleTriggerConditionWS.Cells[1, 11].Value = "Last Amended Name";
            ProcessingRuleTriggerConditionWS.Cells[1, 12].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            ProcessingRuleTriggerConditions = ProcessingRuleTriggerConditions.OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < ProcessingRuleTriggerConditions.Count(); i++)
            {
                ProcessingRuleTriggerConditionWS.Cells[rc, 1].Value = ProcessingRuleTriggerConditions[i].PublicID;
                ProcessingRuleTriggerConditionWS.Cells[rc, 2].Value = ProcessingRuleTriggerConditions[i].Description;
                ProcessingRuleTriggerConditionWS.Cells[rc, 3].Value = ProcessingRuleTriggerConditions[i].SortOrder;
                ProcessingRuleTriggerConditionWS.Cells[rc, 4].Value = ProcessingRuleTriggerConditions[i].CreatedDateTime.ToOADate();
                ProcessingRuleTriggerConditionWS.Cells[rc, 5].Value = ProcessingRuleTriggerConditions[i].CreatedMethod;

                if (users.Any(x => x.ID == ProcessingRuleTriggerConditions[i].CreatedUserID))
                {
                    ProcessingRuleTriggerConditionWS.Cells[rc, 6].Value = users.FirstOrDefault(x => x.ID == ProcessingRuleTriggerConditions[i].CreatedUserID).PublicID;
                    ProcessingRuleTriggerConditionWS.Cells[rc, 7].Value = users.FirstOrDefault(x => x.ID == ProcessingRuleTriggerConditions[i].CreatedUserID).Firstname;
                }
                else
                    ProcessingRuleTriggerConditionWS.Cells[rc, 6].Value = "UserID: " + ProcessingRuleTriggerConditions[i].CreatedUserID.ToString();

                ProcessingRuleTriggerConditionWS.Cells[rc, 8].Value = ProcessingRuleTriggerConditions[i].LastAmendedDateTime.ToOADate();
                ProcessingRuleTriggerConditionWS.Cells[rc, 9].Value = ProcessingRuleTriggerConditions[i].LastAmendedMethod;

                if (users.Any(x => x.ID == ProcessingRuleTriggerConditions[i].LastAmendedUserID))
                {
                    ProcessingRuleTriggerConditionWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == ProcessingRuleTriggerConditions[i].LastAmendedUserID).PublicID;
                    ProcessingRuleTriggerConditionWS.Cells[rc, 11].Value = users.FirstOrDefault(x => x.ID == ProcessingRuleTriggerConditions[i].LastAmendedUserID).Firstname;
                }
                else
                    ProcessingRuleTriggerConditionWS.Cells[rc, 10].Value = "UserID: " + ProcessingRuleTriggerConditions[i].LastAmendedUserID.ToString();

                ProcessingRuleTriggerConditionWS.Cells[rc, 12].Value = ProcessingRuleTriggerConditions[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
