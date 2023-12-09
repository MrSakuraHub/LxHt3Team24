using System.Data.Entity;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using API.Database;
using API.Database.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace API.Endpoints;

public static class AuthEndpoint
{
    public static void MapAuth(this RouteGroupBuilder app)
    {
        app.MapPost("/signup", async (ApplicationDbContext dbContext, AdminEntity admin) =>
        {
            admin.PasswordHash = SecurePasswordHasher.Hash(admin.PasswordHash);
            dbContext.Admins.Add(admin);
            await dbContext.SaveChangesAsync();
            return Results.Ok();
        });
        app.MapPost("/login", async (JSObject loginData, ApplicationDbContext dbContext, HttpContext httpContext) =>
        {
            var loginDataPassword = loginData.GetPropertyAsString("password");

            var loginDataLogin = loginData.GetPropertyAsString("login");

            var admin = await dbContext.Admins.FirstOrDefaultAsync(a =>
                a.AdminLogin == loginDataLogin &&
                SecurePasswordHasher.Verify(loginDataPassword!, a.PasswordHash!));
            if (admin is null) return Results.Unauthorized();

            var claims = new List<Claim> { new(ClaimTypes.Name, admin.AdminLogin!) };
            
            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
            
            return Results.Ok();
        });
        app.MapGet("/logout", async (HttpContext context) =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Ok();
        });
    }
}