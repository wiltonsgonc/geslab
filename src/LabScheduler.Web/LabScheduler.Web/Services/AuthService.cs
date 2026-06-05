using LabScheduler.Web.Models;
using System.Net.Http.Json;

namespace LabScheduler.Web.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly ApiClient _api;

        public AuthService(HttpClient http, ApiClient api)
        {
            _http = http;
            _api = api;
        }

        public async Task<LoginResponse?> LoginAsync(string username, string password)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/auth/login", new LoginRequest
                {
                    Username = username,
                    Password = password
                });

                if (!response.IsSuccessStatusCode) return null;

                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result != null)
                {
                    _api.SetAuthToken(result.Token);
                }
                return result;
            }
            catch
            {
                return null;
            }
        }

        public void Logout()
        {
            _api.ClearAuthToken();
        }
    }
}
