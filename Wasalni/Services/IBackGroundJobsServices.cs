namespace Wasalni.Services
{
    public interface IBackGroundJobsServices
    {
        Task DecrementPassengersLeftDaysInAllTripsAsync();
        //void FillingBusTripsByDriversAndSendNotificationToAllPassengers();
        void checkTripsDates();
    }
}
