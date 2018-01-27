namespace Identity.Web.Features.Account
{
    public class LogoutViewModel : LogoutInputModel
    {
        public bool ShowLogoutPrompt { get; set; }
    }


    public class LogoutInputModel
    {
        public string LogoutId { get; set; }
    }
}
