using Wasalni_Models;

namespace Wasalni.Infrastructure.Specifications
{
    public class AppropriateDriverSpecification:Specification<DriverTripRequest>
    {
        public AppropriateDriverSpecification(string ToGovernerate,string FromGovernerate,VehicleType vehicle):base(x => x.FromGovernerate == FromGovernerate && x.ToGovernerate == ToGovernerate && x.RequestStatus == DriverTripRequestStatus.Requested && x.Driver.Bus.VehicleType == vehicle)
        {
            OrderBy(x => x.CreatedAt);
            includesExpressions.Add(x => x.Driver.Bus);
        }
    }
}
