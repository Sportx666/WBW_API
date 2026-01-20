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
    public class ContainerSizeExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;

        public List<wbm_common.DataObjects.Security_User.dbRow> users = null;
        public List<wbm_common.DataObjects.ContainerSize.dbRow> ContainerSizes = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.ContainerSize.Search ContainerSizeSearchData;
        public ContainerSizeExcel(wbm_common.DataObjects.ContainerSize.Search ContainerSizeSearchData, WBMDatabase wBMDB)
        {
            ms = new MemoryStream();

            this.ContainerSizeSearchData = ContainerSizeSearchData;
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
                        ContainerSizePage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("ContainerSizeExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.ContainerSize.dbRow>> rc = await ContainerSizeHelper.ExecuteSearch(ContainerSizeSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            ContainerSizes = rc.Value;

            // Build list of IDs to retrieve related data
            List<int> ListUserID = ContainerSizes.Select(x => x.CreatedUserID).Union(ContainerSizes.Select(x => x.LastAmendedUserID)).Distinct().ToList();


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

        private void ContainerSizePage(ExcelPackage ep)
        {
            ExcelWorksheet ContainerSizeWS = ep.Workbook.Worksheets.Add("ContainerSize");

            #region header
            ContainerSizeWS.View.FreezePanes(2, 1);
            ContainerSizeWS.Cells[1, 1, 1, 12].Style.Font.Bold = true;
            ContainerSizeWS.Cells[1, 1, 1, 12].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            ContainerSizeWS.Column(1).Width = 15;
            ContainerSizeWS.Column(2).Width = 15;
            ContainerSizeWS.Column(3).Width = 10;
            ContainerSizeWS.Column(4).Width = 25;
            ContainerSizeWS.Column(4).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ContainerSizeWS.Column(5).Width = 25;
            ContainerSizeWS.Column(6).Width = 25;
            ContainerSizeWS.Column(7).Width = 25;
            ContainerSizeWS.Column(8).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            ContainerSizeWS.Column(8).Width = 25;
            ContainerSizeWS.Column(9).Width = 25;
            ContainerSizeWS.Column(10).Width = 25;
            ContainerSizeWS.Column(11).Width = 25;
            ContainerSizeWS.Column(12).Width = 25;

            ContainerSizeWS.Cells[1, 1].Value = "ID";
            ContainerSizeWS.Cells[1, 2].Value = "Description";
            ContainerSizeWS.Cells[1, 3].Value = "Sort Order";
            ContainerSizeWS.Cells[1, 4].Value = "Created DateTime";
            ContainerSizeWS.Cells[1, 5].Value = "Created Method";
            ContainerSizeWS.Cells[1, 6].Value = "Created User";
            ContainerSizeWS.Cells[1, 7].Value = "Created User Name";
            ContainerSizeWS.Cells[1, 8].Value = "Last Amended DateTime";
            ContainerSizeWS.Cells[1, 9].Value = "Last Amended Method";
            ContainerSizeWS.Cells[1, 10].Value = "Last Amended User";
            ContainerSizeWS.Cells[1, 11].Value = "Last Amended Name";
            ContainerSizeWS.Cells[1, 12].Value = "Deleted Flag";
            #endregion

            #region processing
            int rc = 2;
            ContainerSizes = ContainerSizes.OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < ContainerSizes.Count(); i++)
            {
                ContainerSizeWS.Cells[rc, 1].Value = ContainerSizes[i].PublicID;
                ContainerSizeWS.Cells[rc, 2].Value = ContainerSizes[i].Description;
                ContainerSizeWS.Cells[rc, 3].Value = ContainerSizes[i].SortOrder;
                ContainerSizeWS.Cells[rc, 4].Value = ContainerSizes[i].CreatedDateTime.ToOADate();
                ContainerSizeWS.Cells[rc, 5].Value = ContainerSizes[i].CreatedMethod;

                if (users.Any(x => x.ID == ContainerSizes[i].CreatedUserID))
                {
                    ContainerSizeWS.Cells[rc, 6].Value = users.FirstOrDefault(x => x.ID == ContainerSizes[i].CreatedUserID).PublicID;
                    ContainerSizeWS.Cells[rc, 7].Value = users.FirstOrDefault(x => x.ID == ContainerSizes[i].CreatedUserID).Firstname;
                }
                else
                    ContainerSizeWS.Cells[rc, 6].Value = "UserID: " + ContainerSizes[i].CreatedUserID.ToString();

                ContainerSizeWS.Cells[rc, 8].Value = ContainerSizes[i].LastAmendedDateTime.ToOADate();
                ContainerSizeWS.Cells[rc, 9].Value = ContainerSizes[i].LastAmendedMethod;

                if (users.Any(x => x.ID == ContainerSizes[i].LastAmendedUserID))
                {
                    ContainerSizeWS.Cells[rc, 10].Value = users.FirstOrDefault(x => x.ID == ContainerSizes[i].LastAmendedUserID).PublicID;
                    ContainerSizeWS.Cells[rc, 11].Value = users.FirstOrDefault(x => x.ID == ContainerSizes[i].LastAmendedUserID).Firstname;
                }
                else
                    ContainerSizeWS.Cells[rc, 10].Value = "UserID: " + ContainerSizes[i].LastAmendedUserID.ToString();

                ContainerSizeWS.Cells[rc, 12].Value = ContainerSizes[i].DeletedFlag;

                rc++;
            }
            #endregion
        }
    }
}
