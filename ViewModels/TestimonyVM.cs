using Microsoft.Extensions.Configuration;
using projectWebApplication.DTOs;
using projectWebApplication.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.ViewModels
{
    public class TestimonyVM : BaseViewModel
    {

        IConfiguration _configuration;
        public TestimonyVM(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public TestimonyDTO Testimony { get; set; }
        public IEnumerable<TestimonyDTO> Testimonies { get; set; }
        public OutputHandler OutputHandler { get;  set; }
    }
}
