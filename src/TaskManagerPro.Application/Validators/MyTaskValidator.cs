using FluentValidation;
using TaskManagerPro.Application.DTOs.MyTask;

namespace TaskManagerPro.Application.Validators;

public class MyTaskCreateDtoValidator : AbstractValidator<MyTaskCreateDto>
{
    public MyTaskCreateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Priority must be valid.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be valid.");

        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .WithMessage("Start date must be before or equal to end date.");
    }
}

public class MyTaskUpdateDtoValidator : AbstractValidator<MyTaskUpdateDto>
{
    public MyTaskUpdateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Priority must be valid.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be valid.");

        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .WithMessage("Start date must be before or equal to end date.");
    }
}
