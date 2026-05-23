using MediatR;

namespace Catalog.API.Application.Commands;

public record DeleteProductCommand(Guid Id) : IRequest<bool>;
