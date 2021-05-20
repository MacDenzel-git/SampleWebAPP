using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.DTOs
{
    public class BranchDTO
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public long DistrictId { get; set; }
        public DateTime? DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string GoogleMapInfo { get; set; }
        public string LocationDescription { get; set; }
        public bool IsCell { get; set; }
        public bool IsPublished { get; set; }
        public string BranchPastorName { get; set; }
        public string Phone { get; set; }
        public string ImageUrl { get; set; }
        public byte[] ImgBytes { get; set; }
        public IEnumerable<GivingPlatformsDTO> GivingPlatforms { get; set; }
    }
}
