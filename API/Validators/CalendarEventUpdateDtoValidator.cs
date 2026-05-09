using FluentValidation;
using MyApp.Application.DTOs.CalendarEvent;

public class CalendarEventUpdateDtoValidator : AbstractValidator<CalendarEventUpdateDto>
{
    public CalendarEventUpdateDtoValidator()
    {
        RuleFor(x => x.Date)
            .NotEqual(default(DateTime)).WithMessage("Date is required.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid status value.");
    }
}
