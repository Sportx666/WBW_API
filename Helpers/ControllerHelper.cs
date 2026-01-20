using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens.Experimental;
using Newtonsoft.Json;
using NLog;
using NLog.Web;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WBM_API.Helpers.Excel;
using WBM_API.Models.ResponseObjects;
using WBM_API.WBM_API_DB;
using static WBM_API.Helpers.LongRunProcessHelper;

namespace WBM_API.Helpers
{
    public class ControllerHelper
    {
        private static Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
        private static LongRunProcessHelper LRPH = new LongRunProcessHelper();
        public class LogHolder
        {
            public wbm_common.DataObjects.Log.dbRow Log { get; set; }
            public bool success { get; set; }
        }

        public class UnpackReturn
        {
            public bool success { get; set; }
            public wbm_common.DataObjects.Log.dbRow log { get; set; }
            public object genericObject { get; set; }
            public wbm_common.DataObjects.LongRunProcessHeader.dbRow LRPH { get; set; }
        }

        public class APILoggingReturn
        {
            public string error { get; set; }
            public int errorCode { get; set; }
            public wbm_common.DataObjects.Log.dbRow log { get; set; }
            public object genericObject { get; set; }
        }

        public static async Task<LogHolder> CreateNewLog(ClaimsPrincipal User, HttpRequest Request, string APIEndpoint, WBMDatabase _WBMDB, int CounterID, string requeststring = null, JwtSecurityToken JWTST = null)
        {
            if (requeststring != null) // placeholder
            {
                return new LogHolder
                {
                    success = true,
                    Log = new wbm_common.DataObjects.Log.dbRow
                    {
                        APIEndPoint = APIEndpoint,
                        CallTime = DateTime.Now,
                        Authenticated = true,
                        UserID = Convert.ToInt32(JWTST.Claims.FirstOrDefault(x => x.Type == "ID").Value),
                        UserName = JWTST.Claims.FirstOrDefault(x => x.Type == "Firstname").Value + " " + JWTST.Claims.FirstOrDefault(x => x.Type == "Surname").Value,
                        APICallReference = "",
                        ErrorText = "",
                        ReturnData = "",
                        APITransactionReference = await _WBMDB.ApiTransActionReferenceNumber_Get(CounterID),
                        CallingData = requeststring
                    }
                };
            }
            //Get Athenticated user details from JWT for validating DB mappings
            var claimsIdentity = User.Identity as ClaimsIdentity;
            int userID = Convert.ToInt32(claimsIdentity.FindFirst("ID").Value);
            string Firstname = claimsIdentity.FindFirst("Firstname").Value.ToString();
            string Surname = claimsIdentity.FindFirst("Surname").Value.ToString();

            DateTime timeOfCall = DateTime.Now;

            wbm_common.DataObjects.Log.dbRow apiTransactionLog = new wbm_common.DataObjects.Log.dbRow();
            //Get an Api Transaction Reference Number
            string apiTransactionRefNumber = await _WBMDB.ApiTransActionReferenceNumber_Get(CounterID);
            if (apiTransactionRefNumber != null)
                apiTransactionLog.APITransactionReference = apiTransactionRefNumber;
            else
                return new LogHolder
                {
                    Log = apiTransactionLog,
                    success = false,
                };

            //Create a transaction log object
            apiTransactionLog.APIEndPoint = APIEndpoint;
            apiTransactionLog.CallTime = timeOfCall;
            apiTransactionLog.Authenticated = true;
            apiTransactionLog.UserID = userID;
            apiTransactionLog.UserName = Firstname + " " + Surname;
            apiTransactionLog.APICallReference = "";
            apiTransactionLog.ErrorText = "";
            apiTransactionLog.ReturnData = "";

            string request;
            // Read the request data from the request body, catch is above so we can tell difference in failures
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                request = await reader.ReadToEndAsync();
            }
            //Log the request data
            apiTransactionLog.CallingData = request;
            return new LogHolder
            {
                Log = apiTransactionLog,
                success = true,
            };
        }

