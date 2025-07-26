using System.Resources;

namespace SampleFullAuditWebApp;

[GeneratedConstants]
public partial class PermissionDefine
{
    public int SayHello { get; set; }

    /// <summary>
    /// 管理
    /// </summary>
    public partial class Management
    {
        /// <summary>
        /// 查看审计日志
        /// </summary>
        public int ViewAuditingLogs { get; set; }
    }

    /// <summary>
    /// 示例业务
    /// </summary>
    public partial class SampleBusiness
    {
        /// <summary>
        /// 查看业务数据
        /// </summary>
        public int View { get; set; }

        /// <summary>
        /// 写入业务数据
        /// </summary>
        public int WriteBusinessData { get; set; }
    }


}
