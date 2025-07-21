using Cuture.AspNetCore.ActionAuditing;
using Cuture.AspNetCore.ActionAuditing.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace SampleWebApp.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(IAuditValueStore auditValueStore) : ControllerBase
{
    private static readonly string[] s_summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    [HttpGet]
    [PermissionRequired("ReadPermission")]
    [AuditDescription("Get WeatherForecast with page: {page}, pageSize: {pageSize}. Returned: {result.Length}.")]
    public IEnumerable<WeatherForecast> Get(int page, int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

        if (pageSize > 20)
        {
            pageSize = 20;
        }

        var result = Enumerable.Range(page, pageSize).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = s_summaries[Random.Shared.Next(s_summaries.Length)]
        })
        .ToArray();

        auditValueStore.SetValue(result);

        return result;
    }

    [HttpPost]
    [PermissionRequired("WritePermission")]
    public WeatherForecast Set(WeatherForecast weatherForecast)
    {
        //business logic

        return weatherForecast;
    }
}
