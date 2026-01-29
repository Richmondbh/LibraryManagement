using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Features.Auth.Commands.RegisterAdmin;

public record RegisterAdminCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName
) : IRequest<RegisterAdminResponse>;

public record RegisterAdminResponse(
    Guid UserId,
    string Email,
    string FullName,
    string Role
);

