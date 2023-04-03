using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ProyectoERP.Data;
using ProyectoERP.Helpers;
using ProyectoERP.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddAuthentication(options => {
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie();
// Add services to the container.
builder.Services.AddSingleton<HelperAuth>();
builder.Services.AddSingleton<HelperMail>();
builder.Services.AddSingleton<HelperPathProvider>();
builder.Services.AddSingleton<HelperExcelToPdf>();
string connectionString =
    builder.Configuration.GetConnectionString("SqlERP");
builder.Services.AddTransient<IRepo, RepositoryERPSql>();
builder.Services.AddDbContext<ErpContext>(options => options.UseSqlServer(connectionString));
//builder.Services.AddControllersWithViews();
//INDICAMOS QUE UTILIZAMOS NUESTRAS PROPIAS RUTAS DE VALIDACION
builder.Services.AddControllersWithViews(options => options.EnableEndpointRouting = false).AddSessionStateTempDataProvider();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.UseMvc(routes =>
{
    routes.MapRoute(
        name: "default",
        template: "{controller=ClientesPotenciales}/{action=Index}/{id?}");
});

app.Run();
