using Shared.Models;

namespace Shared.Messages.Emails.Commands;

public record SendVerificationMessageCommand(
    Guid UserId,
    string Email,
    string ConfirmationLink,
    VerificationTokenType Type,
    TimeSpan Lifetime);