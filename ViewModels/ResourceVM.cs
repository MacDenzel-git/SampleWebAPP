using Microsoft.Extensions.Configuration;
using PagedList;
using projectWebApplication.DTOs;
using projectWebApplication.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.ViewModels
{
    public class ResourceVM : BaseViewModel
    {
        public string BaseUrl
        {
            get
            {
                return _configuration["EndpointURL"];
            }
        }

        IConfiguration _configuration;
        public ResourceVM(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            
        }

        public PaginatedList<ResourceDTO> PaginatedResources { set; get; }
        public IEnumerable<ResourceDTO> Resources { set; get; }
        public ResourceDTO Resource { set; get; }
        //public IEnumerable<SermonCategoryVM> SermonCategories { get; set; }
        public IEnumerable<ResourceCategoryDTO> ResourceCategories { get; set; }
        public string PageHeader { get; set; }
        public OutputHandler OutputHandler {get;set;}
    }
}
