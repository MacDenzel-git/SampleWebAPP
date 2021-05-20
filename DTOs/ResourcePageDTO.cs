using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.DTOs
{
    public class ResourcePageDTO
    {
        public IEnumerable<ResourceDTO> Resources { get; set; }
        public IEnumerable<ResourceCategoryDTO> ResourceCategories { get; set; }
    }
}
