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
    public class PaperlessDataTypeExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.PaperlessDataType.dbRow> PaperlessDataTypes = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.PaperlessDataType.Search PaperlessDataTypeSearchData;
        public PaperlessDataTypeExcel(wbm_common.DataObjects.PaperlessDataType.Search PaperlessDataTypeSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.PaperlessDataTypeSearchData = PaperlessDataTypeSearchData;
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
                        PaperlessDataTypePage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("PaperlessDataTypeExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.PaperlessDataType.dbRow>> rc = await PaperlessDataTypeHelper.ExecuteSearch(PaperlessDataTypeSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            PaperlessDataTypes = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = PaperlessDataTypes.Select(x => x.CreatedUserID).Union(PaperlessDataTypes.Select(x => x.LastAmendedUserID)).Distinct().ToList();


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

        private void PaperlessDataTypePage(ExcelPackage ep)
        {
            ExcelWorksheet PaperlessDataTypeWS = ep.Workbook.Worksheets.Add("PaperlessDataType");

            #region header
            PaperlessDataTypeWS.View.FreezePanes(2, 1);
            PaperlessDataTypeWS.Cells[1, 1, 1, 12].Style.Font.Bold = true;
            PaperlessDataTypeWS.Cells[1, 1, 1, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            PaperlessDataTypeWS.Column(1).Width = 15;
            PaperlessDataTypeWS.Column(2).Width = 15;
            PaperlessDataTypeWS.Column(3).Width = 10;
            PaperlessDataTypeWS.Column(4).Width = 25;
            PaperlessDataTypeWS.Column(4).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            PaperlessDataTypeWS.Column(5).Width = 25;
            PaperlessDataTypeWS.Column(6).Width = 25;
            PaperlessDataTypeWS.Column(7).Width = 25;
            PaperlessDataTypeWS.Column(8).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            PaperlessDataTypeWS.Column(8).Width = 25;
            PaperlessDataTypeWS.Column(9).Width = 25;
            PaperlessDataTypeWS.Column(10).Width = 25;
            PaperlessDataTypeWS.Column(11).Width = 25;
            PaperlessDataTypeWS.Column(12).Width = 25;

            PaperlessDataTypeWS.Cells[1, 1].Value = "ID";
            PaperlessDataTypeWS.Cells[1, 2].Value = "Description";
            PaperlessDataTypeWS.Cells[1, 3].Value = "Sort Order";
            PaperlessDataTypeWS.Cells[1, 4].Value = "Created DateTime";
            PaperlessDataTypeWS.Cells[1, 5].Value = "Created Method";
            PaperlessDataTypeWS.Cells[1, 6].Value = "Created User";
            PaperlessDataTypeWS.Cells[1, 7].Value = "Created User Name";
            PaperlessDataTypeWS.Cells[1, 8].Value = "Last Amended DateTime";
            PaperlessDataTypeWS.Cells[1, 9].Value = "Last Amended Method";
            PaperlessDataTypeWS.Cells[1, 10].Value = "Last Amended User";
            PaperlessDataTypeWS.Cells[1, 11].Value = "Last Amended Name";
            PaperlessDataTypeWS.Cells[1, 12].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            PaperlessDataTypes = PaperlessDataTypes.OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < PaperlessDataTypes.Count(); i++)
            {
                PaperlessDataTypeWS.Cells[rc, 1].Value = PaperlessDataTypes[i].PublicID;
                PaperlessDataTypeWS.Cells[rc, 2].Value = PaperlessDataTypes[i].Description;
                PaperlessDataTypeWS.Cells[rc, 3].Value = PaperlessDataTypes[i].SortOrder;
                PaperlessDataTypeWS.Cells[rc, 4].Value = PaperlessDataTypes[i].CreatedDateTime.ToOADate();
                PaperlessDataTypeWS.Cells[rc, 5].Value = PaperlessDataTypes[i].CreatedMethod;

                if (users.Any(x => x.ID == PaperlessDataTypes[i].CreatedUserID))
                {
                    PaperlessDataTypeWS.Cells[rc, 6].Value = users.FirstOrDefault(x => x.ID == PaperlessDataTypes[i].CreatedUserID).PublicID;
                    PaperlessDataTypeWS.Cells[rc, 7].Value = users.FirstOrDefault(x => x.ID == PaperlessDataTypes[i].CreatedUserID).Firstname;
                }
                else
                    PaperlessDataTypeWS.Cells[rc, 6].Value = "UserID: " + PaperlessDataTypes[i].CreatedUserID.ToString();

                PaperlessDataTypeWS.Cells[rc, 8].Value = PaperlessDataTypes[i].LastAmendedDateTime.ToOADate();
                PaperlessDataTypeWS.Cells[rc, 9].Value = PaperlessDataTypes[i].LastAmendedMethod;

                if (users.Any(x => x.ID == PaperlessDataTypes[i].LastAmendedUserID))
                {
                    PaperlessDataTypeWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == PaperlessDataTypes[i].LastAmendedUserID).PublicID;
                    PaperlessDataTypeWS.Cells[rc, 11].Value = users.FirstOrDefault(x => x.ID == PaperlessDataTypes[i].LastAmendedUserID).Firstname;
                }
                else
                    PaperlessDataTypeWS.Cells[rc, 10].Value = "UserID: " + PaperlessDataTypes[i].LastAmendedUserID.ToString();

                PaperlessDataTypeWS.Cells[rc, 12].Value = PaperlessDataTypes[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
