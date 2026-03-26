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

        public void checkTripsDates()
        {
            _recurringJobManager.AddOrUpdate(
    "checkTripsDates",
    () => _backGroundJobsServices.checkTripsDates(),
    Cron.Minutely()
);
        }

        public void DecrementPassengersLeftDaysInAllTripsJob()
        {
            _recurringJobManager.AddOrUpdate(
    "DecrementPassengersLeftDays",
    () => _backGroundJobsServices.DecrementPassengersLeftDaysInAllTripsAsync(),
    Cron.Minutely()
);
        }
        public async Task InvitationsRejection()
        {
        _recurringJobManager.AddOrUpdate(
        "invitation-expiry-job",
        () => _backGroundJobsServices.InvitesRejectionJob(),
        Cron.Minutely);

        }
    }
}
