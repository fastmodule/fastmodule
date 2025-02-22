using FastModule.Core.Domain.Entities;

namespace FastModule.Domain.Entities.User;

public sealed class UserEntity : TimeStampEntity
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SubjectId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}

