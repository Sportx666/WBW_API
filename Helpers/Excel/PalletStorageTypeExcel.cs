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
    public class PalletStorageTypeExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.PalletStorageType.dbRow> PalletStorageTypes = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.PalletStorageType.Search PalletStorageTypeSearchData;
        public PalletStorageTypeExcel(wbm_common.DataObjects.PalletStorageType.Search PalletStorageTypeSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.PalletStorageTypeSearchData = PalletStorageTypeSearchData;
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
                        PalletStorageTypePage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("PalletStorageTypeExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.PalletStorageType.dbRow>> rc = await PalletStorageTypeHelper.ExecuteSearch(PalletStorageTypeSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            PalletStorageTypes = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = PalletStorageTypes.Select(x => x.CreatedUserID).Union(PalletStorageTypes.Select(x => x.LastAmendedUserID)).Distinct().ToList();


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

        private void PalletStorageTypePage(ExcelPackage ep)
        {
            ExcelWorksheet PalletStorageTypeWS = ep.Workbook.Worksheets.Add("PalletStorageType");

            #region header
            PalletStorageTypeWS.View.FreezePanes(2, 1);
            PalletStorageTypeWS.Cells[1, 1, 1, 12].Style.Font.Bold = true;
            PalletStorageTypeWS.Cells[1, 1, 1, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            PalletStorageTypeWS.Column(1).Width = 15;
            PalletStorageTypeWS.Column(2).Width = 15;
            PalletStorageTypeWS.Column(3).Width = 10;
            PalletStorageTypeWS.Column(4).Width = 25;
            PalletStorageTypeWS.Column(4).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            PalletStorageTypeWS.Column(5).Width = 25;
            PalletStorageTypeWS.Column(6).Width = 25;
            PalletStorageTypeWS.Column(7).Width = 25;
            PalletStorageTypeWS.Column(8).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            PalletStorageTypeWS.Column(8).Width = 25;
            PalletStorageTypeWS.Column(9).Width = 25;
            PalletStorageTypeWS.Column(10).Width = 25;
            PalletStorageTypeWS.Column(11).Width = 25;
            PalletStorageTypeWS.Column(12).Width = 25;

            PalletStorageTypeWS.Cells[1, 1].Value = "ID";
            PalletStorageTypeWS.Cells[1, 2].Value = "Description";
            PalletStorageTypeWS.Cells[1, 3].Value = "Sort Order";
            PalletStorageTypeWS.Cells[1, 4].Value = "Created DateTime";
            PalletStorageTypeWS.Cells[1, 5].Value = "Created Method";
            PalletStorageTypeWS.Cells[1, 6].Value = "Created User";
            PalletStorageTypeWS.Cells[1, 7].Value = "Created User Name";
            PalletStorageTypeWS.Cells[1, 8].Value = "Last Amended DateTime";
            PalletStorageTypeWS.Cells[1, 9].Value = "Last Amended Method";
            PalletStorageTypeWS.Cells[1, 10].Value = "Last Amended User";
            PalletStorageTypeWS.Cells[1, 11].Value = "Last Amended Name";
            PalletStorageTypeWS.Cells[1, 12].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            PalletStorageTypes = PalletStorageTypes.OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < PalletStorageTypes.Count(); i++)
            {
                PalletStorageTypeWS.Cells[rc, 1].Value = PalletStorageTypes[i].PublicID;
                PalletStorageTypeWS.Cells[rc, 2].Value = PalletStorageTypes[i].Description;
                PalletStorageTypeWS.Cells[rc, 3].Value = PalletStorageTypes[i].SortOrder;
                PalletStorageTypeWS.Cells[rc, 4].Value = PalletStorageTypes[i].CreatedDateTime.ToOADate();
                PalletStorageTypeWS.Cells[rc, 5].Value = PalletStorageTypes[i].CreatedMethod;

                if (users.Any(x => x.ID == PalletStorageTypes[i].CreatedUserID))
                {
                    PalletStorageTypeWS.Cells[rc, 6].Value = users.FirstOrDefault(x => x.ID == PalletStorageTypes[i].CreatedUserID).PublicID;
                    PalletStorageTypeWS.Cells[rc, 7].Value = users.FirstOrDefault(x => x.ID == PalletStorageTypes[i].CreatedUserID).Firstname;
                }
                else
                    PalletStorageTypeWS.Cells[rc, 6].Value = "UserID: " + PalletStorageTypes[i].CreatedUserID.ToString();

                PalletStorageTypeWS.Cells[rc, 8].Value = PalletStorageTypes[i].LastAmendedDateTime.ToOADate();
                PalletStorageTypeWS.Cells[rc, 9].Value = PalletStorageTypes[i].LastAmendedMethod;

                if (users.Any(x => x.ID == PalletStorageTypes[i].LastAmendedUserID))
                {
                    PalletStorageTypeWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == PalletStorageTypes[i].LastAmendedUserID).PublicID;
                    PalletStorageTypeWS.Cells[rc, 11].Value = users.FirstOrDefault(x => x.ID == PalletStorageTypes[i].LastAmendedUserID).Firstname;
                }
                else
                    PalletStorageTypeWS.Cells[rc, 10].Value = "UserID: " + PalletStorageTypes[i].LastAmendedUserID.ToString();

                PalletStorageTypeWS.Cells[rc, 12].Value = PalletStorageTypes[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
