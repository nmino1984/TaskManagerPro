using FluentValidation;
using MyApp.Application.DTOs.SubTask;

namespace MyApp.Application.Validators;

public class SubTaskCreateDtoValidator : AbstractValidator<SubTaskCreateDto>
{
    public SubTaskCreateDtoValidator()
    {
        RuleFor(x => x.TaskId)
            .GreaterThan(0).WithMessage("TaskId must be valid.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(5).WithMessage("Description must be at least 5 characters.")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be valid (Pending, Completed).");

    }
}

public class SubTaskUpdateDtoValidator : AbstractValidator<SubTaskUpdateDto>
{
    public SubTaskUpdateDtoValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(5).WithMessage("Description must be at least 5 characters.")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be valid (Pending, Completed).");

    }
}
