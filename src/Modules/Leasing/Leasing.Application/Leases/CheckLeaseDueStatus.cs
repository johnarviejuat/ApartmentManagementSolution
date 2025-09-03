

using Leasing.Application.Common;
using Leasing.Domain.Abstraction;
using MediatR;

namespace Leasing.Application.Leases;

public sealed record CheckLeaseDueStatusQuery(Guid TenantId, Guid ApartmentId) : IRequest<LeaseDueStatusDto?>;

internal sealed class CheckLeaseDueStatusHandler(
    ILeaseRepository leaseRepo
) : IRequestHandler<CheckLeaseDueStatusQuery, LeaseDueStatusDto?>
{
    private readonly ILeaseRepository _leaseRepo = leaseRepo;

    public async Task<LeaseDueStatusDto?> Handle(CheckLeaseDueStatusQuery q, CancellationToken ct)
    {
        var lease = await _leaseRepo.GetActiveAsync(q.TenantId, q.ApartmentId, ct);
        if (lease is null) return null;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var isOverdue = today > lease.NextDueDate;
        var isDue = today >= lease.NextDueDate;

        var daysOverdue = isOverdue
            ? (today.ToDateTime(TimeOnly.MinValue) - lease.NextDueDate.ToDateTime(TimeOnly.MinValue)).Days
            : 0;

        var message = isOverdue
            ? $"Overdue by {daysOverdue} day(s). Next due date was {lease.NextDueDate:yyyy-MM-dd}."
            : isDue
                ? $"Due today ({lease.NextDueDate:yyyy-MM-dd})."
                : $"Not due. Next due date is {lease.NextDueDate:yyyy-MM-dd}.";

        return new LeaseDueStatusDto(
            TenantId: q.TenantId,
            ApartmentId: q.ApartmentId,
            NextDueDate: lease.NextDueDate,
            TodayUtc: today,
            IsDue: isDue,
            IsOverdue: isOverdue,
            DaysOverdue: daysOverdue,
            Message: message
        );
    }
}
