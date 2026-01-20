using Microsoft.AspNetCore.SignalR;
using NLog;
using NLog.Web;
using System.Data.SqlTypes;
using WBM_API.Models;
using WBM_API.WBM_API_DB;

namespace WBM_API.Helpers
{
    public class Security_RoleGroupHelper
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public static async Task<Result<List<wbm_common.DataObjects.Security_RoleGroup.dbRow>>> ExecuteSearch(wbm_common.DataObjects.Security_RoleGroup.Search _Security_RoleGroupSearchData, WBMDatabase _WBMDB, IHubCallerClients Clients = null, Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> inCallBackStatusFunction = null, wbm_common.DataObjects.LongRunProcessHeader.dbRow longRunProcessHeader = null)
        {
            Result<List<wbm_common.DataObjects.Security_RoleGroup.dbRow>> rc;
            List<wbm_common.DataObjects.Security_RoleGroup.dbRow> Security_RoleGroups;
            switch (_Security_RoleGroupSearchData.SearchMode)
            {
                case wbm_common.DataObjects.Security_RoleGroup.Search.SearchModeType.AllRecords:
                    {
                        rc = await _WBMDB.Security_RoleGroup_List();
                        break;
                    }
                default:
                    {
                        logger.Error("Security_RoleGroupHelper.ExecuteSearch: Security_RoleGroup Search Mode Type Error");
                        rc = Result.Exception<List<wbm_common.DataObjects.Security_RoleGroup.dbRow>>("Internal Exception occured");
                        break;
                    }
            }
            if (inCallBackStatusFunction != null)
                await inCallBackStatusFunction(Clients, _WBMDB, "SearchCompleted", 1, 1, longRunProcessHeader);

            return rc;
        }
    }
}
