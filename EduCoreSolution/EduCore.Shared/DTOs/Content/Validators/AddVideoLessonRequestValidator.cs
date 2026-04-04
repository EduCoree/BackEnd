using FluentValidation;
using System;

namespace EduCore.Shared.DTOs.Content.Validators
{
    public class AddVideoLessonRequestValidator : AbstractValidator<AddVideoLessonRequest>
    {
        private static readonly string[] ValidProviders = { "youtube", "vimeo", "self" };

        public AddVideoLessonRequestValidator()
        {
            RuleFor(x => x.VideoUrl)
                .NotEmpty().WithMessage("Video URL is required.")
                .MaximumLength(255).WithMessage("Video URL must not exceed 255 characters.")
                .Must(BeAValidUrl).WithMessage("Video URL must be a valid URL.");

            RuleFor(x => x.VideoProvider)
                .NotEmpty().WithMessage("Video provider is required.")
                .Must(p => Array.Exists(ValidProviders, v => v.Equals(p, StringComparison.OrdinalIgnoreCase)))
                .WithMessage("Video provider must be one of: youtube, vimeo, self.");

            RuleFor(x => x.ThumbnailUrl)
                .MaximumLength(255).When(x => x.ThumbnailUrl != null)
                .WithMessage("Thumbnail URL must not exceed 255 characters.");
        }

        private static bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var result)
                && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }
    }
}
