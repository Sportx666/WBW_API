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
    public class TransactionExceptionTypeExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.TransactionExceptionType.dbRow> TransactionExceptionTypes = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.TransactionExceptionType.Search TransactionExceptionTypeSearchData;
        public TransactionExceptionTypeExcel(wbm_common.DataObjects.TransactionExceptionType.Search TransactionExceptionTypeSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.TransactionExceptionTypeSearchData = TransactionExceptionTypeSearchData;
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
                        TransactionExceptionTypePage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("TransactionExceptionTypeExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.TransactionExceptionType.dbRow>> rc = await TransactionExceptionTypeHelper.ExecuteSearch(TransactionExceptionTypeSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            TransactionExceptionTypes = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = TransactionExceptionTypes.Select(x => x.CreatedUserID).Union(TransactionExceptionTypes.Select(x => x.LastAmendedUserID)).Distinct().ToList();


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

        private void TransactionExceptionTypePage(ExcelPackage ep)
        {
            ExcelWorksheet TransactionExceptionTypeWS = ep.Workbook.Worksheets.Add("TransactionExceptionType");

            #region header
            TransactionExceptionTypeWS.View.FreezePanes(2, 1);
            TransactionExceptionTypeWS.Cells[1, 1, 1, 12].Style.Font.Bold = true;
            TransactionExceptionTypeWS.Cells[1, 1, 1, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            TransactionExceptionTypeWS.Column(1).Width = 15;
            TransactionExceptionTypeWS.Column(2).Width = 15;
            TransactionExceptionTypeWS.Column(3).Width = 10;
            TransactionExceptionTypeWS.Column(4).Width = 25;
            TransactionExceptionTypeWS.Column(4).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            TransactionExceptionTypeWS.Column(5).Width = 25;
            TransactionExceptionTypeWS.Column(6).Width = 25;
            TransactionExceptionTypeWS.Column(7).Width = 25;
            TransactionExceptionTypeWS.Column(8).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            TransactionExceptionTypeWS.Column(8).Width = 25;
            TransactionExceptionTypeWS.Column(9).Width = 25;
            TransactionExceptionTypeWS.Column(10).Width = 25;
            TransactionExceptionTypeWS.Column(11).Width = 25;
            TransactionExceptionTypeWS.Column(12).Width = 15;

            TransactionExceptionTypeWS.Cells[1, 1].Value = "ID";
            TransactionExceptionTypeWS.Cells[1, 2].Value = "Description";
            TransactionExceptionTypeWS.Cells[1, 3].Value = "Sort Order";
            TransactionExceptionTypeWS.Cells[1, 4].Value = "Created DateTime";
            TransactionExceptionTypeWS.Cells[1, 5].Value = "Created Method";
            TransactionExceptionTypeWS.Cells[1, 6].Value = "Created User";
            TransactionExceptionTypeWS.Cells[1, 7].Value = "Created User Name";
            TransactionExceptionTypeWS.Cells[1, 8].Value = "Last Amended DateTime";
            TransactionExceptionTypeWS.Cells[1, 9].Value = "Last Amended Method";
            TransactionExceptionTypeWS.Cells[1, 10].Value = "Last Amended User";
            TransactionExceptionTypeWS.Cells[1, 11].Value = "Last Amended Name";
            TransactionExceptionTypeWS.Cells[1, 12].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            TransactionExceptionTypes = TransactionExceptionTypes.OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < TransactionExceptionTypes.Count(); i++)
            {
                TransactionExceptionTypeWS.Cells[rc, 1].Value = TransactionExceptionTypes[i].PublicID;
                TransactionExceptionTypeWS.Cells[rc, 2].Value = TransactionExceptionTypes[i].Description;
                TransactionExceptionTypeWS.Cells[rc, 3].Value = TransactionExceptionTypes[i].SortOrder;
                TransactionExceptionTypeWS.Cells[rc, 4].Value = TransactionExceptionTypes[i].CreatedDateTime.ToOADate();
                TransactionExceptionTypeWS.Cells[rc, 5].Value = TransactionExceptionTypes[i].CreatedMethod;

                if (users.Any(x => x.ID == TransactionExceptionTypes[i].CreatedUserID))
                {
                    TransactionExceptionTypeWS.Cells[rc, 6].Value = users.FirstOrDefault(x => x.ID == TransactionExceptionTypes[i].CreatedUserID).PublicID;
                    TransactionExceptionTypeWS.Cells[rc, 7].Value = users.FirstOrDefault(x => x.ID == TransactionExceptionTypes[i].CreatedUserID).Firstname;
                }
                else
                    TransactionExceptionTypeWS.Cells[rc, 6].Value = "UserID: " + TransactionExceptionTypes[i].CreatedUserID.ToString();

                TransactionExceptionTypeWS.Cells[rc, 8].Value = TransactionExceptionTypes[i].LastAmendedDateTime.ToOADate();
                TransactionExceptionTypeWS.Cells[rc, 9].Value = TransactionExceptionTypes[i].LastAmendedMethod;

                if (users.Any(x => x.ID == TransactionExceptionTypes[i].LastAmendedUserID))
                {
                    TransactionExceptionTypeWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == TransactionExceptionTypes[i].LastAmendedUserID).PublicID;
                    TransactionExceptionTypeWS.Cells[rc, 11].Value = users.FirstOrDefault(x => x.ID == TransactionExceptionTypes[i].LastAmendedUserID).Firstname;
                }
                else
                    TransactionExceptionTypeWS.Cells[rc, 10].Value = "UserID: " + TransactionExceptionTypes[i].LastAmendedUserID.ToString();

                TransactionExceptionTypeWS.Cells[rc, 12].Value = TransactionExceptionTypes[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
