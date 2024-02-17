using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GestWeb.Models;

namespace GestWeb.Services
{
    public class AuthenticationService
    {
        private readonly HttpClient _http;
        public string Token { get; private set; }
        public bool IsLoading { get; private set; }
        public bool IsError { get; private set; }

        public AuthenticationService(HttpClient http)
        {
            _http = http;
        }

        public async Task<bool> LoginAsync(UserApp user)
        {
            IsLoading = true;
            IsError = false;

            try
            {
                var response = await _http.PostAsJsonAsync("http://localhost:7298/Auth/login", user);
                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                    if (tokenResponse != null)
                    {
                        Token = tokenResponse.Token;
                        return true;
                    }
                }
            }
            catch
            {
                IsError = true;
            }
            finally
            {
                IsLoading = false;
            }

            return false;
        }
    }
}
