using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WBM_API.Models;
using WBM_API.Models.ResponseObjects;
using WBM_API.WBM_API_DB;
using static WBM_API.Helpers.AccessHelper;

namespace WBM_API.Helpers
{
    public class AuthenticateHelper
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

        public IConfiguration _configuration { get; set; }

        public WBMDatabase _WBMDB;

        public async Task<IActionResult> AuthenticateUser(string UserName, string EmailAddress, string Password, 
            wbm_common.DataObjects.Log.dbRow apiTransactionLog, IConfiguration config, WBMDatabase _WBMDB)
        {
            this._WBMDB = _WBMDB;
            this._configuration = config;

            wbm_common.DataObjects.Security_User.dbRow SecUser = null;

            // If we have email address it's an Azure login so try and get user by email address
            if (EmailAddress != "")
            {
                Result<wbm_common.DataObjects.Security_User.dbRow> SecUserResult = await _WBMDB.SecurityUser_ListByEmailAddress(EmailAddress);
                if (SecUserResult.IsFailure || SecUserResult.IsException)
                {
                    //400 error
                    apiTransactionLog.ErrorText = "Unable to Authenticate User - " + UserName + ", " + SecUserResult.ErrorMessage;
                    logger.Error("Unable to Authenticate User - " + UserName + ", " + SecUserResult.ErrorMessage);
                    return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = "Unable to Authenticate User" } }, false, apiTransactionLog, _WBMDB);
                }
                SecUser = SecUserResult.Value;
            }
            else
            {
                Result<wbm_common.DataObjects.Security_User.dbRow> SecUserResult = await _WBMDB.SecurityUser_GetByUsername(UserName, Password);
                if (SecUserResult.IsFailure || SecUserResult.IsException)
                {
                    //400 error
                    apiTransactionLog.ErrorText = "Unable to Authenticate User - " + UserName + ", " + SecUserResult.ErrorMessage;
                    logger.Error("Unable to Authenticate User - " + UserName + ", " + SecUserResult.ErrorMessage);
                    return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = "Unable to Authenticate User" } }, false, apiTransactionLog, _WBMDB);
                }
                SecUser = SecUserResult.Value;
            }

            if (SecUser == null)
            {
                //400 error
                apiTransactionLog.ErrorText = "Unable to Authenticate User - " + UserName;
                logger.Error("AuthenticateUser : Unable to Authenticate User");
                return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = "Unable to Authenticate User" } }, false, apiTransactionLog, _WBMDB);
            }

            UserName = SecUser.PublicID;

            string apiTransactionRefNumber = await _WBMDB.ApiTransActionReferenceNumber_Get(1);
            if (apiTransactionRefNumber != null)
                apiTransactionLog.APITransactionReference = apiTransactionRefNumber;

            apiTransactionLog.UserName = SecUser.Firstname + " " + SecUser.Surname;
            apiTransactionLog.UserID = SecUser.ID;
            apiTransactionLog.APICallReference = "";

            AuthenticationResponse AuthResp = CreateJWT(apiTransactionLog.APITransactionReference, SecUser, _configuration);

            if (AuthResp != null)
            {
                SecurityItemListHolder AccessLevels = await AccessHelper.GetAccessLevels(SecUser, _WBMDB);
                if (AccessLevels.error == "")
                {
                    //200 All Good
                    AuthResp.AccessList = JsonUtils.tupleToString(AccessLevels.SecurityItems.Select(x => (x.Security_Item.TagID, x.AccessLevel)).ToList());
                    return Helpers.ActionResultUtils.Send200Response(AuthResp, apiTransactionLog, _WBMDB);
                }
                else
                {
                    logger.Error("AuthenticateUser : " + AccessLevels.error);
                    return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = ErrorCodeHelper.ErrorCodeAsString(ErrorCodes.FirstStage) } }, false, apiTransactionLog, _WBMDB);
                }
            }
            else
            {
                //400 error
                logger.Error("AuthenticateUser : Unable to Authenticate User, AuthenticationResponse");
                return Helpers.ActionResultUtils.Send400Response(new List<ErrorMessage> { new ErrorMessage { Error = "Unable to Authenticate User" } }, false, apiTransactionLog, _WBMDB);
            }
        }

        private AuthenticationResponse CreateJWT(string myAPITransactionRefNo, wbm_common.DataObjects.Security_User.dbRow SecUser, IConfiguration _configuration)
        {
            try
            {
                //create claims details based on the user information
                var claims = new[] {
                                            new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                                            new Claim("ID", SecUser.ID.ToString()),
                                            new Claim("PublicID", SecUser.PublicID),
                                            new Claim("Firstname", SecUser.Firstname),
                                            new Claim("Surname", SecUser.Surname),
                                            new Claim("StartCompany", SecUser.StartCompany.ToString())
                                        };

                //Set to expire at midnight today
                var tokenExpiry = DateTime.Now.Date.AddDays(1);

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: tokenExpiry,
                    signingCredentials: signIn);

                var transactionResponse = myAPITransactionRefNo;

                var tokenResponse = new AuthenticationResponse
                {
                    APITransactionReference = transactionResponse,
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    TokenExpiry = tokenExpiry,
                    AccessList = ""
                };

                return tokenResponse;
            }
            catch (Exception ex)
            {
                logger.Error("Error AuthenticateHelper.CreateJWT: " + SecUser.PublicID + " - " + ex.Message, ex);
                return null;
            }
        }

    }
}
