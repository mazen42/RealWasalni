using Newtonsoft.Json.Bson;

namespace Wasalni.Services
{
    public interface IBackGroundJobsServices
    {
        Task DecrementPassengersLeftDaysInAllTripsAsync();
        Task checkTripsDates();
        Task InvitesRejectionJob();
        Task RejectInvitationAsync(int invitationId);
        Task AcceptInvitationAsync(int invitationId);
    }
}
