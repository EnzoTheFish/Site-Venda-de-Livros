using Catalog.API.Models;
using MediatR;

namespace Catalog.API.Application.Commands;

public record CreateProductCommand(Product Product) : IRequest<Product>;
