using FluentValidation;

namespace EduCore.Shared.DTOs.Content.Validators
{
    public class CreateLessonRequestValidator : AbstractValidator<CreateLessonRequest>
    {
        public CreateLessonRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(160).WithMessage("Title must not exceed 160 characters.");

            RuleFor(x => x.SectionId)
                .GreaterThan(0).WithMessage("A valid SectionId is required.");

            RuleFor(x => x.DurationSeconds)
                .GreaterThan(0).When(x => x.DurationSeconds.HasValue)
                .WithMessage("Duration must be a positive number.");

            RuleFor(x => x.SortOrder)
                .GreaterThanOrEqualTo(0).When(x => x.SortOrder.HasValue)
                .WithMessage("Sort order must be zero or positive.");
        }
    }
}
