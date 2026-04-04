using FluentValidation;
using System;

namespace EduCore.Shared.DTOs.Content.Validators
{
    public class UpdateLiveSessionRequestValidator : AbstractValidator<UpdateLiveSessionRequest>
    {
        private static readonly string[] ValidProviders = { "zoom", "microsoftteams", "googlemeet", "jitsi" };

        public UpdateLiveSessionRequestValidator()
        {
            RuleFor(x => x.Provider)
                .Must(p => Array.Exists(ValidProviders, v => v.Equals(p!, StringComparison.OrdinalIgnoreCase)))
                .When(x => !string.IsNullOrEmpty(x.Provider))
                .WithMessage("Provider must be one of: Zoom, MicrosoftTeams, GoogleMeet, Jitsi.");

            RuleFor(x => x.MeetingUrl)
                .MaximumLength(255).When(x => x.MeetingUrl != null)
                .WithMessage("Meeting URL must not exceed 255 characters.");

            RuleFor(x => x.ScheduledAt)
                .GreaterThan(DateTime.UtcNow).When(x => x.ScheduledAt.HasValue)
                .WithMessage("Scheduled time must be in the future.");

            RuleFor(x => x.Title)
                .MaximumLength(200).When(x => x.Title != null)
                .WithMessage("Title must not exceed 200 characters.");
        }
    }
}
