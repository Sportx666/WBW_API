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
    public class AwaitingPurchOrdDWExcel
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public MemoryStream ms;

        private readonly WBMDatabase _WBMDB;
        private readonly PaperlessDatabase _PaperlessDB;

        public List<PURCH_ORD_PROD_DW> PURCH_ORD_PROD_DWDb;
        public List<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow> AwaitingPurchOrdDWs = null;
        public List<wbm_common.DataObjects.Owner.dbRow> Owners = null;
        public Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> CallBackStatusFunction { get; set; }
        private IHubCallerClients Clients;
        public wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph { get; set; }
        public string ErrorMessage { get; set; }

        public Boolean HasErrors = false;

        public wbm_common.DataObjects.AwaitingPurchOrdDW.Search AwaitingPurchOrdDWSearchData;
        public AwaitingPurchOrdDWExcel(wbm_common.DataObjects.AwaitingPurchOrdDW.Search AwaitingPurchOrdDWSearchData, WBMDatabase wBMDB, PaperlessDatabase papDB)
        {
            ms = new MemoryStream();

            this.AwaitingPurchOrdDWSearchData = AwaitingPurchOrdDWSearchData;
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
                        AwaitingPurchOrdDWPage(ep);
                        ep.Save();
                    }
                }
                catch (Exception excpt)
                {
                    logger.Error("AwaitingPurchOrdDWExcelReport: " + excpt.Message);
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

            Result<List<wbm_common.DataObjects.AwaitingPurchOrdDW.dbRow>> rc = await AwaitingPurchOrdDWHelper.ExecuteSearch(AwaitingPurchOrdDWSearchData, _WBMDB);

            if (rc.IsFailure)
            {
                ErrorMessage = rc.ErrorMessage;
                HasErrors = true;
                return false;
            }

            AwaitingPurchOrdDWs = rc.Value;

            // Build list of IDs to retrieve related data
            List<string> ListPURCH_ORD_PROD_DWKEYIDs = AwaitingPurchOrdDWs.Select(x => x.KEYID).Distinct().ToList();
            Result<List<PURCH_ORD_PROD_DW>> prc = await _PaperlessDB.PURCH_ORD_PROD_DW_ListByKEYID(ListPURCH_ORD_PROD_DWKEYIDs);
            if (prc.IsFailure)
            {
                ErrorMessage = prc.ErrorMessage;
                HasErrors = true;
                return false;
            }
            PURCH_ORD_PROD_DWDb = prc.Value;

            Result<List<wbm_common.DataObjects.Owner.dbRow>> orc = await _WBMDB.Owner_List();
            if (orc.IsFailure)
            {
                ErrorMessage = orc.ErrorMessage;
                HasErrors = true;
                return false;
            }
            Owners = orc.Value;

            return true;
        }

        private void AwaitingPurchOrdDWPage(ExcelPackage ep)
        {
            ExcelWorksheet AwaitingPurchOrdDWWS = ep.Workbook.Worksheets.Add("AwaitingPurchOrdDW");

            #region header
            AwaitingPurchOrdDWWS.View.FreezePanes(2, 1);
            AwaitingPurchOrdDWWS.Cells[1, 1, 1, 9].Style.Font.Bold = true;
            AwaitingPurchOrdDWWS.Cells[1, 1, 1, 9].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            AwaitingPurchOrdDWWS.Column(1).Width = 25;
            AwaitingPurchOrdDWWS.Column(2).Width = 15;
            AwaitingPurchOrdDWWS.Column(3).Width = 25;
            AwaitingPurchOrdDWWS.Column(4).Width = 25;
            AwaitingPurchOrdDWWS.Column(5).Width = 30;
            AwaitingPurchOrdDWWS.Column(6).Width = 25;
            AwaitingPurchOrdDWWS.Column(6).Style.Numberformat.Format = "dd/mm/yy HH:mm";
            AwaitingPurchOrdDWWS.Column(7).Width = 15;
            AwaitingPurchOrdDWWS.Column(8).Width = 25;
            AwaitingPurchOrdDWWS.Column(9).Width = 15;

            AwaitingPurchOrdDWWS.Cells[1, 1].Value = "PO_NO";
            AwaitingPurchOrdDWWS.Cells[1, 2].Value = "Site";
            AwaitingPurchOrdDWWS.Cells[1, 3].Value = "Owner Number";
            AwaitingPurchOrdDWWS.Cells[1, 4].Value = "Owner Name";
            AwaitingPurchOrdDWWS.Cells[1, 5].Value = "SUPP_NAME";
            AwaitingPurchOrdDWWS.Cells[1, 6].Value = "PO Date";
            AwaitingPurchOrdDWWS.Cells[1, 7].Value = "RECP_TYPE";
            AwaitingPurchOrdDWWS.Cells[1, 8].Value = "CONTAINER_NO";
            AwaitingPurchOrdDWWS.Cells[1, 9].Value = "CONTAINER_SIZE";
            #endregion

            #region processing
            int rc = 2;
            PURCH_ORD_PROD_DWDb = PURCH_ORD_PROD_DWDb.GroupBy(x => x.PO_NO).Select(x => new PURCH_ORD_PROD_DW
            {
                PO_NO = x.Key,
                SITE_NO = x.First().SITE_NO,
                KEYID = x.First().KEYID,
                SUPP_NAME = x.First().SUPP_NAME,
                DATE_RECV = x.First().DATE_RECV,
                RECP_TYPE = x.Any(y => y.RECP_TYPE != "") ? x.Where(y => y.RECP_TYPE != "").First().RECP_TYPE : "",
                CONTAINER_NO = x.Any(y => y.CONTAINER_NO != "") ? x.Where(y => y.CONTAINER_NO != "").First().CONTAINER_NO : "",
                CONTAINER_SIZE = x.Any(y => y.CONTAINER_SIZE != "") ? x.Where(y => y.CONTAINER_SIZE != "").First().CONTAINER_SIZE : ""
            }).OrderBy(x => x.KEYID).ToList();
            for (int i = 0; i < PURCH_ORD_PROD_DWDb.Count(); i++)
            {
                AwaitingPurchOrdDWWS.Cells[rc, 1].Value = PURCH_ORD_PROD_DWDb[i].PO_NO;
                AwaitingPurchOrdDWWS.Cells[rc, 2].Value = PURCH_ORD_PROD_DWDb[i].SITE_NO;

                if (PURCH_ORD_PROD_DWDb[i].KEYID.Contains("_"))
                {
                    string owner = PURCH_ORD_PROD_DWDb[i].KEYID.Split('_')[1];
                    if (owner.Length >= 5)
                    {
                        owner = owner.Substring(0, 5);
                        AwaitingPurchOrdDWWS.Cells[rc, 3].Value = owner;
                        Owner.dbRow OwnObj = Owners.FirstOrDefault(x => x.SourceOwnerID == owner);
                        if (OwnObj != null && OwnObj.Active)
                            AwaitingPurchOrdDWWS.Cells[rc, 4].Value = OwnObj.Name;
                    }
                }

                AwaitingPurchOrdDWWS.Cells[rc, 5].Value = PURCH_ORD_PROD_DWDb[i].SUPP_NAME;
                if (PURCH_ORD_PROD_DWDb[i].DATE_RECV.HasValue)
                    AwaitingPurchOrdDWWS.Cells[rc, 6].Value = PURCH_ORD_PROD_DWDb[i].DATE_RECV.Value.ToOADate();
                AwaitingPurchOrdDWWS.Cells[rc, 7].Value = PURCH_ORD_PROD_DWDb[i].RECP_TYPE;
                AwaitingPurchOrdDWWS.Cells[rc, 8].Value = PURCH_ORD_PROD_DWDb[i].CONTAINER_NO;
                AwaitingPurchOrdDWWS.Cells[rc, 9].Value = PURCH_ORD_PROD_DWDb[i].CONTAINER_SIZE;

                rc++;
            }
            #endregion
        }
    }
}
