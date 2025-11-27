using E_Commerce.Application.DTOs.Orders;
using FluentValidation;

namespace E_Commerce.Application.Validators.Orders;

public class CheckoutRequestValidator : AbstractValidator<CheckoutRequest>
{
	public CheckoutRequestValidator()
	{
		RuleFor(x => x.PhoneNumber)
			.NotEmpty().WithMessage("Phone number is required")
			.Matches(@"^(\+?254|0)?[17]\d{8}$")
			.WithMessage("Invalid Kenyan phone number. Format: 07XXXXXXXX or 2547XXXXXXXX");
	}
}