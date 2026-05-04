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
            .Must(p => new[] { "High", "Medium", "Low" }.Contains(p))
            .WithMessage("Invalid priority value. Allowed: High, Medium, Low.");

        RuleFor(t => t.Status)
            .Must(s => new[] { "NotStarted", "InProgress", "Completed", "Overdue" }.Contains(s))
            .WithMessage("Invalid status value.");

        RuleFor(t => t.Progress)
            .InclusiveBetween(0, 100)
            .WithMessage("Progress must be between 0 and 100.");
    }
}
