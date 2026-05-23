using Catalog.API.Models;
using MediatR;

namespace Catalog.API.Application.Queries;

public record GetProductByIdQuery(Guid Id) : IRequest<Product?>;
