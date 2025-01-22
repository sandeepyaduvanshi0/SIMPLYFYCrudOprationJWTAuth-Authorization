using Client.Extra.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Client.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;
        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyApiClient"); ;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SignUp(string userid, string pass)
        {
            string apiUrl = "Auth/login";
            var payload = new
            {
                email = userid,
                password = pass
            };
            string jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                LoginResnpose loginResponse = JsonConvert.DeserializeObject<LoginResnpose>(responseContent);
                if (loginResponse.flag == true)
                {
                    Globle.IsLogin = true;
                    Globle.Token = loginResponse.token;
                    TempData["massage"] = loginResponse.message;
                    Globle.UserName = userid;
                    return RedirectToAction("Index", "Emp");
                }
                else
                {

                    Globle.UserName = "";
                    Globle.IsLogin = true;
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (HttpRequestException ex)
            {

                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Login");
        }
        public IActionResult Logout()
        {
            Globle.IsLogin = false;
            Globle.Token = "";
            TempData["massage"] = "Logout Now";
            Globle.UserName = "";
            return RedirectToAction("Index", "Login");
        }



        public IActionResult Registration()
        {
            return View();
        }

        public async Task<IActionResult> Register(string fullName, string registerEmail, string registerPassword, string confirmPass)
        {
            string apiUrl = "Auth/register";
            var payload = new
            {
                email = registerEmail,
                password = registerPassword,
                fullname = fullName,
                confirmPassword = confirmPass
            };
            string jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();

            }
            catch (HttpRequestException ex)
            {

                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Login");
        }





    }
}
