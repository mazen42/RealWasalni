
using Microsoft.AspNetCore.SignalR;
using Wasalni.Infrastructure.Hubs;
using Wasalni.Infrastructure.Interfaces;
using Wasalni_Models;

namespace Wasalni.Services
{
    public class NotificationService : INotificationService
    {
        private IUnitOfWork _unitOfWork;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IUnitOfWork unitOfWork, IHubContext<NotificationHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }
        public async Task SendTripCompletionNotifications(int TripId,string message,string title)
        {
            var trip = await _unitOfWork.busTrip.Get(b => b.Id == TripId, includeProperties: "Passengers,DriverProfile");
            foreach (var passenger in trip.Passengers)
            {
                await _hubContext.Clients.User(passenger.ApplicationUserId).SendAsync("ReceiveNotification", new
                {
                    Title = title,
                    Message = message,
                    Date = DateTime.Now
                });
                Notification notificationuser = new Notification
                {
                    ApplicationUserId = passenger.ApplicationUserId,
                    CreatedAt = DateTime.UtcNow,
                    Message = message,
                    Title = title,
                };
                _unitOfWork.Notifications.Add(notificationuser);
                
            }
            await _hubContext.Clients.User(trip.DriverProfile!.ApplicationUserId).SendAsync("ReceiveNotification", new
            {
                Title = title,
                Message = message,
                Date = DateTime.Now
            });
            Notification notificationdriver = new Notification
            {
                ApplicationUserId = trip.DriverProfile!.ApplicationUserId,
                CreatedAt = DateTime.UtcNow,
                Message = message,
                Title = title,
            };
            _unitOfWork.Notifications.Add(notificationdriver);
            _unitOfWork.Save();
        }
        public async Task sendInvite(string email, int tripId, SeatChar seatChar,int invitationId)
        {
            var trip = await _unitOfWork.busTrip.Get(x => x.Id == tripId, includeProperties: "RoutePlan");
            var user = await _unitOfWork.applicationUser.Get(u => u.Email == email);
            if (user is not null)
            {
                var tripData = new
                {
                    vehicleType = trip.VehicleType,
                    tripId = trip.Id,
                    fromLocation = trip.FromGovernerate,
                    toLocation = trip.ToGovernerate,
                    arrivalTime = trip.ArrivalTime,
                    seatChar = seatChar,
                    invitationId = invitationId
                };
                await _hubContext.Clients.User(user.Id).SendAsync("ReciveTripInvite",tripData);
            }

        }

        public async Task SendNotificationAsync(string id,string title,string message)
        {
            await _hubContext.Clients.User(id).SendAsync(title,message);
        }
    }
}
