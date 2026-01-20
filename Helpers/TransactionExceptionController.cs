using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using NLog.Web;
using System.Diagnostics;
using WBM_API.Helpers.Excel;
using WBM_API.Models;
using WBM_API.Models.ResponseObjects;
using WBM_API.WBM_API_DB;
using static WBM_API.Helpers.ControllerHelper;

namespace WBM_API.Helpers;

[ApiController]
[Authorize(AuthenticationSchemes = "WBMJwt")]
public class TransactionExceptionController : ControllerBase
{
    public IConfiguration _configuration;
    private readonly WBMDatabase _WBMDB;
    private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
    public TransactionExceptionController(IConfiguration configuration, WBMDatabase wBMDB)
    {
        _configuration = configuration;
        _WBMDB = wBMDB;
    }

    [HttpPost]
    [Route("api/TransactionExceptionSearch")]
    public async Task<IActionResult> Search()
    {
        string APIEndPointName = "TransactionExceptionSearch";

        wbm_common.DataObjects.Log.dbRow apiTransactionLog = new wbm_common.DataObjects.Log.dbRow();

        try
        {
            APILoggingReturn APICommonReturn = await CommonAPILogging(User, Request, APIEndPointName, _WBMDB, apiTransactionLog, "IsValid", "ValidationErrors", new wbm_common.DataObjects.TransactionException.Search());
            apiTransactionLog = APICommonReturn.log;
            if (APICommonReturn.errorCode == 400)
                return ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = APICommonReturn.error } }, false, apiTransactionLog, _WBMDB);
            else if (APICommonReturn.errorCode == 500)
                return ActionResultUtils.Send500Response(new List<ErrorMessage> { new ErrorMessage { Error = APICommonReturn.error } }, false, apiTransactionLog, _WBMDB);

            // Do the search
            // Item1 : error message
            // Item2 : search results
            Result<List<wbm_common.DataObjects.TransactionException.dbRow>> TransactionExceptionSearchResult = await TransactionExceptionHelper.ExecuteSearch((wbm_common.DataObjects.TransactionException.Search)APICommonReturn.genericObject, _WBMDB);

            if (TransactionExceptionSearchResult.IsException)
                return ActionResultUtils.Send500Response(new List<ErrorMessage> { new ErrorMessage { Error = TransactionExceptionSearchResult.ErrorMessage } }, false, apiTransactionLog, _WBMDB);
            else if (TransactionExceptionSearchResult.IsFailure)
                return ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = TransactionExceptionSearchResult.ErrorMessage } }, false, apiTransactionLog, _WBMDB);
            else
                return ActionResultUtils.Send200Response(TransactionExceptionSearchResult.Value, apiTransactionLog, _WBMDB);
        }
        catch (Exception ex)
        {
            logger.Error(APIEndPointName + " : " + ex);
            return ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = ex.Message } }, false, apiTransactionLog, _WBMDB);
        }
    }

    [HttpPost]
    [Route("api/TransactionExceptionSave")]
    public async Task<IActionResult> Save()
    {
        string APIEndPointName = "TransactionExceptionSave";
        wbm_common.DataObjects.Log.dbRow apiTransactionLog = new wbm_common.DataObjects.Log.dbRow();

        int UserID = 0;

        try
        {
            APILoggingReturn APICommonReturn = await CommonAPILogging(User, Request, APIEndPointName, _WBMDB, apiTransactionLog, "Save_IsValid", "Save_ValidationErrors", new wbm_common.DataObjects.TransactionException());
            apiTransactionLog = APICommonReturn.log;
            if (APICommonReturn.errorCode == 400)
                return ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = APICommonReturn.error } }, false, apiTransactionLog, _WBMDB);
            else if (APICommonReturn.errorCode == 500)
                return ActionResultUtils.Send500Response(new List<ErrorMessage> { new ErrorMessage { Error = APICommonReturn.error } }, false, apiTransactionLog, _WBMDB);

            TransactionExceptionHelper.TransactionExceptionSave saveClass = new TransactionExceptionHelper.TransactionExceptionSave((wbm_common.DataObjects.TransactionException)APICommonReturn.genericObject, UserID, _WBMDB);

            if (saveClass.Save())
                return ActionResultUtils.Send200Response((wbm_common.DataObjects.TransactionException)APICommonReturn.genericObject, apiTransactionLog, _WBMDB);
            else
            {
                if (saveClass.ReturnState == 400)
                    return ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = saveClass.ErrorMessage } }, false, apiTransactionLog, _WBMDB);
                else
                    return ActionResultUtils.Send500Response(new List<ErrorMessage> { new ErrorMessage { Error = saveClass.ErrorMessage } }, false, apiTransactionLog, _WBMDB);

            }
        }
        catch (Exception ex)
        {
            logger.Error(APIEndPointName + " : " + ex);
            return ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = ex.Message } }, false, apiTransactionLog, _WBMDB);
        }
    }

    [HttpPost]
    [Route("api/TransactionExceptionExcelReport")]
    public async Task<IActionResult> ExcelReport()
    {
        string APIEndPointName = "TransactionExceptionExcelReport";

        wbm_common.DataObjects.Log.dbRow apiTransactionLog = new wbm_common.DataObjects.Log.dbRow();
        try
        {
            APILoggingReturn APICommonReturn = await CommonAPILogging(User, Request, APIEndPointName, _WBMDB, apiTransactionLog, "IsValid", "ValidationErrors", new wbm_common.DataObjects.TransactionException.Search());
            apiTransactionLog = APICommonReturn.log;
            if (APICommonReturn.errorCode == 400)
                return ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = APICommonReturn.error } }, false, apiTransactionLog, _WBMDB);
            else if (APICommonReturn.errorCode == 500)
                return ActionResultUtils.Send500Response(new List<ErrorMessage> { new ErrorMessage { Error = APICommonReturn.error } }, false, apiTransactionLog, _WBMDB);

            // Create the excel doc
            TransactionExceptionExcel CE = new TransactionExceptionExcel((wbm_common.DataObjects.TransactionException.Search)APICommonReturn.genericObject, _WBMDB);

            if (await CE.GenerateExcel())
            {
#if DEBUG
                return await debugWriteFile(CE.ms, apiTransactionLog, _WBMDB, "TransactionExceptionExcelReport");
#else
                return Helpers.ActionResultUtils.Send200Response(CE.ms.ToArray(), apiTransactionLog, _WBMDB);
#endif
            }
            else
                return ActionResultUtils.Send500Response(new List<ErrorMessage> { new ErrorMessage { Error = CE.ErrorMessage } }, false, apiTransactionLog, _WBMDB);
        }
        catch (Exception ex)
        {
            logger.Error(APIEndPointName + ": " + ex);
            return ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = ex.Message } }, false, apiTransactionLog, _WBMDB);
        }
    }

}