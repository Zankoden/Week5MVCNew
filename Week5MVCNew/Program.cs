using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Week5MVCNew.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Week5MVCNewContextConnection") ?? throw new InvalidOperationException("Connection string 'Week5MVCNewContextConnection' not found.");

builder.Services.AddDbContext<Week5MVCNewContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<Week5MVCNewContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication().AddFacebook(opt =>
{
    opt.ClientId = "261911960326493";
    opt.ClientSecret = "cb98d18ae1fa79210c502afbb11ee4d8";
});

//var config = new ConfigurationBuilder()
//    .SetBasePath(Directory.GetCurrentDirectory)
//    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//    .Build;
//builder.Services.AddAuthentication().AddGoogle(googleOptions =>
//{
//    googleOptions ClientId = config("GoogleKeys:567556392791-ef447ciehpoihir78a1nlp4kfm85pc2r.apps.googleusercontent.com");
//    googleOptions ClientSecret = config("GoogleKeys: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");
//});

// for google login
//var config = new ConfigurationBuilder()
//    .SetBasePath(Directory.GetCurrentDirectory())
//    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//    .Build();
//builder.Services.AddAuthentication().AddGoogle(googleOptions =>
//{
//googleOptions.ClientId = config["GoogleKeys:567556392791-ef447ciehpoihir78a1nlp4kfm85pc2r.apps.googleusercontent.com"];
//    googleOptions.ClientSecret = config["GoogleKeys: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"];
//});

// for google login


//var config = new ConfigurationBuilder()
//    .SetBasePath(Directory.GetCurrentDirectory())
//    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//    .Build();
//builder.Services.AddAuthentication().AddGoogle(googleOptions =>
//{
//    googleOptions.ClientId = config["GoogleKeys:567556392791-ef447ciehpoihir78a1nlp4kfm85pc2r.apps.googleusercontent.com"];
//    googleOptions.ClientSecret = config["GoogleKeys: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"];
//});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
//default user role setting
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Admin", "Manager" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string email = "admin@admin.com";
    string password = "Test@1234";
    if (await userManager.FindByEmailAsync(email) == null)
    {
        var user = new IdentityUser();
        user.UserName = email;
        user.Email = email;
        user.EmailConfirmed = true;
        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                // Log or handle each error
                Console.WriteLine(error.Description);
            }
        }

    }

}


app.Run();
