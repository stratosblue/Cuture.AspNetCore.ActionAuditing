using Cuture.AspNetCore.ActionAuditing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SampleFullAuditWebApp;
using SampleFullAuditWebApp.Auditing;
using SampleFullAuditWebApp.EntityFramework;

var builder = WebApplication.CreateBuilder(args);

//AddActionAuditing
builder.Services.AddControllers()
                .AddActionAuditing(options =>
                {
                    options.UsePermissionAuditor<UserPermissionAuditor>();
                    options.UseStorage<SystemAuditDataStorage>();
                });

builder.Services.AddOpenApi();

#region test db

var sqliteConection = new SqliteConnection("DataSource=:memory:");
sqliteConection.Open();
builder.Services.AddDbContextPool<DataDbContext>(options =>
{
    options.UseSqlite(sqliteConection);
});

#endregion test db

var app = builder.Build();

#region test permission data

{
    const long UserId = 1234;
    await using var serviceScope = app.Services.CreateAsyncScope();
    using var dbContext = serviceScope.ServiceProvider.GetRequiredService<DataDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
    foreach (var (descriptor, _) in PermissionDefine.EnumerateItems())  //遍历加入所有权限
    {
        dbContext.UserPermissions.Add(new UserPermission(0, UserId, descriptor.Value));
    }
    await dbContext.SaveChangesAsync(default);
}

#endregion test permission data

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapSwaggerUI();
}

var global = app.MapGroup(string.Empty)
                .WithActionAuditing();

global.MapGet("/Hello", [FeatureName("Hello")][PermissionRequired(PermissionDefine.SayHelloConstant)][AuditDescription("SayHello")] () => "World");

app.UseAuthorization();

app.MapControllers();

app.Run();
