namespace Wasalni.Services
{
    public interface IRecurringJobsInitializer
    {
        void DecrementPassengersLeftDaysInAllTripsJob();
        Task InvitationsRejection();
        void checkTripsDates();
    }
}
