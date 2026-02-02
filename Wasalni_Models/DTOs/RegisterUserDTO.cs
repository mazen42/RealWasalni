using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalni_Models.DTOs
{
    public class RegisterUserDTO : IValidatableObject
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public Location? HomeLocation { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Required]
        public UserType UserType { get; set; }
        [Required]
        public int Age { get; set; }
        public IFormFile? LicenseImageFace { get; set; }
        public IFormFile? IdCardFace { get; set; }
        public IFormFile? LicenseImageBack { get; set; }
        public IFormFile? IdCardBack { get; set; }
        public VehicleType VehicleType { get; set; }
        public string? PlateNumber { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UserType == UserType.Driver)
            {
                if (LicenseImageBack == null)
                    yield return new ValidationResult("License back image is required", new[] { nameof(LicenseImageBack) });
                if (LicenseImageFace == null)
                    yield return new ValidationResult("License face image is required", new[] { nameof(LicenseImageFace) });
                if (IdCardBack == null)
                    yield return new ValidationResult("ID card back image is required", new[] { nameof(IdCardBack) });
                if (IdCardFace == null)
                    yield return new ValidationResult("ID card face image is required", new[] { nameof(IdCardFace) });
                if (string.IsNullOrEmpty(PlateNumber))
                    yield return new ValidationResult("Plate number is required", new[] { nameof(PlateNumber) });
                if (!string.IsNullOrEmpty(PlateNumber) && PlateNumber.Length > 7)
                    yield return new ValidationResult("Plate number must be between 1 and 7", new[] { nameof(PlateNumber) });
            }

        }
    }
}