        private static string tempFolder
        {
            get
            {
                try
                {
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetTempPath() + "WBM_API");
                }
                catch (Exception excpt)
                {
                    logger.Error("Trying to create a folder for Excel temp file" + excpt.Message);
                    return "";
                }
                return System.IO.Path.GetTempPath() + "WBM_API\\";
            }
        }
        public static string GetExcelTempName()
        {
            return tempFolder == "" ? "" : tempFolder + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
        }

        public static async Task<UnpackReturn> CommonUnpack<dynamic>(IHubCallerClients Clients,
            WBMDatabase _WBMDB, string endpointname, string RequestStr, DateTime startOfCall, wbm_common.DataObjects.Log.dbRow apiTransactionLog, dynamic StringAndTokenInput)
        {
            var StringAndTokenInputErrorMessage = WBM_API.Helpers.JsonUtils.PopulateModelFromRequest(RequestStr, ref StringAndTokenInput);
            string token = (string)StringAndTokenInput.GetType().GetProperty("token").GetValue(StringAndTokenInput);
            var _dynamicSearchData = StringAndTokenInput.GetType().GetProperty("search").GetValue(StringAndTokenInput);
            JwtSecurityToken securityToken = new JwtSecurityToken();
            if (StringAndTokenInputErrorMessage != null)
            {
                await Clients.Caller.SendAsync("ERROR", JsonConvert.SerializeObject(new List<ErrorMessage> { new ErrorMessage { Error = StringAndTokenInputErrorMessage } }));
                return new UnpackReturn
                {
                    success = false,
                    log = apiTransactionLog,
                    genericObject = null,
                    LRPH = null
                };
            }
            try
            {
                securityToken = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(token);
            }
            catch (Exception e)
            {
                await Clients.Caller.SendAsync("ERROR", JsonConvert.SerializeObject(new List<ErrorMessage> { new ErrorMessage { Error = "Unauthorized token" } }));
                return new UnpackReturn
                {
                    success = false,
                    log = apiTransactionLog,
                    genericObject = null,
                    LRPH = null
                };
            }

            try
            {
                // first thing is to write to db log we received something. Don't include the token in the log input
                LogHolder CNLReturn = await ControllerHelper.CreateNewLog(null, null, endpointname, _WBMDB, 2, JsonConvert.SerializeObject(_dynamicSearchData), securityToken);
                if (!CNLReturn.success)
                {
                    logger.Error(endpointname + ": Failed to create Log - Error " + ErrorCodeHelper.ErrorCodeAsString(ErrorCodes.CreateLog));
                    await Clients.Caller.SendAsync("ERROR", JsonConvert.SerializeObject(new List<ErrorMessage> { new ErrorMessage { Error = ErrorCodeHelper.ErrorCodeAsString(ErrorCodes.CreateLog) } }));
                    return new UnpackReturn
                    {
                        success = false,
                        log = apiTransactionLog,
                        genericObject = null,
                        LRPH = null
                    };
                }
                apiTransactionLog = CNLReturn.Log;
                if (!_WBMDB.Log_APITransactionToDB(ref apiTransactionLog))
                {
                    await Clients.Caller.SendAsync("ERROR", JsonConvert.SerializeObject(new List<ErrorMessage> { new ErrorMessage { Error = ErrorCodeHelper.ErrorCodeAsString(ErrorCodes.LogDB) } }));
                    return new UnpackReturn
                    {
                        success = false,
                        log = apiTransactionLog,
                        genericObject = null,
                        LRPH = null
                    };
                }
            }
            catch (Exception ex)
            {
                logger.Error(endpointname + ": Failed at first stage, " + ex.Message);
                ErrorCodeHelper.updateErrorLog(apiTransactionLog, startOfCall, "Failed at first stage, " + ex.Message, _WBMDB);
                await Clients.Caller.SendAsync("ERROR", JsonConvert.SerializeObject(new List<ErrorMessage> { new ErrorMessage { Error = ErrorCodeHelper.ErrorCodeAsString(ErrorCodes.FirstStage) } }));
                return new UnpackReturn
                {
                    success = false,
                    log = apiTransactionLog,
                    genericObject = null,
                    LRPH = null
                };
            }

            // search object validate, needs to all be dynamic since this is used for many functions
            List<Tuple<string, string>> validationErrors = null;
            if (!(bool)_dynamicSearchData.GetType().GetMethod("IsValid").Invoke(_dynamicSearchData, null))
            {
                //400 error
                validationErrors = (List<Tuple<string, string>>)_dynamicSearchData.GetType().GetProperty("ValidationErrors").GetValue(_dynamicSearchData);
                logger.Error(endpointname + ": " + string.Join(";", validationErrors.Select(x => x.Item1 + ", " + x.Item2).ToList()));
                ErrorCodeHelper.updateErrorLog(apiTransactionLog, startOfCall, string.Join(";", validationErrors.Select(x => x.Item1 + ", " + x.Item2).ToList()), _WBMDB);
                await Clients.Caller.SendAsync("ERROR", JsonConvert.SerializeObject(validationErrors.Select(x => new ErrorMessage { Error = x.Item1 + ", " + x.Item2 }).ToList()));
                return new UnpackReturn
                {
                    success = false,
                    log = apiTransactionLog,
                    genericObject = _dynamicSearchData,
                    LRPH = null
                };
            }
            validationErrors = (List<Tuple<string, string>>)_dynamicSearchData.GetType().GetProperty("ValidationErrors").GetValue(_dynamicSearchData);

            wbm_common.DataObjects.LongRunProcessHeader.dbRow longRunProcessHeader = LRPH.CreateLongRunTaskHeader(apiTransactionLog.UserID, endpointname, 1, 0, _WBMDB);
            if (longRunProcessHeader == null)
            {
                //400 error
                logger.Error(endpointname + ": LongRunProcessHeader failed to create");
                ErrorCodeHelper.updateErrorLog(apiTransactionLog, startOfCall, "LongRunProcessHeader failed to create", _WBMDB);
                await Clients.Caller.SendAsync("ERROR", JsonConvert.SerializeObject(validationErrors.Select(x => new ErrorMessage { Error = ErrorCodeHelper.ErrorCodeAsString(ErrorCodes.GeneralDB) }).ToList()));
                return new UnpackReturn
                {
                    success = false,
                    log = apiTransactionLog,
                    genericObject = _dynamicSearchData,
                    LRPH = longRunProcessHeader
                };
            }
            longRunProcessHeader = LRPH.CreateStatus(longRunProcessHeader, 1, 0, 0, endpointname + " Start", _WBMDB);
            if (longRunProcessHeader == null)
            {
                //400 error
                logger.Error(endpointname + ": LongRunProcessStatus failed to create");
                ErrorCodeHelper.updateErrorLog(apiTransactionLog, startOfCall, "LongRunProcessStatus failed to create", _WBMDB);
                await Clients.Caller.SendAsync("ERROR", JsonConvert.SerializeObject(validationErrors.Select(x => new ErrorMessage { Error = ErrorCodeHelper.ErrorCodeAsString(ErrorCodes.GeneralDB) }).ToList()));
                return new UnpackReturn
                {
                    success = false,
                    log = apiTransactionLog,
                    genericObject = _dynamicSearchData,
                    LRPH = longRunProcessHeader
                };
            }
            return new UnpackReturn
            {
                success = true,
                log = apiTransactionLog,
                genericObject = _dynamicSearchData,
                LRPH = longRunProcessHeader
            };
        }

        public static async Task<string> callBackToClient(IHubCallerClients Clients,
            WBMDatabase _WBMDB, string StatusDescription, int CurCounter, int maxCounter, wbm_common.DataObjects.LongRunProcessHeader.dbRow lrph)
        {
            wbm_common.DataObjects.LongRunProcessHeader.dbRow longRunProcessHeader = LRPH.CreateStatus(lrph, 1, maxCounter, CurCounter, StatusDescription, _WBMDB);
            await Clients.Caller.SendAsync("StatusUpdate", JsonConvert.SerializeObject(new LongRunProcessStatusUpdate
            {
                StatusDescription = StatusDescription,
                CurrentCounter = CurCounter,
                MaxCounter = maxCounter,
                dateTime = longRunProcessHeader.StatusDateTime
            }));
            return "";
        }

        public static async Task<IActionResult> debugWriteFile(MemoryStream ms, wbm_common.DataObjects.Log.dbRow apiTransactionLog, WBMDatabase _WBMDB, string routineName)
        {
            string ExcelFile = ControllerHelper.GetExcelTempName();
            FileInfo exportFile = new FileInfo(ExcelFile);

            if (exportFile.Exists)
            {
                try
                {
                    exportFile.Delete();
                    exportFile = new FileInfo(ExcelFile);
                }
                catch (Exception excpt)
                {
                    logger.Error(routineName + ": " + excpt.Message);
                    return Helpers.ActionResultUtils.Send500Response(new List<ErrorMessage> { new ErrorMessage { Error = ErrorCodeHelper.ErrorCodeAsString(ErrorCodes.LocalFileFolder) } }, false, apiTransactionLog, _WBMDB);
                }
            }
            try
            {
                using (FileStream file = new FileStream(ExcelFile, FileMode.Create, System.IO.FileAccess.Write))
                    ms.WriteTo(file);
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo(ExcelFile)
                {
                    UseShellExecute = true
                };
                p.Start();
                return Helpers.ActionResultUtils.Send200Response(ms.ToArray(), apiTransactionLog, _WBMDB);
            }
            catch (Exception excpt)
            {
                logger.Error(routineName + ": " + excpt.Message);
                return Helpers.ActionResultUtils.Send500Response(new List<ErrorMessage> { new ErrorMessage { Error = ErrorCodeHelper.ErrorCodeAsString(ErrorCodes.LocalFileFolder) } }, false, apiTransactionLog, _WBMDB);
            }
        }

        public static async Task<APILoggingReturn> CommonAPILogging<dynamic>(ClaimsPrincipal User, HttpRequest Request, 
            string APIEndPointName, WBMDatabase _WBMDB, wbm_common.DataObjects.Log.dbRow apiTransactionLog, string validMethodName, string validationParameterName, dynamic model)
        {
            #region start log
            try
            {
                // first thing is to write to db log we received something
                LogHolder CNLReturn = await ControllerHelper.CreateNewLog(User, Request, APIEndPointName, _WBMDB, 2);
                if (!CNLReturn.success)
                {
                    logger.Error(APIEndPointName + " : Failed to create Log - Error " + ErrorCodes.CreateLog.ToString());
                    return new APILoggingReturn
                    {
                        error = ErrorCodeHelper.ErrorCodeAsString(ErrorCodes.CreateLog),
                        errorCode = 500,
                        log = apiTransactionLog,
                        genericObject = null
                    };
                }
                apiTransactionLog = CNLReturn.Log;
                if (!_WBMDB.Log_APITransactionToDB(ref apiTransactionLog))
                    return new APILoggingReturn
                    {
                        error = ErrorCodeHelper.ErrorCodeAsString(ErrorCodes.LogDB),
                        errorCode = 500,
                        log = apiTransactionLog,
                        genericObject = null
                    };
            }
            catch (Exception ex)
            {
                logger.Error(APIEndPointName + ": Failed at first stage, " + ex.Message);
                return new APILoggingReturn
                {
                    error = ErrorCodeHelper.ErrorCodeAsString(ErrorCodes.FirstStage),
                    errorCode = 400,
                    log = apiTransactionLog,
                    genericObject = null
                };
            }
            #endregion

            #region Check for JSON
            // Check if JSON
            if (!Request.ContentType.Contains("application/json"))
            {
                logger.Error(APIEndPointName + " : Submission must be in json");
                return new APILoggingReturn
                {
                    error = "Submission must be in json",
                    errorCode = 400,
                    log = apiTransactionLog,
                    genericObject = null
                };
            }
            #endregion

            #region populate search object
            // Populate search object and validate
            var ErrorMessage = Helpers.JsonUtils.PopulateModelFromRequest(apiTransactionLog.CallingData, ref model);
            if (ErrorMessage != null)
            {
                //400 error
                logger.Error(APIEndPointName + " : " + ErrorMessage);
                return new APILoggingReturn
                {
                    error = ErrorMessage,
                    errorCode = 400,
                    log = apiTransactionLog,
                    genericObject = null
                };
            }
            else if (validMethodName != "" && validationParameterName != "" && !(bool)model.GetType().GetMethod(validMethodName).Invoke(model, null))
            {
                //400 error
                List<Tuple<string, string>> validationErrors = (List<Tuple<string, string>>)model.GetType().GetProperty(validationParameterName).GetValue(model);
                logger.Error(APIEndPointName + " : " + string.Join(";", validationErrors.Select(x => x.Item1 + ", " + x.Item2).ToList()));
                return new APILoggingReturn
                {
                    error = string.Join(";", validationErrors.Select(x => x.Item1 + ", " + x.Item2)),
                    errorCode = 400,
                    log = apiTransactionLog,
                    genericObject = null
                };
            }
            #endregion

            return new APILoggingReturn
            {
                error = "",
                errorCode = 0,
                log = apiTransactionLog,
                genericObject = model
            };
        }
    }
}