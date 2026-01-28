using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Features.Auth.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<CurrentUserResponse?>;

public record CurrentUserResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    string Role
);    
