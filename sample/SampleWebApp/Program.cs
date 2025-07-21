using Cuture.AspNetCore.ActionAuditing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SampleWebApp;
using SampleWebApp.EntityFramework;

var builder = WebApplication.CreateBuilder(args);

//AddActionAuditing
builder.Services.AddControllers()
                .AddActionAuditing(options =>
                {
                    options.UsePermissionAuditor<UserPermissionAuditor>();
                });

builder.Services.AddOpenApi();

#region test db

var sqliteConection = new SqliteConnection("DataSource=:memory:");
sqliteConection.Open();
builder.Services.AddDbContext<DataDbContext>(options =>
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

global.MapGet("/Hello", [PermissionRequired("Hello")][AuditDescription("SayHello")] () => "World");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
