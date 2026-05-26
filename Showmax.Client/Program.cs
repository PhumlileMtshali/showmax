using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Showmax.Client;
using Showmax.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Point to the API server
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5053/"),
    Timeout = TimeSpan.FromSeconds(30)
});

builder.Services.AddScoped<ApiService>();

await builder.Build().RunAsync();
