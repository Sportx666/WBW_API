using Microsoft.AspNetCore.SignalR;
using NLog;
using NLog.Web;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Collections.Generic;
using WBM_API.Paperless_DB;
using WBM_API.WBM_API_DB;
using wbm_common.DataObjects;
using wbm_common.NonDatabaseObjects;
using wbm_common.Paperless_DB_DataObjects;

namespace WBM_API.Helpers.Excel
{
    public class AwaitingPickHeadDWExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;
        private readonly PaperlessDatabase _PaperlessDB;

        public List<PICK_HEAD_DW> PICK_HEAD_DWDb;
        public List<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow> AwaitingPickHeadDWs = null;
        public List<wbm_common.DataObjects.Owner.dbRow> Owners = null;
        public List<wbm_common.DataObjects.Site.dbRowExtended> Sites = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.AwaitingPickHeadDW.Search AwaitingPickHeadDWSearchData;
        public AwaitingPickHeadDWExcel(wbm_common.DataObjects.AwaitingPickHeadDW.Search AwaitingPickHeadDWSearchData, WBMDatabase wBMDB, PaperlessDatabase papDB)
        {
            ms = new MemoryStream();

            this.AwaitingPickHeadDWSearchData = AwaitingPickHeadDWSearchData;
            _WBMDB = wBMDB;
            _PaperlessDB = papDB;
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
                        AwaitingPickHeadDWPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("AwaitingPickHeadDWExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.AwaitingPickHeadDW.dbRow>> rc = await AwaitingPickHeadDWHelper.ExecuteSearch(AwaitingPickHeadDWSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            AwaitingPickHeadDWs = rc.Value;

            // Build list of IDs to retrieve related data
            List<string> ListPICK_HEAD_DWKEYIDs = AwaitingPickHeadDWs.Select(x => x.KEYID).Distinct().ToList();
            Result<List<PICK_HEAD_DW>> prc = await _PaperlessDB.PICK_HEAD_DW_ListByKEYID(ListPICK_HEAD_DWKEYIDs);
            if (prc.IsFailure)
            {
                ErrorMessage = prc.ErrorMessage;
                HasErrors = true;
                return false;
            }
            PICK_HEAD_DWDb = prc.Value;

            Result<List<wbm_common.DataObjects.Owner.dbRow>> orc = await _WBMDB.Owner_List();
            if (orc.IsFailure)
            {
                ErrorMessage = orc.ErrorMessage;
                HasErrors = true;
                return false;
            }
            Owners = orc.Value;

            Result<List<wbm_common.DataObjects.Site.dbRowExtended>> src = await _WBMDB.Site_List();
            if (src.IsFailure)
            {
                ErrorMessage = src.ErrorMessage;
                HasErrors = true;
                return false;
            }
            Sites = src.Value;

            return true;
        }

        private void AwaitingPickHeadDWPage(ExcelPackage ep)
        {
            ExcelWorksheet AwaitingPickHeadDWWS = ep.Workbook.Worksheets.Add("AwaitingPickHeadDW");

            #region header
            AwaitingPickHeadDWWS.View.FreezePanes(2, 1);
            AwaitingPickHeadDWWS.Cells[1, 1, 1, 13].Style.Font.Bold = true;
            AwaitingPickHeadDWWS.Cells[1, 1, 1, 13].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            AwaitingPickHeadDWWS.Column(1).Width = 25;
            AwaitingPickHeadDWWS.Column(2).Width = 15;
            AwaitingPickHeadDWWS.Column(3).Width = 25;
            AwaitingPickHeadDWWS.Column(4).Width = 25;
            AwaitingPickHeadDWWS.Column(5).Width = 30;
            AwaitingPickHeadDWWS.Column(6).Width = 25;
            AwaitingPickHeadDWWS.Column(7).Width = 15;
            AwaitingPickHeadDWWS.Column(8).Width = 25;
            AwaitingPickHeadDWWS.Column(9).Width = 25;
            AwaitingPickHeadDWWS.Column(10).Width = 25;
            AwaitingPickHeadDWWS.Column(11).Width = 25;
            AwaitingPickHeadDWWS.Column(12).Width = 25;
            AwaitingPickHeadDWWS.Column(12).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            AwaitingPickHeadDWWS.Column(13).Width = 25;

            AwaitingPickHeadDWWS.Cells[1, 1].Value = "Order Number";
            AwaitingPickHeadDWWS.Cells[1, 2].Value = "Site";
            AwaitingPickHeadDWWS.Cells[1, 3].Value = "Owner Number";
            AwaitingPickHeadDWWS.Cells[1, 4].Value = "Owner Name";
            AwaitingPickHeadDWWS.Cells[1, 5].Value = "CUST_NAME";
            AwaitingPickHeadDWWS.Cells[1, 6].Value = "CUST_SUB";
            AwaitingPickHeadDWWS.Cells[1, 7].Value = "CUST_PCODE";
            AwaitingPickHeadDWWS.Cells[1, 8].Value = "CUST_ST1";
            AwaitingPickHeadDWWS.Cells[1, 9].Value = "CUST_ST2";
            AwaitingPickHeadDWWS.Cells[1, 10].Value = "CUST_ORD_NO";
            AwaitingPickHeadDWWS.Cells[1, 11].Value = "Pick Status Code";
            AwaitingPickHeadDWWS.Cells[1, 12].Value = "Order Date";
            AwaitingPickHeadDWWS.Cells[1, 13].Value = "CARRIER_CODE";
            #endregion

            #region processing
            int rc = 2;
            PICK_HEAD_DWDb = PICK_HEAD_DWDb.OrderBy(x => x.KEYID).ToList();
            for (int i = 0; i < PICK_HEAD_DWDb.Count(); i++)
            {
                AwaitingPickHeadDWWS.Cells[rc, 1].Value = PICK_HEAD_DWDb[i].KEYID;
                if (PICK_HEAD_DWDb[i].KEYID.Contains("_"))
                {
                    string site = PICK_HEAD_DWDb[i].KEYID.Split('_')[0];
                    Site.dbRowExtended SiteObj = Sites.FirstOrDefault(x => x.PublicID == site);
                    if (SiteObj != null && SiteObj.Active)
                        AwaitingPickHeadDWWS.Cells[rc, 2].Value = SiteObj.Company_ShortName;

                    string owner = PICK_HEAD_DWDb[i].KEYID.Split('_')[1];
                    if (owner.Length >= 5)
                    {
                        owner = owner.Substring(0, 5);
                        AwaitingPickHeadDWWS.Cells[rc, 3].Value = owner;
                        Owner.dbRow OwnObj = Owners.FirstOrDefault(x => x.SourceOwnerID == owner);
                        if (OwnObj != null && OwnObj.Active)
                            AwaitingPickHeadDWWS.Cells[rc, 4].Value = OwnObj.Name;
                    }
                }

                AwaitingPickHeadDWWS.Cells[rc, 5].Value = PICK_HEAD_DWDb[i].CUST_NAME;
                AwaitingPickHeadDWWS.Cells[rc, 6].Value = PICK_HEAD_DWDb[i].CUST_SUB;
                AwaitingPickHeadDWWS.Cells[rc, 7].Value = PICK_HEAD_DWDb[i].CUST_PCODE;
                AwaitingPickHeadDWWS.Cells[rc, 8].Value = PICK_HEAD_DWDb[i].CUST_ST1;
                AwaitingPickHeadDWWS.Cells[rc, 9].Value = PICK_HEAD_DWDb[i].CUST_ST2;
                AwaitingPickHeadDWWS.Cells[rc, 10].Value = PICK_HEAD_DWDb[i].CUST_ORD_NO;
                AwaitingPickHeadDWWS.Cells[rc, 11].Value = PICK_HEAD_DWDb[i].PHEAD_STATUS;
                if (PICK_HEAD_DWDb[i].DATE_RECV.HasValue && PICK_HEAD_DWDb[i].TIME_RECV.HasValue)
                    AwaitingPickHeadDWWS.Cells[rc, 12].Value = (PICK_HEAD_DWDb[i].DATE_RECV.Value.Date.Add(PICK_HEAD_DWDb[i].TIME_RECV.Value)).ToOADate();
                AwaitingPickHeadDWWS.Cells[rc, 13].Value = PICK_HEAD_DWDb[i].CARRIER_CODE;

                rc++;
            }
            #endregion
        }
    }
}
