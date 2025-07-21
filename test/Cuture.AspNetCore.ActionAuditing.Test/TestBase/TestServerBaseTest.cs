using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Cuture.AspNetCore.ActionAuditing.Test.TestBase;

public abstract class TestServerBaseTest
{
    #region Protected 字段

    protected IServiceProvider RootServiceProvider = null!;

    protected AsyncServiceScope ServiceScope;

    protected TestServer TestServer = null!;

    protected IAsyncDisposable WebApplication = null!;

    #endregion Protected 字段

    #region Protected 属性

    protected IServiceProvider ServiceProvider => ServiceScope.ServiceProvider;

    protected HttpClient TestClient => TestServer.CreateClient();

    #endregion Protected 属性

    #region Public 方法

    [TestCleanup]
    public async Task TestCleanupAsync()
    {
        await ServiceScope.DisposeAsync();
        await WebApplication.DisposeAsync();
    }

    [TestInitialize]
    public Task TestInitializeAsync()
    {
        var webApplicationFactory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseTestServer();
            builder.ConfigureTestServices(ConfigureServices);
        });

        WebApplication = webApplicationFactory;
        TestServer = webApplicationFactory.Server;

        RootServiceProvider = webApplicationFactory.Services;
        ServiceScope = RootServiceProvider.CreateAsyncScope();

        return Task.CompletedTask;
    }

    #endregion Public 方法

    #region Protected 方法

    protected virtual void ConfigureServices(IServiceCollection services)
    { }

    protected HttpClient GetTestHttpClient() => TestServer.CreateClient();

    #endregion Protected 方法
}
