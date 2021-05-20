using projectWebApplication.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectWebApplication.DTOs
{
    public class TestimonyDTO
    {
        public int TestimonyId { get; set; }
        public string TestifierName { get; set; }
        public string TestimonyHeading { get; set; }
        public string TestimonyFullDescription { get; set; }
        public bool IsPublished { get; set; }
        public DateTime TestimonyDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public byte[] ImgBytes { get; set; }
        public string FileName { get; set; }
        public string ImageUrl { get; set; }
        public string StorageSize { get; set; }
        public OutputHandler OutputHandler { get; set; }


    }
}
