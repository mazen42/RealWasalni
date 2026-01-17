using FluentValidation;
using Wasalni_Models.DTOs;

namespace Wasalni.Fluent_Validators
{
    public sealed class TripRequestDTOValidator : AbstractValidator<TripRequestDTO>
    {
        public TripRequestDTOValidator()
        {
            RuleFor(x => x.arrivalTime).NotEmpty().WithMessage("arrival time is required");
            RuleFor(x => x.VehicleType).NotEmpty().WithMessage("vechile type is required");
            RuleFor(x => x.TripType).NotEmpty().WithMessage("trip type is required");
            RuleFor(x => x.FromLocation).NotNull().WithMessage("from location is is required").SetValidator(new LocationDTOValidator());
            RuleFor(x => x.ToLocation).NotNull().WithMessage("to location is is required").SetValidator(new LocationDTOValidator());

        }
    }
}
