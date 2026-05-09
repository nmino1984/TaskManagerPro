using FluentValidation;
using MyApp.Application.DTOs.MyTask;

public class MyTaskCreateDtoValidator : AbstractValidator<MyTaskCreateDto>
{
    public MyTaskCreateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
            .When(x => x.Description is not null);

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("EndDate must be later than StartDate.");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid priority value.");
    }
}
