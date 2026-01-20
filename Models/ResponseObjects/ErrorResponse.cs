
namespace WBM_API.Models.ResponseObjects
{
    public class ErrorResponse
    {
        public string APITransctionReference { get; set; }

        public ErrorList ErrorList { get; set; }
    }

    public class ErrorList
    {
        public List<string> Errors { get; set; }
    }

    public class ErrorMessage
    {
        public string Error { get; set; }
        public bool FiveHundred = false;
    }
}
