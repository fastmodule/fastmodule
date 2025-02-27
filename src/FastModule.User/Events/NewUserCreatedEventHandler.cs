using FastModule.Shared.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastModule.User.Events;

public class NewUserCreatedEventHandler(ILogger<NewUserCreatedEventHandler> logger)
    : INotificationHandler<NewUserCreatedEvent>
{
    public async Task Handle(NewUserCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "New user created: {FullName} ({Email})",
            notification.FullName,
            notification.Email
        );
        // Todo: Add to database
        await Task.CompletedTask;
    }
}
