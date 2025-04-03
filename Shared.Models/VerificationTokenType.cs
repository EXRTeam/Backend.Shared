namespace Shared.Models;

public enum VerificationTokenType : byte {
    EmailConfirmation, AccountDeletionConfirmation, NewPasswordConfirmation, RestoreAccountConfirmation
}