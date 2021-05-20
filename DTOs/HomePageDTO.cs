using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.DTOs
{
    public class HomePageDTO
    {

        public IEnumerable<QouteDTO> Qoutes { get; set; }
        public IEnumerable<MinistryArmDTO> MinistryArms { get; set; }
        public IEnumerable<TeamMembersDTO> TeamMembers { get; set; }
        public EventDTO TimerActivatedEvent { get; set; }

        public IEnumerable<ResourceDTO> Resources { get; set; }
    }
}
