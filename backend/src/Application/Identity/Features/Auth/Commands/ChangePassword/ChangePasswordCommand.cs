using Application.Abstractions.Messaging;

namespace Application.Identity.Features.Auth.Commands.ChangePassword;

public sealed record ChangePasswordCommand(string CurrentPassword, string NewPassword) : ICommand;
