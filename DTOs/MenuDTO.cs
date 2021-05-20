using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.DTOs
{
    public class MenuDTO
    {
        public IEnumerable<MinistryArmDTO> MinistryArms { get; set; }
        public IEnumerable<ResourceTypeDTO> ResourceTypes { get; set; }
        public IEnumerable<BranchDTO> Branches { get; set; }
    }
}
