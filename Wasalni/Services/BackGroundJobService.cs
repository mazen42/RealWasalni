using Dapper;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Wasalni.Infrastructure.Interfaces;
using Wasalni.Infrastructure.Specifications;
using Wasalni_DataAccess.Data;
using Wasalni_Models;

namespace Wasalni.Services
{
    public class BackGroundJobService : IBackGroundJobsServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private INotificationService _notificationService;
        private readonly AppDbContext _appDbContext;

        public BackGroundJobService(IUnitOfWork unitOfWork, INotificationService notificationService, AppDbContext appDbContext)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            this._appDbContext = appDbContext;
        }

        public async Task DecrementPassengersLeftDaysInAllTripsAsync()
        {
            await _unitOfWork._db.Database.GetDbConnection().ExecuteAsync("Update Passengers set DaysLeft = DaysLeft - 1 where DaysLeft > 0 And BusTripId IS NOT NULL");
            //await _unitOfWork._db.Passengers.Where(s => s.BusTripId != null).ExecuteUpdateAsync(e => e.SetProperty(e => e.DaysLeft, e => e.DaysLeft - 1));
        }
        public void FillingBusTripsByDriversAndSendNotificationToAllPassengers()
        {
            //var TripRequestTableCheck = _unitOfWork.RideRequest.GetAll(r => r.ArrivalTime == obj.arrivalTime && r.TripType == obj.TripType && r.VehicleType == obj.VehicleType && r.FromGovernorate == responseFrom && r.ToGovernorate == responseTo);
            var readyTrip = _unitOfWork.RideRequest.GetAll(includeProperties: "passengers");
            var baseDate = DateTime.Today;

            if (readyTrip is not null)
            {
                foreach (var trip in readyTrip)
                {
                    var arrivalDateTime = baseDate.Add(trip.ArrivalTime.ToTimeSpan());
                    var arrivalPlusHour = arrivalDateTime.AddHours(1);
                    var AppropriateDrivers = SpecificationEvaluator.applySpecification(
                        _unitOfWork._db.DriverTripRequests,
                        new AppropriateDriverSpecification(trip.ToGovernorate, trip.FromGovernorate)
                    ).AsEnumerable().ToList();
                    var AppropriateDriverGetter = AppropriateDrivers.FirstOrDefault(d =>
                    {
                        var start = baseDate.Add(d.StartTime.ToTimeSpan());
                        var end = baseDate.Add(d.EndTime.ToTimeSpan());
                        if (end < start)
                            end = end.AddDays(1);
                        return start <= arrivalPlusHour && end >= arrivalPlusHour.AddHours(-1);
                    });
                    if (AppropriateDriverGetter != null) {
                        BusTrip newBusTrip = new BusTrip
                        {
                            Passengers = trip.passengers,
                            CreatedAt = DateTime.Now,
                            DriverProfileId = AppropriateDriverGetter.DriverId,
                            StartDate = DateOnly.FromDateTime(DateTime.Now),
                            FromGovernerate = trip.FromGovernorate,
                            ToGovernerate = trip.ToGovernorate,
                            IsConfirmed = true,
                            IsOptimized = true,
                            NotificationSent = true,
                        };
                        _unitOfWork.busTrip.Add(newBusTrip);
                        _unitOfWork.Save();
                        RoutePlan routePlan = new RoutePlan
                        {
                            BusTripId = newBusTrip.Id,
                            CreatedAt = DateTime.Now,
                        };
                        _unitOfWork.routePlan.Add(routePlan);
                        _unitOfWork.RideRequest.Remove(trip);
                        _unitOfWork.driverTripRequest.Remove(AppropriateDriverGetter);
                        _unitOfWork.Save();
                        //The Message Will Be The Details of the Route Plan After Ai Send It
                        _notificationService.SendTripCompletionNotifications(newBusTrip.Id, "Your Trip In Work Know", "Your Trip Is Done Don't Miss It!");
                    }
                }
            }
        }
    }
}