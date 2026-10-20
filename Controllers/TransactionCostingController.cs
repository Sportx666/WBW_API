using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using NLog.Web;
using WBM_API.Helpers;
using wbm_common.NonDatabaseObjects;
using WBM_API.WBM_API_DB;
using static WBM_API.Helpers.ControllerHelper;

namespace WBM_API.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = "WBMJwt")]
public class TransactionCostingController : ControllerBase
{
    public IConfiguration _configuration;
    private readonly WBMDatabase _WBMDB;
    private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
    public TransactionCostingController(IConfiguration configuration, WBMDatabase wBMDB)
    {
        _configuration = configuration;
        _WBMDB = wBMDB;
    }

    [HttpPost]
    [Route("api/TransactionCostingCost")]
    public async Task<IActionResult> Cost()
    {
        string APIEndPointName = "TransactionCostingCost";

        wbm_common.DataObjects.Log.dbRow apiTransactionLog = new wbm_common.DataObjects.Log.dbRow();

        try
        {
            //APILoggingReturn APICommonReturn = await CommonAPILogging(User, Request, APIEndPointName, _WBMDB, apiTransactionLog, "IsValid", "ValidationErrors", new List<wbm_common.NonDatabaseObjects.Transaction_Costing>());
            APILoggingReturn APICommonReturn = await CommonAPILogging(User, Request, APIEndPointName, _WBMDB, apiTransactionLog, "", "", new Transaction_CostingHolder());
            apiTransactionLog = APICommonReturn.log;
            if (APICommonReturn.errorCode == 400)
                return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = APICommonReturn.error } }, false, apiTransactionLog, _WBMDB);
            else if (APICommonReturn.errorCode == 500)
                return Helpers.ActionResultUtils.Send500Response(new List<ErrorMessage> { new ErrorMessage { Error = APICommonReturn.error } }, false, apiTransactionLog, _WBMDB);

            // Do the search
            // Item1 : error message
            // Item2 : search results
            Result<Transaction_CostingHolder> CostingResult = await TransactionCostingHelper.CostList((Transaction_CostingHolder)APICommonReturn.genericObject, _WBMDB);

            if (CostingResult.IsException)
                return Helpers.ActionResultUtils.Send500Response(new List<ErrorMessage> { new ErrorMessage { Error = CostingResult.ErrorMessage } }, false, apiTransactionLog, _WBMDB);
            else if (CostingResult.IsFailure)
            {
                // Ok so if we couldn't cost something then this is a failure
                // We need to return the records and each record failed and the holder will state CostedOk = false
                // Ideally we can return this on a 400 but not sure how, so we send a 200 and calling code needs to check the details

                //return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = CostingResult.ErrorMessage } }, false, apiTransactionLog, _WBMDB);
                return Helpers.ActionResultUtils.Send200Response(CostingResult.Value, apiTransactionLog, _WBMDB);
            }
            else
                return Helpers.ActionResultUtils.Send200Response(CostingResult.Value, apiTransactionLog, _WBMDB);
        }
        catch (Exception ex)
        {
            logger.Error(APIEndPointName + " : " + ex);
            return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = ex.Message } }, false, apiTransactionLog, _WBMDB);
        }
    }
}