using LibraryManagement.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Features.Auth.Queries.GetCurrentUser
{
    internal class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, CurrentUserResponse?>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;

        public GetCurrentUserQueryHandler(
            ICurrentUserService currentUserService,
            IUserRepository userRepository)
        {
            _currentUserService = currentUserService;
            _userRepository = userRepository;
        }

        public async Task<CurrentUserResponse?> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId is null)
            {
                return null;
            }

            var user = await _userRepository.GetByIdAsync(_currentUserService.UserId.Value, cancellationToken);

            if (user is null)
            {
                return null;
            }

            return new CurrentUserResponse(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.FullName,
                user.Role
            );
        }
    }
}
