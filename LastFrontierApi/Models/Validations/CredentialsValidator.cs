﻿using FluentValidation;

namespace LastFrontierApi.Models.Validations
{
  public class CredentialsValidator : AbstractValidator<Credentials>
  {
    public CredentialsValidator()
    {
      RuleFor(vm => vm.UserName).NotEmpty().WithMessage("Username cannot be empty");
      RuleFor(vm => vm.Password).NotEmpty().WithMessage("Password cannot be empty");
      RuleFor(vm => vm.Password).Length(6, 40).WithMessage("Password must be between 6 and 40 characters");
    }
  }
}