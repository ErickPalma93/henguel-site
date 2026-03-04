using HenguelSistemas.Application.Interfaces;
using HenguelSistemas.Application.Services;
using HenguelSistemas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();

// ── HttpClient para chamadas internas (Blazor Server não injeta HttpClient automaticamente)
builder.Services.AddHttpClient("Internal", client =>
{
    var baseUrl = builder.Configuration["AppBaseUrl"] ?? "https://localhost:7000/";
    client.BaseAddress = new Uri(baseUrl);
});

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? "Server=localhost;Port=3306;Database=henguel;User ID=root;Password=;";

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0))));

builder.Services.AddScoped<IWaitlistService, WaitlistService>();
builder.Services.AddScoped<VisitTrackingService>();

builder.Services.AddResponseCompression(opts =>
    opts.EnableForHttps = true);

builder.WebHost.UseUrls("http://0.0.0.0:8080");

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseResponseCompression();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<HenguelSistemas.Web.Components.App>()
   .AddInteractiveServerRenderMode();

app.MapPost("/api/waitlist/register", async (IWaitlistService svc, RegisterRequest req) =>
{
    var (success, message) = await svc.RegisterAsync(req.Name, req.Email, req.Consent);
    return success ? Results.Ok(new { message }) : Results.BadRequest(new { message });
});

// Rota usada pelo Home.razor
app.MapPost("/api/waitlist", async (IWaitlistService svc, RegisterRequest req) =>
{
    var (success, message) = await svc.RegisterAsync(req.Name, req.Email, req.Consent);
    return success ? Results.Ok(new { message }) : Results.BadRequest(new { message });
});

app.MapGet("/api/waitlist/count", async (IWaitlistService svc) =>
    Results.Ok(await svc.GetCountAsync()));

app.MapPost("/api/visits/track", async (VisitTrackingService svc, HttpContext ctx, TrackRequest req) =>
{
    var ip = ctx.Connection.RemoteIpAddress?.ToString();
    var ua = ctx.Request.Headers.UserAgent.ToString();
    await svc.TrackAsync(req.Page, ip, ua);
    return Results.Ok();
});

app.MapGet("/api/visits/count", async (VisitTrackingService svc) =>
    Results.Ok(await svc.GetTotalVisitsAsync()));

app.Run();

public record RegisterRequest(string Name, string Email, bool Consent);
public record TrackRequest(string Page);