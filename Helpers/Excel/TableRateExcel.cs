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
    public class TableRateExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.TableRate.dbRow> TableRates = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.TableRate.Search TableRateSearchData;
        public TableRateExcel(wbm_common.DataObjects.TableRate.Search TableRateSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.TableRateSearchData = TableRateSearchData;
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
                        TableRatePage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("TableRateExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.TableRate.dbRow>> rc = await TableRateHelper.ExecuteSearch(TableRateSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            TableRates = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = TableRates.Select(x => x.CreatedUserID).Union(TableRates.Select(x => x.LastAmendedUserID)).Distinct().ToList();


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

        private void TableRatePage(ExcelPackage ep)
        {
            ExcelWorksheet TableRateWS = ep.Workbook.Worksheets.Add("TableRate");

            #region header
            TableRateWS.View.FreezePanes(2, 1);
            TableRateWS.Cells[1, 1, 1, 15].Style.Font.Bold = true;
            TableRateWS.Cells[1, 1, 1, 15].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            TableRateWS.Column(1).Width = 15;
            TableRateWS.Column(2).Width = 15;
            TableRateWS.Column(3).Width = 10;
            TableRateWS.Column(4).Width = 25;
            TableRateWS.Column(5).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            TableRateWS.Column(5).Width = 25;
            TableRateWS.Column(6).Width = 25;
            TableRateWS.Column(7).Width = 25;
            TableRateWS.Column(7).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            TableRateWS.Column(8).Width = 25;
            TableRateWS.Column(9).Width = 25;
            TableRateWS.Column(10).Width = 25;
            TableRateWS.Column(11).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            TableRateWS.Column(11).Width = 25;
            TableRateWS.Column(12).Width = 25;
            TableRateWS.Column(13).Width = 25;
            TableRateWS.Column(14).Width = 25;
            TableRateWS.Column(15).Width = 15;

            TableRateWS.Cells[1, 1].Value = "Rate ID";
            TableRateWS.Cells[1, 2].Value = "Table Break";
            TableRateWS.Cells[1, 3].Value = "Table Units";
            TableRateWS.Cells[1, 4].Value = "Table Charge";
            TableRateWS.Cells[1, 5].Value = "Date Of Change";
            TableRateWS.Cells[1, 6].Value = "New Table Charge";
            TableRateWS.Cells[1, 7].Value = "Created DateTime";
            TableRateWS.Cells[1, 8].Value = "Created Method";
            TableRateWS.Cells[1, 9].Value = "Created User";
            TableRateWS.Cells[1, 10].Value = "Created User Name";
            TableRateWS.Cells[1, 11].Value = "Last Amended DateTime";
            TableRateWS.Cells[1, 12].Value = "Last Amended Method";
            TableRateWS.Cells[1, 13].Value = "Last Amended User";
            TableRateWS.Cells[1, 14].Value = "Last Amended Name";
            TableRateWS.Cells[1, 15].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            for (int i = 0; i < TableRates.Count(); i++)
            {
                TableRateWS.Cells[rc, 1].Value = TableRates[i].RateID;
                TableRateWS.Cells[rc, 2].Value = TableRates[i].TableBreak;
                TableRateWS.Cells[rc, 3].Value = TableRates[i].TableUnits;
                TableRateWS.Cells[rc, 4].Value = TableRates[i].TableCharge;
                TableRateWS.Cells[rc, 5].Value = TableRates[i].DateOfChange.ToOADate();
                TableRateWS.Cells[rc, 6].Value = TableRates[i].NewTableCharge;

                TableRateWS.Cells[rc, 7].Value = TableRates[i].CreatedDateTime.ToOADate();
                TableRateWS.Cells[rc, 8].Value = TableRates[i].CreatedMethod;

                if (users.Any(x => x.ID == TableRates[i].CreatedUserID))
                {
                    TableRateWS.Cells[rc, 9].Value = users.FirstOrDefault(x => x.ID == TableRates[i].CreatedUserID).PublicID;
                    TableRateWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == TableRates[i].CreatedUserID).Firstname;
                }
                else
                    TableRateWS.Cells[rc, 9].Value = "UserID: " + TableRates[i].CreatedUserID.ToString();

                TableRateWS.Cells[rc, 11].Value = TableRates[i].LastAmendedDateTime.ToOADate();
                TableRateWS.Cells[rc, 12].Value = TableRates[i].LastAmendedMethod;

                if (users.Any(x => x.ID == TableRates[i].LastAmendedUserID))
                {
                    TableRateWS.Cells[rc, 13].Value = users.FirstOrDefault(x => x.ID == TableRates[i].LastAmendedUserID).PublicID;
                    TableRateWS.Cells[rc, 14].Value = users.FirstOrDefault(x => x.ID == TableRates[i].LastAmendedUserID).Firstname;
                }
                else
                    TableRateWS.Cells[rc, 13].Value = "UserID: " + TableRates[i].LastAmendedUserID.ToString();

                TableRateWS.Cells[rc, 15].Value = TableRates[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
