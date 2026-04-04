using FluentValidation;
using System;

namespace EduCore.Shared.DTOs.Content.Validators
{
    public class UpdateRecordingRequestValidator : AbstractValidator<UpdateRecordingRequest>
    {
        public UpdateRecordingRequestValidator()
        {
            RuleFor(x => x.RecordingUrl)
                .NotEmpty().WithMessage("Recording URL is required.")
                .MaximumLength(255).WithMessage("Recording URL must not exceed 255 characters.")
                .Must(BeAValidUrl).WithMessage("Recording URL must be a valid URL.");
        }

        private static bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var result)
                && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }
    }
}
