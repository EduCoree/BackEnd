using FluentValidation;

namespace EduCore.Shared.DTOs.Content.Validators
{
    public class UpdateLessonRequestValidator : AbstractValidator<UpdateLessonRequest>
    {
        public UpdateLessonRequestValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(160).When(x => x.Title != null)
                .WithMessage("Title must not exceed 160 characters.");

            RuleFor(x => x.DurationSeconds)
                .GreaterThan(0).When(x => x.DurationSeconds.HasValue)
                .WithMessage("Duration must be a positive number.");

            RuleFor(x => x.SortOrder)
                .GreaterThanOrEqualTo(0).When(x => x.SortOrder.HasValue)
                .WithMessage("Sort order must be zero or positive.");
        }
    }
}
