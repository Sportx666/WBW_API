using Newtonsoft.Json;

namespace WBM_API.Helpers
{
    public class JsonUtils
    {
        public static string? PopulateModelFromRequest<dynamic>(string request, ref dynamic model)
        {
            var errorMessageContainer = new ErrorMessageContainer();

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Error,
                Error = (sender, e) => JSONErrorHandler(sender, e, errorMessageContainer),
            };

            try
            {
                //Populate model from JSON in request body
                model = JsonConvert.DeserializeObject<dynamic>(request);

                if (errorMessageContainer.Message != null)
                {
                    return "Error, unable to deserialize JSON user data, the following unexpected elements were found in the request:" + errorMessageContainer.Message;
                }

            }
            catch (Exception ex)
            {
                // log in above call
                if (ex.InnerException == null)
                {
                    return "Error, unable to deserialize JSON user data : " + ex.Message;
                }

                return "Error, unable to deserialize JSON user data : " + ex.Message + ": " + ex.InnerException.Message;
            }
            return null;
        }
        private class ErrorMessageContainer
        {
            public string? Message { get; set; }
        }
        static void JSONErrorHandler(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e, ErrorMessageContainer errorContainer)
        {
            if (e.ErrorContext.Error.Message.StartsWith("Could not find member "))
            {
                var enexpectedElement = "Error:[" + e.ErrorContext.Error.Message + "]";
                if (errorContainer.Message == null)
                {
                    errorContainer.Message = enexpectedElement;
                }
                else
                {
                    errorContainer.Message = errorContainer.Message + ", " + enexpectedElement;
                }

                // hide this error as it was handled
                e.ErrorContext.Handled = true;
            }
        }
        public static string tupleToString(List<(string, int)> list)
        {
            return string.Join("; ", list.Select(x => x.Item1 + ", " + x.Item2));
        }

    }
}
