using FluentValidation;
using MyApp.Application.DTOs.CalendarEvent;

public class CalendarEventCreateDtoValidator : AbstractValidator<CalendarEventCreateDto>
{
    public CalendarEventCreateDtoValidator()
    {
        RuleFor(x => x.TaskId)
            .GreaterThan(0).WithMessage("TaskId must reference a valid task.");

        RuleFor(x => x.Date)
            .NotEqual(default(DateTime)).WithMessage("Date is required.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid status value.");
    }
}
