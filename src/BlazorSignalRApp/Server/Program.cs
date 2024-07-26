using Microsoft.AspNetCore.ResponseCompression;
using BlazorSignalRApp.Server.Data;
using Microsoft.EntityFrameworkCore;
using BlazorSignalRApp.Server.Hubs;
using BlazorSignalRApp.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// TODO: add required configuration for SignalR
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
	opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
		  new[] { "application/octet-stream" });
});

// In-memory database configuration. Do not change!
builder.Services.AddDbContext<AppDataContext>(options =>
    options.UseInMemoryDatabase("appdata"));


var app = builder.Build();

// TODO: Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseResponseCompression();
app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();

// TODO: add required configuration for SignalR hubs
app.MapHub<ChatterHub>("/chathub24");
app.MapHub<WeatherObservationHub>("/weatherobservations24");

app.MapFallbackToFile("index.html");

app.Run();

// The Program class declaration below is needed for the automated tests. 
// DO NOT remove the following line!!!
public partial class Program { }
