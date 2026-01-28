using LibraryManagement.Application.Features.Auth.Commands.Login;
using LibraryManagement.Application.Features.Auth.Commands.Register;
using LibraryManagement.Application.Features.Auth.Queries.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponse>> Register(
            RegisterCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetCurrentUser), new { }, result);
        }

        /// <summary>
        /// Login and get JWT token
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(
            LoginCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get current authenticated user
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<CurrentUserResponse>> GetCurrentUser(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCurrentUserQuery(), cancellationToken);

            if (result is null)
            {
                return Unauthorized();
            }

            return Ok(result);
        }
    }
}
