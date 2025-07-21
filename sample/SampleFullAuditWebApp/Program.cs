using Cuture.AspNetCore.ActionAuditing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
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
    dbContext.UserPermissions.Add(new UserPermission(0, UserId, "ReadPermission"));
    dbContext.UserPermissions.Add(new UserPermission(0, UserId, "WritePermission"));
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

global.MapGet("/Hello", [FeatureName("Hello")][PermissionRequired("Hello")][AuditDescription("SayHello")] () => "World");

app.UseAuthorization();

app.MapControllers();

app.Run();
