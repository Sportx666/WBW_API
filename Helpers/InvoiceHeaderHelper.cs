using Microsoft.AspNetCore.SignalR;
using NLog;
using NLog.Web;
using System.Data.SqlTypes;
using wbm_common.NonDatabaseObjects;
using WBM_API.WBM_API_DB;
using wbm_common.DataObjects;

namespace WBM_API.Helpers
{
    public class InvoiceHeaderHelper
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public static async Task<Result<List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended>>> ExecuteSearch(wbm_common.DataObjects.InvoiceHeader.Search _InvoiceHeaderSearchData, WBMDatabase _WBMDB, IHubCallerClients Clients = null, Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> inCallBackStatusFunction = null, wbm_common.DataObjects.LongRunProcessHeader.dbRow longRunProcessHeader = null)
        {
            Result<List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended>> rc;
            List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended> InvoiceHeaders;
            switch (_InvoiceHeaderSearchData.SearchMode)
            {
                case wbm_common.DataObjects.InvoiceHeader.Search.SearchModeType.Undefined:
                    {
                        _InvoiceHeaderSearchData.GenerateSqlQuery();
                        rc = await _WBMDB.InvoiceHeader_ListBySql(_InvoiceHeaderSearchData.SqlQuery, _InvoiceHeaderSearchData.ListSqlParameter);
                        break;
                    }
                case wbm_common.DataObjects.InvoiceHeader.Search.SearchModeType.SingleID:
                    {
                        rc = await _WBMDB.InvoiceHeader_ListByID(_InvoiceHeaderSearchData.ID);
                        break;
                    }
                case wbm_common.DataObjects.InvoiceHeader.Search.SearchModeType.AllRecords:
                    {
                        rc = await _WBMDB.InvoiceHeader_List();
                        break;
                    }
                default:
                    {
                        logger.Error("InvoiceHeaderHelper.ExecuteSearch: InvoiceHeader Search Mode Type Error");
                        rc = Result.Exception<List<wbm_common.DataObjects.InvoiceHeader.dbRowExtended>>("Internal Exception occured");
                        break;
                    }
            }
            if (inCallBackStatusFunction != null)
                await inCallBackStatusFunction(Clients, _WBMDB, "SearchCompleted", 1, 1, longRunProcessHeader);

            return rc;
        }
    }
}
