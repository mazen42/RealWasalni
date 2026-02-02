using Wasalni_Models;

namespace Wasalni.Infrastructure.Specifications
{
    public class AppropriateDriverSpecification:Specification<DriverTripRequest>
    {
        public AppropriateDriverSpecification(string ToGovernerate,string FromGovernerate):base(x => x.FromGovernerate == FromGovernerate && x.ToGovernerate == ToGovernerate && x.RequestStatus == DriverTripRequestStatus.Requested)
        {
            OrderBy(x => x.CreatedAt);
        }
    }
}
