using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.DTOs
{
    public class PopulateDropdownDTO //This class was created to be used to populate this dropdowns to avoid double API request
    {
        public IEnumerable<ResourceCategoryDTO> ResourceCategories { get; set; }
        public IEnumerable<ResourceTypeDTO> ResourceTypes { get; set; }
    }
}
