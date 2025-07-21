namespace SampleFullAuditWebApp.Auditing.EntityFramework;

internal enum DbContextOperationLogType
{
    StartSaveChanges,
    Error,
    SaveChangesEnd,
    Add,
    Modify,
    Delete,
}
