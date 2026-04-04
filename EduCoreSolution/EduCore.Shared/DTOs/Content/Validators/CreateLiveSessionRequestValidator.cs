using FluentValidation;
using System;

namespace EduCore.Shared.DTOs.Content.Validators
{
    public class CreateLiveSessionRequestValidator : AbstractValidator<CreateLiveSessionRequest>
    {
        private static readonly string[] ValidProviders = { "zoom", "microsoftteams", "googlemeet", "jitsi" };

        public CreateLiveSessionRequestValidator()
        {
            RuleFor(x => x.Provider)
                .NotEmpty().WithMessage("Provider is required.")
                .Must(p => Array.Exists(ValidProviders, v => v.Equals(p, StringComparison.OrdinalIgnoreCase)))
                .WithMessage("Provider must be one of: Zoom, MicrosoftTeams, GoogleMeet, Jitsi.");

            RuleFor(x => x.MeetingUrl)
                .NotEmpty().WithMessage("Meeting URL is required for this provider.")
                .When(x => !x.Provider.Equals("jitsi", StringComparison.OrdinalIgnoreCase))
                .MaximumLength(255).When(x => x.MeetingUrl != null)
                .WithMessage("Meeting URL must not exceed 255 characters.");

            RuleFor(x => x.ScheduledAt)
                .GreaterThan(DateTime.UtcNow).WithMessage("Scheduled time must be in the future.");

            RuleFor(x => x.Title)
                .MaximumLength(200).When(x => x.Title != null)
                .WithMessage("Title must not exceed 200 characters.");
        }
    }
}
