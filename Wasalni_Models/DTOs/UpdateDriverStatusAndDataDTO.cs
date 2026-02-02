using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalni_Models.DTOs
{
    public class UpdateDriverStatusAndDataDTO
    {
        public int id {  get; set; }
        public DriverApprovalStatus DriverApprovalStatus { get; set; }

    }
}
