using Wasalni_Models;

namespace Wasalni.Services
{
    public interface INotificationService
    {
        public Task SendTripCompletionNotifications(int TripId,string message, string title);
        public Task sendInvite(string email, int tripId,SeatChar seatChar,int invitationId);
Task SendNotificationAsync(string id, string title, string message);
    }
}
