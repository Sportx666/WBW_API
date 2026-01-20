using Microsoft.AspNetCore.SignalR;
using NLog;
using NLog.Web;
using System.Data.SqlTypes;
using WBM_API.Models;
using WBM_API.WBM_API_DB;

namespace WBM_API.Helpers
{
    public class LogHelper
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public static async Task<Result<List<wbm_common.DataObjects.Log.dbRow>>> ExecuteSearch(wbm_common.DataObjects.Log.Search _LogSearchData, WBMDatabase _WBMDB, IHubCallerClients Clients = null, Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> inCallBackStatusFunction = null, wbm_common.DataObjects.LongRunProcessHeader.dbRow longRunProcessHeader = null)
        {
            Result<List<wbm_common.DataObjects.Log.dbRow>> rc;
            List<wbm_common.DataObjects.Log.dbRow> Logs;
            switch (_LogSearchData.SearchMode)
            {
                case wbm_common.DataObjects.Log.Search.SearchModeType.AllRecords:
                    {
                        rc = await _WBMDB.Log_List();
                        break;
                    }
                default:
                    {
                        logger.Error("LogHelper.ExecuteSearch: Log Search Mode Type Error");
                        rc = Result.Exception<List<wbm_common.DataObjects.Log.dbRow>>("Internal Exception occured");
                        break;
                    }
            }
            if (inCallBackStatusFunction != null)
                await inCallBackStatusFunction(Clients, _WBMDB, "SearchCompleted", 1, 1, longRunProcessHeader);

            return rc;
        }
    }
}
