using Wasalni_Models;

namespace Wasalni.Infrastructure.Specifications
{
    public class AppropriateDriverSpecification:Specification<DriverTripRequest>
    {
        public AppropriateDriverSpecification(string ToGovernerate,string FromGovernerate):base(x => x.FromGovernerate == FromGovernerate && x.ToGovernerate == ToGovernerate)
        {
            OrderBy(x => x.CreatedAt);
        }
    }
}
