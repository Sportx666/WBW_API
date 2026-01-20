using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using NLog.Web;
using System.Diagnostics;
using WBM_API.Helpers;
using WBM_API.Helpers.Excel;
using WBM_API.Models;
using WBM_API.Models.ResponseObjects;
using WBM_API.WBM_API_DB;
using static WBM_API.Helpers.ControllerHelper;

namespace WBM_API.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = "WBMJwt")]
public class ProcessingRuleProcessController : ControllerBase
{
    public IConfiguration _configuration;
    private readonly WBMDatabase _WBMDB;
    private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
    public ProcessingRuleProcessController(IConfiguration configuration, WBMDatabase wBMDB)
    {
        _configuration = configuration;
        _WBMDB = wBMDB;
    }

    [HttpPost]
    [Route("api/ProcessingRuleProcessSearch")]
    public async Task<IActionResult> Search()
    {
        string APIEndPointName = "ProcessingRuleProcessSearch";

        wbm_common.DataObjects.Log.dbRow apiTransactionLog = new wbm_common.DataObjects.Log.dbRow();

        try
        {
            APILoggingReturn APICommonReturn = await CommonAPILogging(User, Request, APIEndPointName, _WBMDB, apiTransactionLog, "IsValid", "ValidationErrors", new wbm_common.DataObjects.ProcessingRuleProcess.Search());
            apiTransactionLog = APICommonReturn.log;
            if (APICommonReturn.errorCode == 400)
                return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = APICommonReturn.error } }, false, apiTransactionLog, _WBMDB);
            else if (APICommonReturn.errorCode == 500)
                return Helpers.ActionResultUtils.Send500Response(new List<ErrorMessage> { new ErrorMessage { Error = APICommonReturn.error } }, false, apiTransactionLog, _WBMDB);

            // Do the search
            // Item1 : error message
            // Item2 : search results
            Result<List<wbm_common.DataObjects.ProcessingRuleProcess.dbRow>> ProcessingRuleProcessSearchResult = await ProcessingRuleProcessHelper.ExecuteSearch((wbm_common.DataObjects.ProcessingRuleProcess.Search)APICommonReturn.genericObject, _WBMDB);

            if (ProcessingRuleProcessSearchResult.IsException)
                return Helpers.ActionResultUtils.Send500Response(new List<ErrorMessage> { new ErrorMessage { Error = ProcessingRuleProcessSearchResult.ErrorMessage } }, false, apiTransactionLog, _WBMDB);
            else if (ProcessingRuleProcessSearchResult.IsFailure)
                return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = ProcessingRuleProcessSearchResult.ErrorMessage } }, false, apiTransactionLog, _WBMDB);
            else
                return Helpers.ActionResultUtils.Send200Response(ProcessingRuleProcessSearchResult.Value, apiTransactionLog, _WBMDB);
        }
        catch (Exception ex)
        {
            logger.Error(APIEndPointName + " : " + ex);
            return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = ex.Message } }, false, apiTransactionLog, _WBMDB);
        }
    }

    [HttpPost]
    [Route("api/ProcessingRuleProcessSave")]
    public async Task<IActionResult> Save()
    {
        string APIEndPointName = "ProcessingRuleProcessSave";
        wbm_common.DataObjects.Log.dbRow apiTransactionLog = new wbm_common.DataObjects.Log.dbRow();

        int UserID = 0;

        try
        {
            APILoggingReturn APICommonReturn = await CommonAPILogging(User, Request, APIEndPointName, _WBMDB, apiTransactionLog, "Save_IsValid", "Save_ValidationErrors", new wbm_common.DataObjects.ProcessingRuleProcess());
            apiTransactionLog = APICommonReturn.log;
            if (APICommonReturn.errorCode == 400)
                return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = APICommonReturn.error } }, false, apiTransactionLog, _WBMDB);
            else if (APICommonReturn.errorCode == 500)
                return Helpers.ActionResultUtils.Send500Response(new List<ErrorMessage> { new ErrorMessage { Error = APICommonReturn.error } }, false, apiTransactionLog, _WBMDB);

            ProcessingRuleProcessHelper.ProcessingRuleProcessSave saveClass = new ProcessingRuleProcessHelper.ProcessingRuleProcessSave((wbm_common.DataObjects.ProcessingRuleProcess)APICommonReturn.genericObject, UserID, _WBMDB);

            if (saveClass.Save())
                return Helpers.ActionResultUtils.Send200Response((wbm_common.DataObjects.ProcessingRuleProcess)APICommonReturn.genericObject, apiTransactionLog, _WBMDB);
            else
            {
                if (saveClass.ReturnState == 400)
                    return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = saveClass.ErrorMessage } }, false, apiTransactionLog, _WBMDB);
                else
                    return Helpers.ActionResultUtils.Send500Response(new List<ErrorMessage> { new ErrorMessage { Error = saveClass.ErrorMessage } }, false, apiTransactionLog, _WBMDB);

            }
        }
        catch (Exception ex)
        {
            logger.Error(APIEndPointName + " : " + ex);
            return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = ex.Message } }, false, apiTransactionLog, _WBMDB);
        }
    }

    [HttpPost]
    [Route("api/ProcessingRuleProcessExcelReport")]
    public async Task<IActionResult> ExcelReport()
    {
        string APIEndPointName = "ProcessingRuleProcessExcelReport";

        wbm_common.DataObjects.Log.dbRow apiTransactionLog = new wbm_common.DataObjects.Log.dbRow();
        try
        {
            APILoggingReturn APICommonReturn = await CommonAPILogging(User, Request, APIEndPointName, _WBMDB, apiTransactionLog, "IsValid", "ValidationErrors", new wbm_common.DataObjects.ProcessingRuleProcess.Search());
            apiTransactionLog = APICommonReturn.log;
            if (APICommonReturn.errorCode == 400)
                return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = APICommonReturn.error } }, false, apiTransactionLog, _WBMDB);
            else if (APICommonReturn.errorCode == 500)
                return Helpers.ActionResultUtils.Send500Response(new List<ErrorMessage> { new ErrorMessage { Error = APICommonReturn.error } }, false, apiTransactionLog, _WBMDB);

            // Create the excel doc
            ProcessingRuleProcessExcel CE = new ProcessingRuleProcessExcel((wbm_common.DataObjects.ProcessingRuleProcess.Search)APICommonReturn.genericObject, _WBMDB);

            if (await CE.GenerateExcel())
            {
#if DEBUG
                return await debugWriteFile(CE.ms, apiTransactionLog, _WBMDB, "ProcessingRuleProcessExcelReport");
#else
                return Helpers.ActionResultUtils.Send200Response(CE.ms.ToArray(), apiTransactionLog, _WBMDB);
#endif
            }
            else
                return Helpers.ActionResultUtils.Send500Response(new List<ErrorMessage> { new ErrorMessage { Error = CE.ErrorMessage } }, false, apiTransactionLog, _WBMDB);
        }
        catch (Exception ex)
        {
            logger.Error(APIEndPointName + ": " + ex);
            return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = ex.Message } }, false, apiTransactionLog, _WBMDB);
        }
    }

}