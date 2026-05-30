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
            config.AddJsonFile("appsettings.IntegrationTests.json", optional: false, reloadOnChange: false);
        });

        builder.ConfigureTestServices(services =>
        {
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

    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureDeletedAsync();
    }
}
