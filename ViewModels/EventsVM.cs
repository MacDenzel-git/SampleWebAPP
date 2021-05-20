using Microsoft.Extensions.Configuration;
using projectWebApplication.DTOs;
using projectWebApplication.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.ViewModels
{
    public class EventsVM : BaseViewModel
    {

        IConfiguration _configuration;
        public EventsVM(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public int Page { get; set; }
        public EventDTO EventDTO { get; set; }
        public IEnumerable<EventDTO> Events { get; set; }
        public PaginatedList<EventDTO> PaginatedEvents { get; set; }
    }
}
