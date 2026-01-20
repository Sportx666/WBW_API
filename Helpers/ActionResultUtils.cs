using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WBM_API.Models.ResponseObjects;
using WBM_API.WBM_API_DB;

namespace WBM_API.Helpers
{
    public class ActionResultUtils
    {
        public static IActionResult Send400Response(List<ErrorMessage> errorMessages, bool sanitiseResponse = false, wbm_common.DataObjects.Log.dbRow apiTransactionLog = null, WBMDatabase _WBMDB = null) // TODO currently just = null placeholder until all controllers are done, the no null check later will also help us remember this

        {
            //Generate Error response for user
            var response = new ContentResult();
            response.StatusCode = 400;
            response.ContentType = "application/json";
            string errorText = "";
            return SendGenericResponse(errorMessages, response, errorText, sanitiseResponse, apiTransactionLog, _WBMDB);
        }

        public static IActionResult Send500Response(List<ErrorMessage> errorMessages, bool sanitiseResponse = false, wbm_common.DataObjects.Log.dbRow apiTransactionLog = null, WBMDatabase _WBMDB = null) // TODO currently just = null placeholder until all controllers are done, the no null check later will also help us remember this

        {
            //Generate Error response for user
            var response = new ContentResult();
            response.StatusCode = 500;
            response.ContentType = "application/json";
            string errorText = "";
            return SendGenericResponse(errorMessages, response, errorText, sanitiseResponse, apiTransactionLog, _WBMDB);
        }

        public static IActionResult SendGenericResponse(List<ErrorMessage> errorMessages,
            ContentResult response, string errorText, bool sanitiseResponse = false, wbm_common.DataObjects.Log.dbRow apiTransactionLog = null, WBMDatabase _WBMDB = null) // TODO currently just = null placeholder until all controllers are done, the no null check later will also help us remember this

        {
            ErrorResponse errorResponse = new ErrorResponse();
            errorResponse.ErrorList = new ErrorList
            {
                Errors = errorMessages.Select(x => x.Error).ToList()
            };
            response.Content = JsonConvert.SerializeObject(errorResponse);
            errorText = response.Content;

            if (sanitiseResponse == true)
            {
                errorResponse.ErrorList = new ErrorList
                {
                    Errors = new List<string> { "Request Failed!" }
                };
                response.Content = JsonConvert.SerializeObject(errorResponse);

                response.Content = JsonConvert.SerializeObject(errorResponse, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                response.Content = response.Content;
            }
            else
            {
                response.Content = response.Content;
            }

            //Insert Log to DB
            apiTransactionLog.CallSuccess = false;
            apiTransactionLog.ReturnData = "";//response.Content;
            apiTransactionLog.ErrorText = errorText;
            apiTransactionLog.CallLength = (int)(DateTime.Now - apiTransactionLog.CallTime).TotalMilliseconds;

            var insertLogresponseResponse = _WBMDB.Log_APITransactionToDB(ref apiTransactionLog);

            return response;
        }
        public static IActionResult Send200Response<T>(T data, wbm_common.DataObjects.Log.dbRow apiTransactionLog = null, WBMDatabase _WBMDB = null) // TODO currently just = null placeholder until all controllers are done, the no null check later will also help us remember this

        {
            var response = new ContentResult();
            response.StatusCode = 200;
            response.ContentType = "api/authenticate";
            response.Content = JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            //Insert Log to DB
            apiTransactionLog.CallSuccess = true;
            apiTransactionLog.ReturnData = "";//response.Content;
            apiTransactionLog.ErrorText = "";
            apiTransactionLog.CallLength = (int)(DateTime.Now - apiTransactionLog.CallTime).TotalMilliseconds;

            var insertLogresponseResponse = _WBMDB.Log_APITransactionToDB(ref apiTransactionLog);
            _WBMDB.APICallStats_Update(apiTransactionLog.APIEndPoint, apiTransactionLog.CallTime);

            return response;
        }
    }
}
