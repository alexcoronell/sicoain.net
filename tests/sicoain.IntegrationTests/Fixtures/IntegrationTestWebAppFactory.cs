using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using sicoain.shared.Constants;

namespace sicoain.IntegrationTests.Fixtures;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly string _testDbName = $"SICOAIN_Tests_{Guid.NewGuid():N}";
    private const string UserDb = "sa";
    private const string PasswordDb = "51c04in!2024";
    private readonly string _connectionString;

    public IntegrationTestWebAppFactory()
    {
        _connectionString = $"Server=localhost,1434;Database={_testDbName};User Id={UserDb};Password={PasswordDb};TrustServerCertificate=True;MultipleActiveResultSets=true";
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var testAssemblyPath = Path.GetDirectoryName(typeof(IntegrationTestWebAppFactory).Assembly.Location)!;
            var configPath = Path.Combine(testAssemblyPath, "appsettings.IntegrationTests.json");
            config.AddJsonFile(configPath, optional: false, reloadOnChange: false);
        });

        builder.ConfigureTestServices(services =>
        {
            // Override JWT validation key — Program.cs reads it from config before
            // our test config is applied, so we need to set it here explicitly.
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var testSecretKey = Encoding.UTF8.GetBytes("TestSecretKeyForIntegrationTestsThatIsAtLeast32CharsLong");
                options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(testSecretKey);
                options.TokenValidationParameters.ValidIssuer = "SICOAIN-API-Test";
                options.TokenValidationParameters.ValidAudience = "SICOAIN-Frontend-Test";
            });

            // Register authorization policies from AppPermissions constants.
            // Program.cs tries to load these from the DB but fails in tests because
            // the DB doesn't exist yet when Program.cs runs (timing issue).
            var permissionFields = typeof(AppPermissions)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(f => f.FieldType == typeof(string));
            foreach (var field in permissionFields)
            {
                var permName = field.GetValue(null)?.ToString();
                if (!string.IsNullOrEmpty(permName))
                {
                    services.AddAuthorizationBuilder()
                        .AddPolicy(permName, policy => policy.RequireClaim("Permission", permName));
                }
            }

            services.PostConfigure<AntiforgeryOptions>(options =>
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

            services.PostConfigure<MvcOptions>(options =>
            {
                var filter = options.Filters.FirstOrDefault(f => f is AutoValidateAntiforgeryTokenAttribute);
                if (filter != null)
                {
                    options.Filters.Remove(filter);
                }
            });

            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(_connectionString);
            });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Create and migrate the database BEFORE the host is built.
        // Program.cs runs seeders during host build that require an existing DB.
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(_connectionString);
        using var context = new ApplicationDbContext(optionsBuilder.Options);
        context.Database.EnsureDeleted();
        context.Database.Migrate();

        return base.CreateHost(builder);
    }

    public async Task InitializeAsync()
    {
        // Seed an admin user for integration tests
        using var scope = Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        var adminEmail = "admin@test.com";
        var adminPassword = "Admin123!";
        if (!await userManager.Users.AnyAsync(u => u.Email == adminEmail))
        {
            var adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Admin Test",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
        using var seedScope = Services.CreateScope();
        var seedContext = seedScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (!seedContext.RiskClasses.Any())
        {
            seedContext.RiskClasses.Add(new RiskClass { Name = "Clase I", Code = "I", ContributionRate = 0.005m, IsActive = true });
            await seedContext.SaveChangesAsync();
        }
    }

    public new async Task DisposeAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureDeletedAsync();
    }
}
