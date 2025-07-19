using AstApp.Components;
using AstApp.Servicios;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<OpenAIService>();
builder.Services.AddScoped<AstStorageService>();

builder.Services.AddSingleton<OpenAIService>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var apiKey = config["OpenAI:ApiKey"];
    var httpClient = provider.GetRequiredService<HttpClient>();
    return new OpenAIService(httpClient, apiKey);
});

builder.Services.AddServerSideBlazor()
    .AddHubOptions(options =>
    {
        options.MaximumReceiveMessageSize = 1024 * 100; // 100 KB
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();



