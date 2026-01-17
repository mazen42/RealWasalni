using FluentValidation;
using Wasalni_Models.DTOs;

namespace Wasalni.Fluent_Validators
{
    public sealed class LocationDTOValidator : AbstractValidator<LocationDTO>
    {
        public LocationDTOValidator()
        {
            RuleFor(x => x.Longitude).NotEmpty().WithMessage("longitude is required");
            RuleFor(x => x.Latitude).NotEmpty().WithMessage("latitude is required");
        }
    }
}
