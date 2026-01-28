using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Features.Auth.Commands.Login;

public record LoginCommand(
string Email,
string Password
) : IRequest<LoginResponse>;

public record LoginResponse(
    string Token,
    Guid UserId,
    string Email,
    string FullName,
    DateTime ExpiresAt
);

