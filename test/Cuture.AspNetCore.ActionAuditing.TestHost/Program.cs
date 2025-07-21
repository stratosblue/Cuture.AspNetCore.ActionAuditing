using Cuture.AspNetCore.ActionAuditing.TestHost;

var builder = WebApplication.CreateBuilder(args);

if (args.Length > 0
    && args[0] == "SelfHost")
{
    builder.Services.AddControllers()
                    .AddActionAuditing(options =>
                    {
                        options.UsePermissionAuditor<NoopExecutingPermissionAuditor>();
                    });
}
else
{
    builder.Services.AddControllers();
}

#if NET9_0_OR_GREATER
builder.Services.AddOpenApi();
#endif

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
#if NET9_0_OR_GREATER
    app.MapOpenApi();
    app.MapSwaggerUI();
#endif
}

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{ }
