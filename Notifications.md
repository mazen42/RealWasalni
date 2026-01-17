## Notifications

```csharp
public class Notification
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int? TripId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

```csharp
public class NotificationHub : Hub
{
    public async Task SendNotificationToUser(string userId, string title, string message)
    {
        await Clients.User(userId).SendAsync("ReceiveNotification", new
        {
            Title = title,
            Message = message,
            Date = DateTime.Now
        });
    }
}

```

```csharp
public class NotificationService : INotificationService
{
    private readonly AppDbContext _context;
    private readonly IHubContext<NotificationHub> _hub;

    public NotificationService(AppDbContext context,
                               IHubContext<NotificationHub> hub)
    {
        _context = context;
        _hub = hub;
    }

    public async Task SendTripCompletionNotifications(int tripId)
    {
        var users = _context.TripPassengers
            .Where(tp => tp.TripId == tripId)
            .Select(tp => tp.UserId)
            .ToList();

        foreach (var userId in users)
        {
            var notification = new Notification
            {
                UserId = userId,
                TripId = tripId,
                Title = "Trip Completed 🎉",
                Message = "Your trip has been successfully completed!"
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            await _hub.Clients.User(userId).SendAsync("ReceiveNotification", new
            {
                notification.Title,
                notification.Message,
                notification.CreatedAt
            });
        }
    }
}
```

```csharp
public class TripCompletionJob
{
    private readonly AppDbContext _context;
    private readonly INotificationService _notificationService;

    public TripCompletionJob(AppDbContext context,
                             INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task CheckCompletedTripsAsync()
    {
        var completedTrips = _context.Trips
            .Where(t => t.EndDate < DateTime.Now && !t.IsCompleted)
            .ToList();

        foreach (var trip in completedTrips)
        {
            trip.IsCompleted = true;

            await _notificationService.SendTripCompletionNotifications(trip.Id);
        }

        await _context.SaveChangesAsync();
    }
}

```

```csharp
public class RecurringJobsInitializer
{
    private readonly IRecurringJobManager _recurringJobManager;

    public RecurringJobsInitializer(IRecurringJobManager recurringJobManager)
    {
        _recurringJobManager = recurringJobManager;
    }

    public void Initialize()
    {
        _recurringJobManager.AddOrUpdate(
            "CheckCompletedTripsJob",
            () => Console.WriteLine("Check trips..."),
            Cron.Daily); // time at 00:00
    }
}
```

```csharp
[HttpGet("user-notifications")]
public IActionResult GetUserNotifications(string userId)
{
    var notifications = _context.Notifications
        .Where(n => n.UserId == userId)
        .OrderByDescending(n => n.CreatedAt)
        .ToList();

    return Ok(notifications);
}
```


