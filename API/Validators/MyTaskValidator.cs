using FluentValidation;
using MyApp.Domain.Entities;

public class MyTaskValidator : AbstractValidator<MyTask>
{
    public MyTaskValidator()
    {
        RuleFor(t => t.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100);

        RuleFor(t => t.Description)
            .MaximumLength(500);

        RuleFor(t => t.StartDate)
            .LessThan(t => t.EndDate)
            .WithMessage("StartDate must be earlier than EndDate.");

        RuleFor(t => t.Priority)
            .IsInEnum().WithMessage("Invalid priority value.");

        RuleFor(t => t.Status)
            .IsInEnum().WithMessage("Invalid status value.");

        RuleFor(t => t.Progress)
            .InclusiveBetween(0, 100)
            .WithMessage("Progress must be between 0 and 100.");
    }
}
