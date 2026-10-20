using CTI.WebApi.Models.RequestObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using NLog.Web;
using System.Text;
using WBM_API.Helpers;
using WBM_API.Models.ResponseObjects;
using WBM_API.WBM_API_DB;

namespace WBM_API.Controllers
{
    [Route("api/serviceauthenticate")]
    [ApiController]
    public class ServiceAuthenticationController(IConfiguration config, WBMDatabase WBMDB) : ControllerBase
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public IConfiguration _configuration = config;
        private readonly WBMDatabase _WBMDB = WBMDB;

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            wbm_common.DataObjects.Log.dbRow apiTransactionLog = new wbm_common.DataObjects.Log.dbRow();
            apiTransactionLog.APIEndPoint = "api/serviceauthenticate";
            apiTransactionLog.CallTime = DateTime.Now;
            apiTransactionLog.Authenticated = false;
            apiTransactionLog.APICallReference = "";
            apiTransactionLog.APITransactionReference = "";
            apiTransactionLog.ErrorText = "";
            apiTransactionLog.ReturnData = "";
            apiTransactionLog.UserName = "";

            // Enforce mTLS: client must present a certificate
            var clientCert = Request.HttpContext.Connection.ClientCertificate;
            if (!MtlsHelper.IsClientCertificateAllowed(clientCert, _configuration, out string certReason))
            {
                logger.Error("serviceauthenticate : mTLS failure - " + certReason);
                return ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = certReason } }, false, apiTransactionLog, _WBMDB);
            }

            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                apiTransactionLog.CallingData = await reader.ReadToEndAsync();
            }

            if (Request.ContentType == null || !Request.ContentType.Contains("application/json"))
            {
                logger.Error("serviceauthenticate : Submission must be in json");
                return ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = "Submission must be in json" } }, false, apiTransactionLog, _WBMDB);
            }

            if (!_WBMDB.Log_APITransactionToDB(ref apiTransactionLog))
            {
                logger.Error("serviceauthenticate : LogDB Error");
                return ActionResultUtils.Send500Response(new List<ErrorMessage> { new ErrorMessage { Error = ErrorCodeHelper.ErrorCodeAsString(ErrorCodes.LogDB) } }, false, apiTransactionLog, _WBMDB);
            }

            Authenticate _authenticateData = new Authenticate();
            var submissionContent = JsonUtils.PopulateModelFromRequest(apiTransactionLog.CallingData, ref _authenticateData);
            if (submissionContent != null)
            {
                logger.Error("serviceauthenticate : " + submissionContent);
                return ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = submissionContent } }, false, apiTransactionLog, _WBMDB);
            }

            if (string.IsNullOrWhiteSpace(_authenticateData.Username) || string.IsNullOrWhiteSpace(_authenticateData.Password))
            {
                logger.Error("serviceauthenticate : Username and Password are required");
                return ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = "Username and Password are required" } }, false, apiTransactionLog, _WBMDB);
            }

            apiTransactionLog.APICallReference = _authenticateData.CallReference ?? "";

            // Validate that we have user and create token
            AuthenticateHelper authenticateHelper = new AuthenticateHelper();
            return await authenticateHelper.AuthenticateUser(_authenticateData.Username, "", _authenticateData.Password, apiTransactionLog, _configuration, _WBMDB);
        }
    }
}
