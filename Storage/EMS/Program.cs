using Azure.Storage.Blobs;
using EMS.Data;
using EMS.Models;
using EMS.Services.Implementations;
using EMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<AzureBlobSettings>(builder.Configuration.GetSection(AzureBlobSettings.SectionName));

builder.Services.AddSingleton(sp => 
{
    IOptions<AzureBlobSettings> options = sp.GetRequiredService<IOptions<AzureBlobSettings>>(); 
    return new BlobServiceClient(options.Value.ConnectionString); 
});

builder.Services.AddScoped<IBlobService, BlobService>();
builder.Services.AddScoped<IEmployeeDocumentService, EmployeeDocumentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
