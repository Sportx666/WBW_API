using Microsoft.AspNetCore.SignalR;
using NLog;
using NLog.Web;
using System.Data.SqlTypes;
using WBM_API.Models;
using WBM_API.WBM_API_DB;

namespace WBM_API.Helpers
{
    public class Security_User_RoleHelper
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public static async Task<Result<List<wbm_common.DataObjects.Security_User_Role.dbRow>>> ExecuteSearch(wbm_common.DataObjects.Security_User_Role.Search _Security_User_RoleSearchData, WBMDatabase _WBMDB, IHubCallerClients Clients = null, Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> inCallBackStatusFunction = null, wbm_common.DataObjects.LongRunProcessHeader.dbRow longRunProcessHeader = null)
        {
            Result<List<wbm_common.DataObjects.Security_User_Role.dbRow>> rc;
            List<wbm_common.DataObjects.Security_User_Role.dbRow> Security_User_Roles;
            switch (_Security_User_RoleSearchData.SearchMode)
            {
                case wbm_common.DataObjects.Security_User_Role.Search.SearchModeType.AllRecords:
                    {
                        rc = await _WBMDB.Security_User_Role_List();
                        break;
                    }
                default:
                    {
                        logger.Error("Security_User_RoleHelper.ExecuteSearch: Security_User_Role Search Mode Type Error");
                        rc = Result.Exception<List<wbm_common.DataObjects.Security_User_Role.dbRow>>("Internal Exception occured");
                        break;
                    }
            }
            if (inCallBackStatusFunction != null)
                await inCallBackStatusFunction(Clients, _WBMDB, "SearchCompleted", 1, 1, longRunProcessHeader);

            return rc;
        }
    }
}
