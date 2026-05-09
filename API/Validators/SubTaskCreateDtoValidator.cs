using FluentValidation;
using MyApp.Application.DTOs.SubTask;

public class SubTaskCreateDtoValidator : AbstractValidator<SubTaskCreateDto>
{
    public SubTaskCreateDtoValidator()
    {
        RuleFor(x => x.TaskId)
            .GreaterThan(0).WithMessage("TaskId must reference a valid task.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid status value.");
    }
}
