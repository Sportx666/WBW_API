using CTI.WebApi.Models.RequestObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using NLog.Web;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WBM_API.Helpers;
using WBM_API.Models.ResponseObjects;
using WBM_API.WBM_API_DB;

namespace WBM_API.Controllers
{
    [Route("api/mtlsauthenticate")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "MTLS")]
    public class MtlsAuthenticationController(IConfiguration config, WBMDatabase WBMDB) : ControllerBase
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        public IConfiguration _configuration = config;
        private readonly WBMDatabase _WBMDB = WBMDB;

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            wbm_common.DataObjects.Log.dbRow apiTransactionLog = new wbm_common.DataObjects.Log.dbRow();
            apiTransactionLog.APIEndPoint = "api/mtlsauthenticate";
            apiTransactionLog.CallTime = DateTime.Now;
            apiTransactionLog.Authenticated = false;
            apiTransactionLog.APICallReference = "";
            apiTransactionLog.APITransactionReference = "";
            apiTransactionLog.ErrorText = "";
            apiTransactionLog.ReturnData = "";
            apiTransactionLog.UserName = "";

            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                apiTransactionLog.CallingData = await reader.ReadToEndAsync();
            }

            X509Certificate2? clientCertificate = Request.HttpContext.Connection.ClientCertificate;
            if (clientCertificate == null)
            {
                logger.Error("mtlsauthenticate : Client certificate required");
                return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage>
                {
                    new ErrorMessage { Error = "Client certificate required" }
                }, false, apiTransactionLog, _WBMDB);
            }

            if (string.IsNullOrWhiteSpace(Request.ContentType) || !Request.ContentType.Contains("application/json"))
            {
                logger.Error("mtlsauthenticate : Submission must be in json");
                return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage>
                {
                    new ErrorMessage { Error = "Submission must be in json" }
                }, false, apiTransactionLog, _WBMDB);
            }

            if (!_WBMDB.Log_APITransactionToDB(ref apiTransactionLog))
            {
                logger.Error("mtlsauthenticate : LogDB Error");
                return Helpers.ActionResultUtils.Send500Response(new List<ErrorMessage>
                {
                    new ErrorMessage { Error = ErrorCodeHelper.ErrorCodeAsString(ErrorCodes.LogDB) }
                }, false, apiTransactionLog, _WBMDB);
            }

            Authenticate authenticateData = new Authenticate();
            var submissionContent = Helpers.JsonUtils.PopulateModelFromRequest(apiTransactionLog.CallingData, ref authenticateData);
            if (submissionContent != null)
            {
                logger.Error("mtlsauthenticate : " + submissionContent);
                return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage>
                {
                    new ErrorMessage { Error = submissionContent }
                }, false, apiTransactionLog, _WBMDB);
            }

            logger.Info("mtlsauthenticate : Certificate {0}", clientCertificate.Subject);

            AuthenticateHelper authenticateHelper = new AuthenticateHelper();
            return await authenticateHelper.AuthenticateUser(authenticateData.Username, "", authenticateData.Password, apiTransactionLog, _configuration, _WBMDB);
        }
    }
}
