namespace Wasalni.Services
{
    public interface INotificationService
    {
        public Task SendTripCompletionNotifications(int TripId,string message, string title);
    }
}
