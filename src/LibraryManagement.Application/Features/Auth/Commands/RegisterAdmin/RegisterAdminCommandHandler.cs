using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Domain.Constants;
using LibraryManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Features.Auth.Commands.RegisterAdmin;

public class RegisterAdminCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterAdminCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterAdminResponse> Handle(RegisterAdminCommand request, CancellationToken cancellationToken)
    {
        // Checking if user already exists
        var existingUser = await _userRepository.ExistsAsync(request.Email, cancellationToken);
        if (existingUser)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        // Hashing password
        var passwordHash = _passwordHasher.Hash(request.Password);

        // Creating admin user
        var user = User.Create(
            request.Email,
            passwordHash,
            request.FirstName,
            request.LastName,
            Roles.Admin  // Setting role as Admin
        );

        await _userRepository.AddAsync(user, cancellationToken);

        return new RegisterAdminResponse(
            user.Id,
            user.Email,
            user.FullName,
            user.Role
        );
    }
}
