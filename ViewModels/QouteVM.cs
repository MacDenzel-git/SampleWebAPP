using Microsoft.Extensions.Configuration;
using projectWebApplication.DTOs;
using projectWebApplication.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.ViewModels
{
    public class QouteVM: BaseViewModel
    {

        IConfiguration _configuration;
        public QouteVM(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<QouteDTO> QouteDTO { get; set; }
        public OutputHandler OutputHandler { get; set; }
        public PaginatedList<QouteDTO> PaginatedQoutes { set; get; }
        public IEnumerable<IEnumerable<QouteDTO>> List { get; set; }
    }
}
