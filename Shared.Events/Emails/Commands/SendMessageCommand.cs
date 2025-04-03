namespace Shared.Messages.Emails.Commands;

public record SendMessageCommand(string TargetEmail, string Message);