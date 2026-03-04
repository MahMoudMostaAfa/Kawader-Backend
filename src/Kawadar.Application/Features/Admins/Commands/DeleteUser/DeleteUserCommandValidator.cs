
using FluentValidation;

namespace Kawadar.Application.Features.Admins.Commands.DeleteUser
{
    public class DeleteUserCommandValidator: AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserCommandValidator()
        {
            RuleFor(x => x.userName).NotNull().WithMessage("UserName is required")
                .NotEmpty().WithMessage("UserName can't be empty");
        }
    }
}
