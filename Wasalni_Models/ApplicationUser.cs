using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Wasalni_Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public int Age { get; set; }
        [Required]
        public Location HomeLocation { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Required]
        public UserType UserType { get; set; }
        public List<Notification> Notifications { get; set; } 
    }
}
