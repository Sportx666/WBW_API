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
    public class ContainerUnLoadTypeExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.ContainerUnLoadType.dbRow> ContainerUnLoadTypes = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.ContainerUnLoadType.Search ContainerUnLoadTypeSearchData;
        public ContainerUnLoadTypeExcel(wbm_common.DataObjects.ContainerUnLoadType.Search ContainerUnLoadTypeSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.ContainerUnLoadTypeSearchData = ContainerUnLoadTypeSearchData;
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
                        ContainerUnLoadTypePage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("ContainerUnLoadTypeExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.ContainerUnLoadType.dbRow>> rc = await ContainerUnLoadTypeHelper.ExecuteSearch(ContainerUnLoadTypeSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            ContainerUnLoadTypes = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = ContainerUnLoadTypes.Select(x => x.CreatedUserID).Union(ContainerUnLoadTypes.Select(x => x.LastAmendedUserID)).Distinct().ToList();


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

        private void ContainerUnLoadTypePage(ExcelPackage ep)
        {
            ExcelWorksheet ContainerUnLoadTypeWS = ep.Workbook.Worksheets.Add("ContainerUnLoadType");

            #region header
            ContainerUnLoadTypeWS.View.FreezePanes(2, 1);
            ContainerUnLoadTypeWS.Cells[1, 1, 1, 12].Style.Font.Bold = true;
            ContainerUnLoadTypeWS.Cells[1, 1, 1, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            ContainerUnLoadTypeWS.Column(1).Width = 15;
            ContainerUnLoadTypeWS.Column(2).Width = 15;
            ContainerUnLoadTypeWS.Column(3).Width = 10;
            ContainerUnLoadTypeWS.Column(4).Width = 25;
            ContainerUnLoadTypeWS.Column(4).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ContainerUnLoadTypeWS.Column(5).Width = 25;
            ContainerUnLoadTypeWS.Column(6).Width = 25;
            ContainerUnLoadTypeWS.Column(7).Width = 25;
            ContainerUnLoadTypeWS.Column(8).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ContainerUnLoadTypeWS.Column(8).Width = 25;
            ContainerUnLoadTypeWS.Column(9).Width = 25;
            ContainerUnLoadTypeWS.Column(10).Width = 25;
            ContainerUnLoadTypeWS.Column(11).Width = 25;
            ContainerUnLoadTypeWS.Column(12).Width = 25;

            ContainerUnLoadTypeWS.Cells[1, 1].Value = "ID";
            ContainerUnLoadTypeWS.Cells[1, 2].Value = "Description";
            ContainerUnLoadTypeWS.Cells[1, 3].Value = "Sort Order";
            ContainerUnLoadTypeWS.Cells[1, 4].Value = "Created DateTime";
            ContainerUnLoadTypeWS.Cells[1, 5].Value = "Created Method";
            ContainerUnLoadTypeWS.Cells[1, 6].Value = "Created User";
            ContainerUnLoadTypeWS.Cells[1, 7].Value = "Created User Name";
            ContainerUnLoadTypeWS.Cells[1, 8].Value = "Last Amended DateTime";
            ContainerUnLoadTypeWS.Cells[1, 9].Value = "Last Amended Method";
            ContainerUnLoadTypeWS.Cells[1, 10].Value = "Last Amended User";
            ContainerUnLoadTypeWS.Cells[1, 11].Value = "Last Amended Name";
            ContainerUnLoadTypeWS.Cells[1, 12].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            ContainerUnLoadTypes = ContainerUnLoadTypes.OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < ContainerUnLoadTypes.Count(); i++)
            {
                ContainerUnLoadTypeWS.Cells[rc, 1].Value = ContainerUnLoadTypes[i].PublicID;
                ContainerUnLoadTypeWS.Cells[rc, 2].Value = ContainerUnLoadTypes[i].Description;
                ContainerUnLoadTypeWS.Cells[rc, 3].Value = ContainerUnLoadTypes[i].SortOrder;
                ContainerUnLoadTypeWS.Cells[rc, 4].Value = ContainerUnLoadTypes[i].CreatedDateTime.ToOADate();
                ContainerUnLoadTypeWS.Cells[rc, 5].Value = ContainerUnLoadTypes[i].CreatedMethod;

                if (users.Any(x => x.ID == ContainerUnLoadTypes[i].CreatedUserID))
                {
                    ContainerUnLoadTypeWS.Cells[rc, 6].Value = users.FirstOrDefault(x => x.ID == ContainerUnLoadTypes[i].CreatedUserID).PublicID;
                    ContainerUnLoadTypeWS.Cells[rc, 7].Value = users.FirstOrDefault(x => x.ID == ContainerUnLoadTypes[i].CreatedUserID).Firstname;
                }
                else
                    ContainerUnLoadTypeWS.Cells[rc, 6].Value = "UserID: " + ContainerUnLoadTypes[i].CreatedUserID.ToString();

                ContainerUnLoadTypeWS.Cells[rc, 8].Value = ContainerUnLoadTypes[i].LastAmendedDateTime.ToOADate();
                ContainerUnLoadTypeWS.Cells[rc, 9].Value = ContainerUnLoadTypes[i].LastAmendedMethod;

                if (users.Any(x => x.ID == ContainerUnLoadTypes[i].LastAmendedUserID))
                {
                    ContainerUnLoadTypeWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == ContainerUnLoadTypes[i].LastAmendedUserID).PublicID;
                    ContainerUnLoadTypeWS.Cells[rc, 11].Value = users.FirstOrDefault(x => x.ID == ContainerUnLoadTypes[i].LastAmendedUserID).Firstname;
                }
                else
                    ContainerUnLoadTypeWS.Cells[rc, 10].Value = "UserID: " + ContainerUnLoadTypes[i].LastAmendedUserID.ToString();

                ContainerUnLoadTypeWS.Cells[rc, 12].Value = ContainerUnLoadTypes[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
