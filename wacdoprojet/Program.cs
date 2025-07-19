using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using wacdoproject.Services;
using wacdoprojet.Data;
using wacdoprojet.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>() // 🔑 Ajout pour activer user-secrets
    .AddEnvironmentVariables();


// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(connectionString: connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
//   .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddDefaultIdentity<Collaborateur>(options => options.SignIn.RequireConfirmedAccount = true)
     .AddRoles<IdentityRole>() // ⬅️ Nécessaire pour activer les rôles
     .AddEntityFrameworkStores<ApplicationDbContext>();



builder.Services.AddControllersWithViews();
// ajout le 01062025
builder.Services.AddRazorPages();
builder.Services.AddTransient<IEmailSender, EmailSender>();

//Console.WriteLine("API Key test depuis Program.cs: " + builder.Configuration["SendGrid:ApiKey"]);
var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbInitializer.SeedRolesAsync(services);
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();
// DEBUT ne pas permmtre l'acc�s � la fonction Register
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/Identity/Account/Register"))
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("Page introuvable.");
        return;
    }
    await next();
});
// FIN ne pas permettre l'acc�s � la fonction Register
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
