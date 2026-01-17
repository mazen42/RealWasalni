using Hangfire;

namespace Wasalni.Services
{
    public class RecurringJobsInitializer : IRecurringJobsInitializer
    {
        private readonly IBackgroundJobClient _JobClient;
        private readonly IRecurringJobManager _recurringJobManager;
        private IBackGroundJobsServices _backGroundJobsServices;
        public RecurringJobsInitializer(IRecurringJobManager recurringJobManager,IBackgroundJobClient backgroundJobClient, IBackGroundJobsServices backGroundJobsServices)
        {
            this._recurringJobManager = recurringJobManager;
            this._JobClient = backgroundJobClient;
            _backGroundJobsServices = backGroundJobsServices;
        }
        public void DecrementPassengersLeftDaysInAllTripsJob()
        {
            _recurringJobManager.AddOrUpdate(
    "DecrementPassengersLeftDays",
    () => _backGroundJobsServices.DecrementPassengersLeftDaysInAllTripsAsync(),
    Cron.Daily()
);
        }
    
        public void FillingBusTripsByDriversAndSendNotificationToAllPassengersJob()
        {
            _recurringJobManager.AddOrUpdate(
    "FillTripsAndNotifyPassengers",
    () => _backGroundJobsServices.FillingBusTripsByDriversAndSendNotificationToAllPassengers(),
    Cron.Minutely());
        }
    }
}
