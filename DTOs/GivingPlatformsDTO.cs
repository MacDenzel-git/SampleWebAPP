using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.DTOs
{
    public class GivingPlatformsDTO
    {
        public int GivingPlatformId { get; set; }
        public string Platform { get; set; }
        public string Number { get; set; }
        public int BranchId { get; set; }
        public int? Contact { get; set; }

    }
}
