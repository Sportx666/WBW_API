using Azure.Core;
using CTI.WebApi.Models.RequestObjects;
using Microsoft.AspNetCore.Mvc;
using NLog;
using NLog.Web;
using System.Text;
using WBM_API.Helpers;
using WBM_API.Models.ResponseObjects;
using WBM_API.WBM_API_DB;

namespace WBM_API.Controllers
{
    // Following only available when run in debug mode
#if DEBUG
    [Route("api/debugauthenticate")]
    [ApiController]
    public class DebugAuthenticationController(IConfiguration config, WBMDatabase WBMDB) : ControllerBase
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public IConfiguration _configuration = config;
        private readonly WBMDatabase _WBMDB = WBMDB;

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            string error = "";
            wbm_common.DataObjects.Log.dbRow apiTransactionLog = new wbm_common.DataObjects.Log.dbRow();
            apiTransactionLog.APIEndPoint = "api/debugauthenticate";
            apiTransactionLog.CallTime = DateTime.Now;
            apiTransactionLog.Authenticated = false;
            apiTransactionLog.APICallReference = "";
            apiTransactionLog.APITransactionReference = "";
            apiTransactionLog.ErrorText = "";
            apiTransactionLog.ReturnData = "";
            apiTransactionLog.UserName = "";
            // this can't be testing by just me in postman
            try
            {
                if (Request.HttpContext.Connection.RemoteIpAddress.ToString() != "::1")
                {
                    logger.Error("debugauthenticate : OI! no touchy touchy our API, they're calling from " + Request.HttpContext.Connection.RemoteIpAddress.ToString());
                    return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = "OI! no touchy touchy our API" } }, false, apiTransactionLog, _WBMDB);
                }
            }
            catch (Exception ex)
            {
                logger.Error("debugauthenticate : IPAddressError");
                return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = ErrorCodeHelper.ErrorCodeAsString(ErrorCodes.FirstStage) } }, false, apiTransactionLog, _WBMDB);
            }
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                apiTransactionLog.CallingData = await reader.ReadToEndAsync();
            }
            if (!Request.ContentType.Contains("application/json"))
            {
                logger.Error("debugauthenticate : Submission must be in json");
                return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = "Submission must be in json" } }, false, apiTransactionLog, _WBMDB);
            }
            if (!_WBMDB.Log_APITransactionToDB(ref apiTransactionLog))
            {
                logger.Error("debugauthenticate : LogDB Error");
                return Helpers.ActionResultUtils.Send500Response(new List<ErrorMessage> { new ErrorMessage { Error = ErrorCodeHelper.ErrorCodeAsString(ErrorCodes.LogDB) } }, false, apiTransactionLog, _WBMDB);
            }

            Authenticate _authenticateData = new Authenticate();
            var submissionContent = Helpers.JsonUtils.PopulateModelFromRequest(apiTransactionLog.CallingData, ref _authenticateData);
            if (submissionContent != null)
            {
                //400 error
                logger.Error("debugauthenticate : " + submissionContent);
                return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = submissionContent } }, false, apiTransactionLog, _WBMDB);
            }

            // Validate that we have user and create token
            AuthenticateHelper authenticateHelper = new AuthenticateHelper();
            return await authenticateHelper.AuthenticateUser(_authenticateData.Username, "", _authenticateData.Password, apiTransactionLog, _configuration, _WBMDB);
        }
    }
#endif
}
