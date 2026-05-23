using Catalog.API.Models;
using MediatR;

namespace Catalog.API.Application.Queries;

public record GetProductsQuery : IRequest<IReadOnlyCollection<Product>>;
