# Cuture.AspNetCore.ActionAuditing

`AspNetCore`的审计框架。支持`Controller`和`MinimalAPI`。使权限和操作审核更容易。
A auditing framework for `AspNetCore`. Support `Controller` and `MinimalAPI`. Make permission and operation auditing easier.

## 1. Features

- 基于声明的权限审计
- 审计信息存储
- 支持`Controller`和`MinimalAPI`
- 可扩展动态验证

## 2. 快速使用

### 2.1 安装包
```shell
dotnet add package Cuture.AspNetCore.ActionAuditing --prerelease
```

### 2.2 仅权限验证
- 实现执行权限审核器 `IExecutingPermissionAuditor`
   ```C#
   class SamplePermissionAuditor : IExecutingPermissionAuditor
   {
       public async ValueTask<bool> AuditingAsync(ActionAuditingExecutingContext context, CancellationToken cancellationToken = default)
       {
           //进行权限验证，返回 true 认为权限验证通过
           return true;
       }
   }
   ```
 - 添加服务
   ```C#
   services.AddControllers()
           .AddActionAuditing(options =>
           {
               options.UsePermissionAuditor<SamplePermissionAuditor>();  //添加执行权限审核器
           });
   ```
 - 使用默认声明标记 `Controller` 或 `MinimalAPI`
   - 标记 `[PermissionRequired]` 用于声明权限要求
   - 标记 `[AuditDescription]` 用于描述审计信息
   - Controller
     ```C#
     [ApiController]
     [Route("[controller]")]
     public class WeatherForecastController : ControllerBase
     {
         [HttpGet]
         [PermissionRequired("ReadPermission")] //声明需要权限
         [AuditDescription("Get WeatherForecast.")] //声明审计描述, 支持格式化文本，插入方法参数等变量的值
         public IEnumerable<WeatherForecast> Get()
         {
         }
     }
     ```
   - MinimalAPI
     ```C#
     var global = app.MapGroup(string.Empty)
                     .WithActionAuditing();
     global.MapGet("/Hello", [PermissionRequired("Hello")][AuditDescription("SayHello")] () => "World");
     ```
至此完成最简配置，访问对应接口将会验证权限并打印审计信息

### 2.3 审计信息存储
 - 实现审计数据存储器 `IAuditDataStorage` ,可从 `AsyncAuditDataStorage<TData>` 派生以简化开发
   ```C#
   class StorageData
   {
       //自定义需要存储的数据属性
   }

   class SampleAuditDataStorage(ILogger<SampleAuditDataStorage> logger)
       : AsyncAuditDataStorage<StorageData>(logger)
   {
       protected override async ValueTask<StorageData> CreateDataAsync(ActionAuditingExecutingContext context, CancellationToken cancellationToken)
       {
           //从上下文中创建需要保存的数据并返回
       }
   
       protected override async Task SaveDataAsync(StorageData data, CancellationToken cancellationToken)
       {
           //保存数据
       }
   }
   ```
 - 添加服务
   ```C#
   services.AddControllers()
           .AddActionAuditing(options =>
           {
               options.UsePermissionAuditor<SamplePermissionAuditor>();  //设置执行权限审核器
               options.UseStorage<SampleAuditDataStorage>();  //设置审计数据存储器
           });
   ```
至此完成存储的配置，审计信息将会使用 `SampleAuditDataStorage` 进行存储

## 3. 示例
更多功能参照示例程序
- 基础的权限校验: [sample/SampleWebApp](sample/SampleWebApp/)
- 完整的权限校验及数据变动审计: [sample/SampleFullAuditWebApp](sample/SampleFullAuditWebApp/)
