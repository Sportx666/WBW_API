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
public class Security_RoleGroup_RoleController : ControllerBase
{
    public IConfiguration _configuration;
    private readonly WBMDatabase _WBMDB;
    private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
    public Security_RoleGroup_RoleController(IConfiguration configuration, WBMDatabase wBMDB)
    {
        _configuration = configuration;
        _WBMDB = wBMDB;
    }

    [HttpPost]
    [Route("api/Security_RoleGroup_RoleSearch")]
    public async Task<IActionResult> Search()
    {
        string APIEndPointName = "Security_RoleGroup_RoleSearch";

        wbm_common.DataObjects.Log.dbRow apiTransactionLog = new wbm_common.DataObjects.Log.dbRow();

        try
        {
            APILoggingReturn APICommonReturn = await CommonAPILogging(User, Request, APIEndPointName, _WBMDB, apiTransactionLog, "", "", new wbm_common.DataObjects.Security_RoleGroup_Role.Search());
            apiTransactionLog = APICommonReturn.log;
            if (APICommonReturn.errorCode == 400)
                return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = APICommonReturn.error } }, false, apiTransactionLog, _WBMDB);
            else if (APICommonReturn.errorCode == 500)
                return Helpers.ActionResultUtils.Send500Response(new List<ErrorMessage> { new ErrorMessage { Error = APICommonReturn.error } }, false, apiTransactionLog, _WBMDB);

            // Do the search
            // Item1 : error message
            // Item2 : search results
            Result<List<wbm_common.DataObjects.Security_RoleGroup_Role.dbRow>> Security_RoleGroup_RoleSearchResult = await Security_RoleGroup_RoleHelper.ExecuteSearch((wbm_common.DataObjects.Security_RoleGroup_Role.Search)APICommonReturn.genericObject, _WBMDB);

            if (Security_RoleGroup_RoleSearchResult.IsException)
                return Helpers.ActionResultUtils.Send500Response(new List<ErrorMessage> { new ErrorMessage { Error = Security_RoleGroup_RoleSearchResult.ErrorMessage } }, false, apiTransactionLog, _WBMDB);
            else if (Security_RoleGroup_RoleSearchResult.IsFailure)
                return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = Security_RoleGroup_RoleSearchResult.ErrorMessage } }, false, apiTransactionLog, _WBMDB);
            else
                return Helpers.ActionResultUtils.Send200Response(Security_RoleGroup_RoleSearchResult.Value, apiTransactionLog, _WBMDB);
        }
        catch (Exception ex)
        {
            logger.Error(APIEndPointName + " : " + ex);
            return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = ex.Message } }, false, apiTransactionLog, _WBMDB);
        }
    }

    [HttpPost]
    [Route("api/Security_RoleGroup_RoleExcelReport")]
    public async Task<IActionResult> ExcelReport()
    {
        string APIEndPointName = "Security_RoleGroup_RoleExcelReport";

        wbm_common.DataObjects.Log.dbRow apiTransactionLog = new wbm_common.DataObjects.Log.dbRow();
        try
        {
            APILoggingReturn APICommonReturn = await CommonAPILogging(User, Request, APIEndPointName, _WBMDB, apiTransactionLog, "", "", new wbm_common.DataObjects.Security_RoleGroup_Role.Search());
            apiTransactionLog = APICommonReturn.log;
            if (APICommonReturn.errorCode == 400)
                return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = APICommonReturn.error } }, false, apiTransactionLog, _WBMDB);
            else if (APICommonReturn.errorCode == 500)
                return Helpers.ActionResultUtils.Send500Response(new List<ErrorMessage> { new ErrorMessage { Error = APICommonReturn.error } }, false, apiTransactionLog, _WBMDB);

            // Create the excel doc
            Security_RoleGroup_RoleExcel CE = new Security_RoleGroup_RoleExcel((wbm_common.DataObjects.Security_RoleGroup_Role.Search)APICommonReturn.genericObject, _WBMDB);

            if (await CE.GenerateExcel())
            {
#if DEBUG
                return await debugWriteFile(CE.ms, apiTransactionLog, _WBMDB, "Security_RoleGroup_RoleExcelReport");
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