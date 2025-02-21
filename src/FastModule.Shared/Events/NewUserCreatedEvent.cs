using MediatR;

namespace FastModule.Shared.Events;

public sealed class NewUserCreatedEvent : INotification
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SubjectId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}
