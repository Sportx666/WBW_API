
namespace CTI.WebApi.Models.RequestObjects
{
    public class Authenticate
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string CallReference { get; set; }
    }
}