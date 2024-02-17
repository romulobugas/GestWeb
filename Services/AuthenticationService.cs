using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GestWeb.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace GestWeb.Services
{
    public class AuthenticationService
    {
        private readonly HttpClient _http;
        private readonly CustomAuthenticationStateProvider _authenticationStateProvider;
        private readonly IConfiguration _configuration;

        public string Token { get; private set; }
        public bool IsLoading { get; private set; }
        public bool IsError { get; private set; }
        public string ErrorMessage { get; private set; }
        public string statusLogin { get; private set; }


        public AuthenticationService(HttpClient http, CustomAuthenticationStateProvider authenticationStateProvider, IConfiguration configuration)
        {
            _http = http;
            _authenticationStateProvider = authenticationStateProvider;
            _configuration = configuration;
        }

        public bool IsUserAuthenticated => !string.IsNullOrWhiteSpace(Token);

        public async Task<string> LoginAsync(UserApp user)
        {
            IsLoading = true;
            IsError = false;
            ErrorMessage = string.Empty;
            statusLogin = string.Empty;

            try
            {
                var baseUrl = _configuration["ApiSettings:BaseUrl"];
                var requestUrl = baseUrl + "/Auth/login";

                var response = await _http.PostAsJsonAsync(requestUrl, user);

                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                    if (tokenResponse != null)
                    {
                        Token = tokenResponse.Token;
                        _authenticationStateProvider.MarkUserAsAuthenticated(Token);
                        statusLogin = response.ReasonPhrase.ToString();
                        return statusLogin;
                        
                    }
                    else
                    {
                        IsError = true;
                        ErrorMessage = "A resposta da API não incluiu um token.";
                    }
                }
                else
                {
                    statusLogin = response.ReasonPhrase.ToString();
                    IsError = true;
                    ErrorMessage = "Resposta da API: " + response.StatusCode;
                }
            }
            catch (HttpRequestException e)
            {
                IsError = true;
                ErrorMessage = "Falha na comunicação com a API: " + e.Message;
            }
            finally
            {
                IsLoading = false;
                
            }

            return statusLogin;
        }

        public async Task Logout()
        {
            Token = null;
            _authenticationStateProvider.MarkUserAsLoggedOut();
            await Task.CompletedTask;
        }
    }
}
