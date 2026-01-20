using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using NLog.Web;
using WBM_API.Helpers;
using WBM_API.Models.ResponseObjects;
using WBM_API.WBM_API_DB;
using static WBM_API.Helpers.ControllerHelper;

namespace WBM_API.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = "WBMJwt")]
public class ChangeLogController : ControllerBase
{
    public IConfiguration _configuration;
    private readonly WBMDatabase _WBMDB;
    private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
    public ChangeLogController(IConfiguration configuration, WBMDatabase wBMDB)
    {
        _configuration = configuration;
        _WBMDB = wBMDB;
    }

    [HttpGet]
    [Route("api/GetChangeLog")]
    public async Task<IActionResult> GetChangeLog()
    {
        string APIEndPointName = "GetChangeLog";
        string error;
        wbm_common.DataObjects.Log.dbRow apiTransactionLog = new wbm_common.DataObjects.Log.dbRow();
        try
        {
            APILoggingReturn APICommonReturn = await CommonAPILogging(User, Request, APIEndPointName, _WBMDB, apiTransactionLog, "", "", new wbm_common.ComplexDataObjects.ChangeHeaderAndDetailLog());
            apiTransactionLog = APICommonReturn.log;
            if (APICommonReturn.errorCode == 400)
                return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = APICommonReturn.error } }, false, apiTransactionLog, _WBMDB);
            else if (APICommonReturn.errorCode == 500)
                return Helpers.ActionResultUtils.Send500Response(new List<ErrorMessage> { new ErrorMessage { Error = APICommonReturn.error } }, false, apiTransactionLog, _WBMDB);

            // Do the search
            return GetChangeLog_DoWork((wbm_common.ComplexDataObjects.ChangeHeaderAndDetailLog)APICommonReturn.genericObject, apiTransactionLog);
        }
        catch (Exception ex)
        {
            logger.Error(APIEndPointName + " : " + ex);
            return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = ex.Message } }, false, apiTransactionLog, _WBMDB);
        }
    }

    private IActionResult GetChangeLog_DoWork(wbm_common.ComplexDataObjects.ChangeHeaderAndDetailLog _request, wbm_common.DataObjects.Log.dbRow apiTransactionLog)
    {
        // Get list of ChangeHeader records where the ChangeHeaderTableID and ID matches
        // Then get a list of all the changeDetail records who have the same ChangeHeaderID as the ChangeHeader records found
        // Then create ChangeEvents and populate ListChangeEvent for the return

        _request.ListChangeEvent = new List<wbm_common.ComplexDataObjects.ChangeHeaderAndDetailLog.ChangeEvent>();

        Boolean dbErrorOccured = false;

        if (!dbErrorOccured)
            return Helpers.ActionResultUtils.Send200Response(_request, apiTransactionLog, _WBMDB);
        else
            return Helpers.ActionResultUtils.Send500Response(new List<ErrorMessage> { new ErrorMessage { Error = ErrorCodeHelper.ErrorCodeAsString(ErrorCodes.GeneralDB) } }, false, apiTransactionLog, _WBMDB);
    }

}
