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
            await _appDbContext.Set<Passenger>().Include(x => x.BusTrip).Where(x => x.BusTripId != null && x.DaysLeft > 0 && x.BusTrip.TripStatus == TripStatus.INWork).ExecuteUpdateAsync(s => s.SetProperty(x => x.DaysLeft,x => x.DaysLeft - 1));
            //await _unitOfWork._db.Passengers.Where(s => s.BusTripId != null).ExecuteUpdateAsync(e => e.SetProperty(e => e.DaysLeft, e => e.DaysLeft - 1));
        }
        //public void FillingBusTripsByDriversAndSendNotificationToAllPassengers()
        //{
        //    var baseDate = DateTime.Today;
        //    var readyTrip = _unitOfWork.busTrip.GetAll(x => x.DriverProfileId == null && x.TripStatus == TripStatus.Pending, includeProperties: "Passengers",tracked:true);

        //    if (readyTrip is not null)
        //    {
        //        foreach (var trip in readyTrip)
        //        {
        //            var arrivalDateTime = baseDate.Add(trip.ArrivalTime.ToTimeSpan());
        //            var arrivalPlusHour = arrivalDateTime.AddHours(1);
        //            var AppropriateDrivers = SpecificationEvaluator.applySpecification(
        //                _unitOfWork._db.DriverTripRequests,
        //                new AppropriateDriverSpecification(trip.ToGovernerate, trip.FromGovernerate,trip.VehicleType)
        //            ).AsEnumerable().ToList();
        //            var AppropriateDriverGetter = AppropriateDrivers.FirstOrDefault(d =>
        //            {
        //                var start = baseDate.Add(d.StartTime.ToTimeSpan());
        //                var end = baseDate.Add(d.EndTime.ToTimeSpan());
        //                if (end < start)
        //                    end = end.AddDays(1);
        //                return start <= arrivalPlusHour && end >= arrivalPlusHour.AddHours(-1);
        //            });
        //            if (AppropriateDriverGetter != null)
        //            {
        //                AppropriateDriverGetter.RequestStatus = DriverTripRequestStatus.Tripped;
        //                trip.TripStatus = TripStatus.INWork;
        //                trip.DriverProfileId = AppropriateDriverGetter.DriverId;
        //                trip.StartDate = DateOnly.FromDateTime(DateTime.UtcNow);
        //                trip.endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));
        //                _unitOfWork.Save();
        //                RoutePlan routePlan = new RoutePlan
        //                {
        //                    BusTripId = trip.Id,
        //                    CreatedAt = DateTime.Now,
        //                };
        //                _unitOfWork.routePlan.Add(routePlan);
        //                _unitOfWork.Save();
        //                _notificationService.SendTripCompletionNotifications(trip.Id, "Your Trip In Work Know", "Your Trip Is Done Don't Miss It!");
        //            }
        //        }
        //    }
        //}
        public void checkTripsDates()
        {
            _appDbContext.Set<BusTrip>().Where(x => x.endDate != null && x.endDate >= x.StartDate).ExecuteUpdateAsync(x => x.SetProperty(x => x.TripStatus, TripStatus.Finished));
        }
    }
}