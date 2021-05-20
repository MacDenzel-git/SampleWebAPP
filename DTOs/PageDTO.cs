using Microsoft.Extensions.Configuration;
using projectWebApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.DTOs
{
    public class PageDTO : BaseViewModel

    {

        IConfiguration _configuration;
        public PageDTO(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<ResourceDTO> Resources { get; set; }

    }
}
