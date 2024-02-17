using GestWeb;
using GestWeb.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configura o HttpClient
builder.Services.AddScoped(sp =>
{
    var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };

    // Recuperando o valor de BaseUrl do appsettings.json
    var configuration = sp.GetRequiredService<IConfiguration>();
    var baseUrl = builder.Configuration["ApiSettings:BaseUrl"];
    if (!string.IsNullOrEmpty(baseUrl))
    {
        httpClient.BaseAddress = new Uri(baseUrl);
    }

    return httpClient;
});

// Configura o AuthenticationService
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationService>();

await builder.Build().RunAsync();
