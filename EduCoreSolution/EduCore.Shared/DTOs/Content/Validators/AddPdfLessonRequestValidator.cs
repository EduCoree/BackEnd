using FluentValidation;
using System;

namespace EduCore.Shared.DTOs.Content.Validators
{
    public class AddPdfLessonRequestValidator : AbstractValidator<AddPdfLessonRequest>
    {
        public AddPdfLessonRequestValidator()
        {
            RuleFor(x => x.FileUrl)
                .NotEmpty().WithMessage("File URL is required.")
                .MaximumLength(255).WithMessage("File URL must not exceed 255 characters.")
                .Must(BeAValidUrl).WithMessage("File URL must be a valid URL.");

            RuleFor(x => x.FileSizeKb)
                .GreaterThan(0).When(x => x.FileSizeKb.HasValue)
                .WithMessage("File size must be a positive number.");
        }

        private static bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var result)
                && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }
    }
}
