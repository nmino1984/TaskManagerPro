using System;
using TaskManagerPro.Application.DTOs;
using FluentValidation;
using TaskManagerPro.Application.DTOs.Milestone;

namespace TaskManagerPro.Application.Validators;

public class MilestoneCreateDtoValidator : AbstractValidator<MilestoneCreateDto>
{
    public MilestoneCreateDtoValidator()
    {
        RuleFor(x => x.TaskId)
            .GreaterThan(0).WithMessage("TaskId must be valid.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(5).WithMessage("Description must be at least 5 characters.")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.TargetDate)
            .GreaterThanOrEqualTo(x => DateTime.UtcNow.Date)
            .WithMessage("Target date must be today or later.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be valid (Pending, Completed, Overdue).");
    }
}

public class MilestoneUpdateDtoValidator : AbstractValidator<MilestoneUpdateDto>
{
    public MilestoneUpdateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(5).WithMessage("Description must be at least 5 characters.")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.TargetDate)
            .GreaterThanOrEqualTo(x => DateTime.UtcNow.Date)
            .WithMessage("Target date must be today or later.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be valid (Pending, Completed, Overdue).");
    }
}
