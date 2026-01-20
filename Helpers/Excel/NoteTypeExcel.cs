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
    public class NoteTypeExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.NoteType.dbRow> NoteTypes = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.NoteType.Search NoteTypeSearchData;
        public NoteTypeExcel(wbm_common.DataObjects.NoteType.Search NoteTypeSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.NoteTypeSearchData = NoteTypeSearchData;
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
                        NoteTypePage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("NoteTypeExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.NoteType.dbRow>> rc = await NoteTypeHelper.ExecuteSearch(NoteTypeSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            NoteTypes = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = NoteTypes.Select(x => x.CreatedUserID).Union(NoteTypes.Select(x => x.LastAmendedUserID)).Distinct().ToList();


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

        private void NoteTypePage(ExcelPackage ep)
        {
            ExcelWorksheet NoteTypeWS = ep.Workbook.Worksheets.Add("NoteType");

            #region header
            NoteTypeWS.View.FreezePanes(2, 1);
            NoteTypeWS.Cells[1, 1, 1, 12].Style.Font.Bold = true;
            NoteTypeWS.Cells[1, 1, 1, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            NoteTypeWS.Column(1).Width = 15;
            NoteTypeWS.Column(2).Width = 15;
            NoteTypeWS.Column(3).Width = 10;
            NoteTypeWS.Column(4).Width = 25;
            NoteTypeWS.Column(4).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            NoteTypeWS.Column(5).Width = 25;
            NoteTypeWS.Column(6).Width = 25;
            NoteTypeWS.Column(7).Width = 25;
            NoteTypeWS.Column(8).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            NoteTypeWS.Column(8).Width = 25;
            NoteTypeWS.Column(9).Width = 25;
            NoteTypeWS.Column(10).Width = 25;
            NoteTypeWS.Column(11).Width = 25;
            NoteTypeWS.Column(12).Width = 15;

            NoteTypeWS.Cells[1, 1].Value = "ID";
            NoteTypeWS.Cells[1, 2].Value = "Description";
            NoteTypeWS.Cells[1, 3].Value = "Sort Order";
            NoteTypeWS.Cells[1, 4].Value = "Created DateTime";
            NoteTypeWS.Cells[1, 5].Value = "Created Method";
            NoteTypeWS.Cells[1, 6].Value = "Created User";
            NoteTypeWS.Cells[1, 7].Value = "Created User Name";
            NoteTypeWS.Cells[1, 8].Value = "Last Amended DateTime";
            NoteTypeWS.Cells[1, 9].Value = "Last Amended Method";
            NoteTypeWS.Cells[1, 10].Value = "Last Amended User";
            NoteTypeWS.Cells[1, 11].Value = "Last Amended Name";
            NoteTypeWS.Cells[1, 12].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            NoteTypes = NoteTypes.OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < NoteTypes.Count(); i++)
            {
                NoteTypeWS.Cells[rc, 1].Value = NoteTypes[i].PublicID;
                NoteTypeWS.Cells[rc, 2].Value = NoteTypes[i].Description;
                NoteTypeWS.Cells[rc, 3].Value = NoteTypes[i].SortOrder;
                NoteTypeWS.Cells[rc, 4].Value = NoteTypes[i].CreatedDateTime.ToOADate();
                NoteTypeWS.Cells[rc, 5].Value = NoteTypes[i].CreatedMethod;

                if (users.Any(x => x.ID == NoteTypes[i].CreatedUserID))
                {
                    NoteTypeWS.Cells[rc, 6].Value = users.FirstOrDefault(x => x.ID == NoteTypes[i].CreatedUserID).PublicID;
                    NoteTypeWS.Cells[rc, 7].Value = users.FirstOrDefault(x => x.ID == NoteTypes[i].CreatedUserID).Firstname;
                }
                else
                    NoteTypeWS.Cells[rc, 6].Value = "UserID: " + NoteTypes[i].CreatedUserID.ToString();

                NoteTypeWS.Cells[rc, 8].Value = NoteTypes[i].LastAmendedDateTime.ToOADate();
                NoteTypeWS.Cells[rc, 9].Value = NoteTypes[i].LastAmendedMethod;

                if (users.Any(x => x.ID == NoteTypes[i].LastAmendedUserID))
                {
                    NoteTypeWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == NoteTypes[i].LastAmendedUserID).PublicID;
                    NoteTypeWS.Cells[rc, 11].Value = users.FirstOrDefault(x => x.ID == NoteTypes[i].LastAmendedUserID).Firstname;
                }
                else
                    NoteTypeWS.Cells[rc, 10].Value = "UserID: " + NoteTypes[i].LastAmendedUserID.ToString();

                NoteTypeWS.Cells[rc, 12].Value = NoteTypes[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
