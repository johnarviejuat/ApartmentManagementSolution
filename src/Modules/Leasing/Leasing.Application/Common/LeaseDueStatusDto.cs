

namespace Leasing.Application.Common
{
    public sealed record LeaseDueStatusDto(
        Guid TenantId,
        Guid ApartmentId,
        DateOnly NextDueDate,
        DateOnly TodayUtc,
        bool IsDue,
        bool IsOverdue,
        int DaysOverdue,
        string Message
    );
}
