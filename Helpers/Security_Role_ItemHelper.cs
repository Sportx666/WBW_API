using Microsoft.AspNetCore.SignalR;
using NLog;
using NLog.Web;
using System.Data.SqlTypes;
using WBM_API.Models;
using WBM_API.WBM_API_DB;

namespace WBM_API.Helpers
{
    public class Security_Role_ItemHelper
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public static async Task<Result<List<wbm_common.DataObjects.Security_Role_Item.dbRow>>> ExecuteSearch(wbm_common.DataObjects.Security_Role_Item.Search _Security_Role_ItemSearchData, WBMDatabase _WBMDB, IHubCallerClients Clients = null, Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> inCallBackStatusFunction = null, wbm_common.DataObjects.LongRunProcessHeader.dbRow longRunProcessHeader = null)
        {
            Result<List<wbm_common.DataObjects.Security_Role_Item.dbRow>> rc;
            List<wbm_common.DataObjects.Security_Role_Item.dbRow> Security_Role_Items;
            switch (_Security_Role_ItemSearchData.SearchMode)
            {
                case wbm_common.DataObjects.Security_Role_Item.Search.SearchModeType.AllRecords:
                    {
                        rc = await _WBMDB.Security_Role_Item_List();
                        break;
                    }
                default:
                    {
                        logger.Error("Security_Role_ItemHelper.ExecuteSearch: Security_Role_Item Search Mode Type Error");
                        rc = Result.Exception<List<wbm_common.DataObjects.Security_Role_Item.dbRow>>("Internal Exception occured");
                        break;
                    }
            }
            if (inCallBackStatusFunction != null)
                await inCallBackStatusFunction(Clients, _WBMDB, "SearchCompleted", 1, 1, longRunProcessHeader);

            return rc;
        }
    }
}
