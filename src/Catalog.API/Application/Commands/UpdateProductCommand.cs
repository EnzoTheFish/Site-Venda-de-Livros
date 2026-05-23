using Catalog.API.Models;
using MediatR;

namespace Catalog.API.Application.Commands;

public record UpdateProductCommand(Guid Id, Product Product) : IRequest<bool>;
