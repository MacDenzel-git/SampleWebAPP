using Microsoft.Extensions.Configuration;
using projectWebApplication.General;
using projectWebApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.DTOs
{
    public class MinistryArmDTO

    { 

        public int MinistryArmId { get; set; }

        [DisplayName("Ministry Arm Name/Title")]
        public string MinistryArmName { get; set; }
        [DisplayName("Abbreviation")]
        public string MinistryArmAbbreviation { get; set; }
        public string Highlight { get; set; }
        public string Mission { get; set; }
        public string Vision { get; set; }
        [DisplayName("Main Objective")]
        public string MainObjective { get; set; } //cut this for highlight on homepage 
        public string Artwork { get; set; }
        [DisplayName("Footer Main Pragraph")]
        public string FooterSection { get; set; }
        [DisplayName("Footer Final Paragraph")]
        public string FinalSection { get; set; }
        [DisplayName("Published")]
        public bool IsPublished { get; set; }
        [DisplayName("Featured On Home Page")]
        public bool IsFeaturedOnHomePage { get; set; }
        [DisplayName("Should this be added to the Ministry Arm Menu - Directorates should not be shown on the ")]
        public bool IsAddedToMenu { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public byte[] ImgBytes { get; set; }
        public string Filename { get; set; }
        public string OldImageUrl { get; set; }
        public string CurrentImageName { get; set; }
        public string MinistryFullDescription { get; set; }
        public string StorageSize { get; set; }
        public OutputHandler OutputHandler { get; set; }

    }
}
