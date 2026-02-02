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
            await _appDbContext.Set<Passenger>().Where(x => x.BusTripId != null && x.DaysLeft > 0).ExecuteUpdateAsync(s => s.SetProperty(x => x.DaysLeft,x => x.DaysLeft - 1));
            //await _unitOfWork._db.Passengers.Where(s => s.BusTripId != null).ExecuteUpdateAsync(e => e.SetProperty(e => e.DaysLeft, e => e.DaysLeft - 1));
        }
        public void FillingBusTripsByDriversAndSendNotificationToAllPassengers()
        {
            //var TripRequestTableCheck = _unitOfWork.RideRequest.GetAll(r => r.ArrivalTime == obj.arrivalTime && r.TripType == obj.TripType && r.VehicleType == obj.VehicleType && r.FromGovernorate == responseFrom && r.ToGovernorate == responseTo);
            var readyTrip = _unitOfWork.RideRequest.GetAll(includeProperties: "passengers",tracked:false);
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
                        AppropriateDriverGetter.RequestStatus = DriverTripRequestStatus.Tripped;
                        trip.IsDone = true;
                        BusTrip newBusTrip = new BusTrip
                        {
                            CreatedAt = DateTime.Now,
                            DriverProfileId = AppropriateDriverGetter.DriverId,
                            StartDate = DateOnly.FromDateTime(DateTime.Now),
                            endDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(1)),
                            TripStatus = TripStatus.INWork,
                            FromGovernerate = trip.FromGovernorate,
                            ToGovernerate = trip.ToGovernorate,
                            NotificationSent = true,
                        };
                        _unitOfWork.busTrip.Add(newBusTrip);
                        _unitOfWork.Save();
                        foreach(var pass in trip.passengers)
                        {
                            _unitOfWork._db.Passengers.Attach(pass);
                            pass.RideRequestId = null;
                            pass.BusTripId = newBusTrip.Id;
                        }
                        RoutePlan routePlan = new RoutePlan
                        {
                            BusTripId = newBusTrip.Id,
                            CreatedAt = DateTime.Now,
                        };
                        _unitOfWork.routePlan.Add(routePlan);
                        _unitOfWork.Save();
                        //The Message Will Be The Details of the Route Plan After Ai Send It
                        _notificationService.SendTripCompletionNotifications(newBusTrip.Id, "Your Trip In Work Know", "Your Trip Is Done Don't Miss It!");
                    }
                }
            }
        }
    }
}