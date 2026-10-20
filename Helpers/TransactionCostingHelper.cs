using Microsoft.AspNetCore.SignalR;
using NLog;
using NLog.Web;
using System.Data.SqlTypes;
using wbm_common.NonDatabaseObjects;
using WBM_API.WBM_API_DB;
using wbm_common.DataObjects;
using wbm_common.NonDatabaseObjects;

namespace WBM_API.Helpers
{
    public class TransactionCostingHelper
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public static async Task<Result<Transaction_CostingHolder>> CostList(Transaction_CostingHolder sendtch, WBMDatabase _WBMDB, IHubCallerClients Clients = null, Func<IHubCallerClients, WBMDatabase, string, int, int, wbm_common.DataObjects.LongRunProcessHeader.dbRow, Task<string>> inCallBackStatusFunction = null, wbm_common.DataObjects.LongRunProcessHeader.dbRow longRunProcessHeader = null)
        {
            Result<Transaction_CostingHolder> rc;

            Result<Boolean> cost_result;

            Costing.TransactionCosting helper_tc = new Costing.TransactionCosting(sendtch, _WBMDB);
            cost_result = await helper_tc.ProcessList();

            if (cost_result.IsSuccess)
            {
                rc = Result.Success<Transaction_CostingHolder>(helper_tc.TransactionCostingHolder);
            }
            else if (cost_result.IsFailure)
            {
                rc = Result.Failure<Transaction_CostingHolder>(helper_tc.TransactionCostingHolder, cost_result.ErrorMessage);
            }
            else
            {
                rc = Result.Exception<Transaction_CostingHolder>(helper_tc.TransactionCostingHolder, cost_result.ErrorMessage);
            }

            if (inCallBackStatusFunction != null)
                await inCallBackStatusFunction(Clients, _WBMDB, "CostingCompleted", 1, 1, longRunProcessHeader);

            return rc;
        }

    }
}
