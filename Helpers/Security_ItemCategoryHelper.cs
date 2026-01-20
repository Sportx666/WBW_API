using Microsoft.AspNetCore.SignalR;
using NLog;
using NLog.Web;
using System.Data.SqlTypes;
using WBM_API.Models;
using WBM_API.WBM_API_DB;

namespace WBM_API.Helpers
{
    public class Security_ItemCategoryHelper
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public static async Task<Result<List<wbm_common.DataObjects.Security_ItemCategory.dbRow>>> ExecuteSearch(wbm_common.DataObjects.Security_ItemCategory.Search _Security_ItemCategorySearchData, WBMDatabase _WBMDB, IHubCallerClients Clients = null, Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> inCallBackStatusFunction = null, wbm_common.DataObjects.LongRunProcessHeader.dbRow longRunProcessHeader = null)
        {
            Result<List<wbm_common.DataObjects.Security_ItemCategory.dbRow>> rc;
            List<wbm_common.DataObjects.Security_ItemCategory.dbRow> Security_ItemCategorys;
            switch (_Security_ItemCategorySearchData.SearchMode)
            {
                case wbm_common.DataObjects.Security_ItemCategory.Search.SearchModeType.AllRecords:
                    {
                        rc = await _WBMDB.Security_ItemCategory_List();
                        break;
                    }
                default:
                    {
                        logger.Error("Security_ItemCategoryHelper.ExecuteSearch: Security_ItemCategory Search Mode Type Error");
                        rc = Result.Exception<List<wbm_common.DataObjects.Security_ItemCategory.dbRow>>("Internal Exception occured");
                        break;
                    }
            }
            if (inCallBackStatusFunction != null)
                await inCallBackStatusFunction(Clients, _WBMDB, "SearchCompleted", 1, 1, longRunProcessHeader);

            return rc;
        }
    }
}
