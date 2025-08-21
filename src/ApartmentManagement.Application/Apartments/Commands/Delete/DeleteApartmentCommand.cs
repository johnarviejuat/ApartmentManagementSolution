using MediatR;

namespace ApartmentManagement.Application.Apartments.Commands.Delete;

public sealed record DeleteApartmentCommand(Guid Id) : IRequest<bool>;
