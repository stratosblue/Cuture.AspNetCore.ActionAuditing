using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cuture.AspNetCore.ActionAuditing.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Cuture.AspNetCore.ActionAuditing.Test;

/// <summary>
/// <see cref="AsyncAuditDataStorage{TData}"/> 的单元测试
/// </summary>
[TestClass]
public class AsyncAuditDataStorageTest
{
    #region 测试辅助类

    /// <summary>
    /// 用于测试的AsyncAuditDataStorage实现
    /// </summary>
    public class TestAsyncAuditDataStorage : AsyncAuditDataStorage<string>
    {
        public TestAsyncAuditDataStorage(ILogger<AsyncAuditDataStorage<string>> logger) : base(logger)
        {
        }

        protected override ValueTask<string> CreateDataAsync(ActionAuditingExecutingContext context, CancellationToken cancellationToken)
        {
            return ValueTask.FromResult("testData");
        }

        protected internal override Task SaveDataAsync(string data, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    #endregion

    #region 测试方法

    /// <summary>
    /// 测试构造函数是否正确初始化
    /// </summary>
    [TestMethod]
    public void Constructor_ShouldInitializeCorrectly()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<AsyncAuditDataStorage<string>>>();

        // Act
        var storage = new TestAsyncAuditDataStorage(mockLogger.Object);

        // Assert
        Assert.IsNotNull(storage.DataChannel);
        Assert.IsFalse(storage.RunningCancellationToken.IsCancellationRequested);
    }

    /// <summary>
    /// 测试AddAsync方法是否正确写入数据到通道
    /// </summary>
    [TestMethod]
    public async Task AddAsync_ShouldWriteToChannel()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<AsyncAuditDataStorage<string>>>();
        var storage = new TestAsyncAuditDataStorage(mockLogger.Object);
        var mockContext = new Mock<ActionAuditingExecutingContext>(null, null, null, null);

        // Act
        await storage.AddAsync(mockContext.Object, CancellationToken.None);

        // Assert
        var result = await storage.DataChannel.Reader.ReadAsync();
        Assert.AreEqual("testData", result);
    }

    /// <summary>
    /// 测试Dispose方法是否取消运行令牌
    /// </summary>
    [TestMethod]
    public void Dispose_ShouldCancelRunningToken()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<AsyncAuditDataStorage<string>>>();
        var storage = new TestAsyncAuditDataStorage(mockLogger.Object);

        // Act
        storage.Dispose();

        // Assert
        Assert.IsTrue(storage.RunningCancellationToken.IsCancellationRequested);
    }

    /// <summary>
    /// 测试当SaveDataAsync抛出异常时是否记录日志
    /// </summary>
    [TestMethod]
    public async Task RunSaveLoopAsync_ShouldLogError_WhenSaveFails()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<AsyncAuditDataStorage<string>>>();
        var storage = new Mock<TestAsyncAuditDataStorage>(mockLogger.Object) { CallBase = true };
        storage.Setup(x => x.SaveDataAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
               .ThrowsAsync(new Exception("Test exception"));

        // Act
        await storage.Object.DataChannel.Writer.WriteAsync("testData");
        await Task.Delay(100); // 给后台线程时间处理

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Audit data save failed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    /// <summary>
    /// 测试当运行循环被取消时是否记录日志
    /// </summary>
    [TestMethod]
    public async Task RunSaveLoopAsync_ShouldLogWarning_WhenCancelled()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<AsyncAuditDataStorage<string>>>();
        var storage = new TestAsyncAuditDataStorage(mockLogger.Object);

        // Act
        storage.Dispose();
        await Task.Delay(100); // 给后台线程时间处理

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Asynchronous audit data storage has stopped working")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    /// <summary>
    /// 测试Dispose方法是否处理多次调用
    /// </summary>
    [TestMethod]
    public void Dispose_ShouldHandleMultipleCalls()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<AsyncAuditDataStorage<string>>>();
        var storage = new TestAsyncAuditDataStorage(mockLogger.Object);

        // Act
        storage.Dispose();
        storage.Dispose(); // 第二次调用

        // Assert
        Assert.IsTrue(storage.RunningCancellationToken.IsCancellationRequested);
    }

    #endregion
}
