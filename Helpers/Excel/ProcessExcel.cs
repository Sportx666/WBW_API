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
    public class ProcessExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.Process.dbRow> Processs = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.Process.Search ProcessSearchData;
        public ProcessExcel(wbm_common.DataObjects.Process.Search ProcessSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.ProcessSearchData = ProcessSearchData;
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
                        ProcessPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("ProcessExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.Process.dbRow>> rc = await ProcessHelper.ExecuteSearch(ProcessSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            Processs = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = Processs.Select(x => x.CreatedUserID).Union(Processs.Select(x => x.LastAmendedUserID)).Distinct().ToList();

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

        private void ProcessPage(ExcelPackage ep)
        {
            ExcelWorksheet ProcessWS = ep.Workbook.Worksheets.Add("Process");

            #region header
            ProcessWS.View.FreezePanes(2, 1);
            ProcessWS.Cells[1, 1, 1, 12].Style.Font.Bold = true;
            ProcessWS.Cells[1, 1, 1, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            ProcessWS.Column(1).Width = 10;
            ProcessWS.Column(2).Width = 25;
            ProcessWS.Column(3).Width = 10;
            ProcessWS.Column(4).Width = 25;
            ProcessWS.Column(4).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ProcessWS.Column(5).Width = 25;
            ProcessWS.Column(6).Width = 25;
            ProcessWS.Column(7).Width = 25;
            ProcessWS.Column(8).Width = 25;
            ProcessWS.Column(8).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ProcessWS.Column(9).Width = 25;
            ProcessWS.Column(10).Width = 25;
            ProcessWS.Column(11).Width = 25;
            ProcessWS.Column(12).Width = 15;

            ProcessWS.Cells[1, 1].Value = "ID";
            ProcessWS.Cells[1, 2].Value = "Description";
            ProcessWS.Cells[1, 3].Value = "Sort Order";
            ProcessWS.Cells[1, 4].Value = "Created DateTime";
            ProcessWS.Cells[1, 5].Value = "Created Method";
            ProcessWS.Cells[1, 6].Value = "Created User";
            ProcessWS.Cells[1, 7].Value = "Created User Name";
            ProcessWS.Cells[1, 8].Value = "Last Amended DateTime";
            ProcessWS.Cells[1, 9].Value = "Last Amended Method";
            ProcessWS.Cells[1, 10].Value = "Last Amended User";
            ProcessWS.Cells[1, 11].Value = "Last Amended Name";
            ProcessWS.Cells[1, 12].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            Processs = Processs.OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < Processs.Count(); i++)
            {

                ProcessWS.Cells[rc, 1].Value = Processs[i].PublicID;
                ProcessWS.Cells[rc, 2].Value = Processs[i].Description;
                ProcessWS.Cells[rc, 3].Value = Processs[i].SortOrder;

                ProcessWS.Cells[rc, 4].Value = Processs[i].CreatedDateTime.ToOADate();
                ProcessWS.Cells[rc, 5].Value = Processs[i].CreatedMethod;

                if (users.Any(x => x.ID == Processs[i].CreatedUserID))
                {
                    ProcessWS.Cells[rc, 6].Value = users.FirstOrDefault(x => x.ID == Processs[i].CreatedUserID).PublicID;
                    ProcessWS.Cells[rc, 7].Value = users.FirstOrDefault(x => x.ID == Processs[i].CreatedUserID).Firstname;
                }
                else
                    ProcessWS.Cells[rc, 6].Value = "UserID: " + Processs[i].CreatedUserID.ToString();

                ProcessWS.Cells[rc, 8].Value = Processs[i].LastAmendedDateTime.ToOADate();
                ProcessWS.Cells[rc, 9].Value = Processs[i].LastAmendedMethod;

                if (users.Any(x => x.ID == Processs[i].LastAmendedUserID))
                {
                    ProcessWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == Processs[i].LastAmendedUserID).PublicID;
                    ProcessWS.Cells[rc, 11].Value = users.FirstOrDefault(x => x.ID == Processs[i].LastAmendedUserID).Firstname;
                }
                else
                    ProcessWS.Cells[rc, 10].Value = "UserID: " + Processs[i].LastAmendedUserID.ToString();

                ProcessWS.Cells[rc, 12].Value = Processs[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
