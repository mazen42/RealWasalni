using Dapper;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Wasalni.Infrastructure.Interfaces;
using Wasalni.Infrastructure.Specifications;
using Wasalni_DataAccess.Data;
using Wasalni_Models;

namespace Wasalni.Services
{
    public class BackGroundJobService : IBackGroundJobsServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<BackGroundJobService> _logger;

        public BackGroundJobService(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            AppDbContext appDbContext,
            ILogger<BackGroundJobService> logger)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _appDbContext = appDbContext;
            _logger = logger;
        }       

        public async Task DecrementPassengersLeftDaysInAllTripsAsync()
        {
            try
            {
                var updated = await _appDbContext.Set<Passenger>()
                    .Include(x => x.BusTrip)
                    .Where(x => x.BusTripId != null && x.DaysLeft > 0 && x.BusTrip.TripStatus == TripStatus.INWork)
                    .ExecuteUpdateAsync(s => s.SetProperty(x => x.DaysLeft, x => x.DaysLeft - 1));
                
                _logger.LogInformation("Decremented DaysLeft for {Count} passengers", updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error decrementing passenger days");
            }
        }

        public async Task checkTripsDates()
        {
            try
            {
                var updated = await _appDbContext.Set<BusTrip>()
                    .Where(x => x.endDate != null && x.endDate <= DateOnly.FromDateTime(DateTime.Now))
                    .ExecuteUpdateAsync(x => x.SetProperty(x => x.TripStatus, TripStatus.Finished));
                
                _logger.LogInformation("Marked {Count} trips as finished", updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking trip dates");
            }
        }

        /// <summary>
        /// Rejects expired invitations (older than 1 minute) and removes reserved seats.
        /// </summary>
        public async Task InvitesRejectionJob()
        {
            try
            {
                var now = TimeOnly.FromDateTime(DateTime.Now);

                // Get all pending invitations that have expired (ExpiresAt passed current time)
                var expiredInvitations = _unitOfWork.invitation.GetAll(x => x.Status == InvitationStatus.Pending && x.ExpiresAt <= now);

                if (expiredInvitations.Count() == 0)
                {
                    _logger.LogDebug("No expired invitations to reject");
                    return;
                }

                _logger.LogInformation("Found {Count} expired invitations to reject", expiredInvitations.Count());

                foreach (var invitation in expiredInvitations)
                {
                    try
                    {
                        // Mark invitation as rejected
                        invitation.Status = InvitationStatus.Rejected;
                        _unitOfWork.invitation.Update(invitation);
                        _unitOfWork.Save();

                        // Remove the reserved seat
                        var seat = await _unitOfWork.seats.Get(x => x.SeatChar == invitation.seatChar && x.BusTripId == invitation.BusTripId && x.PassengerId == null);

                        if (seat != null)
                        {
                            _unitOfWork.seats.Remove(seat);
                            _logger.LogInformation("Removed reserved seat {SeatChar} from trip {TripId}", invitation.seatChar, invitation.BusTripId);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing expired invitation {InvitationId}", invitation.Id);
                    }
                }

                _unitOfWork.Save();
                _logger.LogInformation("Successfully rejected {Count} expired invitations", expiredInvitations.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error in InvitesRejectionJob");
            }
        }

        /// <summary>
        /// Rejects a specific invitation by ID (called when user rejects).
        /// </summary>
        public async Task RejectInvitationAsync(int invitationId)
        {
            try
            {
                var invitation = await _unitOfWork.invitation.Get(x => x.Id == invitationId && x.Status == InvitationStatus.Pending);

                if (invitation == null)
                {
                    _logger.LogWarning("Invitation {InvitationId} not found or already processed", invitationId);
                    return;
                }

                // Mark as rejected
                invitation.Status = InvitationStatus.Rejected;

                // Remove the reserved seat
                var seat = await _unitOfWork.seats.Get(x => x.SeatChar == invitation.seatChar 
                        && x.BusTripId == invitation.BusTripId 
                        && x.Passenger == null);

                if (seat != null)
                {
                    _unitOfWork.seats.Remove(seat);
                }

                _unitOfWork.Save();
                
                // Send notification to passenger
                await _notificationService.SendNotificationAsync(
                    invitation.ReceiverId,
                    "Invitation Rejected",
                    $"Your invitation for seat {invitation.seatChar} has been rejected.");

                _logger.LogInformation("Invitation {InvitationId} rejected by user", invitationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting invitation {InvitationId}", invitationId);
            }
        }

        /// <summary>
        /// Accepts an invitation, assigns passenger to seat and trip.
        /// </summary>
        public async Task AcceptInvitationAsync(int invitationId)
        {
            try
            {
                var invitation = await _unitOfWork.invitation.Get(x => x.Id == invitationId && x.Status == InvitationStatus.Pending,includeProperties:"BusTrip");

                if (invitation == null)
                {
                    _logger.LogWarning("Invitation {InvitationId} not found or already processed", invitationId);
                    return;
                }

                // Mark invitation as accepted
                invitation.Status = InvitationStatus.Accepted;

                // Get or create passenger
                var passenger = await _unitOfWork.passenger.Get(x => x.ApplicationUserId == invitation.ReceiverId && invitation.BusTripId == x.BusTripId);

                if (passenger == null)
                {
                    passenger = new Passenger
                    {
                        ApplicationUserId = invitation.ReceiverId,
                        BusTripId = invitation.BusTripId,
                        TripType = invitation.BusTrip!.TripType,
                        ArrivalTime = invitation.BusTrip.ArrivalTime,
                        DaysLeft = 30, // or calculate based on trip duration
                    };
                    _unitOfWork.passenger.Add(passenger);
                    _unitOfWork.Save();
                }
                else
                {
                    // Update existing passenger
                    passenger.BusTripId = invitation.BusTripId;
                }

                // Assign seat to passenger
                var seat = await _unitOfWork.seats.Get(x => x.SeatChar == invitation.seatChar 
                        && x.BusTripId == invitation.BusTripId);

                if (seat != null)
                {
                    seat.PassengerId = passenger.Id;
                    seat.SeatStatus = SeatStatus.Booked;
                }

                _unitOfWork.Save();;

                // Send notification
                await _notificationService.SendNotificationAsync(
                    invitation.ReceiverId,
                    "Invitation Accepted",
                    $"You have been confirmed for seat {invitation.seatChar} on the trip.");

                _logger.LogInformation("Invitation {InvitationId} accepted. Passenger {PassengerId} assigned to seat {SeatChar}", 
                    invitationId, passenger.Id, invitation.seatChar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting invitation {InvitationId}", invitationId);
            }
        }

    }
}