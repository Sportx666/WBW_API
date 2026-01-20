using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using WBM_API.Helpers;
using WBM_API.Models.ResponseObjects;
using WBM_API.WBM_API_DB;

namespace WBM_API.Controllers
{
    [Route("api/authenticate")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Azure")]
    public class AuthenticationController(IConfiguration config, WBMDatabase WBMDB) : ControllerBase
    {
        public IConfiguration _configuration = config;
        private readonly WBMDatabase _WBMDB = WBMDB;

        [HttpPost]
        public async Task<IActionResult> Get()
        {
            wbm_common.DataObjects.Log.dbRow apiTransactionLog = new wbm_common.DataObjects.Log.dbRow();
            apiTransactionLog.APIEndPoint = "api/authenticate";
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

            var name = User.Identity?.Name ?? "(no name)";
            var UserEmail = User.FindFirst(ClaimTypes.Upn)?.Value ?? User.Identity?.Name;

            if (!_WBMDB.Log_APITransactionToDB(ref apiTransactionLog))
                return Helpers.ActionResultUtils.Send500Response(new List<ErrorMessage> { new ErrorMessage { Error = ErrorCodeHelper.ErrorCodeAsString(ErrorCodes.LogDB) } }, false, apiTransactionLog, _WBMDB);

            // Validate that we have user and create token
            AuthenticateHelper authenticateHelper = new AuthenticateHelper();
            return await authenticateHelper.AuthenticateUser("", UserEmail, "", apiTransactionLog, _configuration, _WBMDB);
        }
    }
}
