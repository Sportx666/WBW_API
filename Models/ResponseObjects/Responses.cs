
namespace WBM_API.Models.ResponseObjects
{
    public class AuthenticationResponse
    {
        public string? APITransactionReference { get; set; }

        public string Token { get; set; }
        public DateTime TokenExpiry { get; set; }
        public string AccessList { get; set; }
    }
}
