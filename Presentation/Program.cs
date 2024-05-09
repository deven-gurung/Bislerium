using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Presentation;
using Presentation.Services;
using Presentation.Services.Base;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");

builder.RootComponents.Add<HeadOutlet>("head::after");

var apiUrl = AppConfiguration.BaseUrl;

builder.Services.AddScoped(x => new HttpClient
{
    BaseAddress = new Uri(apiUrl)
});

builder.Services.AddTransient<BaseService>();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

await builder.Build().RunAsync();