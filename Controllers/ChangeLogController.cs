using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using NLog.Web;
using System.Threading.Tasks;
using WBM_API.Helpers;
using WBM_API.WBM_API_DB;
using wbm_common.ComplexDataObjects;
using wbm_common.NonDatabaseObjects;
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

    [HttpPost]
    [Route("api/ChangeLog")]
    public async Task<IActionResult> ChangeLog()
    {
        string APIEndPointName = "ChangeLog";
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


            ChangeHeaderAndDetailLog _request = (wbm_common.ComplexDataObjects.ChangeHeaderAndDetailLog)APICommonReturn.genericObject;

            Result<wbm_common.ComplexDataObjects.ChangeHeaderAndDetailLog> changeLogResult = await ChangeLogHelper.GetChangeLog(_request.ChangeHeaderTableID, _request.RecordID, _WBMDB);

            if (changeLogResult.IsSuccess)
                return Helpers.ActionResultUtils.Send200Response(changeLogResult.Value, apiTransactionLog, _WBMDB);
            else if (changeLogResult.IsFailure)
                return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = changeLogResult.ErrorMessage } }, false, apiTransactionLog, _WBMDB);
            else
                return Helpers.ActionResultUtils.Send500Response(new List<ErrorMessage> { new ErrorMessage { Error = changeLogResult.ErrorMessage } }, false, apiTransactionLog, _WBMDB);
        }
        catch (Exception ex)
        {
            logger.Error(APIEndPointName + " : " + ex);
            return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = ex.Message } }, false, apiTransactionLog, _WBMDB);
        }
    }
}
