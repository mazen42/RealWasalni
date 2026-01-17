using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Wasalni_Models
{
    public enum UserType
    {
        Passenger = 0,
        Driver = 1,
        Admin = 2
    }
}
