using Microsoft.AspNetCore.SignalR;
using NLog;
using NLog.Web;
using System.Data.SqlTypes;
using WBM_API.Models;
using WBM_API.WBM_API_DB;

namespace WBM_API.Helpers
{
    public class Security_User_RoleGroupHelper
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public static async Task<Result<List<wbm_common.DataObjects.Security_User_RoleGroup.dbRow>>> ExecuteSearch(wbm_common.DataObjects.Security_User_RoleGroup.Search _Security_User_RoleGroupSearchData, WBMDatabase _WBMDB, IHubCallerClients Clients = null, Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> inCallBackStatusFunction = null, wbm_common.DataObjects.LongRunProcessHeader.dbRow longRunProcessHeader = null)
        {
            Result<List<wbm_common.DataObjects.Security_User_RoleGroup.dbRow>> rc;
            List<wbm_common.DataObjects.Security_User_RoleGroup.dbRow> Security_User_RoleGroups;
            switch (_Security_User_RoleGroupSearchData.SearchMode)
            {
                case wbm_common.DataObjects.Security_User_RoleGroup.Search.SearchModeType.AllRecords:
                    {
                        rc = await _WBMDB.Security_User_RoleGroup_List();
                        break;
                    }
                default:
                    {
                        logger.Error("Security_User_RoleGroupHelper.ExecuteSearch: Security_User_RoleGroup Search Mode Type Error");
                        rc = Result.Exception<List<wbm_common.DataObjects.Security_User_RoleGroup.dbRow>>("Internal Exception occured");
                        break;
                    }
            }
            if (inCallBackStatusFunction != null)
                await inCallBackStatusFunction(Clients, _WBMDB, "SearchCompleted", 1, 1, longRunProcessHeader);

            return rc;
        }
    }
}