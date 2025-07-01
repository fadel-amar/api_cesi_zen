using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using CesiZen_API.Services;
using CesiZen_API.Services.Interfaces;
using CesiZen_API.Data;
using CesiZen_API.Middleware;
using Microsoft.AspNetCore.Mvc;
using CesiZen_API.ModelBlinders;
using Microsoft.Extensions.Options;


public class Program
{
    public static void Main(string[] args)
    {
        DotNetEnv.Env.Load();


        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();

        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(
                connectionString,
                new MySqlServerVersion(new Version(8, 0, 3)),
                mySqlOptions => mySqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 2,                  // nombre max de retries
                    maxRetryDelay: TimeSpan.FromSeconds(3),  // délai max entre retries
                    errorNumbersToAdd: null             // liste des codes d’erreur supplémentaires
                )
            )
        );
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IMenuService, MenuService>();
        builder.Services.AddScoped<IPageService, PageService>();
        builder.Services.AddScoped<IActivityService, ActivityService>();
        builder.Services.AddScoped<AuthService>();


        var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
        if (string.IsNullOrEmpty(jwtKey))
            throw new InvalidOperationException("La variable d'environnement JWT_KEY est manquante.");

        var key = Encoding.UTF8.GetBytes(jwtKey);
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        return context.Response.WriteAsync("{\"message\":\"Authentification requise ou token invalide\"}");
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/json";
                        return context.Response.WriteAsync("{\"message\":\"Accès interdit : rôle administrateur requis\"}");
                    }
                };
            });
        //Dépendance ciruclaire sérialisation
        /*builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                options.JsonSerializerOptions.WriteIndented = true;
            });
        */
        builder.Services.AddAuthorization();
        builder.Services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return new BadRequestObjectResult(new
                    {
                        status = 400,
                        message = "Erreur de validation",
                        errors = errors
                    });
                };
            });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins(
                    "http://localhost:5173",
                    "http://localhost:4173"
                )
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
        });


        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseCors("AllowFrontend");
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<AppDbContext>();
            FakerData.SeedAllData(context);
        }
        app.UseStaticFiles();

        app.Run();
    }
}

