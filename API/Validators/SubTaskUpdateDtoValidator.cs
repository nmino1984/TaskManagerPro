using FluentValidation;
using MyApp.Application.DTOs.SubTask;

public class SubTaskUpdateDtoValidator : AbstractValidator<SubTaskUpdateDto>
{
    public SubTaskUpdateDtoValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid status value.");
    }
}
