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
    public class ProcessingRuleProcessExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.ProcessingRuleProcess.dbRow> ProcessingRuleProcesss = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.ProcessingRuleProcess.Search ProcessingRuleProcessSearchData;
        public ProcessingRuleProcessExcel(wbm_common.DataObjects.ProcessingRuleProcess.Search ProcessingRuleProcessSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.ProcessingRuleProcessSearchData = ProcessingRuleProcessSearchData;
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
                        ProcessingRuleProcessPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("ProcessingRuleProcessExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>> rc = await ProcessingRuleProcessHelper.ExecuteSearch(ProcessingRuleProcessSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            ProcessingRuleProcesss = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = ProcessingRuleProcesss.Select(x => x.CreatedUserID).Union(ProcessingRuleProcesss.Select(x => x.LastAmendedUserID)).Distinct().ToList();


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

        private void ProcessingRuleProcessPage(ExcelPackage ep)
        {
            ExcelWorksheet ProcessingRuleProcessWS = ep.Workbook.Worksheets.Add("ProcessingRuleProcess");

            #region header
            ProcessingRuleProcessWS.View.FreezePanes(2, 1);
            ProcessingRuleProcessWS.Cells[1, 1, 1, 12].Style.Font.Bold = true;
            ProcessingRuleProcessWS.Cells[1, 1, 1, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            ProcessingRuleProcessWS.Column(1).Width = 15;
            ProcessingRuleProcessWS.Column(2).Width = 15;
            ProcessingRuleProcessWS.Column(3).Width = 10;
            ProcessingRuleProcessWS.Column(4).Width = 25;
            ProcessingRuleProcessWS.Column(4).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ProcessingRuleProcessWS.Column(5).Width = 25;
            ProcessingRuleProcessWS.Column(6).Width = 25;
            ProcessingRuleProcessWS.Column(7).Width = 25;
            ProcessingRuleProcessWS.Column(8).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ProcessingRuleProcessWS.Column(8).Width = 25;
            ProcessingRuleProcessWS.Column(9).Width = 25;
            ProcessingRuleProcessWS.Column(10).Width = 25;
            ProcessingRuleProcessWS.Column(11).Width = 25;
            ProcessingRuleProcessWS.Column(12).Width = 15;

            ProcessingRuleProcessWS.Cells[1, 1].Value = "ID";
            ProcessingRuleProcessWS.Cells[1, 2].Value = "Description";
            ProcessingRuleProcessWS.Cells[1, 3].Value = "Sort Order";
            ProcessingRuleProcessWS.Cells[1, 4].Value = "Created DateTime";
            ProcessingRuleProcessWS.Cells[1, 5].Value = "Created Method";
            ProcessingRuleProcessWS.Cells[1, 6].Value = "Created User";
            ProcessingRuleProcessWS.Cells[1, 7].Value = "Created User Name";
            ProcessingRuleProcessWS.Cells[1, 8].Value = "Last Amended DateTime";
            ProcessingRuleProcessWS.Cells[1, 9].Value = "Last Amended Method";
            ProcessingRuleProcessWS.Cells[1, 10].Value = "Last Amended User";
            ProcessingRuleProcessWS.Cells[1, 11].Value = "Last Amended Name";
            ProcessingRuleProcessWS.Cells[1, 12].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            ProcessingRuleProcesss = ProcessingRuleProcesss.OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < ProcessingRuleProcesss.Count(); i++)
            {
                ProcessingRuleProcessWS.Cells[rc, 1].Value = ProcessingRuleProcesss[i].PublicID;
                ProcessingRuleProcessWS.Cells[rc, 2].Value = ProcessingRuleProcesss[i].Description;
                ProcessingRuleProcessWS.Cells[rc, 3].Value = ProcessingRuleProcesss[i].SortOrder;
                ProcessingRuleProcessWS.Cells[rc, 4].Value = ProcessingRuleProcesss[i].CreatedDateTime.ToOADate();
                ProcessingRuleProcessWS.Cells[rc, 5].Value = ProcessingRuleProcesss[i].CreatedMethod;

                if (users.Any(x => x.ID == ProcessingRuleProcesss[i].CreatedUserID))
                {
                    ProcessingRuleProcessWS.Cells[rc, 6].Value = users.FirstOrDefault(x => x.ID == ProcessingRuleProcesss[i].CreatedUserID).PublicID;
                    ProcessingRuleProcessWS.Cells[rc, 7].Value = users.FirstOrDefault(x => x.ID == ProcessingRuleProcesss[i].CreatedUserID).Firstname;
                }
                else
                    ProcessingRuleProcessWS.Cells[rc, 6].Value = "UserID: " + ProcessingRuleProcesss[i].CreatedUserID.ToString();

                ProcessingRuleProcessWS.Cells[rc, 8].Value = ProcessingRuleProcesss[i].LastAmendedDateTime.ToOADate();
                ProcessingRuleProcessWS.Cells[rc, 9].Value = ProcessingRuleProcesss[i].LastAmendedMethod;

                if (users.Any(x => x.ID == ProcessingRuleProcesss[i].LastAmendedUserID))
                {
                    ProcessingRuleProcessWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == ProcessingRuleProcesss[i].LastAmendedUserID).PublicID;
                    ProcessingRuleProcessWS.Cells[rc, 11].Value = users.FirstOrDefault(x => x.ID == ProcessingRuleProcesss[i].LastAmendedUserID).Firstname;
                }
                else
                    ProcessingRuleProcessWS.Cells[rc, 10].Value = "UserID: " + ProcessingRuleProcesss[i].LastAmendedUserID.ToString();

                ProcessingRuleProcessWS.Cells[rc, 12].Value = ProcessingRuleProcesss[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
