using projectWebApplication.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.DTOs
{
    public class TeamMembersDTO
    {
        public long TeamMemberId { get; set; }
        public int MinistryArmId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public int PositionId { get; set; }
        public int RoleId { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsPublished { get; set; }
        public bool IsTopLeadership { get; set; }
        public bool IsDirector { get; set; }
        public bool IsECMember { get; set; }
        public bool IsStaffMember { get; set; }
        public bool IsBranchPastor { get; set; }
        public byte[] Artwork { get; set; }
        public string ImageUrl { get; set; }
        public string Filename { get; set; }
        public string OldImageUrl { get; set; }
        public string CurrentImageName { get; set; }
        public string StorageSize { get; set; }

        public IEnumerable<BranchDTO> Branches {get;set;}
        public IEnumerable<PositionDTO> Positions {get;set;}
        public IEnumerable<MinistryArmDTO> MinistryArms {get;set;}
        public OutputHandler OutputHandler {get;set;}

    }
}
