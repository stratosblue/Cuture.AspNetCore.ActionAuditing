using System.Threading.Channels;
using Cuture.AspNetCore.ActionAuditing.Abstractions;
using Microsoft.Extensions.Logging;

namespace Cuture.AspNetCore.ActionAuditing;

/// <summary>
/// 异步的 <see cref="IAuditDataStorage"/>
/// <br/>由单独的线程异步处理存储数据
/// </summary>
public abstract class AsyncAuditDataStorage<TData> : IAuditDataStorage, IDisposable
{
    #region Private 字段

    private readonly CancellationTokenSource _runningCTS = new();

    private bool _isDisposed;

    #endregion Private 字段

    #region Protected 属性

    /// <summary>
    /// 数据通道
    /// </summary>
    protected internal Channel<TData> DataChannel { get; } = Channel.CreateUnbounded<TData>();

    /// <inheritdoc cref="ILogger"/>
    protected internal ILogger Logger { get; }

    /// <summary>
    /// 运行的令牌
    /// </summary>
    protected internal CancellationToken RunningCancellationToken { get; }

    #endregion Protected 属性

    #region Public 构造函数

    /// <inheritdoc cref="AsyncAuditDataStorage{TData}"/>
    public AsyncAuditDataStorage(ILogger<AsyncAuditDataStorage<TData>> logger)
    {
        Logger = logger;
        RunningCancellationToken = _runningCTS.Token;
        _ = RunSaveLoopAsync();
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public async Task AddAsync(ActionAuditingExecutingContext context, CancellationToken cancellationToken)
    {
        var data = await CreateDataAsync(context, cancellationToken);
        await DataChannel.Writer.WriteAsync(data, CancellationToken.None);
    }

    #endregion Public 方法

    #region Private 方法

    private async Task RunSaveLoopAsync()
    {
        try
        {
            await foreach (var data in DataChannel.Reader.ReadAllAsync(RunningCancellationToken))
            {
                try
                {
                    await SaveDataAsync(data, RunningCancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, "Audit data save failed: {Data}", data);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Asynchronous audit data storage has stopped working");
        }
    }

    #endregion Private 方法

    #region Protected 方法

    /// <summary>
    /// 存储数据
    /// </summary>
    /// <param name="data">要保存的数据</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected internal abstract Task SaveDataAsync(TData data, CancellationToken cancellationToken);

    /// <summary>
    /// 从审计信息中创建最终用于保存的数据
    /// </summary>
    /// <param name="context">审计执行上下文</param>
    /// <param name="cancellationToken">当次请求的取消令牌</param>
    /// <returns></returns>
    protected abstract ValueTask<TData> CreateDataAsync(ActionAuditingExecutingContext context, CancellationToken cancellationToken);

    #endregion Protected 方法

    #region IDisposable

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="Dispose()"/>
    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            try
            {
                _runningCTS.Cancel();
            }
            catch
            {
                if (disposing)
                {
                    throw;
                }
            }
            _runningCTS.Dispose();

            _isDisposed = true;
        }
    }

    #endregion IDisposable
}
