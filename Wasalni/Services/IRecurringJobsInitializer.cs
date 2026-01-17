namespace Wasalni.Services
{
    public interface IRecurringJobsInitializer
    {
        void DecrementPassengersLeftDaysInAllTripsJob();
        void FillingBusTripsByDriversAndSendNotificationToAllPassengersJob();
    }
}
