using FluentValidation;
using TestSwaggerAPI.Models;

namespace TestSwaggerAPI.Validators;

public class DateRequestValidator : AbstractValidator<DateRequest>
{
    public DateRequestValidator()
    {
        RuleFor(x => x.Year)
            .InclusiveBetween(1900, 2100)
            .WithMessage(x => $"Год не должен быть меньше 1900 и больше 2100 на момент {DateTime.Now}");

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12)
            .WithMessage(x => $"Месяц не может быть меньше 1 и больше 12 на момент {DateTime.Now}");
    }
}