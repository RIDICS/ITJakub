using FluentValidation;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Core.Validation
{
    public class SignUpValidator : AbstractValidator<CreateUserContract>
    {
        public SignUpValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.NewPassword).Length(6, 100);
        }
    }
}
