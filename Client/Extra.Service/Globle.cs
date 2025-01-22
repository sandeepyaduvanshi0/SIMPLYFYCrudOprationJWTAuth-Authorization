namespace Client.Extra.Service
{
    public static class Globle
    {
        public static bool IsLogin = false;
        public static string Token;
        public static string UserName;

    }
    public class LoginResnpose
    {
        public bool flag { get; set; }
        public string? message { get; set; }
        public string? token { get; set; }

    }
}
