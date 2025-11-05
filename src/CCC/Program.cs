using CCC.Services;
using CCC.Services.Auth;
using CCC.Services.Notification;
using CCC.Services.Utils;
using CCC.Shared;
using CCC;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MudExtensions.Services;
using System.Net.Http.Headers;
using static CCC.Shared.Constants;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.UseSentry(opt =>
{
    opt.Dsn = "https://40061bf0e0cb17eb0dd789124d054141@o4510166413803520.ingest.us.sentry.io/4510166416424960";
    opt.Debug = true;
    opt.SendDefaultPii = true;

    opt.SetBeforeSend((@event) =>
    {
        if (@event.Exception is ControledException)
        {
            return null;
        }
        return @event;
    });
});

builder.Services.AddMudServices();
builder.Services.AddMudExtensions();

builder.Logging.AddSentry(o => o.InitializeSdk = false);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<CookieService>();
builder.Services.AddSingleton<IAuthService, FirebaseAuthService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddSingleton<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddSingleton<LoadingService>();
builder.Services.AddScoped<NotificationService>();

builder.Services.AddHttpClient(BACK_END_API_NAME, async (serviceProvider, client) =>
{
    client.BaseAddress = new Uri("https://localhost:7016/");
    
    var cookieService = serviceProvider.GetRequiredService<CookieService>();
    var token = await cookieService.GetCookie(FIREBASE_AUTH_TOKEN);
    
    if (!string.IsNullOrEmpty(token))
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
});

await builder.Build().RunAsync();
